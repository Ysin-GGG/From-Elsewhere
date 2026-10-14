using LOI.Gameplay;
using System.Collections.Generic;

namespace LOI.DataStructures
{
    public abstract class TagSet : HashSet<Tag>
    {
        public TagSet() : base(TagSet.TagComparer) { }
        public bool this[Tag tag] => this.Contains(tag);
        public bool this[string tagName] => this.Contains(new Tag(tagName));
        public bool AddTag(Tag tag) => this.Add(tag);
        public bool RemoveTag(Tag tag) => this.Remove(tag);
        public int AddTag(params IEnumerable<Tag> tags) { int cnt = this.Count; this.UnionWith(tags); return this.Count - cnt; }
        public int RemoveTag(params IEnumerable<Tag> tags) { int cnt = this.Count; this.ExceptWith(tags); return cnt - this.Count; }
        public Tag AddTag(string tagName) { Tag tag = new Tag(tagName); return this.AddTag(tag) ? tag : null; }
        public Tag RemoveTag(string tagName) { Tag tag = new Tag(tagName); return this.RemoveTag(tag) ? tag : null; }

        private static readonly EqualityComparer<Tag> TagComparer = EqualityComparer<Tag>.Create((a, b) => a.Name == b.Name, (t) => t.Name.GetHashCode());
    }
}
