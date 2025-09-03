using UnityEngine;

public class LogBoxInfo : MonoBehaviour
{
    public LogBoxType _type;
    [HideInInspector]
    public Transform _transform;

    public void Start()
    {
        _transform = GetComponent<Transform>();
    }

    public Transform GetTransform() { return _transform; }
}
