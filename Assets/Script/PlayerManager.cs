using System;
using System.Collections;
using System.Collections.Generic;
using Script.Weapon;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Script
{
    public enum WeaponType
    {
        Whip, //鞭子
    }

    public class PlayerManager : Singleton<PlayerManager>
    {
        //玩家属性 ScriptableObject
        //等待做成SO
        private int _hp = 100;
        private float _baseAtk = 10;
        private float _extraAtkPercent = 0; //额外攻击力加成
        private float moveSpeed = 3.5f;

        //玩家对象
        private GameObject _player;

        //玩家对象是否存在
        private bool _playerIsExist;


        //玩家武器 List<Weapon>
        private List<WeaponBase> _weapons;

        public GameObject Player => _player;

        private PlayerManager()
        {
            _weapons = new List<WeaponBase>();
        }

        //创建玩家在原点
        public IEnumerator CreatePlayer()
        {
            var resourceRequest = Resources.LoadAsync<GameObject>("Prefab/Player/Player");
            yield return resourceRequest;
            if (resourceRequest == null)
            {
                Debug.LogError("并没有成功创建玩家");
                yield break;
            }

            var playerPrefab = resourceRequest.asset as GameObject;
            _player = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

            _playerIsExist = true;
        }

        private void PlayerMove(Vector2 delta)
        {
            if (!_playerIsExist)
                return;
            Vector3 pos = _player.transform.position;
            pos.x += delta.x * moveSpeed * Time.deltaTime;
            pos.y += delta.y * moveSpeed * Time.deltaTime;
            _player.transform.position = pos;
        }

        public void StartWeapon()
        {
            foreach (var weaponBase in _weapons)
            {
                weaponBase.OnStart();
            }
        }

        public void StopWeapon()
        {
            foreach (var weaponBase in _weapons)
            {
                weaponBase.OnStop();
            }
        }

        public void AddWeapon(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Whip:
                    ResourceManager.Instance.LoadAsync<WeaponBase>("Prefab/Weapon/Whip", OnWeaponLoaded);
                    break;
            }
        }

        private void OnWeaponLoaded(WeaponBase weaponComponentPrefab)
        {
            if (weaponComponentPrefab == null)
                return;
            var weapon = Object.Instantiate(weaponComponentPrefab);
            weapon.SetSpawnTarget(_player.transform);
            _weapons.Add(weapon);
            StartWeapon();
        }

        public void OnEnable()
        {
            if (InputManager.Instance)
                InputManager.Instance.OnMovementInput += PlayerMove;
        }

        public void OnDisable()
        {
            if (InputManager.Instance)
                InputManager.Instance.OnMovementInput -= PlayerMove;
        }
    }
}