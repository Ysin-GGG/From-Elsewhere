using Ref = LOI.DataStructures.DataReference<LOI.Reference.StatisticRef>;

namespace LOI.Reference
{
    public sealed class StatisticRef
    {
        private StatisticRef() { }
        public static readonly Ref Attr_MaxLife = new Ref(1, "Vanilla.MaxLife");
        public static readonly Ref Attr_MaxMana = new Ref(2, "Vanilla.MaxMana");
        public static readonly Ref Attr_Attack = new Ref(3, "Vanilla.Attack");
        public static readonly Ref Attr_Defense = new Ref(4, "Vanilla.Defense");
        public static readonly Ref Attr_MovementSpeed = new Ref(5, "Vanilla.MovementSpeed");
        public static readonly Ref Attr_JumpStrength = new Ref(6, "Vanilla.JumpStrength");

        public static readonly Ref Critical_Chance = new Ref(7, "Vanilla.Critical.Chance");
        public static readonly Ref Critical_Damage = new Ref(8, "Vanilla.Critical.Damage");

        public static readonly Ref Power_Direct = new Ref(20, "Vanilla.Power.Direct");
        public static readonly Ref Power_Indirect = new Ref(21, "Vanilla.Power.Indirect");
        public static readonly Ref Power_Normal = new Ref(22, "Vanilla.Power.Normal");
        public static readonly Ref Power_Special = new Ref(23, "Vanilla.Power.Special");
        public static readonly Ref Power_Primary = new Ref(24, "Vanilla.Power.Primary");
        public static readonly Ref Power_Elemental = new Ref(25, "Vanilla.Power.Elemental");

        public static readonly Ref Power_Slash = new Ref(30, "Vanilla.Power.Slash");
        public static readonly Ref Power_Blunt = new Ref(31, "Vanilla.Power.Blunt");
        public static readonly Ref Power_Pierce = new Ref(32, "Vanilla.Power.Pierce");
        public static readonly Ref Power_Spell = new Ref(33, "Vanilla.Power.Spell");
        public static readonly Ref Power_Thermo = new Ref(34, "Vanilla.Power.Thermo");
        public static readonly Ref Power_Hydro = new Ref(35, "Vanilla.Power.Hydro");
        public static readonly Ref Power_Cyro = new Ref(36, "Vanilla.Power.Cyro");
        public static readonly Ref Power_Bronto = new Ref(37, "Vanilla.Power.Bronto");
        public static readonly Ref Power_Aero = new Ref(38, "Vanilla.Power.Aero");
        public static readonly Ref Power_Petro = new Ref(39, "Vanilla.Power.Petro");

        public static readonly Ref Resistance_Direct = new Ref(70, "Vanilla.Resistance.Direct");
        public static readonly Ref Resistance_Indirect = new Ref(71, "Vanilla.Resistance.Indirect");
        public static readonly Ref Resistance_Normal = new Ref(72, "Vanilla.Resistance.Normal");
        public static readonly Ref Resistance_Special = new Ref(73, "Vanilla.Resistance.Special");
        public static readonly Ref Resistance_Primary = new Ref(74, "Vanilla.Resistance.Primary");
        public static readonly Ref Resistance_Elemental = new Ref(75, "Vanilla.Resistance.Elemental");

        public static readonly Ref Resistance_Slash = new Ref(80, "Vanilla.Resistance.Slash");
        public static readonly Ref Resistance_Blunt = new Ref(81, "Vanilla.Resistance.Blunt");
        public static readonly Ref Resistance_Pierce = new Ref(82, "Vanilla.Resistance.Pierce");
        public static readonly Ref Resistance_Spell = new Ref(83, "Vanilla.Resistance.Spell");
        public static readonly Ref Resistance_Thermo = new Ref(84, "Vanilla.Resistance.Thermo");
        public static readonly Ref Resistance_Hydro = new Ref(85, "Vanilla.Resistance.Hydro");
        public static readonly Ref Resistance_Cyro = new Ref(86, "Vanilla.Resistance.Cyro");
        public static readonly Ref Resistance_Bronto = new Ref(87, "Vanilla.Resistance.Bronto");
        public static readonly Ref Resistance_Aero = new Ref(88, "Vanilla.Resistance.Aero");
        public static readonly Ref Resistance_Petro = new Ref(89, "Vanilla.Resistance.Petro");

        public static readonly Ref Attr_Force = new Ref(90, "Vanilla.Force");
        public static readonly Ref Attr_Poise = new Ref(91, "Vanilla.Poise");
    }
}
