using LOI.DataStructures;
using LOI.States;
using LOI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using static LOI.LegendOfInfinity;

namespace LOI.Gameplay
{
    /// <summary> 武器动作模组 </summary>
    public abstract class WeaponArt
    {
        /// <summary> 限用武器 </summary>
        public WeaponClass RequiredWeapon { get; set; }
        public StateType RequiredState { get; set; }
        public int RequiredStamina { get; set; } = 0;
        public int RequiredMana { get; set; } = 0;
        public WeaponArt RequiredArt { get; set; } = null;
        public WeaponArt PreArt { get; set; } = null;
        public WeaponArt NextArtWithLight { get; set; } = null;
        public WeaponArt NextArtWithHeavy { get; set; } = null;
        public WeaponArt NextArtWithRelease { get; set; } = null;
        public State[] States => this.StateSet.ToArray();
        protected HashSet<State> StateSet { get; set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        private static int TypeCount = 0;
        private static Dictionary<Type, int> TypeMap = new Dictionary<Type, int>();
        private int Type { get; set; } = -1;

        public WeaponArt()
        {
        }

        public virtual bool SetDefaults() => true;

        /// <summary> 输入检测 </summary>
        /// <param name="lightTick">轻攻击按住时长</param>
        /// <param name="heavyTick">重攻击按住时长</param>
        /// <param name="releaseTick">炼气解放按住时长</param>
        public void InputCheck(int lightTick, int heavyTick, int releaseTick)
        {

        }

        public void ComboWith(WeaponArt art, ref List<string> requests)
        {

        }

        public bool CanEquip(Character character)
        {
            return true;
        }

        public State GetState()
        {
            return null;
        }

        public static int GetWeaponArtType<T>() where T : WeaponArt, new()
        {
            Type type = typeof(T);
            if (!WeaponArt.TypeMap.ContainsKey(type)) WeaponArt.TypeMap[type] = WeaponArt.TypeCount++;
            return WeaponArt.TypeMap[type];
        }

        public class TestArt : WeaponArt
        {
            public State State { get; set; }
        }
    }
}
