using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebProducer : MonoBehaviour
{
    float knotDistance;

    WebKnot root;

    DistanceJoint2D rootLink;

    Action DoOnPull;
    Action DoOnRelease;
    Action DoOnCut;

    //==================================================================================================================================================================
    public GameObject webKnotPrefab;

    public LinkedList<GameObject> knots;

    public Action OnWebDone;
    public Action OnWebCut;

    public int knotsLimit = 80;

    public float maximalShootDistance = 7.5f;
    public float minimalShootDistance = 0.5f;

    //==================================================================================================================================================================
    private void Awake()
    {
        rootLink = GetComponent<DistanceJoint2D>();
        knotDistance = webKnotPrefab.GetComponent<DistanceJoint2D>().distance;

        DoOnPull = delegate () { };
        DoOnRelease = delegate () { };
        DoOnCut = delegate () { };

        knots = new LinkedList<GameObject>();
    }

    //==================================================================================================================================================================
    public void ProduceWeb(Vector2 targetPoint)
    {
        Vector2 direction = targetPoint - (Vector2)transform.position;
        float distance = Vector2.Distance(transform.position, targetPoint);

        if(distance > maximalShootDistance)
            distance = maximalShootDistance;
        else if(distance < minimalShootDistance)
            distance = minimalShootDistance;

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
        root.Pull();

        if(knots.Count == 0)
            CutWeb();
    }

    void ActualRelease()
    {
        if(knots.Count < knotsLimit)
            root.Release(GetNewKnot());
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
            GameObject newbie = GetNewKnot(addToKnotsListAsFirst: true);

            newbie.transform.position = Vector2.Lerp(transform.position, calculatedPoint, step * (i + 1));

            WebKnot knotComponentOfNewbie = newbie.GetComponent<WebKnot>();

            spawnedKnot.NextKnot = knotComponentOfNewbie;
            spawnedKnot = knotComponentOfNewbie;
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

    GameObject GetNewKnot(bool addToKnotsListAsFirst = false)
    {
        GameObject newbie = Instantiate(webKnotPrefab);

        if(addToKnotsListAsFirst)
            knots.AddFirst(newbie);
        else
            knots.AddLast(newbie);

        newbie.GetComponent<WebKnot>().OnSelfDestroy += delegate () { knots.Remove(newbie); };

        return newbie;
    }
}
