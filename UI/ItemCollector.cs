using BaseLibrary;
using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace SimpleCollector.UI;

public class ItemCollector : BaseUIPanel<TileEntities.ItemCollector>, IItemStorageUI
{
	protected const int SlotSize = 44;
	protected const int SlotMargin = 4;

	// todo: config screen for shape (circle, square) and size (filtering?)

	public ItemCollector(TileEntities.ItemCollector chest) : base(chest)
	{
		Width.Pixels = 16 + (SlotSize + SlotMargin) * 9 - SlotMargin;
		Height.Pixels = 44 + (SlotSize + SlotMargin) * 9 - SlotMargin;

		UIText textLabel = new UIText(Language.GetText("Mods.SimpleCollector.MapObject.ItemCollector"))
		{
			X = { Percent = 50 },
			Settings = { HorizontalAlignment = HorizontalAlignment.Center }
		};
		Add(textLabel);

		UIText buttonClose = new UIText("X")
		{
			Height = { Pixels = 20 },
			Width = { Pixels = 20 },
			X = { Percent = 100 },
			HoverText = Language.GetText("Mods.BaseLibrary.UI.Close")
		};
		buttonClose.OnMouseDown += args =>
		{
			if (args.Button != MouseButton.Left) return;
			args.Handled = true;

			PanelUI.Instance?.CloseUI(Container);
		};
		buttonClose.OnMouseEnter += _ => buttonClose.Settings.TextColor = Color.Red;
		buttonClose.OnMouseLeave += _ => buttonClose.Settings.TextColor = Color.White;
		Add(buttonClose);

		UIGrid<UIContainerSlot> gridItems = new(9)
		{
			Width = { Percent = 100 },
			Height = { Pixels = -28, Percent = 100 },
			Y = { Pixels = 28 },
			Settings = { ItemMargin = SlotMargin }
		};
		Add(gridItems);

		for (int i = 0; i < GetItemStorage().Count; i++)
		{
			UIContainerSlot slot = new UIContainerSlot(GetItemStorage(), i)
			{
				Width = { Pixels = SlotSize },
				Height = { Pixels = SlotSize }
			};
			gridItems.Add(slot);
		}
	}

	public ItemStorage GetItemStorage() => Container.GetItemStorage();

	public string GetCursorTexture(Item item) => SimpleCollector.TexturePath + "ItemCollectorItem";
}