using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebDrawer : MonoBehaviour
{
    Action DoOnUpdate;

    LineRenderer lineRenderer;

    Vector3 shift;

    //==================================================================================================================================================================
    public WebProducer webProducer;

    public float webWidth = 0.1f;
    public float zShift = 0.5f;

    //==================================================================================================================================================================
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = webWidth;
        lineRenderer.endWidth = webWidth;

        shift = new Vector3(0, 0, zShift);

        DoOnUpdate = delegate () { };

        webProducer.OnWebDone += delegate ()
        {
            DoOnUpdate = DrawWeb;
        };

        webProducer.OnWebCut += delegate ()
        {
             DoOnUpdate = delegate () { };
             lineRenderer.positionCount = 0;
        };
    }

    private void Update()
    {
        DoOnUpdate();
    }

    //==================================================================================================================================================================
    void DrawWeb()
    {
        Vector3[] positions = new Vector3[webProducer.knots.Count + 1];

        int i = 0;

        foreach(WebKnot knot in webProducer.knots)
        {
            positions[i] = knot.transform.position + shift;
            i++;
        }

        positions[i] = new Vector3 (webProducer.transform.position.x, webProducer.transform.position.y, positions[i-1].z); // Make z of last line dot = z of previous knot

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}
