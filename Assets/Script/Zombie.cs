using UnityEngine;

namespace Script
{
    public class Zombie : EnemyBase
    {
        public override void OnReset()
        {
            configSO.CopyData(ref runtimeSOData);
        }

        public override void UpdateMove(Transform target)
        {
            Vector3 direction = target.position - transform.position;
            Vector3 normalDir = direction.normalized;

            Vector2 position = transform.position + normalDir * (Time.deltaTime * runtimeSOData.baseMoveSpeed);


            //推力
            Vector2 separationForce = Vector2.zero;
            var neighbors = EnemyManager.Instance.GetNeighbor(this);
            foreach (var neighbor in neighbors)
            {
                if (neighbor == this)
                    continue;
                // 比如怪物半径是0.5，两个怪物距离平方小于 1.0 就是撞了
                Vector2 dir = this.transform.position - neighbor.transform.position;
                float distSq = dir.sqrMagnitude;

                if (distSq < 1.0f && distSq > 0.001f) // 避免除0
                {
                    // 简单的反向推力：越近推力越大
                    separationForce += dir.normalized / distSq;
                }
            }

            position += separationForce * 0.1f;

            this.transform.position = position;
        }

        public override void UpdateMoveWithForce(Transform target, Vector2 separationForce)
        {
            // 1. 所有的计算都基于 Vector2，不要每一行都读写 transform.position
            Vector2 currentPos = transform.position; // 只读一次

            // 2. 寻路逻辑
            Vector2 targetPos = target.position;
            Vector2 direction = targetPos - currentPos;

            // 这里的 normalized 需要开平方，还是有消耗的。
            // 如果 target 很远，其实不需要每帧都 normalized，可以粗略计算。
            // 但对于 1000 个单位，这一步通常还能接受。
            Vector2 moveDir = direction.normalized;

            // 3. 叠加推力
            // 限制推力最大值，防止怪物被弹飞到天涯海角
            if (separationForce.sqrMagnitude > 100)
            {
                separationForce = separationForce.normalized * 10;
            }

            // 4. 计算最终位移
            // 这是一个加权：寻路占大头，推力占小头，或者直接叠加
            Vector2 finalVelocity = moveDir * runtimeSOData.baseMoveSpeed + separationForce * 1.5f; // 1.5f 是推力权重

            // 5. 应用移动
            currentPos += finalVelocity * Time.deltaTime;

            // 6. 只有最后这一行写回 transform
            transform.position = currentPos;
        }

        public override void OnTriggerStay2D(Collider2D other)
        {
        }

        public override void TakeDamage(float targetAtk)
        {
        }

        public override StateEnum GetState()
        {
            return state;
        }
    }
}