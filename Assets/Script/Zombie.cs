using Script.Weapon;
using UnityEngine;

namespace Script
{
    public class Zombie : EnemyBase
    {
        [SerializeField] private Rigidbody2D rb;

        public override void OnReset()
        {
            state = StateEnum.Alive;
            configSO.CopyData(ref runtimeSOData);
        }

        public override void UpdateMove(Transform target)
        {
            Vector3 direction = target.position - transform.position;
            Vector3 normalDir = direction.normalized;
            rb.velocity = normalDir * runtimeSOData.baseMoveSpeed;
            if (normalDir.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            if (normalDir.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {
            var weaponBase = other.GetComponentInParent<WeaponBase>();
            if (weaponBase == null)
            {
                return;
            }

            TakeDamage(weaponBase.GetDamageValue());
        }

        public override void TakeDamage(float targetAtk)
        {
            runtimeSOData.baseHp -= targetAtk;
            if (runtimeSOData.baseHp <= 0)
                state = StateEnum.Dead;
        }

        public override StateEnum GetState()
        {
            return state;
        }
    }
}