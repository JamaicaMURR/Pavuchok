using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebKnot : MonoBehaviour
{
    WebKnot _nextKnot;
    //==================================================================================================================================================================

    public Action OnSelfDestroy;

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
    private void OnCollisionEnter2D(Collision2D collision) //Problem with web in webproducer!!!!!!!
    {
        BecomeAnchor();

        if(NextKnot != null)
            NextKnot.ChainDestroy();
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

    public void BecomeAnchor()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
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
}
