using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    // Start is called before the first frame update
    void Start()
    {
        lineVisual.positionCount = lineSegment;
    }

    // Update is called once per frame
    void Update()
    {
        vo = (_target.transform.position - transform.position).normalized * 100;

        transform.rotation = Quaternion.identity;

        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");

        //update the position
        //transform.position = transform.position + new Vector3(-horizontalInput * _movementSpeed * Time.deltaTime, verticalInput * _movementSpeed * Time.deltaTime, 0);
        VisualizeLine(vo);
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
