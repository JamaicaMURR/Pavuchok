using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpArrowController : MonoBehaviour
{
    Action EnableDisableArrow;
    Action DoOnCollisionStay;

    SpriteRenderer arrowSpriteRenderer;
    Sticker sticker;

    //==================================================================================================================================================================
    public GameObject arrow;

    public CharController charController;
    public RelativeJumper relativeJumper;

    public Sprite jumpNotChargedArrow;
    public Sprite jumpChargedArrow;

    //==================================================================================================================================================================
    private void Awake()
    {
        arrowSpriteRenderer = arrow.GetComponent<SpriteRenderer>();
        sticker = GetComponent<Sticker>();

        DoOnCollisionStay = () => { };
        EnableDisableArrow = () => { };

        charController.OnBecomeStand += delegate ()
        {
            DoOnCollisionStay = delegate ()
            {
                OrientateAcrossJumpDirection();
                DoOnCollisionStay = () => { };
            };

            EnableDisableArrow = delegate ()
            {
                arrow.SetActive(true);
                EnableDisableArrow = () => { };
            };
        };

        charController.OnBecomeMove += delegate ()
        {
            DoOnCollisionStay = () => { };

            EnableDisableArrow = delegate ()
            {
                arrowSpriteRenderer.sprite = jumpNotChargedArrow;
                arrow.SetActive(false);

                EnableDisableArrow = () => { };
            };
        };

        relativeJumper.OnChargingComplete += delegate ()
        {
            arrowSpriteRenderer.sprite = jumpChargedArrow;
        };

        relativeJumper.OnChargingCancelled += delegate ()
        {
            arrowSpriteRenderer.sprite = null;
        };

        charController.OnCancelledJumpRelease += delegate ()
        {
             arrowSpriteRenderer.sprite = jumpNotChargedArrow;
        };
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EnableDisableArrow();
        DoOnCollisionStay();
    }

    //==================================================================================================================================================================
    void OrientateAcrossJumpDirection()
    {
        arrow.transform.rotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(sticker.SurfaceDirection, Vector2.down)); // Vector to surface calculation does only in Sticker
    }
}
