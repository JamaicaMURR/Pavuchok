using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebAnchor : MonoBehaviour
{
    CircleCollider2D _collider;
    Rigidbody2D _body;

    Transform _attachedTransform;

    public event Action EAnchorCut;

    public Rigidbody2D Body => _body;
    public Vector2 Position => transform.position;
    public Collider2D Collider => _collider;

    //==================================================================================================================================================================
    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
        _body = GetComponent<Rigidbody2D>();
    }

    //==================================================================================================================================================================
    public void AnchorCut() => EAnchorCut?.Invoke();

    //==================================================================================================================================================================
    public void Attach(Transform objTransform)
    {
        WebAnchorInteractor[] interactors = objTransform.GetComponents<WebAnchorInteractor>();

        if(objTransform != _attachedTransform)
        {
            Detach();
            _attachedTransform = objTransform;

            foreach(WebAnchorInteractor wai in interactors)
                wai.Income(this);
        }
        else
        {
            foreach(WebAnchorInteractor wai in interactors)
                wai.Recome(this);
        }
    }

    public void Detach()
    {
        WebAnchorInteractor[] interactors = _attachedTransform?.GetComponents<WebAnchorInteractor>() ?? new WebAnchorInteractor[0];

        foreach(WebAnchorInteractor wai in interactors)
            wai.Outcome();
    }
}
