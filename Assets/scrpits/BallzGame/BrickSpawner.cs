using System.Collections.Generic;
using BallzGame.Bricks;
using UnityEngine;
using UnityEngine.Serialization;

namespace BallzGame.Managers
{
    public class BrickSpawner : MonoBehaviour
    {
        public List<BrickProbability> BrickPrefabs;

        private int totalProbability;


        void Start()
        {
            NormalizedBrickPossibility();
        }

        [ContextMenu("NormalizedBrickPossibility")]
        void NormalizedBrickPossibility()
        {
            totalProbability = 0;

            foreach (var item in BrickPrefabs)
            {
                totalProbability += item.Probability;
            }


            // 如果不是100，自动归一化
            if (totalProbability != 100)
            {
                foreach (var item in BrickPrefabs)
                {
                    item.Probability =
                        Mathf.RoundToInt(
                            item.Probability * 100f / totalProbability
                        );
                }
            }


            totalProbability = 0;

            foreach (var item in BrickPrefabs)
            {
                totalProbability += item.Probability;
            }
        }


        Brick GetRandomBrick()
        {
            int random = Random.Range(0, totalProbability);

            int current = 0;

            foreach (var item in BrickPrefabs)
            {
                current += item.Probability;

                if (random < current)
                {
                    return item.Brick;
                }
            }


            // 防止没有return
            return BrickPrefabs[0].Brick;
        }



        public List<Brick> SpawnRow(int level, int width)
        {
            var bricks = new List<Brick>();

            for (int x = 0; x < width; x++)
            {
                if (Random.value < 0.7f)
                {
                    // 随机砖块
                    Brick prefab = GetRandomBrick();


                    var script =
                        Instantiate(
                            prefab,
                            new Vector3(x, 0, 0),
                            Quaternion.identity
                        );


                    bricks.Add(script);


                    var hp = Random.Range(1f, 1.5f) * level;

                    script.Init((int)hp);
                }
                else
                {
                    bricks.Add(null);
                }
            }

            return bricks;
        }
    }

    [System.Serializable]
    public class BrickProbability
    {
        public Brick Brick;
        public int Probability;
    }
}