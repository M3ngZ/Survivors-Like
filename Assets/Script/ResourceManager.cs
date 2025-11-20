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

            UnityAction<Object> action = obj => callback?.Invoke(obj as T);
            //如果存在回调列表，说明资源已经在加载中了！此时进入等待列表
            if (_callbackListDic.TryGetValue(path, out var callbackList))
            {
                callbackList.Add(action);
                return;
            }

            callbackList = new List<UnityAction<Object>> { action };
            _callbackListDic.Add(path, callbackList);

            MonoManager.Instance.StartCoroutine(LoadAsyncCoroutine<T>(path));
        }


        private IEnumerator LoadAsyncCoroutine<T>(string path) where T : Object
        {
            var req = Resources.LoadAsync<T>(path);
            yield return req;

            Object asset = req.asset;
            if (asset != null)
            {
                _resourceCacheDic.TryAdd(path, asset);
            }
            else
            {
                Debug.LogError($"[ResManager] 资源加载失败，路径不存在: {path}");
            }

            if (_callbackListDic.TryGetValue(path, out var callbackList))
            {
                foreach (var callback in callbackList)
                {
                    callback?.Invoke(asset);
                }

                _callbackListDic.Remove(path);
            }
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