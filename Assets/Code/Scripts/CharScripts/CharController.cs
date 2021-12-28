using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: Ability to death
// TODO: Deadly surfaces
// TODO: Better furr
// TODO: Flies
// TODO: Eyes chasing cursor

public class CharController : MonoBehaviour
{
    ValStorage valStorage;

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
    [HideInInspector]
    public RollingState rollingState;

    public event Action OnBecomeTouch;
    public event Action OnBecomeFly;
    public event Action OnBecomeStand;
    public event Action OnBecomeMove;
    public event Action OnCancelledJumpRelease;
    public event Action OnWebAbilityOn;
    public event Action OnWebAbilityOff;

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
        set 
        {
            if(webProducer.WebAbility != value)
            {
                webProducer.WebAbility = value;

                if(value)
                    OnWebAbilityOn?.Invoke();
                else
                    OnWebAbilityOff?.Invoke();
            }
        }
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
        valStorage = GameObject.Find("Master").GetComponent<ValStorage>();

        collider = GetComponent<CircleCollider2D>();

        wheelDrive = GetComponent<WheelDrive>();
        sticker = GetComponent<Sticker>();
        jumper = GetComponent<RelativeJumper>();
        webProducer = GetComponent<WebProducer>();

        webStrikeCooler = GameObject.Find("Master").GetComponent<WebStrikeCooler>();

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
        wheelDrive.BeginRotate(0, valStorage.brakesTorque);
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
        else
            OnCancelledJumpRelease?.Invoke();
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

            if(collider.attachedRigidbody.velocity.magnitude <= valStorage.standStillVelocityThreshold)
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
        startRollingSpeed = -valStorage.initialRollingSpeed;
        targetRollingSpeed = -valStorage.maximalRollingSpeed;

        rollingState = RollingState.Left;

        rollCoroutine = StartCoroutine(Roll());

        DoOnLeftRoll = delegate () { };
        DoOnRightRoll = delegate () { };
        DoOnStop = ActualStopRoll;
    }

    void ActualRightRoll()
    {
        startRollingSpeed = valStorage.initialRollingSpeed;
        targetRollingSpeed = valStorage.maximalRollingSpeed;

        rollingState = RollingState.Right;

        rollCoroutine = StartCoroutine(Roll());

        DoOnLeftRoll = delegate () { };
        DoOnRightRoll = delegate () { };
        DoOnStop = ActualStopRoll;
    }

    void ActualStopRoll()
    {
        StopCoroutine(rollCoroutine);

        wheelDrive.BeginRotate(0, valStorage.brakesTorque);

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
        yield return new WaitForSeconds(valStorage.unstickableDelay);
        sticker.StickAbility = true;
    }

    IEnumerator Roll()
    {
        yield return new WaitForFixedUpdate();

        float speed = startRollingSpeed;
        float neededDelta = targetRollingSpeed - startRollingSpeed;
        float deltaPerFixedUpdate = neededDelta / valStorage.accelerationTime * Time.fixedDeltaTime;

        float timeSpended = 0;

        while(timeSpended < valStorage.accelerationTime)
        {
            wheelDrive.BeginRotate(speed, valStorage.rotationTorque);
            speed += deltaPerFixedUpdate;
            timeSpended += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        wheelDrive.BeginRotate(targetRollingSpeed, valStorage.rotationTorque);
    }

    IEnumerator Pull()
    {
        float pullDelay = 1 / valStorage.webPullSpeed;

        while(true)
        {
            webProducer.Pull();
            yield return new WaitForSeconds(pullDelay);
        }
    }

    IEnumerator Release()
    {
        float releaseDelay = 1 / valStorage.webReleaseSpeed;

        while(true)
        {
            webProducer.Release();
            yield return new WaitForSeconds(releaseDelay);
        }
    }
}
