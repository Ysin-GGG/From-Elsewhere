using Ref = LOI.DataStructures.DataReference<LOI.Reference.ColorRef>;

namespace LOI.Reference
{
    public sealed class ColorRef
    {
        private ColorRef() { }
        public static readonly Ref Attr_MaxLife = new Ref(1, "Vanilla.MaxLife");
    }
}
