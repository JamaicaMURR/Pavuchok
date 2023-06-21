using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebStriker : CharConnected
{
    Action DoOnProduceWeb;
    Action DoOnCutWeb;
    Action<float> DoOnChangeWebLength;
    Action DoOnActivateLengthChanging;
    Action DoOnDeactivateLengthChanging;

    Action<Vector2> ProduceChute;
    Action DropAnchor;
    Action DropChute;
    Action<float> GrabWeb;

    Action DoOnFixedUpdate;

    Func<Vector2> GetWebPoint;

    DistanceJoint2D _rootJoint;

    public WebAnchor webAnchor;
    public WebChute webChute;

    //==================================================================================================================================================================
    void OnWebAbilityEnable()
    {
        Physics.EBecomeFly += ActivateWebProducing;
        Physics.EBecomeTouch += DeactivateWebProducing;
    }

    void OnWebAbilityDisable()
    {
        DeactivateWebProducing();
        Physics.EBecomeFly -= ActivateWebProducing;
        Physics.EBecomeTouch -= DeactivateWebProducing;
    }

    void OnLengthChangingAbilityEnable()
    {
        DoOnActivateLengthChanging = ActualActivateLengthChanging;
        DoOnDeactivateLengthChanging = ActualDeactivateLengthChanging;
    }

    void OnLengthChangingAbilityDisable()
    {
        DeactivateLengthChanging();
        DoOnActivateLengthChanging = () => { };
        DoOnDeactivateLengthChanging = () => { };
    }

    void OnChuteAbilityEnable()
    {
        ProduceChute = ActualProduceChute;
    }

    void OnChuteAbilityDisable()
    {
        ProduceChute = (v) => { };
    }

    //==================================================================================================================================================================
    private void Awake()
    {
        ConnectToCharacter();

        Character.EWebAbilityEnable += OnWebAbilityEnable;
        Character.EWebAbilityDisable += OnWebAbilityDisable;

        Character.EChangeWebLengthAbilityEnable += OnLengthChangingAbilityEnable;
        Character.EChangeWebLengthAbilityDisable += OnLengthChangingAbilityDisable;

        Character.EChuteAbilityEnable += OnChuteAbilityEnable;
        Character.EChuteAbilityDisable += OnChuteAbilityDisable;

        Character.EProduceWeb += OnProduceWeb;
        Character.EChangeWebLength += OnChangeWebLength;

        Character.ECutWeb += OnCutWeb;
        webAnchor.EAnchorCut += OnCutWeb;

        _rootJoint = GetComponent<DistanceJoint2D>();

        DoOnProduceWeb = () => { };
        DoOnCutWeb = () => { };
        DoOnChangeWebLength = (_) => { };
        DoOnActivateLengthChanging = () => { };
        DoOnDeactivateLengthChanging = () => { };

        ProduceChute = (_) => { };
        DropAnchor = () => { };
        DropChute = () => { };
        GrabWeb = (_) => { };

        DoOnFixedUpdate = () => { };

        GetWebPoint = () => { return Vector2.zero; };
    }

    private void FixedUpdate() => DoOnFixedUpdate();

    //==================================================================================================================================================================
    void OnProduceWeb() => DoOnProduceWeb();
    void OnCutWeb() => DoOnCutWeb();
    void OnChangeWebLength(float delta) => DoOnChangeWebLength(delta);

    //==================================================================================================================================================================
    void ActivateWebProducing() => DoOnProduceWeb = ActualProduceWeb;

    void DeactivateWebProducing()
    {
        DoOnProduceWeb = () => { };
        OnCutWeb();
    }

    void ActualProduceChute(Vector2 position)
    {
        ActivateWebServing();
        SetChute(position);
    }

    void ActualProduceWeb()
    {
        Vector2 position = transform.position;
        Vector2 direction = Physics.CursorPosition - position;
        float distance = direction.magnitude;

        if(distance > Sets.maximalStrikeDistance)
            distance = Sets.maximalStrikeDistance;
        else if(distance < Sets.minimalWebLength)
            distance = Sets.minimalWebLength;

        RaycastHit2D rayHit = Physics2D.Raycast(position, direction, distance, layerMask: LayerMask.GetMask("Default"));

        if(rayHit.transform != null)
        {
            ActivateWebServing();
            SetWebAnchor(rayHit.point, rayHit.transform);
        }
        else
            ProduceChute(direction.normalized * distance + position);
    }

    // WebGrabbing: grab web when closer to WebPoint
    void ActivateWebGrabbing() => GrabWeb = ActualGrabWeb;
    void DeactivateWebGrabbing() => GrabWeb = (_) => { };

    void ActualGrabWeb(float distance)
    {
        if(_rootJoint.distance > distance && distance > Sets.minimalWebLength)
            _rootJoint.distance = distance;
    }

    // WebCutting
    void ActivateWebCutting() => DoOnCutWeb = ActualCutWeb;
    void DeactivateWebCutting() => DoOnCutWeb = () => { };

    void ActualCutWeb()
    {
        DropAnchor();
        DropChute();

        DeactivateWebCutting();
        DeactivateWebPointRefreshing();
        DeactivateLengthChanging();

        Character.WebCutted();
    }

    // WebLengthChanging
    void ActivateLengthChanging() => DoOnActivateLengthChanging();
    void DeactivateLengthChanging() => DoOnDeactivateLengthChanging();

    void ActualActivateLengthChanging() => DoOnChangeWebLength = ActualChangeWebLength;
    void ActualDeactivateLengthChanging() => DoOnChangeWebLength = (d) => { };

    void ActualChangeWebLength(float delta)
    {
        float distance = _rootJoint.distance + delta;

        if(distance <= Sets.maximalWebLength && distance >= Sets.minimalWebLength)
            _rootJoint.distance = distance;
    }

    // WebPointRefreshing
    void ActivateWebPointRefreshing() => DoOnFixedUpdate = RefreshWebPoint;
    void DeactivateWebPointRefreshing() => DoOnFixedUpdate = () => { };

    void RefreshWebPoint()
    {
        Vector2 direction = GetWebPoint() - (Vector2)transform.position;
        float distance = direction.magnitude;
        RaycastHit2D rayHit = Physics2D.Raycast((Vector2)transform.position, direction, distance, layerMask: LayerMask.GetMask("Default"));

        if(rayHit.collider is not null)
        {
            if(rayHit.point != GetWebPoint())
                SetWebAnchor(rayHit.point, rayHit.transform);
        }

        GrabWeb(distance);
    }

    void ActivateWebServing()
    {
        ActivateWebCutting();
        ActivateWebPointRefreshing();
        ActivateLengthChanging();
    }

    //==================================================================================================================================================================
    void SetWebAnchor(Vector2 position, Transform objTransform)
    {
        if(!webAnchor.isActiveAndEnabled)
        {
            DropChute();

            webAnchor.gameObject.SetActive(true);
            _rootJoint.enabled = true;
            _rootJoint.connectedBody = webAnchor.Body;
        }

        webAnchor.transform.position = position;
        _rootJoint.distance = Vector2.Distance((Vector2)transform.position, position);

        Character.WebDone(webAnchor.transform);
        GetWebPoint = GetAnchorPosition;

        DropAnchor = DeleteWebAnchor;

        webAnchor.Attach(objTransform);
    }

    void DeleteWebAnchor()
    {
        webAnchor.Detach();

        webAnchor.gameObject.SetActive(false);
        _rootJoint.enabled = false;

        DropAnchor = () => { };
    }

    void SetChute(Vector2 position)
    {
        if(!webChute.isActiveAndEnabled)
        {
            DropAnchor();

            webChute.gameObject.SetActive(true);
            _rootJoint.enabled = true;
            _rootJoint.connectedBody = webChute.Body;
        }

        webChute.Activate(transform);

        webChute.transform.position = position;
        _rootJoint.distance = Vector2.Distance((Vector2)transform.position, position);

        Character.WebDone(webChute.transform);
        GetWebPoint = GetChutePosition;

        DropChute = DeleteWebChute;

        ActivateWebGrabbing();
    }

    void DeleteWebChute()
    {
        webChute.gameObject.SetActive(false);
        _rootJoint.enabled = false;

        DropChute = () => { };

        DeactivateWebGrabbing();
    }

    //==================================================================================================================================================================
    Vector2 GetAnchorPosition() => webAnchor.Position;

    Vector2 GetChutePosition() => webChute.Position;
}
