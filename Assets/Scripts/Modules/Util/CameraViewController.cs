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
        
        [Tooltip("看向角色的高度偏移（比如查看头部）")] public float lookAtHeight = 1.5f;

        [Tooltip("旋转平滑度（0-1，值越小越平滑）")] [Range(0.1f, 1f)]
        public float rotationSmoothness = 0.2f;
        
        [Header("碰撞处理")]
        [Tooltip("相机碰撞检测的平滑度")]
        public float collisionSmoothness = 0.2f;
        [Tooltip("与障碍物保持的最小距离")]
        public float collisionOffset = 0.1f;

        // 旋转角度存储
        private float currentXRotation; // 垂直旋转（绕X轴）
        private float currentYRotation; // 水平旋转（绕Y轴）

        // 平滑后的旋转角度
        private float smoothXRotation;
        private float smoothYRotation;
        
        private float currentDistance; // 用于平滑处理的当前相机距离


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
            
            currentDistance = distanceFromTarget;

            // 隐藏鼠标光标（游戏中常用）
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // 1. 获取鼠标输入并更新旋转
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            currentYRotation += mouseX;
            currentXRotation -= mouseY;
            currentXRotation = Mathf.Clamp(currentXRotation, minVerticalAngle, maxVerticalAngle);

            // 2. 平滑旋转
            smoothXRotation = Mathf.Lerp(smoothXRotation, currentXRotation, rotationSmoothness / Time.deltaTime);
            smoothYRotation = Mathf.Lerp(smoothYRotation, currentYRotation, rotationSmoothness / Time.deltaTime);
            Quaternion targetRotation = Quaternion.Euler(smoothXRotation, smoothYRotation, 0);
            
            Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeight;

            // 3. 碰撞检测与距离计算
            float desiredDistance = CalculateDesiredDistance(targetRotation, lookAtPoint);

            // 4. 平滑相机距离
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, collisionSmoothness);

            // 5. 计算最终相机位置
            Vector3 finalPosition = lookAtPoint - targetRotation * Vector3.forward * currentDistance;

            // 6. 限制相机最低高度
            float minY = target.position.y + minCameraHeight;
            finalPosition.y = Mathf.Max(finalPosition.y, minY);

            // 7. 应用位置和旋转
            transform.position = finalPosition;
            transform.LookAt(lookAtPoint);
        }

        private float CalculateDesiredDistance(Quaternion targetRotation, Vector3 lookAtPoint)
        {
            // 计算理想的相机位置
            Vector3 idealPosition = lookAtPoint - targetRotation * Vector3.forward * distanceFromTarget;
            Vector3 directionToIdealPosition = idealPosition - lookAtPoint;
            
            RaycastHit hit;
            // 从观察点向理想相机位置发射射线
            if (Physics.Raycast(lookAtPoint, directionToIdealPosition.normalized, out hit, distanceFromTarget))
            {
                // 如果击中非自身的物体，则更新期望距离
                if (hit.collider.gameObject != target.gameObject)
                {
                    return hit.distance - collisionOffset;
                }
            }

            return distanceFromTarget;
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