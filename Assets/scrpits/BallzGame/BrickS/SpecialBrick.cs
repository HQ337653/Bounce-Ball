using UnityEngine;
using Object = System.Object;

namespace BallzGame.Bricks.SpecialBricks
{
    public class SpecialBrick : MonoBehaviour
    {
        public virtual void OnRowMoved()
        {

        }

        public virtual void OnMiniGameStart()
        {

        }

        public virtual void OnMiniGameEnd()
        {


        }

        public virtual void OnHit(bool alive, object sender)
        {

        }
    }
}