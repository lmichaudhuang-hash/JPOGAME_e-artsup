using UnityEngine;

namespace JPO
{
    public class Breakable : MonoBehaviour
    {
        public void Break()
        {
            Destroy(gameObject);
        }
    }
}

