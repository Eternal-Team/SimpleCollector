using BaseLibrary.UI;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SimpleCollector.Tiles;

public class ItemCollector : ModTile
{
	public override string Texture => SimpleCollector.TexturePath + "ItemCollectorTile";

	public override void SetStaticDefaults()
	{
		Main.tileSolidTop[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
		TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TileEntities.ItemCollector>().Hook_AfterPlacement, -1, 0, false);
		TileObjectData.addTile(Type);

		ModTranslation name = CreateMapEntryName();
		AddMapEntry(Color.Aquamarine, name);
	}

	public override bool RightClick(int i, int j)
	{
		if (!TileEntityUtility.TryGetTileEntity(i, j, out TileEntities.ItemCollector collector)) return false;

		PanelUI.Instance?.HandleUI(collector);

		return true;
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (!TileEntityUtility.TryGetTileEntity(i, j, out TileEntities.ItemCollector collector)) return;

		PanelUI.Instance?.CloseUI(collector);

		collector.Kill(i, j);

		Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.ItemCollector>());
	}
}