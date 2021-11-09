using UnityEngine;
using UnityEditor;
using Assets.Scripts.Component;


[CanEditMultipleObjects]
[CustomEditor(typeof(TileBaseComponent))]
public class TileBaseInspector : Editor
{
    void OnEnable()
    {

    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var theClasses = targets;
        var mainEditor = (TileBaseComponent)targets[0];

        GUIContent status = new GUIContent("Tile Status : " + mainEditor.TileStatus);
        EditorGUILayout.LabelField(status);

        EditorGUILayout.Space();
        GUIContent headerstyle = new GUIContent("Tile Style");
        EditorGUILayout.LabelField(headerstyle);


        if (mainEditor.TileTypes)
        {
            GUIContent tileLabel = new GUIContent("Tile Type");
            var indexSelected = EditorGUILayout.Popup(tileLabel, mainEditor.TypeIndex, mainEditor.TileTypes.TileCodes.ToArray());
            var prevSelected = 0;

            foreach (TileBaseComponent tc in theClasses)
            {
                prevSelected = tc.TypeIndex;

                tc.TypeIndex = indexSelected;
                tc.TypeCode = mainEditor.TileTypes.TileCodes[indexSelected];

                if(prevSelected != indexSelected)
                    tc.UpdateType();
            }
        }

        //EditorGUILayout.Space();
        //EditorGUILayout.Space();

        //if (GUILayout.Button("Update Tile"))
        //{
        //    foreach (TileBaseComponent tc in theClasses)
        //    {
        //        tc.UpdateType();
        //    }
        //}

        
    }
}
