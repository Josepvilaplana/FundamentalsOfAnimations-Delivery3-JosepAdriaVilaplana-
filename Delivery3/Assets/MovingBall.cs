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

    Vector3 _dir;

    public LineRenderer lineVisual;
    public int lineSegment = 10;
    Vector3 vo;

    //Ball direction variables
    Vector3 shotDirection;

    //Ball power variables
    public GameObject powerBarGO;
    public Image PowerBarMask;
    public float barChangeSpeed = 1;
    float maxPowerBarValue = 100;
    float currentPowerBarValue;
    bool powerIsIncreasing;
    bool PowerBarON;

    //Ball position variables
    float elapse_time = 0;
    bool ballKicked = false;

    IEnumerator UpdatePowerBar()
    {
        while (PowerBarON)
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
                    PowerBarON = false;
                    _myScorpion.NotifyCanShoot();
                    flashLight.SetEndTime();
                    Debug.Log("Ball shot");
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
        lineVisual.positionCount = lineSegment;

        //Power bar variables on default values
        currentPowerBarValue = 0;
        powerIsIncreasing = true;
        PowerBarON = true;
        StartCoroutine(UpdatePowerBar());
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(shotDirection * 1 * Time.deltaTime);

        transform.rotation = Quaternion.identity;

        //VisualizeLine(vo);
        if (ballKicked)
        {
            elapse_time += Time.deltaTime;
            transform.position = CalculatePosInTime(shotDirection * (currentPowerBarValue/5), elapse_time);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        _myOctopus.NotifyShoot();
        shotDirection = (_target.transform.position - transform.position).normalized;
        ballKicked = true;

    }

    private Vector3 CalculatePosInTime(Vector3 vo, float time)
    {
        Vector3 vxz = vo;
        vxz.y = 0f;

        Vector3 result = transform.position + vo * time;
        float sY = (-0.5f * Mathf.Abs(Physics.gravity.y) * (time * time)) + (vo.y * time) + transform.position.y;

        result.y = sY;

        return result;
    }

    void VisualizeLine(Vector3 vo)
    {
        for (int i = 0; i < lineSegment; i++)
        {
            Vector3 pos = CalculatePosInTime(vo, i / (float)lineSegment);
            lineVisual.SetPosition(i, pos);
        }
    }
}
