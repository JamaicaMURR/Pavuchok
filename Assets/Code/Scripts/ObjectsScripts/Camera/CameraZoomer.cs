using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraZoomer : MonoBehaviour
{
    Camera _camera;

    bool _isLastZoomWasPositive = true;
    float _timePassed = 0;

    Action TickTimer;
    Action StartTimer;
    Action StopTimer;

    public Rigidbody2D targetRigidbody;

    public AnimationCurve zoomCurve;
    public float positiveZoomingSpeed;
    public float negativeZoomingSpeed;
    public float delayBeforePositiveZoom;
    public float delayBeforeNegativeZoom;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        TickTimer = () => { };
        StartTimer = ActualStartTimer;
        StopTimer = () => { };
    }

    private void Update()
    {
        TickTimer();

        float targetSize = zoomCurve.Evaluate(targetRigidbody.velocity.magnitude);
        float sizeGap = targetSize - _camera.orthographicSize;

        bool isPositiveZoomNeeded = sizeGap <= 0;

        if(isPositiveZoomNeeded)
        {
            if(_isLastZoomWasPositive)
            {
                StopTimer();
                Zoom(positiveZoomingSpeed);
            }
            else
            {
                StartTimer();

                if(_timePassed >= delayBeforePositiveZoom)
                {
                    Zoom(positiveZoomingSpeed);
                    _isLastZoomWasPositive = true;
                }
            }
        }
        else
        {
            if(_isLastZoomWasPositive)
            {
                StartTimer();

                if(_timePassed >= delayBeforeNegativeZoom)
                {
                    Zoom(negativeZoomingSpeed);
                    _isLastZoomWasPositive = false;
                }
            }
            else
            {
                StopTimer();
                Zoom(negativeZoomingSpeed);
            }
        }

        void Zoom(float speed)
        {
            float possibleChange = speed * Time.deltaTime * Math.Sign(sizeGap);
            float actualChange;

            if(Math.Abs(sizeGap) > Math.Abs(possibleChange))
                actualChange = possibleChange;
            else
                actualChange = sizeGap;


            _camera.orthographicSize += actualChange;
        }
    }

    void ActualStartTimer()
    {
        TickTimer = () => _timePassed += Time.deltaTime;
        StartTimer = () => { };
        StopTimer = ActualStopTimer;
    }

    void ActualStopTimer()
    {
        _timePassed = 0;
        TickTimer = () => { };
        StartTimer = ActualStartTimer;
        StopTimer = () => { };
    }
}
