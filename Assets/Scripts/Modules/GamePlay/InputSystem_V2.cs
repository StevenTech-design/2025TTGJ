using System.Collections.Generic;
using UnityEngine;
using TTGJ.Framework;
using TTGJ.Interactable;
using TTGJ.UI;
using TTGJ.GamePlay;
using System;
using TTGJ.Plant;
using cfg;
using TTGJ.Data;

namespace TTGJ.GamePlay
{
    [System.Serializable]

    public class InputSystem_V2 : MonoSingleton<InputSystem_V2>
    {
        [SerializeField]
        private MoveKeyCode currentMoveKeyCode = new()
        {
            forward = KeyCode.W,
            back = KeyCode.S,
            left = KeyCode.A,
            right = KeyCode.D
        };
        [SerializeField]
        private PlayerController player;
        [SerializeField]
        private float longTimeDropTime = 1f;
        private float _pressDropTime = 0;
        [SerializeField]
        private LayerMask interactionLayers = ~0;
        [SerializeField]
        private CommandInfo commandInfoUI;
        private List<(KeyCode, string)> currentCommandinfo = new();
        private List<(KeyCode, string)> previewCommandinfo = new();
        private GameObject _currentHightlightObject = null;
        [SerializeField]
        private Color outlineColor = new Color(5f, 5f, 2.3f, 1f);
        [SerializeField]
        [Range(0f, 20f)]
        private float intensity = 1f;
        
        [SerializeField]
        private CameraViewController cameraController;
        [SerializeField]
        private Texture2D cursorSprite;


        private void Start()
        {

            if (cameraController == null)
            {
                cameraController = FindObjectOfType<CameraViewController>();
            }
            Cursor.SetCursor(cursorSprite, Vector2.zero, CursorMode.Auto);
            
        }
        
        private void Update()
        {
            if (GameRoot.Instance._gameState != GameState.Gaming) {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                UIManager.Instance.Push<JournalPanel>();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)) { 
                UIManager.Instance.Push<SettingPanel>();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { 
                UIManager.Instance.Push<ThanksPanel>();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { 
                UIManager.Instance.Push<ExitPanel>();
            }
            if (UIManager.Instance.GetUICount() > 0) {
                return;
            }

            CheckLiftOrHaverstObject();
            CheckPlayerDrop();
            CheckInteractLiftObject();
            //CheckPlayerEat();
            //CheckPlayerWatering();
            //CheckInteraction();
            RefreshCommandInfo();
            currentCommandinfo.Clear();
        }

        private void CheckInteractLiftObject() {
            GameObject liftObject = player.GetLiftObject();
            if (liftObject == null) { 
                return;
            }
            currentCommandinfo.Add((KeyCode.Mouse0,"使用"));
            if (!Input.GetMouseButtonDown(0)) { 
                return;
            }
            Field currentField = null;
            if (liftObject.TryGetComponent<PlantBase>(out var plant) && plant.GetCurrentState() == PlantState.Seed && CheckField(out currentField)) { 
                player.ToPlant(currentField);
                return;
            }else if (plant != null && plant.GetCurrentState() == PlantState.Seed) { 
                return;
            }
            player.ToEat();
        }

        private void FixedUpdate()
        {
            CheckPlayerMove();
        }
        private void CheckLiftOrHaverstObject()
        {
            var result = GetBestAdaptorObj(CheckSpace);
            if (result.Item1 == null) {
                UnhightlightTarget(_currentHightlightObject);
                _currentHightlightObject = null;
                return;
            }
            
            currentCommandinfo.Add((KeyCode.Space, "举起"));
            HightlightTarget(result.Item1.gameObject);
            if (!Input.GetKeyDown(KeyCode.Space))
            {
                return;
            }
            switch(result.Item2) { 
                case InteractionType.Lift:
                    player.ToLift(result.Item1.gameObject);
                    break;
                case InteractionType.Harvest:
                    result.Item1.GetComponent<PlantBase>().ChangeState(PlantState.Harvest);
                    break;
            }
        }
        private void CheckPlayerDrop()
        {
            if (!player.CanDrop())
            {
                return;
            }
            currentCommandinfo.Add((KeyCode.Mouse1, "丢出"));
            if(Input.GetMouseButtonDown(1)) { 
                _pressDropTime = Time.time;
                return;
            }
            if(!Input.GetMouseButtonUp(1)) { 
                return;
            }
            if(Time.time - _pressDropTime >= longTimeDropTime) { 
                player.LongTimeDrop();
                return;
            }
            player.ToDrop();
        }
        private void CheckPlayerMove()
        {
            if (player == null)
            {
                return;
            }

            Vector3 direction = Vector3.zero;

            if (Input.GetKey(currentMoveKeyCode.forward))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(currentMoveKeyCode.back))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(currentMoveKeyCode.left))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(currentMoveKeyCode.right))
            {
                direction += Vector3.right;
            }
            
            
            
            // 基于相机方向计算移动向量（W方向=相机前方向，D方向=相机右方向）
            Vector3 moveDir = (cameraController.GetCameraForward() * direction.z + 
                              cameraController.GetCameraRight() * direction.x).normalized;
            
             // player.ToMove(direction);
             
             if (moveDir.magnitude > 0.1f)  // 有输入时才移动
             {
             
                 player.ToMove(moveDir);  

                 
             }
             
             player.ToMove(moveDir);             
             
        }

        public MoveKeyCode GetMoveKeyCode()
        {
            return currentMoveKeyCode;
        }
        public void SetMoveKeyCode(MoveKeyCode keyCode)
        {
            currentMoveKeyCode = keyCode;
        }
        private (bool, InteractionType) CheckSpace(Collider collider)
        {
            if(collider.TryGetComponent<Liftable>(out var liftable) && liftable.CheckCanLift()) { 
                return (true, InteractionType.Lift);
            }
            if(collider.TryGetComponent<PlantBase>(out var plant)
               && plant.GetCurrentState() == PlantState.Mature) { 
                return (true, InteractionType.Harvest);
            }
            return (false, InteractionType.None);
        }
        private bool CheckField(out Field currentField) { 
            Ray ray = new Ray(player.transform.position + Vector3.up * 0.1f, -player.transform.up);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, interactionLayers)) { 
                if (hit.collider.TryGetComponent<Field>(out var field) && !field.IsPlanted()) { 
                    currentField = field;
                    return true;
                }
            }
            currentField = null;
            return false;
        }
        private (Collider, InteractionType) GetBestAdaptorObj(Func<Collider,(bool, InteractionType)> checkInteractable)
        {
            Vector3 center = player.transform.position + player.transform.forward * 0.2f;
            Vector3 halfExtents = new Vector3(0.3f, 1f, 0.5f); // 对应 1 x 1.8 x 0.5 的盒子
            Collider[] colliders = Physics.OverlapBox(
                center,
                halfExtents,
                Quaternion.identity,
                interactionLayers,
                QueryTriggerInteraction.Collide
            );

            Array.Sort(colliders, (a, b) => (a.transform.position - center).sqrMagnitude.CompareTo((b.transform.position - center).sqrMagnitude));
            foreach (var collider in colliders)
            {
               var checkResult = checkInteractable.Invoke(collider);
               if(checkResult.Item1) { 
                return (collider, checkResult.Item2);
               }
            }
            return (null, InteractionType.None);
        }

        private void RefreshCommandInfo()
        {
            if (CheckCommandEqual() || !DataManager.Instance.GetShowTipUIState()) {
                return;
            }
            commandInfoUI.ShowCommandInfo(currentCommandinfo);
            previewCommandinfo.Clear();
            previewCommandinfo.AddRange(currentCommandinfo);
        }
        private bool CheckCommandEqual() { 
            if (currentCommandinfo.Count != previewCommandinfo.Count) { 
                return false;
            }
            for (int i = 0; i < currentCommandinfo.Count; i++) { 
                if (currentCommandinfo[i].Item1 != previewCommandinfo[i].Item1) { 
                    return false;
                }
            }
            return true;
        }
        private void HightlightTarget(GameObject gameObject)
        {
            if(gameObject == null || gameObject == _currentHightlightObject) { 
                return;
            }
            UnhightlightTarget(_currentHightlightObject);
            var render = gameObject.GetComponentInChildren<Renderer>();
            if (render != null)
            {
                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                propertyBlock.SetColor("_OutLineColor", outlineColor * intensity);
                render.SetPropertyBlock(propertyBlock);
            }
            _currentHightlightObject = gameObject;
        }
        private void UnhightlightTarget(GameObject gameObject)
        {
            if(gameObject == null) { 
                return;
            }
            var render = gameObject.GetComponentInChildren<Renderer>();
            if (render != null)
            {
                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                propertyBlock.SetColor("_OutLineColor", new Color(0.3f, 0.3f, 0.3f, 1f));
                render.SetPropertyBlock(propertyBlock);
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 center = player.transform.position + player.transform.forward * 0.2f;
            Vector3 halfExtents = new Vector3(0.3f, 1f, 0.25f);

            Gizmos.color = Color.red;

            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawWireCube(center, halfExtents * 2f);
        }
    }
}