using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Script
{
    /// <summary>
    /// 资源加载管理器
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager>
    {
        /// <summary>
        /// 缓存加载好的资源
        /// </summary>
        private Dictionary<string, Object> _resourceCacheDic;

        /// <summary>
        /// 存放回调
        /// </summary>
        private Dictionary<string, List<UnityAction<Object>>> _callbackListDic;

        private ResourceManager()
        {
            _resourceCacheDic = new Dictionary<string, Object>();
            _callbackListDic = new Dictionary<string, List<UnityAction<Object>>>();
        }

        #region 资源加载

        /// <summary>
        /// 异步加载资源 （Resources方式）
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="callback">加载成功之后的回调</param>
        public void LoadAsync<T>(string path, UnityAction<T> callback) where T : Object
        {
            if (_resourceCacheDic.TryGetValue(path, out Object resource))
            {
                callback?.Invoke(resource as T);
                return;
            }

            if (_callbackListDic.TryGetValue(path, out List<UnityAction<Object>> callbackList))
            {
                callbackList.Add((obj) => { callback.Invoke(obj as T); });
                return;
            }

            MonoManager.Instance.StartCoroutine(LoadAsyncCoroutine<T>(path));
        }


        private IEnumerator LoadAsyncCoroutine<T>(string path) where T : Object
        {
            var req = Resources.LoadAsync<T>(path);
            yield return req;
            if (req.asset == null)
            {
                // Debug.LogWarning($"资源加载出错，路径：{path} 不存在，或类型错误");
                _callbackListDic.Remove(path);
                yield break;
            }

            _resourceCacheDic.Add(path, req.asset);

            foreach (UnityAction<Object> callback in _callbackListDic[path])
            {
                callback?.Invoke(req.asset);
            }

            _callbackListDic.Remove(path);
        }

        #endregion

        #region 资源卸载

        /// <summary>
        /// 移除某个缓存
        /// </summary>
        /// <param name="path">缓存路径</param>
        public void RemoveFromCache(string path)
        {
            if (_callbackListDic.ContainsKey(path))
            {
                _resourceCacheDic.Remove(path);
            }
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void ClearCache()
        {
            _resourceCacheDic.Clear();
        }

        /// <summary>
        /// 卸载某个资源
        /// </summary>
        /// <param name="asset">资源</param>
        public void UnloadResource(Object asset)
        {
            if (asset != null && asset is not GameObject)
            {
                Resources.UnloadAsset(asset);
            }
        }

        /// <summary>
        /// 卸载未使用的资源
        /// </summary>
        public void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        #endregion
    }
}