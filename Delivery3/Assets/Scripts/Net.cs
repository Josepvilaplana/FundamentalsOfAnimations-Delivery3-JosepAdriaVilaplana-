using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : MonoBehaviour
{
    [SerializeField]
    MovingBall ball;
    public AnimatorControl anim;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ball" && !ball.GetBallStopped())
        {
            anim.SetHasScored(true);
            Debug.Log("Has marcauuuu");
        }
    }

}
