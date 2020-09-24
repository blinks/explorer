using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    bool frozen = false;

    void Update()
    {
        // TODO: If frozen, start growing a platform.
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO: Only collide with terrain.
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        frozen = true;
    }
}
