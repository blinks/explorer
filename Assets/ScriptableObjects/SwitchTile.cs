using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class SwitchTile : Tile {
  public Vector3 lightOffset = new Vector3(.5f, .5f, 0f);

  public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
    // Shift the light object over to where the eye of the sprite is.
    if (go) { go.transform.position = position + lightOffset; }
    return true;
  }
}
