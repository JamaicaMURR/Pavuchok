using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebKnot : MonoBehaviour
{
    WebKnot _nextKnot;
    WebKnot _previousKnot;

    Collision2DHandler DoOnCollision;

    //==================================================================================================================================================================
    public DistanceJoint2D joint;

    public float chuteTransformingDelay = 0.5f;

    public event Action OnSelfDestroy;

    public WebKnot NextKnot
    {
        get { return _nextKnot; }
        set
        {
            _nextKnot = value;

            if(_nextKnot != null)
            {
                joint.enabled = true;
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
        DoOnCollision = Stick;
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

    //==================================================================================================================================================================
    public void BecomeAnchor(Collider2D collider)
    {
        StopAllCoroutines();
        GetComponent<SpriteRenderer>().sprite = null;

        // Different behavior for static and physical objects
        if(collider.attachedRigidbody == null)
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        else
        {
            joint.enabled = true;
            joint.autoConfigureConnectedAnchor = true;
            joint.distance = 0;
            joint.connectedBody = collider.attachedRigidbody;
        }
    }

    public void TransformAtChute(float delay = 0)
    {
        if(PreviousKnot != null)
        {
            joint.enabled = false;
            StartCoroutine(ChuteActivation(delay));
        }
    }

    // DoOnCollision options
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
        Chute chute = GetComponent<Chute>();

        chute.Prepare();

        yield return new WaitForSeconds(delay);

        chute.enabled = true;
        chute.Activate(root: PreviousKnot);

        DoOnCollision = Collapse;
    }
}
