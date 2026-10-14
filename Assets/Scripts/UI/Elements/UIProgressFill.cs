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

        public new ShaderMaterial Material { get => base.Material as ShaderMaterial; set => base.Material = value; }

        public UIProgressFill()
        {
            this.Material = new ShaderMaterial() { Shader = GD.Load<Shader>("res://Assets/Shaders/ProgressFill.gdshader") };
        }
    }
}
