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

	private const int Distance = 16 * 10;

	public static Dust QuickDust(Vector2 pos, Color color)
	{
		Dust dust = Dust.NewDustDirect(pos, 0, 0, DustID.Clentaminator_Cyan);
		dust.position = pos;
		dust.velocity = Vector2.Zero;
		dust.fadeIn = 1f;
		dust.noLight = true;
		dust.noGravity = true;
		dust.color = color;
		return dust;
	}

	public override void Update()
	{
		Vector2 center = Position.ToWorldCoordinates(16f, 16f);
		
		for (int i = 0; i < Main.item.Length; i++)
		{
			ref Item item = ref Main.item[i];

			if (item is null || !item.active || item.IsAir || !ItemStorageUtility.IsValidItemForStorage(item) || item.IsACoin) continue;

			Vector2 itemCenter = item.Center;
			if (Vector2.DistanceSquared(center, itemCenter) <= Distance * Distance && Storage.InsertItem(this, ref item))
			{
				// todo: check if this works in MP
				for (float t = 0.0f; t <= 1.0f; t += 0.1f)
					QuickDust(Vector2.Lerp(itemCenter, center, t), Color.White);
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