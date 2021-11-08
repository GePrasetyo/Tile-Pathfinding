using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileStyler))]
public class TileStylerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var theClass = (TileStyler)target;

        if (theClass.StyleData)
        {
            GUIContent tileLabel = new GUIContent("Tile Style");
            var indexSelected = EditorGUILayout.Popup(tileLabel, theClass.StyleIndex, theClass.StyleData.StyleCodes.ToArray());

            theClass.StyleIndex = indexSelected;
            theClass.StyleCode = theClass.StyleData.StyleCodes[indexSelected];
        }
    }
}
