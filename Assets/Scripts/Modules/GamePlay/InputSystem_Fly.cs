using System.Collections.Generic;
using UnityEngine;
using TTGJ.Framework;
using TTGJ.Interactable;
using TTGJ.UI;
using TTGJ.GamePlay;
using System;
using TTGJ.Plant;
using cfg;

namespace TTGJ.GamePlay
{
    [System.Serializable]

    public class InputSystem_Fly : MonoBehaviour
    {
        // 移动速度相关变量
        [Header("移动设置")]
        public float moveSpeed = 5f;           // 基础移动速度
        public float speedMultiplier = 1f;     // 速度倍数，可用于调整
        
        // 漂浮效果相关变量
        [Header("漂浮设置")]
        public float floatAmplitude = 0.5f;    // 漂浮幅度
        public float floatFrequency = 1f;      // 漂浮频率
        
        private Vector3 startPosition;         // 起始位置，用于计算漂浮
        private Rigidbody rb;                  // 刚体组件
        private CharacterController controller; // 字符控制器（备选方案）

        private void Start()
        {
            // 保存初始位置
            startPosition = transform.position;
            
            // 获取组件
            rb = GetComponent<Rigidbody>();
            controller = GetComponent<CharacterController>();
            
            // 如果有刚体，设置为动力学模式并冻结旋转
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        private void Update()
        {
            // 处理移动输入
            HandleMovementInput();
            
            // 应用漂浮效果
            ApplyFloatingEffect();
        }

        private void HandleMovementInput()
        {
            // 获取输入
            float horizontal = Input.GetAxis("Horizontal"); // 左右
            float vertical = Input.GetAxis("Vertical");   // 前后
            
            // 计算移动方向
            Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
            
            // 将移动方向从局部坐标转换为世界坐标
            moveDirection = transform.TransformDirection(moveDirection);
            
            // 应用移动
            if (controller != null)
            {
                // 使用CharacterController
                controller.SimpleMove(moveDirection * moveSpeed * speedMultiplier);
            }
            else if (rb != null)
            {
                // 使用Rigidbody
                Vector3 targetVelocity = moveDirection * moveSpeed * speedMultiplier;
                rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
            }
            else
            {
                // 使用Transform直接移动
                transform.Translate(moveDirection * moveSpeed * speedMultiplier * Time.deltaTime, Space.World);
            }
        }

        private void ApplyFloatingEffect()
        {
            // 计算漂浮偏移
            float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            
            // 应用漂浮效果
            if (controller != null)
            {
                // 对于CharacterController，我们需要特殊处理
                Vector3 floatPosition = startPosition + new Vector3(0, floatOffset, 0);
                Vector3 verticalOffset = floatPosition - transform.position;
                controller.Move(verticalOffset * Time.deltaTime);
            }
            else if (rb != null)
            {
                // 对于Rigidbody，设置目标Y位置
                Vector3 targetPosition = startPosition + new Vector3(0, floatOffset, 0);
                rb.MovePosition(new Vector3(transform.position.x, targetPosition.y, transform.position.z));
            }
            else
            {
                // 只修改Y轴位置，保留X和Z轴的移动
                Vector3 newPosition = transform.position;
                newPosition.y = startPosition.y + floatOffset;
                transform.position = newPosition;
            }
        }
        
        // 公开方法，用于从其他脚本调整速度
        public void SetMoveSpeed(float newSpeed)
        {
            moveSpeed = newSpeed;
        }
        
        // 公开方法，用于从其他脚本调整速度倍数
        public void SetSpeedMultiplier(float multiplier)
        {
            speedMultiplier = multiplier;
        }
    }
}