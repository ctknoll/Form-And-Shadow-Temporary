using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_Anim : MonoBehaviour
{

    private Animator animator;

    void Start ()
    {
        animator = GetComponent<Animator>();
	}
	
	void Update ()
    {
        animator.SetFloat("VSpeed", Input.GetAxis("Vertical"));
        animator.SetFloat("HSpeed", Input.GetAxis("Horizontal"));

	}
}
