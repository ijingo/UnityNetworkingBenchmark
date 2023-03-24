using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Benchmark.Fishnet
{
    public class PlayerScore : NetworkBehaviour
    {
        private static int _playerCount = 0;
        
        [SyncVar]
        public int playerNumber;

        [SyncVar]
        public int scoreIndex;

        [SyncVar]
        public uint score;

        public override void OnStartServer()
        {
            base.OnStartServer();
            _playerCount += 1;
            playerNumber = _playerCount;
            scoreIndex = _playerCount;
            score = 0;
        }


        void OnGUI()
        {
            GUI.Box(new Rect(10f + (scoreIndex * 110), 10f, 100f, 25f), $"P{playerNumber}: {score}");
        }
    }
}