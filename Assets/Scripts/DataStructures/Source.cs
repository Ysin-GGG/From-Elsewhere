using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace LOI.DataStructures
{
    [Serializable]
    public abstract class Source
    {
        public string Name { get; }

    }

    public sealed class ModSource : Source
    {
        private ModSource() { }

        public Version Version { get; }
        public string ModName { get; }
        public string Author { get; }
    }
}
