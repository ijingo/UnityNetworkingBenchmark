using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mirror.Examples.MultipleAdditiveScenes
{
    internal class RewardSpawner
    {
        static public int range = 50;
        
        [ServerCallback]
        internal static void InitialSpawn(Scene scene)
        {
            for (int i = 0; i < 100; i++)
                SpawnReward(scene);
        }

        [ServerCallback]
        internal static void SpawnReward(Scene scene)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-range + 1, range), 1, Random.Range(-range + 1, range));
            GameObject reward = Object.Instantiate(((MultiSceneNetManager)NetworkManager.singleton).rewardPrefab, spawnPosition, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(reward, scene);
            NetworkServer.Spawn(reward);
        }
    }
    
    internal class IcosphereSpawner
    {
        static public int range = 50;
        
        [ServerCallback]
        internal static void InitialSpawn(Scene scene)
        {
            for (int i = 0; i < 100; i++)
                SpawnIcosphere(scene);
        }

        [ServerCallback]
        internal static void SpawnIcosphere(Scene scene)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-range + 1, range), 1, Random.Range(-range + 1, range));
            GameObject reward = Object.Instantiate(((MultiSceneNetManager)NetworkManager.singleton).icospherePrefab, spawnPosition, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(reward, scene);
            NetworkServer.Spawn(reward);
        }
    }
}
