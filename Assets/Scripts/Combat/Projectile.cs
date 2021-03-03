using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
	public LayerMask collisionsMask;
	public float projectileDamage = 10f;
    public float speed = 10f;
	public float skinWidth = 0.1f;

    private void Awake()
    {
		StartCoroutine(DestroyAfterLifetime());
    }

	public void SetSpeed(float newSpeed)
	{
		speed = newSpeed;
		StartCoroutine(TranslateProjectile(newSpeed));
	}

	IEnumerator TranslateProjectile(float speed)
    {
		Vector3 direction = transform.forward;

		WaitForEndOfFrame wait = new WaitForEndOfFrame();
		while(this.gameObject != null)
        {
			this.transform.position += (direction * speed * Time.deltaTime);
			CheckCollisions();
			yield return wait;
        }
    }

	[Client]
	void CheckCollisions()
	{
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, skinWidth, collisionsMask, QueryTriggerInteraction.Collide))
		{
			// Instantiate HitFX here
			OnHitObject(hit);
		}

		CmdCheckCollisions();
	}

	[Command]
	void CmdCheckCollisions()
    {
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, skinWidth, collisionsMask, QueryTriggerInteraction.Collide))
		{
			// Instantiate HitFX here
			//OnHitObject(hit);
		}

		RpcCheckCollisions();
	}

	[ClientRpc]
	void RpcCheckCollisions()
    {
		// Your client has already done this code locally
        if (base.hasAuthority) { return; }

		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, skinWidth, collisionsMask, QueryTriggerInteraction.Collide))
		{
			// Instantiate HitFX here
			//OnHitObject(hit);
		}
	}

	void OnHitObject(RaycastHit hit)
	{
        HealthManager hitTarget = hit.collider.GetComponent<HealthManager>();
        if (hitTarget != null)
        {
            hitTarget.TakeDamage(projectileDamage);
        }

		// SHOULD I CALL A COMMAND FUNCTION HERE???

		Debug.Log("HIT");
		GameObject.Destroy(this.gameObject);
	}

	IEnumerator DestroyAfterLifetime()
	{
		yield return new WaitForSeconds(2f);

        Destroy(gameObject);
	}
}
