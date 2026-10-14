using LOI.DataStructures;
using LOI.Interface;
using LOI.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static LOI.LegendOfInfinity;

namespace LOI.Gameplay
{
    public abstract class Equipment(int width, int height) : Item(width, height), IModifierItem<Character>
    {
        public CharacterStatisticCollection Stats { get; } = new CharacterStatisticCollection();


        public abstract EquipmentType EquipType { get; }
        public abstract EquipmentRarity EquipRarity { get; }

        public int ItemLevel { get => this.il; set => this.il = value.Max(1); }
        public int UpgradeLevel { get => this.ul; set => this.ul = value.Max(0); }


        public List<Modifier<Character>> Modifiers { get; } = new List<Modifier<Character>>();

        IList IModifierItem.Modifiers => this.Modifiers;

        private int il = 1;
        private int ul = 0;
        private DependentFactor lvBouns;

        public override bool SetDefaults()
        {
            this.lvBouns = new DependentFactor("lv", () => (1d + this.il).Ln().Sqrt() * this.ul / LazyMath.Phi1 / 10 + 1);
            // foreach (Statistic2 stat in this.Stats.Array)
                //stat.AddModifier (this.lvBouns);
            return true;
        }

        /// <summary> 强化成功率计算 </summary>
        /// <returns>是否强化成功</returns>
        /// <param name="eq1">一号装备素材</param>
        /// <param name="eq2">二号装备素材</param>
        /// <param name="eq3">三号装备素材</param>
        /// <param name="lucky">是否应用“幸运”效果</param>
        /// <returns>实际强化成功率</returns>
        public double GetUpgradeChance(Equipment eq1, Equipment eq2, Equipment eq3, bool lucky = false)
        {
            double factor = 16 * (this.UpgradeLevel + 1) / (this.UpgradeLevel + 4);
            double chance = 1 / (4 + this.UpgradeLevel);
            if (eq1 != null) chance += (eq1.UpgradeLevel + 1) / (this.UpgradeLevel + 1) / factor;
            if (eq2 != null) chance += (eq2.UpgradeLevel + 1) / (this.UpgradeLevel + 1) / factor;
            if (eq3 != null) chance += (eq3.UpgradeLevel + 1) / (this.UpgradeLevel + 1) / factor;
            chance = chance.Min(1);
            return lucky ? 2 * chance - chance * chance : chance;
        }
    }
}
