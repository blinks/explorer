using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Tilemaps;

public class EyeSwitchController : MonoBehaviour {
  public Tilemap tilemap;
  public Vector3Int position;
  public Light2D glowingEye;

  void OnTriggerEnter2D(Collider2D collision) {
    tilemap.SetTile(position, null);
    tilemap.RefreshTile(position);
    glowingEye.color = Color.green;
  }
}
