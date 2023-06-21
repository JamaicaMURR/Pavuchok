using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeDistanceLineDrawer : CharConnected
{
    LineRenderer _lineRenderer;

    float step;

    public int corners = 35;

    //==================================================================================================================================================================
    void OnWebAbilityEnable()
    {
        Physics.EBecomeFly += SwitchToFlightMode;
        Physics.EBecomeTouch += SwitchToTouchMode;
    }

    void OnWebAbilityDisable()
    {
        SwitchToTouchMode();

        Physics.EBecomeFly -= SwitchToFlightMode;
        Physics.EBecomeTouch -= SwitchToTouchMode;
    }

    //==================================================================================================================================================================
    private void Awake()
    {
        ConnectToCharacter();

        Character.EWebAbilityEnable += OnWebAbilityEnable;
        Character.EWebAbilityDisable += OnWebAbilityDisable;

        _lineRenderer = GetComponent<LineRenderer>();

        step = 360f / corners;

        _lineRenderer.enabled = true;
        DrawLine();
        _lineRenderer.enabled = false;

    }

    void DrawLine()
    {
        _lineRenderer.positionCount = corners;

        Vector3 radius = new Vector3(Sets.maximalStrikeDistance, 0, 0);

        Vector3[] dots = new Vector3[corners];

        for(int i = 0; i < dots.Length; i++)
        {
            dots[i] = radius;

            radius = Quaternion.Euler(0, 0, step) * radius;
        }

        _lineRenderer.SetPositions(dots);
    }

    void SwitchToFlightMode() => StartCoroutine(EnableLineRendererAfterDelay());

    void SwitchToTouchMode()
    {
        StopAllCoroutines();
        _lineRenderer.enabled = false;
    }

    //==================================================================================================================================================================
    IEnumerator EnableLineRendererAfterDelay()
    {
        yield return new WaitForSeconds(Sets.strikeDistanceAppearingDelay);
        _lineRenderer.enabled = true;
    }
}
