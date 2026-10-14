using LOI.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LOI.DataStructures
{
    public abstract class DataReference
    {
        public int ID { get; }
        public string Name { get; }
        public object Value { get; }
        public V GetValue<V>() => (V)this.Value;
        public abstract Type TypeValue { get; }

        public DataReference(int id, string name, object value = null)
        {
            this.ID = id;
            this.Name = name;
            this.Value = value;
        }

        public static implicit operator int(DataReference @ref) => @ref.ID;
        public static implicit operator string(DataReference @ref) => @ref.Name;

        public static bool operator ==(DataReference ref1, DataReference ref2) => ref1.TypeValue == ref2.TypeValue && ref1.ID == ref2.ID && ref1.Name == ref2.Name;
        public static bool operator !=(DataReference ref1, DataReference ref2) => ref1.TypeValue != ref2.TypeValue || ref1.ID != ref2.ID || ref1.Name != ref2.Name;
        public override bool Equals(object obj) => obj is DataReference @ref ? this == @ref : false;
        public override int GetHashCode() => HashCode.Combine<int, string>(this.ID, this.Name);
    }

    public sealed class DataReference<T> : DataReference
    {
        private static int GenID = 0;
        private static readonly Dictionary<int, DataReference<T>> Ref1 = new Dictionary<int, DataReference<T>>();
        private static readonly Dictionary<string, DataReference<T>> Ref2 = new Dictionary<string, DataReference<T>>();
        private static readonly Type Type = typeof(T);

        public override Type TypeValue => DataReference<T>.Type;

        public DataReference(string name, object value = null) : this(DataReference<T>.GenID, name, value) { }
        public DataReference(int id, string name, object value = null) : base(id, name, value)
        {
            if (DataReference<T>.Ref1.ContainsKey(this.ID))
                throw new InvalidOperationException("Attempting to add a duplicate reference: id = " + id);
            if (DataReference<T>.Ref2.ContainsKey(this.Name))
                throw new InvalidOperationException("Attempting to add a duplicate reference: name = " + name);
            DataReference<T>.GenID = DataReference<T>.GenID.Max(id) + 1;
            DataReference<T>.Ref1.Add(this.ID, this);
            DataReference<T>.Ref2.Add(this.Name, this);
        }

        public static int Clear()
        {
            int cnt = DataReference<T>.Ref1.Count;
            DataReference<T>.Ref1.Clear();
            DataReference<T>.Ref2.Clear();
            return cnt;
        }

        public static int[] GetRefIDs() => DataReference<T>.Ref1.Keys.ToArray();
        public static string[] GetRefNames() => DataReference<T>.Ref2.Keys.ToArray();
        public static DataReference<T>[] GetRefs() => DataReference<T>.Ref1.Values.ToArray();
        public static DataReference<T> GetRef(int id) => DataReference<T>.Ref1.TryGetValue(id, out DataReference<T> value) ? value : null;
        public static DataReference<T> GetRef(string name) => DataReference<T>.Ref2.TryGetValue(name, out DataReference<T> value) ? value : null;

        public static bool RemoveRef(int id)
        {
            if (DataReference<T>.Ref1.TryGetValue(id, out DataReference<T> @ref))
            {
                DataReference<T>.Ref1.Remove(@ref.ID);
                DataReference<T>.Ref2.Remove(@ref.Name);
                return true;
            }
            return false;
        }

        public static bool RemoveRef(string name)
        {
            if (DataReference<T>.Ref2.TryGetValue(name, out DataReference<T> @ref))
            {
                DataReference<T>.Ref1.Remove(@ref.ID);
                DataReference<T>.Ref2.Remove(@ref.Name);
                return true;
            }
            return false;
        }
        public static bool RemoveRef(DataReference<T> @ref)
        {
            if (DataReference<T>.Ref1.TryGetValue(@ref.ID, out DataReference<T> ref1) && DataReference<T>.Ref2.TryGetValue(@ref.Name, out DataReference<T> ref2) && ref1 == ref2)
            {
                DataReference<T>.Ref1.Remove(@ref.ID);
                DataReference<T>.Ref2.Remove(@ref.Name);
                return true;
            }
            return false;
        }

        public static implicit operator int(DataReference<T> @ref) => @ref.ID;
        public static implicit operator string(DataReference<T> @ref) => @ref.Name;

        public static bool operator ==(DataReference<T> ref1, DataReference<T> ref2) => ref1.ID == ref2.ID && ref1.Name == ref2.Name;
        public static bool operator !=(DataReference<T> ref1, DataReference<T> ref2) => ref1.ID != ref2.ID || ref1.Name != ref2.Name;
        public override bool Equals(object obj) => obj is DataReference<T> @ref ? this == @ref : false;
        public override int GetHashCode() => base.GetHashCode();
    }
}
