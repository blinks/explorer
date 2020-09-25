using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    bool frozen = false;

    void Update()
    {
        if (!frozen)
        {
            // Rotate to follow the arrow's flight.
            var body = GetComponent<Rigidbody2D>();
            body.rotation = Mathf.Atan2(body.velocity.y, body.velocity.x) * Mathf.Rad2Deg;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Terrain")
        {
            frozen = true;
            var body = GetComponent<Rigidbody2D>();
            body.constraints = RigidbodyConstraints2D.FreezeAll;
            GetComponent<BoxCollider2D>().enabled = false;

            // If relatively horizontal, enable the platform effector.
            if (Mathf.Abs(Mathf.Sin(body.rotation * Mathf.Deg2Rad)) < 0.5)
            {
                GetComponent<PolygonCollider2D>().enabled = true;

                // Fix the platform effector's "up".
                var effector = GetComponent<PlatformEffector2D>();
                if (Mathf.Cos(body.rotation * Mathf.Deg2Rad) < 0)
                {
                    effector.rotationalOffset = 180;
                }
                effector.enabled = true;
            }
        }
    }
}
