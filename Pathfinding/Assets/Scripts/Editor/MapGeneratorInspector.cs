using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Assets.Scripts.Manager.MapGenerator))]
public class MapGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var theClass = (Assets.Scripts.Manager.MapGenerator)target;

        if (theClass.TileCollections)
        {
            GUIContent tileLabel = new GUIContent("Default Tile");
            var indexSelected = EditorGUILayout.Popup(tileLabel, theClass.DefaultTile, theClass.TileCollections.TileCodes.ToArray());

            theClass.DefaultTile = indexSelected;
            theClass.DefaultTileCode = theClass.TileCollections.TileCodes[indexSelected];
        }        

        EditorGUILayout.Space();        
        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Map"))
        {
            theClass.GenerateTileMap();
        }

        if (!theClass.OnPlay)
        {
            if (GUILayout.Button("Clear Map"))
            {
                theClass.ClearMap();
            }
        }        

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Save To JSON"))
        {
            theClass.SaveToJSON();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Load JSON Map"))
        {
            theClass.LoadJSON();
        }
    }
}
