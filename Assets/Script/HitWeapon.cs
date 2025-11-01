using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public class HitWeapon : MonoBehaviour
    {
        [SerializeField] private Animation hitAni;

        private bool _canCountTime; //是否可以自己开始计时了 让自己消失

        private float _lastRunTime = 0;
        [SerializeField] private float loopTime;

        private void OnEnable()
        {
            if (!hitAni) return;
            hitAni.Play();
            _canCountTime = true;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_canCountTime) return;
            _lastRunTime += Time.deltaTime;

            if (_lastRunTime < loopTime) return;
            _lastRunTime = 0;
            _canCountTime = false;
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other != null)
            {
                print(other);
                other.gameObject.SetActive(false);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other != null)
            {
                print(other);
                other.gameObject.SetActive(false);
            }
        }
    }
}