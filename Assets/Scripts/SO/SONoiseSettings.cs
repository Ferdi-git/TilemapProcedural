using UnityEngine;

[CreateAssetMenu]
public class SONoiseSettings : ScriptableObject 
{
    public int offsetX = 0;
    public int offsetY = 0; 
    
    public float scale = 0.05f;

    [Range(1, 8)]
    public int octaves = 4; 

    [Range(0f, 1f)]
    public float persistence = 0.5f; 

    [Range(1f, 4f)]
    public float lacunarity = 2f;    
}
