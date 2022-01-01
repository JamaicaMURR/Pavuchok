using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    Action<Collision2D> DoOnCollisionStay;

    public float force = 5;
    public float coolingTime = 0.1f;

    private void Awake()
    {
        DoOnCollisionStay = ActualReact;
        gameObject.tag = "Unstickable";
    }

    private void OnCollisionStay2D(Collision2D collision) => DoOnCollisionStay(collision);

    void ActualReact(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Vector2 impulse = -collision.gameObject.GetComponent<Sticker>().SurfaceDirection * force;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(impulse, ForceMode2D.Impulse);

            StartCoroutine(Cool());
        }
    }

    IEnumerator Cool()
    {
        DoOnCollisionStay = (c) => { };

        yield return new WaitForSeconds(coolingTime);

        DoOnCollisionStay = ActualReact;
    }
}
