using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Setiings", menuName ="ScriptableObjects/ChuteSettings")]
public class ChuteSettings : ScriptableObject
{
    public float deployingSpeed = 1;
    public float preparedDrag = 5;
    public float relativeDrag = 50;
    public float colliderRelativeConstant = .01f;
    public float deployingDelay = .25f;
}
