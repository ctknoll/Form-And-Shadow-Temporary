using UnityEngine;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour
{
    [Range(1.5f, 6f)][SerializeField] float m_MoveSpeed = 2f;
    [Range(0f, 4f)][SerializeField] float m_PauseTime = 2f;
    [SerializeField] bool m_LoopRoute = true;
    public List<Transform> m_PathLocations;
    protected bool m_IsFinished = false;
    public float slowValue = 1;

    int pathIndex = 0;
    int pathIndexInc = +1;
    bool isPause = false;
    float timeCounter = 0f;

    protected void Start()
    {
        pathIndex = 0;
        pathIndexInc = +1;
        m_IsFinished = false;

        if (m_PathLocations.Count > 0)
        {
            transform.position = m_PathLocations[pathIndex].position;
        }
    }

    protected void Update()
    {
        if (PlayerShadowInteraction.m_CurrentPlayerState != PlayerShadowInteraction.PlayerState.Shifting && !GameController.m_Paused)
        {
            if (timeCounter > 0)
            {
                timeCounter -= Time.deltaTime;
                return;
            }
            else
            {
                isPause = false;
            }

            if (isPause || m_IsFinished) { return; }
            if (m_PathLocations.Count == 0) { return; }

            transform.position = Vector3.MoveTowards(transform.position, m_PathLocations[pathIndex].position, m_MoveSpeed * slowValue * Time.fixedDeltaTime);
            if (Vector3.Distance(transform.position, m_PathLocations[pathIndex].position) < m_MoveSpeed * Time.fixedDeltaTime)
            {
                pathIndex += pathIndexInc;
                if (m_LoopRoute)
                {
                    if (pathIndex >= m_PathLocations.Count || pathIndex < 0)
                    {
                        pathIndexInc *= -1;
                        pathIndex += pathIndexInc;
                    }
                    else
                    {
                        timeCounter = m_PauseTime;
                        isPause = true;
                    }
                }
                else
                {
                    if (pathIndex >= m_PathLocations.Count)
                    {
                        m_IsFinished = true;
                    }
                    else
                    {
                        timeCounter = m_PauseTime;
                        isPause = true;
                    }
                }
            }
        }
    }
}
