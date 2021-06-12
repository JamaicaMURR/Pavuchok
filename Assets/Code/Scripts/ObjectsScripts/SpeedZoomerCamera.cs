using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedZoomerCamera : MonoBehaviour // TODO: Free view mode
{
    new Camera camera;

    int lastZoomGapSign;

    float timeBeforeActivity;
    float deltaZoom;
    float deltaZoomingSpeed;
    //==================================================================================================================================================================
    public Rigidbody2D target;

    public float defaultZoom;
    public float farestZoom;

    public float farestZoomVelocity;

    public float minimalZoomingSpeed;
    public float maximalZoomingSpeed;
    public float zoomingDelay;

    //==================================================================================================================================================================
    private void Awake()
    {
        camera = GetComponent<Camera>();

        lastZoomGapSign = 1;
        timeBeforeActivity = zoomingDelay;

        deltaZoom = farestZoom - defaultZoom;
        deltaZoomingSpeed = maximalZoomingSpeed - minimalZoomingSpeed;
    }

    private void Update()
    {
        if(timeBeforeActivity > 0)
            timeBeforeActivity -= Time.deltaTime;

        float zoomModifier = target.velocity.magnitude / farestZoomVelocity;

        if(zoomModifier > 1)
            zoomModifier = 1;

        float zoomingSpeed = deltaZoomingSpeed * zoomModifier + minimalZoomingSpeed;
        float targetZoom = deltaZoom * zoomModifier + defaultZoom;
        float zoomGap = targetZoom - camera.orthographicSize;

        if(Math.Sign(zoomGap) == lastZoomGapSign)
        {
            if(timeBeforeActivity <= 0)
            {
                float maximalGap = zoomingSpeed * Time.deltaTime;

                float zoomChangeValue;

                if(Mathf.Abs(zoomGap) > maximalGap)
                    zoomChangeValue = maximalGap * Math.Sign(zoomGap);
                else
                    zoomChangeValue = zoomGap;

                camera.orthographicSize += zoomChangeValue;
            }
        }
        else
        {
            lastZoomGapSign = Math.Sign(zoomGap);
            timeBeforeActivity = zoomingDelay;
        }

    }
}
