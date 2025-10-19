using System.Collections.Generic;
using UnityEngine;
using TTGJ.Framework;
using TTGJ.Interactable;
using TTGJ.UI;
using TTGJ.GamePlay;
using System;
using TTGJ.Plant;

namespace TTGJ.GamePlay
{
    [System.Serializable]
    public struct MoveKeyCode
    {
        public KeyCode forward;
        public KeyCode back;
        public KeyCode left;
        public KeyCode right;
    }
    public enum InteractionType
    {
        None,
        Lift,
        Harvest,
        Eat,
        Drop,
        Warning,
        Planting,
        NPC,
    }
    public class InputSystem : MonoSingleton<InputSystem>
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
        private LayerMask interactionLayers = ~0;
        [SerializeField]
        private CommandInfo commandInfoUI;
        private List<(KeyCode, string)> currentCommandinfo = new();
        private List<(KeyCode, string)> previewCommandinfo = new();



        private void Update()
        {
            CheckClickSpace();
            CheckPlayerDrop();
            CheckPlayerEat();
            CheckInteraction();
            RefreshCommandInfo();
            currentCommandinfo.Clear();
        }
        private void FixedUpdate()
        {
            CheckPlayerMove();
        }
        private void CheckClickSpace()
        {
            var result = GetBestAdaptorObj(CheckSpace);
            if (result.Item1 == null) { 
                return;
            }
            currentCommandinfo.Add((KeyCode.Space, result.Item2.ToString()));
            if (!Input.GetKeyDown(KeyCode.Space)) { 
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
            if (player.CanDrop())
            {
                currentCommandinfo.Add((KeyCode.Q, "Drop"));

            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                player.ToDrop();
            }
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
            if (direction != Vector3.zero)
            {
                player.ToMove(direction);
            }
        }

        private void CheckInteraction()
        {
            var result = GetBestAdaptorObj(CheckInteractable);
            if (result.Item1 == null) { 
                return;
            }
            currentCommandinfo.Add((KeyCode.J, result.Item2.ToString()));
            if (!Input.GetKeyDown(KeyCode.J)) { 
                return;
            }
            switch(result.Item2) { 
                case InteractionType.Warning:
                    _ = result.Item1.GetComponent<PlantBase>().OnWatering();
                    break;
                case InteractionType.Planting:
                    player.ToPlant(result.Item1.GetComponent<Field>());
                    break;
            }
        }
        private void CheckPlayerEat()
        {
            if (player.CanEat())
            {
                currentCommandinfo.Add((KeyCode.K, "Eat"));
            }
            if (!Input.GetKeyDown(KeyCode.K))
            {
                return;
            }
            player.ToEat();
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
        private (bool, InteractionType) CheckInteractable(Collider collider)
        {
            if(collider.TryGetComponent<PlantBase>(out var plant) 
               && (plant.GetCurrentState() == PlantState.Mature 
                  || plant.GetCurrentState() == PlantState.Germination)) { 
                return (true, InteractionType.Warning);
            }
            if(collider.CompareTag("Field") && collider.TryGetComponent<Field>(out var field) && !field.IsPlanted()) { 
                return (true, InteractionType.Planting);
            }
            if(collider.CompareTag("NPC")) { 
                return (true, InteractionType.NPC);
            }
            return (false, InteractionType.None);
        }
        private (Collider, InteractionType) GetBestAdaptorObj(Func<Collider,(bool, InteractionType)> checkInteractable)
        {
            Vector3 center = player.transform.position + player.transform.forward * 0.2f;
            Vector3 halfExtents = new Vector3(0.3f, 1f, 0.25f); // 对应 1 x 1.8 x 0.5 的盒子
            Collider[] colliders = Physics.OverlapBox(
                center,
                halfExtents,
                Quaternion.identity,
                interactionLayers,
                QueryTriggerInteraction.Ignore
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
            if (CheckCommandEqual()) {
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