using UnityEngine;

namespace TTGJ.Kitchen {
    public enum FoodQuality { 
        Failure,
        Normal,
        Good,
        Perfect,
    }
    public class Food : MonoBehaviour { 
        private int currentCookingProcess = 0;
        private FoodQuality foodQuality = FoodQuality.Failure;

        public int Cook(int cookingProcess) { 
            currentCookingProcess += cookingProcess;
            return currentCookingProcess;

        }

        public int GetCurrentCookingProcess() { 
            return currentCookingProcess;
        }

    }
}