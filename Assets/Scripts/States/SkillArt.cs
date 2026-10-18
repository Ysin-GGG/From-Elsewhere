using Godot;
using LOI.Controllers;
using LOI.Gameplay;
using LOI.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using static LOI.LegendOfInfinity;

namespace LOI.States
{
    public class SkillArt(int mana, int cd) : State, ISkillArt
    {
        public int ManaCost { get; } = mana;
        public int Cooldown { get; } = cd;

        public State GetInstance() => this;

        public static class Blade
        {
            public class FinalSlash() : SkillArt(1000, 120)
            {
                public override bool Ctrl => false;
                public override MoveType MT => MoveType.Attack;
                public override PhysicsType PT => PhysicsType.Ground;
                public override StateType ST => StateType.Stand;
                protected override void Update(Entity entity, int stateNo, int stateTime)
                {
                    entity.SetHitCollision(new Vector4(-800f, -800f, 800f, 800f));
                    if (stateTime == 5)
                    {
                        HitDef hitDef = new HitDef(entity)
                        {
                            Power = 11.45,
                            HitTime = 5,
                            HitImmuneTime = -1,
                            HitStopSelf = 0,
                            HitStopTarget = 60,
                            HitRemainAmount = -1,
                            AffectFriends = false,
                            AffectHostiles = true,
                            IsSlashDamage = true,
                            CanBeBlocked = false,
                            CanBeParried = false,
                            HitFlag = HitFlag.SCA,
                        };
                    }
                    if (stateTime == 10)
                        entity.CurrState = State.GetState<CommonState.IdleStand>();
                }
            }
        }
    }
}
