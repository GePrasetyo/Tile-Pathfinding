using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Component
{
    public class TileBaseComponent : MonoBehaviour
    {
        public TileCollections TileTypes;

        [HideInInspector] public int TypeIndex;
        [HideInInspector] public string TypeCode;
        
        private TileModel _model;

        [HideInInspector] public TileStatusEnum TileStatus;

        //A* Variables
        /*[HideInInspector]*/ public int X;
        /*[HideInInspector]*/ public int Y;
        /*[HideInInspector]*/ public int Cost;
        /*[HideInInspector]*/ public int Heuristic;
        public TileBaseComponent ThePreviousStep;

        [HideInInspector] public int TotalEstimated => Cost+Heuristic;


        public List<TileBaseComponent> myNeighboor;

        public void InitType()
        {
            _model = TileTypes.TileModels[TypeIndex];
            var component = Instantiate(_model.TilePrefab, this.transform);

            TileStatus = _model.BaseStatus;
        }

        public void UpdateType()
        {
            foreach (TileComponent t in this.GetComponentsInChildren<TileComponent>())
                DestroyImmediate(t.gameObject);

            InitType();

            if (Assets.Scripts.Manager.PathfindingManager.instance)
                Assets.Scripts.Manager.PathfindingManager.instance.TileUpdated?.Invoke();
        }

        public void CalculateEstimate(int targetX, int targetY)
        {
            Heuristic = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
            
        }

        public void ResetCalculation()
        {
            Cost = 0;
            Heuristic = 0;
            ThePreviousStep = null;
        }
    }
}