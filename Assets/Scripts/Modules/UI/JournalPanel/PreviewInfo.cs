using System.Collections;
using System.Collections.Generic;
using TMPro;
using TTGJ.Common;
using TTGJ.Framework;
using TTGJ.Luban;
using TTGJ.Plant;
using UnityEngine;
using UnityEngine.UI;

namespace TTGJ.UI
{
    public class PreviewInfo : MonoBehaviour
    {
        [SerializeField]
        private Image icon;
        [SerializeField]
        private TMP_Text _name;
        [SerializeField]
        private TMP_Text _description;

        [SerializeField]
        private GameObject _modelRoot;

        public void SetInfo(int itemId) { 
            var item = LubanManager.Instance.GetItemNew(itemId);
            if(item == null) {
                return;
            }
            icon.sprite = StResources.Instance.LoadByResources<Sprite>(item.Icon);
            _name.text = item.Name;
            _description.text = item.ItemTip;
            GameObject model = Instantiate(StResources.Instance.LoadByResources<GameObject>(item.Model), _modelRoot.transform);
            if(model == null) {
                return;
            }
            if (model.TryGetComponent<Rigidbody>(out var rigidbody)) { 
                Destroy(rigidbody);
            }
            if (model.TryGetComponent<Collider>(out var collider)) { 
                Destroy(collider);
            }
            model.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            LayerUtility.ChangeLayerRecursively(model, LayerMask.NameToLayer("RenderModel"));
        }


    }
}
