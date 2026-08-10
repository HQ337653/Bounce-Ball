using System.Collections.Generic;
using BallzGame.Balls;
using BallzGame.InventorySystem;
using GameMeta;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BallzGame.Managers.Shop
{
    public class ShopController : MonoBehaviour
    {
        public int CurrentCoin
        {
            get { return _currentCoin; }
            set
            {
                MainMenu.Instance.InGamePanel.Coin.text = value.ToString();
                _currentCoin = value;
                UpdateGachaButton();

            }
        }

       [SerializeField] private int _currentCoin;
        public Image fillImage;

        public List<BallData> Balls;

        public int Gacha6Price;
        public int Gacha1Price;

        public TextMeshProUGUI GoldText;
        public List<ShopItem> Items;

        [SerializeField] Button shopButton;

        public void RefreshShopItems()
        {
            List<ShopItem> result = new List<ShopItem>();

            int count = Mathf.Min(3, Items.Count); // 一次展示3个

            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, Items.Count);
                result.Add(Items[index]);
            }

            MainMenu.Instance.InGamePanel.RefreshItemShop(result);
        }


        private void Start()
        {
            UpdateUI();

            shopButton.onClick.AddListener(OpenShowPanel);

            MainMenu.Instance.InGamePanel.ShopMultiplePull.onClick.AddListener(doGacha5);
            MainMenu.Instance.InGamePanel.ShopOnePull.onClick.AddListener(doGacha1);
        }

        private void OpenShowPanel()
        {
            MainMenu.Instance.InGamePanel.ShopSubPanel.SetActive(true);

        }

        public void GainCoin(int amount)
        {
            CurrentCoin += amount;
            UpdateUI();
        }

        public void UpdateGachaButton()
        {
            MainMenu.Instance.InGamePanel.ShopMultiplePull.interactable = CurrentCoin >= Gacha6Price;

            MainMenu.Instance.InGamePanel.ShopOnePull.interactable = CurrentCoin >= Gacha1Price;
        }

        private void doGacha5()
        {
            if (CurrentCoin < Gacha6Price)
            {
                Debug.Log("金币不足！");
                return;
            }

            CurrentCoin -= Gacha6Price;

            List<Ball> resultBalls = new List<Ball>();
            List<BallData> resultDatas = new List<BallData>();

            for (int i = 0; i < 5; i++)
            {
                BallData data = GetRandomBallData();
                resultDatas.Add(data);

                Ball ballPrefab = data.BallPrefab;
                resultBalls.Add(ballPrefab);
            }

            GameManager.Instance.launcher.AddBalls(resultBalls);

            // ⭐ 显示抽卡结果
            MainMenu.Instance.InGamePanel.ShowGainBalls(resultDatas);

            UpdateUI();
        }

        private void doGacha1()
        {
            if (CurrentCoin < Gacha1Price)
            {
                Debug.Log("金币不足！");
                return;
            }

            CurrentCoin -= Gacha1Price;

            List<Ball> resultBalls = new List<Ball>();
            List<BallData> resultDatas = new List<BallData>();

            BallData data = GetRandomBallData();
            resultDatas.Add(data);

            Ball ballPrefab = data.BallPrefab.GetComponent<Ball>();
            resultBalls.Add(ballPrefab);

            GameManager.Instance.launcher.AddBalls(resultBalls);

            // ⭐ 显示抽卡结果
            MainMenu.Instance.InGamePanel.ShowGainBalls(resultDatas);

            UpdateUI();
        }

        private BallData GetRandomBallData()
        {
            int index = Random.Range(0, Balls.Count);
            return Balls[index];
        }

        private void UpdateUI()
        {
            if (GoldText != null)
                GoldText.text = CurrentCoin.ToString();

            if (fillImage != null)
            {
                fillImage.fillAmount = CurrentCoin / (float)Gacha6Price;
            }
        }
    }
}