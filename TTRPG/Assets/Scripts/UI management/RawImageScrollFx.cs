using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawImageScrollFx : MonoBehaviour
{
    [SerializeField] RawImage _rawImage = null;
    [SerializeField] Vector2 _speed = default;

    private void Update()
    {
        var _position = _rawImage.uvRect.position + _speed * Time.deltaTime;
        _rawImage.uvRect = new Rect(_position, _rawImage.uvRect.size);
    }
}
