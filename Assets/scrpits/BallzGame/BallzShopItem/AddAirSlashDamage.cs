using BallzGame.Balls;
using BallzGame.InventorySystem;
using BallzGame.Managers;
using UnityEngine;

namespace BallzGame.InventorySystem.ShopItems
{
    public class AddAirSlashDamage : ShopItem
    {
        public override bool Spawnable()
        {
            return InventoryHasBelow(5);
            ;
        }

        private BallExtraDamage BallExtraDamageController;

        public override void OnAdded()
        {
            if (Count == 0)
            {
                Init();
            }

            Count += 1;
            BallExtraDamageController.AddExtraDamage(BallExtraDamage.BallType.AirSlashSpawned, 1);
        }

        public override void OnRemoved()
        {
            Count -= 1;
            BallExtraDamageController.AddExtraDamage(
                BallExtraDamage.BallType.AirSlashSpawned,
                -1
            );
        }

        public void Init()
        {
            BallExtraDamageController = GameManager.Instance.BallExtraDamageController;
        }
    }
}