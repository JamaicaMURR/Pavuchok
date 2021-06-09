using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    new Collider2D collider;

    Sticker sticker;
    RelativeJumper jumper;
    WheelDrive wheelDrive;
    WebProducer webProducer;

    float startRollingSpeed;
    float targetRollingSpeed;

    Action DoOnLeftRoll;
    Action DoOnRightRoll;
    Action DoOnStop;

    Coroutine rollCoroutine;

    Action DoOnPull;
    Action DoOnRelease;
    Action DoOnStopWeb;

    Coroutine pullCoroutine;
    Coroutine releaseCoroutine;

    bool release;

    //==================================================================================================================================================================
    public float initialStickingForce = 100;
    public float usualStickForce = 17.5f;
    public float unstickableDelay = 0.05f;

    public float jumpForce = 10;
    public float jumpTimeWindow = 0.1f;

    public float initialRollingSpeed = 50;
    public float maximalRollingSpeed = 350;
    public float accelerationTime = 1;
    public float rotationTorque = 1000;
    public float brakesTorque = 5;

    public int maximumKnots = 40;
    public float webPullSpeed = 1;
    public float webReleaseSpeed = 1;
    public float webProducingDelay = 0.5f;

    public RollingState rollingState;

    //==================================================================================================================================================================
    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();

        sticker = GetComponent<Sticker>();
        jumper = GetComponent<RelativeJumper>();
        wheelDrive = GetComponent<WheelDrive>();
        webProducer = GetComponent<WebProducer>();

        DoOnLeftRoll = ActualLeftRoll;
        DoOnRightRoll = ActualRightRoll;
        DoOnStop = delegate () { };

        DoOnPull = delegate () { };
        DoOnRelease = delegate () { };
        DoOnStopWeb = delegate () { };

        webProducer.OnWebDone += delegate ()
        {
            DoOnPull = ActualPull;
            DoOnRelease = ActualRelease;
            DoOnStopWeb = delegate () { };
        };

        webProducer.OnWebCut += delegate ()
        {
            StopWeb();

            DoOnPull = delegate () { };
            DoOnRelease = delegate () { };
            DoOnStopWeb = delegate () { };
        };
    }

    private void Start()
    {
        sticker.initialStickingForce = initialStickingForce;
        sticker.stickingForce = usualStickForce;

        wheelDrive.BeginRotate(0, brakesTorque);

        jumper.jumpForce = jumpForce;
        jumper.jumpTimeWindow = jumpTimeWindow;

        webProducer.KnotsLimit = maximumKnots;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CutWeb();
    }

    //<><><><><><> TO THE INPUT HANDLER!!!!<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>
    public Camera cam;

    private void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            CutWeb();
            Jump();
        }

        float horizontal = Input.GetAxis("Horizontal");

        if(horizontal < 0)
            RunLeft();
        else if(horizontal > 0)
            RunRight();
        else
            StopRun();

        if(Input.GetButtonDown("Fire1"))
        {
            CutWeb();
            ProduceWeb(cam.ScreenToWorldPoint(Input.mousePosition));
        }

        if(Input.GetButtonDown("Fire2"))
        {
            CutWeb();
            StartCoroutine(WaitUnstickableDelay());
        }

        float vertical = Input.GetAxis("Vertical");

        if(vertical > 0)
            PullWeb();
        else if(vertical < 0)
            ReleaseWeb();
        else
            StopWeb();
    }

    //==================================================================================================================================================================
    public void Jump()
    {
        StartCoroutine(WaitUnstickableDelay());
        jumper.Jump();
    }

    public void RunLeft()
    {
        DoOnLeftRoll();
    }

    public void RunRight()
    {
        DoOnRightRoll();
    }

    public void StopRun()
    {
        DoOnStop();
    }

    public void ProduceWeb(Vector2 targetPoint)
    {
        CutWeb();

        if(!collider.IsTouching(new ContactFilter2D() { layerMask = LayerMask.GetMask("Default") })) //Collider must not touch any surface from map
        {
            WebKnot lastKnot = webProducer.ProduceWeb(targetPoint);
            lastKnot.OnCollapse += CutWeb;
        }

    }

    public void PullWeb()
    {
        DoOnPull();
    }

    public void ReleaseWeb()
    {
        DoOnRelease();
    }

    public void StopWeb()
    {
        DoOnStopWeb();
    }

    public void CutWeb()
    {
        webProducer.CutWeb();
    }

    //Actuals===========================================================================================================================================================
    void ActualLeftRoll()
    {
        startRollingSpeed = -initialRollingSpeed;
        targetRollingSpeed = -maximalRollingSpeed;

        rollingState = RollingState.Left;

        rollCoroutine = StartCoroutine(Roll());

        DoOnLeftRoll = delegate () { };
        DoOnRightRoll = delegate () { };
        DoOnStop = ActualStopRoll;
    }

    void ActualRightRoll()
    {
        startRollingSpeed = initialRollingSpeed;
        targetRollingSpeed = maximalRollingSpeed;

        rollingState = RollingState.Right;

        rollCoroutine = StartCoroutine(Roll());

        DoOnLeftRoll = delegate () { };
        DoOnRightRoll = delegate () { };
        DoOnStop = ActualStopRoll;
    }

    void ActualStopRoll()
    {
        StopCoroutine(rollCoroutine);

        wheelDrive.BeginRotate(0, brakesTorque);

        rollingState = RollingState.Stop;

        DoOnLeftRoll = ActualLeftRoll;
        DoOnRightRoll = ActualRightRoll;
    }

    void ActualPull()
    {
        pullCoroutine = StartCoroutine(Pull());
        DoOnPull = delegate () { };
        DoOnRelease = delegate () { };
        DoOnStopWeb = PullWebStop;
    }

    void ActualRelease()
    {
        releaseCoroutine = StartCoroutine(Release());
        DoOnPull = delegate () { };
        DoOnRelease = delegate () { };
        DoOnStopWeb = ReleaseWebStop;
    }

    void PullWebStop()
    {
        StopCoroutine(pullCoroutine);

        DoOnPull = ActualPull;
        DoOnRelease = ActualRelease;
        DoOnStopWeb = delegate () { };
    }

    void ReleaseWebStop()
    {
        StopCoroutine(releaseCoroutine);

        DoOnPull = ActualPull;
        DoOnRelease = ActualRelease;
        DoOnStopWeb = delegate () { };
    }

    //Coroutines========================================================================================================================================================
    IEnumerator WaitUnstickableDelay()
    {
        sticker.IsSticky = false;
        yield return new WaitForSeconds(unstickableDelay);
        sticker.IsSticky = true;
    }

    IEnumerator Roll()
    {
        yield return new WaitForFixedUpdate();

        float speed = startRollingSpeed;
        float neededDelta = targetRollingSpeed - startRollingSpeed;
        float deltaPerFixedUpdate = neededDelta / accelerationTime * Time.fixedDeltaTime;

        float timeSpended = 0;

        while(timeSpended < accelerationTime)
        {
            wheelDrive.BeginRotate(speed, rotationTorque);
            speed += deltaPerFixedUpdate;
            timeSpended += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        wheelDrive.BeginRotate(targetRollingSpeed, rotationTorque);
    }

    IEnumerator Pull()
    {
        float pullDelay = 1 / webPullSpeed;

        while(true)
        {
            webProducer.Pull();
            yield return new WaitForSeconds(pullDelay);
        }
    }

    IEnumerator Release()
    {
        float releaseDelay = 1 / webReleaseSpeed;

        while(true)
        {
            webProducer.Release();
            yield return new WaitForSeconds(releaseDelay);
        }
    }
}
