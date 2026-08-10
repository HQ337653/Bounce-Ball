using System;
using System.Collections.Generic;
using BallzGame.Managers;
using BallzGame.Balls;
using BallzGame.InventorySystem;
using BallzGame.Managers.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameMeta
{
	public class InGamePanel : Panel
	{
		public GameObject Panel;
		public GameObject DieSubPanel;
		public GameObject ShopSubPanel;

		public Button DieSubPanelConfirmButton;
		public Button ShopOnePull;
		public Button ShopMultiplePull;
		public Button ShopSubPanelExit;
		public Button GainBallConfirm;

		public GameObject GainBallPanel;
		public List<GainBallCard> GainBallPanelItems;
		public List<GainItemCard>  GainItemCards;
		public TextMeshProUGUI Coin;
		private void Start()
		{
			ShopSubPanelExit.onClick.AddListener(() =>
				{
					ShopSubPanel.SetActive(false);
				}
			);
			GainBallConfirm.onClick.AddListener(() =>
				{
					GainBallPanel.SetActive(false);
				}
			);
		}
		public void ShowGainBalls(List<BallData> datas)
		{
			GainBallPanel.SetActive(true);

			// 全部隐藏
			for (int i = 0; i < GainBallPanelItems.Count; i++)
			{
				GainBallPanelItems[i].gameObject.SetActive(false);
			}

			// 填充
			for (int i = 0; i < datas.Count && i < GainBallPanelItems.Count; i++)
			{
				GainBallPanelItems[i].gameObject.SetActive(true);
				GainBallPanelItems[i].SetData(datas[i]);
			}
		}

		public void RefreshItemShop(List<ShopItem> shopItems)
		{
			// 全部隐藏
			for (int i = 0; i < GainItemCards.Count; i++)
			{
				GainItemCards[i].gameObject.SetActive(false);
			}

			// 填充
			for (int i = 0; i < shopItems.Count && i < GainItemCards.Count; i++)
			{
				var card = GainItemCards[i];
				var data = shopItems[i];

				card.gameObject.SetActive(true);
				card.SetData(data);

				// 先清掉旧监听（很重要）
				card.button.onClick.RemoveAllListeners();

				card.button.onClick.AddListener(() =>
				{
					// 扣钱（你可以自己加价格判断）
					var shop = GameManager.Instance.shopController;

					if (shop.CurrentCoin <= 0)
						return;

					shop.CurrentCoin -= data.Price;

					// 加入背包
					GameManager.Instance.inventory.AddItemToInventory(data);

					Debug.Log("购买：" + data.name);
				});
			}
		}

		public override void Activate()
		{
			Panel.SetActive(true);
		}

		public override void Disable()
		{
			Panel.SetActive(false);
		}
	}
}