using System;
using System.IO;
using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace SimpleCollector.TileEntities;

public class ItemCollector : BaseTileEntity, IItemStorage, IHasUI
{
	protected override Type TileType => typeof(Tiles.ItemCollector);

	private new Guid ID;
	private ItemStorage Storage;

	public ItemCollector()
	{
		ID = Guid.NewGuid();
		Storage = new ItemStorage(81); // extract-only storage?
	}

	public override void Update()
	{
		for (int i = 0; i < Main.item.Length; i++)
		{
			ref Item item = ref Main.item[i];

			if (item is null || !item.active || item.IsAir || !ItemStorageUtility.IsValidItemForStorage(item)) continue;

			if (item.getRect().Intersects(new Rectangle(Position.X * 16, Position.Y * 16, 32, 32)))
			{
				Storage.InsertItem(this, ref item);
			}
		}
	}

	public override void OnKill()
	{
		Storage.DropItems(null, new Rectangle(Position.X * 16, Position.Y * 16, 32, 32));
	}

	public override void SaveData(TagCompound tag)
	{
		tag["ID"] = ID;
		tag["Items"] = Storage.Save();
	}

	public override void LoadData(TagCompound tag)
	{
		ID = tag.Get<Guid>("ID");
		Storage.Load(tag.GetCompound("Items"));
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(ID);
	}

	public override void NetReceive(BinaryReader reader)
	{
		ID = reader.ReadGuid();
	}

	public ItemStorage GetItemStorage() => Storage;

	public Guid GetID() => ID;
}