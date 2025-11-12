using UnityEngine;

namespace Script.SO
{
    // [CreateAssetMenu("Player1So", "ScriptableObject/Player/Player1", 0)]
    [CreateAssetMenu(fileName = "Player1So", menuName = "ScriptableObject/Player/Player1")]
    public class PlayerSo : ScriptableObject
    {
        //基础血量
        public float baseHp;

        //血量加成
        public float extraHpPer;

        //基础攻击
        public float baseAtk;

        //攻击加成
        public float extraAtkPer;

        //基础移动速度
        public float baseMoveSpeed;

        //移动速度加成
        public float extraMoveSpeedPer;
    }
}