using UnityEngine;

namespace JPO
{
    public class GestionTailleCamera : MonoBehaviour
    {
        public float TailleNormale = 5f;
        public float TaillePetite = 2f;

        void Start()
        {
            Camera.main.orthographicSize = TailleNormale;
        }

        public void EntrerLimiteLieu()
        {
            Camera.main.orthographicSize = TaillePetite;

        }

        public void RepasseLimiteLieu_01()
        {
            Camera.main.orthographicSize = TailleNormale;
        }
    }
}


        