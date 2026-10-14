using Ref = LOI.DataStructures.DataReference<LOI.Reference.LayerRef>;

namespace LOI.Reference
{
    public sealed class LayerRef
    {
        private LayerRef() { }
        /// <summary> 2D物理层：边界 </summary>
        public static readonly Ref Physics2D_Border = new Ref(1, "Layer.Physics2D.Border", 1);
        /// <summary> 2D物理层：地面 </summary>
        public static readonly Ref Physics2D_Ground = new Ref(2, "Layer.Physics2D.Ground", 2);
        /// <summary> 2D物理层：墙壁 </summary>
        public static readonly Ref Physics2D_Wall = new Ref(3, "Layer.Physics2D.Wall", 3);
        /// <summary> 2D物理层：平台 </summary>
        public static readonly Ref Physics2D_Platform = new Ref(4, "Layer.Physics2D.Platform", 4);
        /// <summary> 2D物理层：角色 </summary>
        public static readonly Ref Physics2D_Character = new Ref(9, "Layer.Physics2D.Character", 9);
        /// <summary> 2D物理层：敌人 </summary>
        public static readonly Ref Physics2D_Enemy = new Ref(10, "Layer.Physics2D.Enemy", 10);
        /// <summary> 2D物理层：盟友 </summary>
        public static readonly Ref Physics2D_Ally = new Ref(11, "Layer.Physics2D.Ally", 11);
        /// <summary> 2D物理层：援助体 </summary>
        public static readonly Ref Physics2D_Helper = new Ref(12, "Layer.Physics2D.Helper", 12);
        /// <summary> 2D物理层：攻击 </summary>
        public static readonly Ref Physics2D_Hit = new Ref(17, "Layer.Physics2D.Hit", 17);
        /// <summary> 2D物理层：当身 </summary>
        public static readonly Ref Physics2D_Parry = new Ref(18, "Layer.Physics2D.Parry", 18);
        /// <summary> 2D物理层：投射物 </summary>
        public static readonly Ref Physics2D_Projectile = new Ref(19, "Layer.Physics2D.Projectile", 19);
    }
}
