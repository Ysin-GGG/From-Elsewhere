using Godot;
using LOI.Controllers;
using LOI.DataStructures;
using LOI.Manager;
using LOI.Reference;
using LOI.States;
using LOI.UI.Elements;
using LOI.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using static LOI.LegendOfInfinity;

namespace LOI.Gameplay
{
    public abstract class Entity
    {
        public int Level { get; set; } = 1;
        public int Facing { get => field; set { if ((value = value < 0 ? -1 : 1) == -field) this.BodyCollision.ApplyScale(new Vector2(-1, 1)); field = value;   } }
        public bool IsAlive { get; set; } = true;
        public bool IsEnforcing { get; set; }
        public bool IsKnockedDown => this.StateType == StateType.Down && this.MoveType == MoveType.Hit && !this.Ctrl;
        public virtual bool IsFriendly { get; set; } = false;
        public virtual bool IsHostile { get; set; } = false;
        public Vector2 Position { get => this.BodyCollision.Position; set => this.SetPosition(value); }
        public Vector2 Velocity { get => this.BodyCollision.Velocity; set => this.SetVelocity(value); }
        public float PosX { get => this.BodyCollision.Position.X; set => this.SetPosition(value, null); }
        public float PosY { get => this.BodyCollision.Position.Y; set => this.SetPosition(null, value); }
        public float VelX { get => this.BodyCollision.Velocity.X; set => this.SetVelocity(value, null); }
        public float VelY { get => this.BodyCollision.Velocity.Y; set => this.SetVelocity(null, value); }
        public Vector2 PrePosition { get => this.prePosition; private set => this.prePosition = value; }
        public Vector2 PreVelocity { get => this.preVelocity; private set => this.preVelocity = value; }
        public float PrePosX { get => this.prePosition.X; private set => this.prePosition.X = value; }
        public float PrePosY { get => this.prePosition.Y; private set => this.prePosition.Y = value; }
        public float PreVelX { get => this.preVelocity.X; private set => this.preVelocity.X = value; }
        public float PreVelY { get => this.preVelocity.Y; private set => this.preVelocity.Y = value; }
        public float[] Armor { get; } = new float[3] { 0f, 0f, 0f };
        public int StateTime { get; private set; } = 0;
        public int HitStopTime { get; set; } = 0;
        public int Poise { get; set; } = 0;
        public Entity Owner { get; protected set; } = null;
        public State PreState { get; private set; } = null;
        public State CurrState { get => this.state; set => this.ChangeState(value); }
        public PhysicsType PhysicsType => this.pt;
        public StateType StateType => this.st;
        public MoveType MoveType => this.mt;
        public bool Ctrl => this.state?.Ctrl ?? false;
        /// <summary> 无敌时间（帧）<para>每帧递减1，直至归零，为负数时则会永久生效直至手动调零</para>实体处于无敌期间，其生命值不会减少，同时也不会受到任何攻击 </summary>
        public int InvincibleTime { get; set; }
        /// <summary> 体力值/血条 <para>判定实体是否存活的根本条件，归零时进行死亡判定，若判定失败保留1点体力，否则宣告死亡</para>体力值不会溢出，即不会超过最大体力值 </summary>
        public double Life { get => field.Ceiling(); set { if ((field = LazyMath.Clamp(value, 0d, this.Stats.MaxLife)) < double.Epsilon) { this.Kill(); field = this.IsAlive.ToInt(); } else this.IsAlive = true; } }
        /// <summary> 气力值/蓝条 <para>用于释放技能或特殊攻击的必要资源，会自动持续恢复，也可以通过攻击或受击恢复</para>气力值不会溢出，即不会超过最大气力值 </summary>
        public double Mana { get => field.Ceiling(); set => field = LazyMath.Clamp(value, 0d, this.Stats.MaxMana); }
        public HitDef HitController { get; internal set; }
        public ParryDef ReversalController { get; internal set; }
        /// <summary> 对象编号 </summary>
        public int ID { get => this.BodyCollision.GetMeta("PoolID").AsInt32(); private set => this.BodyCollision.SetMeta("PoolID", value); }
        public HashSet<Entity> TargetCatch { get; }
        public event Action<Entity> Updated;
        public event Action<DamageData, Entity> Hit; // Self Target
        public event Action<DamageData, Entity> Damaged; // Attacker Self
        public event Action<DamageData, Entity> Died; // Attacker Self

        public abstract StatisticCollection Stats { get; }

        private State state;
        private bool stateInit;
        private int[] threatLevel;
        private int[] threatInterval;
        private bool[] threatStatus;
        private Vector2 prePosition;
        private Vector2 preVelocity;
        private PhysicsType pt;
        private StateType st;
        private MoveType mt;

        private CharacterBody2D BodyCollision;
        private Area2D HitCollision;
        private Area2D ParryCollision;

        private ObjPool<CollisionShape2D> bodyCollisions;
        private ObjPool<CollisionShape2D> hitCollisions;
        private ObjPool<CollisionShape2D> parryCollisions;

        private void SetPosition(Vector2 pos)
        {
            this.prePosition = this.BodyCollision.Position;
            this.BodyCollision.Position = pos;
        }

        private void SetPosition(float? x, float? y) => this.SetPosition(new Vector2(x ?? this.BodyCollision.Position.X, y ?? this.BodyCollision.Position.Y));

        private void SetVelocity(Vector2 vel)
        {
            this.preVelocity = this.BodyCollision.Velocity;
            this.BodyCollision.Velocity = vel;
        }

        private void SetVelocity(float? x, float? y) => this.SetVelocity(new Vector2(x ?? this.BodyCollision.Velocity.X, y ?? this.BodyCollision.Velocity.Y));

        public Entity(CharacterBody2D obj, int id)
        {
            this.BodyCollision = obj;
            this.Facing = 1;
            this.PreState = this.state = State.GetState<CommonState.IdleStand>();
            this.Life = this.Stats.MaxLife;
            this.HitCollision = this.BodyCollision.GetNode<Area2D>(nameof(this.HitCollision));
            this.ParryCollision = this.BodyCollision.GetNode<Area2D>(nameof(this.ParryCollision));
            this.bodyCollisions = this.NewCollisionPool(this.BodyCollision, new Color(0f, 0f, 1f, .5f));
            this.hitCollisions = this.NewCollisionPool(this.HitCollision, new Color(1f, 0f, 0f, .5f));
            this.parryCollisions = this.NewCollisionPool(this.ParryCollision, new Color(1f, 0f, 0f, .5f));
            this.ID = id;
            this.TargetCatch = new HashSet<Entity>();
            this.ChangeState(State.GetState<CommonState.IdleStand>());
            this.InitThreat();
        }

        public double Damage(DamageData source, out bool crit)
        {
            Entity ent = source.Owner;
            int delta_Lvl = ent.Level - this.Level;
            double delta_AnD = ent.Stats.Attack - this.Stats.Defense;// source.Owner[StatisticRef.Attr_Attack] - this[StatisticRef.Attr_Defense];
            double delta_PnR = 0f;
            double crit_chance = ent.Stats.CritChance;// source.Owner[StatisticRef.Critical_Chance];
            double crit_damage = ent.Stats.CritDamage;//source.Owner[StatisticRef.Critical_Damage];
            double damage = source.Extra + source.Value * ent.Stats.Attack;// source.Owner[StatisticRef.Attr_Attack];

            if (source.IsPrimaryDamage) delta_PnR += ent.Stats.PrimaryPower - this.Stats.PrimaryResistance;// (double)source.Owner[StatisticRef.Power_Primary] - (double)this[StatisticRef.Resistance_Primary];
            if (source.IsElementalDamage) delta_PnR += ent.Stats.ElementalPower - this.Stats.ElementalResistance;// (double)source.Owner[StatisticRef.Power_Elemental] - (double)this[StatisticRef.Resistance_Elemental];
            if (source.IsSlashDamage) delta_PnR += ent.Stats.SlashPower - this.Stats.SlashResistance;// (double)source.Owner[StatisticRef.Power_Slash] - (double)this[StatisticRef.Resistance_Slash];
            if (source.IsBluntDamage) delta_PnR += ent.Stats.BluntPower - this.Stats.BluntResistance;// (double)source.Owner[StatisticRef.Power_Blunt] - (double)this[StatisticRef.Resistance_Blunt];
            if (source.IsPierceDamage) delta_PnR += ent.Stats.PiercePower - this.Stats.PierceResistance;// (double)source.Owner[StatisticRef.Power_Pierce] - (double)this[StatisticRef.Resistance_Pierce];
            if (source.IsSpellDamage) delta_PnR += ent.Stats.SpellPower - this.Stats.SpellResistance;// (double)source.Owner[StatisticRef.Power_Spell] - (double)this[StatisticRef.Resistance_Spell];
            if (source.IsThermoDamage) delta_PnR += ent.Stats.ThermoPower - this.Stats.ThermoPower;// (double)source.Owner[StatisticRef.Power_Thermo] - (double)this[StatisticRef.Resistance_Thermo];
            if (source.IsHydroDamage) delta_PnR += ent.Stats.HydroPower - this.Stats.HydroResistance;// (double)source.Owner[StatisticRef.Power_Hydro] - (double)this[StatisticRef.Resistance_Hydro];
            if (source.IsCyroDamage) delta_PnR += ent.Stats.CyroPower - this.Stats.CyroResistance;// (double)source.Owner[StatisticRef.Power_Cyro] - (double)this[StatisticRef.Resistance_Cyro];
            if (source.IsBrontoDamage) delta_PnR += ent.Stats.BrontoPower - this.Stats.BrontoResistance;// (double)source.Owner[StatisticRef.Power_Bronto] - (double)this[StatisticRef.Resistance_Bronto];
            if (source.IsAeroDamage) delta_PnR += ent.Stats.AeroPower - this.Stats.AeroResistance;// (double)source.Owner[StatisticRef.Power_Aero] - (double)this[StatisticRef.Resistance_Aero];
            if (source.IsPetroDamage) delta_PnR += ent.Stats.PetroPower - this.Stats.PetroResistance;// (double)source.Owner[StatisticRef.Power_Petro] - (double)this[StatisticRef.Resistance_Petro];

            if (ent.PreHit(source, this, ref damage, ref delta_Lvl, ref delta_AnD, ref delta_PnR, ref crit_chance, ref crit_damage) && this.PreDamage(source, ref damage, ref delta_Lvl, ref delta_AnD, ref delta_PnR, ref crit_chance, ref crit_damage))
            {
                double modify_Lvl = Rand.NextDouble(.75 / (1 - delta_Lvl.Min(0) * 0.01), 1.25 + delta_Lvl.Max(0) * 0.01).Clamp(0.5, 1.5); // 随机修正值（等级&浮动）
                double modify_AnD = Math.Log10(Math.Log10(delta_AnD.Abs() + 10)) * 2 + 1; // 面板修正值（攻击&防御）
                double modify_PnR = Math.Sqrt(Math.Sqrt(delta_PnR * delta_PnR + 1) + delta_PnR); // 属性修正值（强度&抗性）
                //已弃用：double modify_AnD = Math.Log10(1000 + delta_AnD * delta_AnD) - 2; // 面板修正值（攻击&防御）
                //已弃用：double modify_PnR = Math.Sqrt(Math.Sqrt(delta_PnR * delta_PnR + 1) + delta_PnR); // 属性修正值（强度&抗性）
                damage = damage * modify_Lvl * (delta_AnD > 0 ? modify_PnR * modify_AnD : modify_PnR / modify_AnD);
                if (crit = Rand.Instance.NextBool(crit_chance))
                    if (crit_damage > 0f) damage *= 1d + crit_damage;
                    else damage /= 1d - crit_damage;
                this.Threat(source, damage, crit);
                return damage;
            }
            crit = Rand.Instance.NextBool(crit_chance);
            return 0;
        }

        public CombatTextBox DamageDirectly(DamageData source, double damage, bool crit, bool kill = true, bool showCombatText = true)
        {
            this.IsAlive = !kill;
            this.Life -= damage;
            this.PostDamage(source, damage, crit);
            source.Owner.PostHit(source, this, damage, crit);
            if (showCombatText) return UIManager.NewCombatText(damage, crit, this.Position);
            return null;
        }

        public void RestoreLife(double amount)
        {
            this.Life += amount;
        }

        public void RestoreLife(double amount, int time)
        {
            this.Life += amount;
        }

        public void RestoreMana(double amount)
        {
            this.Mana += amount;
        }

        public void RestoreMana(double amount, int time)
        {
            this.Mana += amount;
        }

        public void Kill()
        {
            if (!(this.IsAlive = !this.PreKill())) this.PostKill();
        }

        public void FreeState()
        {
            this.ChangeState(State.GetState<CommonState.IdleStand>());
        }

        public void ChangeState(State state)
        {
            if (state != null && this.PreChangeState(this, ref state) & this.CurrState.InvokeCall(this, ref state))
            {
                this.StateTime = 0;
                this.PreState = this.state;
                this.state = state;
                this.stateInit = true;
                this.HitController = null;
                this.ReversalController = null;
                if (this.state == null)
                {
                    this.pt = PhysicsType.Undefined;
                    this.st = StateType.Undefined;
                    this.mt = MoveType.Undefined;
                }
                else
                {
                    this.pt = this.state.PT == PhysicsType.Inherited ? this.pt : this.state.PT;
                    this.st = this.state.ST == StateType.Inherited ? this.st : this.state.ST;
                    this.mt = this.state.MT == MoveType.Inherited ? this.mt : this.state.MT;
                }
                this.PostChangeState(this, this.PreState, this.CurrState);
                this.PreState.InvokeCall(this, this.CurrState);
            }
        }

        internal void Threat(DamageData source, double damage, bool crit)
        {
            int modifier = damage > this.Stats.MaxLife * .01d ? 1 : 2;
            bool[] flags = { 
                source.IsSlashDamage, source.IsBluntDamage, source.IsPierceDamage, source.IsSpellDamage,
                source.IsThermoDamage, source.IsHydroDamage, source.IsCyroDamage, source.IsBrontoDamage, source.IsAeroDamage, source.IsPetroDamage
            };
            for (int i = 0; i < flags.Length; i++)
                if (flags[i])
                    if (i < 4)
                    {

                    }
                    else
                    {
                        if (Rand.NextBool(1d - this.threatInterval[i] / 3 / 20d)) this.threatLevel[i] += (Rand.NextInt(2, 5) + crit.ToInt()) / modifier;
                        if (Rand.NextBool((this.threatLevel[i] - 50d).Max(0) / 50d)) this.threatStatus[i] = true;
                    }
        }

        internal bool MoveAndCollide(Vector2 motion)
        {
            motion *= new Vector2(this.Facing, 1) * (float)(double)this.Stats.MovementSpeed;
            this.PrePosition = this.BodyCollision.Position;
            KinematicCollision2D kc2d = this.BodyCollision.MoveAndCollide(motion, false, 0f, false);
            CollisionObject2D co2d = kc2d?.GetCollider() as CollisionObject2D;
            if (kc2d != null)
            {
                if (motion.Y != 0 || (!co2d?.GetCollisionLayerValue(18) ?? false))
                {
                    this.OnCollided(kc2d, co2d, motion);
                    this.CurrState.InvokeCall(this, kc2d, co2d, motion);
                    return true;
                }
                if (kc2d.GetAngle() <= this.BodyCollision.FloorMaxAngle)
                {
                    this.Position = this.PrePosition + motion.Slide(kc2d.GetNormal());
                    //this.EntityObj.MoveAndCollide(motion.Slide(kc2d.GetNormal()), false, 0f, false);
                    //Debug.Print("upslope");
                }
            }
            else if (pt == PhysicsType.Ground)
            {
                KinematicCollision2D kc2d2 = this.BodyCollision.MoveAndCollide(Vector2.Down * LazyMath.g, true, 0, false);
                kc2d = this.BodyCollision.MoveAndCollide(Vector2.Down * 2f, true, 0.08f, false);
                co2d = kc2d2?.GetCollider() as CollisionObject2D;
                if (kc2d == null && (kc2d2 == null || (!co2d?.GetCollisionLayerValue(18) ?? false))) // 使用技能时禁用
                {
                    if (this.VelX == 0f) this.ChangeState(State.GetState<CommonState.IdleJumpingDown>());
                    else switch (this.CurrState.ID)
                        {
                            case StateRef.RunningBegin:
                            case StateRef.RunningLoop:
                            case StateRef.RunningEnd:
                                this.ChangeState(State.GetState<CommonState.RunJumpingDown>());
                                break;
                            default:
                                this.ChangeState(State.GetState<CommonState.WalkJumpingDown>());
                                break;
                        }
                    return false;
                }
                this.BodyCollision.MoveAndCollide(Vector2.Down * LazyMath.g, false, 0, false);
                //Debug.Print("downslope");
            }
            return false;
        }

        internal void UpdateSelf()
        {
            if (GameManager.Instance.trail is Line2D trail && trail != null)
            {
                trail.RemovePoint(0);
                trail.AddPoint(this.BodyCollision.Position);
                trail = trail.GetChild(0) as Line2D;
                trail?.RemovePoint(0);
                trail?.AddPoint(this.BodyCollision.Position);
            }
            if (this.stateInit) this.CurrState.Update(this, this.stateInit = false); // 初始化状态
            if (this.HitStopTime > 0) // 命中硬直
            {
                this.CurrState.Update(this, true);
                this.PosX += (1 - this.HitStopTime-- % 3) * 3;
                //Debug.Print(this.BodyCollision.Name + this.HitStopTime);
            }
            else // 常规更新
            {
                PhysicsType pt = this.PhysicsType;
                StateType st = this.StateType;
                bool hit = this.MoveType == MoveType.Hit;
                if (pt == PhysicsType.Ground) this.Velocity *= new Vector2(this.VelX.Abs() < hit.ToFloat(.5f, .1f) ? 0f : 1f - hit.ToFloat(.1f, .2f) * (float)this.StateType, 0f);
                else if (pt == PhysicsType.Air && st == StateType.Air) this.VelY += LazyMath.g * LazyMath.phi1 * .1f; // 实则应为+
                this.CurrState.Update(this, true);
                this.StateTime++;
                if (this.VelX != 0f || this.VelY != 0f) this.MoveAndCollide(this.Velocity);
            }

            if (this.HitController != null) // 攻击判定
            {
                Node2D[] nodes = this.HitCollision.GetOverlappingBodies().ToArray();
                List<Entity> entities = new List<Entity>();
                foreach (var enemy in GameManager.Enemy) entities.Add(enemy);
                foreach (var character in GameManager.Character) entities.Add(character);
                this.HitController.CheckAll(entities);
            }
            else if (this.hitCollisions.Count > 0)
            {
                this.hitCollisions.Clear();
            } // 无攻击时禁用判定

            if (this.IsKnockedDown && this.Armor[2] < 1f) this.Armor[2] = 1f; // 倒地时常驻金刚身
            else if (this.Armor[2] > 0f) this.Armor[2]--; // 金刚身持续时间递减
            else if (this.Armor[1] > 0f) { this.Armor[2] = 0; this.Armor[1]--; } // 蓝刚身持续时间递减
            else if (this.Armor[0] > 0f) { this.Armor[1] = 0; this.Armor[0]--; } // 白刚身持续时间递减
            else this.Armor[2] = this.Armor[1] = this.Armor[0] = 0f; // 无刚身数据还原
            this.UpdateThreat(); // 元素异常状态相关更新
            this.ChangeState(this.CommonState()); // 公共状态更新
            this.Update(); // 实体相关更新（接口）
            this.UpdateState(this.CurrState.ID, this.StateTime); // 状态相关更新
            if (GameManager.GameTime % 60 == 0) this.Mana++; // 气力值持续恢复
        }

        internal void InitThreat()
        {
            this.threatInterval = new int[10];
            this.threatLevel = new int[10];
            this.threatStatus = new bool[10];
            Factor hydroEffect = new IndependentFactor("Threat.Hydro.Effect", () => this.threatStatus[4] ? 1d : 1d + this.threatLevel[4] * .010d);
            //this[StatisticRef.Attr_Attack].AddModifier(hydroEffect);

            // Osmosis
            Factor hydroStatus1 = new IndependentFactor("Threat.Hydro.Status", () => this.threatStatus[4] ? 1d + this.threatLevel[4] * .010d : 1d);
            Factor hydroStatus2 = new IndependentFactor("Threat.Hydro.Status", () => this.threatStatus[4] ? 1d + this.threatLevel[4] * .005d : 1d);
            /*
            this[StatisticRef.Power_Direct].AddModifier(hydroStatus1);
            this[StatisticRef.Resistance_Hydro].AddModifier(hydroStatus2);
            this[StatisticRef.Resistance_Indirect].AddModifier(hydroStatus2);
            */
        }

        internal void UpdateThreat()
        {
            for (int i = 0; i < this.threatInterval.Length; i++)
                this.threatInterval[i]++;
            // Bleeding
            // Contusion
            // Puncture
            if (this.threatStatus[3]) // ...
            {
                Projectile blast = new Projectile(this);
            }
            // Osmosis
            if (GameManager.GameTime % 60 == 0) this.threatLevel[4] -= this.threatStatus[4] ? 4 : 2;
            // ...
            // Paralysis
            // ...
            // Cataclasis
            for (int i = 0; i < this.threatLevel.Length; i++)
            {
                if (this.threatLevel[i] < 0) this.threatLevel[i] = 0;
                if (this.threatLevel[i] > 100) this.threatLevel[i] = 100;
                if (this.threatLevel[i] == 0 && this.threatStatus[i]) this.threatStatus[i] = false;
            }
        }

        internal virtual State CommonState()
        {
            // todo
            return null;
        }

        protected virtual void OnCollided(KinematicCollision2D kc2d, CollisionObject2D co2d, Vector2 motion) { }
        protected virtual void Update() { }
        protected virtual void UpdateState(int stateno, int time) { }
        /// <summary> 跳转到当前状态-预处理 </summary>
        /// <param name="entity">实体</param>
        /// <param name="nextState">状态</param>
        /// <returns>是否允许跳转状态（默认true）</returns>
        protected virtual bool PreChangeState(Entity entity, ref State nextState) => true;
        /// <summary> 跳转到当前状态-后处理 </summary>
        /// <param name="entity">实体</param>
        /// <param name="preState">前一个状态</param>
        /// <param name="currState">当前状态</param>
        protected virtual void PostChangeState(Entity entity, State preState, State currState) { }
        protected virtual bool PreKill() => !this.IsAlive;
        protected virtual void PostKill() { }
        protected virtual bool PreHit(DamageData source, Entity target, ref double damage, ref int delta_Lvl, ref double delta_AnD, ref double delta_PnR, ref double crit_chance, ref double crit_damage) => true;
        protected virtual void PostHit(DamageData source, Entity target, double damage, bool crit) { }
        protected virtual bool PreDamage(DamageData source, ref double damage, ref int delta_Lvl, ref double delta_AnD, ref double delta_PnR, ref double crit_chance, ref double crit_damage) => true;
        protected virtual void PostDamage(DamageData source, double damage, bool crit) { }

        private ObjPool<CollisionShape2D> NewCollisionPool(CollisionObject2D parent, Color color)
        {
            ObjPool<CollisionShape2D> pool = new((id) =>
            {
                CollisionShape2D obj = new CollisionShape2D();
                obj.Name = "Collision " + id;
                obj.DebugColor = color;
                obj.Disabled = true;
                obj.Visible = false;
                obj.SetProcess(false);
                obj.SetPhysicsProcess(false);
                parent.AddChild(obj);
                return obj;
            }, 4);
            pool.OnRequest += (c) => { c.Disabled = false; c.Visible = true; c.SetProcess(true); c.SetPhysicsProcess(true); };
            pool.OnRelease += (c) => { c.Disabled = true; c.Visible = false; c.SetProcess(false); c.SetPhysicsProcess(false); };
            return pool;
        }

        public bool CheckCollision(Entity target)
        {
            if (target.InvincibleTime > 0) return false;
            if (!this.HitCollision.GetOverlappingBodies().Contains(target.BodyCollision)) return false;
            //if (target.parryCollisions) return this.HitController.Check(target);
            return true;
        }

        public void SetBodyCollision(params Rect2[] boxes) => this.SetCollision(this.bodyCollisions, boxes);

        public void SetBodyCollision(params Vector4[] borders) => this.SetCollision(this.bodyCollisions, borders);

        public void SetHitCollision(params Rect2[] boxes) => this.SetCollision(this.hitCollisions, boxes);

        public void SetHitCollision(params Vector4[] borders) => this.SetCollision(this.hitCollisions, borders);

        public void SetParryCollision(params Rect2[] boxes) => this.SetCollision(this.parryCollisions, boxes);

        public void SetParryCollision(params Vector4[] borders) => this.SetCollision(this.parryCollisions, borders);

        /// <summary> 重设碰撞框（边界） </summary>
        /// <remarks>每个 Vector4 会被转换为一个 Rect2 以定义碰撞区域。确保每个 Vector4 中的坐标有效，并代表预期的碰撞区域</remarks>
        /// <param name="borders">一个 Vector4 数组，每个 Vector4 表示一个边界框，包含左（X）、上（Y）、右（Z）、下（W）坐标。</param>
        private void SetCollision(ObjPool<CollisionShape2D> pool, params Vector4[] borders)
        {
            pool.Clear();
            if (borders?.Length > 0)
                foreach (var border in borders)
                {
                    CollisionShape2D obj = pool.Request();
                    obj.Position = new Vector2(border.Z + border.X, border.W + border.Y) / 2f;
                    obj.Shape = new RectangleShape2D() { Size = new Vector2(border.Z - border.X, border.W - border.Y) };
                }
        }

        /// <summary>
        /// Clears the specified object pool and creates new collision shapes based on the provided rectangular regions.
        /// </summary>
        /// <remarks>Use this method to dynamically update collision shapes in a 2D environment. The pool
        /// must be initialized before calling this method. Existing shapes in the pool are cleared before new shapes
        /// are created.</remarks>
        /// <param name="pool">The object pool used to manage and reuse instances of <see cref="CollisionShape2D"/>.</param>
        /// <param name="boxes">An optional array of <see cref="Rect2"/> values that define the positions and sizes of the collision shapes
        /// to be created. If no rectangles are provided, no shapes are added.</param>
        private void SetCollision(ObjPool<CollisionShape2D> pool, params Rect2[] boxes)
        {
            pool.Clear();
            if (boxes?.Length > 0)
                foreach (var box in boxes)
                {
                    CollisionShape2D obj = pool.Request();
                    obj.Position = box.Position;
                    obj.Shape = new RectangleShape2D() { Size = box.Size };
                }
        }

        public void Spectate(Camera2D camera) => camera.Reparent(this.BodyCollision);

        public static implicit operator CharacterBody2D(Entity entity) => entity.BodyCollision;
    }
}
