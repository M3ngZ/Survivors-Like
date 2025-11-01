using UnityEngine;

namespace Script
{
    public class CameraMove : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        private Transform _playerTras;

        private void Awake()
        {
            if (player)
            {
                _playerTras = player.transform;
            }
        }

        private void LateUpdate()
        {
            if (_playerTras)
            {
                this.transform.position = Vector3.right * _playerTras.position.x + Vector3.up * _playerTras.position.y +
                                          Vector3.forward * transform.position.z;
            }
        }
    }
}