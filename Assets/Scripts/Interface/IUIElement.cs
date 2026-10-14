
using Godot;
using LOI.UI.Elements;

namespace LOI.Interface
{
    public interface IUIElement<T> where T : Control
    {
        protected T Element { get; }
    }

    public abstract class UIElement<T> where T : Control, new()
    {
        public string Name { get => this.Element.Name; set => this.Element.Name = value; }

        public bool Enable { get => this.Element.Visible; set { this.Element.Visible = value; this.Element.SetProcess(value); this.Element.SetPhysicsProcess(value); } }

        protected T Element { get; }

        public UIElement() => this.Element = new T();

        public void AddToNode(Node parent) => this.Element.Reparent(parent);

        public static implicit operator Node(UIElement<T> ele) => ele.Element;
    }
}
