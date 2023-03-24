using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.UI;

namespace Benchmark.Fishnet
{
    
    public class ShowCCU : NetworkBehaviour
    {

        public Text ccuText;
    
        [SyncVar]
        public int playersConnected;
        void Update ()
        {
            if (IsServer)
            {
                playersConnected = NetworkManager.ClientManager.Clients.Count;
            }

            ccuText.text = "Client counts: " + playersConnected;
        }
    }
    
}