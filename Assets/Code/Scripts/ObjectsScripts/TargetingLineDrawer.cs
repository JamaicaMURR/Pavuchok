using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingLineDrawer : MonoBehaviour
{
    ValStorage valStorage;

    LineRenderer lineRenderer;

    Action DoOnUpdate;

    public CharController charController;

    new public Camera camera;

    public float z;

    private void Awake()
    {
        valStorage = GameObject.Find("Master").GetComponent<ValStorage>();

        lineRenderer = GetComponent<LineRenderer>();

        DoOnUpdate = () => { };

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        charController.OnWebAbilityOn += delegate ()
        {
            charController.OnBecomeFly += SwitchToFlightMode;
            charController.OnBecomeTouch += SwitchToTouchMode;
        };

        charController.OnWebAbilityOff += delegate ()
        {
            SwitchToTouchMode();

            charController.OnBecomeFly -= SwitchToFlightMode;
            charController.OnBecomeTouch -= SwitchToTouchMode;
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

    void SwitchToFlightMode() => StartCoroutine("EnableLineDrawingAfterDelay");

    void SwitchToTouchMode()
    {
        StopAllCoroutines();

        DoOnUpdate = () => { };
        lineRenderer.enabled = false;
    }

    IEnumerator EnableLineDrawingAfterDelay()
    {
        yield return new WaitForSeconds(valStorage.targetLineAppearingDelay);

        lineRenderer.enabled = true;
        DoOnUpdate = DrawLine;
    }
}
