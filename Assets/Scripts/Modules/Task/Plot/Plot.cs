using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TTGJ.Luban;
using UnityEngine;
namespace TTGJ.Task
{
    public class Plot : MonoBehaviour
    {
        public Action onPlotComplete;
        private List<int> plotIds = new List<int>();
        private int currentPlotIndex = 0;
        [SerializeField]
        private TMP_Text plotText;
        private Coroutine plotCoroutine;


        public void SetPlot(List<int> plotIds)
        {
            if (plotIds == null || plotIds.Count == 0)
            {
                onPlotComplete?.Invoke();
                return;
            }
            
            Debug.Log("SetPlot");
            this.plotIds = plotIds;
            plotText.text = "";
            gameObject.SetActive(true);
            currentPlotIndex = 0;
            if (plotCoroutine != null)
            {
                StopCoroutine(plotCoroutine);
            }
            plotCoroutine = StartCoroutine(StartPlot());
        }

        private IEnumerator StartPlot()
        {
            Debug.Log("StartPlot");
            while (currentPlotIndex < plotIds.Count)
            {
                Debug.Log("StartPlot: " + currentPlotIndex + " " + plotIds[currentPlotIndex]);
                var plot = LubanManager.Instance.GetPlot(plotIds[currentPlotIndex]);
                if (plot == null)
                {
                    yield break;
                }
                plotText.text = plot.TextTime;
                yield return new WaitForSeconds(plot.Time);
                currentPlotIndex++;
            }
            gameObject.SetActive(false);
            plotCoroutine = null;
            Debug.Log("Plot complete: " + plotIds.Count);
            Action onPlotComplete = this.onPlotComplete;
            this.onPlotComplete = null;
            onPlotComplete?.Invoke();
        }

        public void UpdateFont(TMP_FontAsset font)
        { 
            plotText.font = font;
        } 
        
    }
}