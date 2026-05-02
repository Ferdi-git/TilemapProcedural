using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using UnityEngine;
using UnityEngine.UI;

public class NoiseTexturePreview : MonoBehaviour
{
    [SerializeField] SONoiseSettings noiseSettings;


    [PreviewField(256)]
    public Texture2D noiseTexture;

    void Start() => Generate();

    [Button]
    public void Generate()
    {
        noiseTexture = new Texture2D(100, 100);
        Color[] pixels = new Color[100 * 100];

        for (int x = 0; x < 100; x++)
            for (int y = 0; y < 100; y++)
            {
                float value = PerlinNoise.GetHeight(new Vector2Int(x, y), noiseSettings);
                pixels[y * 100 + x] = new Color(value, value, value);
            }

        noiseTexture.SetPixels(pixels);
        noiseTexture.Apply();

    }
}
