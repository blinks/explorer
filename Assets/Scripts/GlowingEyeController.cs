using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GlowingEyeController : MonoBehaviour {
  public LayerMask projectileMask;

  void OnTriggerEnter2D(Collider2D collider) {
    if (0 != ((1 << collider.gameObject.layer) & projectileMask)) {
      GetComponent<Light2D>().color = Color.green;
      // TODO: also fire off an event to open doors, etc. 
    }
  }
}
