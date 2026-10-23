using UnityEngine;

namespace JPO
{
    public class CandyCollector : MonoBehaviour
    {
        private bool _isDoorOpen;
        private int _candyCount;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Candy")) return;
            if (!collision.TryGetComponent(out Candy candy)) return;
            _candyCount += candy.CandyValue;
            Destroy(collision.gameObject);
            
            EventBus.OnCandyCollected(_candyCount);
            if (_candyCount >= 1000 && !_isDoorOpen)
            {
                _isDoorOpen = true;
                EventBus.OnKeyCollected();
            }
        }

    }
}
