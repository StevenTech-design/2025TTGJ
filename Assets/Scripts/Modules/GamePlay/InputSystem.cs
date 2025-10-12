using UnityEngine;

namespace TTGJ.GamePlay
{
    public class InputSystem : MonoBehaviour
    {
        [SerializeField]
        private PlayerController player;
        [SerializeField]
        private LayerMask pickableLayers = ~0; // 默认全部
        [SerializeField]
        private float rayDistance = 100f;
        private void Update() {
            if (Input.GetMouseButtonDown(0)) { 
                if (player == null) return;
                Camera cam = Camera.main;
                if (cam == null) return;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, pickableLayers, QueryTriggerInteraction.Ignore))
                {
                    player.ToLift(hit.collider.gameObject);
                }
            }
        }
        private void FixedUpdate() { 
            if (player == null) return;
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            player.ToMove(direction);
        }
    }
}