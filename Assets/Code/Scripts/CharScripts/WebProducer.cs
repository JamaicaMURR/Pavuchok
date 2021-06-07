using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebProducer : MonoBehaviour
{
    float maximalLength;
    float minimalLength;
    float knotDistance;

    int knotsCount;

    WebKnot root;

    DistanceJoint2D rootLink;

    Action DoOnPull;
    Action DoOnRelease;

    //==================================================================================================================================================================
    public GameObject webKnotPrefab;

    public int maximumKnots = 20;

    //==================================================================================================================================================================
    private void Awake()
    {
        rootLink = GetComponent<DistanceJoint2D>();
        knotDistance = webKnotPrefab.GetComponent<DistanceJoint2D>().distance;

        maximalLength = maximumKnots * knotDistance;
        minimalLength = knotDistance;

        DoOnPull = delegate () { };
        DoOnRelease = delegate () { };
    }

    //<><><><><> TEST!!!
    public Camera cam;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(root != null)
                CutWeb();

            Vector2 curpos = cam.ScreenToWorldPoint(Input.mousePosition);

            ProduceWeb(curpos);
        }

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            if(root != null)
                CutWeb();
        }

        if(Input.GetKeyDown(KeyCode.W))
            Pull();

        if(Input.GetKeyDown(KeyCode.S))
            Release();

        Debug.Log(knotsCount);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, cam.ScreenToWorldPoint(Input.mousePosition));
    }

    //==================================================================================================================================================================
    public void ProduceWeb(Vector2 targetPoint)
    {
        Vector2 direction = targetPoint - (Vector2)transform.position;

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, direction, maximalLength, layerMask: LayerMask.GetMask("Default"));

        if(rayHit.transform != null)
        {
            int knotsToSpawn = (int)(rayHit.distance / knotDistance);
            float step = rayHit.distance / knotsToSpawn / rayHit.distance;

            GameObject rootKnot = Instantiate(webKnotPrefab);

            rootKnot.transform.position = transform.position;
            root = rootKnot.GetComponent<WebKnot>();

            WebKnot spawnedKnot = root;

            for(int i = 0; i < knotsToSpawn; i++)
            {
                GameObject newbie = Instantiate(webKnotPrefab);

                newbie.transform.position = Vector2.Lerp(transform.position, rayHit.point, step * (i + 1));

                WebKnot knotComponentOfNewbie = newbie.GetComponent<WebKnot>();

                spawnedKnot.NextKnot = knotComponentOfNewbie;
                spawnedKnot = knotComponentOfNewbie;

                knotsCount++;
                knotComponentOfNewbie.OnSelfDestroy += delegate () { knotsCount--; };
            }

            spawnedKnot.BecomeAnchor();

            rootLink.enabled = true;
            rootLink.connectedBody = root.GetComponent<Rigidbody2D>();

            DoOnPull = ActualPull;
            DoOnRelease = ActualRelease;
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
        rootLink.enabled = false;

        root.ChainDestroy();

        DoOnPull = delegate () { };
        DoOnRelease = delegate () { };
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
        if(knotsCount < maximumKnots)
        {
            GameObject newKnot = Instantiate(webKnotPrefab);

            root.Release(newKnot);

            knotsCount++;
            newKnot.GetComponent<WebKnot>().OnSelfDestroy += delegate () { knotsCount--; };
        }
    }
}
