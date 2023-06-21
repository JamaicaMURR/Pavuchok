using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Setiings", menuName = "ScriptableObjects/CaracterSettings")]
public class CharacterSettings : ScriptableObject
{
    [Header("Rolling Settings")]
    public float standStillVelocityThreshold = 0.01f;
    public float initialRollingSpeed = 50;
    public float maximalRollingSpeed = 350;
    public float accelerationTime = 1;
    public float rotationTorque = 1000;
    public float brakesTorque = 5;

    [Header("Jumping Settings")]
    public float jumpForce = 12.5f;

    [Header("Air Jumping Settings")]
    public float airJumpForce = 6.25f;

    [Header("Sticking Settings")]
    public float initialStickingForce = 100;
    public float usualStickingForce = 17.5f;
    public float returnStickingForce = 50;
    public float stickingAngleThreshold = 45;

    [Header("Web Settings")]
    public float maximalStrikeDistance = 10;
    public float minimalWebLength = 1;
    public float maximalWebLength = 10;
    public float lengthChangingSpeed = 1;

    [Header("Respawn settings")]
    public float respawnStunTime = 0.1f;

    [Header("Animation settings")]
    public float jumpDirectionArrowDelay = 0.1f;
    public float velocityArrowDelay = 0.1f;
    public float strikeDistanceAppearingDelay = 0.25f;
    public float targetLineAppearingDelay = 0.25f;

    [Header("Control settings")]
    public float flightControlForce = 2;
    public float pendulumForce = 0.5f;

    [Header("Tags")]
    public string unstickableTag = "Unstickable";
}
