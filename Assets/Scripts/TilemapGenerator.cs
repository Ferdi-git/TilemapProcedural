using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile BaseTile;

    public int width = 80;
    public int height = 40;
    public int seed;

    public int averageRoomSize = 5;

    void Start() => Generate();

    private void OnValidate()
    {
        Generate();
    }

    public void Generate()
    {
        tilemap.ClearAllTiles();
        Vector2 offset = new Vector2(seed, seed);

        // Grid Created
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                TileBase tile = BaseTile;
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        
        bool foundRoom = false;
        while (!foundRoom)
        {
            int randX = Random.Range(0, width);
            int randY = Random.Range(0, height); ;
        }

    }


}
