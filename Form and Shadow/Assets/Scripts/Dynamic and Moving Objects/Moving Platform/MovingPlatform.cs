using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    float speed = 2f;

    [SerializeField]
    float pauseTime = 2f;

    [SerializeField]
    bool isLoop = true;

    [SerializeField]
    Transform[] path;

    private int pathIndex = 0;
    private int pathIndexInc = +1;
    private bool isFinished = false;
    private bool isPause = false;
    private float timeCounter = 0f;
    public float slowValue = 1;


    // Use this for initialization
    void Start()
    {
        pathIndex = 0;
        pathIndexInc = +1;
        isFinished = false;
        GetComponent<BoxCollider>().size = new Vector3(gameObject.transform.GetChild(0).gameObject.GetComponent<Transform>().localScale.x, 0.5f,
            gameObject.transform.GetChild(0).gameObject.GetComponent<Transform>().localScale.z);
        if (path.Length > 0)
        {
            transform.position = path[pathIndex].position;
        }
    }

    void Update()
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

        if (isPause || isFinished) { return; }
        if (path.Length == 0) { return; }

        transform.position = Vector3.MoveTowards(transform.position, path[pathIndex].position, speed * Time.fixedDeltaTime);
        if (Vector3.Distance(transform.position, path[pathIndex].position) < speed * Time.fixedDeltaTime)
        {
            pathIndex += pathIndexInc;
            if (isLoop)
            {
                if (pathIndex >= path.Length || pathIndex < 0)
                {
                    pathIndexInc *= -1;
                    pathIndex += pathIndexInc;
                }
                else
                {
                    timeCounter = pauseTime;
                    isPause = true;
                }
            }
            else
            {
                if (pathIndex >= path.Length)
                {
                    isFinished = true;
                }
                else
                {
                    timeCounter = pauseTime;
                    isPause = true;
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.parent = gameObject.transform;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.parent = null;
        }
    }
}
