using UnityEngine;

namespace JPO
{
    public class Candy : MonoBehaviour
    {
        [SerializeField] private int candyValue=10;
        public int CandyValue => candyValue;
    }
}
