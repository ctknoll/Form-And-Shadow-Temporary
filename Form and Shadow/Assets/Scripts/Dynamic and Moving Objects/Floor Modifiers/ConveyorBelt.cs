using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour {

    public float moveSpeed;
    public MoveDirection moveDirection;

    [HideInInspector]
    public enum MoveDirection {Forward, Right};

    // Use this for initialization
	void Start ()
    {
        foreach (Transform child in transform.parent)
        {
            if (child.name == "Conveyor_Direction_Pointer")
            {
                child.rotation = transform.parent.rotation;
                child.transform.Rotate(Vector3.up * 90, Space.Self);
                if (moveDirection == MoveDirection.Forward && moveSpeed < 0)
                    child.transform.Rotate(Vector3.up * 180, Space.Self);
                else if (moveDirection == MoveDirection.Right && moveSpeed >= 0)
                    child.transform.Rotate(Vector3.up * 90, Space.Self);
                else if (moveDirection == MoveDirection.Right && moveSpeed < 0)
                    child.transform.Rotate(Vector3.up * -90, Space.Self);
            }   
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<Collider>() != null && !other.GetComponent<Collider>().isTrigger)
        {
            if(moveDirection == MoveDirection.Forward)
            {
                other.transform.position += moveSpeed * Time.deltaTime * transform.forward;
            }
            else other.transform.position += moveSpeed* Time.deltaTime* transform.right;
        }
    }
}
