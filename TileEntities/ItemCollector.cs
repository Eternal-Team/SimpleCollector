using System;
using System.IO;
using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.IO;

namespace SimpleCollector.TileEntities;

public class ItemCollector : BaseTileEntity, IItemStorage, IHasUI
{
	private class ItemCollectorStorage : ItemStorage
	{
		public ItemCollectorStorage(int size) : base(size)
		{
		}

		public override bool CanInteract(int slot, Operation operation, object user) => user is ItemCollector || !operation.HasFlag(Operation.Insert);
	}

	protected override Type TileType => typeof(Tiles.ItemCollector);

	private new Guid ID;
	private ItemCollectorStorage Storage;

	public ItemCollector()
	{
		ID = Guid.NewGuid();
		Storage = new ItemCollectorStorage(81);
	}

	public override void Update()
	{
		for (int i = 0; i < Main.item.Length; i++)
		{
			ref Item item = ref Main.item[i];

			if (item is null || !item.active || item.IsAir || !ItemStorageUtility.IsValidItemForStorage(item)) continue;

			// draw items in, when item is in center, collect
			// draw some fancy graphic
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