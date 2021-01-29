using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MovingBall : MonoBehaviour
{
    [SerializeField]
    IK_tentacles _myOctopus;
    [SerializeField]
    IK_Scorpion _myScorpion;
    [SerializeField]
    LightBlink flashLight;
    [SerializeField]
    GameObject _target;

    //movement speed in units per second
    [Range(-1.0f, 1.0f)]
    [SerializeField]
    private float _movementSpeed = 5f;

    //Ball direction variables
    Vector3 shotDirection;

    //Ball power variables
    public GameObject powerBarGO;
    public Image PowerBarMask;
    public float barChangeSpeed = 1;
    float maxPowerBarValue = 100;
    float currentPowerBarValue;
    bool powerIsIncreasing;
    public bool powerBarON;

    //Ball position variables
    float elapse_time = 6;
    bool ballKicked = true;
    bool ballStopped = false;
    public Vector3 initialPosition;


    IEnumerator UpdatePowerBar()
    {
        while (powerBarON)
        {
            if (Input.GetKey("space") && _myScorpion.inShootingPosition)
            {
                if (!powerIsIncreasing)
                {
                    currentPowerBarValue -= barChangeSpeed;
                    if (currentPowerBarValue <= 0)
                    {
                        powerIsIncreasing = true;
                    }
                }
                if (powerIsIncreasing)
                {
                    currentPowerBarValue += barChangeSpeed;
                    if (currentPowerBarValue >= maxPowerBarValue)
                    {
                        powerIsIncreasing = false;
                    }
                }

                float fill = currentPowerBarValue / maxPowerBarValue;
                PowerBarMask.fillAmount = fill;
                yield return new WaitForSeconds(0.01f);

                if (Input.GetKeyUp("space"))
                {
                    powerBarON = false;
                    _myScorpion.NotifyCanShoot(true);
                    flashLight.SetEndTime();
                    StartCoroutine(TurnOffPowerBar());

                }
            }
            else
            {
                yield return new WaitForSeconds(0.01f);
            }
        }
        yield return null;
    }
    IEnumerator TurnOffPowerBar()
    {
        yield return new WaitForSeconds(2.5f);
        powerBarGO.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Power bar variables on default values
        currentPowerBarValue = 0;
        powerIsIncreasing = true;
        powerBarGO.SetActive(true);
        powerBarON = true;
        initialPosition = transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        //transform.Translate(shotDirection * 1 * Time.deltaTime);
        transform.rotation = Quaternion.identity;

        if (ballKicked && elapse_time > 5)
        {
            ballKicked = false;
            ballStopped = false;
            transform.position = initialPosition;
            elapse_time = 0;
            PowerBarMask.fillAmount = 0;
            currentPowerBarValue = 0;
            powerIsIncreasing = true;
            powerBarGO.SetActive(true);
            powerBarON = true;
            StartCoroutine(UpdatePowerBar());
            _myScorpion.ResetScene();
        }
        else if (ballKicked)
        {
            elapse_time += Time.deltaTime;
            //Reduim el temps de calcul de la pilota a un cop hagi creuat la meta per evitar futurs problemes
            if(elapse_time < 0.5 && !ballStopped)
            {
                transform.position = CalculatePosInTime(shotDirection * (currentPowerBarValue / 6), elapse_time);
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "TailSphere")
        {
            _myOctopus.NotifyShoot();
            shotDirection = (_target.transform.position - transform.position).normalized;
            ballKicked = true;
        }

    }



    private Vector3 CalculatePosInTime(Vector3 vo, float time)
    {
        
        Vector3 result = transform.position + vo * time;
        float sY = (-0.5f * Mathf.Abs(Physics.gravity.y) * (time * time)) + (vo.y * time) + transform.position.y;

        result.y = sY;

        return result;
    }

    public void SetBallStopped(bool state)
    {
        ballStopped = state;
    }

    public bool GetBallStopped()
    {
        return ballStopped;
    }
}
