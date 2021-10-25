using UnityEngine;

[CreateAssetMenu()]
public class ColorSettings : ScriptableObject
{
    public Material material;
    public BiomeColorSettings biomeColorSettings;
    public Gradient oceanColor;

    [System.Serializable]
    public class BiomeColorSettings
    {
        public Biome[] biomes;
        public NoiseSettings noise;
        public float noiseStrength;
        public float noiseOffset;

        [Range(0f, 1f)]
        public float blendAmount;

        [System.Serializable]
        public class Biome
        {
            public Gradient gradient;
            public Color tint;

            [Range(0f, 1f)]
            public float startHeight;
            [Range(0f, 1f)]
            public float tintPercent;
        }
    }
}
