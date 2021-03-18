using System.Collections;
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
        myAnim.SetBool(isJumpingParam, playerMgmt.playerMovement.isJumping);
        myAnim.SetBool(isGroundedParam, playerMgmt.playerMovement.isGrounded);
        myAnim.SetFloat(yVelocityParam, playerMgmt.myRb.velocity.y);

        myAnim.SetBool(inCombatParam, playerMgmt.combatMgmt.inCombat);

        if (playerMgmt.inputMgmt.attackInputHeld)
        {
            if (playerMgmt.equipmentMgmt.currentlyEquippedWeapon != null &&
                !playerMgmt.equipmentMgmt.currentlyEquippedWeapon.weaponData.isChargeable)
            {
                MeleeWeapon myWeapon = playerMgmt.equipmentMgmt.currentlyEquippedWeapon as MeleeWeapon;
                if((playerMgmt.vitalsMgmt.stamina.GetCurrentValue() - myWeapon.meleeData.staminaCost) > 0)
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
        if (!base.hasAuthority) { return; }
        if (dir.z > 0.1f && dir.x > 0.5f)
        {
            // FORWARD RIGHT
            netAnim.SetTrigger("dodge_FR");
        }
        else if (dir.z > 0.1f && dir.x < -0.5f)
        {
            // FORWARD LEFT
            netAnim.SetTrigger("dodge_FL");
        }
        else if (dir.z < -0.1f && dir.x > 0.5f)
        {
            // BACKWARDS RIGHT
            netAnim.SetTrigger("dodge_BR");
        }
        else if (dir.z < -0.1f && dir.x < -0.5f)
        {
            // BACKWARDS LEFT
            netAnim.SetTrigger("dodge_BL");
        }
        else if(dir.z > -0.1f && dir.z < 0.1f && dir.x > 0.1f)
        {
            // RIGHT
            netAnim.SetTrigger("dodge_FR");
        }
        else if (dir.z > -0.1f && dir.z < 0.1f && dir.x < -0.1f)
        {
            // LEFT
            netAnim.SetTrigger("dodge_FL");
        }
        else if(dir.z > 0.1f && dir.x < 0.1f && dir.x > -0.1f)
        {
            // FORWARDS
            netAnim.SetTrigger("dodge_F");
        }
        else if(dir.z < -0.1f && dir.x < 0.1f && dir.x > -0.1f)
        {
            // BACKWARDS
            netAnim.SetTrigger("dodge_B");
        }
    }
}
