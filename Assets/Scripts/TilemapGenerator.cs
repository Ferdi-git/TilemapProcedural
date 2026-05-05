using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    [SerializeField] SONoiseSettings noiseSettings;
    [SerializeField] SONoiseSettings treeNoiseSettings;
    [SerializeField] GameObject PlayerSpawnPoint;
    public int widthX = 80;
    public int heightY = 40;
    public int seed;

    [Header("Village")]

    public int numberTentVillage = 3;
    public float minTentDistance = 5f;
    public float maxTentDistance = 15f;
    public float villageDecorRadius = 10f;

    [Header("Tilemap")]

    public Tilemap baseTilemap,baseWaterTilemap,biomeTilemap,decorTilemap,buildingsTilemap;

    public Tile Grass,Water,Forest;
    public Tile[] UpTree, DownTree , GrassFlowers, WaterDecor, ForestDecor, DecorVillage;

    public GameObject[] TentsVillage;


    public Tile[] grassCornerTiles, forestCornerTiles;

    public Tile[] grassTiles,waterTiles;

    public GameObject[] Trees;

    [Header("Limits")]

    public float limitWater = 0.4f;
    public float limitForest = 0.4f;

    public float[] limitTrees;

    public float[] limitGrassFlowers;

    public float[] limitWaterDecor;

    public float limitForestDecor;

    private TileBase[,] biomeSnapshot;

    private List<Vector3Int> placedTentPositions = new List<Vector3Int>();

    private void OnValidate()
    {
        //Generate();
    }

    private void Start()
    {
        for (int i = buildingsTilemap.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(buildingsTilemap.transform.GetChild(i).gameObject);
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

        CleanBiomeTilemap();

        treeNoiseSettings.offsetX = Random.Range(0, 10000);
        treeNoiseSettings.offsetY = Random.Range(0, 10000);

        GenerateTreesAndFlower();


        GenerateVillage();

        PlacePlayerSpawnPoint();

        GenerateVillageDecor();

    }

    private void GenerateBaseTilemap()
    {
        baseTilemap.ClearAllTiles();
        baseWaterTilemap.ClearAllTiles();

        for (int x = 0; x < widthX; x++)
            for (int y = 0; y < heightY; y++)
            {
                float height = PerlinNoise.GetHeight(new Vector2Int(x, y), noiseSettings);
                TileBase tile = GetCorrespondingBaseTile(height);

                if(tile == Water)
                {
                    baseWaterTilemap.SetTile(new Vector3Int(x, y), tile);

                }
                else
                {
                    baseTilemap.SetTile(new Vector3Int(x, y), tile);
                }
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

    }
    private void CleanBiomeTilemap()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int x = 0; x < widthX; x++)
                for (int y = 0; y < heightY; y++)
                    ApplyForestAutoTile(x, y);
        }
    }

    private void GenerateTreesAndFlower()
    {
        decorTilemap.ClearAllTiles();

        for (int x = 0; x < widthX; x++)
            for (int y = 0; y < heightY; y++)
            {
                Vector3Int basePos = new Vector3Int(x, y);

                float height = PerlinNoise.GetHeight(new Vector2Int(x, y), treeNoiseSettings);

                for (int i = 0; limitTrees.Length > i; i++)
                {
                    SetTrees(height, UpTree[i], DownTree[i], limitTrees[i], basePos, Trees[i]);

                }

                for (int i = 0; limitGrassFlowers.Length > i; i++)
                {
                    SetDecor(height, GrassFlowers[i], limitGrassFlowers[i], basePos);

                }

                SetForestDecor(height, limitForestDecor, basePos);
                

                for (int i = 0; limitWaterDecor.Length > i; i++)
                {
                    SetWaterDecor(height, WaterDecor[i], limitWaterDecor[i], basePos);
                }

            }

    }


    private void SetTrees(float height, Tile tileToPutUp, Tile tileToPutDown, float limitToUse, Vector3Int pos, GameObject Tree)
    {  
        if (height < limitToUse && decorTilemap.GetTile(pos) == null && decorTilemap.GetTile(new Vector3Int(pos.x, pos.y + 1)) == null && (!forestCornerTiles.Contains(biomeTilemap.GetTile(pos)) && biomeTilemap.GetTile(pos) == Forest))
        {
            decorTilemap.SetTile(new Vector3Int(pos.x, pos.y + 1), tileToPutUp);


            decorTilemap.SetTile(pos, tileToPutDown);

            Vector3 worldPos = decorTilemap.GetCellCenterWorld(pos);
            Instantiate(Tree, worldPos, Quaternion.identity);
        }
    }

    private void SetDecor(float height, Tile tileToPut, float limitToUse, Vector3Int pos)
    {
        TileBase currentBiomeTile = biomeTilemap.GetTile(pos);
        TileBase currentBaseTile = baseTilemap.GetTile(pos);
        if (height < limitToUse && currentBaseTile != null && (currentBiomeTile != Forest && !grassCornerTiles.Contains(currentBaseTile) && !forestCornerTiles.Contains(currentBiomeTile) && baseTilemap.GetTile(pos) != Water))
        {
            decorTilemap.SetTile(pos, tileToPut);
        }
    }

    private void SetForestDecor(float height, float limitToUse, Vector3Int pos)
    {
        TileBase currentBiomeTile = biomeTilemap.GetTile(pos);
        TileBase currentBaseTile = baseTilemap.GetTile(pos);
        TileBase currentDecorTile = decorTilemap.GetTile(pos);
        if (height < limitToUse && currentDecorTile == null && (currentBiomeTile == Forest || forestCornerTiles.Contains(currentBaseTile)))
        {
            int randInt = Random.Range(0,ForestDecor.Length);
            decorTilemap.SetTile(pos, ForestDecor[randInt]);
        }
    }


    private void SetWaterDecor(float height, Tile tileToPut, float limitToUse, Vector3Int pos)
    {
        TileBase currentWaterTile = baseWaterTilemap.GetTile(pos);

        if (height < limitToUse && currentWaterTile == Water)
        {
            decorTilemap.SetTile(pos, tileToPut);
        }
    }


    private void ApplyBaseAutoTile(int x, int y)
    {
        Tile tile = CheckGrassWaterCorners(new Vector2Int(x, y));

        if (tile != null)
            baseWaterTilemap.SetTile(new Vector3Int(x, y), tile);
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
        if (x < 0 || x >= widthX || y < 0 || y >= heightY)
            return true;

        return baseWaterTilemap.GetTile(new Vector3Int(x, y)) != null;
    }
    // ---------------- WATER/GRASS AUTOTILE ----------------
    private Tile CheckGrassWaterCorners(Vector2Int pos)
    {
        TileBase current = baseWaterTilemap.GetTile(new Vector3Int(pos.x, pos.y));

        if (current == null)
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
            _ => null,
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




    //BUILDLIDNgGS



    private void GenerateVillage()
    {

        placedTentPositions.Clear();

        for (int i = 0; i < numberTentVillage; i++)
        {
            bool findVillageSpot = false;
            Vector3Int randPos = Vector3Int.zero;

            int maxAttempts = 1000;
            float currentMaxDistance = maxTentDistance;

            while (!findVillageSpot && maxAttempts-- > 0)
            {
                // Every 200 failed attempts, relax the max distance
                if (maxAttempts % 200 == 0 && currentMaxDistance < widthX)
                    currentMaxDistance *= 1.5f;

                randPos = new Vector3Int(Random.Range(0, widthX), Random.Range(0, heightY));
                findVillageSpot = CheckPlaceForTent(randPos, currentMaxDistance);
            }

            if (findVillageSpot)
            {
                placedTentPositions.Add(randPos);
                SetupTent(randPos);
            }
            else
            {
                Debug.LogWarning($"Could not place tent {i + 1}/{numberTentVillage} after max attempts.");
            }
        }


    }



    private bool CheckPlaceForTent(Vector3Int basePose, float currentMaxDistance)
    {
        Vector3Int randPos2 = new Vector3Int(basePose.x - 1, basePose.y);
        Vector3Int randPos3 = new Vector3Int(basePose.x - 1, basePose.y +1);
        Vector3Int randPos4 = new Vector3Int(basePose.x, basePose.y + 1);

        // grass check
        if (baseTilemap.GetTile(basePose) == null || baseTilemap.GetTile(randPos2) == null ||
            baseTilemap.GetTile(randPos3) == null || baseTilemap.GetTile(randPos4) == null)
            return false;

        // Biome must be null
        if (biomeTilemap.GetTile(basePose) != null || biomeTilemap.GetTile(randPos2) != null ||
            biomeTilemap.GetTile(randPos3) != null || biomeTilemap.GetTile(randPos4) != null)
            return false;

        if (baseWaterTilemap.GetTile(basePose) != null || baseWaterTilemap.GetTile(randPos2) != null ||
            baseWaterTilemap.GetTile(randPos3) != null || baseWaterTilemap.GetTile(randPos4) != null)
            return false;


        foreach (Vector3Int placed in placedTentPositions)
        {
            float dist = Vector3Int.Distance(basePose, placed);
            if (dist == 0 || dist < minTentDistance || dist > currentMaxDistance)
                return false;
        }

        return true;
    }

    private void SetupTent(Vector3Int pos)
    {
        int RandomTent = Random.Range(0, TentsVillage.Length);
        Vector3 worldPos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);
        Instantiate(TentsVillage[RandomTent], worldPos, Quaternion.identity, buildingsTilemap.transform);
    }


    private void GenerateVillageDecor()
    {
        if (placedTentPositions.Count == 0) return;

        int radius = Mathf.RoundToInt(villageDecorRadius);

        for (int i = 0; i < DecorVillage.Length; i++)
        {
            // Pick a random tent for each decor piece
            Vector3Int tentPos = placedTentPositions[Random.Range(0, placedTentPositions.Count)];

            List<Vector3Int> validPositions = new List<Vector3Int>();

            for (int x = tentPos.x - radius; x <= tentPos.x + radius; x++)
            {
                for (int y = tentPos.y - radius; y <= tentPos.y + radius; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y);

                    if (x < 0 || x >= widthX || y < 0 || y >= heightY) continue;
                    if (Vector3Int.Distance(pos, tentPos) > radius) continue;
                    if (baseTilemap.GetTile(pos) == null) continue;
                    if (baseWaterTilemap.GetTile(pos) != null) continue;
                    if (biomeTilemap.GetTile(pos) != null) continue;
                    if (decorTilemap.GetTile(pos) != null) continue;

                    validPositions.Add(pos);
                }
            }

            if (validPositions.Count == 0)
            {
                Debug.LogWarning($"No valid spot for village decor {i}.");
                continue;
            }

            Vector3Int chosen = validPositions[Random.Range(0, validPositions.Count)];
            decorTilemap.SetTile(chosen, DecorVillage[i]);
        }
    }


    private void PlacePlayerSpawnPoint()
    {
        if (placedTentPositions.Count == 0) return;

        Vector3 center = Vector3.zero;
        foreach (Vector3Int tent in placedTentPositions)
            center += new Vector3(tent.x + 0.5f, tent.y + 0.5f, 0);
        center /= placedTentPositions.Count;

        int maxAttempts = 500;
        float searchRadius = 1f;

        while (maxAttempts-- > 0)
        {
            if (maxAttempts % 100 == 0) searchRadius += 1f;

            int x = Mathf.RoundToInt(center.x + Random.Range(-searchRadius, searchRadius));
            int y = Mathf.RoundToInt(center.y + Random.Range(-searchRadius, searchRadius));

            Vector3Int checkPos = new Vector3Int(x, y);

            if (x < 0 || x >= widthX || y < 0 || y >= heightY) continue;
            if (baseTilemap.GetTile(checkPos) == null) continue;
            if (baseWaterTilemap.GetTile(checkPos) != null) continue;
            if (biomeTilemap.GetTile(checkPos) != null) continue;

            // Make sure it's not on top of a tent
            bool onTent = false;
            foreach (Vector3Int tent in placedTentPositions)
            {
                if (Vector3Int.Distance(checkPos, tent) < 2f)
                {
                    onTent = true;
                    break;
                }
            }
            if (onTent) continue;

            PlayerSpawnPoint.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
            return;
        }

        Debug.LogWarning("Could not find a valid player spawn point.");
    }
}