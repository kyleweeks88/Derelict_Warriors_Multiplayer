using System.Collections;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
	public GameObject hitFx_Pf;
	public LayerMask collisionsMask;
	public float projDmg = 10f;
    public float projSpeed = 10f;
	public float raycastLength = 0.1f;

	void Start()
    {
        SetSpeed(projSpeed);
		StartCoroutine(DestroyAfterLifetime());
    }

    public void SetSpeed(float newSpeed)
    {
    	projSpeed = newSpeed;
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
	
    [Server]
	void CheckCollisions()
	{
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, raycastLength, collisionsMask, QueryTriggerInteraction.Collide))
		{
			OnHitObject(hit);
		}
	}
	
    [Server]
	void OnHitObject(RaycastHit hit)
	{
        NetworkIdentity hitId = hit.collider.GetComponent<NetworkIdentity>();
        if(hitId != null)
        {
            HealthManager hitTarget = hit.collider.GetComponent<HealthManager>();
            if (hitTarget != null)
            {
                hitTarget.TakeDamage(projDmg);
            }
        }

        RpcOnHitObject(hitId, hit.point);

        if (base.isClient)
        {
            GameObject hitFx = Instantiate(hitFx_Pf, hit.point, Quaternion.identity);
            GameObject.Destroy(this.gameObject);
        }

        GameObject.Destroy(this.gameObject);
    }

    [ClientRpc]
    void RpcOnHitObject(NetworkIdentity hitId, Vector3 hitPoint)
    {
        if (hitId != null)
        {
            HealthManager hitTarget = hitId.gameObject.GetComponent<HealthManager>();
            if (hitTarget != null)
            {
                hitTarget.TakeDamage(projDmg);
            }
        }

        GameObject hitFx = Instantiate(hitFx_Pf, hitPoint, Quaternion.identity);
        GameObject.Destroy(this.gameObject);
    }

    IEnumerator DestroyAfterLifetime()
	{
		yield return new WaitForSeconds(2f);

        Destroy(gameObject);
	}
}
