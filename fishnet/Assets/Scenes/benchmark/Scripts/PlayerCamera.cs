using FishNet.Object;
using UnityEngine;

// This sets up the scene camera for the local player

namespace Benchmark.Fishnet
{
    public class PlayerCamera : NetworkBehaviour
    {
        Camera mainCam;

        void Awake()
        {
            mainCam = Camera.main;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (base.IsOwner)
            {
                if (mainCam != null)
                {
                    // configure and make camera a child of player with 3rd person offset
                    mainCam.orthographic = false;
                    mainCam.transform.SetParent(transform);
                    mainCam.transform.localPosition = new Vector3(0f, 3f, -8f);
                    mainCam.transform.localEulerAngles = new Vector3(10f, 0f, 0f);
                }
                else
                    Debug.LogWarning("PlayerCamera: Could not find a camera in scene with 'MainCamera' tag.");
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            if (base.IsOwner)
            {
                if (mainCam != null)
                {
                    mainCam.transform.SetParent(null);
                    UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(
                        mainCam.gameObject, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                    mainCam.orthographic = true;
                    mainCam.orthographicSize = 15f;
                    mainCam.transform.localPosition = new Vector3(0f, 70f, 0f);
                    mainCam.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                }
            }
        }
    }
}