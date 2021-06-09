using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chute : MonoBehaviour
{
    new CircleCollider2D collider;
    new Rigidbody2D rigidbody;
    SpriteRenderer spriteRenderer;
    WebKnot rootKnot;

    int frameNumber;

    Action DoOnUpdate;
    Action DoOnFixedUpdate;
    //==================================================================================================================================================================
    public float deployingSpeed;
    public float initialDeployedDrag;
    public float fullDeployedDrag;
    public float colliderRelativeConstant;

    public Sprite[] frames;
    //==================================================================================================================================================================
    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        DoOnUpdate = delegate () { };
        DoOnFixedUpdate = delegate () { };
    }

    private void Update()
    {
        DoOnUpdate();
    }

    private void FixedUpdate()
    {
        DoOnFixedUpdate();
    }
    //==================================================================================================================================================================
    public void StartDeploy(WebKnot root)
    {
        if(root != null)
        {
            rootKnot = root;
            DoOnUpdate = MonitorDeploying;
        }
    }

    //==================================================================================================================================================================
    void MonitorDeploying()
    {
        if(rigidbody.velocity.magnitude >= deployingSpeed)
        {
            DoOnFixedUpdate = Deploying;
            DoOnUpdate = OrientateAcrossVelocity;
        }
    }

    void Deploying()
    {
        if(frameNumber != frames.Length)
        {
            spriteRenderer.sprite = frames[frameNumber];
            collider.radius = frames[frameNumber].rect.width / 2 * colliderRelativeConstant;
            rigidbody.drag = initialDeployedDrag + (fullDeployedDrag - initialDeployedDrag) / (frames.Length - 1) * frameNumber;
            frameNumber++;
        }
        else
            DoOnFixedUpdate = delegate () { };

    }

    void OrientateAcrossVelocity()
    {
        if(transform.position.x >= rootKnot.transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, -Vector2.Angle((Vector2)transform.position - (Vector2)rootKnot.transform.position, Vector2.up));
        else
            transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle((Vector2)transform.position - (Vector2)rootKnot.transform.position, Vector2.up));
    }
}
