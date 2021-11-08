using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Assets.Scripts.Component.TileBaseComponent))]
public class TileBaseInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var theClass = (Assets.Scripts.Component.TileBaseComponent)target;

        GUIContent status = new GUIContent("Tile Status : " +theClass.TileStatus);
        EditorGUILayout.LabelField(status);

        if (theClass.TileTypes)
        {
            GUIContent tileLabel = new GUIContent("Tile Type");
            var indexSelected = EditorGUILayout.Popup(tileLabel, theClass.TypeIndex, theClass.TileTypes.TileCodes.ToArray());

            theClass.TypeIndex = indexSelected;
            theClass.TypeCode = theClass.TileTypes.TileCodes[indexSelected];

            
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Update Tile"))
        {
            theClass.UpdateType();
        }
    }
}
