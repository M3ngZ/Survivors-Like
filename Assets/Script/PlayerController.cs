using System;
using UnityEngine;

namespace Script
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject weaponPref;
        private GameObject _weapon;
        [SerializeField] private float speed;

        private float _lastRunTime = 0;
        [SerializeField] private float loopTime;

        private void Update()
        {
            CheckLoop();
        }

        private void UseWeapon()
        {
            if (!_weapon)
            {
                _weapon = Instantiate(weaponPref);
            }

            _weapon.SetActive(true);
            _weapon.transform.position = transform.position + new Vector3(2.8f, .1f);
        }

        private void CheckLoop()
        {
            _lastRunTime += Time.deltaTime;
            if (_lastRunTime >= loopTime)
            {
                _lastRunTime = 0;
                // UseWeapon();
            }
        }
    }
}