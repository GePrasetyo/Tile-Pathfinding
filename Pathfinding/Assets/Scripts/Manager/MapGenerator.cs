using System;
using UnityEngine;
using Assets.Scripts.Component;
using System.Collections.Generic;


namespace Assets.Scripts.Manager
{
    public class MapGenerator : MonoBehaviour
    {
        public event Action<List<TileBaseComponent>, int, int> MapSizeUpdated;

        [SerializeField] private Camera _myCam;
        [SerializeField] private Transform _parentTile;
        [SerializeField] private TileBaseComponent _baseTile;

        [HideInInspector]public int DefaultTile;
        [HideInInspector]public string DefaultTileCode;

        public TileCollections TileCollections;

        [SerializeField] [Range(0, 40)] private int _tileXsize;
        [SerializeField] [Range(0, 40)] private int _tileYsize;
        [SerializeField] private string _mapName = "DefaultName";

        [SerializeField ]private List<TileBaseComponent> _tileDumpPool = new List<TileBaseComponent>();

        public bool OnPlay = false;

        void Start()
        {
            OnPlay = true;

            PathfindingManager.instance.StoreMap(_tileDumpPool, _tileXsize, _tileYsize);
            PathfindingManager.instance.CurrentTile = _tileDumpPool[0];
        }

        void OnApplicationQuit()
        {
            OnPlay = false;
        }

        public void GenerateTileMap(List<TileJSON> singleSetup = null)
        {
            if (_tileXsize <= 0 || _tileYsize <= 0)
                return;

            SetupCameraPost();
            ClearMap();
            
            List<TileBaseComponent> prevColumnList = new List<TileBaseComponent>();
            List<TileBaseComponent> currentColumnList = new List<TileBaseComponent>();

            for (int x = 0; x < _tileXsize; x++)
            {
                TileBaseComponent prevTile = null;

                for (int y = 0; y< _tileYsize; y++)
                {
                    var tileGenerated = Instantiate(_baseTile, new Vector3(x, 0, y), Quaternion.identity, _parentTile);

                    if (prevTile != null)
                    { 
                        prevTile.myNeighboor.Add(tileGenerated);
                        tileGenerated.myNeighboor.Add(prevTile);
                    }

                    if (prevColumnList.Count > y)
                    {
                        prevColumnList[y].myNeighboor.Add(tileGenerated);

                        tileGenerated.myNeighboor.Add(prevColumnList[y]);
                    }


                    tileGenerated.X = x;
                    tileGenerated.Y = y;

                    if (singleSetup == null)
                    {
                        tileGenerated.TypeIndex = DefaultTile;
                        tileGenerated.TypeCode = DefaultTileCode;
                    }
                    else
                    {
                        var setup = singleSetup.Find(t => t.X == x && t.Y == y);
                        
                        tileGenerated.TypeIndex = setup.TypeIndex;
                        tileGenerated.TypeCode = setup.TypeString;
                    }

                    
                    tileGenerated.InitType();

                    _tileDumpPool.Add(tileGenerated);

                    prevTile = tileGenerated;
                    currentColumnList.Add(tileGenerated);
                }
                prevColumnList.Clear();
                prevColumnList.AddRange(currentColumnList);

                currentColumnList.Clear();

            }

            MapSizeUpdated?.Invoke(_tileDumpPool, _tileXsize, _tileYsize);
        }

        public void ClearMap()
        {
            foreach (TileBaseComponent tc in _parentTile.GetComponentsInChildren<TileBaseComponent>())
                DestroyImmediate(tc.gameObject);

            _tileDumpPool.Clear();
        }

        public void SaveToJSON()
        {
            var JSONReady = SerializeMapToJson(_tileDumpPool.ToArray());            
            var stringReady = JsonUtility.ToJson(JSONReady);

            SavingTextFile.SaveMap(_mapName, stringReady);
        }

        public void LoadJSON()
        { 
            var loadedString = (TextAsset)Resources.Load("mapJSON/" + _mapName);
            var mapLoaded = JsonUtility.FromJson<MapSerializeJSON>(loadedString.ToString());

            SerializeJSONToMap(mapLoaded);
        }

        private void SetupCameraPost()
        {
            _myCam.orthographicSize = 1; //reset cam size

            var sizeX = _tileXsize / 3;
            var sizeY = _tileYsize / 2;

            if (sizeY > _myCam.orthographicSize)
                _myCam.orthographicSize = sizeY + 1;

            if (sizeX > _myCam.orthographicSize)
                _myCam.orthographicSize = sizeX;


            _myCam.transform.position = new Vector3((_tileXsize-1)/2f, 10f, (_tileYsize-1)/2f);

        }

        private MapSerializeJSON SerializeMapToJson(TileBaseComponent[] map)
        {
            List<TileJSON> tJSON = new List<TileJSON>();
            foreach (TileBaseComponent tbs in map)
            {
                tJSON.Add(new TileJSON(tbs.X, tbs.Y, tbs.TypeIndex, tbs.TypeCode));
            }

            return new MapSerializeJSON(_tileXsize, _tileYsize, tJSON.ToArray());
        }

        private void SerializeJSONToMap(MapSerializeJSON json)
        {
            _tileXsize = json.SizeX;
            _tileYsize = json.SizeY;

            var list = new List<TileJSON>();
            list.AddRange(json.Tiles);

            GenerateTileMap(list);
        }
    }
}