using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebDrawer : MonoBehaviour
{
    LineRenderer lineRenderer;

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
    }

    private void Update()
    {
        Vector3[] positions = new Vector3[webProducer.knots.Count];
        Vector3 shift = new Vector3(0, 0, zShift);

        int i = 0;

        foreach(GameObject knot in webProducer.knots)
        {
            positions[i] = knot.transform.position + shift;
            i++;
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}
