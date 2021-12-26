using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChaser : MonoBehaviour
{
    new public Camera camera;

    void Update()
    {
        Vector2 cursor2DPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(cursor2DPosition.x, cursor2DPosition.y, transform.position.z);
    }
}
