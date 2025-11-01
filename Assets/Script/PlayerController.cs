using UnityEngine;

namespace Script
{
    public class PlayerController : MonoBehaviour
    {
        private const string AxisRawX = "Horizontal";
        private const string AxisRawY = "Vertical";
        [SerializeField] private GameObject weaponPref;
        private GameObject _weapon;
        [SerializeField] private float speed;
        private float _deltaX = 0;
        private float _deltaY = 0;

        private float _lastRunTime = 0;
        [SerializeField] private float loopTime;

        private void Update()
        {
            CheckLoop();
            _deltaX = Input.GetAxisRaw(AxisRawX);
            _deltaY = Input.GetAxisRaw(AxisRawY);
            this.transform.position += Vector3.right * (_deltaX * Time.deltaTime * speed);
            this.transform.position += Vector3.up * (_deltaY * Time.deltaTime * speed);
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
                UseWeapon();
            }
        }
    }
}