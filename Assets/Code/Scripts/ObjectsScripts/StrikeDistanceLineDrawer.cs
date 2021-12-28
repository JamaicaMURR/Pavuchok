using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeDistanceLineDrawer : MonoBehaviour
{
    LineRenderer lineRenderer;

    Action DoOnUpdate;

    float step;

    public CharController charController;

    public int corners = 35;

    public float appearanceDelay = 0.15f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        step = 360f / corners;

        lineRenderer.enabled = true;
        DrawLine();
        lineRenderer.enabled = false;

        charController.OnWebAbilityOn += delegate ()
        {
            charController.OnBecomeFly += InFlightMode;
            charController.OnBecomeTouch += InTouchMode;
        };

        charController.OnWebAbilityOff += delegate ()
        {
            DoOnUpdate = () => { };

            charController.OnBecomeFly -= InFlightMode;
            charController.OnBecomeTouch -= InTouchMode;
        };
    }

    private void Update() => DoOnUpdate();

    void DrawLine()
    {
        lineRenderer.positionCount = corners;

        Vector3 radius = new Vector3(charController.maximalStrikeDistance, 0, 0);

        Vector3[] dots = new Vector3[corners];

        for(int i = 0; i < dots.Length; i++)
        {
            dots[i] = radius;

            radius = Quaternion.Euler(0, 0, step) * radius;
        }

        lineRenderer.SetPositions(dots);
    }

    void InFlightMode()
    {
        StartCoroutine("EnableLineRendererAfterDelay");
    }

    void InTouchMode()
    {
        StopAllCoroutines();
        lineRenderer.enabled = false;
    }

    IEnumerator EnableLineRendererAfterDelay()
    {
        yield return new WaitForSeconds(appearanceDelay);
        lineRenderer.enabled = true;
    }
}
