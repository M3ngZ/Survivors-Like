using UnityEngine;

namespace Script
{
    public class Zombie : EnemyBase
    {
        [SerializeField] private float speed;

        public override void UpdateMove(Transform target)
        {
            Vector3 direction = target.position - transform.position;
            Vector3 normalDir = direction.normalized;
            transform.position += normalDir * (Time.deltaTime * speed);
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