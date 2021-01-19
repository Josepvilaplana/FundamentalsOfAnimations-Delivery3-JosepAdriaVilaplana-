using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MovingBall : MonoBehaviour
{
    [SerializeField]
    IK_tentacles _myOctopus;
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

    IEnumerator UpdatePowerBar()
    {
        while (PowerBarON)
        {
            if (Input.GetKey("space"))
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
                    LaunchRocket();
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

    public void LaunchRocket()
    {
        Debug.Log("Ball shot");
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

        shotDirection = (_target.transform.position - transform.position).normalized;
        transform.Translate(shotDirection * 1 * Time.deltaTime);

        transform.rotation = Quaternion.identity;

        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");

        //update the position
        //transform.position = transform.position + new Vector3(-horizontalInput * _movementSpeed * Time.deltaTime, verticalInput * _movementSpeed * Time.deltaTime, 0);
        //VisualizeLine(vo);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _myOctopus.NotifyShoot();
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
