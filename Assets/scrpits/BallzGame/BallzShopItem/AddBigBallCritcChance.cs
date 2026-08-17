using BallzGame.Balls;
using BallzGame.Managers;

namespace BallzGame.InventorySystem.ShopItems{

	public class AddBigBallCritcChance: ShopItem
	{
		private BallExtraDamage ballExtraDamageController;

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
			ballExtraDamageController.AddEffectValue(BallExtraDamage.EffectType.BigBallCritPossibility, 5);
		}

		public override void OnRemoved()
		{
			Count -= 1;
			ballExtraDamageController.AddEffectValue(
				BallExtraDamage.EffectType.BigBallCritPossibility,
				-1
			);
		}

		public void Init()
		{
			ballExtraDamageController = GameManager.Instance.BallExtraDamageController;
		}
	}

}