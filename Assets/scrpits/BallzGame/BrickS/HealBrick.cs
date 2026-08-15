using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace BallzGame.Bricks.SpecialBricks
{
    public class HealBrick : SpecialBrick

    {
        public TriggerCollector triggers;
        public GameObject HealVisual;
        public int HealAmount = 3;

        public FloatText FloatTextPrefab;
        private bool activated;

        [SerializeField]
        private int moveCount;

        public override void OnRowMoved()
        {
            base.OnRowMoved();
            moveCount += 1;
            if (moveCount == 1)
            {
                HealVisual.SetActive(true);
            }
            else if (moveCount > 1)
            {

                StartCoroutine(Heal());
            }

        }

        public override void OnMiniGameStart()
        {
            base.OnMiniGameStart();
            HealVisual.SetActive(false);

        }

        public override void OnMiniGameEnd()
        {
            if (moveCount == 1)
            {
                HealVisual.SetActive(true);
            }

        }

        public IEnumerator Heal()
        {
            triggers.gameObject.SetActive(true);
            yield return new WaitForFixedUpdate();
            AddHP();
            triggers.gameObject.SetActive(false);
        }

        void FloatText(Vector3 pos, string val)
        {
            if (FloatTextPrefab != null)
            {
                Vector3 spawnPos = pos;

                // 👉 稍微抬高一点避免重叠
                spawnPos += Vector3.up * 0.2f;

                // 👉 随机一点方向（更自然）
                Vector2 dir = (Vector2.up + Random.insideUnitCircle * 0.5f).normalized;

                // 👉 实例化
                FloatText ft = Instantiate(
                    FloatTextPrefab,
                    spawnPos,
                    Quaternion.identity
                );

                // 👉 颜色可以自己调（这里红色伤害）
                ft.DoFloatText(
                    val,
                    spawnPos,
                    Color.green,
                    dir,
                    1
                );
            }
        }

        void AddHP()
        {
            foreach (var VARIABLE in triggers.Colliders)
            {
                if(!VARIABLE)
                    return;
                var script = VARIABLE?.GetComponent<Brick>();
                if (script)
                {
                    Vector3 spawnPos = script.transform.position;

                    // 👉 稍微抬高一点避免重叠
                    spawnPos += Vector3.up * 0.2f;

                    FloatText(spawnPos, "+" + HealAmount);
                    script.AddHP(HealAmount);
                }
            }
        }
    }

}
