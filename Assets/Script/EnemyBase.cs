using System;
using Script.SO;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    /// <summary>
    /// 敌人基类
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        [SerializeField] protected EnemySO configSO;
        protected EnemySOData runtimeSOData;
        [SerializeField] protected StateEnum state;
        [SerializeField] protected EnemyEnum enemyType;


        //属于哪种敌人类型
        public EnemyEnum EnemyType => enemyType;

        public abstract void OnReset();

        public abstract void UpdateMove(Transform target);

        //检测攻击
        // public abstract bool CheckAttack();

        public abstract void OnTriggerStay2D(Collider2D other);

        public abstract void TakeDamage(float targetAtk);

        public abstract StateEnum GetState();
    }
}