using Godot;
using LOI.Controllers;
using LOI.DataStructures;
using LOI.Gameplay;
using LOI.Manager;
using LOI.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using static LOI.LegendOfInfinity;

namespace LOI.States
{
    public abstract class BladeArt() : State(false, PhysicsType.Inherited, StateType.Inherited, MoveType.Attack)
    {
        public abstract class PlungingThrust : BladeArt
        {
            public class Thrust : PlungingThrust
            {
            }

            public class Quake : PlungingThrust
            {
            }

            public class Counter : PlungingThrust
            {
            }
        }

        public class TestAttack : BladeArt
        {
            protected override void FirstUpdate(Entity entity, int stateNo)
            {
                entity.SetHitCollision(new Vector4(0f, -80f, 320f, 80f));
                HitDef hitDef = new HitDef(entity)
                {
                    Power = 0,
                    HitTime = 10,
                    HitImmuneTime = -1,
                    HitStopSelf = 0,
                    HitStopTarget = 30,
                    HitRemainAmount = -1,
                    AffectFriends = false,
                    AffectHostiles = true,
                    IsSlashDamage = true,
                    CanBeBlocked = false,
                    CanBeParried = false,
                    HitFlag = HitFlag.SCA,
                };
                hitDef.OnHit = (self, target) =>
                {
                    self.TargetCatch.Add(target);
                    return true;
                };
            }

            protected override void Update(Entity entity, int stateNo, int stateTime)
            {
                entity.Velocity = new Vector2(160f, 0f);
                if (stateTime > 5)
                {
                    entity.VelX = 0f;
                    if (entity.TargetCatch.Count > 0) entity.ChangeState(State.GetState<BladeArt.TestAttack2>());
                    else entity.CurrState = State.GetState<CommonState.IdleStand>();
                }
            }
        }

        public class TestAttack2 : BladeArt
        {
            protected override void FirstUpdate(Entity entity, int stateNo)
            {
                entity.SetHitCollision(Vector4.Zero);
                HitDef hitDef = new HitDef(entity)
                {
                    Power = .32,
                    HitTime = 45,
                    HitImmuneTime = -1,
                    HitStopSelf = 0,
                    HitStopTarget = 8,
                    HitRemainAmount = -1,
                    AffectFriends = false,
                    AffectHostiles = true,
                    IsSlashDamage = true,
                    CanBeBlocked = false,
                    CanBeParried = false,
                    HitFlag = HitFlag.SCA,
                };
            }

            protected override void Update(Entity entity, int stateNo, int stateTime)
            {
                if (stateTime > 60)
                {
                    entity.ChangeState(State.GetState<CommonState.IdleStand>());
                    entity.TargetCatch.Clear();
                }
                else if (stateTime > 15 && entity.HitController.NotNull() && stateTime % 5 == 4)
                {
                    foreach (Entity target in entity.TargetCatch)
                        entity.HitController.Apply(target);
                }
            }
        }

        public abstract class DriveSlash : BladeArt
        {
            public abstract int ManaCost { get; }

            public class I : DriveSlash
            {
                public override int ManaCost => 20;
            }

            public class II : DriveSlash
            {
                public override int ManaCost => 35;
            }

            public class III : DriveSlash
            {
                public override int ManaCost => 55;
            }
        }

        public abstract class FlashIaiSlash : BladeArt
        {
            public class Parry : FlashIaiSlash
            {
                protected override void UpdateAlways(Entity entity, int stateNo, int stateTime)
                {
                    entity.ChangeState(State.GetState<BladeArt.FlashIaiSlash.Attack>());
                }

                protected override bool PreChangeState(Entity entity, ref State nextState)
                {
                    if (entity is Character character && character.Blade_BreathStance)
                        nextState = State.GetState<BladeArt.FlashIaiSlash.Attack>();
                    return true;
                }
            }

            public class Attack : FlashIaiSlash
            {
                protected override void FirstUpdate(Entity entity, int stateNo)
                {
                    if (entity is Character character)
                        character.Blade_BreathStance = true;
                }
            }
        }

        public class SharpBladeDance : BladeArt
        {
            protected override bool PreChangeState(Entity entity, ref State nextState)
                => entity is Character character && character.Blade_BreathStance;
        }
    }
}
