using UnityEngine;
using UnityEngine.Networking;

namespace Traps.TeddyBear
{
    public class TeddyBearCollider : NetworkBehaviour
    {
        void Start()
        {
            if (!isServer) enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("EnemyLegs") || other.CompareTag("EnemyHead") || other.CompareTag("EnemyBody"))
                transform.parent.GetComponent<TeddyBearServer>().DamageEntryDetected(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("EnemyLegs") || other.CompareTag("EnemyHead") || other.CompareTag("EnemyBody"))
                transform.parent.GetComponent<TeddyBearServer>().DamageExitDetected(other);
        }
    }
}
