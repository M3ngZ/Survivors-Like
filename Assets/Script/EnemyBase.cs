using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    /// <summary>
    /// 敌人基类
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        [SerializeField] private int hp = 10;
        [SerializeField] private int atk = 10;
        [SerializeField] protected StateEnum state;
        [SerializeField] protected EnemyEnum enemyType;

        //属于哪种敌人类型
        public EnemyEnum EnemyType => enemyType;

        public abstract void UpdateMove(Transform target);

        public abstract void OnTriggerStay2D(Collider2D other);

        public abstract void TakeDamage(float targetAtk);

        public abstract StateEnum GetState();
    }
}