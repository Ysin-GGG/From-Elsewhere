using LOI.Gameplay;
using System.Collections.Generic;
using static LOI.LegendOfInfinity;

namespace LOI.DataStructures
{
    public class DamageData : TagSet
    {
        public string Name { get; set; } = string.Empty;
        public double Extra { get; set; }
        public double Value { get; set; }
        public Entity Root { get; set; }
        public Entity Owner { get; set; }
        public bool IsPrimaryDamage => true;
        public bool IsElementalDamage => this.damageType[true, 2, 3, 4, 5, 6, 7];
        public bool IsSlashDamage { get => !this.damageType[0] && !this.damageType[1]; set { if (value) { this.damageType[0] = false; this.damageType[1] = false; } } }
        public bool IsBluntDamage { get => !this.damageType[0] && this.damageType[1]; set { if (value) { this.damageType[0] = false; this.damageType[1] = true; } } }
        public bool IsPierceDamage { get => this.damageType[0] && !this.damageType[1]; set { if (value) { this.damageType[0] = true; this.damageType[1] = false; } } }
        public bool IsSpellDamage { get => this.damageType[0] && this.damageType[1]; set { if (value) { this.damageType[0] = true; this.damageType[1] = true; } } }
        public bool IsThermoDamage { get => this.damageType[2]; set => this.damageType[2] = value; }
        public bool IsHydroDamage { get => this.damageType[3]; set => this.damageType[3] = value; }
        public bool IsCyroDamage { get => this.damageType[4]; set => this.damageType[4] = value; }
        public bool IsBrontoDamage { get => this.damageType[5]; set => this.damageType[5] = value; }
        public bool IsAeroDamage { get => this.damageType[6]; set => this.damageType[6] = value; }
        public bool IsPetroDamage { get => this.damageType[7]; set => this.damageType[7] = value; }

        internal Bit8 damageType;

        public DamageData(Entity owner, double value, double extra, params IEnumerable<Tag> tags)
        {
            this.damageType = 0;
            this.Value = value;
            this.Extra = extra;
            this.Owner = owner;
            if (tags != null) this.AddTag(tags);
        }
    }
}