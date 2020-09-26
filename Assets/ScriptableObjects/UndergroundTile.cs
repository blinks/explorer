using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class UndergroundTile : TileBase {
  public Sprite broken;

  public Sprite northwest;
  public Sprite north;
  public Sprite northeast;
  public Sprite west;
  public Sprite east;
  public Sprite southwest;
  public Sprite south;
  public Sprite southeast;

  // open corners, non-orthogonal
  public Sprite openNorthwest;
  public Sprite openNortheast;
  public Sprite openSouthwest;
  public Sprite openSoutheast;

  int orthogonallyDug(Vector3Int p, ITilemap map) {
    // Return 0bNESW -> where bits are 1 if that direction is dug out.
    // Use "empty" tiles (_not_ dug out) to represent the hard border of the map.
    return (map.GetTile(p + Vector3Int.up) ? 0b1000 : 0)
      + (map.GetTile(p + Vector3Int.right) ? 0b0100 : 0)
      + (map.GetTile(p + Vector3Int.down) ? 0b0010 : 0)
      + (map.GetTile(p + Vector3Int.left) ? 0b0001 : 0);
  }

  int diagonallyDug(Vector3Int p, ITilemap map) {
    // Return 0b(NW)(NE)(SW)(SE) -> where bits are 1 if that direction is dug out.
    // Use "empty" tiles (_not_ dug out) to represent the hard border of the map.
    return (map.GetTile(p + Vector3Int.up + Vector3Int.left) ? 0b1000 : 0)
      + (map.GetTile(p + Vector3Int.up + Vector3Int.right) ? 0b0100 : 0)
      + (map.GetTile(p + Vector3Int.down + Vector3Int.left) ? 0b0010 : 0)
      + (map.GetTile(p + Vector3Int.down + Vector3Int.right) ? 0b0001 : 0);
  }

  public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
    // Index into sprites based on full (true) or empty (false) neighbors.
    tileData.colliderType = Tile.ColliderType.Grid; // default grid collider.
    switch (orthogonallyDug(position, tilemap)) {
      // dug out on all four sides: refer to corners.
      case 0b1111:
        switch (diagonallyDug(position, tilemap)) {
          case 0b1110: tileData.sprite = northwest; break;
          case 0b1101: tileData.sprite = northeast; break;
          case 0b1011: tileData.sprite = southwest; break;
          case 0b0111: tileData.sprite = southeast; break;
          default:
            // If all dug out, turn off the sprite + collider.
            tileData.colliderType = Tile.ColliderType.None;
            tileData.sprite = null;
            break;
        }
        break;

      case 0b1100: tileData.sprite = openNortheast; break;
      case 0b1001: tileData.sprite = openNorthwest; break;
      case 0b0011: tileData.sprite = openSouthwest; break;
      case 0b0110: tileData.sprite = openSoutheast; break;

      case 0b1011: tileData.sprite = west; break;
      case 0b0111: tileData.sprite = south; break;
      case 0b1110: tileData.sprite = east; break;
      case 0b1101: tileData.sprite = north; break;

      default: tileData.sprite = broken; break;
    }
  }

  public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
    // Only needs to refresh orthogonally-adjacent tiles to dig correctly.
    for (int i = -1; i <= 1; i++) {
      for (int j = -1; j <= 1; j++) {
        tilemap.RefreshTile(position + new Vector3Int(i, j, 0));
      }
    }
  }
}