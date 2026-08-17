using System;
using System.Collections.Generic;
using BallzGame.Bricks;
using UnityEngine;

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
            Crisis[0].DoCrisis();
        }
    }

    public abstract class CrisisBehaviour : MonoBehaviour
    {
        public abstract void DoCrisis();

    }
}