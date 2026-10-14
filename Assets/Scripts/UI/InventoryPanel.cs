using Godot;
using Godot.Collections;
using LOI.Gameplay;
using LOI.Manager;
using LOI.Utility;
using System;
using System.Data.Common;
using System.Linq;
using System.Text;
using static LOI.States.BladeArt.DriveSlash;
using static LOI.States.State.Model;

namespace LOI.UI
{
    [Tool]
    [GlobalClass]
    public partial class InventoryPanel : Control
    {
        [ExportGroup("Constants")]
        [Export]
        public Vector2I GridSize { get => field; set => this.Refresh(field = value); } = new Vector2I(12, 12);
        [ExportGroup("Constants")]
        [Export]
        public Vector2I GridCellSize { get => field; set => this.Refresh(field = value); } = new Vector2I(64, 64);
        [ExportGroup("Constants")]
        [Export]
        public Vector2I GridSeparation { get => field; set => this.Refresh(field = value); } = new Vector2I(4, 4);
        [ExportGroup("Constants")]
        [Export]
        public Vector2I BorderSize { get => field; set => this.Refresh(field = value); } = new Vector2I(8, 8);
        [ExportGroup("Styles")]
        [Export]
        public StyleBoxTexture SelectBox { get => field; set => this.Refresh(field = value); }
        [ExportGroup("Styles")]
        [Export]
        public StyleBoxTexture BackgroundBox { get => field; set => this.Refresh(field = value); }
        [ExportGroup("Styles")]
        [Export]
        public StyleBoxTexture CellBox { get => field; set => this.Refresh(field = value); }
        [ExportGroup("Styles")]
        [Export]
        public StyleBoxTexture CellBackgroundBox { get => field; set => this.Refresh(field = value); }
        [ExportGroup("Styles")]
        [Export]
        public StyleBoxTexture ItemBox { get => field; set => this.Refresh(field = value); }
        [ExportGroup("Styles")]
        [Export]
        public StyleBoxTexture ItemBackgroundBox { get => field; set => this.Refresh(field = value); }

        private Rect2 targetRect;
        private Vector2I oldMouseCellPosition;
        private Vector2I selectedCellPosition;

        public Inventory Inventory { get => field; set => this.Refresh(field = this.LoadInventory(value)); }
        private Array<Rect2I> Content = new Array<Rect2I>();
        private Array<Color> Colors = new Array<Color>();
        private Panel DraggedItemBox1;
        private Panel DraggedItemBox2;
        private Panel SelectedItemBox;
        private Item MouseItem { get => field; set => this.MouseItemUpdated(field = value); }

        public InventoryPanel()
        {
            UIManager.Instantiate(nameof(this.SelectedItemBox), ref this.SelectedItemBox, this);
            UIManager.Instantiate(nameof(this.DraggedItemBox1), ref this.DraggedItemBox1, this);
            UIManager.Instantiate(nameof(this.DraggedItemBox2), ref this.DraggedItemBox2, this.DraggedItemBox1);
        }

        public override void _Ready()
        {
            this.MouseExited += () => this.oldMouseCellPosition = -Vector2I.One;
            this.DraggedItemBox1.MouseFilter = MouseFilterEnum.Ignore;
            this.DraggedItemBox2.MouseFilter = MouseFilterEnum.Ignore;
            this.SelectedItemBox.MouseFilter = MouseFilterEnum.Ignore;
            this.DraggedItemBox2.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            this.DraggedItemBox2.AddThemeStyleboxOverride("panel", this.ItemBox);
            this.SelectedItemBox.AddThemeStyleboxOverride("panel", this.SelectBox);
            this.Inventory = new Inventory(10, 12)
                .SetItem(new Item(3, 1) { RarityColor = new Color("FF0000FF") }, 0, 1)
                .SetItem(new Item(1, 5) { RarityColor = new Color("00FF00FF") }, 4, 5)
                .SetItem(new Item(2, 2) { RarityColor = new Color("0000FFFF") }, 8, 8);
        }

        public override void _Process(double delta)
        {
            if (this.targetRect == new Rect2I(Vector2I.Zero, Vector2I.Zero)) this.SelectedItemBox.Visible = false;
            else if (this.SelectedItemBox.Visible && this.SelectedItemBox.Position.DistanceTo(this.targetRect.Position) > 1f)
            {
                this.SelectedItemBox.Position = this.SelectedItemBox.Position.Lerp(this.targetRect.Position, .2f);
                this.SelectedItemBox.Size = this.SelectedItemBox.Size.Lerp(this.targetRect.Size, .2f);
            }
            else
            {
                this.SelectedItemBox.Position = this.targetRect.Position;
                this.SelectedItemBox.Size = this.targetRect.Size;
                this.SelectedItemBox.Visible = true;
            }
            if (this.MouseItem.NotNull())
            {
                this.DraggedItemBox1.Position = this.GetLocalMousePosition() - this.DraggedItemBox1.Size / 2;
            }
        }

        public override void _GuiInput(InputEvent evt)
        {
            if (evt is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)
            {
                Vector2I cellPosition = this.ToCellPosition(mouseButton.Position);
                if (cellPosition.X.InRange(0, this.GridSize.X) && cellPosition.Y.InRange(0, this.GridSize.Y))
                    if (mouseButton.Pressed) this.OnClickCell(cellPosition);
                    else if (this.Inventory.HasItem(this.selectedCellPosition))
                    {
                        Vector2 localSize = this.ToLocalSize(this.MouseItem.Size);
                        Vector2 localPosition = this.ToLocalPosition(this.ToCellPosition(mouseButton.Position - localSize / 2f, true));
                        if (this.Inventory.HasItem(cellPosition))
                            this.Inventory.SwapItem(this.Inventory.GetItemPosition(this.selectedCellPosition), this.Inventory.GetItemPosition(this.ToCellPosition(mouseButton.Position - localSize / 2f, true)));
                        else this.Inventory.MoveItem(this.Inventory.GetItemPosition(this.selectedCellPosition), this.Inventory.GetItemPosition(this.ToCellPosition(mouseButton.Position - localSize / 2f, true)));
                    }
                this.MouseItem = mouseButton.Pressed ? this.Inventory.GetItem(cellPosition) : null;
            }
            if (evt is InputEventMouseMotion mouseMotion)
            {
                Vector2 mousePosition = mouseMotion.Position;
                int column = (int)((mousePosition.X - this.BorderSize.X) / (this.GridCellSize.X + this.GridSeparation.X));
                int line = (int)((mousePosition.Y - this.BorderSize.Y) / (this.GridCellSize.Y + this.GridSeparation.Y));
                Vector2I cellPosition = new Vector2I(column, line);
                bool inRange = column.InRange(0, this.GridSize.X) && line.InRange(0, this.GridSize.Y);
                if (inRange)
                {
                    if (this.MouseItem.NotNull())
                    {
                        Vector2 localSize = this.ToLocalSize(this.MouseItem.Size);
                        Vector2 localPosition = this.ToLocalPosition(this.ToCellPosition(mousePosition - localSize / 2f, true));
                        this.targetRect = new Rect2(localPosition, localSize);
                    }
                    else if (this.oldMouseCellPosition != cellPosition)
                    {
                        this.targetRect = new Rect2(this.ToLocalPosition(cellPosition), this.GridCellSize);
                        foreach (Rect2I item in this.Content)
                            if (cellPosition.X.InRange(item.Position.X, item.Position.X + item.Size.X, true, false) && cellPosition.Y.InRange(item.Position.Y, item.Position.Y + item.Size.Y, true, false))
                            {
                                this.targetRect.Position = this.ToLocalPosition(item.Position);
                                this.targetRect.Size = item.Size * (this.GridCellSize + this.GridSeparation) - this.GridSeparation;
                                break;
                            }
                    }
                    this.oldMouseCellPosition = cellPosition;
                }
                else
                {
                    this.oldMouseCellPosition = -Vector2I.One;
                    this.targetRect = new Rect2(Vector2.Zero, Vector2.Zero);
                }
            }
        }

        public override void _Draw()
        {
            Vector2 position;
            Vector2 size = this.GridCellSize;
            Rect2 background = new Rect2(Vector2.One * this.BorderSize, this.GridSize * (this.GridCellSize + this.GridSeparation) - Vector2.One * this.GridSeparation);
            Rect2 border = new Rect2(Vector2.Zero, 2 * background.Position + background.Size);
            this.DrawStyleBox(this.BackgroundBox, border);
            for (int i = 0; i < this.GridSize.Y; i++)
            {
                position.X = this.BorderSize.X;
                position.Y = i * (this.GridCellSize.Y + this.GridSeparation.Y) + this.BorderSize.Y;
                for (int j = 0; j < this.GridSize.X; j++)
                {
                    Rect2 cell = new Rect2(position, size);
                    this.DrawStyleBox(this.CellBackgroundBox, cell);
                    this.DrawStyleBox(this.CellBox, cell);
                    position.X += this.GridCellSize.X + this.GridSeparation.X;
                }
            }
            for (int i = 0; i < this.Content.Count; i++)
            {
                Rect2I item = this.Content[i];
                position = this.ToLocalPosition(item.Position);
                size = this.ToLocalSize(item.Size);
                size.X = size.X.Clamp(0, background.Size.X);
                size.Y = size.Y.Clamp(0, background.Size.Y);
                Rect2 cell = new Rect2(position, size);
                StyleBoxTexture box = this.ItemBackgroundBox.Duplicate() as StyleBoxTexture;
                box.ModulateColor = i < this.Colors.Count ? this.Colors[i] : new Color("FFFFFFFF");
                this.DrawStyleBox(box, cell);
                this.DrawStyleBox(this.ItemBox, cell);
            }
        }

        public override void _ValidateProperty(Dictionary property)
        {
            string name = property["name"].AsString();
            if (name == "size" || name == "custom_minimum_size")
                property["usage"] = (int)property["usage"] | (int)PropertyUsageFlags.ReadOnly;
        }

        private Inventory LoadInventory(Inventory inventory)
        {
            inventory?.Updated += this.OnInventoryUpdated;
            this.Inventory?.Updated -= this.OnInventoryUpdated;
            this.GridSize = inventory?.Size ?? Vector2I.Zero;
            foreach (Item item in inventory)
            {
                this.Content.Add(new Rect2I(inventory.GetItemPosition(item), item.Size));
                this.Colors.Add(item.RarityColor);
            }
            return inventory;
        }

        private void OnInventoryUpdated()
        {
            this.Content.Clear();
            this.Colors.Clear();
            foreach (Item item in this.Inventory)
            {
                this.Content.Add(new Rect2I(this.Inventory.GetItemPosition(item), item.Size));
                this.Colors.Add(item.RarityColor);
            }
            this.Refresh();
        }

        private void Refresh(object obj = null)
        {
            this.Size = this.CustomMinimumSize = this.GridSize * (this.GridCellSize + this.GridSeparation) + (2 * this.BorderSize - this.GridSeparation) * Vector2.One;
            this.QueueRedraw();
        }

        private void MouseItemUpdated(Item item)
        {
            if (item.NotNull())
            {
                StyleBoxTexture draggedItemStyle = this.ItemBackgroundBox.Duplicate() as StyleBoxTexture;
                draggedItemStyle.ModulateColor = item.RarityColor;
                this.DraggedItemBox1.Size = item.Size * this.GridCellSize + (item.Size - Vector2.One) * this.GridSeparation;
                this.DraggedItemBox1.Visible = true;
                this.DraggedItemBox1.AddThemeStyleboxOverride("panel", draggedItemStyle);
            }
            else this.DraggedItemBox1.Visible = false;

        }

        private void OnClickCell(Vector2I cellPosition)
        {
            this.selectedCellPosition = cellPosition;
        }

        private Vector2I ToCellPosition(Vector2 position, bool round = false)
        {
            position = (position - this.BorderSize) / (this.GridCellSize + this.GridSeparation);
            Vector2I cellPosition = round ? LazyMath.Round(position) : (Vector2I)position;
            if(cellPosition.X.InRange(0,this.GridSize.X) && cellPosition.Y.InRange(0,this.GridSize.Y))
                return cellPosition;
            return -Vector2I.One;
        }

        private Vector2 ToLocalPosition(Vector2I cellPosition)
            => cellPosition * (this.GridCellSize + this.GridSeparation) + this.BorderSize;

        private Vector2 ToLocalSize(Vector2I cellSize)
            => cellSize * (this.GridCellSize + this.GridSeparation) - this.GridSeparation;

        private bool IsValidCellPosition(Vector2I cellPosition)
            => cellPosition.X.InRange(0, this.GridSize.X) && cellPosition.Y.InRange(0, this.GridSize.Y);
    }
}
