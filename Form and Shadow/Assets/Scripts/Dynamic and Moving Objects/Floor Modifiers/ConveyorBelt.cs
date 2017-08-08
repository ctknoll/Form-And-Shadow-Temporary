using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] float m_ConveyorBeltSpeed;
    Vector3 m_MoveDirection;
    Rigidbody m_Body;
    GameObject m_Player;
    bool m_PlayerInTrigger;

    // Use this for initialization
    void Start()
    {
        m_MoveDirection = transform.forward;
        m_Body = GetComponent<Rigidbody>();
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if(m_PlayerInTrigger)
        {
            m_Player.transform.position += m_MoveDirection * m_ConveyorBeltSpeed * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        m_Body.position -= m_MoveDirection * m_ConveyorBeltSpeed * Time.deltaTime;
        m_Body.MovePosition(m_Body.position + m_MoveDirection * m_ConveyorBeltSpeed * Time.deltaTime);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_PlayerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_PlayerInTrigger = false;
        }
    }
}