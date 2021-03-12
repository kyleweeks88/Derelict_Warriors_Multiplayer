using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AnimationManager : NetworkBehaviour
{
    public Animator myAnim;
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
        myAnim.SetBool(isGroundedParam, playerMgmt.charCtrl.isGrounded);
        myAnim.SetFloat(yVelocityParam, playerMgmt.playerMovement.yVelocity);

        myAnim.SetBool(inCombatParam, playerMgmt.combatMgmt.inCombat);

        if (playerMgmt.combatMgmt.attackAnim != null)
            myAnim.SetBool(playerMgmt.combatMgmt.attackAnim, playerMgmt.inputMgmt.attackInputHeld);
    }

    public void MovementAnimation(float xMove, float zMove)
    {
        myAnim.SetFloat(playerMgmt.animMgmt.inputXParam, xMove);
        myAnim.SetFloat(playerMgmt.animMgmt.inputYParam, zMove);
    }
}
