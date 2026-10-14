using Godot;
using LOI.DataStructures;
using LOI.Gameplay;
using LOI.UI.Elements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOI.UI
{
    public partial class CharacterInfo : PanelContainer
    {
        [Export]
        public GridContainer DualColumnLeft { get; private set; }
        [Export]
        public GridContainer DualColumnRight { get; private set; }

        private Dictionary<Statistic, (RichTextLabel, RichTextLabel)> dict;

        public CharacterInfo()
        {
            this.dict = new();
            if (this.DualColumnLeft == null) this.DualColumnLeft = new();
        }

        public override void _Ready()
        {
            this.DualColumnLeft.Name = nameof(this.DualColumnLeft);
            this.DualColumnLeft.Columns = 2;
            this.DualColumnLeft.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            this.DualColumnLeft.AddThemeConstantOverride("h_separation", 16);
        }

        public void LoadCharacter(Character character)
        {
            foreach (Statistic stat in this.dict.Keys)
                this.RemoveStat(stat);
            foreach (Statistic stat in character.Stats.Array)
                this.AddStat(stat);
        }

        public bool AddStat(Statistic stat)
        {
            if (!this.dict.ContainsKey(stat))
            {
                var pair 
                = (new RichTextLabel() {
                    Name = "StatName_" + this.dict.Count,
                    FitContent = true,
                    ScrollActive = false,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    SizeFlagsHorizontal = SizeFlags.ExpandFill,
                    CustomMinimumSize = new Vector2(0f, 24),
                }, new RichTextLabel() {
                    Name = "StatValue_" + this.dict.Count,
                    FitContent = true,
                    ScrollActive = false,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    SizeFlagsHorizontal = SizeFlags.ExpandFill,
                    CustomMinimumSize = new Vector2(0f, 24),
                });
                this.DualColumnLeft.AddChild(pair.Item1);
                this.DualColumnRight.AddChild(pair.Item2);
                this.dict.Add(stat, pair);
                this.UpdateText(stat);
                stat.Updated += this.UpdateText;
                return true;
            }
            return false;
        }

        public bool RemoveStat(Statistic stat)
        {
            if (this.dict.TryGetValue(stat, out var pair))
            {
                stat.Updated -= this.UpdateText;
                pair.Item1.Free();
                pair.Item2.Free();
                this.dict.Remove(stat);
                return true;
            }
            return false;
        }

        private void UpdateText(Statistic stat)
        {
            if (this.dict.TryGetValue(stat, out var pair)) 
            {
                pair.Item1.Text = stat.DisplayName;
                pair.Item2.Text = stat.ToString();
            }
        }

        public void UpdateAll()
        {
            foreach (Statistic stat in this.dict.Keys)
                this.UpdateText(stat);
        }
    }
}
