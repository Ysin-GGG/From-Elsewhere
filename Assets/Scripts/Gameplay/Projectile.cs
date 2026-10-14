using Godot;
using LOI.Controllers;
using LOI.DataStructures;
using LOI.Manager;
using LOI.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LOI.Controllers.HitDef;
using static LOI.LegendOfInfinity;

namespace LOI.Gameplay
{
    public class Projectile(Entity owner) : HitDef(owner)
    {
        /// <summary> 击中后，【弹幕归属实体】的状态跳转<para>默认为null（不跳转）</para> </summary>
        public new State HitStateSelf { get; set; } = null;
        /// <summary> 击中后，【目标实体】的状态跳转<para>默认为null（系统自动跳转）</para> </summary>
        public State HitStateTarget { get; set; } = null;
        /// <summary> 命中后自身停顿时长（帧）<para>默认为3，一帧内多次命中也不会叠加</para> </summary>
        public int HitStopSelf { get; set; } = 3;
        /// <summary> 命中后目标停顿时长（帧）<para>默认为3</para>  </summary>
        public int HitStopTarget { get; set; } = 3;
        /// <summary> 命中目标后，目标对该判定的无敌时长（帧）<para>默认为-1，即本次攻击对于同一个目标不会重复命中</para> </summary>
        public int HitImmuneTime { get; set; }
        /// <summary> 剩余可成功命中次数<para>为负数时，可命中次数为无限，否则可命中次数即为值</para>默认为-1 </summary>
        public int HitRemainAmount { get; set; }
        /// <summary> 一帧内可穿透的目标次数<para>为负数时，一帧内可最多命中无上限数量的目标</para>默认为0，即一帧内最多只能命中1个目标 </summary>
        public int HitPenetrationAmount { get; set; }
        /// <summary> 有效帧<para>该值逐帧递减1，直至归0后销毁</para>默认为1 </summary>
        public int HitTime { get; set; }
        /// <summary> 命中目标时，为自身恢复的基础气力值<para>该值在计算中作为基础值使用，实际数值会受到各种属性影响</para>默认为5 </summary>
        public int GainManaSelf { get; set; }
        /// <summary> 命中目标时，为目标恢复的基础气力值<para>该值在计算中作为基础值使用，实际数值会受到各种属性影响</para>默认为10 </summary>
        public int GainManaTarget { get; set; }
        /// <summary> 基础数值<para>该值在计算中作为基础值使用，实际数值会受到各种属性影响</para>默认为0 </summary>
        public int Value { get; set; }
        /// <summary> 基础倍率<para>该值在计算中作为基础值使用，实际数值会受到各种属性影响</para>默认为0 </summary>
        public double Power { get; set; }
        /// <summary> 是否可造成击杀<para>设为true时，可正常触发击杀相关机制，反之会为目标保留1点生命</para>默认为true（启用） </summary>
        public bool CanKill { get; set; }
        /// <summary> 允许击中己方单位<para>此处的己方单位是指：与【攻击实体】势力相同的实体</para>默认为false（禁用） </summary>
        public bool AffectFriends { get; set; }
        /// <summary> 允许击中敌方单位<para>此处的敌方单位是指：与【攻击实体】势力不同的实体</para>默认为true（启用） </summary>
        public bool AffectHostiles { get; set; }
        /// <summary> 击中在地面的目标时，为其赋予的水平初速度<para>负数会将其拉近，正数会将其击退</para><para>默认为null（不改变）</para> </summary>
        public float? GroundHitVelX { get => this.groundHitVelX; set => this.groundHitVelX = value; }
        /// <summary> 击中在地面的目标时，为其赋予的垂直初速度<para>负数会将其击飞，正数会将其击落</para><para>默认为null（不改变）</para> </summary>
        public float? GroundHitVelY { get => this.groundHitVelY; set => this.groundHitVelY = value; }
        /// <summary> 击中在地面的目标时，为其赋予的初速度<para>默认为null（不改变）</para> </summary>
        public Vector2? GroundHitVel { get => this.groundHitVelX == null || this.groundHitVelY == null ? null : new Vector2(this.groundHitVelX.Value, this.groundHitVelY.Value); set { this.groundHitVelX = value.Value.X; this.groundHitVelY = value.Value.Y; } }
        /// <summary> 击中在空中的目标时，为其赋予的水平初速度<para>负数会将其拉近，正数会将其击退</para><para>默认为null（不改变）</para> </summary>
        public float? AirHitVelX { get => this.airHitVelX; set => this.airHitVelX = value; }
        /// <summary> 击中在空中的目标时，为其赋予的垂直初速度<para>负数会将其击飞，正数会将其击落</para><para>默认为null（不改变）</para> </summary>
        public float? AirHitVelY { get => this.airHitVelY; set => this.airHitVelY = value; }
        /// <summary> 击中在空中的目标时，为其赋予的初速度<para>默认为null（不改变）</para> </summary>
        public Vector2? AirHitVel { get => this.airHitVelX == null || this.AirHitVelY == null ? null : new Vector2(this.airHitVelX.Value, this.airHitVelY.Value); set { this.airHitVelX = value.Value.X; this.airHitVelY = value.Value.Y; } }
        /// <summary> 攻击类型<para>可命中指定姿势类型的目标</para>默认为N，即不会攻击到任何姿势类型的目标 </summary>
        public HitFlag HitFlags { get; set; }
        /// <summary> 命中时事件 </summary>
        public Func<Entity, Entity, bool> OnHit { get; set; } = (e, t) => true;

        private Dictionary<Entity, int> immune = new Dictionary<Entity, int>();
        private float? groundHitVelX = null;
        private float? groundHitVelY = null;
        private float? airHitVelX = null;
        private float? airHitVelY = null;
        private Entity owner = null;
        private RectangleShape2D hitShape = null;
        private Bit8 damageType = new Bit8(0);
    }
}
