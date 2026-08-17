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
		public TextMeshProUGUI NameText;
		public Image IconImage;

		public void SetData(ShopItem data)
		{
			NameText.text = data.Name;
			IconImage.sprite = data.Icon;
		}

		public Button button;
	}
}