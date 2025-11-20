using System;
using UnityEngine;

namespace Script.SO
{
    [Serializable]
    public struct PlayerSOData
    {
        //基础血量
        public float baseHp;

        //基础攻击
        public float baseAtk;

        //基础移动速度
        public float baseMoveSpeed;

        //血量加成
        public float extraHpPer;

        //攻击加成
        public float extraAtkPer;

        //移动速度加成
        public float extraMoveSpeedPe;
    }

    [CreateAssetMenu(fileName = "PlayerSo", menuName = "ScriptableObject/Role/Player")]
    public class PlayerSO : RoleSO<PlayerSOData>
    {
    }
}