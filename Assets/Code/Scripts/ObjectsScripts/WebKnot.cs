using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebKnot : MonoBehaviour
{
    float breakingGapSize;

    WebKnot _nextKnot;
    WebKnot _previousKnot;

    Collision2DHandler DoOnCollision;

    //==================================================================================================================================================================
    public DistanceJoint2D joint;

    public float breakingGapSizeModifier = 1.5f;
    public float chuteTransformingDelay = 0.5f;

    public Sprite chuteSprite;

    public event Action OnSelfDestroy;
    public event Action OnRip;

    public WebKnot NextKnot
    {
        get { return _nextKnot; }
        set
        {
            _nextKnot = value;

            if(_nextKnot != null)
            {
                joint.connectedBody = _nextKnot.GetComponent<Rigidbody2D>();
                _nextKnot.PreviousKnot = this;
            }

        }
    }

    public WebKnot PreviousKnot
    {
        get { return _previousKnot; }
        set { _previousKnot = value; }
    }

    //==================================================================================================================================================================
    private void Awake()
    {
        breakingGapSize = joint.distance * breakingGapSizeModifier;
        DoOnCollision = Stick;
    }

    private void FixedUpdate() // EXP:
    {
        //if(NextKnot != null && Vector2.Distance(transform.position, NextKnot.transform.position) > breakingGapSize)
        //{
        //    NextKnot.ChainDestroy();
        //    TransformAtChute();

        //    if(OnRip != null)
        //        OnRip();
        //}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DoOnCollision(collision);
    }

    private void OnDestroy()
    {
        if(OnSelfDestroy != null)
            OnSelfDestroy();
    }

    //==================================================================================================================================================================
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ChainDestroy()
    {
        if(NextKnot != null)
            NextKnot.ChainDestroy();

        DestroySelf();
    }

    public void BecomeAnchor(Collider2D collider)
    {
        if(collider.gameObject.tag != "WebUnstickable")
        {
            StopAllCoroutines();
            GetComponent<SpriteRenderer>().sprite = null;

            if(collider.attachedRigidbody == null)
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            else
            {
                joint.autoConfigureConnectedAnchor = true;
                joint.distance = 0;
                joint.connectedBody = collider.attachedRigidbody;
            }
        }
        else
            TransformAtChute();
    }

    public void TransformAtChute(float delay = 0)
    {
        if(PreviousKnot != null)
        {
            StartCoroutine(ChuteActivation(delay));
        }
    }

    //public void Release(GameObject obj)
    //{
    
    //}

    //==================================================================================================================================================================
    void Stick(Collision2D collision)
    {
        BecomeAnchor(collision.collider);

        if(NextKnot != null)
            NextKnot.ChainDestroy();
    }

    void Collapse(Collision2D collision)
    {
        PreviousKnot.TransformAtChute(chuteTransformingDelay);
        DestroySelf();
    }

    //==================================================================================================================================================================
    IEnumerator ChuteActivation(float delay)
    {
        GetComponent<SpriteRenderer>().sprite = chuteSprite;

        yield return new WaitForSeconds(delay);

        joint.enabled = false;
        GetComponent<Chute>().Activate(PreviousKnot);
        DoOnCollision = Collapse;
    }
}
