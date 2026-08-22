#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Rocket))]
public class RocketEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        Rocket rocket = (Rocket)target;


        EditorGUILayout.LabelField("Rocket Info", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Speed:", rocket.speed + " Km/m");
        EditorGUILayout.LabelField("Height:", rocket.height + " M");

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Value:", rocket.baseValue + "$");
        EditorGUILayout.LabelField("Multiplier", rocket.multiplier + "x");

        float totalValue = rocket.baseValue * rocket.multiplier;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Total Value:", totalValue + "$");
    }
}
#endif
