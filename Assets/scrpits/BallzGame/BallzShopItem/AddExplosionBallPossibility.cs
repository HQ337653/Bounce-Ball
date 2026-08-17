using BallzGame.Balls;
using BallzGame.InventorySystem;
using BallzGame.Managers;
using UnityEngine;

namespace BallzGame.InventorySystem.ShopItems
{
	public class AddExplosionBallPossibility : ShopItem
	{
		private BallExtraDamage BallExtraDamageController;

		public override bool Spawnable()
		{
			return InventoryHasBelow(5);
			;
		}

		public override void OnAdded()
		{
			if (Count == 0)
			{
				Init();
			}

			Count += 1;
			BallExtraDamageController.AddEffectValue(BallExtraDamage.EffectType.ExplosionPossibility, 5);
		}

		public override void OnRemoved()
		{
			Count -= 1;
			BallExtraDamageController.AddEffectValue(
				BallExtraDamage.EffectType.ExplosionPossibility,
				-1
			);
		}

		public void Init()
		{
			BallExtraDamageController = GameManager.Instance.BallExtraDamageController;
		}
	}
}