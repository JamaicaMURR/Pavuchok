using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorWatcher : CharConnected
{
    public Transform anchorPoint;
    public new Camera camera;

    float _radiusOfOrbit;

    void Awake()
    {
        _radiusOfOrbit = Vector2.Distance(transform.position, anchorPoint.position);

        ConnectToCharacter();
    }

    void Update()
    {
        Vector2 anchorPosition = anchorPoint.position;
        Vector2 newPoint = Physics.MouseCursorDirection * _radiusOfOrbit + anchorPosition;

        transform.position = new Vector3(newPoint.x, newPoint.y, transform.position.z);
    }
}
