using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile UncheckedTile,CheckedTile;

    public int width = 80;
    public int height = 40;
    public int numberOfRooms = 2;
    public int seed;

    public int averageRoomSize = 5;

    void Start() => Generate();


    [Button]
    public void Generate()
    {
        tilemap.ClearAllTiles();
        Vector2 offset = new Vector2(seed, seed);

        // Grid Created
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                TileBase tile = UncheckedTile;
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        
        for(int i = 0 ; i < numberOfRooms; i++)
        {
            RoomData newRoom = new RoomData();
            newRoom.roomId = i;
            newRoom.roomType = RoomType.simple;

            bool foundRoom = false;

            int maxAttempts = 100;
            int attempts = 0;

            while (!foundRoom && attempts < maxAttempts)
            {
                
                int randX = Random.Range(0, width);
                int randY = Random.Range(0, height);
                newRoom.pos = new Vector2Int(randX, randY);
                int randSize = Random.Range(0, 4);
                newRoom.roomSizeX = averageRoomSize + randSize;
                newRoom.roomSizeY = averageRoomSize - randSize;
                foundRoom = CheckIfRoomFit(newRoom);
                attempts++;

            }

            if (!foundRoom)
            {
                Debug.LogWarning($"Room {i} could not be placed after {maxAttempts} attempts, skipping.");
                break;
            }
            else
            {
                BuildRoom(newRoom);

            }
        }


    }


    private bool CheckIfRoomFit(RoomData roomData)
    {
        for (int x = 0; x < roomData.roomSizeX; x ++)
        {
            for (int y = 0; y < roomData.roomSizeY; y++)
            {
                Vector3Int newPosTile = new Vector3Int(roomData.pos.x + x, roomData.pos.y + y,0);
                if (tilemap.GetTile(newPosTile) != UncheckedTile)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void BuildRoom(RoomData roomData)
    {
        for (int x = 0; x < roomData.roomSizeX; x++)
        {
            for (int y = 0; y < roomData.roomSizeY; y++)
            {
                Vector3Int newPosTile = new Vector3Int(roomData.pos.x + x, roomData.pos.y + y, 0);
                tilemap.SetTile(newPosTile, CheckedTile);
            }
        }
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

