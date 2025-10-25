using UnityEngine;

namespace TTGJ.GamePlay
{
    [RequireComponent(typeof(Camera))]
    public class CameraViewController : MonoBehaviour
    {
        [Header("目标设置")] [Tooltip("需要跟随的角色")] public Transform target; // 拖拽角色对象到这里

        [Header("相机参数")] [Tooltip("相机与角色的距离")] public float distanceFromTarget = 5f;
        [Tooltip("鼠标灵敏度")] public float mouseSensitivity = 100f;
        [Tooltip("垂直旋转最小角度（向下看的限制）")] public float minVerticalAngle = -30f;
        [Tooltip("垂直旋转最大角度（向上看的限制）")] public float maxVerticalAngle = 60f;
        
        [Tooltip("相机最低高度（相对于角色脚下，避免卡入地面，建议0.3-0.8m）")]
        public float minCameraHeight = 0.5f;  // 例如：最低高于角色脚下0.5米
        
        [Tooltip("看向角色的高度偏移（比如看向头部）")] public float lookAtHeight = 1.5f;

        [Tooltip("旋转平滑度（0-1，值越小越平滑）")] [Range(0.1f, 1f)]
        public float rotationSmoothness = 0.2f;

        // 旋转角度存储
        private float currentXRotation; // 垂直旋转（绕X轴）

        private float currentYRotation; // 水平旋转（绕Y轴）

        // 平滑后的旋转角度
        private float smoothXRotation;
        private float smoothYRotation;


        private void Start()
        {
            // 初始化旋转角度（基于初始相机位置）
            if (target != null)
            {
                Vector3 direction = transform.position - target.position;
                Quaternion initialRotation = Quaternion.LookRotation(direction);
                currentXRotation = initialRotation.eulerAngles.x;
                currentYRotation = initialRotation.eulerAngles.y;

                smoothXRotation = currentXRotation;
                smoothYRotation = currentYRotation;
            }

            // 隐藏鼠标光标（游戏中常用）
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // 1. 获取鼠标输入
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // 2. 更新旋转角度（水平旋转累加，垂直旋转限制范围）
            currentYRotation += mouseX; // 水平旋转（绕角色Y轴）
            currentXRotation -= mouseY; // 垂直旋转（绕X轴，减号使鼠标上移时相机上仰）
            currentXRotation = Mathf.Clamp(currentXRotation, minVerticalAngle, maxVerticalAngle);

            // 3. 平滑旋转（避免生硬跳动）
            smoothXRotation = Mathf.Lerp(smoothXRotation, currentXRotation, rotationSmoothness / Time.deltaTime);
            smoothYRotation = Mathf.Lerp(smoothYRotation, currentYRotation, rotationSmoothness / Time.deltaTime);

            // 4. 计算相机位置和旋转
            Quaternion targetRotation = Quaternion.Euler(smoothXRotation, smoothYRotation, 0);
            // 相机位置 = 角色位置 - 相机前方向 * 距离（确保相机在角色后方）
            Vector3 targetPosition = target.position - targetRotation * Vector3.forward * distanceFromTarget;

            // 5. 限制相机最低高度（核心：避免低于地面）
            // 角色脚下的Y坐标 + 最低高度限制 = 相机最低Y值
            float minY = target.position.y + minCameraHeight;
            targetPosition.y = Mathf.Max(targetPosition.y, minY);  // 确保相机Y轴不低于最低值
            
            // 5. 应用位置和旋转
            transform.position = targetPosition;
            transform.rotation = targetRotation;

            // 6. 确保相机看向角色的指定高度（比如头部）
            transform.LookAt(target.position + Vector3.up * lookAtHeight);

            // 可选：射线检测避免相机穿模（如果有障碍物，拉近相机）
            AvoidObstacles();
        }

        // 避免相机穿过场景物体（优化体验）
        private void AvoidObstacles()
        {
            RaycastHit hit;
            // 从角色到相机的射线
            Vector3 direction = transform.position - target.position;
            if (Physics.Raycast(target.position, direction.normalized, out hit, distanceFromTarget))
            {
                // 如果射线击中物体（非角色自身），调整相机位置到碰撞点前
                if (hit.collider.gameObject != target.gameObject)
                {
                    transform.position = hit.point + direction.normalized * 0.1f; // 0.1f是避免贴太近
                }
            }
        }

        // 供角色移动脚本获取相机前方向（水平面上）
        public Vector3 GetCameraForward()
        {
            Vector3 forward = transform.forward;
            forward.y = 0; // 忽略Y轴，确保在水平面上
            return forward.normalized;
        }

        // 供角色移动脚本获取相机右方向（水平面上）
        public Vector3 GetCameraRight()
        {
            Vector3 right = transform.right;
            right.y = 0; // 忽略Y轴，确保在水平面上
            return right.normalized;
        }
    }
}