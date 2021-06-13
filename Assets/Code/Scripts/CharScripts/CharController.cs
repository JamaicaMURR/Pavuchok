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

    Action DoOnChargeControl;
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
    [Header("Sticking Settings")]
    public float initialStickingForce = 100;
    public float usualStickingForce = 17.5f;
    public float unstickableDelay = 0.05f;

    [Header("Jumping Settings")]
    public float jumpChargeModeVelocityThreshold = 0.01f;
    public float jumpForceInitial = 10;
    public float jumpForcePeak = 15;
    public float jumpChargeTime = 0.25f;
    public float jumpTimeWindow = 0.1f;

    public int chargingSteps = 1;

    [Header("Rolling Settings")]
    public float initialRollingSpeed = 50;
    public float maximalRollingSpeed = 350;
    public float accelerationTime = 1;
    public float rotationTorque = 1000;
    public float brakesTorque = 5;

    [Header("Web Settings")]
    public float webPullSpeed = 1;
    public float webReleaseSpeed = 1;
    public int maximumKnots = 40;
    public float maximalShootDistance = 10;
    public float minimalWebLength = 1;
    public float reactionImpulsePerShotedKnot = 0.1f;

    [HideInInspector]
    public RollingState rollingState;
    //==================================================================================================================================================================
    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();

        sticker = GetComponent<Sticker>();
        jumper = GetComponent<RelativeJumper>();
        wheelDrive = GetComponent<WheelDrive>();
        webProducer = GetComponent<WebProducer>();

        DoOnChargeControl = delegate () { };

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
        sticker.stickingForce = usualStickingForce;

        jumper.jumpForceInitial = jumpForceInitial;
        jumper.jumpForcePeak = jumpForcePeak;
        jumper.jumpChargeTime = jumpChargeTime;
        jumper.jumpTimeWindow = jumpTimeWindow;

        jumper.chargingSteps = chargingSteps;

        wheelDrive.BeginRotate(0, brakesTorque);

        webProducer.knotsLimit = maximumKnots;
        webProducer.maximalShootDistance = maximalShootDistance;
        webProducer.minimalWebLength = minimalWebLength;
        webProducer.reactionImpulsePerShotedKnot = reactionImpulsePerShotedKnot;
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
            ChargeJumpBegin();

        if(Input.GetButton("Jump"))
            ChargeJumpControl();

        if(Input.GetButtonUp("Jump"))
            ReleaseJump();

        float horizontal = Input.GetAxis("Horizontal");

        if(horizontal < 0)
            RunLeft();
        else if(horizontal > 0)
            RunRight();
        else
            StopRun();

        if(Input.GetButtonDown("Fire1"))
            ProduceWeb(cam.ScreenToWorldPoint(Input.mousePosition));

        if(Input.GetButtonDown("Fire2"))
            UnStick();

        float vertical = Input.GetAxis("Vertical");

        if(vertical > 0)
            PullWeb();
        else if(vertical < 0)
            ReleaseWeb();
        else
            StopWeb();
    }

    //==================================================================================================================================================================
    public void ChargeJumpBegin()
    {
        if(collider.attachedRigidbody.velocity.magnitude <= jumpChargeModeVelocityThreshold)
        {
            jumper.BeginCharge();
            DoOnChargeControl = ActualChargeControl;
        }
    }

    public void ChargeJumpControl()
    {
        DoOnChargeControl();
    }

    public void ReleaseJump()
    {
        CutWeb();
        jumper.Jump();

        DoOnChargeControl = delegate () { };
    }

    public void UnStick()
    {
        CutWeb();
        StartCoroutine(WaitUnstickableDelay());
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
            webProducer.ProduceWeb(targetPoint);
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
    void ActualChargeControl()
    {
        if(collider.attachedRigidbody.velocity.magnitude > jumpChargeModeVelocityThreshold)
        {
            jumper.CancelCharge();
            DoOnChargeControl = delegate () { };
        }
    }

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
