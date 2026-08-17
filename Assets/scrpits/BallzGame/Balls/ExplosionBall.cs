using System;
using BallzGame.Bricks;
using BallzGame.Managers;
using UnityEngine;
using Utils;
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
            if (Random.Range(0, 1f) < ExplosionPossibility)
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