using UnityEngine;

public class ColorGenerator
{
    ColorSettings settings;
    Texture2D texture;
    const int textureResolution = 50;
    INoiseFilter biomeNoiseFilter;

    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        if (texture == null || texture.height != settings.biomeColorSettings.biomes.Length)
        {
            // First half of the texture is the ocean, the other half is the rest
            texture = new Texture2D(textureResolution * 2, settings.biomeColorSettings.biomes.Length, TextureFormat.RGBA32, false);
        }

        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
    }
    
    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.material.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    /// <summary>
    /// Returns value of 0 when the point is in the first biome, 1 if the point is in the last biome, and anything in between 0-1 for other biomes
    /// </summary>
    /// <param name="pointOnUnitSphere"> Point on planet </param>
    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
        heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - settings.biomeColorSettings.noiseOffset) * settings.biomeColorSettings.noiseStrength;

        float biomeIndex = 0;
        int numBiomes = settings.biomeColorSettings.biomes.Length;
        float blendRange = settings.biomeColorSettings.blendAmount / 2f + .001f;  // Adding small amout, otherwise if this is 0, the InverseLerp won't work nicely

        for(int i = 0; i < numBiomes; i++)
        {
            float dist = heightPercent - settings.biomeColorSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dist);
            biomeIndex *= (1 - weight);
            biomeIndex += i * weight;
        }

        return biomeIndex / Mathf.Max(1, (numBiomes - 1));
    }

    public void UpdateColors()
    {
        Color[] colors = new Color[texture.width * texture.height];

        int colorIndex = 0;
        foreach(var biome in settings.biomeColorSettings.biomes)
        {
            for (int i = 0; i < textureResolution * 2; i++)
            {
                Color gradientColor;

                // If this is true, then getting color for ocean, otherwise for biomes
                if(i < textureResolution)
                {
                    gradientColor = settings.oceanColor.Evaluate(i / (textureResolution - 1f));
                }
                else
                {
                    gradientColor = biome.gradient.Evaluate((i - textureResolution) / (textureResolution - 1f));
                }
                
                Color tintColor = biome.tint;
                colors[colorIndex] = gradientColor * (1 - biome.tintPercent) + tintColor * biome.tintPercent;
                colorIndex++;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();
        settings.material.SetTexture("_texture", texture);
    }
}
