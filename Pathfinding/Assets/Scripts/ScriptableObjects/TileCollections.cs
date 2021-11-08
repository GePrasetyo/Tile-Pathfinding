using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public struct TileModel
{
    public string TileName;
    public Assets.Scripts.Component.TileComponent TilePrefab;
    public TileStatusEnum BaseStatus;
}


//This could be inherit from one class, because TileStyleCollection also have the same flow, only different struct which could be solve by generic perhaps. (Will revisit this if still got time)
[CreateAssetMenu(fileName = "Tile Collections", menuName = "Model/Tile")]
public class TileCollections : ScriptableObject
{
    public List<string> TileCodes;
    public List<TileModel> TileModels;

    void OnValidate()
    {
        if (!TileModels.Any())
            return;

        for (int i = 0; i < TileModels.Count; i++)
        {
            if (TileCodes.Count <= i)
                TileCodes.Add("");

            TileCodes[i] = TileModels[i].TileName;
        }

        //Make Sure StyleCode length  = TileStyles
        if (TileCodes.Count > TileModels.Count)
        {
            var startIdx = TileModels.Count;
            var trim = TileCodes.Count - startIdx;

            TileCodes.RemoveRange(startIdx, trim);
        }
    }
}
