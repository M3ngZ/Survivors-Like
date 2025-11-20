using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Script
{
    public enum EnemyEnum
    {
        Zombie
    }

    public class EnemyManager : Singleton<EnemyManager>
    {
        //要攻击的目标
        public Transform target;

        //需要被更新位置的敌人
        private List<EnemyBase> _enemyList;

        //缓存池
        private Dictionary<EnemyEnum, Queue<EnemyBase>> _enemyPool;

        private EnemyManager()
        {
            _enemyList = new List<EnemyBase>();
            _enemyPool = new Dictionary<EnemyEnum, Queue<EnemyBase>>();
        }

        public void StartSpawn()
        {
            //测试用的生成怪物
            MonoManager.Instance.StartCoroutine(SpawnEnemyCoroutine(1000));
        }

        private IEnumerator SpawnEnemyCoroutine(int count)
        {
            if (!target)
                yield break;
            Vector3 playerPos = target.position;
            for (int i = 0; i < count; i++)
            {
                float x = Random.Range(playerPos.x - 10, playerPos.x + 10);
                float y = Random.Range(playerPos.y - 10, playerPos.y + 10);
                SpawnEnemy(EnemyEnum.Zombie, playerPos + new Vector3(x, y));
                yield return null;
            }
        }

        //生成怪物
        private void SpawnEnemy(EnemyEnum enemyType, Vector3 spawnPos)
        {
            //如果缓存池中有 则从缓存池中取
            if (_enemyPool.TryGetValue(enemyType, out Queue<EnemyBase> queue) && queue.Count > 0)
            {
                EnemyBase enemy = queue.Dequeue();
                enemy.transform.position = spawnPos;
                enemy.gameObject.SetActive(true);
                enemy.OnReset();
                _enemyList.Add(enemy);
                return;
            }

            switch (enemyType)
            {
                case EnemyEnum.Zombie:
                    ResourceManager.Instance.LoadAsync<EnemyBase>("Prefab/Enemy/Zombie0",
                        (enemyBase) => { OnEnemyLoaded(enemyBase, spawnPos); });
                    break;
                default:
                    // Debug.Log("没有这个怪物");
                    break;
            }
        }

        private void OnEnemyLoaded(EnemyBase enemyComponentPrefab, Vector3 spawnPos)
        {
            if (enemyComponentPrefab == null)
                return;
            EnemyBase newEnemy = Object.Instantiate(enemyComponentPrefab, spawnPos, Quaternion.identity);
            _enemyList.Add(newEnemy);
            newEnemy.OnReset();
        }

        public void Update()
        {
            int count = _enemyList.Count;

            for (int i = 0; i < count;)
            {
                EnemyBase enemy = _enemyList[i];
                StateEnum state = enemy.GetState();

                if (state == StateEnum.Dead)
                {
                    UnSpawnEnemy(enemy);

                    int lastIndex = count - 1;
                    _enemyList[i] = _enemyList[lastIndex];
                    _enemyList.RemoveAt(lastIndex);

                    count--;
                }
                else
                {
                    if (state != StateEnum.BeIce)
                        enemy.UpdateMove(target);

                    i++;
                }
            }
        }

        private void UnSpawnEnemy(EnemyBase enemy)
        {
            EnemyEnum enemyType = enemy.EnemyType;
            if (!_enemyPool.ContainsKey(enemyType))
            {
                _enemyPool[enemyType] = new Queue<EnemyBase>();
            }

            enemy.gameObject.SetActive(false);
            _enemyPool[enemyType].Enqueue(enemy);
        }
    }
}