using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet planet;

    Editor shapeEditor;
    Editor colorEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();

            if(check.changed)
            {
                planet.GeneratePlanet();
            }
        }

        if(GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
        }

        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdate, ref shapeEditor, ref planet.shapeSettingsFoldout);
        DrawSettingsEditor(planet.colorSettings, planet.OnColorSettingsUpdated, ref colorEditor, ref planet.colorSettingsFoldout);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref Editor editor, ref bool foldout)
    {
        if (settings == null) return;

        foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            // Only draw the editor when foldout is true
            if (!foldout) return;

            CreateCachedEditor(settings, null, ref editor);
            editor.OnInspectorGUI();

            if(check.changed)
            {
                if(onSettingsUpdated != null)
                {
                    onSettingsUpdated.Invoke();
                }
            }
        }
    }

    private void OnEnable()
    {
        planet = (Planet)target;
    }
}
