using UnityEngine;
using UnityEngine.EventSystems;

namespace TTGJ.Build
{
    public class Building : MonoBehaviour, IPointerEnterHandler,
     IPointerExitHandler, IPointerClickHandler,IPointerDownHandler,IDragHandler
    {
        public Vector2 buildingSize = Vector2.one;
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("OnPointerEnter");
            //TODO:显示信息
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnDrag(PointerEventData eventData)
        {
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }
    }
}