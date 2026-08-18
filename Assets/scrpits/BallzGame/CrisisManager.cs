using System;
using System.Collections.Generic;
using BallzGame.Bricks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BallzGame.Managers
{


    public class CrisisManager : MonoBehaviour
    {
        public List<CrisisBehaviour> Crisis;

        private void Start()
        {
            GameManager.Instance.BeforeRowSpawn.AddListener(BeforeRowSpawn);
        }

        void BeforeRowSpawn()
        {

        }
        public void DoCrisis()
        {
            var index=Random.Range(0, Crisis.Count);
            Crisis[index].DoCrisis();
        }
    }

    public abstract class CrisisBehaviour : MonoBehaviour
    {
        public abstract void DoCrisis();

    }
}