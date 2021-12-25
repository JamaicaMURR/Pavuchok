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
    Action<Vector2, float> DoOnChuteProducing;
    Action<Vector2> DoOnWebProducing;

    Action PullReleaseActivate;
    Action PullReleaseDeactivate;

    WebKnot rootKnot;

    new Rigidbody2D rigidbody;

    //==================================================================================================================================================================
    public GameObject webKnotPrefab;
    public LinkedList<WebKnot> knots;
    public DistanceJoint2D rootJoint;

    [HideInInspector]
    public int knotsLimit;

    [HideInInspector]
    public float maximalShootDistance, minimalWebLength, reactionImpulsePerShotedKnot;

    public event Action OnWebDone;
    public event Action OnWebCut;

    //==================================================================================================================================================================
    public bool WebAbility
    {
        get { return DoOnWebProducing == ActualProduceWeb; }

        set
        {
            if(value)
                DoOnWebProducing = ActualProduceWeb;
            else
                DoOnWebProducing = delegate (Vector2 v2) { };
        }
    }

    public bool PullReleaseAbility
    {
        get { return PullReleaseActivate == ActualPullReleaseActivate; }

        set
        {
            if(value)
            {
                PullReleaseActivate = ActualPullReleaseActivate;
                PullReleaseDeactivate = ActualPullReleaseDeactivate;
            }
            else
            {
                PullReleaseActivate = delegate () { };
                PullReleaseDeactivate = delegate () { };
            }
        }
    }

    public bool ChuteAbility
    {
        get { return DoOnChuteProducing == ActualProduceChute; }

        set
        {
            if(value)
                DoOnChuteProducing = ActualProduceChute;
            else
                DoOnChuteProducing = delegate (Vector2 v2, float f) { };
        }
    }

    //==================================================================================================================================================================
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        knotDistance = webKnotPrefab.GetComponent<DistanceJoint2D>().distance;

        DoOnPull = delegate () { };
        DoOnRelease = delegate () { };
        DoOnCut = delegate () { };

        WebAbility = false;
        PullReleaseAbility = false;
        ChuteAbility = false;

        knots = new LinkedList<WebKnot>();
    }

    //==================================================================================================================================================================
    public void ProduceWeb(Vector2 targetPoint)
    {
        DoOnWebProducing(targetPoint);
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

        PullReleaseDeactivate();

        DoOnCut = delegate () { };

        if(OnWebCut != null)
            OnWebCut();
    }

    void ActualProduceWeb(Vector2 targetPoint)
    {
        Vector2 direction = targetPoint - (Vector2)transform.position;
        float distance = Vector2.Distance(transform.position, targetPoint);

        if(distance > maximalShootDistance)
            distance = maximalShootDistance;
        else if(distance < minimalWebLength)
            distance = minimalWebLength;

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, direction, distance, layerMask: LayerMask.GetMask("Default"));

        //EXP: pay attention (Make Web Become Anchor)
        if(rayHit.transform != null)
            MakeWeb(rayHit.point);
        else
            DoOnChuteProducing(direction, distance);
    }

    void ActualPullReleaseActivate()
    {
        DoOnPull = ActualPull;
        DoOnRelease = ActualRelease;
    }

    void ActualPullReleaseDeactivate()
    {
        DoOnPull = delegate () { };
        DoOnRelease = delegate () { };
    }

    void ActualProduceChute(Vector2 direction, float distance)
    {
        Vector2 chutePoint = direction.normalized * distance + (Vector2)transform.position;

        MakeWeb(chutePoint).TransformAtChute(); //MakeWeb returns last knot, last knot transforms at chute 
    }

    //==================================================================================================================================================================
    WebKnot MakeWeb(Vector2 calculatedPoint)
    {
        float distance = Vector2.Distance(transform.position, calculatedPoint);

        int knotsToSpawn = (int)(distance / knotDistance);

        float firstGap = distance - (knotsToSpawn - 1) * knotDistance;

        float firstStep = firstGap / distance;
        float step = knotDistance / distance;


        rootKnot = GetNewKnot(addToKnotsListAsFirst: true);

        rootKnot.transform.position = Vector2.Lerp(transform.position, calculatedPoint, firstStep);

        rootJoint.enabled = true;
        rootJoint.distance = firstGap;
        rootJoint.connectedBody = rootKnot.GetComponent<Rigidbody2D>();

        WebKnot spawnedKnot = rootKnot;

        for(int i = 1; i < knotsToSpawn; i++)
        {
            WebKnot newbie = GetNewKnot(addToKnotsListAsFirst: true);

            newbie.transform.position = Vector2.Lerp(transform.position, calculatedPoint, firstStep + step * (i + 1));

            spawnedKnot.NextKnot = newbie;
            spawnedKnot = newbie;
        }

        PullReleaseActivate();

        DoOnCut = ActualCut;

        if(OnWebDone != null)
            OnWebDone();

        rigidbody.AddForce(-(calculatedPoint - (Vector2)transform.position).normalized * reactionImpulsePerShotedKnot * knots.Count, ForceMode2D.Impulse);

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
