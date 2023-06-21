using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WebAnchorInteractor : MonoBehaviour
{
    public abstract void Income(WebAnchor webAnchor);
    public abstract void Recome(WebAnchor webAnchor);
    public abstract void Outcome();
}
