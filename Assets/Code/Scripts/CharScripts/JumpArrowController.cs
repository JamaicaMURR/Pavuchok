using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpArrowController : MonoBehaviour
{
    Action DoOnUpdate;
    Action DoOnWebDone;
    Action DoOnWebCut;

    SpriteRenderer arrowSpriteRenderer;
    Sticker sticker;
    WebProducer webProducer;
    ValStorage valStorage;

    new Rigidbody2D rigidbody;

    //==================================================================================================================================================================
    public GameObject arrow;

    public CharController charController;
    public RelativeJumper relativeJumper;

    public Sprite jumpNotChargedArrow;
    public Sprite jumpChargedArrow;
    public Sprite inJumpVelocityArrow;

    //==================================================================================================================================================================
    private void Awake()
    {
        valStorage = GameObject.Find("Master").GetComponent<ValStorage>();

        arrowSpriteRenderer = arrow.GetComponent<SpriteRenderer>();
        sticker = GetComponent<Sticker>();
        webProducer = GetComponent<WebProducer>();
        rigidbody = GetComponent<Rigidbody2D>();

        DoOnUpdate = () => { };

        DoOnWebDone = () => { };
        DoOnWebCut = () => { };

        webProducer.OnWebDone += () => DoOnWebDone();
        webProducer.OnWebCut += () => DoOnWebCut();

        charController.OnBecomeStand += delegate ()
        {
            arrowSpriteRenderer.sprite = jumpNotChargedArrow;
            StartCoroutine(EnableArrowAfterDelay());
            OrientateAcrossJumpDirection();
        };

        charController.OnBecomeMove += delegate ()
        {
            StopAllCoroutines();
            arrow.SetActive(false);
        };

        charController.OnBecomeFly += delegate ()
        {
            DoOnWebDone = delegate ()
            {
                arrowSpriteRenderer.sprite = null;

                StopAllCoroutines();
                arrow.SetActive(false);

                DoOnUpdate = () => { };
            };

            DoOnWebCut = delegate ()
            {
                arrowSpriteRenderer.sprite = inJumpVelocityArrow;
                StartCoroutine(EnableArrowAfterDelay());
                DoOnUpdate = OrientateAcrossVelocity;
            };

            DoOnWebCut(); // Velocity arrow appears
        };

        charController.OnBecomeTouch += delegate ()
        {
            DoOnWebDone(); // Velocity arrow disappears

            DoOnWebDone = () => { };
            DoOnWebCut = () => { };
        };

        relativeJumper.OnChargingComplete += () => arrowSpriteRenderer.sprite = jumpChargedArrow;

        relativeJumper.OnChargingCancelled += () => arrowSpriteRenderer.sprite = null;

        charController.OnCancelledJumpRelease += () => arrowSpriteRenderer.sprite = jumpNotChargedArrow;
    }


    private void Update()
    {
        DoOnUpdate();
    }

    //==================================================================================================================================================================
    void OrientateAcrossJumpDirection()
    {
        arrow.transform.rotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(sticker.SurfaceDirection, Vector2.down)); // Vector to surface calculation does only in Sticker
    }

    void OrientateAcrossVelocity()
    {
        arrow.transform.rotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(rigidbody.velocity, Vector2.up));
    }

    //==================================================================================================================================================================
    IEnumerator EnableArrowAfterDelay()
    {
        yield return new WaitForSeconds(valStorage.velocityArrowDelay);
        arrow.SetActive(true);
    }
}
