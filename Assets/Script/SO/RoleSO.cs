using UnityEngine;

namespace Script.SO
{
    public abstract class RoleSO<T> : ScriptableObject where T : struct
    {
        [SerializeField] protected T data;

        /// <summary>
        /// 将自己的数据深拷贝给目标
        /// </summary>
        /// <param name="targetData">目标数据</param>
        public void CopyData(ref T targetData)
        {
            targetData = this.data;
        }
    }
}