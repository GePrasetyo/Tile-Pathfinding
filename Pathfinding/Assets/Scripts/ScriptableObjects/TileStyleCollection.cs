using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public struct TileStyle
{
    public string StyleName;
    public Material[] Materials;
    public Mesh Mesh;
}

[CreateAssetMenu(fileName = "Tile Style Colelction", menuName = "Model/Tile Style")]
public class TileStyleCollection : ScriptableObject
{
    public List<string> StyleCodes;
    public List<TileStyle> TileStyles;

    public TileStyle Highlight;

    void OnValidate()
    {        
        if (!TileStyles.Any())
            return;

        for (int i = 0; i < TileStyles.Count; i++)
        {
            if (StyleCodes.Count <= i)
                StyleCodes.Add("");

            StyleCodes[i] = TileStyles[i].StyleName;
        }

        //Make Sure StyleCode length  = TileStyles
        if (StyleCodes.Count > TileStyles.Count)
        {
            var startIdx = TileStyles.Count;
            var trim = StyleCodes.Count - startIdx;

            StyleCodes.RemoveRange(startIdx, trim);
        }        
    }
}
