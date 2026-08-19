using BallzGame.Balls;
using BallzGame.Bricks;
using BallzGame.Managers;
using UnityEngine;

public class BigBallCritDamage : MonoBehaviour
{

    [SerializeField] private float CritDamageBasePossibility;
    [SerializeField] private BallExtraDamage.EffectType PossibilityType;
    [SerializeField] private BallExtraDamage.EffectType DamageType;
    [SerializeField] private int baseDamage;
    public void OnBallHit(Brick brick)
    {
        var actualPossibility=CritDamageBasePossibility+GameManager.Instance.BallExtraDamageController.GetEffectValue(PossibilityType)/100f;
        if (Random.Range(0, 1f) < actualPossibility)
        {
            var damage=baseDamage+GameManager.Instance.BallExtraDamageController.GetEffectValue(DamageType);
            brick.TakeDamage(damage,this);
            GameManager.DoCritText(brick.transform.position,damage);
        }
    }
}
