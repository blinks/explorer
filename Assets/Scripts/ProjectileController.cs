using UnityEngine;

public class ProjectileController : MonoBehaviour {
  public LayerMask freezingLayerMask;
  public int frozenLayer;

  bool Frozen => (gameObject.layer == frozenLayer);

  bool FreezingLayer(int layer) {
    return 0 != (freezingLayerMask & (1 << layer));
  }

  void Update() {
    if (!Frozen) {
      // Rotate to follow the arrow's flight.
      var body = GetComponent<Rigidbody2D>();
      body.rotation = Mathf.Atan2(body.velocity.y, body.velocity.x) * Mathf.Rad2Deg;
    }
  }

  void OnCollisionEnter2D(Collision2D collision) {
    // Freeze on hitting the right object layers.
    if (!Frozen && FreezingLayer(collision.gameObject.layer)) {
      gameObject.layer = frozenLayer;

      var body = GetComponent<Rigidbody2D>();
      body.constraints = RigidbodyConstraints2D.FreezeAll;

      // If relatively horizontal, enable the platform effector and disable full collider.
      if (Mathf.Abs(Mathf.Sin(body.rotation * Mathf.Deg2Rad)) < 0.75) {
        GetComponent<PolygonCollider2D>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = false;

        var effector = GetComponent<PlatformEffector2D>();
        effector.enabled = true;
        // Fix the platform effector's "up".
        if (Mathf.Cos(body.rotation * Mathf.Deg2Rad) < 0) {
          effector.rotationalOffset = 180;
          body.rotation = 180f;
        } else {
          body.rotation = 0f;
        }
      }
    }
  }
}
