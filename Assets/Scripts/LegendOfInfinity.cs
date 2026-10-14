using LOI.Gameplay;
using System;
using System.Collections.Generic;

namespace LOI
{
    // Form Elsewhere
    [ModRegister("Legend Of Infinity", "Vanilla.LOI", "0.0.0.1", "YsinGGG")]
    public sealed class LegendOfInfinity : Mod
    {
        internal static Dictionary<Type, IList<Modifier>> ModifierRegisters { get; } = new Dictionary<Type, IList<Modifier>>();

        public enum Direction
        {
            N = -1,
            U, R, D, L
        }

        public enum EquipmentRarity
        {
            Ancient, // 鉴定
            Modern, // 匠造
            Taboo, // 禁忌
            Special, // 特殊
        }

        public enum EquipmentType
        {
            Weapon,
            Head,
            Chest,
            Waist,
            Leg,
            Foot,
            Accessory,
        }


        public enum WeaponClass
        {
            None,
            Artefact,
            Blade,
            Bow,
            Dagger,
            Handgun,
            Polearm,
            SwordAndShield,
            Whip,
        }

        public enum InputKey
        {
            /// <summary>未定义键位</summary>
            Undefined = -1,

            /// <summary>字母键 A</summary>
            A,
            /// <summary>字母键 B</summary>
            B,
            /// <summary>字母键 C</summary>
            C,
            /// <summary>字母键 D</summary>
            D,
            /// <summary>字母键 E</summary>
            E,
            /// <summary>字母键 F</summary>
            F,
            /// <summary>字母键 G</summary>
            G,
            /// <summary>字母键 H</summary>
            H,
            /// <summary>字母键 I</summary>
            I,
            /// <summary>字母键 J</summary>
            J,
            /// <summary>字母键 K</summary>
            K,
            /// <summary>字母键 L</summary>
            L,
            /// <summary>字母键 M</summary>
            M,
            /// <summary>字母键 N</summary>
            N,
            /// <summary>字母键 O</summary>
            O,
            /// <summary>字母键 P</summary>
            P,
            /// <summary>字母键 Q</summary>
            Q,
            /// <summary>字母键 R</summary>
            R,
            /// <summary>字母键 S</summary>
            S,
            /// <summary>字母键 T</summary>
            T,
            /// <summary>字母键 U</summary>
            U,
            /// <summary>字母键 V</summary>
            V,
            /// <summary>字母键 W</summary>
            W,
            /// <summary>字母键 X</summary>
            X,
            /// <summary>字母键 Y</summary>
            Y,
            /// <summary>字母键 Z</summary>
            Z,

            /// <summary>功能键 F1</summary>
            F1,
            /// <summary>功能键 F2</summary>
            F2,
            /// <summary>功能键 F3</summary>
            F3,
            /// <summary>功能键 F4</summary>
            F4,
            /// <summary>功能键 F5</summary>
            F5,
            /// <summary>功能键 F6</summary>
            F6,
            /// <summary>功能键 F7</summary>
            F7,
            /// <summary>功能键 F8</summary>
            F8,
            /// <summary>功能键 F9</summary>
            F9,
            /// <summary>功能键 F10</summary>
            F10,
            /// <summary>功能键 F11</summary>
            F11,
            /// <summary>功能键 F12</summary>
            F12,

            /// <summary>数字键0（主键盘）</summary>
            Number0,
            /// <summary>数字键1（主键盘）</summary>
            Number1,
            /// <summary>数字键2（主键盘）</summary>
            Number2,
            /// <summary>数字键3（主键盘）</summary>
            Number3,
            /// <summary>数字键4（主键盘）</summary>
            Number4,
            /// <summary>数字键5（主键盘）</summary>
            Number5,
            /// <summary>数字键6（主键盘）</summary>
            Number6,
            /// <summary>数字键7（主键盘）</summary>
            Number7,
            /// <summary>数字键8（主键盘）</summary>
            Number8,
            /// <summary>数字键9（主键盘）</summary>
            Number9,

            /// <summary>数字键0（小键盘）</summary>
            Keypad0,
            /// <summary>数字键1（小键盘）</summary>
            Keypad1,
            /// <summary>数字键2（小键盘）</summary>
            Keypad2,
            /// <summary>数字键3（小键盘）</summary>
            Keypad3,
            /// <summary>数字键4（小键盘）</summary>
            Keypad4,
            /// <summary>数字键5（小键盘）</summary>
            Keypad5,
            /// <summary>数字键6（小键盘）</summary>
            Keypad6,
            /// <summary>数字键7（小键盘）</summary>
            Keypad7,
            /// <summary>数字键8（小键盘）</summary>
            Keypad8,
            /// <summary>数字键9（小键盘）</summary>
            Keypad9,

            /// <summary>回车（小键盘）</summary>
            KeypadEnter,
            /// <summary>加号（小键盘）</summary>
            KeypadAdd,
            /// <summary>减号（小键盘）</summary>
            KeypadSubtract,
            /// <summary>乘号（小键盘）</summary>
            KeypadMultiply,
            /// <summary>除号（小键盘）</summary>
            KeypadDivide,
            /// <summary>小数点号（小键盘）</summary>
            KeypadPeriod,

            /// <summary>方向键-上（↑）</summary>
            UpArrow,
            /// <summary>方向键-下（↓）</summary>
            DownArrow,
            /// <summary>方向键-左（←）</summary>
            LeftArrow,
            /// <summary>方向键-右（→）</summary>
            RightArrow,

            /// <summary>左Ctrl</summary>
            LeftControl,
            /// <summary>右Ctrl</summary>
            RightControl,
            /// <summary>左Shift</summary>
            LeftShift,
            /// <summary>右Shift</summary>
            RightShift,
            /// <summary>左Alt</summary>
            LeftAlt,
            /// <summary>右Alt</summary>
            RightAlt,

            /// <summary>空格（Space）</summary>
            Space,
            /// <summary>退出（Esc）</summary>
            Escape,
            /// <summary>制表（Tab）</summary>
            Tab,
            /// <summary>大写锁定（Caps Lock）</summary>
            CapsLock,
            /// <summary>退格（Backspace）</summary>
            Backspace,
            /// <summary>回车（Enter）</summary>
            Enter,

            /// <summary>分号 ;</summary>
            Semicolon,
            /// <summary>等号 =</summary>
            Equal,
            /// <summary>逗号 ,</summary>
            Comma,
            /// <summary>减号 -</summary>
            Minus,
            /// <summary>句号 .</summary>
            Period,
            /// <summary>斜杠 /</summary>
            Slash,
            /// <summary>反斜杠 \</summary>
            Backslash,
            /// <summary>单引号 '</summary>
            Apostrophe,
            /// <summary>左方括号 [</summary>
            BracketLeft,
            /// <summary>右方括号 ]</summary>
            BracketRight,
            /// <summary>反引号 ` (位于 Esc 下方，数字 1 左边)</summary>
            Grave,

            /// <summary>Home 键</summary>
            Home,
            /// <summary>End 键</summary>
            End,
            /// <summary>Page Up 键</summary>
            PageUp,
            /// <summary>Page Down 键</summary>
            PageDown,
            /// <summary>Insert 键</summary>
            Insert,
            /// <summary>Delete 键</summary>
            Delete,

            /// <summary>Print Screen 键</summary>
            PrintScreen,
            /// <summary>Scroll Lock 键</summary>
            ScrollLock,
            /// <summary>Pause / Break 键</summary>
            Pause,
            /// <summary>Num Lock 键 (小键盘锁定)</summary>
            NumLock,

            /// <summary>左摇杆上拨（LS-Up）</summary>
            JoyLeftStickUp,
            /// <summary>左摇杆下拨（LS-Down）</summary>
            JoyLeftStickDown,
            /// <summary>左摇杆左拨（LS-Left）</summary>
            JoyLeftStickLeft,
            /// <summary>左摇杆右拨（LS-Right）</summary>
            JoyLeftStickRight,
            /// <summary>左摇杆按下 (Xbox:LS / Sony:L3 / Nintendo:LS)</summary>
            JoyLeftStickButton,

            /// <summary>右摇杆上拨（RS-Up）</summary>
            JoyRightStickUp,
            /// <summary>右摇杆下拨（RS-Down）</summary>
            JoyRightStickDown,
            /// <summary>右摇杆左拨（RS-Left）</summary>
            JoyRightStickLeft,
            /// <summary>右摇杆右拨（RS-Right）</summary>
            JoyRightStickRight,
            /// <summary>右摇杆按下 (Xbox:RS / Sony:R3 / Nintendo:RS)</summary>
            JoyRightStickButton,

            /// <summary>十字键-上 (Dpad-Up)</summary>
            JoyDpadUp,
            /// <summary>十字键-下 (Dpad-Down)</summary>
            JoyDpadDown,
            /// <summary>十字键-左 (Dpad-Left)</summary>
            JoyDpadLeft,
            /// <summary>十字键-右 (Dpad-Right)</summary>
            JoyDpadRight,

            /// <summary>手柄A键 (Xbox:A / Sony:Cross / Nintendo:B)</summary>
            JoyButtonA,
            /// <summary>手柄B键 (Xbox:B / Sony:Circle / Nintendo:A)</summary>
            JoyButtonB,
            /// <summary>手柄X键 (Xbox:X / Sony:Square / Nintendo:Y)</summary>
            JoyButtonX,
            /// <summary>手柄Y键 (Xbox:Y / Sony:Triangle / Nintendo:X)</summary>
            JoyButtonY,

            /// <summary>左肩键 (Xbox:LB / Sony:L1 / Nintendo:L)</summary>
            JoyLeftShoulder,
            /// <summary>右肩键 (Xbox:RB / Sony:R1 / Nintendo:R)</summary>
            JoyRightShoulder,
            /// <summary>左扳机键 (Xbox:LT / Sony:L2 / Nintendo:L2)</summary>
            JoyLeftTrigger,
            /// <summary>右扳机键 (Xbox:RT / Sony:R2 / Nintendo:R2)</summary>
            JoyRightTrigger,

            /// <summary>手柄扩展键1 (Xbox背键1)</summary>
            JoyButtonPaddle1,
            /// <summary>手柄扩展键2 (Xbox背键2)</summary>
            JoyButtonPaddle2,
            /// <summary>手柄扩展键3 (Xbox背键3)</summary>
            JoyButtonPaddle3,
            /// <summary>手柄扩展键4 (Xbox背键4)</summary>
            JoyButtonPaddle4,
            /// <summary>手柄触屏 (Sony触屏)</summary>
            JoyButtonTouchpad,

            /// <summary>开始键 (Xbox:Start / Sony:Options / Nintendo:+)</summary>
            JoyButtonStart,
            /// <summary>返回键 (Xbox:Back / Sony:Share / Nintendo:-)</summary>
            JoyButtonBack,
            /// <summary>导航键 (Xbox:Guide / Sony:PS)</summary>
            JoyButtonGuide,
            /// <summary>杂项键（Xbox:Share / Sony:Microphone）</summary>
            JoyButtonMisc,

            /// <summary>鼠标滚轮向上滚动</summary>
            MouseWheelUp,
            /// <summary>鼠标滚轮向下滚动</summary>
            MouseWheelDown,
            /// <summary>鼠标滚轮向左滚动</summary>
            MouseWheelLeft,
            /// <summary>鼠标滚轮向右滚动</summary>
            MouseWheelRight,

            /// <summary>鼠标左键（LMB）</summary>
            MouseLeft,
            /// <summary>鼠标右键（RMB）</summary>
            MouseRight,
            /// <summary>鼠标中键（MMB，滚轮按下）</summary>
            MouseMiddle,
            /// <summary>鼠标扩展键4 (前侧键/近)</summary>
            MouseButton4,
            /// <summary>鼠标扩展键5 (后侧键/远)</summary>
            MouseButton5,
        }

        /// <summary> 游戏键位 </summary>
        public enum GameKey
        {
            /// <summary> UI焦点上移，角色向上跳跃 </summary>
            Up,
            /// <summary> UI焦点下移，角色向下跳跃 </summary>
            Down,
            /// <summary> UI焦点左移，角色向左移动 </summary>
            Left,
            /// <summary> UI焦点右移，角色向右移动 </summary>
            Right,
            /// <summary> 武器攻击1 </summary>
            WeaponAttack1,
            /// <summary> 武器攻击2 </summary>
            WeaponAttack2,
            /// <summary> 武器特殊动作 </summary>
            WeaponAction,
            /// <summary> 交互 </summary>
            Interact,
            /// <summary> 主动技1 </summary>
            ActiveSkill1,
            /// <summary> 主动技2 </summary>
            ActiveSkill2,
            /// <summary> 主动技3 </summary>
            ActiveSkill3,
            /// <summary> 主动技4 </summary>
            ActiveSkill4,
            /// <summary> 快捷道具1 </summary>
            QuickProp1,
            /// <summary> 快捷道具2 </summary>
            QuickProp2,
            /// <summary> 快捷道具3 </summary>
            QuickProp3,
            /// <summary> 快捷道具4 </summary>
            QuickProp4,
            /// <summary> 快捷道具四向轮盘 </summary>
            QuickPropWheel,
            /// <summary> 技能栏四向轮盘 </summary>
            ActiveSkillWheel,
            /// <summary> 背包界面 </summary>
            Inventory,
            /// <summary> 角色界面 </summary>
            Character,
            /// <summary> 任务界面 </summary>
            Quest,
            /// <summary> 地图界面 </summary>
            Map,
            /// <summary> 菜单界面 </summary>
            Menu,
        }

        /// <summary> 内置物理运动类型 </summary>
        public enum PhysicsType
        {
            /// <summary> 未定义<para>- 不会受到该内置物理效果影响</para> </summary>
            Undefined,
            /// <summary> 位于地面<para>水平方向速度逐帧衰减，垂直速度锁定为0</para>对于不同的姿势类型，水平方向速度的衰减系数也不同<para>- 空中：1.0x（不合规不予处理）</para><para>- 站立：0.8x（衰减20%）</para><para>- 蹲伏：0.4x（衰减60%）</para><para>- 倒地：0.2x（衰减80%）</para> </summary>
            Ground,
            /// <summary> 位于空中<para>水平方向速度保持不变，垂直方向速度逐帧增加</para>上述效果仅当状态类型为“空中”时生效</summary>
            Air,
            /// <summary> 继承<para>- 自动使用上一个状态的内置物理运动类型</para> </summary>
            Inherited,
        }

        /// <summary> 姿势类型 </summary>
        public enum StateType
        {
            /// <summary> 未定义<para>- 不会被用于任何相关判定</para> </summary>
            Undefined,
            /// <summary> 站立 </summary>
            Stand,
            /// <summary> 蹲伏 </summary>
            Crouch,
            /// <summary> 滞空 </summary>
            Air,
            /// <summary> 倒地 </summary>
            Down,
            /// <summary> 继承<para>- 自动使用上一个状态的姿势类型</para> </summary>
            Inherited,
        }

        /// <summary> 行动类型 </summary>
        public enum MoveType
        {
            /// <summary> 未定义<para>不会被用于任何相关判定</para> </summary>
            Undefined,
            /// <summary> 空闲（自由态） </summary>
            Idle,
            /// <summary> 攻击（连携态） </summary>
            Attack,
            /// <summary> 受击（硬直态） </summary>
            Hit,
            /// <summary> 继承<para>- 自动使用上一个状态的行动类型</para> </summary>
            Inherited,
        }

        /// <summary> 进攻判定类型 </summary>
        public enum HitFlag
        {
            /// <summary> 无判定 </summary>
            N = 0,
            /// <summary> 站立 </summary>
            S = 0b0001,
            /// <summary> 蹲伏 </summary>
            C = 0b0010,
            /// <summary> 滞空 </summary>
            A = 0b0100,
            /// <summary> 倒地 </summary>
            D = 0b1000,
            /// <summary> 站立/蹲伏 </summary>
            SC = S | C,
            /// <summary> 站立/滞空 </summary>
            SA = S | A,
            /// <summary> 站立/倒地 </summary>
            SD = S | D,
            /// <summary> 蹲伏/滞空 </summary>
            CA = C | A,
            /// <summary> 蹲伏/倒地 </summary>
            CD = C | D,
            /// <summary> 滞空/倒地 </summary>
            AD = A | D,
            /// <summary> 站立/蹲伏/滞空 </summary>
            SCA = S | C | A,
            /// <summary> 站立/蹲伏/倒地 </summary>
            SCD = S | C | D,
            /// <summary> 站立/滞空/倒地 </summary>
            SAD = S | A | D,
            /// <summary> 蹲伏/滞空/倒地 </summary>
            CAD = C | A | D,
            /// <summary> 站立/蹲伏/滞空/倒地 </summary>
            SCAD = S | C | A | D,
        }

        /// <summary> 最大帧率 </summary>
        public enum FPSLimit
        {
            /// <summary> 自动调整帧率（自动获取并设为当前显示器的刷新率） </summary>
            Auto = -1,
            /// <summary> 30 FPS </summary>
            FPS30 = 30,
            /// <summary> 60 FPS </summary>
            FPS60 = 60,
            /// <summary> 120 FPS </summary>
            FPS120 = 120,
            /// <summary> 240 FPS </summary>
            FPS240 = 240,
            /// <summary> 300 FPS </summary>
            FPS300 = 300,
        }

        /// <summary> 显示模式 </summary>
        public enum DisplayMode
        {
            BorderlessWindowed = 0,
            Windowed = 1,
            Fullscreen = 2,
            WindowedFullscreen = 3,
        }
    }
}
