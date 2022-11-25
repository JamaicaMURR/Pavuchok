using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorWatcher : MonoBehaviour
{
    public Transform anchorPoint;
    public new Camera camera;

    float radiusOfOrbit;

    void Awake()
    {
        radiusOfOrbit = Vector2.Distance(transform.position, anchorPoint.position);
    }

    void Update()
    {
        Vector2 anchorPosition = anchorPoint.position;
        Vector2 newPoint = ((Vector2)camera.ScreenToWorldPoint(Input.mousePosition) - anchorPosition).normalized * radiusOfOrbit + anchorPosition;

        transform.position = new Vector3(newPoint.x, newPoint.y, transform.position.z);
    }
}
