using System;
using System.Collections;
using System.Collections.Generic;
using TTGJ.Entity;
using TTGJ.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TTGJ.Input { 
    public class InputSystem : MonoBehaviour
    {
        private InputController inputController;
        private PlayerEntity player;
        private AirShipEntity airship;
        
        private void Awake()
        {
            inputController = new InputController();
            inputController.Enable();
            inputController.Player.Enable();
            inputController.Airship.Disable();
        }

        private void Update()
        {
            // 获取Player实体
            if (player == null) {
                var players = EntityManager.Instance.GetEntitiesByType(EntityType.Player);
                if (players != null && players.Count > 0) {
                    player = players[0] as PlayerEntity;
                }
            }
            
            // 获取Airship实体
            if (airship == null) {
                var airships = EntityManager.Instance.GetEntitiesByType(EntityType.AirShip);
                if (airships != null && airships.Count > 0) {
                    airship = airships[0] as AirShipEntity;
                }
            }
            
            // 处理Player移动
            if (player != null && inputController.Player.enabled) {
                Vector2 moveInput = inputController.Player.Move.ReadValue<Vector2>();
                if (moveInput.magnitude > 0.1f) {
                    player.Move(moveInput);
                }
            }
            
            if (airship != null && inputController.Airship.enabled) {
                // 水平移动
                Vector2 horizontalInput = inputController.Airship.FlyHorizontal.ReadValue<Vector2>();
                
                // 垂直移动（上升/下降）
                float riseValue = inputController.Airship.FlyRise.ReadValue<float>();
                float fallValue = inputController.Airship.FlyFall.ReadValue<float>();
                float verticalInput = riseValue - fallValue;
                
                // 组合移动向量
                Vector3 moveDirection = new Vector3(horizontalInput.x, verticalInput, horizontalInput.y);
                
                if (moveDirection.magnitude > 0.1f) {
                    airship.Move(moveDirection);
                }
            }
        }
    }
}