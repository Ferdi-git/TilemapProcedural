using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    [SerializeField] SONoiseSettings noiseSettings;

    public Tilemap baseTilemap;
    public Tilemap biomeTilemap;

    public Tile Grass;
    public Tile Water;
    public Tile Forest;

    public Tile[] grassCornerTiles;
    public Tile[] grassTiles;
    public Tile[] forestCornerTiles;
    public Tile[] waterTiles;

    public int widthX = 80;
    public int heightY = 40;
    public int seed;

    public float limitWater = 0.4f;
    public float limitForest = 0.4f;

    private TileBase[,] biomeSnapshot;

    private void OnValidate()
    {
        Generate();
    }

    [Button]
    public void Generate()
    {
        Random.InitState(seed);

        noiseSettings.offsetX = Random.Range(0, 10000);
        noiseSettings.offsetY = Random.Range(0, 10000);

        GenerateBaseTilemap();

        noiseSettings.offsetX = Random.Range(0, 10000);
        noiseSettings.offsetY = Random.Range(0, 10000);

        GenerateBiomeTilemap();
    }

    private void GenerateBaseTilemap()
    {
        baseTilemap.ClearAllTiles();

        for (int x = 0; x < widthX; x++)
            for (int y = 0; y < heightY; y++)
            {
                float height = PerlinNoise.GetHeight(new Vector2Int(x, y), noiseSettings);
                TileBase tile = GetCorrespondingBaseTile(height);
                baseTilemap.SetTile(new Vector3Int(x, y), tile);
            }

        for (int i = 0; i < 3; i++)
        {
            for (int x = 0; x < widthX; x++)
                for (int y = 0; y < heightY; y++)
                    ApplyBaseAutoTile(x, y);
        }
    }

    private void GenerateBiomeTilemap()
    {
        biomeTilemap.ClearAllTiles();

        biomeSnapshot = new TileBase[widthX, heightY];

        for (int x = 0; x < widthX; x++)
            for (int y = 0; y < heightY; y++)
            {
                if (baseTilemap.GetTile(new Vector3Int(x, y)) != Grass)
                    continue;

                float height = PerlinNoise.GetHeight(new Vector2Int(x, y), noiseSettings);
                TileBase tile = GetCorrespondingBiomeTile(height);

                biomeSnapshot[x, y] = tile; 
                biomeTilemap.SetTile(new Vector3Int(x, y), tile);
            }
        for (int i = 0; i < 3; i++)
        {
            for (int x = 0; x < widthX; x++)
                for (int y = 0; y < heightY; y++)
                    ApplyForestAutoTile(x, y);
        }
    }

    private void ApplyBaseAutoTile(int x, int y)
    {
        Tile tile = CheckGrassWaterCorners(new Vector2Int(x, y));

        if (tile != null)
            baseTilemap.SetTile(new Vector3Int(x, y), tile);
    }

    private Tile GetCorrespondingBaseTile(float noiseValue)
    {
        return noiseValue < limitWater ? Water : Grass;
    }

    private Tile GetCorrespondingBiomeTile(float noiseValue)
    {
        return noiseValue < limitForest ? null : Forest;
    }

    // ---------------- WATER CHECK ----------------
    private bool IsWater(int x, int y)
    {
        return baseTilemap.GetTile(new Vector3Int(x, y)) != Grass;
    }

    // ---------------- WATER/GRASS AUTOTILE ----------------
    private Tile CheckGrassWaterCorners(Vector2Int pos)
    {
        TileBase current = baseTilemap.GetTile(new Vector3Int(pos.x, pos.y));

        if (current == Grass)
            return null;

        bool n = !IsWater(pos.x, pos.y + 1);
        bool s = !IsWater(pos.x, pos.y - 1);
        bool e = !IsWater(pos.x + 1, pos.y);
        bool w = !IsWater(pos.x - 1, pos.y);

        bool ne = !IsWater(pos.x + 1, pos.y + 1);
        bool nw = !IsWater(pos.x - 1, pos.y + 1);
        bool se = !IsWater(pos.x + 1, pos.y - 1);
        bool sw = !IsWater(pos.x - 1, pos.y - 1);

        int mask = (n ? 1 : 0) | (s ? 2 : 0) | (e ? 4 : 0) | (w ? 8 : 0);

        if (!n && !s && !e && !w)
        {
            if (ne) return grassCornerTiles[8];
            if (nw) return grassCornerTiles[9];
            if (se) return grassCornerTiles[10];
            if (sw) return grassCornerTiles[11];
        }

        return mask switch
        {
            0b0001 => grassCornerTiles[0],
            0b0010 => grassCornerTiles[1],
            0b0100 => grassCornerTiles[2],
            0b1000 => grassCornerTiles[3],

            0b0101 => grassCornerTiles[4],
            0b1001 => grassCornerTiles[5],
            0b0110 => grassCornerTiles[6],
            0b1010 => grassCornerTiles[7],

            0 => Water,
            _ => Grass
        };
    }

    private void ApplyForestAutoTile(int x, int y)
    {
        Vector3Int pos = new Vector3Int(x, y);

        if (biomeSnapshot[x, y] != Forest)
            return;

        bool n = !IsForest(x, y + 1);
        bool s = !IsForest(x, y - 1);
        bool e = !IsForest(x + 1, y);
        bool w = !IsForest(x - 1, y);

        bool ne = !IsForest(x + 1, y + 1);
        bool nw = !IsForest(x - 1, y + 1);
        bool se = !IsForest(x + 1, y - 1);
        bool sw = !IsForest(x - 1, y - 1);

        int mask = (n ? 1 : 0) | (s ? 2 : 0) | (e ? 4 : 0) | (w ? 8 : 0);

        if (!n && !s && !e && !w)
        {
            if (ne) biomeTilemap.SetTile(pos, forestCornerTiles[8]);
            if (nw) biomeTilemap.SetTile(pos, forestCornerTiles[9]);
            if (se) biomeTilemap.SetTile(pos, forestCornerTiles[10]);
            if (sw) biomeTilemap.SetTile(pos, forestCornerTiles[11]);
            return;
        }

        Tile tile = mask switch
        {
            0b0001 => forestCornerTiles[0],
            0b0010 => forestCornerTiles[1],
            0b0100 => forestCornerTiles[2],
            0b1000 => forestCornerTiles[3],

            0b0101 => forestCornerTiles[4],
            0b1001 => forestCornerTiles[5],
            0b0110 => forestCornerTiles[6],
            0b1010 => forestCornerTiles[7],

            _ => null
        };

        biomeTilemap.SetTile(pos, tile);
    }

    private bool IsForest(int x, int y)
    {
        if (x < 0 || x >= widthX || y < 0 || y >= heightY)
            return false;

        return biomeSnapshot[x, y] == Forest;
    }
}