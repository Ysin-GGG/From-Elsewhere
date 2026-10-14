using Ref = LOI.DataStructures.DataReference<LOI.Reference.TagRef>;

namespace LOI.Reference
{
    public sealed class TagRef
    {
        private TagRef() { }
        public static readonly Ref DirectDamage = new Ref(1, "Vanilla.DirectDamage");
        public static readonly Ref IndirectDamage = new Ref(2, "Vanilla.IndirectDamage");
        public static readonly Ref NormalDamage = new Ref(3, "Vanilla.NormalDamage");
        public static readonly Ref SpecicalDamage = new Ref(4, "Vanilla.SpecicalDamage");
        public static readonly Ref PrimaryDamage = new Ref(5, "Vanilla.PrimaryDamage");
        public static readonly Ref ElementalDamage = new Ref(6, "Vanilla.ElementalDamage");
        public static readonly Ref SlashDamage = new Ref(11, "Vanilla.SlashDamage");
        public static readonly Ref BluntDamage = new Ref(12, "Vanilla.BluntDamage");
        public static readonly Ref PierceDamage = new Ref(13, "Vanilla.PierceDamage");
        public static readonly Ref ThermoDamage = new Ref(14, "Vanilla.ThermoDamage");
        public static readonly Ref HydroDamage = new Ref(15, "Vanilla.HydroDamage");
        public static readonly Ref CyroDamage = new Ref(16, "Vanilla.CyroDamage");
        public static readonly Ref BrontoDamage = new Ref(17, "Vanilla.BrontoDamage");
        public static readonly Ref AeroDamage = new Ref(18, "Vanilla.AeroDamage");
        public static readonly Ref PetroDamage = new Ref(19, "Vanilla.PetroDamage");
        public static readonly Ref OpeningArt = new Ref(21, "Vanilla.OpeningArt");
        public static readonly Ref ComboArt = new Ref(22, "Vanilla.ComboArt");
        public static readonly Ref FinishingArt = new Ref(23, "Vanilla.FinishingArt");
    }
}
