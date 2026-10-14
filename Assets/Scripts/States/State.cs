using Godot;
using LOI.DataStructures;
using LOI.Gameplay;
using LOI.Reference;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static LOI.LegendOfInfinity;

namespace LOI.States
{
    public abstract class State(bool ctrl = false, PhysicsType physicsType = PhysicsType.Undefined, StateType stateType = StateType.Undefined, MoveType moveType = MoveType.Undefined) : IComparer<State>, IEqualityComparer<State>
    {
        private Mod ModSource;
        public int ID { get; private set; } // 即将移除？
        public virtual bool Ctrl { get; protected set; } = ctrl;
        /// <summary> 行动类型 </summary>
        public virtual MoveType MT { get; protected set; } = moveType;
        /// <summary> 姿势类型 </summary>
        public virtual StateType ST { get; protected set; } = stateType;
        /// <summary> 物理类型 </summary>
        public virtual PhysicsType PT { get; protected set; } = physicsType;

        private bool valid = false;
        private static Dictionary<int, State> factory = new Dictionary<int, State>(); // 优化为Dictionary<Type, State>
        private static int AutoID = int.MaxValue;

        public static T GetState<T>() where T : State, new()
        {
            Type type = typeof(T);
            T state = new T();
            state.valid = true;
            state.ID = DataReference<StateRef>.GetRef(StateRef.KeyDef + type.Name) ?? State.AutoID--;
            if (State.factory.TryGetValue(state.ID, out State value) && value.GetType() == type && value.valid)
                return value as T;
            else if (State.factory.TryAdd(state.ID, state))
            {
                state.Initialization();
                return state;
            }
            throw new ArgumentException($"Try to register StateID({state.ID}) for State({type.Name}), but a same StateID has been register by State({State.factory[state.ID].GetType().Name}).", "StateID");
        }

        internal void Update(Entity entity, bool common)
        {
            if (common)
            {
                if (entity.HitStopTime == 0) this.Update(entity, entity.CurrState.ID, entity.StateTime);
                this.UpdateAlways(entity, entity.CurrState.ID, entity.StateTime);
            }
            else this.FirstUpdate(entity, entity.CurrState.ID);
        }

        internal bool InvokeCall(Entity ent, ref State state) => this.PreChangeState(ent, ref state);
        internal void InvokeCall(Entity ent, State state) => this.PostChangeState(ent, state);
        internal void InvokeCall(Entity entity, KinematicCollision2D kc2d, CollisionObject2D co2d, Vector2 motion) => this.OnCollided(entity, kc2d, co2d, motion);

        protected virtual void Initialization() { }
        protected virtual void FirstUpdate(Entity entity, int stateNo) { }
        protected virtual void Update(Entity entity, int stateNo, int stateTime) { }
        protected virtual void UpdateAlways(Entity entity, int stateNo, int stateTime) { }
        protected virtual bool PreChangeState(Entity entity, ref State nextState) => true;
        protected virtual void PostChangeState(Entity entity, State currState) { }
        protected virtual void OnCollided(Entity entity, KinematicCollision2D kc2d, CollisionObject2D co2d, Vector2 motion) { }

        public bool Equals(State x, State y) => x.ID == y.ID;

        public int GetHashCode([DisallowNull] State obj) => obj.ID.GetHashCode();

        public int Compare(State x, State y) => (x.ID == y.ID) ? 0 : ((x.ID < y.ID) ? -1 : 1);

        public override string ToString() => $"{this.GetType().Name}({this.ID})";

        //public static implicit operator int(State state) => state?.ID ?? -1;
        public static implicit operator State(int refID) => State.factory.TryGetValue(refID, out State state) ? state : null;
        public static implicit operator State(DataReference<StateRef> reference) => State.factory.TryGetValue(reference, out State state) ? state : null;

        public static class Model
        {
            public abstract class Air() : State(true, PhysicsType.Air, StateType.Air, MoveType.Idle);

            public abstract class Crouch() : State(true, PhysicsType.Ground, StateType.Crouch, MoveType.Idle);

            public abstract class Stand() : State(true, PhysicsType.Ground, StateType.Stand, MoveType.Idle);

            public abstract class Lie() : State(false, PhysicsType.Ground, StateType.Down, MoveType.Hit);

            public abstract class HitStop(State.Model.HitKnockback nextState) : State(false, PhysicsType.Inherited, StateType.Inherited, MoveType.Hit)
            {
                protected override void Update(Entity entity, int stateNo, int stateTime) => entity.ChangeState(nextState);
            }

            public abstract class HitKnockback(State idleState) : State(false, PhysicsType.Inherited, StateType.Inherited, MoveType.Hit)
            {
                protected override void Update(Entity entity, int stateNo, int stateTime)
                {
                    if (entity.PhysicsType == PhysicsType.Ground && entity.VelX == 0f) entity.ChangeState(idleState);
                }

                protected override void OnCollided(Entity entity, KinematicCollision2D kc2d, CollisionObject2D co2d, Vector2 motion)
                {
                    if (co2d is StaticBody2D sb2d && entity.PhysicsType == PhysicsType.Air && entity.VelY >= 0f) entity.ChangeState(idleState);
                }
            }

            public abstract class JumpBegin(State.Model.JumpUp nextState, float velAdd, int endTime) : State.Model.Stand
            {
                protected override void Update(Entity entity, int stateNo, int stateTime)
                {
                    if (stateTime < endTime) entity.VelX += velAdd;
                    else entity.ChangeState(nextState);
                }
            }

            public abstract class JumpUp(State.Model.JumpDown nextState, float velMul) : State.Model.Air
            {
                protected override void FirstUpdate(Entity entity, int stateNo) => entity.VelY = (float)(double)entity.Stats.JumpStrength * velMul * -32f;

                protected override void Update(Entity entity, int stateNo, int stateTime)
                {
                    if (entity.VelY > 0f) entity.ChangeState(nextState);
                }
            }

            public abstract class JumpDown(State.Model.JumpEnd nextState) : State.Model.Air
            {
                protected override void OnCollided(Entity entity, KinematicCollision2D kc2d, CollisionObject2D co2d, Vector2 motion)
                {
                    if (co2d is StaticBody2D sb2d) entity.ChangeState(nextState);
                }
            }

            public abstract class JumpEnd(int endTime) : State.Model.Stand
            {
                protected override void Update(Entity entity, int stateNo, int stateTime)
                {
                    if (stateTime >= endTime) entity.ChangeState(State.GetState<CommonState.IdleStand>());
                }
            }

            public abstract class MoveBegin(State.Model.MoveLoop nextState, float velAdd, int endTime) : State.Model.Stand
            {
                public override StateType ST => nextState.ST;

                protected override void Update(Entity entity, int stateNo, int stateTime)
                {
                    if (stateTime < endTime) entity.VelX += velAdd;
                    else entity.ChangeState(nextState);
                }
            }

            public abstract class MoveLoop(State.Model.MoveEnd nextState, float velMul) : State.Model.Stand
            {
                public override StateType ST => nextState.ST;

                protected override void Update(Entity entity, int stateNo, int stateTime)
                {
                    float velX = velMul * 8f;
                    if (entity.VelX < velX) entity.VelX = velX;
                }
            }

            public abstract class MoveEnd(State idleState, int endTime) : State.Model.Stand
            {
                public override StateType ST => idleState.ST;

                protected override void Update(Entity entity, int stateNo, int stateTime)
                {
                    if (stateTime >= endTime) entity.ChangeState(idleState);
                }
            }
        }
    }
}
