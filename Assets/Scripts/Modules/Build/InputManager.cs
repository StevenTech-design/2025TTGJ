using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TTGJ.Build
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private Camera sceneCamera;

        private Vector3 lastPosition;

        [SerializeField]
        private LayerMask placementLayermask;

        public event Action OnClicked, OnExit;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                OnClicked?.Invoke();
            if (Input.GetKeyDown(KeyCode.Escape))
                OnExit?.Invoke();
        }

        public bool IsPointerOverUI()
        {
            // 如果没有 EventSystem，则认为鼠标不在 UI 上
            if (EventSystem.current == null)
                return false;
                
            return EventSystem.current.IsPointerOverGameObject();
        }

        public Vector3 GetSelectedMapPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = sceneCamera.nearClipPlane;
            Ray ray = sceneCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, placementLayermask))
            {
                lastPosition = hit.point;
            }
            return lastPosition;
        }
    }

}