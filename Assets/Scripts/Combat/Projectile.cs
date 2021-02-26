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

	IEnumerator SpawnProjectiles(float speed)
    {
		Vector3 direction = transform.forward;

		WaitForEndOfFrame wait = new WaitForEndOfFrame();
		while(this.gameObject != null)
        {
			this.transform.position += (direction * speed * Time.deltaTime);
			CheckCollisions(0);
			yield return wait;
        }
    }

	public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
		StartCoroutine(SpawnProjectiles(newSpeed));
    }

	void CheckCollisions(float moveDistance)
	{
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionsMask, QueryTriggerInteraction.Collide))
		{
			OnHitObject(hit);
			Debug.Log("HIT");
		}
	}

	void OnHitObject(RaycastHit hit)
	{
		CharacterStats hitTarget = hit.collider.GetComponent<CharacterStats>();
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
