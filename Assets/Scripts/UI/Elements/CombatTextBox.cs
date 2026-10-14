using Godot;
using LOI.Manager;
using LOI.Utility;

namespace LOI.UI.Elements
{
    public partial class CombatTextBox : HBoxContainer
    {
        public string Text { get => this.Label.Text; set => this.Label.Text = value; }
        public int TextSize { get => this.Label.LabelSettings.FontSize; set => this.Label.LabelSettings.FontSize = value; }
        public Font TextFont { get => this.Label.LabelSettings.Font; set => this.Label.LabelSettings.Font = value; }
        public Color TextColor { get => this.Label.LabelSettings.FontColor; set => this.Label.LabelSettings.FontColor = value; }
        public Vector2 Velocity { get; set; }

        [Export]
        private TextureRect Icon;
        [Export]
        private Label Label;
        [Export]
        private Curve2D ScaleCurve;

        private int time;

        private static Curve2D normal;
        private static Curve2D critial;

        public CombatTextBox()
        {
            if (CombatTextBox.normal == null) 
            {
                CombatTextBox.normal = new Curve2D();
                CombatTextBox.normal.AddPoint(new Vector2(0f, 0f));
                CombatTextBox.normal.AddPoint(new Vector2(.3f, 1f));
                CombatTextBox.normal.AddPoint(new Vector2(.7f, 1f));
                CombatTextBox.normal.AddPoint(new Vector2(1f, 0f));
            }
            if (CombatTextBox.critial == null) 
            {
                CombatTextBox.critial = new Curve2D();
                CombatTextBox.critial.AddPoint(new Vector2(0f, 0f));
                CombatTextBox.critial.AddPoint(new Vector2(.2f, 1f));
                CombatTextBox.critial.AddPoint(new Vector2(.8f, 1f));
                CombatTextBox.critial.AddPoint(new Vector2(1f, 0f));
            }
            if (this.Icon == null) this.AddChild(this.Icon = new TextureRect());
            if (this.Label == null) this.AddChild(this.Label = new Label() { LabelSettings = new LabelSettings() });
        }

        public void SetByDamage(int time, double damage, bool crit, Vector2 position)
        {
            this.time = time;
            this.Icon.Visible = crit;
            this.TextColor = new Color(1f, .66f, 0f, 1f);
            this.Velocity = Vector2.Up;
            this.Scale = Vector2.One;
            this.Size = Vector2.Zero;
            this.Text = damage.ToString("#");
            this.Position = position;
            this.Visible = true;
            this.SetProcess(true);
            if (crit)
            {
                this.TextColor = new Color(1f, .33f, 0f, 1f);
                this.time += time;
                this.Text += '!';
                if (this.time > 100) this.time = 100;
            }
        }

        public override void _Ready()
        {
            this.SetProcess(false);
            this.Visible = false;
            this.ClipContents = true;
            this.Alignment = AlignmentMode.Center;
            this.PivotOffsetRatio = new Vector2(.5f, .5f);

            this.Icon.Name = nameof(this.Icon);
            this.Icon.Texture = AssetManager.Instance.CombatTextIcon;
            this.Icon.ExpandMode = TextureRect.ExpandModeEnum.FitWidth;
            this.Icon.StretchMode = TextureRect.StretchModeEnum.Scale;

            this.Label.Name = nameof(this.Label);
            this.Label.LabelSettings.Font = AssetManager.Instance.CombatTextFont;
            this.Label.LabelSettings.FontColor = new Color(1f, .5f, 0f, 1f);
            this.Label.LabelSettings.FontSize = 18;
        }

        public override void _Process(double delta)
        {
            this.Position += this.Velocity;
            this.Velocity *= .98f;
            if (this.Velocity.X.Abs() < .01f) this.Velocity = new Vector2(0f, this.Velocity.Y);
            if (this.Velocity.Y.Abs() < .01f) this.Velocity = new Vector2(this.Velocity.X, 0f);
            if (this.Icon.Visible)
            {
                this.Scale *= 1 + (this.time / 12000f).Round(3);
                this.Velocity *= 1.01f;
            }
            if (--this.time < 0)
            {
                this.Scale *= .95f;
                if (this.Scale.X < .2f || this.Scale.Y < .2f)
                {
                    this.Visible = false;
                    this.SetProcess(false);
                    UIManager.Instance.combatTextBoxes.Release(this);
                }
            }
        }
    }
}
