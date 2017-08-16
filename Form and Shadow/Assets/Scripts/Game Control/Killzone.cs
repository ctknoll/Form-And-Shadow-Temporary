using UnityEngine;

public class Killzone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!GameController.m_Resetting)
            {
                GameController.m_Instance.ResetLevel();
            }
        }
    }
}
