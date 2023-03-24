using FishNet.Managing.Logging;
using FishNet.Object;
using UnityEngine;

namespace Benchmark.Fishnet
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsCollision : NetworkBehaviour
    {
        [Tooltip("how forcefully to push this object")]
        public float force = 12;

        public Rigidbody rigidbody3D;

        protected override void OnValidate()
        {
            if (rigidbody3D == null)
                rigidbody3D = GetComponent<Rigidbody>();
        }

        [Client(Logging = LoggingType.Off)]
        void Start()
        {
            rigidbody3D.isKinematic = IsClientOnly;
        }

        [Server]
        void OnCollisionStay(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // get direction from which player is contacting object
                Vector3 direction = other.contacts[0].normal;

                // zero the y and normalize so we don't shove this through the floor or launch this over the wall
                direction.y = 0;
                direction = direction.normalized;

                rigidbody3D.AddForce(direction * force);

                other.gameObject.GetComponent<PlayerController>()
                    .TargetClearTarget(other.gameObject.GetComponent<NetworkObject>().Owner);
            }
        }
    }
}
