using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoadGenerator))]
public class RoadGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoadGenerator generator = (RoadGenerator)target;
        // Show default inspector property editor
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            generator.Regenerate();
        }
    }
}
