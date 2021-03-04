﻿using UnityEngine;

namespace Mirror.Examples.Benchmark
{
    public class MonsterMovement : NetworkBehaviour
    {
        public float speed = 1;
        public float movementProbability = 0.5f;
        public float movementDistance = 20;
        public float travelRange = 1f;

        bool moving;
        Vector3 start;
        Vector3 destination;

        public override void OnStartServer()
        {
            start = transform.position;
        }

        [ServerCallback]
        void Update()
        {
            if (moving)
            {
                if (Vector3.Distance(transform.position, destination) <= 0.01f)
                {
                    moving = false;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                }
            }
            else
            {
                float r = Random.value;
                if (r < movementProbability * Time.deltaTime)
                {
                    Vector2 circlePos = Random.insideUnitCircle * travelRange;

                    // Raycast down to check for ground layer
                    // if ray hits obstacle then create a new random circlePos
                    // until ray hits the ground layer.

                    Vector3 dir = new Vector3(circlePos.x, 0, circlePos.y);
                    Vector3 dest = transform.position + dir * movementDistance;

                    // within move dist around start?
                    // (don't want to wander off)
                    if (Vector3.Distance(start, dest) <= movementDistance)
                    {
                        destination = dest;
                        moving = true;
                    }
                }
            }
        }
    }
}
