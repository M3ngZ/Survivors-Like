using UnityEngine;

namespace Script.SO
{
    [System.Serializable]
    public struct WeaponData
    {
        //当前等级
        public int curLevel;

        //基础攻击力
        public float baseAtk;

        //额外增加的攻击力百分比（可能随着等级提升）
        public float extraAtkPer;

        //攻击间隔（每个数量之间）
        public float baseAtkSpeed;

        //基础攻击范围
        public float baseRange;

        //额外估计范围百分比
        public float extraRangePer;

        //冷却时间 秒
        public float cd;

        //减少冷却时间百分比
        public float cdReducePer;

        //存在时间 秒
        public float existTime;

        //额外存在时间百分比
        public float extraExistTime;
    }


    [CreateAssetMenu(fileName = "WeaponSo", menuName = "ScriptableObject/Weapon", order = 0)]
    public class WeaponSO : ScriptableObject
    {
        //最大等级
        public int maxLevel = 8;

        //武器信息
        public WeaponData data;

        public void CopyData(ref WeaponData target)
        {
            target = data;
        }
    }
}