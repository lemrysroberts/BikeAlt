using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CityGenerator))]
public class CityGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CityGenerator generator = (CityGenerator)target;
        // Show default inspector property editor
        DrawDefaultInspector();

        if(GUILayout.Button("Generate"))
        {
            generator.Regenerate();
        }
    }
}
