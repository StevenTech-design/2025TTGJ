using System.Collections;
using TTGJ.QTE;
using UnityEngine;

namespace TTGJ.Kitchen {
    public class Kitchenware : MonoBehaviour,IInteractable {
        [SerializeField] private int cookingSpeed = 1;
        private Food currentFood = null;

        public void PlaceFood(Food food) {
            if (currentFood != null) { 
                Debug.LogWarning("Kitchenware: 已经有食物了");
                return;
            }
            currentFood = food;
            StartCoroutine(CookingFood());
        }

        public bool CheckCanInteract()
        {
            return currentFood != null;
        }

        public KeyCode GetInteractKeyCode()
        {
            return KeyCode.Space;
        }

        public void Interact()
        {
           QTESystem.Instance.StartQTE((result) => {
            if(result == QTEType.Perfect) {
                int process =currentFood.Cook(GetQTEProcess(result));
                Debug.Log($"Cooking process: {process}");
            }
           });
        }
        private int GetQTEProcess(QTEType result) { 
            switch(result) {
                case QTEType.Perfect:
                    return 3;
                case QTEType.Hit:
                    return 2;
                case QTEType.Failure:
                    return 0;
                default:
                    return 0;
            }
        }

        private IEnumerator CookingFood() {
            while (currentFood != null && currentFood.GetCurrentCookingProcess() < 100)
            {
                yield return new WaitForSeconds(1);
                currentFood.Cook(cookingSpeed);
            }
        }
    }
}