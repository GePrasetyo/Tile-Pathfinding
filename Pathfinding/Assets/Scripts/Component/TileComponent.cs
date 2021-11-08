using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Component
{
    public class TileComponent : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            var baseComponent = transform.GetComponentInParent<TileBaseComponent>();
            PathfindingManager.instance.UpdateTarget(baseComponent);
        }
    }
}