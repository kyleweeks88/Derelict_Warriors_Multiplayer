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

	//   [ServerCallback]
	//void Update()
	//{
	//	if (speed > 0)
	//	{
	//		float moveDistance = speed * Time.deltaTime;
	//		CheckCollisions(moveDistance);

	//		transform.Translate(Vector3.forward * moveDistance);
	//	}
	//}

	public void SetSpeed(float newSpeed)
	{
		speed = newSpeed;
		StartCoroutine(SpawnProjectiles(newSpeed));
	}

	IEnumerator SpawnProjectiles(float speed)
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
			OnHitObject(hit);
		}

		RpcCheckCollisions();
	}

	[ClientRpc]
	void RpcCheckCollisions()
    {
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, skinWidth, collisionsMask, QueryTriggerInteraction.Collide))
		{
			// Instantiate HitFX here
			OnHitObject(hit);
		}
	}

	void OnHitObject(RaycastHit hit)
	{
		CharacterStats hitTarget = hit.collider.GetComponent<CharacterStats>();
		if (hitTarget != null)
		{
			hitTarget.TakeDamage(projectileDamage);
		}

		NetworkIdentity _hit = hit.collider.gameObject.GetComponent<NetworkIdentity>();
		CmdOnHitObject(_hit);
		GameObject.Destroy(gameObject);
		Debug.Log("HIT");
	}

	[Command]
	void CmdOnHitObject(NetworkIdentity hit)
    {
		CharacterStats hitTarget = hit.gameObject.GetComponent<CharacterStats>();
		if (hitTarget != null)
		{
			hitTarget.TakeDamage(projectileDamage);
		}

		RpcOnHitObject(hit);
		GameObject.Destroy(gameObject);
	}

	[ClientRpc]
	void RpcOnHitObject(NetworkIdentity hit)
    {
        if (base.hasAuthority) { return; }

		CharacterStats hitTarget = hit.gameObject.GetComponent<CharacterStats>();
		if (hitTarget != null)
		{
			hitTarget.TakeDamage(projectileDamage);
		}

		GameObject.Destroy(gameObject);
	}

	IEnumerator DestroyAfterLifetime()
	{
		yield return new WaitForSeconds(2f);

        Destroy(gameObject);
	}
}
