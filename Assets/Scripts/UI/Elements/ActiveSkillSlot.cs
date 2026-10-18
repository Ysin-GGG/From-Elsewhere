using Godot;
using LOI.Gameplay;
using LOI.Interface;
using LOI.Manager;
using LOI.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Text;

namespace LOI.UI.Elements
{
    [GlobalClass]
    public partial class ActiveSkillSlot : TextureRect
    {
        [Export] private PanelContainer Box;
        [Export] private Label CooldownLabel;

        public double CooldownValue { get => field; set => this.Material.SetShaderParameter(nameof(this.CooldownValue), field = value); } = .5d;
        public new Vector2 Size { get => base.Size; set => this.CustomMinimumSize = base.Size = value; }
        public new ShaderMaterial Material { get => base.Material as ShaderMaterial; set => base.Material = value is ShaderMaterial sm ? sm : base.Material; }
        public int FontSize { get => this.CooldownLabel.GetThemeFontSize("font_size"); set => this.CooldownLabel.AddThemeFontSizeOverride("font_size", value); }
        public Color FontShadowColor { get => this.CooldownLabel.GetThemeColor("font_shadow_color"); set => this.CooldownLabel.AddThemeColorOverride("font_shadow_color", value); }
        public int ShadowOutlineSize { get => this.CooldownLabel.GetThemeConstant("shadow_outline_size"); set => this.CooldownLabel.AddThemeConstantOverride("shadow_outline_size", value); }
        public int ShadowOffsetX { get => this.CooldownLabel.GetThemeConstant("shadow_offset_x"); set => this.CooldownLabel.AddThemeConstantOverride("shadow_offset_x", value); }
        public int ShadowOffsetY { get => this.CooldownLabel.GetThemeConstant("shadow_offset_y"); set => this.CooldownLabel.AddThemeConstantOverride("shadow_offset_y", value); }
        public Vector2I ShadowOffset { get => new Vector2I(this.ShadowOffsetX, this.ShadowOffsetY); set { this.ShadowOffsetX = value.X; this.ShadowOffsetY = value.Y; } }
        public Vector4 BoxMargin { get => this.GetMargin(); set => this.SetMargin(value); }
        public Texture2D BoxTexture { get => this.sbt.Texture; set => this.sbt.Texture = value; }

        private SkillInfo info;
        private StyleBoxTexture sbt;
        private static Shader defShader = GD.Load<Shader>("res://Assets/Shaders/SkillIcon.gdshader");
        private static Texture2D defBox = GD.Load<Texture2D>("res://Assets/Images/UI/Panel/Panel_1.png");

        public ActiveSkillSlot()
        {
            this.Box = UIManager.Instantiate(nameof(this.Box), ref this.Box, this);
            this.CooldownLabel = UIManager.Instantiate(nameof(this.CooldownLabel), ref this.CooldownLabel, this);
            if (this.Material.IsNull()) this.Material = new ShaderMaterial() { Shader = ActiveSkillSlot.defShader.Duplicate() as Shader };
            else if (this.Material.Shader.IsNull()) this.Material.Shader = ActiveSkillSlot.defShader.Duplicate() as Shader;
        }

        private Vector4 GetMargin()
        {
            Vector4 margin = Vector4.Zero;
            if (this.sbt != null)
            {
                margin.X = this.sbt.TextureMarginLeft;
                margin.Y = this.sbt.TextureMarginTop;
                margin.Z = this.sbt.TextureMarginRight;
                margin.W = this.sbt.TextureMarginBottom;
            }
            return margin;
        }

        private bool SetMargin(Vector4 margin)
        {
            if (this.sbt != null)
            {
                this.sbt.TextureMarginLeft = margin.X;
                this.sbt.TextureMarginTop = margin.Y;
                this.sbt.TextureMarginRight = margin.Z;
                this.sbt.TextureMarginBottom = margin.W;
                return true;
            }
            return false;
        }

        public override void _Ready()
        {
            this.ExpandMode = ExpandModeEnum.IgnoreSize;
            if (this.Box.GetThemeStylebox("panel") is StyleBoxTexture sbt) // 先确保Box不为null
                this.sbt = sbt;
            else
            {
                this.sbt = new StyleBoxTexture();
                this.Box.AddThemeStyleboxOverride("panel", this.sbt);
            }
            this.Box.SetAnchorsPreset(LayoutPreset.FullRect);
            this.BoxMargin = Vector4.One * 2;
            this.BoxTexture = ActiveSkillSlot.defBox;

            this.CooldownLabel.SetAnchorsPreset(LayoutPreset.FullRect);
            this.CooldownLabel.HorizontalAlignment = HorizontalAlignment.Center;
            this.CooldownLabel.VerticalAlignment = VerticalAlignment.Center;
            this.CooldownLabel.ClipText = true;
            this.FontSize = 24;
            this.FontShadowColor = new Color(0xFF);
            this.ShadowOffset = Vector2I.Zero;
            this.ShadowOutlineSize = 4;

            this.CooldownValue = 0d;
        }

        public void BindTo(SkillInfo skill)
        {
            this.info?.CooldownChanged -= this.HookValue;
            this.info = skill;
            this.Texture = skill?.Icon;
            this.info?.CooldownChanged += this.HookValue;
            this.Visible = skill != null;
        }

        private void HookValue(double cd)
        {
            this.CooldownValue = cd;
            this.CooldownLabel.Text = string.Format("{0:F1}s", this.info.Cooldown2 / 60f);
            this.CooldownLabel.Visible = cd > 0;
        }
    }
}
