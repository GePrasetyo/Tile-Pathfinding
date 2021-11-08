using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TileStyler : MonoBehaviour
{
    [HideInInspector] public int StyleIndex;
    [HideInInspector] public string StyleCode;

    //private TileStyle _thisStyle;

    public TileStyleCollection StyleData;

    void OnValidate()
    {
        //_thisStyle = StyleData.TileStyles[StyleIndex];
        SetupMaterial();        
    }

    public void ChangeToHighlight()
    {
        var highlightStyle = StyleData.Highlight;

        GetComponent<MeshFilter>().sharedMesh = highlightStyle.Mesh;
        GetComponent<MeshRenderer>().sharedMaterials = highlightStyle.Materials;
    }

    public void SetupMaterial()
    {
        GetComponent<MeshFilter>().sharedMesh = StyleData.TileStyles[StyleIndex].Mesh;
        GetComponent<MeshRenderer>().sharedMaterials = StyleData.TileStyles[StyleIndex].Materials;
    }
}
