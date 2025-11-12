using System;
using System.Collections;
using UnityEngine.Events;

namespace Script
{
    /// <summary>
    /// 公共Mono管理类 可利用它来注册方法 使得未继承MonoBehavior的类也能执行相关方法
    /// </summary>
    public class MonoManager : SingletonMono<MonoManager>
    {
        private event UnityAction UpdateAction;

        public void RegisterUpdate(UnityAction action)
        {
            UpdateAction += action;
        }

        public void UnRegisterUpdate(UnityAction action)
        {
            UpdateAction -= action;
        }

        private void Update()
        {
            UpdateAction?.Invoke();
        }
    }
}