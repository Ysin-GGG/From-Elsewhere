using Godot;
using Godot.Collections;
using LOI.DataStructures;
using LOI.Manager;
using LOI.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace LOI.UI.Elements
{
    [GlobalClass]
    public partial class UIProgressBar : NinePatchRect
    {
        public Vector4I Margin { get => new Vector4I(this.PatchMarginLeft, this.PatchMarginTop, this.PatchMarginRight, this.PatchMarginBottom); set => this.SetMargin(value); }
        private List<UIProgressFill> fills { get; } = new List<UIProgressFill>();

        [Export]
        private RichTextLabel Label;

        [Export]
        private Array<UIProgressFill> _fills;

        public UIProgressFill this[int index]
        {
            get => this.fills[index];
        }

        private void SetMargin(Vector4I value)
        {
            this.PatchMarginLeft = value.X;
            this.PatchMarginTop = value.Y;
            this.PatchMarginRight = value.Z;
            this.PatchMarginBottom = value.W;
            foreach (UIProgressFill fill in this.fills)
            {
                fill.PatchMarginLeft = value.X;
                fill.PatchMarginTop = value.Y;
                fill.PatchMarginRight = value.Z;
                fill.PatchMarginBottom = value.W;
            }
        }

        public UIProgressBar()
        {
            if (this._fills.NotNull())
                foreach (UIProgressFill fill in this._fills)
                    this.AddProgressFill(fill);
        }

        public override void _Ready()
        {
            if (this._fills.NotNull())
                foreach (UIProgressFill fill in this._fills)
                    this.AddProgressFill(fill);
            foreach (Node node in this.GetChildren())
                if (node is UIProgressFill fill)
                    this.AddProgressFill(fill);
            //UIManager.Instantiate(nameof(this.Label), ref this.Label, this);
            //this.Label.SetAnchorsPreset()
        }

        public int AddProgressFill(UIProgressFill fill)
        {
            int idx = this.fills.IndexOf(fill);
            if (idx < 0)
            {
                this.fills.Add(fill);
                idx = this.fills.Count;
                fill.Bar = this;
                fill.Reparent(this);
                fill.Name = "Fill_" + idx;
                fill.Size = this.Size;
                fill.PatchMarginLeft = this.PatchMarginLeft;
                fill.PatchMarginTop = this.PatchMarginTop;
                fill.PatchMarginRight = this.PatchMarginRight;
                fill.PatchMarginBottom = this.PatchMarginBottom;
            }
            return idx;
        }
    }
}
