using Godot;
using LOI.DataStructures;
using LOI.GameObject;
using LOI.Interface;
using LOI.Manager;
using LOI.Reference;
using LOI.States;
using LOI.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static LOI.LegendOfInfinity;

namespace LOI.Gameplay
{
    public class Character : Entity
    {
        public override bool IsFriendly => true;
        public int Stamina { get => field; set => field = value.Clamp(0, this.MaxStamina); }
        public int MaxStamina { get; set; } = 4000;// { get => this.Stats["MaxStamina"] as Statistic2<int>; set => (this.Stats["MaxStamina"] as Statistic2<int>).DefValue = value; }
        /// <summary> 炼气值 <para>角色执行特殊动作或机制时使用，主要通过使用技能一次性消耗大量气力的方式，按1000:1的比例获得炼气</para>炼气值不会溢出，即不会超过最大炼气值 </summary>
        public int Focus { get => field; set => field = value.Clamp(0, this.MaxFocus); }
        /// <summary> 最大炼气值 <para>角色执行特殊动作或机制时使用，主要通过使用技能一次性消耗大量气力的方式，按1000:1的比例获得炼气</para>炼气值不会溢出，即不会超过最大炼气值 </summary>
        public int MaxFocus { get => field; set => this.Focus = this.Focus.Clamp(0, field = value.Clamp(0, 5)); }
        public int Fatigue { get => field; set => field = value.Clamp(0, 90); }
        public int Exhaustion { get => field; set => field = value.Max(0); }

        /// <summary> 长刃-御气姿态 </summary>
        public bool Blade_BreathStance { get => this._weaponType[(int)WeaponClass.Blade] && this._WeaponVar_Byte1 == 1; set => this._weaponType[(int)WeaponClass.Blade].IfElse(this.UpdateWeaponVar(this._WeaponVar_Byte1 = 1, 1), 0); }
        /// <summary> 长刃-识破姿态 </summary>
        public bool Blade_RiposteStance { get => this._weaponType[(int)WeaponClass.Blade] && this._WeaponVar_Byte1 == 2; set => this._weaponType[(int)WeaponClass.Blade].IfElse(this.UpdateWeaponVar(this._WeaponVar_Byte1 = 2, 1), 0); }
        /// <summary> 长刃-剑气值 </summary>
        public short Blade_SwordBreeze { get => this._weaponType[(int)WeaponClass.Blade].IfElse<short>(this._WeaponVar_Short1, -1); set => this._weaponType[(int)WeaponClass.Blade].IfElse(this.UpdateWeaponVar(this._WeaponVar_Short1 = value.Clamp<short>(0, this.Blade_SwordBreezeMax), 1), 0); }
        public short Blade_SwordBreezeMax { get => this._weaponType[(int)WeaponClass.Blade].IfElse<short>(this._WeaponVar_Short2, -1); set => this._weaponType [(int)WeaponClass.Blade].IfElse(this.UpdateWeaponVar(this._WeaponVar_Short2 = value.Clamp<short>(1000, 9000), 2), 0); }
        private byte _WeaponVar_Byte1 { get; set; }
        private byte _WeaponVar_Byte2 { get; set; }
        private short _WeaponVar_Short1 { get; set; }
        private short _WeaponVar_Short2 { get; set; }
        private int _WeaponVar_Int1 { get; set; }
        private int _WeaponVar_Int2 { get; set; }
        private bool _WeaponVar_Bool1 { get; set; }
        private bool _WeaponVar_Bool2 { get; set; }
        private float _WeaponVar_Float1 { get; set; }
        private float _WeaponVar_Float2 { get; set; }
        private double _WeaponVar_Double1 { get; set; }
        private double _WeaponVar_Double2 { get; set; }
        private Bit16 _weaponType = new Bit16(0);

        public override CharacterStatisticCollection Stats { get; } = new CharacterStatisticCollection();

        private T UpdateWeaponVar<T>(T var, int id)
        {
            switch (var)
            {
                case byte b:
                    //if (this._weaponType[(int)WeaponClass.Blade])
                    break;
                case short s:
                    if (this._weaponType[(int)WeaponClass.Blade])
                        this.Stats.Attack.SwordBreeze.Multiplier = 1 - this.Blade_SwordBreeze / 1000 * .05d;
                    break;
            }
            return var;
        }

        public void OnWeaponInit()
        {
            Weapon weapon = this.Equipment[0] as Weapon;
            if (weapon.NotNull())
            {
                this._WeaponVar_Byte1 = 1;
                this._WeaponVar_Byte2 = 0;
                this._WeaponVar_Short1 = 0;
                this._WeaponVar_Short2 = 0;
                this._WeaponVar_Int1 = 0;
                this._WeaponVar_Int2 = 0;
                this._WeaponVar_Bool1 = false;
                this._WeaponVar_Bool2 = false;
                this._weaponType = 1 << (int)weapon.WeaponType;
                switch (weapon.WeaponType)
                {
                    case WeaponClass.Blade:
                        this.Blade_RiposteStance = true;
                        this.Blade_SwordBreeze = 0;
                        break;
                }
            }
        }

        public void OnWeaponUpdated()
        {
            switch (this.WeaponClass)
            {
                case WeaponClass.Artefact:
                case WeaponClass.Bow:
                case WeaponClass.Dagger:
                case WeaponClass.Handgun:
                case WeaponClass.Polearm:
                case WeaponClass.SwordAndShield:
                case WeaponClass.Whip:
                    throw new NotImplementedException("The weapon class is not implemented yet.");
                case WeaponClass.None:
                    break;
                case WeaponClass.Blade:
                    break;
            }
        }

        public WeaponClass WeaponClass => (this.Equipment[0] as Weapon)?.WeaponType ?? WeaponClass.None;

        private Equipment[] Equipment { get; } = new Equipment[10]; // 0: 武器，1-4：饰品，5-9: 防具
        public List<Cate> Dishes { get; } = new List<Cate>();

        public Character(CharacterBody2D obj, int id) : base(obj, id)
        {
            /*
            this.Stats.MaxLife.ValueChanged += (oldV, newV) => this.Life = (this.Life / (double)(oldV / newV)).Ceiling();
            this.Life = this.Stats.MaxLife;
            this.Stats.AddStat(new Statistic2<int>("MaxStamina", 4000, 1000, 9000));
            */
            //this.Stats.AddStat(DataReference<StatisticRef>.GetRef(9) ?? new DataReference<StatisticRef>(9, "Vanilla.MaxStamina"), 4000d, 1000d, 9000d);
            /*
            //new DataReference<StatisticRef>(9, "Vanilla.LifeCost");
            new DataReference<StatisticRef>(9, "Vanilla.LifeRegeneration");
            new DataReference<StatisticRef>(9, "Vanilla.ManaCost");
            new DataReference<StatisticRef>(9, "Vanilla.ManaGainRate");
            new DataReference<StatisticRef>(9, "Vanilla.StaminaCost");
            new DataReference<StatisticRef>(9, "Vanilla.StaminaGainRate");
            Curve2D normalCurve  = new Curve2D();
            normalCurve.AddPoint(new Vector2(0, 0));
            normalCurve.AddPoint(new Vector2(.33f, 1f));
            normalCurve.AddPoint(new Vector2(.66f, 1f));
            normalCurve.AddPoint(new Vector2(1, 0));
            */
            this.Init(WeaponClass.Blade);
        }

        public void Init(WeaponClass type)
        {
            this._WeaponVar_Byte1 = 1;
            this._WeaponVar_Byte2 = 0;
            this._WeaponVar_Short1 = 0;
            this._WeaponVar_Short2 = 0;
            this._WeaponVar_Int1 = 0;
            this._WeaponVar_Int2 = 0;
            this._WeaponVar_Bool1 = false;
            this._WeaponVar_Bool2 = false;
            this._weaponType = 1 << (int)type;
            switch (type)
            {
                case WeaponClass.Blade:
                    this.Blade_RiposteStance = true;
                    this.Blade_SwordBreezeMax = 4000;
                    this.Blade_SwordBreeze = 0;
                    break;
            }
        }

        protected override void Update()
        {
            if (this.Ctrl && this.MoveType == MoveType.Idle)
                if (InputManager.OnPressed(GameKey.Right)) { if (!InputManager.OnPressed(GameKey.Left)) this.Facing = 1; }
                else if (InputManager.OnPressed(GameKey.Left)) this.Facing = -1;
            if (this.Blade_RiposteStance && this.Blade_SwordBreeze < this.Blade_SwordBreezeMax) this.Blade_SwordBreeze += 5;
        }

        protected override bool PreHit(DamageData source, Entity target, ref double damage, ref int delta_Lvl, ref double delta_AnD, ref double delta_PnR, ref double crit_chance, ref double crit_damage)
        {
            this.Blade_SwordBreeze -= (source.Value > 1.5).IfElse<short>(150, 100);
            return true;
        }

        internal override State CommonState()
        {
            State parent = base.CommonState();
            if (parent == null)
            {
                bool ctrl = this.Ctrl;
                bool u = InputManager.OnPressed(GameKey.Up);
                bool d = InputManager.OnPressed(GameKey.Down);
                bool l = InputManager.OnPressed(GameKey.Left);
                bool r = InputManager.OnPressed(GameKey.Right);
                bool dl = InputManager.OnDoubleClicked(GameKey.Left);
                bool dr = InputManager.OnDoubleClicked(GameKey.Right);
                bool lor = l || r;
                bool dlor = dl || dr;
                bool MTI = this.MoveType == MoveType.Idle;
                bool MTA = this.MoveType == MoveType.Attack;
                bool MTH = this.MoveType == MoveType.Hit;
                bool STS = this.StateType == StateType.Stand;
                bool STC = this.StateType == StateType.Crouch;
                bool STA = this.StateType == StateType.Air;
                bool STL = this.StateType == StateType.Down;
                if (STS && MTI && dlor) return State.GetState<CommonState.RunningBegin>();
                if (MTI && InputManager.OnPressed(GameKey.WeaponAttack1)) return State.GetState<BladeArt.TestAttack>();
                switch (this.CurrState.ID)
                {
                    case StateRef.IdleStand:
                        if (d) return State.GetState<CommonState.StandToCrouch>();
                        else if (u) return State.GetState<CommonState.IdleJumpingBegin>();
                        else if (lor) return State.GetState<CommonState.StandWalkingBegin>();
                        else break;
                    case StateRef.StandToCrouch:
                        if (!d) return State.GetState<CommonState.CrouchToStand>();
                        else break;
                    case StateRef.IdleCrouch:
                        if (!d) return State.GetState<CommonState.CrouchToStand>();
                        else if (lor) return State.GetState<CommonState.CrouchWalkingBegin>();
                        else break;
                    case StateRef.CrouchToStand:
                        if (d) return State.GetState<CommonState.StandToCrouch>();
                        else break;
                    case StateRef.StandWalkingBegin:
                    case StateRef.StandWalkingLoop:
                        if (u) return State.GetState<CommonState.WalkJumpingBegin>();
                        else if (d) return State.GetState<CommonState.StandToCrouch>();
                        else if (!lor) return State.GetState<CommonState.StandWalkingEnd>();
                        else break;
                    case StateRef.StandWalkingEnd:
                        if (u) return State.GetState<CommonState.WalkJumpingBegin>();
                        else if (d) return State.GetState<CommonState.StandToCrouch>();
                        else if (lor) return State.GetState<CommonState.StandWalkingBegin>();
                        else break;
                    case StateRef.CrouchWalkingBegin:
                    case StateRef.CrouchWalkingLoop:
                        if (!d) return State.GetState<CommonState.CrouchToStand>();
                        else if (!lor) return State.GetState<CommonState.CrouchWalkingEnd>();
                        else break;
                    case StateRef.CrouchWalkingEnd:
                        if (!d) return State.GetState<CommonState.CrouchToStand>();
                        else if (lor) return State.GetState<CommonState.CrouchWalkingBegin>();
                        else break;
                    case StateRef.RunningBegin:
                    case StateRef.RunningLoop:
                        if (u) return State.GetState<CommonState.RunJumpingBegin>();
                        else if (!lor) return State.GetState<CommonState.RunningEnd>();
                        else break;
                    case StateRef.RunningEnd:
                        if (u) return State.GetState<CommonState.RunJumpingBegin>();
                        else break;
                    case StateRef.IdleJumpingEnd:
                        if (u && !lor) return State.GetState<CommonState.IdleJumpingBegin>();
                        else if (u && lor) return State.GetState<CommonState.WalkJumpingBegin>();
                        else if (u && dlor) return State.GetState<CommonState.RunJumpingBegin>();
                        else break;
                    case StateRef.WalkJumpingEnd:
                        if (u && !lor) return State.GetState<CommonState.IdleJumpingBegin>();
                        else if (u && lor) return State.GetState<CommonState.WalkJumpingBegin>();
                        else if (u && dlor) return State.GetState<CommonState.RunJumpingBegin>();
                        else break;
                    case StateRef.RunJumpingEnd:
                        if (lor) return State.GetState<CommonState.RunningBegin>();
                        else break;
                }
            }
            return null;
        }
    }
}
