using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControl : MonoBehaviour
{

    [SerializeField]
    Animator[] anim;
    [SerializeField]
    MovingBall ball;

    bool hasScored;
        
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < anim.Length; i++)
        {
            anim[i] = anim[i].GetComponent<Animator>();
        }

        hasScored = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (ball.GetBallStopped())
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].Play("Disbelief");
            }
        }

        else if (hasScored)
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].Play("Victory");
            }
            hasScored = false;
        }
    }

    public void SetHasScored(bool state)
    {
        hasScored = state;
    }

}
