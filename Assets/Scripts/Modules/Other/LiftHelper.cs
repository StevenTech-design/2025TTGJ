using System.Collections.Generic;
using TTGJ.Common;
using UnityEngine;
namespace TTGJ.Other {
    public class LiftHelper: MonoBehaviour { 
        private List<GameObject> lifts = new List<GameObject>();
        [SerializeField]
        private Transform liftBaseTransform;
        private float currentLiftHeight = 0;
        [SerializeField] private float throwForce = 1;
        [SerializeField] private UnityEngine.Vector3 throwAngleOffset = Vector3.zero;

        private void Awake() { 
            if (liftBaseTransform == null) {
                liftBaseTransform = transform;
            }
        }
        

        public void AddLift(GameObject lift) { 
            lift.transform.SetParent(liftBaseTransform);
            lift.transform.localPosition = new Vector3(0, currentLiftHeight, 0);
            lift.transform.localRotation = Quaternion.identity;
            lifts.Add(lift);
        }
        public void RemoveLift()
        {
            var lastLift = lifts[lifts.Count - 1];
            lastLift.transform.SetParent(null);
            lifts.RemoveAt(lifts.Count - 1);
            if (!lastLift.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                return;
            }
            var throwDirection = lastLift.transform.forward + throwAngleOffset;
            rigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }

        public void RemoveAllLifts() { 
            while(lifts.Count > 0) {
                RemoveLift();
            }
        }
    }
}