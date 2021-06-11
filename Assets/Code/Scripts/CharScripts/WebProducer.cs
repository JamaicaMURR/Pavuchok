using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebProducer : MonoBehaviour
{
    float knotDistance;

    Action DoOnPull;
    Action DoOnRelease;
    Action DoOnCut;

    WebKnot rootKnot;

    //==================================================================================================================================================================
    public GameObject webKnotPrefab;

    public LinkedList<WebKnot> knots;

    public DistanceJoint2D rootJoint;

    public int knotsLimit = 80;

    public float maximalShootDistance = 10;
    public float minimalWebLength = 1;

    public float pullCompensationImpulse = 1;

    public event Action OnWebDone;
    public event Action OnWebCut;

    //==================================================================================================================================================================
    private void Awake()
    {
        knotDistance = webKnotPrefab.GetComponent<DistanceJoint2D>().distance;

        DoOnPull = delegate () { };
        DoOnRelease = delegate () { };
        DoOnCut = delegate () { };

        knots = new LinkedList<WebKnot>();
    }

    //==================================================================================================================================================================
    public void ProduceWeb(Vector2 targetPoint) // TODO: Fix bug with uncorrect position of last knot
    {
        Vector2 direction = targetPoint - (Vector2)transform.position;
        float distance = Vector2.Distance(transform.position, targetPoint);

        if(distance > maximalShootDistance)
            distance = maximalShootDistance;
        else if(distance < minimalWebLength)
            distance = minimalWebLength;

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, direction, distance, layerMask: LayerMask.GetMask("Default"));

        WebKnot lastKnot;

        if(rayHit.transform != null)
        {
            lastKnot = MakeWeb(rayHit.point);
            lastKnot.BecomeAnchor(rayHit.collider);
        }
        else
        {
            Vector2 chutePoint = direction.normalized * distance + (Vector2)transform.position;

            lastKnot = MakeWeb(chutePoint);
            lastKnot.TransformAtChute();
        }
    }

    public void Pull()
    {
        DoOnPull();
    }

    public void Release()
    {
        DoOnRelease();
    }

    public void CutWeb()
    {
        DoOnCut();
    }

    //==================================================================================================================================================================
    void ActualPull()
    {
        if(knots.Count * knotDistance > minimalWebLength)
        {
            if(rootKnot.NextKnot != null)
            {
                rootJoint.connectedBody = rootKnot.NextKnot.GetComponent<Rigidbody2D>();

                WebKnot knotToDestroy = rootKnot;
                rootKnot = rootKnot.NextKnot;
                knotToDestroy.DestroySelf();
            }
        }
    }

    void ActualRelease()
    {
        if(knots.Count < knotsLimit)
        {
            WebKnot newbie = GetNewKnot();

            newbie.transform.position = transform.position;
            newbie.NextKnot = rootKnot;
            rootKnot = newbie;
            rootJoint.connectedBody = newbie.GetComponent<Rigidbody2D>();
        }
    }

    void ActualCut()
    {
        rootJoint.enabled = false;

        rootKnot.ChainDestroy();

        DoOnPull = delegate () { };
        DoOnRelease = delegate () { };
        DoOnCut = delegate () { };

        if(OnWebCut != null)
            OnWebCut();
    }

    //==================================================================================================================================================================
    WebKnot MakeWeb(Vector2 calculatedPoint)
    {
        float distance = Vector2.Distance(transform.position, calculatedPoint);
        int knotsToSpawn = (int)(distance / knotDistance);
        float step = distance / knotsToSpawn / distance;

        rootKnot = GetNewKnot(addToKnotsListAsFirst: true);

        rootKnot.transform.position = Vector2.Lerp(transform.position, calculatedPoint, step);

        rootJoint.enabled = true;
        rootJoint.distance = distance - (knotsToSpawn - 1) * knotDistance;
        rootJoint.connectedBody = rootKnot.GetComponent<Rigidbody2D>();

        WebKnot spawnedKnot = rootKnot;

        for(int i = 1; i < knotsToSpawn; i++)
        {
            WebKnot newbie = GetNewKnot(addToKnotsListAsFirst: true);

            newbie.transform.position = Vector2.Lerp(transform.position, calculatedPoint, step * (i + 1));

            spawnedKnot.NextKnot = newbie;
            spawnedKnot = newbie;
        }

        DoOnPull = ActualPull;
        DoOnRelease = ActualRelease;
        DoOnCut = ActualCut;

        if(OnWebDone != null)
            OnWebDone();

        return spawnedKnot; //Last knot returns
    }

    WebKnot GetNewKnot(bool addToKnotsListAsFirst = false)
    {
        GameObject newbie = Instantiate(webKnotPrefab);
        WebKnot newbieKnot = newbie.GetComponent<WebKnot>();

        if(addToKnotsListAsFirst)
            knots.AddFirst(newbieKnot);
        else
            knots.AddLast(newbieKnot);

        newbieKnot.OnSelfDestroy += delegate () { knots.Remove(newbieKnot); };

        return newbieKnot;
    }
}
