using Godot;
using LOI.Manager;
using LOI.Utility;
using System.Diagnostics;
using System.IO;

namespace LOI.UI.Elements
{
    public partial class KeywordPanel : PanelContainer
    {
        public Vector2 Offset { get; set; }
        public Vector4 Padding { get; set; }
        public string TitleText { get => this.Title.Text; set => this.Title.Text = this.SetText(this.Title.Text, value); }
        public string ContentText { get => this.Content.Text; set => this.Content.Text = this.SetText(this.Content.Text, value); }
        public new bool Visible { get => base.Visible; set => this.SetProcess(base.Visible = value); }

        [Export]
        private VBoxContainer Box;
        [Export]
        private Label Title;
        [Export]
        private TextureRect Split;
        [Export]
        private RichTextLabel Content;

        public KeywordPanel()
        {
            UIManager.Instantiate(nameof(this.Box), ref this.Box, this);
            UIManager.Instantiate(nameof(this.Title), ref this.Title, this.Box);
            UIManager.Instantiate(nameof(this.Split), ref this.Split, this.Box);
            UIManager.Instantiate(nameof(this.Content), ref this.Content, this.Box);
            this.Visible = false;
        }

        public override void _Ready()
        {
            this.Offset = new Vector2(16, 16);
            this.Padding = Vector4.Zero;// new Vector4(16, 16, 16, 16);
            this.Theme = ResourceLoader.Load<Theme>("res://Assets/Themes/UI1.tres");
            this.Split.Texture = ResourceLoader.Load<Texture2D>("res://Assets/Images/Pixel.png");
            this.Title.HorizontalAlignment = HorizontalAlignment.Center;
            this.Title.VerticalAlignment = VerticalAlignment.Center;
            this.Title.Theme = this.Theme;
            this.Content.BbcodeEnabled = true;
            this.Content.FitContent = true;
            this.Content.AutowrapMode = TextServer.AutowrapMode.Off;
            this.Content.MetaHoverStarted += this.OnMetaHoverStarted;
            this.Content.MetaHoverEnded += this.OnMetaHoverEnded;
            this.Content.MetaClicked += this.OnMetaClicked;
            this.ContentText = AssetManager.GetText(this.ContentText);
            this.Content.Theme = this.Theme;
        }

        public override void _EnterTree()
        {
            base._EnterTree();
        }


        public override void _Process(double delta)
        {
            Vector2 position = UIManager.MousePosition + this.Offset;
            Vector2 maxSize = this.GetViewportRect().Size;
            Vector2 minSize = this.Size;
            if (position.X + minSize.X > maxSize.X) position.X = maxSize.X - minSize.X;
            if (position.Y + minSize.Y > maxSize.Y) position.Y = maxSize.Y - minSize.Y;
            if (position.X < 0) position.X = 0;
            if (position.Y < 0) position.Y = 0;
            if (minSize.X > maxSize.X || this.Size.Y + this.Padding.Y + this.Padding.W > minSize.Y) this.RecaculateSize();
            this.Position = position;
        }

        public void OnMetaHoverStarted(Variant meta) { Debug.Print(meta.AsString()); }
        public void OnMetaHoverEnded(Variant meta) { }
        public void OnMetaClicked(Variant meta) { Debug.Print(meta.AsString()); }
        public void OnContentChanged(string oldContent, string newContent) { }// => this.RecaculateSize();

        private void RecaculateSize()
        {
            Vector2 screen = this.GetViewportRect().Size;
            Vector2 border = new Vector2(this.Padding.X + this.Padding.Z, this.Padding.Y + this.Padding.W);
            Vector2 size = new Vector2(this.Content.GetContentWidth(), this.Content.GetContentHeight());
            //this.Content.Position = new Vector2(this.Padding.X, this.Padding.Y);
            if (screen.X < size.X + border.X)
            {
                /*
                this.Content.Size = new Vector2(this.Panel.Size.X - border.X, size.Y);
                this.Panel.Size = new Vector2(screen.X, size.Y + border.Y);
                this.Size = this.Panel.Size;
                */
                this.Size = new Vector2(screen.X, size.Y + border.Y);
            }
            else
            {
                /*
                this.Content.Size = size;
                this.Panel.Size = size + border;
                this.Size = this.Panel.Size;
                */
                this.Size = size + border;
            }
        }

        private string SetText(string oldText, string newText)
        {
            newText = AssetManager.GetText(newText);
            this.OnContentChanged(oldText, newText);
            this.Visible = newText.NotNull();
            return newText;
        }
    }
}
