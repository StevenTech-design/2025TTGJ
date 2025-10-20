using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandInfoItem : MonoBehaviour
{
	[SerializeField] private TMP_Text keyCodeText;
	[SerializeField] private TMP_Text desText;

	public void Set(KeyCode key, string des)
	{
		if (keyCodeText != null) keyCodeText.text = key.ToString();
		if (desText != null) desText.text = des;
	}
}