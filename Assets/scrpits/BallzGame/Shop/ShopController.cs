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

        // 抽卡球列表（带概率权重）
        public List<BallProbability> GachaBalls;

        public int Gacha6Price;
        public int Gacha1Price;

        public TextMeshProUGUI GoldText;

        // 商店商品列表（带概率权重）
        public List<ShopItemProbability> Items;

        [SerializeField] Button shopButton;

        private int totalGachaProbability;
        private int totalItemProbability;

        private void Start()
        {
            // 初始化时归一化概率
            NormalizedProbability();

            UpdateUI();

            shopButton.onClick.AddListener(OpenShowPanel);

            MainMenu.Instance.InGamePanel.ShopMultiplePull.onClick.AddListener(DoGacha6);
            MainMenu.Instance.InGamePanel.ShopOnePull.onClick.AddListener(DoGacha1);
        }

        // ==================== 概率归一化 ====================

        [ContextMenu("NormalizedProbability")]
        void NormalizedProbability()
        {
            totalGachaProbability = 0;

            foreach (var item in GachaBalls)
            {
                totalGachaProbability += item.Probability;
            }

            // 如果不是100，自动归一化
            if (totalGachaProbability != 100)
            {
                foreach (var item in GachaBalls)
                {
                    item.Probability =
                        Mathf.RoundToInt(
                            item.Probability * 100f / totalGachaProbability
                        );
                }
            }

            totalGachaProbability = 0;

            foreach (var item in GachaBalls)
            {
                totalGachaProbability += item.Probability;
            }
            totalItemProbability = 0;

            foreach (var item in Items)
            {
                totalItemProbability += item.Probability;
            }

            // 如果不是100，自动归一化
            if (totalItemProbability != 100)
            {
                foreach (var item in Items)
                {
                    item.Probability =
                        Mathf.RoundToInt(
                            item.Probability * 100f / totalItemProbability
                        );
                }
            }

            totalItemProbability = 0;

            foreach (var item in Items)
            {
                totalItemProbability += item.Probability;
            }
        }


        // ==================== 随机抽取 ====================

        /// <summary>
        /// 按概率随机获取一个Ball（抽卡用）
        /// </summary>
        Ball GetRandomBallData()
        {
            int random = Random.Range(0, totalGachaProbability);

            int current = 0;

            foreach (var item in GachaBalls)
            {
                current += item.Probability;

                if (random < current)
                {
                    return item.Ball;
                }
            }

            // 防止没有return
            return GachaBalls[0].Ball;
        }

        /// <summary>
        /// 按概率随机获取不重复的商店商品
        /// </summary>
        public void RefreshShopItems()
        {
            List<ShopItem> result = new List<ShopItem>();

            int count = Mathf.Min(3, Items.Count); // 一次展示3个

            // 用临时列表做无重复抽取
            List<ShopItemProbability> tempList = new List<ShopItemProbability>(Items);

            for (int i = 0; i < count && tempList.Count > 0; i++)
            {
                int random = Random.Range(0, totalItemProbability);

                int current = 0;
                int selectedIndex = 0;

                for (int j = 0; j < tempList.Count; j++)
                {
                    current += tempList[j].Probability;

                    if (random < current)
                    {
                        selectedIndex = j;
                        break;
                    }
                }

                result.Add(tempList[selectedIndex].Item);

                // 从临时列表中移除已选中的，避免重复
                int removedProb = tempList[selectedIndex].Probability;
                tempList.RemoveAt(selectedIndex);

                // 重新计算剩余总概率
                totalItemProbability -= removedProb;
            }

            // 刷新完恢复总概率（如果有外部修改需求可重新归一化）
            NormalizedProbability();

            MainMenu.Instance.InGamePanel.RefreshItemShop(result);
        }

        // ==================== UI & 交互 ====================

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

        private void DoGacha6()
        {
            DoGacha(6, Gacha6Price);
        }

        private void DoGacha1()
        {
            DoGacha(1, Gacha1Price);
        }

        private void DoGacha(int count, int price)
        {
            if (CurrentCoin < price)
            {
                Debug.Log("金币不足！");
                return;
            }

            CurrentCoin -= price;

            List<Ball> resultBalls = new List<Ball>();
            List<BallData> resultDatas = new List<BallData>();

            for (int i = 0; i < count; i++)
            {
                Ball ball = GetRandomBallData();

                resultBalls.Add(ball);
                resultDatas.Add(ball.Data);
            }

            GameManager.Instance.launcher.AddBalls(resultBalls);

            // 显示抽卡结果
            MainMenu.Instance.InGamePanel.ShowGainBalls(resultDatas);

            UpdateUI();
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

    // ==================== 概率配置类 ====================

    /// <summary>
    /// 抽卡球概率配置（对应 BrickProbability）
    /// </summary>
    [System.Serializable]
    public class BallProbability
    {
        public Ball Ball;
        public int Probability;
    }

    /// <summary>
    /// 商店商品概率配置（对应 BrickProbability）
    /// </summary>
    [System.Serializable]
    public class ShopItemProbability
    {
        public ShopItem Item;
        public int Probability;
    }
}