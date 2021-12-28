using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingLineDrawer : MonoBehaviour
{
    LineRenderer lineRenderer;

    Action DoOnUpdate;

    public CharController charController;

    new public Camera camera;

    public float z;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        DoOnUpdate = () => { };

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        charController.OnWebAbilityOn += delegate ()
        {
            lineRenderer.enabled = true;
            DoOnUpdate = DrawLine;
        };

        charController.OnWebAbilityOff += delegate ()
        {
            DoOnUpdate = () => { };
            lineRenderer.enabled = false;
        };
    }

    private void Update() => DoOnUpdate();

    void DrawLine()
    {
        Vector3 charPosition = charController.transform.position;

        lineRenderer.SetPosition(0, new Vector3(charPosition.x, charPosition.y, z));

        Vector3 cursorPosition = camera.ScreenToWorldPoint(Input.mousePosition);

        lineRenderer.SetPosition(1, new Vector3(cursorPosition.x, cursorPosition.y, z));
    }
}
