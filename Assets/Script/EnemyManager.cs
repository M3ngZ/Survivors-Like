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

        //区域大小
        private const int GridSize = 2;

        //区域划分
        private Dictionary<Vector2Int, List<EnemyBase>> _gridDic;


        private EnemyManager()
        {
            _enemyList = new List<EnemyBase>();
            _enemyPool = new Dictionary<EnemyEnum, Queue<EnemyBase>>();
            _gridDic = new Dictionary<Vector2Int, List<EnemyBase>>();
        }

        #region 怪物创建与回收

        public void StartSpawn()
        {
            //测试用的生成怪物
            MonoManager.Instance.StartCoroutine(SpawnEnemyCoroutine(10000));
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

        #endregion

        #region 怪物移动更新

        public void Update()
        {
            UpdateGrids();
            int count = _enemyList.Count;

            // 缓存 GridSize 的倒数，乘法比除法快
            float gridSizeInv = 1.0f / GridSize;

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
                    {
                        // --- 性能关键点：直接在这里获取 enemy 的坐标，只获取一次 ---
                        Vector3 enemyPos = enemy.transform.position;
                        Vector2Int enemyGridKey = new Vector2Int(
                            Mathf.FloorToInt(enemyPos.x * gridSizeInv),
                            Mathf.FloorToInt(enemyPos.y * gridSizeInv)
                        );

                        // --- 计算推力 (Soft Collision) ---
                        Vector2 separationForce = Vector2.zero;

                        // 直接遍历 3x3 网格，不生成 List
                        for (int x = -1; x <= 1; x++)
                        {
                            for (int y = -1; y <= 1; y++)
                            {
                                Vector2Int checkKey = new Vector2Int(enemyGridKey.x + x, enemyGridKey.y + y);

                                if (_gridDic.TryGetValue(checkKey, out var neighborList))
                                {
                                    // 遍历该格子里的怪物
                                    for (int j = 0; j < neighborList.Count; j++)
                                    {
                                        EnemyBase neighbor = neighborList[j];
                                        if (neighbor == enemy) continue;

                                        // 纯数学计算，不访问 transform
                                        Vector3 otherPos = neighbor.transform.position; // 这里依然要访问一次，无法完全避免，除非缓存数据

                                        // 手动计算距离平方，避免 Vector3 操作带来的额外开销
                                        float dx = enemyPos.x - otherPos.x;
                                        float dy = enemyPos.y - otherPos.y;
                                        float distSq = dx * dx + dy * dy;

                                        // 判定碰撞半径 (比如半径之和的平方是 1.0)
                                        if (distSq < 1.0f && distSq > 0.001f)
                                        {
                                            // 简单的反比例推力：距离越近(distSq越小)，推力越大
                                            // 甚至不需要开平方(normalized)，直接用分量除以平方即可
                                            float forceStrength = 1.0f / distSq;
                                            separationForce.x += dx * forceStrength;
                                            separationForce.y += dy * forceStrength;
                                        }
                                    }
                                }
                            }
                        }

                        enemy.UpdateMoveWithForce(target, separationForce);
                    }

                    i++;
                }
            }
        }

        //更新网格的划分
        private void UpdateGrids()
        {
            foreach (var list in _gridDic.Values)
            {
                list.Clear();
            }

            for (int i = 0; i < _enemyList.Count; i++)
            {
                EnemyBase enemy = _enemyList[i];
                StateEnum state = enemy.GetState();
                if (state == StateEnum.Dead)
                    continue;

                var key = GetGridKey(enemy.transform.position);
                if (!_gridDic.ContainsKey(key))
                {
                    _gridDic[key] = new List<EnemyBase>();
                }

                _gridDic[key].Add(enemy);
            }
        }

        //根据坐标获取一个二维坐标键
        private Vector2Int GetGridKey(Vector3 position)
        {
            return new Vector2Int(Mathf.FloorToInt(position.x / GridSize), Mathf.FloorToInt(position.y / GridSize));
        }

        private List<EnemyBase> _tempNeighbors = new List<EnemyBase>();

        public List<EnemyBase> GetNeighbor(EnemyBase enemy)
        {
            _tempNeighbors.Clear();
            var curVec2 = GetGridKey(enemy.transform.position);
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int checkKey = new Vector2Int(curVec2.x + x, curVec2.y + y);
                    if (!_gridDic.ContainsKey(checkKey))
                        continue;
                    var list = _gridDic[checkKey];
                    _tempNeighbors.AddRange(list);
                }
            }

            return _tempNeighbors;
        }

        #endregion
    }
}