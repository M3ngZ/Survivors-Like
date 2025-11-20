using System;
using System.Collections;
using Script.SO;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Weapon
{
    public class Whip : WeaponBase
    {
        [SerializeField] private Animation whipAni;
        [SerializeField] private GameObject whipSpObject;
        [SerializeField] private GameObject whipColliderObj;

        [SerializeField] private WeaponSO weaponConfig;
        private WeaponData _runTimeWeaponData;

        private Coroutine _coroutine;

        public void Awake()
        {
            weaponConfig.CopyData(ref _runTimeWeaponData);
            whipColliderObj.SetActive(false);
        }

        public override void OnStart()
        {
            if (_coroutine != null)
                return;
            _coroutine = StartCoroutine(UpdateWeapon());
        }

        public override void OnStop()
        {
            if (_coroutine == null)
                return;
            StopCoroutine(_coroutine);
            _coroutine = null;
            //强制停止
            OnOver();
        }

        public override void OnUse()
        {
            this.transform.position = spawnTarget.position;
            whipColliderObj.SetActive(true);
            whipAni.Play();
        }

        public override void OnOver()
        {
            whipColliderObj.SetActive(false);
        }

        public override void SetSpawnTarget(Transform target)
        {
            spawnTarget = target;
        }


        private IEnumerator UpdateWeapon()
        {
            while (true)
            {
                OnUse();
                yield return new WaitForSeconds(_runTimeWeaponData.existTime);
                OnOver();
                yield return new WaitForSeconds(_runTimeWeaponData.cd);
            }
        }
    }
}