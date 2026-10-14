using Godot;
using LOI.DataStructures;
using LOI.Gameplay;
using LOI.Interface;
using LOI.UI;
using LOI.UI.Elements;
using LOI.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static LOI.LegendOfInfinity;

namespace LOI.Manager
{
    public sealed partial class UIManager : LOIManager
    {
        public static UIManager Instance { get; private set; }
        public static Vector2 MousePosition => UIManager.Instance.GetViewport().GetMousePosition();

        public static CharacterInfo CharacterInfo => UIManager.Instance._CharacterInfo;

        public static Color SwordBreezeBar_Color { get => UIManager.Instance._SwordBreezeBar[0].Color; set => UIManager.Instance._SwordBreezeBar[0].Color = value; }
        public static double SwordBreezeBar_Value { get => UIManager.Instance._SwordBreezeBar[0].Value; set => UIManager.Instance._SwordBreezeBar[0].Value = value; }

        [Export]
        private RichTextLabel DebugInfo;

        [Export]
        private CharacterInfo _CharacterInfo;

        [Export]
        private Control _Sliders;

        [Export]
        private TabContainer _Options;

        [Export]
        private Control KwPsControl;

        [Export]
        private Control CbTsControl;

        [Export]
        private UIProgressBar _SwordBreezeBar;

        [Export]
        private CanvasLayer _ScreenCanvas;

        internal ObjPool<KeywordPanel> keywordPanels;
        internal ObjPool<CombatTextBox> combatTextBoxes;

        protected override void Initialization()
        {
            this.KwPsControl = UIManager.Instantiate(nameof(this.KwPsControl), ref this.KwPsControl, this._ScreenCanvas);
            this.CbTsControl = UIManager.Instantiate(nameof(this.CbTsControl), ref this.CbTsControl, this);
            this.keywordPanels = new ObjPool<KeywordPanel>(((i) => { KeywordPanel obj = new(); this.KwPsControl.AddChild(obj.Disable()); obj.Visible = false; return obj; }), 8, ObjPool<KeywordPanel>.NodeComparer);
            this.keywordPanels.OnRequest += (k) => LazyUtils.Enable(k);
            this.keywordPanels.OnRelease += (k) => LazyUtils.Disable(k);
            this.combatTextBoxes = new ObjPool<CombatTextBox>(((i) => { CombatTextBox obj = new(); this.CbTsControl.AddChild(obj.Disable());obj.Visible = false; return obj; }), 32, ObjPool<CombatTextBox>.NodeComparer);
            this._CharacterInfo.LoadCharacter(GameManager.LocalCharacter);
        }

        protected override void EnterTree()
        {
            UIManager.Instance = this;
            InputManager.OnKeyPressed += (key) => { if (key == GameKey.Character) UIManager.CharacterInfo.Visible = !UIManager.CharacterInfo.Visible; UIManager.CharacterInfo.UpdateAll(); };
            if (this._Sliders != null)
            {
                string name = nameof(Options.MasterVolume);
                UIHSlider slider = new() { Name = name, DefValue = Options.MasterVolume.Default, CurrValue = Options.MasterVolume };
                slider.OnValueChanged += (value) => { Options.MasterVolume = (float)value; };
                this._Sliders.AddChild(slider);
                UIManager.AddSlider(this._Sliders, Options.MusicVolume);

                name = nameof(Options.SFXVolume);
                slider = new() { Name = name, DefValue = Options.SFXVolume.Default, CurrValue = Options.SFXVolume };
                slider.OnValueChanged += (value) => { Options.SFXVolume = (float)value; };
                this._Sliders.AddChild(slider);

                name = nameof(Options.AmbientVolume);
                slider = new() { Name = name, DefValue = Options.AmbientVolume.Default, CurrValue = Options.AmbientVolume };
                slider.OnValueChanged += (value) => { Options.AmbientVolume = (float)value; };
                this._Sliders.AddChild(slider);
            }
        }

        protected override void GameUpdate()
        {
            Character c = GameManager.LocalCharacter;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("HP, MP = {0}({1:0.00%}), {2}({3:0.00%})", c.Life, c.Life / c.Stats.MaxLife, c.Mana, c.Mana / c.Stats.MaxMana));
            sb.AppendLine(string.Format("Sta = {0}({1:0.00%})", c.Stamina, c.Stamina / c.MaxStamina));
            sb.AppendLine("Hitstop Time = " + c.HitStopTime);
            sb.AppendLine("State Time = " + c.StateTime);
            sb.AppendLine("Curr State = " + c.CurrState);
            sb.AppendLine("Pre State = " + c.PreState);
            sb.AppendLine("Physics Type = " + c.PhysicsType);
            sb.AppendLine("State Type = " + c.StateType);
            sb.AppendLine("Move Type = " + c.MoveType);
            sb.AppendLine("Facing = " + c.Facing);
            sb.AppendLine("Pos(X, Y) = " + c.Position.ToString("0.00"));
            sb.AppendLine("Vel(X, Y) = " + c.Velocity.ToString("0.00"));
            sb.AppendLine("FPS(R, P) = " + new Vector2I(LOIManager.RFPS.Round(), LOIManager.PFPS.Round()));
            this.DebugInfo.Text = sb.ToString();
            
        }

        public static CombatTextBox NewCombatText(double damage, bool crit, Vector2 position)
        {
            CombatTextBox ele = UIManager.Instance.combatTextBoxes.Request();
            ele.SetByDamage(80, damage, crit, position);
            return ele;
        }

        public static UIHSlider AddSlider(Control parent, Options.Option<float> option,  double step = 0.01d)
        {
            UIHSlider slider = new();
            slider.Name = option.Name;
            slider.DefValue = option.Default;
            slider.CurrValue = option.Value;
            slider.Step = step;
            slider.OnValueChanged += (v) => { option.Value = (float)v; };
            parent.AddChild(slider);
            return slider;
        }

        public static UIHSlider AddSlider(Control parent, Action<double> evt, string name, double defValue, double currValue, double minValue = 0.0d, double maxValue = 1.0d, double step = 0.01d)
        {
            UIHSlider slider = new();
            slider.Name = name;
            slider.DefValue = defValue;
            slider.CurrValue = currValue;
            slider.MinValue = minValue;
            slider.MaxValue = maxValue;
            slider.Step = step;
            if (evt != null) slider.OnValueChanged += evt;
            parent.AddChild(slider);
            return slider;
        }

        /// <summary> 快速创建新对象并绑定 </summary>
        /// <typeparam name="T">Control对象类型</typeparam>
        /// <param name="name">对象内部名</param>
        /// <param name="control">重写属性</param>
        /// <param name="parent">亲对象</param>
        /// <returns>实例化的对象</returns>
        public static T Instantiate<T>(string name, ref T control, Node parent) where T : Control, new()
        {
            if (control != null) control.Name = name;
            else if (parent.NotNull() && parent.FindChild(name) is T child) control = child;
            else control = new T() { Name = name };
            if (parent != null && control != null && parent != control.GetParent()) parent.AddChild(control);
            return control;
        }

        public static KeywordPanel NewHoverTooltip(string title, string context)
        {
            var kp = UIManager.Instance.keywordPanels.Request();
            kp.TitleText = AssetManager.GetText(title);
            kp.ContentText = AssetManager.GetText(context);
            return kp;
        }

        public static void FreeHoverTooltip(KeywordPanel kp)
        {
            UIManager.Instance.keywordPanels.Release(kp);
        }
    }
}
