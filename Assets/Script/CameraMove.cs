using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public class CameraMove : MonoBehaviour
    {
        public Transform playerTrans;
        public float moveSpeed;
        private float _distance = 10;
        private Vector3 offset;

        private void Awake()
        {
            offset = Vector3.forward * -_distance;
        }

        private void LateUpdate()
        {
            if (!playerTrans)
                return;

            Vector3 newPos = Vector3.Lerp(this.transform.position, playerTrans.position + offset,
                moveSpeed * Time.deltaTime);
            this.transform.position = newPos;
        }
    }
}