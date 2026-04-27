using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    [SerializeField] SONoiseSettings noiseSettings;

    public Tilemap tilemap;
    public Tile[] tiles;
    public int widthX = 80;
    public int heightY = 40;
    public int numberOfRooms = 2;
    public int seed;
    public float scale = 0.05f;
    public int averageRoomSize = 5;

    void Start() => Generate();

    private void OnValidate()
    {
        Generate();
    }
    [Button]
    public void Generate()
    {
        Random.InitState(seed);
        int offset = Random.Range(0, 10000);
        noiseSettings.offset = offset;

        tilemap.ClearAllTiles();

        for (int x = 0; x < widthX; x++)
            for (int y = 0; y < heightY; y++)
            {
                float height = PerlinNoise.GetHeight(new Vector2Int(x, y), noiseSettings);
                TileBase tile = GetCorrespondingTile(height);
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
    }

    private Tile GetCorrespondingTile(float noiseValue)
    {
        int index = Mathf.Clamp(Mathf.FloorToInt(noiseValue * tiles.Length), 0, tiles.Length - 1);
        return tiles[index];
    }
}

public class RoomData
{
    public int roomId;
    public int roomSizeX;
    public int roomSizeY;
    public Vector2Int pos;
    public RoomType roomType;
}

public enum RoomType
{
    simple,
    startVillage,
    end
}