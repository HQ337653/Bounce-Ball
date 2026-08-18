using System;
using System.Collections.Generic;
using System.Linq;
using BallzGame.Balls;
using UnityEngine;

namespace BallzGame.Managers.Shop.UI
{
	public class BallsDisplay : MonoBehaviour
	{
		public List<BallCard> cards;

		private void Reset()
		{
			BallCard[] foundCards = GetComponentsInChildren<BallCard>(true);

			cards = new List<BallCard>(foundCards);
		}

		public void ShowBalls(Dictionary<BallData, int> target)
		{
			int index = 0;

			// 使用 foreach 遍历字典
			foreach (var pair in target)
			{
				if (index >= cards.Count) break; // 防止索引越界

				// 激活卡片并设置数据
				cards[index].gameObject.SetActive(true);
				cards[index].SetData(pair.Key, pair.Value);

				index++;
			}

			// 隐藏剩余的卡片
			for (int i = index; i < cards.Count; i++)
			{
				cards[i].gameObject.SetActive(false);
			}
		}

	}
}