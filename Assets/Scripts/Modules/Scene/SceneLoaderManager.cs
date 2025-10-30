using System.Collections.Generic;
using TTGJ.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TTGJ.Scene
{
    public class SceneLoaderManager : Singleton<SceneLoaderManager>
    {
        private List<string> _loadedScenes = new List<string>();
        private string _currentSceneName = "StartScene";
        private List<int> _loadedSceneIndexes = new List<int>();
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
            GetFirstRoot(sceneName).SetActive(true);
            _currentSceneName = sceneName;
        }

        public void LoadScene(int sceneIndex)
        {
            if (_currentSceneIndex >= 0)
            {
                GetFirstRoot(_currentSceneIndex).SetActive(false);
            }
            if (!_loadedSceneIndexes.Contains(sceneIndex))
            {
                SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
                _loadedSceneIndexes.Add(sceneIndex);
            }
            GetFirstRoot(sceneIndex).SetActive(true);
            _currentSceneIndex = sceneIndex;
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
            if (!scene.IsValid() || !scene.isLoaded) return null;

            var roots = scene.GetRootGameObjects();
            return roots != null && roots.Length > 0 ? roots[0] : null;
        }
    }
}