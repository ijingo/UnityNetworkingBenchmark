using FishNet.Managing.Logging;
using FishNet.Object;
using UnityEngine;

namespace Benchmark.Fishnet
{
    [RequireComponent(typeof(RandomColor))]
    public class Reward : NetworkBehaviour
    {
        public bool available = true;
        public RandomColor randomColor;
        private PlayerController _localChasingPlayer;

        public void SetChasingPlayer(PlayerController playerController)
        {
            _localChasingPlayer = playerController;
        }
        
        public void ClearChasingPlayer()
        {
            _localChasingPlayer = null;
        }

        protected override void OnValidate()
        {
            if (randomColor == null)
                randomColor = GetComponent<RandomColor>();
        }

        [Server(Logging = LoggingType.Off)]
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                ClaimPrize(other.gameObject);
        }

        // This is called from PlayerController.CmdClaimPrize which is invoked by PlayerController.OnControllerColliderHit
        // This only runs on the server
        [Server(Logging = LoggingType.Off)]
        public void ClaimPrize(GameObject player)
        {
            if (available)
            {
                // This is a fast switch to prevent two players claiming the prize in a bang-bang close contest for it.
                // First hit turns it off, pending the object being destroyed a few frames later.
                available = false;

                Color32 color = randomColor.color;

                // calculate the points from the color ... lighter scores higher as the average approaches 255
                // UnityEngine.Color RGB values are float fractions of 255
                uint points = (uint)(((color.r) + (color.g) + (color.b)) / 3);

                // award the points via SyncVar on the PlayerController
                player.GetComponent<PlayerScore>().score += points;

                // spawn a replacement
                Spawner.SpawnReward();

                // destroy this one
                ServerManager.Despawn(gameObject);
            }
        }
        
        public void OnDestroy()
        {
            if (_localChasingPlayer)
            {
                _localChasingPlayer.ClearTarget();
            }
        }
    }
}