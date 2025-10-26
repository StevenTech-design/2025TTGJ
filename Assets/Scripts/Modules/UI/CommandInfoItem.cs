using TMPro;
using TTGJ.Framework;
using TTGJ.Generate;
using UnityEngine;
using UnityEngine.UI;

public class CommandInfoItem : MonoBehaviour
{
	[SerializeField] private Image keyIcon;
	[SerializeField] private TMP_Text desText;

	public void Set(KeyCode key, string des)
	{
        if (keyIcon != null) { 
			var sprite = GetKeyIcon(key);
			Debug.Log("sprite: " + sprite);
			keyIcon.sprite = sprite;
		}
        if (desText != null) desText.text = des;
	}
    private Sprite GetKeyIcon(KeyCode key) { 
		switch(key) { 
			case KeyCode.Mouse0:
				return StResources.Instance.LoadByResources<Sprite>(ResPathConfig.Input_Mouse0);
			case KeyCode.Mouse1:
				return StResources.Instance.LoadByResources<Sprite>(ResPathConfig.Input_Mouse1);
			case KeyCode.Space:
				return StResources.Instance.LoadByResources<Sprite>(ResPathConfig.Input_Space);
		}
		return null;
	}

}