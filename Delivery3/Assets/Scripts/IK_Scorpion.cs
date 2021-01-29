using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OctopusController;

public class IK_Scorpion : MonoBehaviour
{
    MyScorpionController _myController= new MyScorpionController();

    public IK_tentacles _myOctopus;
    public MovingBall ball;

    [Header("Body")]
    float animTime;
    public float animDuration = 5;
    bool animPlaying = false;
    public Transform Body;
    public Transform StartPos;
    public Transform EndPos;

    [Header("Tail")]
    public Transform tailTarget;
    public Transform tail;

    [Header("Legs")]
    public Transform[] legs;
    public Transform[] legTargets;
    public Transform[] futureLegBases;

    public bool inShootingPosition;
    bool stopTheBall = true;
    
    //---Raycast variables---
    //This will later hold the data for the hit
    RaycastHit hit;
    //Modify it to change the length of the Ray
    public float distance = 50f;
    //A variable to store the location of the hit.
    Vector3 targetLocation;

    // Start is called before the first frame update
    void Start()
    {
        _myController.InitLegs(legs,futureLegBases,legTargets);
        _myController.InitTail(tail);

    }

    // Update is called once per frame
    void Update()
    {
        if(animPlaying)
            animTime += Time.deltaTime;

        NotifyTailTarget();
        

        if (animTime < animDuration)
        {
            UpdateFutureLegPositions();
            Body.position = Vector3.Lerp(StartPos.position, EndPos.position, animTime / animDuration);
        }
        else if (animTime >= animDuration && animPlaying)
        {
            Body.position = EndPos.position;
            inShootingPosition = true;
            animPlaying = false;
        }

        _myController.UpdateIK();
    }
    
    void UpdateFutureLegPositions()
    { 

        for (int i = 0; i < futureLegBases.Length; i++)
        {
            if (Physics.Raycast(futureLegBases[i].transform.position + new Vector3(0,30,0), Vector3.down, out hit, distance))
            {
                //Set the target location to the location of the hit.
                targetLocation = hit.point;
                //Move the object to the target location.
                futureLegBases[i].transform.position = targetLocation;
            }
        }
    }

    //Function to send the tail target transform to the dll
    public void NotifyTailTarget()
    {
        _myController.NotifyTailTarget(tailTarget);
    }

    //Trigger Function to start the walk animation
    public void NotifyStartWalk()
    {

        _myController.NotifyStartWalk();
    }

    public void NotifyCanShoot(bool shoot)
    {
        _myController.CanShoot(shoot);
    }

    public void ResetScene()
    {
        ball.gameObject.GetComponent<SphereCollider>().enabled = true;
        animTime = 0;
        animPlaying = true;
        inShootingPosition = false;
        _myController.RestartBodyPosition();
        _myController.CanShoot(false);
        if (stopTheBall)
        {
            _myOctopus.SetLetHimScore(true);
        }
        else
        {
            _myOctopus.SetLetHimScore(false);
        }
        stopTheBall = !stopTheBall;
    }
}
