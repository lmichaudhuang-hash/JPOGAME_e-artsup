using UnityEngine;
using UnityEngine.InputSystem;

namespace JPO
{
    public class PlayerGrab : MonoBehaviour
    {
        [SerializeField] private Transform holdPoint; //où l'objet est tenu
        [SerializeField] private float grabRadius = 1.2f; //distance pour attraper
        [SerializeField] private float throwSpeed = 12f;

        private Throwable heldObject;
        void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;
            if (kb.eKey.wasPressedThisFrame && heldObject == null)
                TryGrab();
            else if (kb.spaceKey.wasPressedThisFrame && heldObject != null)
                Throw();

        }

        void TryGrab()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, grabRadius);
            foreach (var hit in hits)
            {
                Throwable t = hit.GetComponent<Throwable>();
                if (t != null)
                {
                    heldObject = t;
                    t.PickUp(holdPoint);
                    return;
                }
            }

        }

        void Throw()
        {
            // transform.up = direction vers laquelle pointe le perso
            heldObject.Throw(transform.up * throwSpeed);
            heldObject = null;
        }
    }
}