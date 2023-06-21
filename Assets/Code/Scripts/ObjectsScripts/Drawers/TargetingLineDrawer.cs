using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingLineDrawer : CharConnected
{
    LineRenderer _lineRenderer;

    Action DoOnUpdate;

    public float z;

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

        DoOnUpdate = () => { };

        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;
    }

    private void Update() => DoOnUpdate();

    void DrawLine()
    {
        _lineRenderer.SetPosition(0, new Vector3(CharacterPosition.x, CharacterPosition.y, z));
        _lineRenderer.SetPosition(1, new Vector3(Physics.CursorPosition.x, Physics.CursorPosition.y, z));
    }

    void SwitchToFlightMode() => StartCoroutine(EnableLineDrawingAfterDelay());

    void SwitchToTouchMode()
    {
        StopAllCoroutines();

        DoOnUpdate = () => { };
        _lineRenderer.enabled = false;
    }

    //==================================================================================================================================================================
    IEnumerator EnableLineDrawingAfterDelay()
    {
        yield return new WaitForSeconds(Sets.targetLineAppearingDelay);

        _lineRenderer.enabled = true;
        DoOnUpdate = DrawLine;
    }
}
