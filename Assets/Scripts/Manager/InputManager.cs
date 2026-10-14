using Godot;
using LOI.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static LOI.LegendOfInfinity;
using static LOI.States.BladeArt.DriveSlash;

namespace LOI.Manager
{
    public sealed partial class InputManager : LOIManager
    {
        public static InputManager Instance { get; private set; }

        public static event Action<GameKey> OnKeyPressed;
        public static event Action<GameKey> OnKeyReleased;
        public static event Action<GameKey> OnKeyDoubleClicked;

        /// <summary> 键位数 </summary>
        public const int MaxKeymap = 23;

        /// <summary> 当前键位映射表 </summary>
        private static InputKey[] CurrentKeymap => Keyboard_DefKeymap1;

        /// <summary> 默认键位映射表 </summary>
        private static InputKey[] Keyboard_DefKeymap1 { get; } = new InputKey[InputManager.MaxKeymap]
        {
            InputKey.UpArrow, InputKey.DownArrow, InputKey.LeftArrow, InputKey.RightArrow,
            InputKey.Z, InputKey.X, InputKey.C, InputKey.F,
            InputKey.A, InputKey.S, InputKey.D, InputKey.V,
            InputKey.Q, InputKey.W, InputKey.E, InputKey.R,
            InputKey.LeftAlt, InputKey.LeftControl,
            InputKey.I, InputKey.U, InputKey.O, InputKey.M, InputKey.Escape,
        };

        private static InputKey[] Keyboard_DefKeymap2 { get; } = new InputKey[InputManager.MaxKeymap]
        {
            InputKey.W, InputKey.S, InputKey.A, InputKey.D,
            InputKey.Keypad1, InputKey.Keypad2, InputKey.Keypad3, InputKey.F,
            InputKey.Keypad4, InputKey.Keypad5, InputKey.Keypad6, InputKey.KeypadEnter,
            InputKey.Keypad7, InputKey.Keypad8, InputKey.Keypad9, InputKey.KeypadAdd,
            InputKey.Space, InputKey.LeftShift,
            InputKey.I, InputKey.U, InputKey.O, InputKey.M, InputKey.Escape,
        };

        private static InputKey[] Joypad_DefKeymap1 { get; } = new InputKey[InputManager.MaxKeymap]
        {
            InputKey.JoyLeftStickUp, InputKey.JoyLeftStickDown, InputKey.JoyLeftStickLeft, InputKey.JoyLeftStickRight,
            InputKey.JoyButtonY, InputKey.JoyButtonB, InputKey.JoyRightTrigger, InputKey.JoyButtonA,
            InputKey.JoyDpadLeft, InputKey.JoyDpadUp, InputKey.JoyDpadRight, InputKey.JoyDpadDown,
            InputKey.JoyRightStickLeft, InputKey.JoyRightStickUp, InputKey.JoyRightStickRight, InputKey.JoyRightStickDown,
            InputKey.JoyLeftStickButton, InputKey.JoyRightStickButton,
            InputKey.JoyButtonX, InputKey.JoyRightShoulder, InputKey.JoyLeftShoulder, InputKey.JoyButtonStart, InputKey.JoyButtonBack,
        };

        private static InputKey[] Joypad_DefKeymap2 { get; } = new InputKey[InputManager.MaxKeymap]
        {
            InputKey.JoyLeftStickUp, InputKey.JoyLeftStickDown, InputKey.JoyLeftStickLeft, InputKey.JoyLeftStickRight,
            InputKey.JoyButtonY, InputKey.JoyButtonB, InputKey.JoyRightTrigger, InputKey.JoyButtonA,
            InputKey.JoyDpadLeft, InputKey.JoyDpadUp, InputKey.JoyDpadRight, InputKey.JoyDpadDown,
            InputKey.JoyRightStickLeft, InputKey.JoyRightStickUp, InputKey.JoyRightStickRight, InputKey.JoyRightStickDown,
            InputKey.JoyLeftStickButton, InputKey.JoyRightStickButton,
            InputKey.JoyButtonX, InputKey.JoyRightShoulder, InputKey.JoyLeftShoulder, InputKey.JoyButtonStart, InputKey.JoyButtonBack,
        };

        private static Dictionary<InputKey, GameKey> InputKeymap { get; } = new Dictionary<InputKey, GameKey>();

        private HashSet<GameKey> presses = new HashSet<GameKey>();
        private HashSet<GameKey> releases = new HashSet<GameKey>();
        private HashSet<GameKey> doubleClicks = new HashSet<GameKey>();
        private Dictionary<GameKey, int> dbcKeys = new Dictionary<GameKey, int>();
        private float[] prevAxisValue = new float[6];
        private float[] currAxisValue = new float[6];
        private static List<Options.Option<InputKey[]>> controls = new List<Options.Option<InputKey[]>>();

        protected override void Initialization()
        {
            InputManager.Instance = this;
        }

        protected override void EnterTree()
        {
            var modify = (InputKey[] d, InputKey[] v) => { for (int i = 0; i < v.Length; i++) v[i] = Enum.IsDefined(v[i]).IfElse(v[i], d[i]); return v; };
            for (int i = 0; i < InputManager.MaxKeymap; i++)
            {
                GameKey gameKey = (GameKey)i;
                string name = gameKey.ToString();
                InputKey[] def = { InputManager.Keyboard_DefKeymap1[i], InputKey.Undefined, InputManager.Joypad_DefKeymap1[i], InputKey.Undefined };
                InputManager.controls.Add(new(name, $"{nameof(GameKey)}.{name}", def, modify));
                InputManager.SetKeymap(InputManager.CurrentKeymap[i], gameKey);
                this.dbcKeys.Add(gameKey, 0);
            }
        }

        public void SetGameKey(GameKey key, InputKey input, int index)
        {
            InputKey[] value = InputManager.controls[(int)key];
            value[index] = input;
            InputManager.controls[(int)key].Value = value;
        }

        protected override void GameUpdate()
        {
            if (this.releases.Count > 0)
                this.releases.Clear();
            if (this.doubleClicks.Count > 0)
                this.doubleClicks.Clear();
            foreach (GameKey gameKey in this.dbcKeys.Keys)
                if (this.dbcKeys[gameKey] > 0)
                    this.dbcKeys[gameKey]--;
        }

        protected override void InputUpdate(InputEvent evt)
        {
            bool pressed = false;
            InputKey key = InputKey.Undefined;
            switch (evt)
            {
                case InputEventKey keyEvt:
                    pressed = keyEvt.Pressed;
                    switch (keyEvt.Keycode)
                    {
                        case Key.Minus: key = InputKey.Minus; break;
                        case Key.Equal: key = InputKey.Equal; break;
                        case Key.Slash: key = InputKey.Slash; break;
                        case Key.Backslash: key = InputKey.Backslash; break;
                        case Key.Comma: key = InputKey.Comma; break;
                        case Key.Apostrophe: key = InputKey.Apostrophe; break;
                        case Key.Semicolon: key = InputKey.Semicolon; break;
                        case Key.Quoteleft: key = InputKey.Grave; break;
                        case Key.Period: key = InputKey.Period; break;
                        case Key.Bracketleft: key = InputKey.BracketLeft; break;
                        case Key.Bracketright: key = InputKey.BracketRight; break;

                        case Key.Escape: key = InputKey.Escape; break;
                        case Key.Enter: key = InputKey.Enter; break;
                        case Key.Space: key = InputKey.Space; break;
                        case Key.Capslock: key = InputKey.CapsLock; break;
                        case Key.Numlock: key = InputKey.NumLock; break;
                        case Key.Tab: key = InputKey.Tab; break;
                        case Key.Backspace: key = InputKey.Backspace; break;

                        case Key.Left: key = InputKey.LeftArrow; break;
                        case Key.Right: key = InputKey.RightArrow; break;
                        case Key.Up: key = InputKey.UpArrow; break;
                        case Key.Down: key = InputKey.DownArrow; break;

                        case Key.Insert: key = InputKey.Insert; break;
                        case Key.Delete: key = InputKey.Delete; break;
                        case Key.Home: key = InputKey.Home; break;
                        case Key.End: key = InputKey.End; break;
                        case Key.Pageup: key = InputKey.PageUp; break;
                        case Key.Pagedown: key = InputKey.PageDown; break;

                        case Key.Print: key = InputKey.PrintScreen; break;
                        case Key.Scrolllock: key = InputKey.ScrollLock; break;
                        case Key.Pause: key = InputKey.Pause; break;

                        case Key.A: case Key.B: case Key.C: case Key.D: case Key.E: case Key.F: case Key.G:
                        case Key.H: case Key.I: case Key.J: case Key.K: case Key.L: case Key.M: case Key.N:
                        case Key.O: case Key.P: case Key.Q: case Key.R: case Key.S: case Key.T:
                        case Key.U: case Key.V: case Key.W: case Key.X: case Key.Y: case Key.Z:
                            key = InputKey.A + (int)(keyEvt.Keycode - Key.A);
                            break;

                        case Key.F1: case Key.F2: case Key.F3: case Key.F4:
                        case Key.F5: case Key.F6: case Key.F7: case Key.F8:
                        case Key.F9: case Key.F10: case Key.F11: case Key.F12:
                            key = InputKey.F1 + (int)(keyEvt.Keycode - Key.F1);
                            break;

                        case Key.Key0: case Key.Key1: case Key.Key2: case Key.Key3: case Key.Key4:
                        case Key.Key5: case Key.Key6: case Key.Key7: case Key.Key8: case Key.Key9:
                            key = InputKey.Number0 + (int)(keyEvt.Keycode - Key.Key0);
                            break;

                        case Key.Kp0: case Key.Kp1: case Key.Kp2: case Key.Kp3: case Key.Kp4:
                        case Key.Kp5: case Key.Kp6: case Key.Kp7: case Key.Kp8: case Key.Kp9:
                            key = InputKey.Keypad0 + (int)(keyEvt.Keycode - Key.Kp0);
                            break;

                        case Key.KpAdd: key = InputKey.KeypadAdd; break;
                        case Key.KpSubtract: key = InputKey.KeypadSubtract; break;
                        case Key.KpDivide: key = InputKey.KeypadDivide; break;
                        case Key.KpMultiply: key = InputKey.KeypadMultiply; break;
                        case Key.KpPeriod: key = InputKey.KeypadPeriod; break;
                        case Key.KpEnter: key = InputKey.KeypadEnter; break;

                        case Key.Shift: key = keyEvt.Location.IfElse(KeyLocation.Right, InputKey.RightShift, InputKey.LeftShift); break;
                        case Key.Ctrl: key = keyEvt.Location.IfElse(KeyLocation.Right, InputKey.RightControl, InputKey.LeftControl); break;
                        case Key.Alt: key = keyEvt.Location.IfElse(KeyLocation.Right, InputKey.RightAlt, InputKey.LeftAlt); break;
                    }
                    break;
                case InputEventMouseButton mouseEvt:
                    pressed = mouseEvt.Pressed;
                    switch (mouseEvt.ButtonIndex)
                    {
                        case MouseButton.Left: key = InputKey.MouseLeft; break;
                        case MouseButton.Right: key = InputKey.MouseRight; break;
                        case MouseButton.Middle: key = InputKey.MouseMiddle; break;
                        case MouseButton.Xbutton1: key = InputKey.MouseButton4; break;
                        case MouseButton.Xbutton2: key = InputKey.MouseButton5; break;
                        case MouseButton.WheelUp: key = InputKey.MouseWheelUp; break;
                        case MouseButton.WheelDown: key = InputKey.MouseWheelDown; break;
                        case MouseButton.WheelLeft: key = InputKey.MouseWheelLeft; break;
                        case MouseButton.WheelRight: key = InputKey.MouseWheelRight; break;
                    }
                    break;
                case InputEventJoypadButton gamepadEvt:
                    pressed = gamepadEvt.Pressed;
                    switch (gamepadEvt.ButtonIndex)
                    {
                        case JoyButton.A: key = InputKey.JoyButtonA; break;
                        case JoyButton.B: key = InputKey.JoyButtonB; break;
                        case JoyButton.X: key = InputKey.JoyButtonX; break;
                        case JoyButton.Y: key = InputKey.JoyButtonY; break;
                        case JoyButton.DpadUp: key = InputKey.JoyDpadUp; break;
                        case JoyButton.DpadDown: key = InputKey.JoyDpadDown; break;
                        case JoyButton.DpadLeft: key = InputKey.JoyDpadLeft; break;
                        case JoyButton.DpadRight: key = InputKey.JoyDpadRight; break;
                        case JoyButton.Paddle1: key = InputKey.JoyButtonPaddle1; break;
                        case JoyButton.Paddle2: key = InputKey.JoyButtonPaddle2; break;
                        case JoyButton.Paddle3: key = InputKey.JoyButtonPaddle3; break;
                        case JoyButton.Paddle4: key = InputKey.JoyButtonPaddle4; break;
                        case JoyButton.Touchpad: key = InputKey.JoyButtonTouchpad; break;
                        case JoyButton.Start: key = InputKey.JoyButtonStart; break;
                        case JoyButton.Back: key = InputKey.JoyButtonBack; break;
                        case JoyButton.Guide: key = InputKey.JoyButtonGuide; break;
                        case JoyButton.Misc1: key = InputKey.JoyButtonMisc; break;
                        case JoyButton.LeftShoulder: key = InputKey.JoyLeftShoulder; break;
                        case JoyButton.RightShoulder: key = InputKey.JoyRightShoulder; break;
                        case JoyButton.LeftStick: key = InputKey.JoyLeftStickButton; break;
                        case JoyButton.RightStick: key = InputKey.JoyRightStickButton; break;
                    }
                    break;
                case InputEventJoypadMotion gamepadMotionEvt:
                    int axis = (int)gamepadMotionEvt.Axis;
                    this.prevAxisValue[axis] = this.currAxisValue[axis];
                    this.currAxisValue[axis] = gamepadMotionEvt.AxisValue;
                    this.UpdateAxis(0, Options.LeftStickDeadzone.X, InputKey.JoyLeftStickRight);
                    this.UpdateAxis(0, -Options.LeftStickDeadzone.Z, InputKey.JoyLeftStickLeft);
                    this.UpdateAxis(1, Options.LeftStickDeadzone.Y, InputKey.JoyLeftStickDown);
                    this.UpdateAxis(1, -Options.LeftStickDeadzone.W, InputKey.JoyLeftStickUp);
                    this.UpdateAxis(2, Options.RightStickDeadzone.X, InputKey.JoyRightStickRight);
                    this.UpdateAxis(2, -Options.RightStickDeadzone.Z, InputKey.JoyRightStickLeft);
                    this.UpdateAxis(3, Options.RightStickDeadzone.Y, InputKey.JoyRightStickDown);
                    this.UpdateAxis(3, -Options.RightStickDeadzone.W, InputKey.JoyRightStickUp);
                    this.UpdateAxis(4, Options.TriggerDeadzone.X, InputKey.JoyLeftTrigger);
                    this.UpdateAxis(5, Options.TriggerDeadzone.Y, InputKey.JoyRightTrigger);
                    return;
            }
            this.UpdateKey(key, pressed);
        }

        private void UpdateKey(InputKey key, bool pressed)
        {
            if (InputManager.InputKeymap.TryGetValue(key, out GameKey gameKey))
                if (pressed)
                {
                    if (!this.presses.Add(gameKey)) return;
                    if (this.dbcKeys[gameKey] > 0 && this.doubleClicks.Add(gameKey))
                        InputManager.OnKeyDoubleClicked?.Invoke(gameKey);
                    this.dbcKeys[gameKey] = Options.DoubleClickDelay;
                    InputManager.OnKeyPressed?.Invoke(gameKey);
                }
                else if (this.presses.Remove(gameKey) && this.releases.Add(gameKey))
                    InputManager.OnKeyReleased?.Invoke(gameKey);
        }

        private void UpdateAxis(int axis, float deadzone, InputKey key)
        {
            float curr = this.currAxisValue[axis];
            float prev = this.prevAxisValue[axis];
            bool flag1 = deadzone < 0 ? curr <= deadzone : curr >= deadzone;
            bool flag2 = deadzone < 0 ? prev > deadzone : prev < deadzone;
            if (flag1 == flag2) this.UpdateKey(key, flag1);
        }


        public static void SetKeymap(InputKey key, GameKey gameKey) => InputManager.InputKeymap.TrySet(key, gameKey);
        public static bool OnPressed(GameKey key) => InputManager.Instance.presses.Contains(key);
        public static bool OnReleased(GameKey key) => InputManager.Instance.releases.Contains(key);
        public static bool OnDoubleClicked(GameKey key) => InputManager.Instance.doubleClicks.Contains(key);
    }
}