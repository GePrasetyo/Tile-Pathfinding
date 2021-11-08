using System;

[Serializable]
public class MapSerializeJSON
{
    public int SizeX;
    public int SizeY;

    public TileJSON[] Tiles;

    public MapSerializeJSON(int x, int y, TileJSON[] ts)
    {
        SizeX = x;
        SizeY = y;
        Tiles = ts;
    }
}

[Serializable]
public class TileJSON
{
    public int X;
    public int Y;
    public int TypeIndex;
    public string TypeString;

    public TileJSON(int x, int y, int type, string typeCode)
    {
        X = x;
        Y = y;
        TypeIndex = type;
        TypeString = typeCode;
    }
}
