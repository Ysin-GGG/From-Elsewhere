using Godot;
using LOI.DataStructures;
using LOI.Gameplay;
using LOI.Manager;
using LOI.States;
using LOI.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using static LOI.LegendOfInfinity;
using LOI.Reference;

namespace LOI.Controllers
{
    /// <summary> 攻击判定控制器 </summary>
    public class HitDef : TagSet
    {
        /// <summary> 击中后，【攻击实体】的状态跳转<para>默认为null（不跳转）</para> </summary>
        public State HitStateSelf { get; set; }
        /// <summary> 击中后，【目标实体】的状态跳转<para>默认为null（系统自动跳转）</para> </summary>
        public State HitStateTarget { get; set; }
        /// <summary> 命中目标后，使自身处于硬直的时长（帧）<para>重复触发只取最高值，而并非叠加</para>默认为6 </summary>
        public int HitStopSelf { get; set; }
        /// <summary> 命中目标后，使目标处于硬直的时长（帧）<para>重复触发只取最高值，而并非叠加</para>默认为8 </summary>
        public int HitStopTarget { get; set; }
        /// <summary> 命中目标后，使自身处于硬直的至短时长（帧）<para>重复触发只取最高值，而并非叠加</para>默认为0 </summary>
        public int ForceHitStopSelf { get; set; }
        /// <summary> 命中目标后，使目标处于硬直的至短时长（帧）<para>重复触发只取最高值，而并非叠加</para>默认为0 </summary>
        public int ForceHitStopTarget { get; set; }
        /// <summary> 命中目标后，目标对该判定的无敌时长（帧）<para>默认为-1，即本次攻击对于同一个目标不会重复命中</para> </summary>
        public int HitImmuneTime { get; set; }
        /// <summary> 剩余可成功命中次数<para>为负数时，可命中次数为无限，否则可命中次数即为值</para>默认为-1 </summary>
        public int HitRemainAmount { get; set; }
        /// <summary> 一帧内可穿透的目标次数<para>为负数时，一帧内可最多命中无上限数量的目标</para>默认为0，即一帧内最多只能命中1个目标 </summary>
        public int HitPenetrationAmount { get; set; }
        /// <summary> 有效帧<para>该值逐帧递减1，直至归0后销毁</para>默认为1 </summary>
        public int HitTime {  get; set; }
        /// <summary> 命中延迟 </summary> <remarks>在击中目标后，延迟生效的时间<para>默认为0</para></remarks>
        [Obsolete]
        public int HitDelay { get; set; }
        /// <summary> 命中目标时，为自身恢复的基础气力值<para>该值在计算中作为基础值使用，实际数值会受到各种属性影响</para>默认为5 </summary>
        public int GainManaSelf { get; set; }
        /// <summary> 命中目标时，为目标恢复的基础气力值<para>该值在计算中作为基础值使用，实际数值会受到各种属性影响</para>默认为10 </summary>
        public int GainManaTarget { get; set; }
        /// <summary> 基础数值<para>该值在计算中作为基础值使用，实际数值会受到各种属性影响</para>默认为0 </summary>
        public int Value { get; set; }
        /// <summary> 基础倍率<para>该值在计算中作为基础值使用，实际数值会受到各种属性影响</para>默认为0 </summary>
        public double Power { get; set; }
        /// <summary> 力道倍率<para>本次攻击判定的力道倍率</para>默认为1 </summary>
        public double Force { get; set; }
        /// <summary> 是否可被招架 </summary><remarks>设为<see langword="true"/>时，可正常触发招架相关机制，反之会跳过招架判定<para>默认为<see langword="true"/>（启用）</para></remarks>
        public bool CanBeParried { get; set; }
        /// <summary> 是否可被格挡 </summary><remarks>设为<see langword="true"/>时，可正常触发格挡相关机制，反之会跳过格挡判定<para>默认为<see langword="true"/>（启用）</para></remarks>
        public bool CanBeBlocked { get; set; }
        /// <summary> 是否可造成击杀 </summary><remarks>设为<see langword="true"/>时，可正常触发击杀相关机制，反之会为目标保留1点生命<para>默认为<see langword="true"/>（启用）</para></remarks>
        public bool CanKill { get; set; }
        /// <summary> 允许击中己方单位 </summary> <remarks>此处的己方单位是指：与【攻击实体】势力相同的实体<para>默认为<see langword="false"/>（禁用）</para></remarks>
        public bool AffectFriends { get; set; }
        /// <summary> 允许击中敌方单位 </summary> <remarks>此处的敌方单位是指：与【攻击实体】势力不同的实体<para>默认为<see langword="true"/>（启用）</para></remarks>
        public bool AffectHostiles { get; set; }
        /// <summary> 击中在地面的目标时，为其赋予的水平初速度 </summary><remarks>负数会将其拉近，正数会将其击退<para>默认为<see langword="null"/>（不改变）</para></remarks>
        public float? GroundHitVelX { get => this.groundHitVelX ; set => this.groundHitVelX = value; }
        /// <summary> 击中在地面的目标时，为其赋予的垂直初速度 </summary> <remarks>负数会将其击飞，正数会将其击落<para>默认为<see langword="null"/>（不改变）</para></remarks>
        public float? GroundHitVelY { get => this.groundHitVelY; set => this.groundHitVelY = value; }
        /// <summary> 击中在地面的目标时，为其赋予的初速度 </summary> <remarks>默认为<see langword="null"/>（不改变）</remarks>
        public Vector2? GroundHitVel { get => this.groundHitVelX == null || this.groundHitVelY == null ? null : new Vector2(this.groundHitVelX.Value, this.groundHitVelY.Value); set { this.groundHitVelX = value.Value.X; this.groundHitVelY = value.Value.Y; } }
        /// <summary> 击中在空中的目标时，为其赋予的水平初速度 </summary><remarks>负数会将其拉近，正数会将其击退<para>默认为<see langword="null"/>（不改变）</para></remarks>
        public float? AirHitVelX { get => this.airHitVelX; set => this.airHitVelX = value; }
        /// <summary> 击中在空中的目标时，为其赋予的垂直初速度 </summary> <remarks>负数会将其击飞，正数会将其击落<para>默认为<see langword="null"/>（不改变）</para></remarks>
        public float? AirHitVelY { get => this.airHitVelY; set => this.airHitVelY = value; }
        /// <summary> 击中在空中的目标时，为其赋予的初速度 </summary> <remarks>默认为<see langword="null"/>（不改变）</remarks>
        public Vector2? AirHitVel { get => this.airHitVelX == null || this.AirHitVelY == null ? null : new Vector2(this.airHitVelX.Value, this.airHitVelY.Value); set { this.airHitVelX = value.Value.X; this.airHitVelY = value.Value.Y; } }
        /// <summary> 攻击类型 </summary><remarks>可命中指定姿势类型的目标<para>默认为<see cref="HitFlag.N"/>，即不会攻击到任何姿势类型的目标</para></remarks>
        public HitFlag HitFlag { get; set; }
        /// <summary> 攻击类型 </summary><remarks>可命中指定姿势类型的目标，其中StateType.Inherit可以命中StateType.Undefined以外的所有StateType<para>默认为<see langword="null"/>，即不会攻击到任何姿势类型的目标</para></remarks>
        public HashSet<StateType> StateFlag { get; set; } = null;
        /// <summary> 命中音效 </summary><remarks>默认为<see langword="null"/>（无音效）</remarks>
        public AudioStream HitSound { get; set; }
        /// <summary> 命中时事件 </summary>
        public Func<Entity, Entity, bool> OnHit { get; set; } = (e, t) => true;

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

        private Dictionary<Entity, int> immune = new Dictionary<Entity, int>();
        private float? groundHitVelX = null;
        private float? groundHitVelY = null;
        private float? airHitVelX = null;
        private float? airHitVelY = null;
        private Entity owner = null;
        private Bit8 damageType = new Bit8(0);
        public Rect2 CombatTextRandomSpawn { get; set; } = new Rect2();

        public HitDef(Entity owner)
        {
            this.owner = owner;
            this.owner.HitController = this;
            this.HitStateSelf = null;
            this.HitStateTarget = null;
            this.HitStopSelf = 6;
            this.HitStopTarget = 8;
            this.ForceHitStopSelf = 0;
            this.ForceHitStopTarget = 0;
            this.HitImmuneTime = -1;
            this.HitRemainAmount = -1;
            this.HitPenetrationAmount = 0;
            this.HitTime = 3;
            this.HitDelay = 0;
            this.GainManaSelf = 5;
            this.GainManaTarget = 10;
            this.Value = 0;
            this.CanBeParried = true;
            this.CanBeBlocked = true;
            this.CanKill = true;
            this.AffectFriends = false;
            this.AffectHostiles = true;
            this.HitFlag = HitFlag.N;
            this.HitSound = null;
        }

        public bool CheckRelation(Entity target) => 0 != (
            (this.AffectFriends.ToInt(0b1100) | this.AffectHostiles.ToInt(0b0011)) &
            (this.owner.IsFriendly.ToInt(0b1010) | this.owner.IsHostile.ToInt(0b0101)) &
            (target.IsFriendly.ToInt(0b1001) | target.IsHostile.ToInt(0b0110)));

        public bool CheckFlag(Entity target) => 0 != 
            ((int)this.HitFlag & 1 << (int)target.StateType >> 1);

        public bool Check(Entity target)
        {
            if (this.HitImmuneTime == 0 || !this.immune.ContainsKey(target) || this.immune[target]-- == 0)
                if (this.CheckRelation(target) && this.owner.CheckCollision(target) && this.CheckFlag(target))
                    return true;
            return false;
        }

        public void CheckAll(params ICollection<Entity> entities)
        {
            if (this.HitRemainAmount == 0 || this.HitTime <= 0)
            {
                this.HitTime = 0;
                this.owner.HitController = null;
                return;
            }
            int p = 0;
            this.HitTime--;
            if (entities != null && entities.Count > 0)
                foreach (Entity target in entities)
                    if (this.Check(target) && (this.OnHit?.Invoke(this.owner, target) ?? true))
                        if (p++ > this.HitPenetrationAmount) break;
                        else this.Apply(target);
            if (p > 0) AssetManager.PlaySound(this.HitSound, this.owner.Position, 1f);
        }

        /// <summary> 命中目标 <para>亦可用作直接命中的攻击</para> </summary>
        /// <param name="target">目标实体</param>
        public void Apply(Entity target)
        {
            DamageData source = new(this.owner, this.Power, this.Value, this);
            source.damageType = this.damageType;
            this.owner.Mana += this.GainManaSelf;
            target.Mana += this.GainManaTarget;
            if (target.PhysicsType == PhysicsType.Ground)
            {
                if (this.groundHitVelX.HasValue) target.VelX = this.GroundHitVelX.Value * this.owner.Facing;
                if (this.groundHitVelY.HasValue) target.VelY = this.GroundHitVelY.Value * this.owner.Facing;
            }
            else if (target.PhysicsType == PhysicsType.Air)
            {
                if (this.airHitVelX.HasValue) target.VelX = this.airHitVelX.Value * this.owner.Facing;
                if (this.airHitVelY.HasValue) target.VelY = this.airHitVelY.Value * this.owner.Facing;
            }
            double force = this.owner.Stats.Force * this.Force;
            double poise = target.Stats.Poise;
            double diff = 1d;
            if (force > poise) diff += 1d.Max(force - poise);
            else if (force < poise - 1d) diff -= 1d.Max(poise - force - 1d);
            this.owner.HitStopTime = this.ForceHitStopSelf.Max(this.HitStopSelf, this.owner.HitStopTime);
            target.HitStopTime = this.ForceHitStopTarget.Max((this.HitStopTarget * diff).Round(), target.HitStopTime);
            //bool actionBreak = Rand.NextBool((poise - force));
            if (this.HitImmuneTime != 0 && !this.immune.TryAdd(target, this.HitImmuneTime))
                this.immune[target] = this.HitImmuneTime;

            if (this.Power > 0)
            {
                double damage = target.Damage(source, out bool crit);
                var t = target.DamageDirectly(source, damage, crit, this.CanKill, true);
                t.Position += this.CombatTextRandomSpawn.Position + new Vector2(Rand.NextFloat(0, this.CombatTextRandomSpawn.Size.X), Rand.NextFloat(0, this.CombatTextRandomSpawn.Size.Y));
            }
            else target.DamageDirectly(source, 0, false, this.CanKill, false);
            if (target.ReversalController != null) target.ReversalController.Check(this.owner);
            else
            {
                this.owner.ChangeState(this.HitStateSelf);
                if (this.HitStateTarget != null) target.ChangeState(this.HitStateTarget);
                else if (target.StateType == StateType.Stand)
                    target.ChangeState(State.GetState<CommonState.StandHighHitStop>());// target.ChangeState(this.HitBox.Position.Y < target.HitBox.Size.Y ? State.GetState<CommonState.StandLowHitStop>() : State.GetState<CommonState.StandHighHitStop>());
                else if (target.StateType == StateType.Crouch)
                    target.ChangeState(State.GetState<CommonState.CrouchHitStop>());
                else if (target.StateType == StateType.Air)
                    target.ChangeState(State.GetState<CommonState.AirHitStop>());
                if (--this.HitRemainAmount == 0)
                    this.HitTime = 0;
            }
        }
    }
}
