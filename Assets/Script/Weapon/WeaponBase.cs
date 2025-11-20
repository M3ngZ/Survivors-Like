using UnityEngine;

namespace Script.Weapon
{
    public abstract class WeaponBase : MonoBehaviour
    {
        protected Transform spawnTarget;

        //启动循环
        public abstract void OnStart();

        //暂停循环
        public abstract void OnStop();

        //使用武器
        public abstract void OnUse();

        //结束使用武器
        public abstract void OnOver();

        //设置创建时的中心点
        public abstract void SetSpawnTarget(Transform target);
    }
}