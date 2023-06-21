using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebLineDrawer : CharConnected
{
    LineRenderer _lineRenderer;
    Transform _target;

    Action DoOnUpdate;

    //==================================================================================================================================================================
    public float zShift = 0.5f;

    //==================================================================================================================================================================
    void OnWebDone(Transform targetTransform)
    {
        _target = targetTransform;
        _lineRenderer.positionCount = 2;
        DoOnUpdate = DrawWeb;
    }

    void OnWebCutted()
    {
        _lineRenderer.positionCount = 0;
        DoOnUpdate = () => { };
    }

    //==================================================================================================================================================================
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        ConnectToCharacter();

        Character.EWebDone += OnWebDone;
        Character.EWebCutted += OnWebCutted;


        DoOnUpdate = () => { };
    }

    private void Update()
    {
        DoOnUpdate();
    }

    //==================================================================================================================================================================
    void DrawWeb()
    {
        Vector3[] positions = new Vector3[2] { new Vector3(CharacterPosition.x, CharacterPosition.y, zShift), new Vector3(_target.position.x, _target.position.y, zShift) };
        _lineRenderer.SetPositions(positions);
    }
}
