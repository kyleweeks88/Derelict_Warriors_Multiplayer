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

    public void Dodge(Vector3 dir)
    {
        if(cooldown > 0) { return; }
        // INVULNERABLE FUNCTION CALLED HERE

        // PLAY DODGE ANIMATION FROM AnimationManager
        playerMgmt.animMgmt.TriggerDodgeAnim();

        Vector3 _dir = new Vector3
        {
            x = dir.x,
            z = dir.y
        }.normalized;

        playerMgmt.myRb.AddForce(_dir * dodgeVelocity, ForceMode.Impulse);

        cooldown = dodgeCooldown;
        Debug.Log("TEST");
    } 
}
