using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZone2D : MonoBehaviour
{
    public Vector2 wind;

    //==================================================================================================================================================================
    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.attachedRigidbody.AddForce(wind * collision.attachedRigidbody.drag);
    }
}
