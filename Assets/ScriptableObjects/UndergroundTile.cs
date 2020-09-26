using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class UndergroundTile : TileBase {
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

  // https://docs.unity3d.com/2020.1/Documentation/ScriptReference/Tilemaps.TileData.html
  public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
    tileData.colliderType = Tile.ColliderType.Grid; // always grid collider for static dirt tiles.
    int i = (tilemap.GetTile(position + Vector3Int.up) != null) ? 0b1000 : 0;
    i += (tilemap.GetTile(position + Vector3Int.right) != null) ? 0b0100 : 0;
    i += (tilemap.GetTile(position + Vector3Int.down) != null) ? 0b0010 : 0;
    i += (tilemap.GetTile(position + Vector3Int.left) != null) ? 0b0001 : 0;
    // Index into sprites based on full (true) or empty (false) neighbors.
    switch (i) {
      case 0b1111: // 1111 -> 15; all adjacent, full tile.
        if (!tilemap.GetTile(position + Vector3Int.up + Vector3Int.left)) {
          tileData.sprite = openNorthwest;
        } else if (!tilemap.GetTile(position + Vector3Int.up + Vector3Int.right)) {
          tileData.sprite = openNortheast;
        } else if (!tilemap.GetTile(position + Vector3Int.down + Vector3Int.left)) {
          tileData.sprite = openSouthwest;
        } else if (!tilemap.GetTile(position + Vector3Int.down + Vector3Int.right)) {
          tileData.sprite = openSoutheast;
        }
        break;

      case 0b0000: // 0000 -> 0; nothing adjacent, platform tile.
      case 0b0001: // 0001 -> 1; only west adjacent, platform.
      case 0b0100: // 0100 -> 4; east adjacent; platform.
      case 0b0101: // 0101 -> 5; east and west adjacent; platform.
      case 0b0111: // 0111 -> 7; east, south, west adjacent; platform.
        tileData.sprite = north;
        break;

      case 0b1011: // 1011 -> 11; north, south, west adjacent; east.
        tileData.sprite = east;
        break;

      case 0b1101: // 1101 -> 13; north, east, and west adjacent; south.
        tileData.sprite = south;
        break;

      case 0b1110: // 1110 -> 14; north, east, and south adjacent; west.
        tileData.sprite = west;
        break;

      case 0b0011: // 0011 -> 3; south and west adjacent; northeast corner.
        tileData.sprite = northeast;
        break;

      case 0b0110: // 0110 -> 6; east and south adjacent; northwest corner.
        tileData.sprite = northwest;
        break;

      case 0b1001: // 1001 -> 9; north and west adjacent; southeast corner.
        tileData.sprite = southeast;
        break;

      case 0b1100: // 1100 -> 12; north and east adjacent; southwest corner;
        tileData.sprite = southwest;
        break;

      default:
        // 0010 -> 2; only south adjacent; broken? top column?
        // 1000 -> 8; north adjacent; broken? hanging column?
        // 1010 -> 10; north and south adjacent; broken? middle/base column?
        break;
    }
  }

  public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData) {
    return false; // no animation data for static dirt tiles.
  }

  public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
    // Only needs to refresh orthogonally-adjacent tiles to dig correctly.
    for (int i = -1; i <= 1; i++) {
      for (int j = -1; j <= 1; j++) {
        if (i == 0 && j == 0) { continue; }
        tilemap.RefreshTile(position + new Vector3Int(i, j, 0));
      }
    }
  }

  public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
    return true; // no special startup needed.
  }
}