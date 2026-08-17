using BallzGame.Balls;
using BallzGame.Managers;
using UnityEngine;

namespace BallzGame.InventorySystem.ShopItems
{
	public class LazerCollideCreateExplosion : ShopItem
	{
		private BallExtraDamage BallExtraDamageController;

		public override bool Spawnable()
		{
			return InventoryHasBelow(1);
		}

		public override void OnAdded()
		{
			if (Count == 0)
			{
				Init();
			}

			Count += 1;
			BallExtraDamageController.AddEffectValue(BallExtraDamage.EffectType.LazerCollideExplosion, 1);
		}

		public override void OnRemoved()
		{
			Count -= 1;
			BallExtraDamageController.AddEffectValue(BallExtraDamage.EffectType.LazerCollideExplosion, -1);

		}

		public void Init()
		{
			BallExtraDamageController = GameManager.Instance.BallExtraDamageController;
		}
	}
}
