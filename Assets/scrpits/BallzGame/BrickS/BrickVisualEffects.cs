using UnityEngine;
using System.Collections;
using BallzGame.Managers;
using TMPro;
using UnityEngine.Serialization;
using Utils;

namespace BallzGame.Bricks
{
	public class BrickVisualEffects : MonoBehaviour
	{
        [Header("Brick Visual")]
        [FormerlySerializedAs("BrickVisual")] [SerializeField]private GameObject brickVisual;
        [FormerlySerializedAs("CenterSprite")] [SerializeField]private SpriteRenderer centerSprite;
        [FormerlySerializedAs("Outline")] [SerializeField]private SpriteRenderer outline;
        [SerializeField]private TMP_Text hpText;
        [FormerlySerializedAs("Colors")] [SerializeField]private Gradient colors;
        [FormerlySerializedAs("ShieldColor")] [SerializeField]private Color shieldColor;
        [FormerlySerializedAs("NormalColor")] [SerializeField]private Color normalColor;
        [Header("flash")]
		[FormerlySerializedAs("Flash")] [SerializeField] private SpriteRenderer flash;
        [SerializeField]private float flashSpeed = 5f;
        [Header("Knock back")]
        [SerializeField]private float knockbackTime = 0.03f;
        [SerializeField]private float returnTime = 0.1f;
        [SerializeField]private float knockbackMultiplier;
        [Header("Visual effect prefabs")]
		[FormerlySerializedAs("EffectPrefab")] [SerializeField] private ParticleSystem effectPrefab;
        [FormerlySerializedAs("FloatTextPrefab")] [SerializeField]private FloatText floatTextPrefab;

        private Coroutine flashCoroutine;
        private Coroutine knockbackCoroutine;
        public void UpdateHPText(int currentHP, int maxHP)
        {
            if (hpText)
            {
                float hpPercent = Mathf.Clamp01((float)currentHP / maxHP);
                hpText.text = currentHP.ToString();
                centerSprite.color = colors.Evaluate(hpPercent);
            }
        }

        /// <summary>
        /// 显示/隐藏砖块主体视觉
        /// </summary>
        public void SetVisibility(bool showing)
        {
            brickVisual.SetActive(showing);
        }

        /// <summary>
        /// 显示浮动文字（伤害或护盾减少）
        /// </summary>
        public void ShowFloatText(Vector3 pos, int val)
        {
            if (floatTextPrefab != null)
            {
                Vector3 spawnPos = pos + Vector3.up * 0.2f;
                Vector2 dir = (Vector2.up + Random.insideUnitCircle * 0.5f).normalized;
                FloatText ft = Instantiate(floatTextPrefab, spawnPos, Quaternion.identity);
                ft.DoFloatText(-val, spawnPos, Color.white, dir);
            }
        }

        // ---------- 原有方法（微调） ----------

        public void SetShieldEffect(bool shield)
        {
            outline.color = shield ? shieldColor : normalColor;
        }

        [ContextMenu("hit")]
        public void DoHit(Vector2 force)
        {
            // Flash
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashEffect());

            // Knockback
            if (knockbackCoroutine == null && force != Vector2.zero)
            {
                knockbackCoroutine = StartCoroutine(KnockbackEffect(force));
            }
        }

        public void MakeBreakEffect()
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity, GameManager.Instance.VisualEffectsParent);
        }

        private IEnumerator FlashEffect()
        {
            Color color = flash.color;
            while (color.a < 1f)
            {
                color.a = Mathf.MoveTowards(color.a, 1f, flashSpeed * Time.deltaTime);
                flash.color = color;
                yield return null;
            }
            while (color.a > 0f)
            {
                color.a = Mathf.MoveTowards(color.a, 0f, flashSpeed * Time.deltaTime);
                flash.color = color;
                yield return null;
            }
            flashCoroutine = null;
        }

        private IEnumerator KnockbackEffect(Vector2 force)
        {
            Vector3 startPos = Vector3.zero;
            Vector2 direction = new Vector2(force.x, -force.y);
            Vector3 targetPos = startPos + (Vector3)direction * knockbackMultiplier;

            transform.localPosition = startPos;
            float timer = 0f;

            while (timer < knockbackTime)
            {
                timer += Time.deltaTime;
                float t = timer / knockbackTime;
                transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }
            transform.localPosition = targetPos;

            timer = 0f;
            while (timer < returnTime)
            {
                timer += Time.deltaTime;
                float t = timer / returnTime;
                transform.localPosition = Vector3.Lerp(targetPos, startPos, t);
                yield return null;
            }
            transform.localPosition = startPos;
            knockbackCoroutine = null;
        }
    }

}