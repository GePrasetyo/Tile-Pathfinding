using UnityEditor;
using UnityEngine;

public class AltaUIInstance : Editor
{
    //static GameObject clickedObject;

    [MenuItem("GameObject/Alta Object/Tile", priority = 0)]
    public static void AddTile()
    {
        Create("tile");
    }

    private static GameObject Create(string objectName)
    {
        GameObject instance = Instantiate(Resources.Load<GameObject>(objectName));
        instance.name = objectName;

        //clickedObject = UnityEditor.Selection.activeObject as GameObject;
        //if (clickedObject != null)
        //{
        //    instance.transform.SetParent(clickedObject.transform, false);
        //}

        return instance;
    }
}
