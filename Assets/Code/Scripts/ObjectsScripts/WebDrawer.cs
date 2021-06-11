using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebDrawer : MonoBehaviour
{
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
    }

    private void Update()
    {
        Vector3[] positions = new Vector3[webProducer.knots.Count + 1];

        int i = 0;

        foreach(WebKnot knot in webProducer.knots)
        {
            positions[i] = knot.transform.position + shift;
            i++;
        }

        positions[i] = webProducer.transform.position + shift;

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}
