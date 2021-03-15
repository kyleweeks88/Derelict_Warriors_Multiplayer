﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AnimationManager : NetworkBehaviour
{
    public Animator myAnim;
    public NetworkAnimator netAnim;
    [SerializeField] PlayerManager playerMgmt;

    #region Animator Parameters
    // My Animator parameters turned from costly Strings to cheap Ints
    [HideInInspector] public int isSprintingParam = Animator.StringToHash("isSprinting");
    int isJumpingParam = Animator.StringToHash("isJumping");
    int isGroundedParam = Animator.StringToHash("isGrounded");
    int yVelocityParam = Animator.StringToHash("yVelocity");
    [HideInInspector] public int inputXParam = Animator.StringToHash("InputX");
    [HideInInspector] public int inputYParam = Animator.StringToHash("InputY");
    int inCombatParam = Animator.StringToHash("inCombat");
    int isInteractingParam = Animator.StringToHash("isInteracting");
    #endregion

    public override void OnStartAuthority()
    {
        enabled = true;
        playerMgmt = GetComponent<PlayerManager>();
    }

    public void SetAnimation(AnimatorOverrideController overrideCtrl)
    {
        myAnim.runtimeAnimatorController = overrideCtrl;
    }

    [ClientCallback]
    void Update()
    {
        myAnim.SetBool(isSprintingParam, playerMgmt.playerMovement.isSprinting);
        myAnim.SetBool(isInteractingParam, playerMgmt.isInteracting);
        myAnim.SetBool(isJumpingParam, playerMgmt.playerMovement.isJumping);
        myAnim.SetBool(isGroundedParam, playerMgmt.playerMovement.isGrounded);
        myAnim.SetFloat(yVelocityParam, playerMgmt.playerMovement.yVelocity);

        myAnim.SetBool(inCombatParam, playerMgmt.combatMgmt.inCombat);

        if (playerMgmt.inputMgmt.attackInputHeld)
        {
            if (playerMgmt.equipmentMgmt.currentlyEquippedWeapon != null &&
                !playerMgmt.equipmentMgmt.currentlyEquippedWeapon.weaponData.isChargeable)
            {
                MeleeWeapon myWeapon = playerMgmt.equipmentMgmt.currentlyEquippedWeapon as MeleeWeapon;
                if((playerMgmt.staminaMgmt.GetCurrentVital() - myWeapon.meleeData.staminaCost) > 0)
                    netAnim.SetTrigger(playerMgmt.combatMgmt.attackAnim);
            }
        }
    }

    public void MovementAnimation(float xMove, float zMove)
    {
        myAnim.SetFloat(inputXParam, xMove);
        myAnim.SetFloat(inputYParam, zMove);
    }

    public void HandleMeleeAttackAnimation(bool boolVal)
    {
        myAnim.SetBool(playerMgmt.combatMgmt.attackAnim, boolVal);
    }

    public void HandleRangedAttackAnimation(bool boolVal)
    {
        myAnim.SetBool(playerMgmt.combatMgmt.attackAnim, boolVal);
    }

    public void TriggerDodgeAnim(Vector3 dir)
    {
        if (dir.z > 0.1)
        {
            netAnim.SetTrigger("isDodgingForwards");
        }
        else if(dir.z < -0.1)
        {
            netAnim.SetTrigger("isDodgingBackwards");
        }
    }
}
