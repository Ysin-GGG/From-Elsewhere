using LOI.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace LOI.Interface
{
    public interface ISkillArt 
    {
        public int ManaCost { get; }
        public int Cooldown { get; }
        public State GetInstance();
    }
}
