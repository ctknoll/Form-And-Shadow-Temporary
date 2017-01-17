using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDimensionalObject : MonoBehaviour {

    [SerializeField]
    GameObject connectedPlane;
    bool inWall;
    
    // Use this for initialization
	void Start ()
    {
        inWall = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 dir = new Vector3(0, 0, 0);
        dir.x += Input.GetAxisRaw("Horizontal");
        dir.y += Input.GetAxisRaw("Vertical");
        this.MoveRelativeToPlane(dir, connectedPlane);
        //this is a temporary spot
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
        //this too
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Recieved");
            if (!inWall)
            {
                Debug.Log("Not In Wall");
                inWall = castToWall();
            }
        }
    }

    //funnily enough, this doesnt have to be a plane, just any game object who's transform.right is fully parallel to it's face (like a cube)
    void MoveRelativeToPlane(Vector3 dir, GameObject plane)
    {
        Vector3 movement = (plane.GetComponent<Transform>().right * dir.x) + (plane.GetComponent<Transform>().forward * dir.y);
        gameObject.GetComponent<Rigidbody>().velocity = movement.normalized * 5;
    }

    bool castToWall()
    {
        Vector3 dir = gameObject.transform.position - Camera.main.transform.position;
        RaycastHit hitInfo;
        Debug.Log(Physics.Linecast(Camera.main.transform.position, dir, out hitInfo));
        Debug.Log(Physics.Raycast(transform.position, -1 * (Camera.main.transform.position - transform.position), out hitInfo, 50));
        //checks if theres nothing between the player and the camera
        if (Physics.Linecast(Camera.main.transform.position, transform.position, out hitInfo))
        {
            //should cast a ray from the player away from the camera, and return true if there is a plane within 50 units
            if(Physics.Raycast(transform.position, -1 * (Camera.main.transform.position - transform.position), out hitInfo, 50))
            {
                Debug.Log(Physics.Raycast(transform.position, -1 * (Camera.main.transform.position - transform.position), out hitInfo, 50));
                //this doesn't check for planes; unity primitives are arbitrary meshes. We should label our meldable walls somehow!
                //remove this object from the camera
                //this.gameObject.layer = 8;
                transform.position = hitInfo.point;
                return true;
            }
            return false;
        }
        return false;
    }
}
