using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class DodgeControl : NetworkBehaviour
{
    [SerializeField] PlayerManager playerMgmt = null;

    public float delayBeforeInvulnerable = 0.2f;
    public float invulnerableDuration = 0.5f;

    public float dodgeCooldown = 1f;
    float cooldown;

    public float dodgeVelocity = 3f;

    [ClientCallback]
    private void FixedUpdate()
    {
        if(cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
        else if(cooldown < 0)
        {
            cooldown = 0f;
        }
    }

    // MAKE THIS MORE LIKE A DASH WITHOUT I-FRAMES
    public void Dodge(Vector3 dir)
    {
        // If the player is in the air then exit
        if (!playerMgmt.playerMovement.isGrounded) { return; }
        // If the cooldown isn't ready then exit
        if (cooldown > 0) { return; }

        // If the entity has enough stamina to dodge
        if ((playerMgmt.staminaMgmt.GetCurrentVital() - 10f) > 0)
        {
            playerMgmt.isInteracting = true;
            playerMgmt.staminaMgmt.TakeDamage(10f);

            // INVULNERABLE FUNCTION CALLED HERE
            playerMgmt.playerStats.Invulnerability();

            // normalizes the dir vector
            Vector3 _dir = new Vector3
            {
                x = dir.x,
                z = dir.y
            }.normalized;

            // Get entity's direction/rotation relative to the camera
            Vector3 rotationMovement = Quaternion.Euler(0, playerMgmt.myCamera.transform.rotation.eulerAngles.y, 0) * _dir;
            Vector3 verticalMovement = Vector3.up * playerMgmt.myRb.velocity.y;

            // Adds force relative to the camera in a direction
            playerMgmt.myRb.AddForce((verticalMovement + (rotationMovement * dodgeVelocity)), ForceMode.Impulse);

            // PLAY DODGE ANIMATION FROM AnimationManager
            playerMgmt.animMgmt.TriggerDodgeAnim(_dir);

            // Resets the cooldown timer
            cooldown = dodgeCooldown;
        }
    } 

    // DO A DODGE ROLL WITH I-FRAMES IF THE PLAYER DOUBLE TAPS DODGE
}
