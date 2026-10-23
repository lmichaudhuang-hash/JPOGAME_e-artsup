using UnityEngine;

namespace JPO
{
    public class Throwable : MonoBehaviour
    {
        private Rigidbody2D rb;
        private bool isThrown;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

        }

        public void PickUp(Transform holdPoint)
        {
            isThrown = false;
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false; //coupe la physique pendant qu'il est tenu
            transform.SetParent(holdPoint);
            transform.localPosition = Vector3.zero;

        }
        public void Throw(Vector2 velocity)
        {
            transform.SetParent(null);
            rb.simulated = true;
            rb.linearVelocity = velocity;
            isThrown = true;
        }
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isThrown) return;
            Breakable wall = collision.collider.GetComponent<Breakable>();
            if (wall != null)
                wall.Break();

            Destroy(gameObject); //l'objet se casse à l'impact
        }

    }
}
