using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : NetworkBehaviour
{
    NavMeshAgent myNavAgent;
    public float lookRadius;
    public bool targetVisible;
    Transform targetPos;

    public List<GameObject> targetObjects = new List<GameObject>();
    [SerializeField] List<GameObject> visibleTargets = new List<GameObject>();

    private void Start()
    {
        myNavAgent = GetComponent<NavMeshAgent>();
    }

    public override void OnStartServer()
    {
        //if(!isServer) { return; }

        GameObject[] newObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject target in newObjects)
        {
            if(!targetObjects.Contains(target))
            {
                targetObjects.Add(target);
            }
        }
    }

    // Call when a new player joins???
    void InitializePlayer()
    {
        GameObject[] newObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject target in newObjects)
        {
            if (!targetObjects.Contains(target))
            {
                targetObjects.Add(target);
            }
        }
    }

    [ServerCallback]
    private void Update()
    {
        TargetInSight();

        if(visibleTargets != null)
        {
            myNavAgent.SetDestination(FindClosestTarget().transform.position);
        }    
    }

    void TargetInSight()
    {
        foreach (GameObject target in targetObjects)
        {
            float distToTarget = Vector3.Distance(this.transform.position, target.transform.position);
            if(distToTarget <= lookRadius)
            {
                if(!visibleTargets.Contains(target))
                {
                    visibleTargets.Add(target);
                }
            }
            else
            {
                if(visibleTargets.Contains(target))
                {
                    visibleTargets.Remove(target);
                }
            }
        }
    }

    GameObject FindClosestTarget()
    {
        float dist = Mathf.Infinity;
        GameObject closestTarget = null;

        foreach (GameObject go in visibleTargets)
        {
            float distToGo = Vector3.Distance(go.transform.position, this.transform.position);
            if(distToGo < dist)
            {
                dist = distToGo;
                closestTarget = go;
            }
        }

        return closestTarget;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
