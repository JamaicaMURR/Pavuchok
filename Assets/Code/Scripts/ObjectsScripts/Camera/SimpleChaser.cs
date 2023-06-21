using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleChaser : MonoBehaviour
{
    public Transform target;

    private void LateUpdate()
    {
        transform.Translate((Vector2)(target.position - transform.position));
    }
}
