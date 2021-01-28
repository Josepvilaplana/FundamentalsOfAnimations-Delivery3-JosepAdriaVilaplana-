using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControl : MonoBehaviour
{

    [SerializeField]
    Animator[] anim;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < anim.Length; i++)
        {
            anim[i] = anim[i].GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].Play("Victory");
            }
        }
        if (Input.GetKey(KeyCode.R))
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].Play("Disbelief");
            }
        }
    }
}
