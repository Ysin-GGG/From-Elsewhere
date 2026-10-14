using Godot.NativeInterop;
using System.Data;
using static LOI.LegendOfInfinity;

namespace LOI.Gameplay
{
    public abstract class Weapon(WeaponClass type) : Equipment(1, 1)
    {
        public override EquipmentType EquipType => EquipmentType.Weapon;
        public WeaponClass WeaponType { get; } = type;
    }
}
