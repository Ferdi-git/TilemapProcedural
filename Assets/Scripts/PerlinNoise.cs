using UnityEngine;

public static class PerlinNoise 
{
    public static float GetHeight(Vector2Int pos, SONoiseSettings settings)
    {
        float height = Mathf.PerlinNoise(
            (pos.x + settings.offset) * settings.scale,
            (pos.y + settings.offset) * settings.scale
        );
         return height;
            
    }




}
