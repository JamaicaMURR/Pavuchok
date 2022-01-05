using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValStorage : MonoBehaviour
{
    [Header("Sticking Settings")]
    public float initialStickingForce = 100;
    public float usualStickingForce = 17.5f;
    public float returnStickingForce = 50;
    public float stickingAngleThreshold = 45;
    public float unstickableDelay = 0.05f;

    [Header("Jumping Settings")]
    public float jumpForceInitial = 10;
    public float jumpForcePeak = 15;
    public float jumpChargeTime = 0.25f;
    public float jumpTimeWindow = 0.1f;
    public float velocityArrowDelay = 0.1f;

    public int chargingSteps = 1;

    [Header("Rolling Settings")]
    public float standStillVelocityThreshold = 0.01f;
    public float initialRollingSpeed = 50;
    public float maximalRollingSpeed = 350;
    public float accelerationTime = 1;
    public float rotationTorque = 1000;
    public float brakesTorque = 5;
    public float magnusEffectMultiplier = 0.001f;

    [Header("Web Settings")]
    public float webPullSpeed = 1;
    public float webReleaseSpeed = 1;

    public int maximumKnots = 40;

    public float maximalStrikeDistance = 10;
    public float minimalWebLength = 1;
    public float reactionImpulsePerShotedKnot = 0.1f;
    public float webRestoringDelay = 0.5f;

    public float strikeDistanceAppearingDelay = 0.25f;
    public float targetLineAppearingDelay = 0.25f;

    [Header("Respawn settings")]
    public float respawnStunTime = 0.1f;
}
