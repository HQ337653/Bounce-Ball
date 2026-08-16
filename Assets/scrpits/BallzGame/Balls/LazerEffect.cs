using System;
using System.Collections;
using UnityEngine;

namespace BallzGame.Balls.SpecialBalls
{


    public class LazerEffect : MonoBehaviour
    {
	    [SerializeField] private SpriteRenderer visual;
	    [SerializeField] private float totalTime;

	    private void Start()
	    {
		    StartCoroutine(Animation());
	    }
	    public IEnumerator Animation()
	    {
		    var time = 0f;
		    while (time <totalTime)
		    {
			    visual.gameObject.transform.localScale =
				    Mathf.Sin(time /totalTime * Mathf.PI) * new Vector3(37f, 0.15f, 1f);
			    time += Time.deltaTime;
			    yield return null;
		    }
	    }
    }
}