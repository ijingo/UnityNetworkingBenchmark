using System;
using System.Collections;
using FishNet.Managing;
using UnityEngine;
using UnityEngine.Rendering;

namespace Benchmark.Fishnet
{
    public class HeadlessStarter : MonoBehaviour
    {
        private NetworkManager _networkManager;
        
        bool IsHeadless() {
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
        }

        // Start is called before the first frame update
        public void Start()
        {
            Application.targetFrameRate = 60;
            if (!IsHeadless()) return;
            
            Debug.Log("Server: - file - s - spawnObjectCount");
            Debug.Log("Client: - file - c - hostIP - enableAuto");

            _networkManager = FindObjectOfType<NetworkManager>();
            if (_networkManager == null)
            {
                Debug.LogError("NetworkManager not found, HUD will not function.");
            }
            else
            {
                StartCoroutine("StartHeadless");
            }
        }

        IEnumerator StartHeadless()
        {
            string[] args = Environment.GetCommandLineArgs();
            Debug.Log("args = " + string.Join(",", args));
        
            if (args == null || (args != null && args.Length <= 1))
            {
                Debug.Log("Starting a default server setup.");
                _networkManager.ServerManager.StartConnection();
            }
            else
            {
                yield return new WaitForSeconds(1.0f);
                
                if (args[1] == "s")
                {
                    if (args.Length >= 3)
                    {
                        Configuration.spawnNumber = int.Parse(args[2]);
                    }
                    _networkManager.ServerManager.StartConnection();
                }
                else if (args[1] == "c")
                {
                    string addr = "localhost";
                    if (args.Length >= 3)
                    {
                        addr = args[2];
                    }
        
                    if (args.Length >= 4)
                    {
                        Configuration.autoControl = bool.Parse(args[3]);
                    }
                    
                    yield return new WaitForSeconds(UnityEngine.Random.Range(0.0f, 2.0f));
                    _networkManager.ClientManager.StartConnection(addr);
                }
            }
        }
        
        // Update is called once per frame
        void Update()
        {

        }
    }
}