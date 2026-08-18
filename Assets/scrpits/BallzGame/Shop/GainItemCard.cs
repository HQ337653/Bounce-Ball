using System.Net.Mime;

using BallzGame.Balls;
using BallzGame.InventorySystem;
using BallzGame.InventorySystem.ShopItems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BallzGame.Managers.Shop
{
	public class GainItemCard : MonoBehaviour
	{
		public TMP_Text NameText;
		public TMP_Text DescriptionText;
		public TMP_Text Count;
		public TMP_Text PriceText;
		public Image IconImage;
		public Button button;

		public void SetData(ShopItem data)
		{
			if (NameText)
			{
				NameText.text = data.Name;
			}

			if (IconImage)
			{
				IconImage.sprite = data.Icon;
			}

			if (DescriptionText)
			{
				DescriptionText.text = data.Description;
			}
			if (PriceText)
			{
				PriceText.text = data.Price.ToString();
			}

			if (Count)
			{
				Count.text = data.Count.ToString();
			}
		}

	}
}