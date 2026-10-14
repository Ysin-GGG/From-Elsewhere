using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace LOI.Gameplay
{
    public abstract class Modifier
    {
        public string Description { get; set; }

        public Modifier(Type type, string description)
        {
            this.Description = description;
            if (LegendOfInfinity.ModifierRegisters.TryGetValue(type, out IList<Modifier> list)) list.Add(this);
            else LegendOfInfinity.ModifierRegisters.Add(type, new List<Modifier>());
        }
    }

    public abstract class Modifier<T>(string description) : Modifier(typeof(T), description)
    {
        public abstract void Apply(T info);

        public abstract string GetDisplay(T info);
    }
}
