using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeJumper : MonoBehaviour
{
    bool _jumpAwaits;

    new Collider2D collider;
    //==================================================================================================================================================================
    public float jumpForce = 10;
    public float jumpTimeWindow = 0.1f;

    //==================================================================================================================================================================
    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(_jumpAwaits)
        {
            StopCoroutine(JumpReady());

            Vector2 force = -(collision.contacts[0].point - (Vector2)transform.position).normalized * jumpForce;

            collider.attachedRigidbody.AddForce(force, ForceMode2D.Impulse);

            if(collision.collider.attachedRigidbody != null)
                collision.collider.attachedRigidbody.AddForce(-force, ForceMode2D.Impulse); // second Newton's law
        }

        _jumpAwaits = false;
    }

    //==================================================================================================================================================================
    public void Jump()
    {
        StopCoroutine(JumpReady());
        StartCoroutine(JumpReady());
    }

    //==================================================================================================================================================================
    IEnumerator JumpReady()
    {
        _jumpAwaits = true;

        yield return new WaitForSeconds(jumpTimeWindow);

        _jumpAwaits = false;
    }
}
