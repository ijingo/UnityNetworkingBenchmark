using UnityEngine;
using UnityEngine.UI;

namespace Benchmark.Fishnet
{
    public class ShowFPS : MonoBehaviour
    {
        public Text fpsText;
        public float deltaTime;
        public int updateDisplayCount = 0;
 
        void Update ()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;

            if (updateDisplayCount > 30) { fpsText.text = "fps: " + Mathf.Ceil(fps); updateDisplayCount = 0; }
            else { updateDisplayCount += 1; }
        }
    }
    
}