using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebKnot : MonoBehaviour
{
    WebKnot _nextKnot;

    Collision2DHandler DoOnCollision;

    //==================================================================================================================================================================
    public Action OnSelfDestroy;
    public Action OnCollapse;

    public WebKnot NextKnot
    {
        get { return _nextKnot; }
        set
        {
            _nextKnot = value;

            if(_nextKnot != null)
                GetComponent<DistanceJoint2D>().connectedBody = _nextKnot.GetComponent<Rigidbody2D>();
        }
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

    public void BecomeAnchor(Collider2D collider)
    {
        if(collider.attachedRigidbody == null)
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        else
        {
            DistanceJoint2D joint = GetComponent<DistanceJoint2D>();

            joint.autoConfigureConnectedAnchor = true;
            joint.distance = 0;
            joint.connectedBody = collider.attachedRigidbody;
        }
    }

    public void ReleaseChute(GameObject obj)
    {
        Release(obj);
        NextKnot.TransformAtChute(transform);
    }

    public void TransformAtChute(Transform t)
    {
        GetComponent<DistanceJoint2D>().enabled = false;
        GetComponent<Chute>().StartDeploy(t);
        DoOnCollision = Collapse;
    }

    public void Pull() // NextKnot must not be null
    {
        WebKnot last = NextKnot;
        NextKnot = NextKnot.NextKnot;

        last.DestroySelf();
    }

    public void Release(GameObject obj)
    {
        WebKnot knot = obj.GetComponent<WebKnot>();

        knot.transform.position = transform.position;
        knot.NextKnot = NextKnot;
        NextKnot = knot;
    }

    //==================================================================================================================================================================
    void Stick(Collision2D collision)
    {
        BecomeAnchor(collision.collider);

        if(NextKnot != null)
            NextKnot.ChainDestroy();
    }

    void Collapse(Collision2D collision)
    {
        if(OnCollapse != null)
            OnCollapse();
    }
}
