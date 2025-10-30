using System.Collections.Generic;
using TTGJ.Audio;
using TTGJ.Framework;
using TTGJ.Generate;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TTGJ.Scene
{
    public class SceneLoaderManager : Singleton<SceneLoaderManager>
    {
        private List<string> _loadedScenes = new List<string>();
        private string _currentSceneName = "StartScene";
        private List<int> _loadedSceneIndexes = new List<int>() { 0};
        private int _currentSceneIndex = 0;

        public void LoadScene(string sceneName)
        {
            if (!string.IsNullOrEmpty(_currentSceneName))
            {
                GetFirstRoot(_currentSceneName).SetActive(false);
            }
            if (!_loadedScenes.Contains(sceneName))
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                _loadedScenes.Add(sceneName);
            }
            if(GetFirstRoot(sceneName) != null) {
                GetFirstRoot(sceneName).SetActive(true);
            }
            _currentSceneName = sceneName;
           
            
        }

        public void LoadScene(int sceneIndex)
        {
           if (GetFirstRoot(_currentSceneIndex) != null)
            {
                GetFirstRoot(_currentSceneIndex).SetActive(false);
            }
            if (!_loadedSceneIndexes.Contains(sceneIndex))
            {
                Debug.Log("LoadScene: dddd" + sceneIndex);
                SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
                _loadedSceneIndexes.Add(sceneIndex);
            }
            _currentSceneIndex = sceneIndex;
            if (GetFirstRoot(_currentSceneIndex) != null)
            {
                GetFirstRoot(_currentSceneIndex).SetActive(true);
            }

            if (sceneIndex == 0)
            { 
                AudioManager.Instance.PlayBGM(ResPathConfig.BGM_BGM_island);
            }
            else if (sceneIndex == 1)
            {
                AudioManager.Instance.PlayBGM(ResPathConfig.BGM_BGM_main);
            }
        }

        public GameObject GetFirstRoot(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid() || !scene.isLoaded) return null;

            var roots = scene.GetRootGameObjects();
            return roots != null && roots.Length > 0 ? roots[0] : null;
        }
        public GameObject GetFirstRoot(int sceneBuildIndex)
        {
            var scene = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);
            Debug.Log("GetFirstRoot: " + sceneBuildIndex + " " + scene.name);
            if (!scene.IsValid() || !scene.isLoaded) return null;
            Debug.Log("GetFirstRoot: " + scene.name);
            var roots = scene.GetRootGameObjects();
            Debug.Log("GetFirstRoot: " + roots.Length);
            return roots != null && roots.Length > 0 ? roots[0] : null;
        }
    }
}