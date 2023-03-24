using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using UnityEngine;

namespace Benchmark.Fishnet
{
    public class Spawner : NetworkBehaviour
    {
        // [Header("Spawner Setup")]
        // [Tooltip("Reward Prefab for the Spawner")]
        // public GameObject rewardPrefab;
        [Tooltip("Icosphere Prefab for the Spawner")]
        public GameObject icospherePrefab;
        
        static public int range = 50;

        public override void OnStartServer()
        {
            base.OnStartServer();
            StartCoroutine(InitSpawn());
        }

        IEnumerator InitSpawn()
        {
            for (int i = 0; i < Configuration.spawnNumber; ++i)
            {
                SpawnIcosphere();
                yield return null;
            }
        }

        void SpawnIcosphere()
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-range + 1, range), 1, Random.Range(-range + 1, range));
            GameObject icosphere = Instantiate(icospherePrefab, spawnPosition, Quaternion.identity);
            InstanceFinder.ServerManager.Spawn(icosphere);
        }
    }

}