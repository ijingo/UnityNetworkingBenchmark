using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;

namespace Benchmark.Fishnet
{
    [AddComponentMenu("Network/Network Manager HUD")]
    [RequireComponent(typeof(NetworkManager))]
    public class NetworkHUD : MonoBehaviour
    {
        NetworkManager _networkManager;

        public int offsetX;
        public int offsetY;
        
        private LocalConnectionState _clientState = LocalConnectionState.Stopped;
        private LocalConnectionState _serverState = LocalConnectionState.Stopped;
        private string _hostAddr = "127.0.0.1";

        void Awake()
        {
            _networkManager = GetComponent<NetworkManager>();
        }

        void Start()
        {
            
            _networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }
        
        void OnDestroy()
        {
            if (_networkManager == null)
                return;

            _networkManager.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
        }
        
        private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
        {
            _clientState = obj.ConnectionState;
        }


        private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj)
        {
            _serverState = obj.ConnectionState;
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 250, 9999));

            if (_clientState == LocalConnectionState.Stopped)
            {
                // Client + IP
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Client"))
                {
                    _networkManager.ClientManager.StartConnection(_hostAddr);
                }

                _hostAddr = GUILayout.TextField(_hostAddr);
                // This updates networkAddress every frame from the TextField
                GUILayout.EndHorizontal();

            }
            if (_serverState == LocalConnectionState.Stopped) {
                if (GUILayout.Button("Server")) _networkManager.ServerManager.StartConnection();
            }
            
            if (_clientState == LocalConnectionState.Started && _serverState == LocalConnectionState.Started)
            {
                GUILayout.Label($"<b>Host</b>: running");
            }
            // server only
            else if (_serverState == LocalConnectionState.Started)
            {
                GUILayout.Label($"<b>Server</b>: running");
            }
            // client only
            else if (_clientState == LocalConnectionState.Started)
            {
                GUILayout.Label($"<b>Client</b>: connected to {_hostAddr}");
            }

            StopButtons();

            GUILayout.EndArea();
        }

        void StopButtons()
        {
            if (_clientState == LocalConnectionState.Started)
            {
                if (GUILayout.Button("Stop Client"))
                {
                    _networkManager.ClientManager.StopConnection();
                }
            }
            // stop server 
            if (_serverState == LocalConnectionState.Started)
            {
                if (GUILayout.Button("Stop Server"))
                {
                    _networkManager.ServerManager.StopConnection(true);
                }
            }
        }
    }
}