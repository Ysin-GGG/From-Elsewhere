using Godot;
using Godot.Collections;
using LOI.Utility;
using System;
using System.Collections.Generic;

namespace LOI.UI.Elements
{
    [GlobalClass]
    public partial class UIProgressFill : NinePatchRect
    {
        public UIProgressBar Bar { get => field; internal set => field = value; }

        public Color Color { get => field; set => this.Material.SetShaderParameter(nameof(this.Color), field = value); } = new Color(int.MaxValue);
        public double Value { get => field; set => this.Material.SetShaderParameter(nameof(this.Value), field = value); } = .5d;
        public Func<double> HookValue;
        public Func<Color> HookColor;

        public new ShaderMaterial Material { get => base.Material as ShaderMaterial; set => base.Material = value; }

        public UIProgressFill()
        {
            if(this.Material.IsNull()) this.Material = new ShaderMaterial() { Shader = GD.Load<Shader>("res://Assets/Shaders/ProgressFill.gdshader") };
        }

        public override void _EnterTree()
        {
            if(this.Material.IsNull()) this.Material = new ShaderMaterial() { Shader = GD.Load<Shader>("res://Assets/Shaders/ProgressFill.gdshader") };
        }

        public override void _Process(double delta)
        {
            if (this.HookValue != null) this.Value = this.HookValue.Invoke();
            if (this.HookColor != null) this.Color = this.HookColor.Invoke();
        }
    }
}
