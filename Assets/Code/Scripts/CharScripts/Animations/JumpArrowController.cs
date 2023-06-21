using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpArrowController : CharConnected
{
    Action DoOnUpdate;
    Action DoOnWebDone;
    Action DoOnWebCut;

    SpriteRenderer _arrowSpriteRenderer;

    //==================================================================================================================================================================
    public GameObject arrow;

    public Sprite jumpDirectionArrow;
    public Sprite VelocityArrow;

    //==================================================================================================================================================================
    private void Awake()
    {
        _arrowSpriteRenderer = arrow.GetComponent<SpriteRenderer>();

        ConnectToCharacter();

        DoOnUpdate = () => { };

        DoOnWebDone = () => { };
        DoOnWebCut = () => { };

        Character.EWebDone += (t) => DoOnWebDone();
        Character.EWebCutted += () => DoOnWebCut();

        Physics.ESurfaceBecomeClose += OnSurfaceBecomeClose;
        Physics.ESurfaceBecomeFar += OnSurfaceBecomeFar;
    }

    private void Update()
    {
        DoOnUpdate();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    //==================================================================================================================================================================
    void OnSurfaceBecomeClose()
    {
        HideArrow();
        ShowJumpDirectionArrow();

        DoOnWebDone = () => { };
        DoOnWebCut = () => { };
    }

    void OnSurfaceBecomeFar()
    {
        HideArrow();
        ShowVelocityArrow();

        DoOnWebDone = HideArrow;
        DoOnWebCut = ShowVelocityArrow;
    }

    void HideArrow()
    {
        StopAllCoroutines();
        arrow.SetActive(false);
        DoOnUpdate = () => { };
    }

    void ShowJumpDirectionArrow()
    {
        _arrowSpriteRenderer.sprite = jumpDirectionArrow;
        StartCoroutine(EnableArrowAfterDelay());
        DoOnUpdate = OrientateAcrossJumpDirection;
    }

    void ShowVelocityArrow()
    {
        _arrowSpriteRenderer.sprite = VelocityArrow;
        StartCoroutine(EnableArrowAfterDelay());
        DoOnUpdate = OrientateAcrossVelocity;
    }

    //==================================================================================================================================================================
    void OrientateAcrossJumpDirection()
    {
        arrow.transform.rotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(Physics.SurfaceDirection, Vector2.down));
    }

    void OrientateAcrossVelocity()
    {
        arrow.transform.rotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(Physics.Collider.attachedRigidbody.velocity, Vector2.up));
    }

    //==================================================================================================================================================================
    IEnumerator EnableArrowAfterDelay()
    {
        yield return new WaitForSeconds(Sets.jumpDirectionArrowDelay);
        arrow.SetActive(true);
    }
}
