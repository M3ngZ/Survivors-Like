using UnityEngine;

namespace Script.SO
{
    [System.Serializable]
    public struct EnemySOData
    {
        //基础血量
        public float baseHp;

        //基础攻击
        public float baseAtk;

        //基础移动速度
        public float baseMoveSpeed;
    }

    [CreateAssetMenu(fileName = "EnemySo", menuName = "ScriptableObject/Role/Enemy")]
    public class EnemySO : RoleSO<EnemySOData>
    {
    }
}