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
        private void Update()
        {
            CheckPlayerLift();
            CheckPlayerDrop();
            //CheckPlayerMove();
        }
        private void FixedUpdate()
        {
            CheckPlayerMove();
        }
        private void CheckPlayerLift()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (player == null) return;
                Camera cam = Camera.main;
                if (cam == null) return;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, pickableLayers)
                    && hit.collider.gameObject.TryGetComponent<Liftable>(out var liftable))
                {
                    if (!liftable.IsLiftable()) return;
                    player.ToLift(hit.collider.gameObject);
                }
            }
        }
        private void CheckPlayerDrop()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.ToDrop();
            }
        }
        private void CheckPlayerMove()
        { 
            if (player == null) return;
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (direction != Vector3.zero)
            {
                player.ToMove(direction);
            }
        }
    }
}