using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    Rigidbody2D body;
    bool frozen = false;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!frozen)
        {
            // Rotate to follow the arrow's flight.
            body.rotation = Mathf.Atan2(body.velocity.y, body.velocity.x) * Mathf.Rad2Deg;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Terrain")
        {
            // Fix the platform effector's "up" before freezing.
            var effector = GetComponent<PlatformEffector2D>();
            effector.enabled = true;
            if (Mathf.Cos(body.rotation * Mathf.Deg2Rad) < 0)
            {
                effector.rotationalOffset = 180;
            }

            frozen = true;
            body.constraints = RigidbodyConstraints2D.FreezeAll;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
