using Ref = LOI.DataStructures.DataReference<LOI.Reference.ItemRef>;

namespace LOI.Reference
{
    public sealed class ItemRef
    {
        private ItemRef() { }
        /// <summary> 空白符文 </summary>
        public static readonly Ref RuneOfVoid = new Ref(1, "Vanilla.Item.RuneOfVoid");
        /// <summary> 狂热符文 </summary>
        public static readonly Ref RuneOfMania = new Ref(2, "Vanilla.Item.RuneOfMania");
        /// <summary> 忘却符文 </summary>
        public static readonly Ref RuneOfAmnesia = new Ref(3, "Vanilla.Item.RuneOfAmnesia");
        /// <summary> 无常符文 </summary>
        public static readonly Ref RuneOfEpilepsia = new Ref(4, "Vanilla.Item.RuneOfEpilepsia");
        /// <summary> 裂生符文 </summary>
        public static readonly Ref RuneOfSchizophrenia = new Ref(5, "Vanilla.Item.RuneOfSchizophrenia");
        /// <summary> 妄执符文 </summary>
        public static readonly Ref RuneOfParanoia = new Ref(6, "Vanilla.Item.RuneOfParanoia");
    }
}
