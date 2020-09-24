using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    Rigidbody2D body;

    void Start()
    {
        // Memoize the body for update.
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Rotate to follow the "arrowhead".
        var v = body.velocity;
        body.SetRotation(Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg);
    }
}
