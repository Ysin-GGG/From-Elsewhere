using Godot;

namespace LOI.Manager
{
    public partial class LOIManager : Node
    {
        public const double UpdateThreshold = 1.0 / 60.0;
        public static double RFPS {  get; private set; }
        public static double PFPS {  get; private set; }

        public Variant this[string name]
        {
            get => this._Get(name);
            set => this._Set(name, value);
        }

        public sealed override void _Ready() => this.Initialization();

        public sealed override void _Process(double delta) { LOIManager.RFPS = 1d / delta; this.RenderUpdate(); }

        public sealed override void _PhysicsProcess(double delta) { LOIManager.PFPS = 1d / delta; this.GameUpdate(); }

        public sealed override void _Input(InputEvent evt) => this.InputUpdate(evt);

        public sealed override void _EnterTree() =>this.EnterTree();
        
        public sealed override void _ExitTree() => this.ExitTree();

        protected virtual void EnterTree() { }
        protected virtual void ExitTree() { }
        protected virtual void Initialization() { }
        protected virtual void RenderUpdate() { }
        protected virtual void GameUpdate() { }
        protected virtual void InputUpdate(InputEvent evt) { }
    }
}
