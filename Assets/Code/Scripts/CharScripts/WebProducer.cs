using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebProducer : MonoBehaviour
{
    int _knotsLimit;

    float maximalLength;
    float minimalLength;
    float knotDistance;

    int knotsCount;

    WebKnot root;

    DistanceJoint2D rootLink;

    Action DoOnPull;
    Action DoOnRelease;
    Action DoOnCut;

    //==================================================================================================================================================================
    public GameObject webKnotPrefab;

    public Action OnWebDone;
    public Action OnWebCut;

    public int KnotsLimit
    {
        get { return _knotsLimit; }
        set
        {
            _knotsLimit = value;
            maximalLength = _knotsLimit * knotDistance;
        }
    }
    //==================================================================================================================================================================
    private void Awake()
    {
        rootLink = GetComponent<DistanceJoint2D>();
        knotDistance = webKnotPrefab.GetComponent<DistanceJoint2D>().distance;

        minimalLength = GetComponent<CircleCollider2D>().radius;

        DoOnPull = delegate () { };
        DoOnRelease = delegate () { };
        DoOnCut = delegate () { };
    }

    //==================================================================================================================================================================
    public void ProduceWeb(Vector2 targetPoint)
    {
        Vector2 direction = targetPoint - (Vector2)transform.position;
        float distance = Vector2.Distance(transform.position, targetPoint);

        if(distance > maximalLength)
            distance = maximalLength;
        else if(distance < minimalLength)
            distance = minimalLength;

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, direction, distance, layerMask: LayerMask.GetMask("Default"));

        if(rayHit.transform != null)
        {
            if(rayHit.distance > minimalLength)
            {
                WebKnot lastKnot = MakeWeb(rayHit.point);
                lastKnot.BecomeAnchor(rayHit.collider);
            }
        }
        else
        {
            Vector2 chutePoint = direction.normalized * distance + (Vector2)transform.position;

            WebKnot lastKnot = MakeWeb(chutePoint);
            lastKnot.BecomeChute();
            lastKnot.OnCollapse += delegate () { CutWeb(); };
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
        if(root.NextKnot != null)
            root.Pull();

        if(knotsCount == 0)
            CutWeb();
    }

    void ActualRelease()
    {
        if(knotsCount < KnotsLimit)
        {
            GameObject newKnot = Instantiate(webKnotPrefab);

            root.Release(newKnot);

            knotsCount++;
            newKnot.GetComponent<WebKnot>().OnSelfDestroy += delegate () { knotsCount--; };
        }
    }

    void ActualCut()
    {
        rootLink.enabled = false;

        root.ChainDestroy();

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

        GameObject rootKnot = Instantiate(webKnotPrefab);

        rootKnot.transform.position = transform.position;
        root = rootKnot.GetComponent<WebKnot>();

        WebKnot spawnedKnot = root;

        for(int i = 0; i < knotsToSpawn; i++)
        {
            GameObject newbie = Instantiate(webKnotPrefab);

            newbie.transform.position = Vector2.Lerp(transform.position, calculatedPoint, step * (i + 1));

            WebKnot knotComponentOfNewbie = newbie.GetComponent<WebKnot>();

            spawnedKnot.NextKnot = knotComponentOfNewbie;
            spawnedKnot = knotComponentOfNewbie;

            knotsCount++;
            knotComponentOfNewbie.OnSelfDestroy += delegate () { knotsCount--; };
        }

        rootLink.enabled = true;
        rootLink.connectedBody = root.GetComponent<Rigidbody2D>();

        DoOnPull = ActualPull;
        DoOnRelease = ActualRelease;
        DoOnCut = ActualCut;

        if(OnWebDone != null)
            OnWebDone();

        return spawnedKnot; //Last knot returns
    }
}
