using Godot;
using LOI.DataStructures;
using LOI.Gameplay;
using LOI.Manager;
using LOI.UI.Elements;
using LOI.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOI.UI
{
    [GlobalClass]
    public partial class HeadsUpDisplay : VBoxContainer
    {
        public Color LifeBar_Color { get => this.LifeBar[0].Color; set => this.LifeBar[0].Color = value; }
        public Color ManaBar_Color { get => this.ManaBar[0].Color; set => this.ManaBar[0].Color = value; }
        public Color StaminaBar_Color { get => this.StaminaBar[0].Color; set => this.StaminaBar[0].Color = value; }
        public Color SwordBreezeBar_Color { get => this.SwordBreezeBar[0].Color; set => this.SwordBreezeBar[0].Color = value; }
        public double Life { get => this.LifeBar[0].Value; set => this.LifeBar[0].Value = value; }
        public double Mana { get => this.ManaBar[0].Value; set => this.ManaBar[0].Value = value; }
        public double Stamina { get => this.StaminaBar[0].Value; set => this.StaminaBar[0].Value = value; }
        public double SwordBreeze { get => this.SwordBreezeBar[0].Value; set => this.SwordBreezeBar[0].Value = value; }

        [Export] protected UIProgressBar LifeBar { get; private set; }
        [Export] protected UIProgressBar ManaBar { get; private set; }
        [Export] protected UIProgressBar StaminaBar { get; private set; }
        [Export] protected UIProgressBar SwordBreezeBar { get; private set; }
        private UIProgressBar[] Bars = new UIProgressBar[4];

        public override void _Ready()
        {
            this.Bars[0] = this.LifeBar = UIManager.Instantiate(nameof(this.LifeBar), this.LifeBar, this);
            this.Bars[1] = this.ManaBar = UIManager.Instantiate(nameof(this.ManaBar), this.ManaBar, this);
            this.Bars[2] = this.StaminaBar = UIManager.Instantiate(nameof(this.StaminaBar), this.StaminaBar, this);
            this.Bars[3] = this.SwordBreezeBar = UIManager.Instantiate(nameof(this.SwordBreezeBar), this.SwordBreezeBar, this);
            foreach (var bar in this.Bars)
                bar.Margin = new Vector4I(8, 2, 20, 2);
        }

        public void BindTo(Character character)
        {
            if (character.IsNull()) return;
            this.LifeBar[0].Color = new Color(0xFF3F3FFF);
            this.ManaBar[0].Color = new Color(0x3F3FFFFF);
            this.StaminaBar[0].Color = new Color(0xFFCF3FFF);
            this.LifeBar[0].HookValue = () => character.Life / character.Stats.MaxLife;
            this.ManaBar[0].HookValue = () => character.Mana / character.Stats.MaxMana;
            this.StaminaBar[0].HookValue = () => (double)character.Stamina / character.Stats.MaxStamina;
            this.SwordBreezeBar[0].HookValue = () => (double)character.Blade_SwordBreeze / character.Blade_SwordBreezeMax;
            this.SwordBreezeBar[0].HookColor = () => character.Blade_RiposteStance ? new Color(0x3FFFFFFF) : new Color(0xFF3F3FFF);
        }
    }
}
