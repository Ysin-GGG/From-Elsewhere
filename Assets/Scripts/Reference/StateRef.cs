using Ref = LOI.DataStructures.DataReference<LOI.Reference.StateRef>;

namespace LOI.Reference
{
    public sealed class StateRef
    {
        private StateRef() { }

        public const string KeyDef = "Vanilla.State.";

        /// <summary> 原地站立 </summary>
        public const int IdleStand = 0;
        /// <summary> 原地蹲伏 </summary>
        public const int IdleCrouch = 10;
        /// <summary> 站立转蹲伏 </summary>
        public const int StandToCrouch = 5;
        /// <summary> 蹲伏转站立 </summary>
        public const int CrouchToStand = 15;
        /// <summary> 站立行走（预备） </summary>
        public const int StandWalkingBegin = 100;
        /// <summary> 站立行走（循环） </summary>
        public const int StandWalkingLoop = 101;
        /// <summary> 站立行走（停止） </summary>
        public const int StandWalkingEnd = 102;
        /// <summary> 站立行走中转身 </summary>
        public const int StandWalkingTurnAround = 105;
        /// <summary> 蹲伏行走（预备） </summary>
        public const int CrouchWalkingBegin = 110;
        /// <summary> 蹲伏行走（循环） </summary>
        public const int CrouchWalkingLoop = 111;
        /// <summary> 蹲伏行走（停止） </summary>
        public const int CrouchWalkingEnd = 112;
        /// <summary> 蹲伏行走中转身 </summary>
        public const int CrouchWalkingTurnAround = 115;
        /// <summary> 奔跑（预备） </summary>
        public const int RunningBegin = 120;
        /// <summary> 奔跑（循环） </summary>
        public const int RunningLoop = 121;
        /// <summary> 奔跑（停止） </summary>
        public const int RunningEnd = 122;
        /// <summary> 奔跑中转身 </summary>
        public const int RunningTurnAround = 125;
        /// <summary> 原地时跳跃（预备） </summary>
        public const int IdleJumpingBegin = 140;
        /// <summary> 原地时跳跃（上升） </summary>
        public const int IdleJumpingUp = 141;
        /// <summary> 原地时跳跃（下落） </summary>
        public const int IdleJumpingDown = 142;
        /// <summary> 原地时跳跃（着地） </summary>
        public const int IdleJumpingEnd = 143;
        /// <summary> 原地跳跃中转身 </summary>
        public const int IdleJumpingTurnAround = 145;
        /// <summary> 行走时跳跃（预备） </summary>
        public const int WalkJumpingBegin = 150;
        /// <summary> 行走时跳跃（上升） </summary>
        public const int WalkJumpingUp = 151;
        /// <summary> 行走时跳跃（下落） </summary>
        public const int WalkJumpingDown = 152;
        /// <summary> 行走时跳跃（着地） </summary>
        public const int WalkJumpingEnd = 153;
        /// <summary> 行走跳跃中转身 </summary>
        public const int WalkJumpingTurnAround = 155;
        /// <summary> 奔跑时跳跃（预备） </summary>
        public const int RunJumpingBegin = 160;
        /// <summary> 奔跑时跳跃（上升） </summary>
        public const int RunJumpingUp = 161;
        /// <summary> 奔跑时跳跃（下落） </summary>
        public const int RunJumpingDown = 162;
        /// <summary> 奔跑时跳跃（着地） </summary>
        public const int RunJumpingEnd = 163;
        /// <summary> 奔跑跳跃中转身 </summary>
        public const int RunJumpingTurnAround = 165;
        /// <summary> 站立上段受击（硬直） </summary>
        public const int StandHighHitStop = 5000;
        /// <summary> 站立上段受击（击退） </summary>
        public const int StandHighHitKnockback = 5001;
        /// <summary> 站立下段受击（硬直） </summary>
        public const int StandLowHitStop = 5010;
        /// <summary> 站立下段受击（击退） </summary>
        public const int StandLowHitKnockback = 5011;
        /// <summary> 蹲伏受击（硬直） </summary>
        public const int CrouchHitStop = 5020;
        /// <summary> 蹲伏受击（击退） </summary>
        public const int CrouchHitKnockback = 5021;
        /// <summary> 空中受击（硬直） </summary>
        public const int AirHitStop = 5050;
        /// <summary> 空中受击（上升） </summary>
        public const int AirHitUp = 5051;
        /// <summary> 空中受击（下落） </summary>
        public const int AirHitDown = 5052;
        /// <summary> 站立转倒地 </summary>
        public const int StandToLie = 5100;
        /// <summary> 蹲伏转倒地 </summary>
        public const int CrouchToLie = 5110;
        /// <summary> 空中转倒地 </summary>
        public const int AirToLie = 5120;
        /// <summary> 倒地弹起 </summary>
        public const int LieBounce = 5150;
        /// <summary> 倒地（循环） </summary>
        public const int Lying = 5200;
        /// <summary> 眩晕（过渡） </summary>
        public const int DizzyTrans = 5900;
        /// <summary> 眩晕（循环） </summary>
        public const int DizzyLoop = 5901;
        /// <summary> 原地起身 </summary>
        public const int IdleGetUp = 6000;
        /// <summary> 向前起身 </summary>
        public const int ForwardGetUp = 6010;
        /// <summary> 向后起身 </summary>
        public const int BackwardGetUp = 6020;
        /// <summary> 空中受身 </summary>
        public const int AirRecover = 6550;
        /// <summary> 地面受身 </summary>
        public const int GroundRecover = 6500;

        /// <summary> 原地站立-映射 </summary>
        public static readonly Ref IdleStandRef = new Ref(StateRef.IdleStand, StateRef.KeyDef + nameof(StateRef.IdleStand));
        /// <summary> 原地蹲伏-映射 </summary>
        public static readonly Ref IdleCrouchRef = new Ref(StateRef.IdleCrouch, StateRef.KeyDef + nameof(StateRef.IdleCrouch));
        /// <summary> 站立转蹲伏-映射 </summary>
        public static readonly Ref StandToCrouchRef = new Ref(StateRef.StandToCrouch, StateRef.KeyDef + nameof(StateRef.StandToCrouch));
        /// <summary> 蹲伏转站立-映射 </summary>
        public static readonly Ref CrouchToStandRef = new Ref(StateRef.CrouchToStand, StateRef.KeyDef + nameof(StateRef.CrouchToStand));
        /// <summary> 站立行走（预备）-映射 </summary>
        public static readonly Ref StandWalkingBeginRef = new Ref(StateRef.StandWalkingBegin, StateRef.KeyDef + nameof(StateRef.StandWalkingBegin));
        /// <summary> 站立行走（循环）-映射 </summary>
        public static readonly Ref StandWalkingLoopRef = new Ref(StateRef.StandWalkingLoop, StateRef.KeyDef + nameof(StateRef.StandWalkingLoop));
        /// <summary> 站立行走（停止）-映射 </summary>
        public static readonly Ref StandWalkingEndRef = new Ref(StateRef.StandWalkingEnd, StateRef.KeyDef + nameof(StateRef.StandWalkingEnd));
        /// <summary> 站立行走中转身-映射 </summary>
        public static readonly Ref StandWalkingTurnAroundRef = new Ref(StateRef.StandWalkingTurnAround, StateRef.KeyDef + nameof(StateRef.StandWalkingTurnAround));
        /// <summary> 蹲伏行走（预备）-映射 </summary>
        public static readonly Ref CrouchWalkingBeginRef = new Ref(StateRef.CrouchWalkingBegin, StateRef.KeyDef + nameof(StateRef.CrouchWalkingBegin));
        /// <summary> 蹲伏行走（循环）-映射 </summary>
        public static readonly Ref CrouchWalkingLoopRef = new Ref(StateRef.CrouchWalkingLoop, StateRef.KeyDef + nameof(StateRef.CrouchWalkingLoop));
        /// <summary> 蹲伏行走（停止）-映射 </summary>
        public static readonly Ref CrouchWalkingEndRef = new Ref(StateRef.CrouchWalkingEnd, StateRef.KeyDef + nameof(StateRef.CrouchWalkingEnd));
        /// <summary> 蹲伏行走中转身-映射 </summary>
        public static readonly Ref CrouchWalkingTurnAroundRef = new Ref(StateRef.CrouchWalkingTurnAround, StateRef.KeyDef + nameof(StateRef.CrouchWalkingTurnAround));
        /// <summary> 奔跑（预备）-映射 </summary>
        public static readonly Ref RunningBeginRef = new Ref(StateRef.RunningBegin, StateRef.KeyDef + nameof(StateRef.RunningBegin));
        /// <summary> 奔跑（循环）-映射 </summary>
        public static readonly Ref RunningLoopRef = new Ref(StateRef.RunningLoop, StateRef.KeyDef + nameof(StateRef.RunningLoop));
        /// <summary> 奔跑（停止）-映射 </summary>
        public static readonly Ref RunningEndRef = new Ref(StateRef.RunningEnd, StateRef.KeyDef + nameof(StateRef.RunningEnd));
        /// <summary> 奔跑中转身-映射 </summary>
        public static readonly Ref RunningTurnAroundRef = new Ref(StateRef.RunningTurnAround, StateRef.KeyDef + nameof(StateRef.RunningTurnAround));
        /// <summary> 原地时跳跃（预备）-映射 </summary>
        public static readonly Ref IdleJumpingBeginRef = new Ref(StateRef.IdleJumpingBegin, StateRef.KeyDef + nameof(StateRef.IdleJumpingBegin));
        /// <summary> 原地时跳跃（上升）-映射 </summary>
        public static readonly Ref IdleJumpingUpRef = new Ref(StateRef.IdleJumpingUp, StateRef.KeyDef + nameof(StateRef.IdleJumpingUp));
        /// <summary> 原地时跳跃（下落）-映射 </summary>
        public static readonly Ref IdleJumpingDownRef = new Ref(StateRef.IdleJumpingDown, StateRef.KeyDef + nameof(StateRef.IdleJumpingDown));
        /// <summary> 原地时跳跃（着地）-映射 </summary>
        public static readonly Ref IdleJumpingEndRef = new Ref(StateRef.IdleJumpingEnd, StateRef.KeyDef + nameof(StateRef.IdleJumpingEnd));
        /// <summary> 原地跳跃中转身-映射 </summary>
        public static readonly Ref IdleJumpingTurnAroundRef = new Ref(StateRef.IdleJumpingTurnAround, StateRef.KeyDef + nameof(StateRef.IdleJumpingTurnAround));
        /// <summary> 行走时跳跃（预备）-映射 </summary>
        public static readonly Ref WalkJumpingBeginRef = new Ref(StateRef.WalkJumpingBegin, StateRef.KeyDef + nameof(StateRef.WalkJumpingBegin));
        /// <summary> 行走时跳跃（上升）-映射 </summary>
        public static readonly Ref WalkJumpingUpRef = new Ref(StateRef.WalkJumpingUp, StateRef.KeyDef + nameof(StateRef.WalkJumpingUp));
        /// <summary> 行走时跳跃（下落）-映射 </summary>
        public static readonly Ref WalkJumpingDownRef = new Ref(StateRef.WalkJumpingDown, StateRef.KeyDef + nameof(StateRef.WalkJumpingDown));
        /// <summary> 行走时跳跃（着地）-映射 </summary>
        public static readonly Ref WalkJumpingEndRef = new Ref(StateRef.WalkJumpingEnd, StateRef.KeyDef + nameof(StateRef.WalkJumpingEnd));
        /// <summary> 行走跳跃中转身-映射 </summary>
        public static readonly Ref WalkJumpingTurnAroundRef = new Ref(StateRef.WalkJumpingTurnAround, StateRef.KeyDef + nameof(StateRef.WalkJumpingTurnAround));
        /// <summary> 奔跑时跳跃（预备）-映射 </summary>
        public static readonly Ref RunJumpingBeginRef = new Ref(StateRef.RunJumpingBegin, StateRef.KeyDef + nameof(StateRef.RunJumpingBegin));
        /// <summary> 奔跑时跳跃（上升）-映射 </summary>
        public static readonly Ref RunJumpingUpRef = new Ref(StateRef.RunJumpingUp, StateRef.KeyDef + nameof(StateRef.RunJumpingUp));
        /// <summary> 奔跑时跳跃（下落）-映射 </summary>
        public static readonly Ref RunJumpingDownRef = new Ref(StateRef.RunJumpingDown, StateRef.KeyDef + nameof(StateRef.RunJumpingDown));
        /// <summary> 奔跑时跳跃（着地）-映射 </summary>
        public static readonly Ref RunJumpingEndRef = new Ref(StateRef.RunJumpingEnd, StateRef.KeyDef + nameof(StateRef.RunJumpingEnd));
        /// <summary> 奔跑跳跃中转身-映射 </summary>
        public static readonly Ref RunJumpingTurnAroundRef = new Ref(StateRef.RunJumpingTurnAround, StateRef.KeyDef + nameof(StateRef.RunJumpingTurnAround));
        /// <summary> 站立上段受击（硬直）-映射 </summary>
        public static readonly Ref StandHighHitStopRef = new Ref(StateRef.StandHighHitStop, StateRef.KeyDef + nameof(StateRef.StandHighHitStop));
        /// <summary> 站立上段受击（击退）-映射 </summary>
        public static readonly Ref StandHighHitKnockbackRef = new Ref(StateRef.StandHighHitKnockback, StateRef.KeyDef + nameof(StateRef.StandHighHitKnockback));
        /// <summary> 站立下段受击（硬直）-映射 </summary>
        public static readonly Ref StandLowHitStopRef = new Ref(StateRef.StandLowHitStop, StateRef.KeyDef + nameof(StateRef.StandLowHitStop));
        /// <summary> 站立下段受击（击退）-映射 </summary>
        public static readonly Ref StandLowHitKnockbackRef = new Ref(StateRef.StandLowHitKnockback, StateRef.KeyDef + nameof(StateRef.StandLowHitKnockback));
        /// <summary> 蹲伏受击（硬直）-映射 </summary>
        public static readonly Ref CrouchHitStopRef = new Ref(StateRef.CrouchHitStop, StateRef.KeyDef + nameof(StateRef.CrouchHitStop));
        /// <summary> 蹲伏受击（击退）-映射 </summary>
        public static readonly Ref CrouchHitKnockbackRef = new Ref(StateRef.CrouchHitKnockback, StateRef.KeyDef + nameof(StateRef.CrouchHitKnockback));
        /// <summary> 空中受击（硬直）-映射 </summary>
        public static readonly Ref AirHitStopRef = new Ref(StateRef.AirHitStop, StateRef.KeyDef + nameof(StateRef.AirHitStop));
        /// <summary> 空中受击（上升）-映射 </summary>
        public static readonly Ref AirHitUpRef = new Ref(StateRef.AirHitUp, StateRef.KeyDef + nameof(StateRef.AirHitUp));
        /// <summary> 空中受击（下落）-映射 </summary>
        public static readonly Ref AirHitDownRef = new Ref(StateRef.AirHitDown, StateRef.KeyDef + nameof(StateRef.AirHitDown));
        /// <summary> 站立转倒地-映射 </summary>
        public static readonly Ref StandToLieRef = new Ref(StateRef.StandToLie, StateRef.KeyDef + nameof(StateRef.StandToLie));
        /// <summary> 蹲伏转倒地-映射 </summary>
        public static readonly Ref CrouchToLieRef = new Ref(StateRef.CrouchToLie, StateRef.KeyDef + nameof(StateRef.CrouchToLie));
        /// <summary> 空中转倒地-映射 </summary>
        public static readonly Ref AirToLieRef = new Ref(StateRef.AirToLie, StateRef.KeyDef + nameof(StateRef.AirToLie));
        /// <summary> 倒地弹起-映射 </summary>
        public static readonly Ref LieBounceRef = new Ref(StateRef.LieBounce, StateRef.KeyDef + nameof(StateRef.LieBounce));
        /// <summary> 倒地（循环）-映射 </summary>
        public static readonly Ref LyingRef = new Ref(StateRef.Lying, StateRef.KeyDef + nameof(StateRef.Lying));
        /// <summary> 眩晕（过渡）-映射 </summary>
        public static readonly Ref DizzyTransRef = new Ref(StateRef.DizzyTrans, StateRef.KeyDef + nameof(StateRef.DizzyTrans));
        /// <summary> 眩晕（循环）-映射 </summary>
        public static readonly Ref DizzyLoopRef = new Ref(StateRef.DizzyLoop, StateRef.KeyDef + nameof(StateRef.DizzyLoop));
        /// <summary> 原地起身-映射 </summary>
        public static readonly Ref IdleGetUpRef = new Ref(StateRef.IdleGetUp, StateRef.KeyDef + nameof(StateRef.IdleGetUp));
        /// <summary> 向前起身-映射 </summary>
        public static readonly Ref ForwardGetUpRef = new Ref(StateRef.ForwardGetUp, StateRef.KeyDef + nameof(StateRef.ForwardGetUp));
        /// <summary> 向后起身-映射 </summary>
        public static readonly Ref BackwardGetUpRef = new Ref(StateRef.BackwardGetUp, StateRef.KeyDef + nameof(StateRef.BackwardGetUp));
        /// <summary> 地面受身-映射 </summary>
        public static readonly Ref GroundRecoverRef = new Ref(StateRef.GroundRecover, StateRef.KeyDef + nameof(StateRef.GroundRecover));
        /// <summary> 空中受身-映射 </summary>
        public static readonly Ref AirRecoverRef = new Ref(StateRef.AirRecover, StateRef.KeyDef + nameof(StateRef.AirRecover));
    }
}
/*
 站立（循环）
 蹲伏（循环）
 站立→蹲伏
 蹲伏→站立
 行走-站立时（预备）
 行走-站立时（循环）
 行走-站立时（停止）
 行走-蹲伏时（预备）
 行走-蹲伏时（循环）
 行走-蹲伏时（停止）
 奔跑（预备）
 奔跑（循环）
 奔跑（停止）
 转身-站立行走中
 转身-蹲伏行走中
 转身-奔跑中
 转身-跳跃中
 跳跃-原地时（预备）
 跳跃-行走时（预备）
 跳跃-奔跑时（预备）
 跳跃-原地时（上升）
 跳跃-行走时（上升）
 跳跃-奔跑时（上升）
 跳跃-原地时（下落）
 跳跃-行走时（下落）
 跳跃-奔跑时（下落）
 跳跃-原地时（落地）
 跳跃-行走时（落地）
 跳跃-奔跑时（落地）
 站立受击-上段（硬直）
 站立受击-上段（被击退）
 站立受击-下段（硬直）
 站立受击-下段（被击退）
 蹲伏受击（硬直）
 蹲伏受击（被击退）
 空中受击（硬直）
 空中受击-上升（被击飞）
 空中受击-过渡（被击飞）
 空中受击-下落（被击飞
 倒地-弹起
 站立→倒地
 蹲伏→倒地
 空中→倒地
 倒地（循环）
 倒地起身-原地
 倒地起身-后滚
 倒地起身-前滚
 快速受身-空中
 快速受身-落地
 晕眩（起始）
 晕眩（循环）
 */