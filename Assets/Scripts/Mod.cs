using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace LOI
{
    public abstract class Mod 
    {
        public string Author => this.ModSource.Author;
        public string KeyName => this.ModSource.KeyName;
        public string ModName => this.ModSource.ModName;
        public string Version => this.ModSource.Version;
        public string DisplayName { get; protected set; }
        public string Description { get; protected set; }
        public bool IsEnabled { get; private set; }
        public ModRegister ModSource { get; }
        public Mod[] ModAPI { get; }
        public Mod[] ModBan { get; }
        public static Dictionary<ModRegister, Mod> ModInstance { get; } = new Dictionary<ModRegister, Mod>();

        protected Mod()
        {
            Type type = this.GetType();
            if (!type.IsSealed) throw new InvalidOperationException($"The Class({type.Name}) must be sealed when inheriting from Class({nameof(Mod)})");
            object[] attrs = type.GetCustomAttributes(typeof(ModRegister), false);
            if (attrs == null || attrs.Length == 0)
                throw new InvalidOperationException($"The Class({type.Name}) must have attribute({nameof(ModRegister)}) inherit from Class({nameof(Mod)}).");
            if(!Mod.ModInstance.TryAdd(this.ModSource = attrs[0] as ModRegister, this))
                throw new InvalidOperationException($"The mod({type.Name}) has existed or loaded.", new ArgumentException("Duplicate ModInfo: " + this.ModSource, nameof(this.ModSource)));
            this.DisplayName = this.ModName;
            this.Description = "None";
            this.ModAPI = null;
            this.ModBan = null;
        }

        protected internal virtual void OnLoad() { }
        protected internal virtual void OnUnload() { }
        protected internal virtual void OnEnable() { }
        protected internal virtual void OnDisable() { }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public class ModRegister : Attribute, IEqualityComparer<ModRegister>
        {
            internal string Version { get; }
            internal string Author { get; }
            internal string KeyName { get; }
            internal string ModName { get; }

            public ModRegister(string name, string key, string ver, string author)
            {
                List<string> vals = new List<string>();
                if (string.IsNullOrEmpty(this.Version = ver)) vals.Add(nameof(ver));
                if (string.IsNullOrEmpty(this.Author = author)) vals.Add(nameof(author));
                if (string.IsNullOrEmpty(this.KeyName = key)) vals.Add(nameof(key));
                if (string.IsNullOrEmpty(this.ModName = name)) vals.Add(nameof(name));
                if (vals.Count > 0) throw new ArgumentNullException(string.Join(", ", vals), "Cannot be set to null or empty.");
            }

            public bool Equals(ModRegister x, ModRegister y)
            {
                if (object.ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;

                return //string.Equals(x.Version, y.Version, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(x.Author, y.Author, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(x.KeyName, y.KeyName, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(x.ModName, y.ModName, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode([DisallowNull] ModRegister obj)
            {
                if (obj is null) return 0;

                HashCode hashCode = new HashCode();
                //hashCode.Add(obj.Version, StringComparer.OrdinalIgnoreCase);
                hashCode.Add(obj.Author, StringComparer.OrdinalIgnoreCase);
                hashCode.Add(obj.KeyName, StringComparer.OrdinalIgnoreCase);
                hashCode.Add(obj.ModName, StringComparer.OrdinalIgnoreCase);
                return hashCode.ToHashCode();
            }

            public override bool Equals(object obj) => obj is ModRegister other ? this.Equals(this, other) : false;

            public override int GetHashCode() => this.GetHashCode(this);

            public override string ToString() => $"{this.ModName}(v{this.Version}-{this.KeyName}) by {this.Author}";
        }
    }
}
