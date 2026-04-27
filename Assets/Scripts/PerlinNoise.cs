using UnityEngine;

public static class PerlinNoise 
{
    public static float GetHeight(Vector2Int pos, SONoiseSettings s)
    {
        float value = 0f;
        float amplitude = 1f;
        float frequency = 1f;
        float maxValue = 0f;  

        for (int i = 0; i < s.octaves; i++)
        {
            float sampleX = (pos.x + s.offsetX) * s.scale * frequency;
            float sampleY = (pos.y + s.offsetY) * s.scale * frequency;

            value += Mathf.PerlinNoise(sampleX, sampleY) * amplitude;
            maxValue += amplitude;

            amplitude *= s.persistence; 
            frequency *= s.lacunarity;  
        }

        return value / maxValue;   
    }




}
