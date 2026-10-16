using LOI.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace LOI.States
{
    public class SkillArt : State, ISkillArt
    {
        public State Instance { get; }
        public int ManaCost { get; }
        public int Cooldown { get; }

        public State GetInstance() => this;
    }
}
