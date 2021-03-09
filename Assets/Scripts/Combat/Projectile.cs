using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
	public GameObject hitFxPrefab;
	public LayerMask collisionsMask;
	public float projectileDamage = 10f;
    public float speed = 10f;
	public float skinWidth = 0.1f;

	void Start()
    {
        SetSpeed(speed);
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

	
	void CheckCollisions()
	{
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, skinWidth, collisionsMask, QueryTriggerInteraction.Collide))
		{
			OnHitObject(hit);
		}

		//CmdCheckCollisions();
	}

	//[Command]
	//void CmdCheckCollisions()
 //   {
 //       if (!base.isClient) { return; }

	//	Ray ray = new Ray(transform.position, transform.forward);
	//	RaycastHit hit;

	//	if (Physics.Raycast(ray, out hit, skinWidth, collisionsMask, QueryTriggerInteraction.Collide))
	//	{
	//		OnHitObject(hit);
	//	}

	//	RpcCheckCollisions();
	//}

	//[ClientRpc]
	//void RpcCheckCollisions()
 //   {
	//	Ray ray = new Ray(transform.position, transform.forward);
	//	RaycastHit hit;

	//	if (Physics.Raycast(ray, out hit, skinWidth, collisionsMask, QueryTriggerInteraction.Collide))
	//	{
	//		OnHitObject(hit);
	//	}
	//}
	
	void OnHitObject(RaycastHit hit)
	{
  //      HealthManager hitTarget = hit.collider.GetComponent<HealthManager>();
  //      if (hitTarget != null)
  //      {
  //          hitTarget.TakeDamage(projectileDamage);
  //      }

		//GameObject hitFx = Instantiate(hitFxPrefab, hit.point, Quaternion.identity);
		//GameObject.Destroy(this.gameObject);

		NetworkIdentity hitId = hit.collider.GetComponent<NetworkIdentity>();
		CmdOnHitObject(hitId, hit.point);
	}

	[Command]
	void CmdOnHitObject(NetworkIdentity hitId, Vector3 hitPoint)
    {
		if (hitId != null)
		{
			HealthManager hitTarget = hitId.gameObject.GetComponent<HealthManager>();
			if (hitTarget != null)
			{
				hitTarget.TakeDamage(projectileDamage);
			}
		}

		if (base.isClient)
		{
			GameObject hitFx = Instantiate(hitFxPrefab, hitPoint, Quaternion.identity);
		}

		GameObject.Destroy(this.gameObject);
		RpcOnHitObject(hitId, hitPoint);
	}

    [ClientRpc]
    void RpcOnHitObject(NetworkIdentity hitId, Vector3 hitPoint)
    {
        if (hitId != null)
        {
            HealthManager hitTarget = hitId.gameObject.GetComponent<HealthManager>();
            if (hitTarget != null)
            {
                hitTarget.TakeDamage(projectileDamage);
            }
        }

        GameObject hitFx = Instantiate(hitFxPrefab, hitPoint, Quaternion.identity);
        GameObject.Destroy(this.gameObject);
    }

    IEnumerator DestroyAfterLifetime()
	{
		yield return new WaitForSeconds(2f);

        Destroy(gameObject);
	}
}
