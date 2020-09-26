using UnityEngine;
using UnityEngine.Tilemaps;

public class LanternController : MonoBehaviour {
  public Tilemap tilemap;
  public float detonationImpulse;

  void OnCollisionEnter2D(Collision2D collision) {
    if (collision.GetContact(0).normalImpulse > detonationImpulse) {
      var cell = tilemap.WorldToCell(transform.position);
      Debug.LogFormat("Lantern hit {0} -> {1}", collision.gameObject.name, cell);
      tilemap.SetTilesBlock(new BoundsInt(cell.x - 1, cell.y - 1, 0, 3, 3, 1),
          new TileBase[9] { null, null, null, null, null, null, null, null, null });
      tilemap.RefreshAllTiles();
      tilemap.GetComponent<TilemapCollider2D>().ProcessTilemapChanges();
      Destroy(gameObject);
    }
  }
}