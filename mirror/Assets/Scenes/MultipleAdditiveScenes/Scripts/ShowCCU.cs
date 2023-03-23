using Mirror;
using UnityEngine;
using UnityEngine.UI;
 
public class ShowCCU : NetworkBehaviour
{

    public Text ccuText;
    
    [SyncVar]
    public int playersConnected;
    void Update ()
    {
        if (isServer)
        {
            playersConnected = NetworkServer.connections.Count;
        }

        ccuText.text = "Client counts: " + playersConnected;
    }
}