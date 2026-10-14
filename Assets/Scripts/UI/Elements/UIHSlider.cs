using Godot;
using LOI.Utility;
using System;

namespace LOI.UI.Elements
{
    [GlobalClass]
    public partial class UIHSlider : HBoxContainer
    {
        public new string Name { get => base.Name; set => base.Name = this.Title.Text = value; }
        public double MinValue { get => this.Slider.MinValue; set => this.Slider.MinValue = value; }
        public double MaxValue { get => this.Slider.MaxValue; set => this.Slider.MaxValue = value; }
        public double DefValue { get => field; set => field = value.Clamp(this.MinValue, this.MaxValue); }
        public double CurrValue { get => this.Slider.Value; set => this.SetValue(value); }
        public double PrevValue { get => field; private set => field = value; }
        public double Step { get => this.Slider.Step; set => this.Slider.Step = value; }
        public bool Rounded { get => this.Slider.Rounded; set => this.Slider.Rounded = value; }
        public bool Editable { get => this.Slider.Editable; set => this.Slider.Editable = value; }
        public bool Scrollable { get => this.Slider.Scrollable; set => this.Slider.Scrollable = value; }
        public bool ButtonFlat { get => this.Reset.Flat; set => this.Reset.Flat = value; }
        public string ValueFormat { get => field; set => this.Value.Text = this.CurrValue.ToString(field = value); }
        public Color SliderColor { get => this.Slider.Modulate; set => this.Slider.Modulate = value; }
        public Texture2D ButtonIcon { get => this.Reset.Icon; set => this.Reset.Icon = value; }

        public event Action<double> OnValueChanged;
        public event Action<double> OnValueReset;

        [Export]
        private Label Title;
        [Export]
        private Label Value;
        [Export]
        private Button Reset;
        [Export]
        private HSlider Slider;

        private bool ValueLock = true;

        public UIHSlider() => this._EnterTree();

        public override void _EnterTree()
        {
            if (this.ValueLock)
            {
                if (this.Title == null) this.AddChild(this.Title = new() { Name = nameof(this.Title) });
                if (this.Slider == null)
                {
                    this.AddChild(this.Slider = new() { Name = nameof(this.Slider) });
                    this.Slider.ValueChanged += (value) =>
                    {
                        this.Value.Text = value.ToString(this.ValueFormat);
                        this.ValueLock = true;
                        this.OnValueChanged?.Invoke(this.Slider.Value);
                        this.ValueLock = false;
                    };
                }
                if (this.Value == null) this.AddChild(this.Value = new() { Name = nameof(this.Value) });
                if (this.Reset == null)
                {
                    this.AddChild(this.Reset = new() { Name = nameof(this.Reset) });
                    this.Reset.ButtonUp += () =>
                    {
                        this.CurrValue = this.DefValue;
                        this.OnValueReset?.Invoke(this.CurrValue);
                    };
                }
                this.Title.HorizontalAlignment = HorizontalAlignment.Center;
                this.Title.VerticalAlignment = VerticalAlignment.Center;
                this.Title.AutowrapMode = TextServer.AutowrapMode.Arbitrary;
                this.Title.CustomMinimumSize = new Vector2(256f, 0f);
                this.Title.Text = base.Name;
                this.Value.HorizontalAlignment = HorizontalAlignment.Center;
                this.Value.VerticalAlignment = VerticalAlignment.Center;
                this.Value.AutowrapMode = TextServer.AutowrapMode.Arbitrary;
                this.Value.CustomMinimumSize = new Vector2(64f, 0f);
                this.Slider.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                this.Slider.SizeFlagsVertical = SizeFlags.ShrinkCenter;
                this.ButtonIcon = GD.Load<Texture2D>("res://Assets/Images/UI/ResetIcon.png");
                this.ButtonFlat = true;
                this.Rounded = false;
                this.ValueLock = false;
                this.Step = 0.01d;
                this.MinValue = 0.0d;
                this.MaxValue = 1.0d;
                this.DefValue = 0.5d;
                this.CurrValue = this.DefValue;
                this.ValueFormat = "p0";
            }
        }

        private void SetValue(double value)
        {
            if (this.ValueLock) 
                GD.PushWarning($"Stackflow Warning: Try to set '{nameof(UIHSlider.CurrValue)}' in callback '{nameof(UIHSlider.OnValueChanged)}'");
            else if (this.CurrValue != value)
            {
                this.PrevValue = this.Slider.Value;
                this.Slider.Value = value.Clamp(this.MinValue, this.MaxValue);
                this.Value.Text = value.ToString(this.ValueFormat);
            }
        }
    }
}
