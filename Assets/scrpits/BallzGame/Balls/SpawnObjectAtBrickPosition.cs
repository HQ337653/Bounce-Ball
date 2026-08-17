using System;
using BallzGame.Bricks;
using BallzGame.Managers;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace BallzGame.Balls.SpecialBalls
{
    public class SpawnObjectAtBrickPosition : MonoBehaviour
    {
        public BallExtraDamage.EffectType EffectType;
        public GameObject ExplosionEffectPrefab;
        int damage;
        public float ExplosionPossibility;
        public void OnBallHit(Brick brick)
        {
            var actualPossibility=ExplosionPossibility+GameManager.Instance.BallExtraDamageController.GetEffectValue(EffectType)/100f;
            if (Random.Range(0, 1f) < actualPossibility)
            {
                if (brick.Status.Contains(Brick.BrickStatus.DisableEffect))
                {
                    GameManager.DoBlockFloatText(brick.transform.position);
                }
                else
                {
                    Instantiate(
                        ExplosionEffectPrefab, brick.transform.position, Quaternion.identity,
                        GameManager.Instance.BallsParent
                    );
                }
            }
        }



    }
}