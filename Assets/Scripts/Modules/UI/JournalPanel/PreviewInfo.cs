using System.Collections;
using System.Collections.Generic;
using TMPro;
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
            if(model != null) {
                GameObject go = Instantiate(model, _modelRoot.transform);
                go.transform.localScale = new Vector3(item.ModelSize, item.ModelSize, item.ModelSize);
                go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }


    }
}
