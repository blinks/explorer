using UnityEngine;

public class ProjectileController : MonoBehaviour {
  bool frozen = false;

  void Update() {
    if (!frozen) {
      // Rotate to follow the arrow's flight.
      var body = GetComponent<Rigidbody2D>();
      body.rotation = Mathf.Atan2(body.velocity.y, body.velocity.x) * Mathf.Rad2Deg;
    }
  }

  void OnCollisionEnter2D(Collision2D collision) {
    if (collision.gameObject.name == "Terrain") {
      frozen = true;
      var body = GetComponent<Rigidbody2D>();
      body.constraints = RigidbodyConstraints2D.FreezeAll;

      // If relatively horizontal, enable the platform effector and disable full collider.
      if (Mathf.Abs(Mathf.Sin(body.rotation * Mathf.Deg2Rad)) < 0.75) {
        // Fix the platform effector's "up".
        var effector = GetComponent<PlatformEffector2D>();
        GetComponent<PolygonCollider2D>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = false;
        if (Mathf.Cos(body.rotation * Mathf.Deg2Rad) < 0) {
          effector.rotationalOffset = 180;
          body.rotation = 180f;
        } else {
          body.rotation = 0f;
        }
        effector.enabled = true;
        gameObject.layer = 8; // Terrain
      }
    }

    if (!frozen && collision.gameObject.name == "Explorer") {
      Destroy(gameObject);
    } else if (frozen && collision.gameObject.name == "Explorer" && !GetComponent<PlatformEffector2D>().isActiveAndEnabled) {
      Destroy(gameObject);
    }
  }
}
