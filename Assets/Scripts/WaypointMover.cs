using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JPO
{
    public class WaypointMover : MonoBehaviour
    {
        public Transform waypointParent;
        public float moveSpeed = 2f;
        public float waitTime = 2f;
        public bool loopWaypoints = true;

        private Transform[] waypoints;
        private int currentWaypointIndex;
        private bool isWaiting;
        void Start()
        {
            waypoints = new Transform[waypointParent.childCount];
            for(int i = 0; i < waypointParent.childCount; i++)
            {
                waypoints[i] = waypointParent.GetChild(i);
            }
        }

      
        void Update()
        {
        
            MoveToWaypoint();
        }

        void MoveToWaypoint()
        {
            Transform target = waypoints[currentWaypointIndex];

            

            if (Vector2.Distance(transform.position, target.position) < 0.1f)
            {
                if (!isWaiting)
                {
                    transform.position = target.position;
                    StartCoroutine(WaitAtWaypoint());
                }

            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            }

        }
        IEnumerator WaitAtWaypoint()
        {
            isWaiting = true;
            yield return new WaitForSeconds(waitTime);

            currentWaypointIndex = loopWaypoints ? (currentWaypointIndex + 1) % waypoints.Length : Mathf.Min(currentWaypointIndex + 1, waypoints.Length - 1);

            isWaiting = false;
        }
    } 
      
}
