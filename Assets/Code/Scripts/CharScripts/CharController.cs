using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: Ability to death
// TODO: Deadly surfaces
// TODO: Strike zone indication
// TODO: Better furr
// TODO: Flies
// TODO: Web Strike reload animation

public class CharController : MonoBehaviour
{
    new Collider2D collider;

    Sticker sticker;
    RelativeJumper jumper;
    WheelDrive wheelDrive;
    WebProducer webProducer;
    WebStrikeCooler webStrikeCooler;

    float startRollingSpeed;
    float targetRollingSpeed;

    Action DoOnJumpChargeBegin;

    Action<Vector2> DoOnTouchFlyCheck;
    Action<Vector2> DoOnWebStrikeCoolCheck;

    Action DoOnLeftRoll;
    Action DoOnRightRoll;
    Action DoOnStop;

    Coroutine rollCoroutine;

    Action DoOnPull;
    Action DoOnRelease;
    Action DoOnStopWeb;

    Coroutine pullCoroutine;
    Coroutine releaseCoroutine;

    bool isTouching;
    bool isStandStill;

    bool isChargingJump;

    //==================================================================================================================================================================
    [Header("Common Settings")]
    public float standStillVelocityThreshold = 0.01f;

    [Header("Sticking Settings")]
    public float initialStickingForce = 100;
    public float usualStickingForce = 17.5f;
    public float unstickableDelay = 0.05f;

    [Header("Jumping Settings")]
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
    public int strikesLimit = 3;

    public float maximalShootDistance = 10;
    public float minimalWebLength = 1;
    public float reactionImpulsePerShotedKnot = 0.1f;
    public float webRestoringDelay = 0.5f;

    [HideInInspector]
    public RollingState rollingState;

    public event Action OnBecomeTouch;
    public event Action OnBecomeFly;
    public event Action OnBecomeStand;
    public event Action OnBecomeMove;

    //ABILITIES PROPERTIES==============================================================================================================================================
    public bool StickAbility
    {
        get { return sticker.StickAbility; }
        set { sticker.StickAbility = value; }
    }

    public bool JumpChargingAbility
    {
        get { return jumper.JumpChargingAbility; }
        set { jumper.JumpChargingAbility = value; }
    }

    public bool WebAbility
    {
        get { return webProducer.WebAbility; }
        set { webProducer.WebAbility = value; }
    }

    public bool PullReleaseAbility
    {
        get { return webProducer.PullReleaseAbility; }
        set { webProducer.PullReleaseAbility = value; }
    }

    public bool ChuteAbility
    {
        get { return webProducer.ChuteAbility; }
        set { webProducer.ChuteAbility = value; }
    }

    //==================================================================================================================================================================
    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();

        wheelDrive = GetComponent<WheelDrive>();
        sticker = GetComponent<Sticker>();
        jumper = GetComponent<RelativeJumper>();
        webProducer = GetComponent<WebProducer>();
        webStrikeCooler = GetComponent<WebStrikeCooler>();

        //----------------------------------------------------------------//
        DoOnJumpChargeBegin = delegate () { };

        OnBecomeMove += delegate ()
        {
            DoOnJumpChargeBegin = delegate () { };
            jumper.CancelCharge();
        };

        OnBecomeStand += delegate ()
        {
            DoOnJumpChargeBegin = ActualJumpCharge;

            if(isChargingJump)
                DoOnJumpChargeBegin();
        };

        //----------------------------------------------------------------//
        DoOnTouchFlyCheck = delegate (Vector2 v2) { };
        DoOnWebStrikeCoolCheck = TouchFlyCheck;

        OnBecomeTouch += delegate ()
        {
            DoOnTouchFlyCheck = delegate (Vector2 v2) { };
        };

        OnBecomeFly += delegate ()
        {
            DoOnTouchFlyCheck = ActualProduceWeb;
        };

        //----------------------------------------------------------------//
        DoOnLeftRoll = ActualLeftRoll;
        DoOnRightRoll = ActualRightRoll;
        DoOnStop = delegate () { };

        //----------------------------------------------------------------//
        DoOnPull = delegate () { };
        DoOnRelease = delegate () { };
        DoOnStopWeb = delegate () { };

        //----------------------------------------------------------------//
        webProducer.OnWebDone += delegate ()
        {
            DoOnPull = ActualPull;
            DoOnRelease = ActualRelease;
            DoOnStopWeb = delegate () { };

            DoOnWebStrikeCoolCheck = delegate (Vector2 v2) { };

            webStrikeCooler.BeginCooling();
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

    private void Update()
    {
        MonitorSelfState();
    }

    //==================================================================================================================================================================
    public void WebStrikeCooled() // Calls from WebStrikeCooler animator
    {
        DoOnWebStrikeCoolCheck = TouchFlyCheck;
    }

    //==================================================================================================================================================================
    public void ChargeJumpBegin()
    {
        isChargingJump = true;

        DoOnJumpChargeBegin();
    }

    public void ReleaseJump()
    {
        if(isChargingJump)
        {
            isChargingJump = false;

            CutWeb();
            jumper.Jump();
        }
    }

    public void UnStick()
    {
        CutWeb();

        if(isChargingJump)
        {
            jumper.CancelCharge();
            isChargingJump = false;
        }
        else if(sticker.StickAbility)
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
        DoOnWebStrikeCoolCheck(targetPoint);
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

    //==================================================================================================================================================================
    void MonitorSelfState()
    {
        if(collider.IsTouching(new ContactFilter2D() { layerMask = LayerMask.GetMask("Default") }))
        {
            if(!isTouching)
            {
                isTouching = true;

                if(OnBecomeTouch != null)
                    OnBecomeTouch();
            }

            if(collider.attachedRigidbody.velocity.magnitude <= standStillVelocityThreshold)
            {
                if(!isStandStill)
                {
                    isStandStill = true;

                    if(OnBecomeStand != null)
                        OnBecomeStand();
                }
            }
            else
            {
                if(isStandStill)
                {
                    isStandStill = false;

                    if(OnBecomeMove != null)
                        OnBecomeMove();
                }
            }
        }
        else
        {
            if(isTouching)
            {
                isTouching = false;

                if(OnBecomeFly != null)
                    OnBecomeFly();
            }
        }
    }

    //Actuals===========================================================================================================================================================
    void ActualJumpCharge()
    {
        jumper.BeginCharge();
    }

    void TouchFlyCheck(Vector2 targetPoint)
    {
        DoOnTouchFlyCheck(targetPoint);
    }

    void ActualProduceWeb(Vector2 targetPoint)
    {
        CutWeb();
        webProducer.ProduceWeb(targetPoint);
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
        sticker.StickAbility = false;
        yield return new WaitForSeconds(unstickableDelay);
        sticker.StickAbility = true;
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
