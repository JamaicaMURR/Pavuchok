using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebChute : MonoBehaviour
{
    CircleCollider2D _collider;
    Rigidbody2D _body;
    SpriteRenderer _spriteRenderer;
    Animator _animator;

    Transform _paratrooper;

    Action DoOnUpdate;
    Action DoOnFixedUpdate;

    float _initialColliderRadius;

    public Rigidbody2D Body => _body;
    public Vector2 Position => transform.position;
    public Collider2D Collider => _collider;

    //==================================================================================================================================================================
    public ChuteSettings chuteSettings;

    //==================================================================================================================================================================
    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
        _body = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _initialColliderRadius = _collider.radius;

        DoOnUpdate = () => { };
        DoOnFixedUpdate = () => { };
    }

    private void Update()
    {
        DoOnUpdate();
    }

    private void FixedUpdate()
    {
        DoOnFixedUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Activate(_paratrooper);
    }

    //==================================================================================================================================================================
    public void Activate(Transform paratrooper)
    {
        StopAllCoroutines();

        DoOnUpdate = () => { };
        DoOnFixedUpdate = () => { };

        _paratrooper = paratrooper;
        _spriteRenderer.sprite = null;

        StartCoroutine(DeployAfterDelay(chuteSettings.deployingDelay));
    }

    //==================================================================================================================================================================
    public void StartDeploying() // Must be called from deploying animation
    {
        DoOnFixedUpdate = ChangePhysicsAcrossAnimation;
        DoOnUpdate = OrientateAcrossVelocity;
    }

    public void EndDeploying() // Must be called from deploying animation
    {
        DoOnFixedUpdate();
        DoOnFixedUpdate = () => { };
    }

    // DoOnUpdate options
    //==================================================================================================================================================================
    void MonitorDeploying()
    {
        if(_body.velocity.magnitude >= chuteSettings.deployingSpeed)
        {
            _animator.SetTrigger("DeployChute");
            DoOnUpdate = () => { };
        }
    }

    void OrientateAcrossVelocity()
    {
        transform.rotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(Position - (Vector2)_paratrooper.position, Vector2.up));
    }

    // DoOnFixedUpdate options
    //==================================================================================================================================================================
    void ChangePhysicsAcrossAnimation()
    {
        _collider.radius = _spriteRenderer.sprite.rect.width / 2 * chuteSettings.colliderRelativeConstant;
        _body.drag = chuteSettings.relativeDrag * _collider.radius * transform.lossyScale.x;
    }

    //==================================================================================================================================================================
    IEnumerator DeployAfterDelay(float delay)
    {
        _collider.radius = _initialColliderRadius;
        _body.drag = chuteSettings.preparedDrag;
        _animator.SetTrigger("ChuteReady");

        yield return new WaitForSeconds(delay);

        DoOnUpdate = MonitorDeploying;
    }
}
