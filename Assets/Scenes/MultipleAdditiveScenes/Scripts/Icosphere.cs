using UnityEngine;

namespace Mirror.Examples.MultipleAdditiveScenes
{
    [RequireComponent(typeof(RandomColor))]
    public class Icosphere : NetworkBehaviour
    {
        public RandomColor randomColor;

        void OnValidate()
        {
            if (randomColor == null)
                randomColor = GetComponent<RandomColor>();
        }
    }
}
