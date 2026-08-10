using System;
using System.Collections.Generic;
using BallzGame.Bricks.SpecialBricks;
using BallzGame.Effects;
using BallzGame.Managers;
using UnityEngine;
using TMPro;
using Utils;
using Random = UnityEngine.Random;

namespace BallzGame.Bricks
{
    public class Brick : MonoBehaviour
    {
        public int hp = 3;

        public Action OnDestory;
        public int DefensePoint;
        public TMP_Text hpText;
        public GameObject BrickVisual;
        public Gradient colors;
        public SpriteRenderer CenterSprite;
        public int OriginalHp;
        public BrickEffects brickEffects;
        public BrickPoint PointPrefab;
        public FloatText FloatTextPrefab;
        public SpecialBrick SpecialBrick;

        void Start()
        {
            UpdateHPText();
            SetDefence(DefensePoint);
        }

        public void OnRowMoved()
        {
            SpecialBrick?.OnRowMoved();
        }

        public void OnMiniGameStart()
        {
            SpecialBrick?.OnMiniGameStart();

        }

        public void OnMiniGameEnd()
        {
            SpecialBrick?.OnMiniGameEnd();


        }

        public void SetDefence(int defence)
        {
            DefensePoint = defence;
            if (DefensePoint > 0)
            {
                brickEffects.SetSheildEffect(true);
            }
            else
            {
                brickEffects.SetSheildEffect(false);

            }
        }

        public void AddHP(int hp)
        {
            this.hp += hp;
            UpdateHPText();
        }

        // 被 Ball 调用
        public void TakeDamage(int damage, Vector2 force = new Vector2())
        {
            int originalDefense = DefensePoint;
            int remainingDamage = damage;
            // 先扣护盾
            if (DefensePoint > 0)
            {
                if (DefensePoint >= remainingDamage)
                {
                    SetDefence(DefensePoint - remainingDamage);
                    remainingDamage = 0;
                }
                else
                {
                    remainingDamage -= DefensePoint;
                    SetDefence(0);
                }

                // 显示护盾减少
                int shieldLoss = originalDefense - DefensePoint;
                if (shieldLoss > 0)
                {
                    FloatText(transform.position, shieldLoss);
                }
            }

            // 再扣血
            if (remainingDamage > 0)
            {
                hp -= remainingDamage;
                UpdateHPText();

                // 只有真正掉血才触发受击效果
                brickEffects.doHit(force);

                if (hp <= 0)
                {
                    Die(force);
                }
            }
        }

        void FloatText(Vector3 pos, int val)
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
                    -val,
                    spawnPos,
                    Color.white,
                    dir
                );
            }
        }

        public void ChangeVisual(bool showing)
        {
            BrickVisual.SetActive(showing);
        }

        public void Die(Vector2 force)
        {
            brickEffects.MakeBreakEffect();
            PointPrefab = Instantiate(PointPrefab, transform.position, Quaternion.identity);
            PointPrefab.Init(transform.position, force);

            GameManager.Instance.feverController.AddFeverPoints(1);
            GameManager.Instance.CurrentResult.BricksCount += 1;
            GameManager.Instance.CurrentResult.Points += OriginalHp;
            OnDestory?.Invoke();

            Destroy(gameObject);
        }

        void UpdateHPText()
        {
            if (hpText != null)
            {
                // 计算血量百分比（0~1）
                float hpPercent = (float)hp / 10f;

                // 设置文字
                hpText.text = hp.ToString();

                // 根据渐变设置颜色
                CenterSprite.color = colors.Evaluate(hpPercent);
            }
        }

        public void Init(int i)
        {
            hp = i;
            OriginalHp = hp;
            UpdateHPText();
        }
    }
}