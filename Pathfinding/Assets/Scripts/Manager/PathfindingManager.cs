using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Component;

namespace Assets.Scripts.Manager
{
    public class PathfindingManager : MonoBehaviour
    {
        public static PathfindingManager instance;
        public MapGenerator MapGen;

        private List<TileBaseComponent> _tileArray = new List<TileBaseComponent>();
        private List<TileBaseComponent> _highlightedTile = new List<TileBaseComponent>();

        [SerializeField] private GameObject _avatar;
        private Transform myChar;

        public Action<TileBaseComponent> UpdateTarget;
        public Action TileUpdated;

        public TileBaseComponent CurrentTarget;
        public TileBaseComponent CurrentTile;

        private Vector2 _currentCoordinate;
        private Vector2 _sizeMap;

        void Awake()
        {
            instance = this;

            UpdateTarget += ChangeTarget;
            TileUpdated += RecalculatingTileUpdated;
            MapGen.MapSizeUpdated += MapChangedDuringGame;
        }

        void OnDisable()
        {
            UpdateTarget -= ChangeTarget;
            TileUpdated -= RecalculatingTileUpdated;
            MapGen.MapSizeUpdated -= MapChangedDuringGame;
        }

        void Start()
        {
            myChar = Instantiate(_avatar, Vector3.zero, Quaternion.identity).transform;
            
            _currentCoordinate = Vector2.zero;
        }

        /// <summary>
        /// Storing Map To Pathfinding Script
        /// </summary>
        /// <param name="tiles"> tiles </param>
        /// <param name="x"> size x of map</param>
        /// <param name="y"> size y of map</param>
        public void StoreMap(List<TileBaseComponent> tiles, int x, int y)
        {
            _tileArray.Clear();
            _tileArray.AddRange(tiles);
            _sizeMap = new Vector2(x, y);
        }

        private void ChangeTarget(TileBaseComponent target)
        {
            CurrentTarget = target;

            StopAllCoroutines();
            StartCoroutine(CalculatingPath());
        }

        private void RecalculatingTileUpdated()
        {
            StopAllCoroutines();
            if (CurrentTarget != null)
                StartCoroutine(CalculatingPath());
        }

        private void ClearHighlightedTile()
        {
            _highlightedTile.ForEach(tb => { tb.ResetCalculation(); tb.GetComponentInChildren<TileStyler>().SetupMaterial(); });
        }

        #region Pathfinding Calculation

        private void MapChangedDuringGame(List<TileBaseComponent> tiles, int x, int y)
        {
            StopCoroutine(CourotineWalking());

            Debug.LogWarning("-------------MAP SIZE CHANGED DURING GAME---------------");

            StoreMap(tiles, x, y);

            CurrentTile = FindMyCurrentTile();
        }

        private TileBaseComponent FindMyCurrentTile()
        {
            if (_tileArray.Exists(t => t.X == _currentCoordinate.x && t.Y == _currentCoordinate.y))
                return _tileArray.Find((t) => t.X == _currentCoordinate.x && t.Y == _currentCoordinate.y);
            else
                return _tileArray[0];
        }

        /// <summary>
        /// Algorithm Reference : https://brilliant.org/wiki/a-star-search/
        /// </summary>
        IEnumerator CalculatingPath()
        {
            StopCoroutine(CourotineWalking());
            ClearHighlightedTile();

            if (CurrentTile == null)
                CurrentTile = FindMyCurrentTile();

            //itenary list
            var openList = new List<TileBaseComponent>();
            openList.Add(CurrentTile);
            CurrentTile.CalculateEstimate(CurrentTarget.X, CurrentTarget.Y);

            var closedList = new List<TileBaseComponent>();
            var arrived = false;

            while (!arrived)
            {
                yield return WaitForSecondsCache.WAIT_END_FRAME;
                //sort the tile with the lowest "total estimate" cost in the itenary list
                if (!openList.Any()) //No Possible Path
                {
                    arrived = true;
                    Debug.LogError("------NO PATH FOUND-----");
                    break;
                }

                var latestPath = openList.OrderBy(t => t.TotalEstimated).First();

                if (latestPath == CurrentTarget)
                {
                    arrived = true;

                    _highlightedTile.Clear();

                    var checkTile = latestPath;
                    var tracingComplete = false;

                    //tracing back step
                    while (!tracingComplete)
                    {
                        checkTile.GetComponentInChildren<TileStyler>().ChangeToHighlight();
                        _highlightedTile.Add(checkTile);
                        
                        if (checkTile.ThePreviousStep == null)
                            tracingComplete = true;
                        else
                            checkTile = checkTile.ThePreviousStep;
                    };

                    StartCoroutine(CourotineWalking());
                    break;
                }
                else
                {
                    closedList.Add(latestPath);
                    openList.Remove(latestPath);

                    var neighborsTile = GetNeighbors(latestPath);

                    foreach (var nTile in neighborsTile)
                    {
                        //We just visited this tile.
                        if (closedList.Contains(nTile))
                            continue;

                        //This to straighten the route.
                        if (openList.Contains(nTile))
                        {
                            if (nTile.TotalEstimated < latestPath.TotalEstimated)
                            {
                                openList.Remove(nTile);
                                openList.Add(nTile);
                            }
                        }
                        else
                        {
                            //Add it to the route itenary
                            nTile.ThePreviousStep = latestPath;
                            openList.Add(nTile);
                        }
                    }
                }
            }

        }

        private List<TileBaseComponent> GetNeighbors(TileBaseComponent currentCheckTile)
        {
            var neighboors = currentCheckTile.myNeighboor;

            var cX = currentCheckTile.X;
            var cY = currentCheckTile.Y;

            var tX = CurrentTarget.X;
            var tY = CurrentTarget.Y;

            foreach (TileBaseComponent tb in neighboors)
            {
                tb.Cost = currentCheckTile.Cost + 1;
                tb.CalculateEstimate(tX, tY);
            }

            return neighboors.Where(nT => nT.TileStatus != TileStatusEnum.Block && nT.TileStatus != TileStatusEnum.None).ToList();
        }
        #endregion

        IEnumerator CourotineWalking()
        {
            yield return WaitForSecondsCache.HALF_SEC_WAIT;

            var stepCount = _highlightedTile.Count;            

            for (int i = stepCount-1; i >= 0; i--)
            {
                if (_highlightedTile.Count - 1 < i)
                    break;

                CurrentTile = _highlightedTile[i];

                if (!CurrentTile)
                    break;

                _currentCoordinate = new Vector2(CurrentTile.X , CurrentTile.Y);
                CurrentTile.GetComponentInChildren<TileStyler>().SetupMaterial();

                CurrentTile.ResetCalculation();

                var startTime = Time.time;
                var journeyLength = Vector3.Distance(myChar.position, _highlightedTile[i].transform.position);
                var targetPos = _highlightedTile[i].transform.position;

                //https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html
                while (myChar.position != targetPos)
                {
                    float distCovered = (Time.time - startTime) * 1f;
                    float fractionOfJourney = distCovered / journeyLength;

                    myChar.position = Vector3.Lerp(myChar.position, targetPos, fractionOfJourney);

                    yield return WaitForSecondsCache.WAIT_END_FRAME;
                }
            }

            _highlightedTile.Clear();
        }
    }
}