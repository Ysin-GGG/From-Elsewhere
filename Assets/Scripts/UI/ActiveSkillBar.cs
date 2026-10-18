using Godot;
using LOI.Gameplay;
using LOI.Manager;
using LOI.UI.Elements;
using System.Collections.Generic;

namespace LOI.UI
{
    [GlobalClass]
    public partial class ActiveSkillBar : GridContainer
    {
        public Vector2 IconSize { get => field; set => this.SetIconSize(field = value); }
        private List<ActiveSkillSlot> slots;

        public ActiveSkillBar()
        {
            this.Columns = 4;
            this.slots = new List<ActiveSkillSlot>(this.Columns);
            for (int i = 0; i < this.Columns; i++)
                this.slots.Add(null);
        }

        public override void _Ready()
        {
            for (int i = 0; i < this.Columns; i++)
                this.slots[i] = UIManager.Instantiate<ActiveSkillSlot>("SkillIcon" + i, null, this);
            this.IconSize = Vector2.One * 64f;
        }


        private void SetIconSize(Vector2 size)
        {
            foreach (ActiveSkillSlot slot in this.slots)
                slot.Size = size;
        }

        public void BindTo(Character character)
        {
            this.Columns = character.SkillInfo.Length;
            for (int i = 0; i < this.Columns; i++)
                if (i < this.slots.Count)
                    this.slots[i].BindTo(character.SkillInfo[i]);
        }
    }
}