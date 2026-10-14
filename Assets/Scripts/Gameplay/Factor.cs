using Godot;
using LOI.DataStructures;
using LOI.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace LOI.Gameplay
{
    public abstract class Factor : IEquatable<Factor>, IEqualityComparer<Factor>
    {
        public double Value => this.GetValue();
        public string Key { get; } 
        public string Name { get; set; }
        public Color Positive { get; }
        public Color Negative { get; }

        public event Action<double> OnChanged;
        private Func<object, double> getter1;
        private Func<double> getter2;
        private object metadata;
        private double val;
        private bool valLock;

        public Factor(string name, double value, Color positive, Color negative)
        {
            this.Key = $"{this.GetType().AssemblyQualifiedName}:{name}";
            this.Name = name;
            this.val = value;
            this.metadata = null;
            this.getter1 = null;
            this.getter2 = null;
            this.valLock = false;
        }

        public Factor(string name, Func<double> getter, Color positive, Color negative)
        {
            this.Key = $"{this.GetType().AssemblyQualifiedName}:{name}";
            this.Name = name;
            this.val = getter.Invoke();
            this.metadata = null;
            this.getter1 = null;
            this.getter2 = getter;
            this.valLock = false;
        }

        public Factor(string name, object metadata, Func<object, double> getter, Color positive, Color negative)
        {
            this.Key = $"{this.GetType().AssemblyQualifiedName}:{name}";
            this.Name = name;
            this.val = getter.Invoke(metadata);
            this.metadata = metadata;
            this.getter1 = getter;
            this.getter2 = null;
            this.valLock = false;
        }

        private double GetValue()
        {
            if (this.valLock) return this.val;
            this.valLock = true;
            double val = this.val;
            if (this.getter1.NotNull()) val = this.getter1.Invoke(this.metadata);
            else if (this.getter2.NotNull()) val = this.getter2.Invoke();
            if (this.val != val)
            {
                this.val = val;
                this.OnChanged?.Invoke(val);
            }
            this.valLock = false;
            return this.val;
        }

        public abstract void Apply(double basicValue, ref double currValue);
        public bool Equals(Factor other) => this.Key == other.Key;
        public bool Equals(Factor x, Factor y) => x.Key == y.Key;
        public int GetHashCode([DisallowNull] Factor obj) => obj.Key.GetHashCode();
        public override string ToString() => $"{this.Key}={this.Value}";
        public abstract string GetDisplay(string source);
    }

    public class DependentFactor : Factor
    {
        private static readonly Color positive = new Color(0f, 1f, 0f, 1f);
        private static readonly Color negative = new Color(1f, 0f, 0f, 1f);

        public bool IsPercent { get; set; }

        public DependentFactor(string key, double value, bool isPercent = false) : base(key, value, DependentFactor.positive, DependentFactor.negative) => this.IsPercent = isPercent;
        public DependentFactor(string key, Func<double> getter, bool isPercent = false) : base(key, getter, DependentFactor.positive, DependentFactor.negative) => this.IsPercent = isPercent;
        public DependentFactor(string key, object info, Func<object, double> getter, bool isPercent = false) : base(key, info, getter, DependentFactor.positive, DependentFactor.negative) => this.IsPercent = isPercent;
        public override void Apply(double basicValue, ref double currValue) => currValue += this.IsPercent.IfElse(basicValue * this.Value, this.Value);
        public override string GetDisplay(string source) => this.Value.IfElse(0d, null, $"[{this.Name}] - {source} {(this.Value > 0).IfElse("increased", "decreased")} by {this.Value:0.#}");
    }

    public class IndependentFactor : Factor
    {
        private static readonly Color positive = new Color(0f, .5f, 1f, 1f);
        private static readonly Color negative = new Color(1f, .5f, 0f, 1f);

        public IndependentFactor(string key, double value) : base(key, value, IndependentFactor.positive, IndependentFactor.negative) { }
        public IndependentFactor(string key, Func<double> getter) : base(key, getter, IndependentFactor.positive, IndependentFactor.negative) { }
        public IndependentFactor(string key, object info, Func<object, double> getter) : base(key, info, getter, IndependentFactor.positive, IndependentFactor.negative) { }
        public override void Apply(double basicValue, ref double currValue) => currValue *= (this.Value + 1d).Max(0d);
        public override string GetDisplay(string source) => this.Value.IfElse(0d, null, $"[{this.Name}] - Gains {this.Value:0.#} {(this.Value > 0).IfElse("more", "less")} {source}");
    }

    public class Factor<T> : IEquatable<Factor<T>>, IEqualityComparer<Factor<T>> where T : INumber<T>, IMinMaxValue<T>
    {
        public UniqueToken Key { get; }
        public Color Positive { get; set; }
        public Color Negative { get; set; }
        public string Name { get; set; }

        public T Adder { get => field; set { if (field != value) { field = value; this.ValueChanged?.Invoke(value, this.Multiplier); } } } = T.Zero;
        public double Multiplier { get => field; set { field = value; this.ValueChanged?.Invoke(this.Adder, value); } } = 1d;
        public event Action<T, double> ValueChanged;
        public event Func<string, string> ModifiedDisplay;

        public Factor(string name, Color positive, Color negative)
        {
            this.Key = new UniqueToken($"{this.GetType().FullName}.{name}", '.');
            this.Name = name;
        }

        public bool Equals(Factor<T> other) => this.Key == other.Key;
        public bool Equals(Factor<T> x, Factor<T> y) => x.Key == y.Key;
        public int GetHashCode([DisallowNull] Factor<T> obj) => obj.Key.GetHashCode();
        public string GetDisplay(string source)
        {
            if (this.ModifiedDisplay.NotNull()) return this.ModifiedDisplay.Invoke(source);
            return source;
        }
    }
}
