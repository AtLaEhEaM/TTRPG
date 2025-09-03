using UnityEngine;

public class LogBoxInfo : MonoBehaviour
{
    public LogBoxType _type;
    [HideInInspector]
    public Transform _transform;
    [HideInInspector]
    public GameObject _gameObject;

    public void Start()
    {
        _gameObject = GetComponent<GameObject>();
        _transform = GetComponent<Transform>();
    }

    public Transform GetTransform() { return _transform; }
}
