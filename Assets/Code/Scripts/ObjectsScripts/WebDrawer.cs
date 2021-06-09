using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebDrawer : MonoBehaviour
{
    LineRenderer lineRenderer;

    //==================================================================================================================================================================
    public WebProducer webProducer;

    //==================================================================================================================================================================
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    private void Update()
    {
        Vector3[] positions = new Vector3[webProducer.knots.Count];

        int i = 0;

        foreach(GameObject knot in webProducer.knots)
        {
            positions[i] = knot.transform.position;
            i++;
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}
