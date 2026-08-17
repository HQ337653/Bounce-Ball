using System;
using BallzGame.Balls;
using BallzGame.Managers;
using UnityEngine;

public class LazerExplosionGenerator : MonoBehaviour
{
    public bool IsVertical;
    public GameObject ExplosionEffectPrefab;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance.BallExtraDamageController.GetEffectValue(BallExtraDamage.EffectType.LazerCollideExplosion)<1)
            return;
        if (IsVertical)
        {
            return;
        }

        var script = other.GetComponent<LazerExplosionGenerator>();
        if (script&&script.IsVertical)
        {
            var position=new Vector3(other.transform.position.x,transform.position.y,transform.position.z);
            Instantiate(
                ExplosionEffectPrefab, position, Quaternion.identity,
                GameManager.Instance.BallsParent
            );
        }

    }
}
