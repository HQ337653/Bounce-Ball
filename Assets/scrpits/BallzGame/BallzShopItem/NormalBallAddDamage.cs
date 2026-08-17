using BallzGame.Balls;
using BallzGame.Managers;
using UnityEngine;

namespace BallzGame.InventorySystem.ShopItems
{
	public class NormalBallAddDamage : ShopItem
	{
		public override bool Spawnable()
		{
			return  InventoryHasBelow(5);;
		}

		private BallExtraDamage BallExtraDamageController;

		public override void OnAdded()
		{
			if (Count == 0)
			{
				Init();
			}

			Count += 1;
			BallExtraDamageController.AddExtraDamage(BallExtraDamage.BallType.NormalBall, 1);
		}

		public override void OnRemoved()
		{
			Count -= 1;
			BallExtraDamageController.AddExtraDamage(
				BallExtraDamage.BallType.NormalBall,
				-1
			);
		}

		public void Init()
		{
			BallExtraDamageController = GameManager.Instance.BallExtraDamageController;
		}
	}

	public abstract class ShopItem:MonoBehaviour
	{
		public abstract bool Spawnable();
		public abstract void OnAdded();
		public abstract void OnRemoved();
		public string Name;
		public string Description;
		public Sprite Icon;
		public int Count = 0;
		public int Price;
		protected bool InventoryHasBelow(int amount)
		{
			var item = (GameManager.Instance.inventory.GetItem(GetType()));
			if (item!=null&&item.Count >= amount)
			{
				return false;
			}
			return true;
		}
	}
}