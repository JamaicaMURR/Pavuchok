using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Knot of all character scripts
public class Character : MonoBehaviour
{
    public CharacterSettings settings;
    public PhysicsSupport physicsSupport;

    bool _rollAbility;
    bool _jumpAbility;
    bool _airJumpAbility;
    bool _stickAbility;
    bool _webAbility;
    bool _changeWebLengthAbility;
    bool _chuteAbility;

    //==================================================================================================================================================================
    public event Action ERollAbilityEnable;
    public event Action ERollAbilityDisable;

    public bool RollAbility
    {
        get => _rollAbility;

        set
        {
            if(_rollAbility)
            {
                if(!value)
                {
                    ERollAbilityDisable?.Invoke();
                    _rollAbility = false;
                }
            }
            else
            {
                if(value)
                {
                    ERollAbilityEnable?.Invoke();
                    _rollAbility = true;
                }
            }
        }
    }

    // Rolling commands events
    public event Action EStartRollLeft;
    public event Action EStartRollRight;
    public event Action EStop;

    // Rolling commands
    public void RollLeftCommand() => EStartRollLeft?.Invoke();
    public void RollRightCommand() => EStartRollRight?.Invoke();
    public void StopCommand() => EStop?.Invoke();

    // Rolling states events
    public event Action ERollLeftStarted;
    public event Action ERollRightStarted;
    public event Action ELeftBreakingStarted;
    public event Action ERightBreakingStarted;

    // Rolling states
    public void RollLeftStarted() => ERollLeftStarted?.Invoke();
    public void RollRightStarted() => ERollRightStarted?.Invoke();
    public void LeftBreakingStarted() => ELeftBreakingStarted?.Invoke();
    public void RightBreakingStarted() => ERightBreakingStarted?.Invoke();

    //==================================================================================================================================================================
    public event Action EJumpAbilityEnable;
    public event Action EJumpAbilityDisable;

    public bool JumpAbility
    {
        get => _jumpAbility;

        set
        {
            if(_jumpAbility)
            {
                if(!value)
                {
                    EJumpAbilityDisable?.Invoke();
                    _jumpAbility = false;
                }
            }
            else
            {
                if(value)
                {
                    EJumpAbilityEnable?.Invoke();
                    _jumpAbility = true;
                }
            }
        }
    }

    // Jumping commands events
    public event Action EPrepareJump;
    public event Action EReleaseJump;

    // Jumping commands
    public void PrepareJump() => EPrepareJump();
    public void ReleaseJump() => EReleaseJump();

    //==================================================================================================================================================================
    public event Action EAirJumpAbiltyEnable;
    public event Action EAirJumpAbilityDisable;

    public bool AirJumpAbility
    {
        get => _airJumpAbility;

        set
        {
            if(JumpAbility)
            {
                if(_airJumpAbility)
                {
                    if(!value)
                    {
                        EAirJumpAbilityDisable?.Invoke();
                        _airJumpAbility = false;
                    }
                }
                else
                {
                    if(value)
                    {
                        EAirJumpAbiltyEnable?.Invoke();
                        _airJumpAbility = true;
                    }
                }
            }
        }
    }

    public event Action EAirJumpReady;
    public event Action EAirJumpDone;

    public void AirJumpReady() => EAirJumpReady?.Invoke();
    public void AirJumpDone() => EAirJumpDone?.Invoke();

    //==================================================================================================================================================================
    public Action EStickAbilityEnable;
    public Action EStickAbilityDisable;

    public bool StickAbility
    {
        get => _stickAbility;

        set
        {
            if(_stickAbility)
            {
                if(!value)
                {
                    EStickAbilityDisable?.Invoke();
                    _stickAbility = false;
                }
            }
            else
            {
                if(value)
                {
                    EStickAbilityEnable?.Invoke();
                    _stickAbility = true;
                }
            }
        }
    }

    // Sticking commands events
    public Action EStick;
    public Action EUnstick;

    // Stick commands
    public void StickCommand() => EStick?.Invoke();
    public void UnstickCommand() => EUnstick?.Invoke();

    // Stick states events
    public Action EStickingActivated;
    public Action EStickingDeactivated;

    // Stick states
    public void StickingActivated() => EStickingActivated?.Invoke();
    public void StickingDeactivated() => EStickingDeactivated?.Invoke();

    //==================================================================================================================================================================
    // WebAbility
    public event Action EWebAbilityEnable;
    public event Action EWebAbilityDisable;

    public bool WebAbility
    {
        get => _webAbility;

        set
        {
            if(_webAbility)
            {
                if(!value)
                {
                    EWebAbilityDisable?.Invoke();
                    _webAbility = false;
                }
            }
            else
            {
                if(value)
                {
                    EWebAbilityEnable?.Invoke();
                    _webAbility = true;
                }
            }
        }
    }

    public event Action EProduceWeb;
    public event Action ECutWeb;

    public void ProduceWebCommand() => EProduceWeb?.Invoke();
    public void CutWebCommand() => ECutWeb?.Invoke();

    public event Action<Transform> EWebDone;
    public event Action EWebCutted;

    public void WebDone(Transform transform) => EWebDone?.Invoke(transform);
    public void WebCutted() => EWebCutted?.Invoke();

    // ChangeWebLengthAbility
    public event Action EChangeWebLengthAbilityEnable;
    public event Action EChangeWebLengthAbilityDisable;

    public bool ChangeWebLengthAbility
    {
        get => _changeWebLengthAbility;

        set
        {
            if(_changeWebLengthAbility)
            {
                if(!value)
                {
                    EChangeWebLengthAbilityDisable?.Invoke();
                    _changeWebLengthAbility = false;
                }
            }
            else
            {
                if(value)
                {
                    EChangeWebLengthAbilityEnable?.Invoke();
                    _changeWebLengthAbility = true;
                }
            }
        }
    }

    public event Action<float> EChangeWebLength;

    public void ChangeWebLengthCommand(float delta) => EChangeWebLength?.Invoke(delta);

    // ChuteAbility
    public event Action EChuteAbilityEnable;
    public event Action EChuteAbilityDisable;

    public bool ChuteAbility
    {
        get => _chuteAbility;

        set
        {
            if(_chuteAbility)
            {
                if(!value)
                {
                    EChuteAbilityDisable?.Invoke();
                    _chuteAbility = false;
                }
            }
            else
            {
                if(value)
                {
                    EChuteAbilityEnable?.Invoke();
                    _chuteAbility = true;
                }
            }
        }
    }
}
