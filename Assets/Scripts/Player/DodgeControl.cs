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
        if(cooldown > 0) { return; }

        // INVULNERABLE FUNCTION CALLED HERE

        Vector3 _dir = new Vector3
        {
            x = dir.x,
            z = dir.y
        }.normalized;

        Vector3 rotationMovement = Quaternion.Euler(0, playerMgmt.myCamera.transform.rotation.eulerAngles.y, 0) * _dir;
        Vector3 verticalMovement = Vector3.up * playerMgmt.myRb.velocity.y;

        playerMgmt.myRb.AddForce((verticalMovement + (rotationMovement * dodgeVelocity)), ForceMode.Impulse);

        // PLAY DODGE ANIMATION FROM AnimationManager
        playerMgmt.animMgmt.TriggerDodgeAnim(_dir);

        cooldown = dodgeCooldown;
    } 

    // DO A DODGE ROLL WITH I-FRAMES IF THE PLAYER DOUBLE TAPS DODGE
}
