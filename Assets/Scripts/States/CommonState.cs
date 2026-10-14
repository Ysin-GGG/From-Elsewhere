using Godot;
using LOI.Gameplay;
using System.Diagnostics;
using static LOI.LegendOfInfinity;

namespace LOI.States
{
    public static class CommonState
    {

        public class IdleStand : State.Model.Stand;

        public class IdleCrouch : State.Model.Crouch;

        public class StandToCrouch : State.Model.Crouch
        {
            protected override void Update(Entity entity, int stateNo, int stateTime)
            {
                if (stateTime >= 4) entity.ChangeState(State.GetState<CommonState.IdleCrouch>());
            }
        }

        public class CrouchToStand : State.Model.Stand
        {
            protected override void Update(Entity entity, int stateNo, int stateTime)
            {
                if (stateTime >= 4) entity.ChangeState(State.GetState<CommonState.IdleStand>());
            }
        }

        public class AirToLie : State.Model.Lie
        {
            protected override void FirstUpdate(Entity entity, int stateNo)
            {
                entity.ChangeState(entity.VelY > 32f ? State.GetState<CommonState.LieBounce>() : State.GetState<CommonState.Lying>());
            }

            protected override void OnCollided(Entity entity, KinematicCollision2D kc2d, CollisionObject2D co2d, Vector2 motion)
            {
                if (co2d is StaticBody2D sb2d) entity.ChangeState(State.GetState<CommonState.Lying>());
            }
        }

        public class LieBounce : State.Model.Lie
        {
            public override PhysicsType PT => PhysicsType.Air;

            protected override void FirstUpdate(Entity entity, int stateNo)
            {
                Debug.Print(new Vector2(entity.VelY, entity.PreVelY).ToString());
                entity.VelY *= -.618f;
            }

            protected override void OnCollided(Entity entity, KinematicCollision2D kc2d, CollisionObject2D co2d, Vector2 motion)
            {
                if (co2d is StaticBody2D sb2d) entity.ChangeState(State.GetState<CommonState.Lying>());
            }
        }

        public class Lying : State.Model.Lie
        {
            protected override void Update(Entity entity, int stateNo, int stateTime)
            {
                if (stateTime >= 16) entity.ChangeState(State.GetState<CommonState.CrouchToStand>());
            }
        }

        public class StandHighHitStop() : State.Model.HitStop(State.GetState<CommonState.StandHighHitKnockback>());
        public class StandHighHitKnockback() : State.Model.HitKnockback(State.GetState<CommonState.IdleStand>());

        public class CrouchHitStop() : State.Model.HitStop(State.GetState<CommonState.CrouchHitKnockback>());
        public class StandLowHitKnockback() : State.Model.HitKnockback(State.GetState<CommonState.IdleStand>());

        public class StandLowHitStop() : State.Model.HitStop(State.GetState<CommonState.StandLowHitKnockback>());
        public class CrouchHitKnockback() : State.Model.HitKnockback(State.GetState<CommonState.IdleCrouch>());

        public class AirHitStop() : State.Model.HitStop(State.GetState<CommonState.AirHitUp>());
        public class AirHitUp() : State.Model.HitKnockback(State.GetState<CommonState.AirToLie>()) { protected override void Update(Entity entity, int stateNo, int stateTime) { if (entity.PhysicsType == PhysicsType.Air && entity.VelY > 0f) entity.ChangeState(State.GetState<CommonState.AirHitDown>()); } }
        public class AirHitDown() : State.Model.HitKnockback(State.GetState<CommonState.AirToLie>());

        public class CrouchWalkingBegin() : State.Model.MoveBegin(State.GetState<CommonState.CrouchWalkingLoop>(), 0.5f, 3);
        public class CrouchWalkingLoop() : State.Model.MoveLoop(State.GetState<CommonState.CrouchWalkingEnd>(), 0.5f);
        public class CrouchWalkingEnd() : State.Model.MoveEnd(State.GetState<CommonState.IdleCrouch>(), 4);

        public class StandWalkingBegin() : State.Model.MoveBegin(State.GetState<CommonState.StandWalkingLoop>(), 1.0f, 4);
        public class StandWalkingLoop() : State.Model.MoveLoop(State.GetState<CommonState.StandWalkingEnd>(), 1.0f);
        public class StandWalkingEnd() : State.Model.MoveEnd(State.GetState<CommonState.IdleStand>(), 4);

        public class RunningBegin() : State.Model.MoveBegin(State.GetState<CommonState.RunningLoop>(), 2.0f, 5);
        public class RunningLoop() : State.Model.MoveLoop(State.GetState<CommonState.RunningEnd>(), 2.0f);
        public class RunningEnd() : State.Model.MoveEnd(State.GetState<CommonState.IdleStand>(), 6);

        public class IdleJumpingBegin() : State.Model.JumpBegin(State.GetState<CommonState.IdleJumpingUp>(), 0f, 2);
        public class IdleJumpingUp() : State.Model.JumpUp(State.GetState<CommonState.IdleJumpingDown>(), 1.0f);
        public class IdleJumpingDown() : State.Model.JumpDown(State.GetState<CommonState.IdleJumpingEnd>());
        public class IdleJumpingEnd() : State.Model.JumpEnd(2);

        public class WalkJumpingBegin() : State.Model.JumpBegin(State.GetState<CommonState.WalkJumpingUp>(), 6f, 1);
        public class WalkJumpingUp() : State.Model.JumpUp(State.GetState<CommonState.WalkJumpingDown>(), 0.9f);
        public class WalkJumpingDown() : State.Model.JumpDown(State.GetState<CommonState.WalkJumpingEnd>());
        public class WalkJumpingEnd() : State.Model.JumpEnd(3);

        public class RunJumpingBegin() : State.Model.JumpBegin(State.GetState<CommonState.RunJumpingUp>(), 4f, 4);
        public class RunJumpingUp() : State.Model.JumpUp(State.GetState<CommonState.RunJumpingDown>(), 1.1f);
        public class RunJumpingDown() : State.Model.JumpDown(State.GetState<CommonState.RunJumpingEnd>());
        public class RunJumpingEnd() : State.Model.JumpEnd(4);
    }
}
