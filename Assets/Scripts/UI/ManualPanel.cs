using Godot;
using LOI.DataStructures;
using LOI.Manager;
using LOI.UI.Elements;
using LOI.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using FileAccess = Godot.FileAccess;

namespace LOI.UI
{
    public partial class ManualPanel : PanelContainer
    {
        //public static bool AllowLoopPage { get; set; } = true;
        private readonly static List<KeywordInfo> Manual = new List<KeywordInfo>();
        private readonly static List<KeywordInfo> Search = new List<KeywordInfo>();
        private List<KeywordInfo> Current => string.IsNullOrEmpty(this.SearchLine.Text) ? ManualPanel.Manual : ManualPanel.Search;

        [Export]
        private LineEdit SearchLine;

        [Export]
        private LineEdit PageLine;

        [Export]
        private VBoxContainer EntryList;

        [Export]
        private Label Title;

        [Export]
        private RichTextLabel Description;

        [Export]
        private Button PrevPage;

        [Export]
        private Button NextPage;

        private List<Button> btns;
        private KeywordPanel kp;

        public int Page { get => field; set => field = this.SetPage(value); }

        public int MaxPage { get => field; private set => this.Page = (field = value).Min(this.Page); }

        public override void _GuiInput(InputEvent evt)
        {
            if (evt is InputEventMouseButton mb && mb.IsPressed())
                switch (mb.ButtonIndex)
                {
                    case MouseButton.WheelLeft:
                        this.Page = 1;
                        break;
                    case MouseButton.WheelUp:
                        this.Page--;
                        break;
                    case MouseButton.WheelRight:
                        this.Page = this.MaxPage;
                        break;
                    case MouseButton.WheelDown:
                        this.Page++;
                        break;
                }
        }

        public override void _Ready()
        {
            this.btns = new List<Button>(16);
            for (int i = 0; i < this.btns.Capacity; i++)
            {
                Button btn = new Button();
                btn.Name = "Entry " + i;
                btn.Text = "";
                btn.Visible = false;
                btn.Alignment = HorizontalAlignment.Left;
                btn.ButtonDown += () => this.SelectEntry(btn);
                btn.SetMeta("Index", i);
                this.EntryList.AddChild(btn);
                this.btns.Add(btn);
            }
            this.LoadKeywords(@"res://Assets//Languages//zh-Hans//Keyword.json");
            this.PageLine.EditingToggled += this.OnToggleEdit;
            this.PrevPage.ButtonDown += () => this.Page--;
            this.NextPage.ButtonDown += () => this.Page++;
            this.Title.Text = "-";
            this.Description.Text = "";
            this.MaxPage = this.Current.Count / this.btns.Count + 1;
            this.Page = 1;
            this.Description.MetaHoverStarted += this.MetaHoverStarted;
            this.Description.MetaHoverEnded += this.MetaHoverEnded;
            this.Description.MetaClicked += this.MetaClick;
            this.SearchLine.TextChanged += this.SearchEntry;
        }

        public void LoadKeywords(string filePath)
        {
            if (FileAccess.FileExists(filePath))
                using (FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read))
                {
                    JToken token = JToken.Parse(file.GetAsText(), new() { CommentHandling = CommentHandling.Ignore });
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    foreach (JToken entry in token)
                    {
                        JToken info = entry.First;
                        if (info.SelectToken("IgnoreManual").NotNull()) continue;
                        KeywordInfo kinfo = new KeywordInfo();
                        kinfo.Key = string.Join('.', fileName, entry.Path);
                        kinfo.Meta = string.Join('.', fileName, info["Meta"].Path);
                        kinfo.Name = string.Join('.', fileName, info["Name"].Path);
                        kinfo.Description = string.Join('.', fileName, info["Description"].Path);
                        ManualPanel.Manual.Add(kinfo);
                    }
                }
            else
                throw new FileNotFoundException($"File not found: {filePath}");
        }

        public void SelectEntry(Button btn)
        {
            int index = btn.GetMeta("Index").AsInt32();
            if (index.InRange(0, this.Current.Count, true, false))
            {
                this.Title.Text = AssetManager.GetText(this.Current[index].Name);
                this.Description.Text = AssetManager.GetText(this.Current[index].Description);
            }
        }

        private void MetaHoverStarted(Variant meta)
        {
            string key = meta.AsString();
            var match = new Predicate<KeywordInfo>((t) => t.Key == key);
            int index = ManualPanel.Manual.FindIndex(match);
            KeywordInfo info = ManualPanel.Manual[index];
            this.kp = UIManager.NewHoverTooltip(info.Name, info.Description);
        }

        private void MetaHoverEnded(Variant meta)
        {
            UIManager.FreeHoverTooltip(this.kp);
            this.kp = null;
        }

        private void MetaClick(Variant meta)
        {
            string key = meta.AsString();
            var match = new Predicate<KeywordInfo>((t) => t.Key == key);
            int index = ManualPanel.Manual.FindIndex(match);
            if (index >= 0)
            {
                this.Page = index / this.btns.Count + 1;
                this.SelectEntry(this.btns[index % this.btns.Count]);
            }
        }

        private void OnToggleEdit(bool enabled)
        {
            if (enabled) this.PageLine.Text = this.Page.ToString();
            else
            {
                if (int.TryParse(this.PageLine.Text, out int page))
                    this.Page = page;
                if (this.MaxPage <= 0) this.PageLine.Text = this.PageLine.PlaceholderText; 
                else this.PageLine.Text = $"{this.Page}/{this.MaxPage}";
            }
        }

        private void SearchEntry(string match)
        {
            ManualPanel.Search.Clear();
            if (!string.IsNullOrEmpty(match))
                foreach (KeywordInfo entry in ManualPanel.Manual)
                    if (AssetManager.HasText(entry.Description) && AssetManager.GetText(entry.Description).Contains(match)) ManualPanel.Search.Add(entry); // 会将url中的索引也进行判定，待改进
                    else if (AssetManager.HasText(entry.Name) && AssetManager.GetText(entry.Name).Contains(match)) ManualPanel.Search.Add(entry);
            this.MaxPage = this.Current.Count / this.btns.Count + 1;
        }

        private int SetPage(int page)
        {
            page = page.Clamp(1, this.MaxPage);
            int index = page * this.btns.Capacity - this.btns.Capacity;
            for (int i = 0, j = index; i < this.btns.Capacity; i++, j++)
                if (this.btns[i].Visible = this.Current.Count > j)
                {
                    this.btns[i].Text = AssetManager.GetText(this.Current[j].Name);
                    this.btns[i].SetMeta("Index", j);
                }
            if (this.MaxPage <= 0) this.PageLine.Text = this.PageLine.PlaceholderText;
            else this.PageLine.Text = $"{page}/{this.MaxPage}";
            return page;
        }

        private class KeywordInfo()
        {
            public string Key { get; set; }

            public string Meta { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }
    }
}
