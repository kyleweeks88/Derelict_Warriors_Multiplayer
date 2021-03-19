using System.Collections;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
	public GameObject hitFx_Pf;
	public LayerMask collisionsMask;
	public float projDmg = 10f;
    public float projSpeed = 10f;
	public float raycastLength = 1f;

	void Start()
    {
		StartCoroutine(DestroyAfterLifetime());
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider col)
    {
        ColHit(col);
    }

    [Server]
    void ColHit(Collider col)
    {
        NetworkIdentity hitId = col.GetComponent<NetworkIdentity>();
        if (hitId != null)
        {
            //NpcHealthManager hitTarget = col.GetComponent<NpcHealthManager>();
            VitalsManager hitTarget = col.GetComponent<VitalsManager>();
            if (hitTarget != null)
            {
                hitTarget.TakeDamage(hitTarget.health, projDmg);
            }
        }

        Vector3 hitPoint = col.ClosestPointOnBounds(this.transform.position);

        if (base.isClient)
        {
            GameObject hitFx = Instantiate(hitFx_Pf, hitPoint, Quaternion.identity);
        }
        
        RpcColHit(hitId, hitPoint);
        GameObject.Destroy(this.gameObject);
    }

    [ClientRpc]
    void RpcColHit(NetworkIdentity hitId, Vector3 hitPoint)
    {
        GameObject hitFx = Instantiate(hitFx_Pf, hitPoint, Quaternion.identity);
        GameObject.Destroy(this.gameObject);
    }

    public void SetSpeed(float newSpeed, Vector3 dir)
    {
    	projSpeed = newSpeed;
    	StartCoroutine(TranslateProjectile(newSpeed, dir));
    }

    IEnumerator TranslateProjectile(float speed, Vector3 dir)
    {
    	WaitForEndOfFrame wait = new WaitForEndOfFrame();
    	while(this.gameObject != null)
        {
    		this.transform.position += (dir * speed * Time.deltaTime);
    		yield return wait;
        }
    }

    IEnumerator DestroyAfterLifetime()
	{
		yield return new WaitForSeconds(2f);

        Destroy(gameObject);
	}
}
