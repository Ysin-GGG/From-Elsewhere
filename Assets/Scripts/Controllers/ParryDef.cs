using Godot;
using LOI.Gameplay;
using LOI.States;
using static LOI.LegendOfInfinity;

namespace LOI.Controllers
{
    /// <summary> 当身判定控制器 </summary>
    public class ParryDef
    {
        public HitFlag HitFlag { get; set; }
        /// <summary> 击中后，【攻击实体】的状态跳转<para>默认为null（不跳转）</para> </summary>
        public State SelfState { get; set; }
        /// <summary> 击中后，【目标实体】的状态跳转<para>默认为null（系统自动跳转）</para> </summary>
        public State TargetState { get; set; }

        private Entity owner = null;

        public ParryDef(Entity owner, Rect2 area)
        {
            this.owner = owner;
            this.owner.ReversalController = this;
        }

        public void Check(Entity target)
        {
            bool allow = (this.HitFlag & target.HitController.HitFlag) > 0;
        }

        public void Apply(Entity target) { }
    }
}
