using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resurrecter : MonoBehaviour
{
    new Rigidbody2D rigidbody;

    ValStorage valStorage;

    GameObject lastSpawner;

    //==================================================================================================================================================================
    private void Awake()
    {
        valStorage = GameObject.Find("Master").GetComponent<ValStorage>();

        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Spawner")
            lastSpawner = collision.gameObject;
    }

    //==================================================================================================================================================================
    public void Respawn()
    {
        StartCoroutine(RespawnStun());

        transform.position = new Vector3(lastSpawner.transform.position.x, lastSpawner.transform.position.y, transform.position.z);
    }

    //==================================================================================================================================================================
    IEnumerator RespawnStun()
    {
        rigidbody.bodyType = RigidbodyType2D.Static;

        yield return new WaitForSeconds(valStorage.respawnStunTime);

        rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
}
