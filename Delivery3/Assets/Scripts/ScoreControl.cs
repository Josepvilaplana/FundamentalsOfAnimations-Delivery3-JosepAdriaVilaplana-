using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreControl : MonoBehaviour
{

    [SerializeField]
    IK_tentacles _myOctopus;
    [SerializeField]
    MovingBall ball;

    public AnimatorControl animator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ball")
        {
            ball.SetBallStopped(true);
            ball.transform.position += new Vector3(0, 0, 10);
            Debug.Log("The octopus has stopped the ball!");
        }

    }
}
