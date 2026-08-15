using System;
using BallzGame.Bricks;
using BallzGame.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BallzGame.Balls.SpecialBalls
{
    public class ExplosionBall : MonoBehaviour
    {
        public GameObject ExplosionEffectPrefab;
        int damage;
        public float ExplosionPossibility;

        public void OnBallHit(Brick brick)
        {
            if (Random.Range(0, 1) < ExplosionPossibility&&!brick.Status.Contains(Brick.BrickStatus.DisableEffect))
            {
                Instantiate(ExplosionEffectPrefab, brick.transform.position, Quaternion.identity,GameManager.Instance.BallsParent);
            }
        }

    }
}