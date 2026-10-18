using Godot;
using LOI.DataStructures;
using LOI.Interface;
using LOI.Manager;
using LOI.States;
using LOI.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace LOI.Gameplay
{
    public class SkillInfo
    {
        public double Cooldown { get => field; set { field = value; this.CooldownChanged?.Invoke(value); } }
        public ISkillArt skill;
        private State instance;
        public Texture2D Icon { get; }
        public event Action<double> CooldownChanged;

        public int Cooldown2 { get => (int)(this.Cooldown * this.skill.Cooldown); set => this.Cooldown = (double)value / this.skill.Cooldown; }

        public SkillInfo(ISkillArt skill, Texture2D icon)
        {
            this.instance = skill?.GetInstance();
            this.skill = skill;
            this.Icon = icon;
        }

        public bool TryApply(Character character, bool changeState = false)
        {
            if (this.Cooldown > 0) return false;
            if (character.Mana < this.skill.ManaCost) return false;
            if (character.StateType != this.instance.ST) return false;
            if (!character.Ctrl) return false;
            if (changeState) character.ChangeState(this.instance);
            character.Mana -= this.skill.ManaCost;
            this.Cooldown = 1d;
            return true;
        }

        public static implicit operator State(SkillInfo info) => info.instance;

        public void SetCooldown(int cooldown) => this.Cooldown = ((double)cooldown / this.skill?.Cooldown) ?? 0;
    }
}
