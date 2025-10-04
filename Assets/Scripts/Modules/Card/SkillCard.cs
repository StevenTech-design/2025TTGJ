using System.Collections;
using System.Collections.Generic;
using cfg;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TTGJ.Luban;
using TTGJ.Framework;

public class SkillCard : MonoBehaviour
{
    [SerializeField] private TMP_Text skillCost;
    [SerializeField] private TMP_Text skillName;
    [SerializeField] private Image skillQuality;
    [SerializeField] private TMP_Text skillDescription;
    private move skillData;

    public void InitData(move data){
        skillData = data;
        RefreshData();

    }
    private async void RefreshData(){
        skillCost.text = skillData.Cost.ToString();
        skillName.text = skillData.Name;
        skillQuality.sprite = await StResources.Instance.LoadAsync<Sprite>(skillData.Quality);
        skillDescription.text = skillData.MoveTips;
    }
}
