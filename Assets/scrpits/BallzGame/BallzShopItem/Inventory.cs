using System.Collections.Generic;
using BallzGame.InventorySystem.ShopItems;
using UnityEditor;
using UnityEngine;

namespace BallzGame.InventorySystem
{
	public class Inventory : MonoBehaviour
	{
		// Inventory 里当前拥有的 Item
		private List<ShopItem> items = new List<ShopItem>();

		public void AddItemToInventory(ShopItem item)
		{
			if (item == null)
				return;

			// 检查是否已经拥有这个类型的 Item
			ShopItem existingItem = GetItem(item.GetType());

			if (existingItem != null)
			{
				existingItem.OnAdded();
				return;
			}

			// 没有这个 Item，创建一个新的
			ShopItem newItem = Instantiate(item, transform);

			newItem.Count = 0;
			newItem.OnAdded();

			items.Add(newItem);
		}

		public ShopItem GetItem(System.Type type)
		{
			foreach (ShopItem item in items)
			{
				if (item.GetType() == type)
					return item;
			}

			return null;
		}

		public T GetItem<T>() where T : ShopItem
		{
			foreach (ShopItem item in items)
			{
				if (item is T)
					return (T)item;
			}

			return null;
		}

		public bool HasItem<T>() where T : ShopItem
		{
			return GetItem<T>() != null;
		}

		public void RemoveItem(ShopItem item)
		{
			if (item == null)
				return;

			ShopItem existingItem = GetItem(item.GetType());

			if (existingItem == null)
				return;

			existingItem.OnRemoved();

			// Count 到 0 后，从 Inventory 移除
			if (existingItem.Count <= 0)
			{
				items.Remove(existingItem);
				Destroy(existingItem.gameObject);
			}
		}
	}
}