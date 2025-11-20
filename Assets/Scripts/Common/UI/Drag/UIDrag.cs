using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TTGJ.UI
{
    public class UIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        protected Canvas canvas;
        protected Transform _preParent;

        protected CanvasGroup canvasGroup;
        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            //transform.position = Input.mousePosition;
            
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
           _preParent = transform.parent;
           transform.SetParent(canvas.transform);
           canvasGroup.blocksRaycasts = false;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!CanPlace(eventData))
            {
                transform.SetParent(_preParent);
                transform.localPosition = Vector3.zero;
                canvasGroup.blocksRaycasts = true;
                return;
            }
            transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform);
            transform.localPosition = Vector3.zero;
            canvasGroup.blocksRaycasts = true;
        }


        protected virtual bool CanPlace(PointerEventData eventData)
        {
            if (eventData == null || eventData.pointerCurrentRaycast.gameObject == null)
            {
                return false;
            }

            return eventData.pointerCurrentRaycast.gameObject.GetComponent<UIBox>() != null;
        }
    }
}