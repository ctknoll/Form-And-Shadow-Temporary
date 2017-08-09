using UnityEngine;

public class PressureSwitchMoveGeometry : PressureSwitch
{
    [System.Serializable] public class MoveableGeometry
    {
        public GameObject geometryGameObject;
        public float geometryMoveSpeed;
        public Vector3 geometryMoveDirection;
        [HideInInspector] public Vector3 geometryStartPosition;
        [HideInInspector] public Vector3 geometryEndPosition;
    }

    [SerializeField] MoveableGeometry [] m_TargetGeometry;

	new void Start ()
    {
        base.Start();
        foreach(MoveableGeometry currentTargetGeometry in m_TargetGeometry)
        {
            currentTargetGeometry.geometryStartPosition = currentTargetGeometry.geometryGameObject.transform.position;
            currentTargetGeometry.geometryEndPosition = currentTargetGeometry.geometryStartPosition + currentTargetGeometry.geometryMoveDirection;
        }
    }

    new void Update ()
    {
        base.Update();
		if(m_Pressed)
        {
            foreach(MoveableGeometry currentTargetGeometry in m_TargetGeometry)
            {
                currentTargetGeometry.geometryGameObject.transform.position = Vector3.MoveTowards(currentTargetGeometry.geometryGameObject.transform.position, 
                    currentTargetGeometry.geometryEndPosition, currentTargetGeometry.geometryMoveSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            foreach (MoveableGeometry currentTargetGeometry in m_TargetGeometry)
            {
                currentTargetGeometry.geometryGameObject.transform.position = Vector3.MoveTowards(currentTargetGeometry.geometryGameObject.transform.position,
                    currentTargetGeometry.geometryStartPosition, currentTargetGeometry.geometryMoveSpeed * Time.fixedDeltaTime);
            }
        }
	}
}
