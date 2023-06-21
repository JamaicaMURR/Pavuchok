using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurrAnimatorServer : CharConnected
{
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if(_animator is null)
            throw new NullReferenceException("No animator");

        ConnectToCharacter();

        Character.ERollLeftStarted += () => _animator.SetTrigger("RollLeft");
        Character.ERollRightStarted += () => _animator.SetTrigger("RollRight");
        Character.ELeftBreakingStarted += () => _animator.SetTrigger("LeftBreaking");
        Character.ERightBreakingStarted += () => _animator.SetTrigger("RightBreaking");
    }
}
