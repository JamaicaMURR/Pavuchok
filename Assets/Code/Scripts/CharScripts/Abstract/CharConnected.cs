using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharConnected : MonoBehaviour
{
    Character _character;

    protected Character Character
    {
        get
        {
            if(_character is null)
                throw new NullReferenceException("Character is not connected");
            else
                return _character;
        }
    }

    protected CharacterSettings Sets => Character.settings;
    protected PhysicsSupport Physics => Character.physicsSupport;
    protected Vector2 CharacterPosition => _character.transform.position;

    public void ConnectToCharacter()
    {
        if(_character is null)
        {
            _character = gameObject.GetComponent<Character>();

            if(_character is null)
                _character = gameObject.GetComponentInParent<Character>();

            if(_character is null)
                throw new NullReferenceException("Character not found");
        }
        else
            throw new Exception("Character is already connected");
    }
}
