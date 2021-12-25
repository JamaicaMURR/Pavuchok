using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpArrowController : MonoBehaviour
{
    Action EnableDisableArrow;
    Collision2DHandler DoOnCollisionStay;

    SpriteRenderer arrowSpriteRenderer;

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

        DoOnCollisionStay = delegate (Collision2D c) { };
        EnableDisableArrow = delegate () { };

        charController.OnBecomeStand += delegate ()
        {
            DoOnCollisionStay = OrientateAcrossJumpDirection;

            EnableDisableArrow = delegate ()
            {
                arrow.SetActive(true);
                EnableDisableArrow = delegate () { };
            };
        };

        charController.OnBecomeMove += delegate ()
        {
            DoOnCollisionStay = delegate (Collision2D c) { };

            EnableDisableArrow = delegate ()
            {
                arrowSpriteRenderer.sprite = jumpNotChargedArrow;
                arrow.SetActive(false);

                EnableDisableArrow = delegate () { };
            };
        };

        relativeJumper.OnChargingComplete += delegate ()
        {
            arrowSpriteRenderer.sprite = jumpChargedArrow;
        };
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EnableDisableArrow();
        DoOnCollisionStay(collision);
    }

    //==================================================================================================================================================================
    void OrientateAcrossJumpDirection(Collision2D collision)
    {
        arrow.transform.rotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle((Vector2)transform.position - collision.contacts[0].point, Vector2.up));
    }
}
