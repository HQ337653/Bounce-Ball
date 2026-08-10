using BallzGame.Balls;
using BallzGame.Managers;
using UnityEngine;

namespace BallzGame.InventorySystem
{
	public class NormalBallAddDamage : ShopItem
	{
		public BallExtraDamage BallExtraDamageController;

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
		public abstract void OnAdded();
		public abstract void OnRemoved();
		public string Name;
		public string Description;
		public Sprite Icon;
		public int Count = 0;
		public int Price;
		public bool Buyable;
	}
}