using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TL.Core
{
    public class MoveController : MonoBehaviour
    {
        private NavMeshAgent agent;
        public Transform destination;

        
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

       
        void Update()
        {
            
        }

        public void MoveTo(Vector3 position)
        {
            agent.destination = position;
        }
    }
}
