using Godot;
using LOI.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace LOI.UI.Elements
{
    public class UICountdownBar : UIElement<ProgressBar>
    {
        public int TimeSet { get; set; }

        public bool Pause { get; set; }


        public event Action OnTimeUp;

        public UICountdownBar() 
        {
        }
    }
}
