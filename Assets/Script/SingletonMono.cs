using UnityEngine;

namespace Script
{
    /// <summary>
    /// 单例模式模板
    /// </summary>
    /// <typeparam name="T">要作为单例使用的类型</typeparam>
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        // 用于线程锁，防止多线程同时访问时出问题
        private static readonly object _lock = new object();

        // 静态实例变量
        private static T _instance;

        // 一个标志位，用于标记程序是否正在退出
        private static bool applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));
                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError(
                                $"[单例] 不止一个单例：'{typeof(T)}'，需要检查场景.");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singletonGo = new GameObject();
                            _instance = singletonGo.AddComponent<T>();
                            singletonGo.name = $"(Singleton) {typeof(T)}";
                        }
                    }

                    return _instance;
                }
            }
        }

        protected virtual void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }

        /// <summary>
        /// 方便手动放置单例对象
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}