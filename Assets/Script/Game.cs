using System;
using System.Collections;
using UnityEngine;

namespace Script
{
    public class Game : SingletonMono<Game>
    {
        private PlayerManager _playerManager;
        private EnemyManager _enemyManager;
        private InputManager _inputManager;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            // 在场景加载前创建一次
            var instance = Instance;
            DontDestroyOnLoad(instance.gameObject);
        }

        protected override void Awake()
        {
            _playerManager = PlayerManager.Instance;
            _enemyManager = EnemyManager.Instance;
            _inputManager = InputManager.Instance;
        }

        IEnumerator Start()
        {
            //创建玩家
            yield return StartCoroutine(_playerManager.CreatePlayer());

            //测试添加武器
            _playerManager.AddWeapon(WeaponType.Whip);

            //等玩家创建完之后，给相机加上追踪脚本，跟着玩家
            var mainCamera = Camera.main;
            if (mainCamera)
            {
                var cameraMove = mainCamera.gameObject.GetComponent<CameraMove>();
                cameraMove.playerTrans = _playerManager.Player.transform;
            }

            //初始化敌人管理器
            _enemyManager.target = _playerManager.Player.transform;
            _enemyManager.StartSpawn();
        }

        private void Update()
        {
            _enemyManager.Update();
        }

        public void OnEnable()
        {
            _playerManager?.OnEnable();
        }

        public void OnDisable()
        {
            _playerManager?.OnDisable();
        }
    }
}