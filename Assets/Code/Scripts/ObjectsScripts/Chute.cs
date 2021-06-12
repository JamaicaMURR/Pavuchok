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
    Animator animator;

    int frameNumber;

    Action DoOnUpdate;
    Action DoOnFixedUpdate;

    //==================================================================================================================================================================
    public float deployingAtSpeed = 1;
    public float preparedDrag = 5;
    public float relativeDrag = 2;
    public float colliderRelativeConstant = 0.01f;
    public float deployingTime = 0.5f;

    //==================================================================================================================================================================
    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

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
    public void Activate(WebKnot root)
    {
        if(root != null)
        {
            rootKnot = root;
            DoOnUpdate = MonitorDeploying;
        }
    }

    //==================================================================================================================================================================
    public void Prepare()
    {
        rigidbody.drag = preparedDrag;
        animator.SetTrigger("ChuteReady");
    }

    public void StartDeploying() // Must be called from deploying animation
    {
        DoOnFixedUpdate = ChangePhysicsAcrossAnimation;
        DoOnUpdate = OrientateAcrossVelocity;
    }

    public void EndDeploying() // Must be called from deploying animation
    {
        DoOnFixedUpdate();
        DoOnFixedUpdate = delegate () { };
    }

    //==================================================================================================================================================================
    void MonitorDeploying()
    {
        if(rigidbody.velocity.magnitude >= deployingAtSpeed)
            animator.SetTrigger("DeployChute");
    }

    void ChangePhysicsAcrossAnimation()
    {
        collider.radius = spriteRenderer.sprite.rect.width / 2 * colliderRelativeConstant;
        rigidbody.drag = relativeDrag * collider.radius * transform.lossyScale.x;
    }

    void OrientateAcrossVelocity()
    {
        if(rootKnot != null)
        {
            if(transform.position.x >= rootKnot.transform.position.x)
                transform.rotation = Quaternion.Euler(0, 0, -Vector2.Angle((Vector2)transform.position - (Vector2)rootKnot.transform.position, Vector2.up));
            else
                transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle((Vector2)transform.position - (Vector2)rootKnot.transform.position, Vector2.up));
        }
    }
}
