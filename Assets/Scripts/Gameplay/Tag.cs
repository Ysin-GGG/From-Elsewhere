using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LOI.Gameplay
{
    public class Tag : IEqualityComparer<Tag>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Tag(string Name)
        {
            this.Name = Name;
        }

        public bool Equals(Tag x, Tag y) => x.Name == y.Name;

        public int GetHashCode([DisallowNull] Tag obj) => obj.Name.GetHashCode();
    }
}
