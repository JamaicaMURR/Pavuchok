using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebKnot : MonoBehaviour
{
    new Rigidbody2D rigidbody;
    new CircleCollider2D collider;

    DistanceJoint2D joint;

    WebKnot _nextKnot;
    //==================================================================================================================================================================
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

    // TEST ONLY!!!!<><><><><><><><><><><><><><><><><><><
    public GameObject knotPrefab;
    public bool isControlable;

    public WebKnot next;

    private void Awake()
    {


        joint = GetComponent<DistanceJoint2D>();


        NextKnot = next;
    }

    private void Update()
    {
        if(isControlable)
        {
            if(Input.GetKeyDown(KeyCode.T))
                Pull();

            if(Input.GetKeyDown(KeyCode.Y))
                Release(Instantiate(knotPrefab));
        }
    }

    //==================================================================================================================================================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        if(NextKnot != null)
            NextKnot.ChainDestroy();
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

    public void Pull()
    {
        if(NextKnot != null)
        {
            WebKnot last = NextKnot;
            NextKnot = NextKnot.NextKnot;

            last.DestroySelf();
        }
    }

    public void Release(GameObject obj)
    {
        WebKnot knot = obj.GetComponent<WebKnot>();

        knot.transform.position = transform.position;
        knot.NextKnot = NextKnot;
        NextKnot = knot;
    }
}
