using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeDistanceLineDrawer : MonoBehaviour
{
    ValStorage valStorage;

    LineRenderer lineRenderer;

    float step;

    public CharController charController;

    public int corners = 35;

    private void Awake()
    {
        valStorage = GameObject.Find("Master").GetComponent<ValStorage>();

        lineRenderer = GetComponent<LineRenderer>();

        step = 360f / corners;

        lineRenderer.enabled = true;
        DrawLine();
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

    void DrawLine()
    {
        lineRenderer.positionCount = corners;

        Vector3 radius = new Vector3(valStorage.maximalStrikeDistance, 0, 0);

        Vector3[] dots = new Vector3[corners];

        for(int i = 0; i < dots.Length; i++)
        {
            dots[i] = radius;

            radius = Quaternion.Euler(0, 0, step) * radius;
        }

        lineRenderer.SetPositions(dots);
    }

    void SwitchToFlightMode() => StartCoroutine("EnableLineRendererAfterDelay");

    void SwitchToTouchMode()
    {
        StopAllCoroutines();
        lineRenderer.enabled = false;
    }

    IEnumerator EnableLineRendererAfterDelay()
    {
        yield return new WaitForSeconds(valStorage.strikeDistanceAppearingDelay);
        lineRenderer.enabled = true;
    }
}
