using LOI.Interface;
using LOI.Manager;
using LOI.States;
using LOI.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace LOI.Gameplay
{
    public class SkillInfo
    {
        private double Cooldown { get; set; }
        private ISkillArt skill;
        private State Instance;

        public SkillInfo(ISkillArt skill)
        {
            this.Instance = skill.GetInstance();
        }

        public bool Apply(Character character)
        {
            if(character.Mana >= this.skill.ManaCost)
            character.ChangeState(this.Instance);
            return true;
        }

        public void SetCooldown(double cooldown) => this.Cooldown = cooldown;

        public void SetCooldown(int cooldown) => this.Cooldown = (double)cooldown / skill.Cooldown;
    }
}
