using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] float m_ConveyorBeltSpeed;
    Vector3 moveDirection;
    Rigidbody body;
    GameObject player;
    bool playerInTrigger;

    void Start()
    {
        moveDirection = transform.forward;
        body = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if(playerInTrigger)
        {
            player.transform.position += moveDirection * m_ConveyorBeltSpeed * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        body.position -= moveDirection * m_ConveyorBeltSpeed * Time.fixedDeltaTime;
        body.MovePosition(body.position + moveDirection * m_ConveyorBeltSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }
}