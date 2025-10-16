using UnityEngine;

namespace TTGJ.Common
{
    /// <summary>
    /// Layer utility for managing Unity layers and layer masks
    /// </summary>
    public static class LayerUtility
    {
        #region Layer Name Constants
        
        public const string DEFAULT_LAYER = "Default";
        public const string TRANSPARENT_FX = "TransparentFX";
        public const string IGNORE_RAYCAST = "Ignore Raycast";
        public const string WATER = "Water";
        public const string UI = "UI";
        public const string PLAYER = "Player";
        public const string GROUND = "Ground";
        public const string INTERACTABLE = "Interactable";
        public const string PICKABLE = "Pickable";
        
        #endregion
        
        #region Layer Management
        
        /// <summary>
        /// Change the layer of a GameObject and all its children
        /// </summary>
        /// <param name="gameObject">Target GameObject</param>
        /// <param name="layerName">Target layer name</param>
        public static void ChangeLayer(GameObject gameObject, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            ChangeLayer(gameObject, layer);
        }
        
        /// <summary>
        /// Change the layer of a GameObject and all its children
        /// </summary>
        /// <param name="gameObject">Target GameObject</param>
        /// <param name="layer">Target layer index</param>
        public static void ChangeLayer(GameObject gameObject, int layer)
        {
            if (gameObject == null)
            {
                Debug.LogWarning("LayerUtility: GameObject is null");
                return;
            }
            
            if (layer < 0 || layer > 31)
            {
                Debug.LogWarning($"LayerUtility: Invalid layer index {layer}. Must be between 0 and 31");
                return;
            }
            
            gameObject.layer = layer;
        }
        
        /// <summary>
        /// Change the layer of a GameObject and all its children recursively
        /// </summary>
        /// <param name="gameObject">Target GameObject</param>
        /// <param name="layerName">Target layer name</param>
        /// <param name="includeChildren">Include children in the layer change</param>
        public static void ChangeLayerRecursively(GameObject gameObject, string layerName, bool includeChildren = true)
        {
            int layer = LayerMask.NameToLayer(layerName);
            ChangeLayerRecursively(gameObject, layer, includeChildren);
        }
        
        /// <summary>
        /// Change the layer of a GameObject and all its children recursively
        /// </summary>
        /// <param name="gameObject">Target GameObject</param>
        /// <param name="layer">Target layer index</param>
        /// <param name="includeChildren">Include children in the layer change</param>
        public static void ChangeLayerRecursively(GameObject gameObject, int layer, bool includeChildren = true)
        {
            if (gameObject == null)
            {
                Debug.LogWarning("LayerUtility: GameObject is null");
                return;
            }
            
            gameObject.layer = layer;
            
            if (includeChildren)
            {
                Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in children)
                {
                    if (child != gameObject.transform)
                    {
                        child.gameObject.layer = layer;
                    }
                }
            }
        }
        
        #endregion
        
        #region Layer Mask Utilities
        
        /// <summary>
        /// Create a LayerMask from layer names
        /// </summary>
        /// <param name="layerNames">Array of layer names</param>
        /// <returns>LayerMask containing the specified layers</returns>
        public static LayerMask CreateLayerMask(params string[] layerNames)
        {
            LayerMask layerMask = 0;
            foreach (string layerName in layerNames)
            {
                int layer = LayerMask.NameToLayer(layerName);
                if (layer >= 0)
                {
                    layerMask |= (1 << layer);
                }
                else
                {
                    Debug.LogWarning($"LayerUtility: Layer '{layerName}' does not exist");
                }
            }
            return layerMask;
        }
        
        /// <summary>
        /// Create a LayerMask from layer indices
        /// </summary>
        /// <param name="layerIndices">Array of layer indices</param>
        /// <returns>LayerMask containing the specified layers</returns>
        public static LayerMask CreateLayerMask(params int[] layerIndices)
        {
            LayerMask layerMask = 0;
            foreach (int layer in layerIndices)
            {
                if (layer >= 0 && layer <= 31)
                {
                    layerMask |= (1 << layer);
                }
                else
                {
                    Debug.LogWarning($"LayerUtility: Invalid layer index {layer}. Must be between 0 and 31");
                }
            }
            return layerMask;
        }
        
        /// <summary>
        /// Check if a layer is included in a LayerMask
        /// </summary>
        /// <param name="layerMask">LayerMask to check</param>
        /// <param name="layer">Layer index to check</param>
        /// <returns>True if the layer is included in the mask</returns>
        public static bool IsLayerInMask(LayerMask layerMask, int layer)
        {
            return (layerMask.value & (1 << layer)) != 0;
        }
        
        /// <summary>
        /// Check if a GameObject's layer is included in a LayerMask
        /// </summary>
        /// <param name="layerMask">LayerMask to check</param>
        /// <param name="gameObject">GameObject to check</param>
        /// <returns>True if the GameObject's layer is included in the mask</returns>
        public static bool IsInLayerMask(LayerMask layerMask, GameObject gameObject)
        {
            if (gameObject == null) return false;
            return IsLayerInMask(layerMask, gameObject.layer);
        }
        
        #endregion
        
        #region Camera Layer Management
        
        /// <summary>
        /// Set the culling mask of the main camera
        /// </summary>
        /// <param name="layerMask">LayerMask to set</param>
        public static void SetMainCameraRenderLayers(LayerMask layerMask)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.cullingMask = layerMask;
                Debug.Log($"LayerUtility: Main camera culling mask set to {layerMask.value}");
            }
            else
            {
                Debug.LogWarning("LayerUtility: Main camera not found");
            }
        }
        
        /// <summary>
        /// Set the culling mask of a specific camera
        /// </summary>
        /// <param name="camera">Target camera</param>
        /// <param name="layerMask">LayerMask to set</param>
        public static void SetCameraRenderLayers(Camera camera, LayerMask layerMask)
        {
            if (camera != null)
            {
                camera.cullingMask = layerMask;
                Debug.Log($"LayerUtility: Camera '{camera.name}' culling mask set to {layerMask.value}");
            }
            else
            {
                Debug.LogWarning("LayerUtility: Camera is null");
            }
        }
        
        /// <summary>
        /// Add layers to the main camera's culling mask
        /// </summary>
        /// <param name="layerNames">Layer names to add</param>
        public static void AddLayersToMainCamera(params string[] layerNames)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                LayerMask additionalLayers = CreateLayerMask(layerNames);
                mainCamera.cullingMask |= additionalLayers;
                Debug.Log($"LayerUtility: Added layers {string.Join(", ", layerNames)} to main camera");
            }
            else
            {
                Debug.LogWarning("LayerUtility: Main camera not found");
            }
        }
        
        /// <summary>
        /// Remove layers from the main camera's culling mask
        /// </summary>
        /// <param name="layerNames">Layer names to remove</param>
        public static void RemoveLayersFromMainCamera(params string[] layerNames)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                LayerMask layersToRemove = CreateLayerMask(layerNames);
                mainCamera.cullingMask &= ~layersToRemove;
                Debug.Log($"LayerUtility: Removed layers {string.Join(", ", layerNames)} from main camera");
            }
            else
            {
                Debug.LogWarning("LayerUtility: Main camera not found");
            }
        }
        
        #endregion
        
        #region Debug Utilities
        
        /// <summary>
        /// Get the name of a layer by its index
        /// </summary>
        /// <param name="layer">Layer index</param>
        /// <returns>Layer name or "Unknown" if not found</returns>
        public static string GetLayerName(int layer)
        {
            return LayerMask.LayerToName(layer);
        }
        
        /// <summary>
        /// Get the index of a layer by its name
        /// </summary>
        /// <param name="layerName">Layer name</param>
        /// <returns>Layer index or -1 if not found</returns>
        public static int GetLayerIndex(string layerName)
        {
            return LayerMask.NameToLayer(layerName);
        }
        
        /// <summary>
        /// Check if a layer exists
        /// </summary>
        /// <param name="layerName">Layer name to check</param>
        /// <returns>True if the layer exists</returns>
        public static bool LayerExists(string layerName)
        {
            return LayerMask.NameToLayer(layerName) >= 0;
        }
        
        /// <summary>
        /// Log all active layers in a LayerMask
        /// </summary>
        /// <param name="layerMask">LayerMask to analyze</param>
        public static void LogLayerMask(LayerMask layerMask)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"LayerMask value: {layerMask.value}");
            sb.AppendLine("Active layers:");
            
            for (int i = 0; i < 32; i++)
            {
                if (IsLayerInMask(layerMask, i))
                {
                    string layerName = GetLayerName(i);
                    sb.AppendLine($"  Layer {i}: {layerName}");
                }
            }
            
            Debug.Log(sb.ToString());
        }
        
        #endregion
    }
}