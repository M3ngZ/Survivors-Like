using UnityEngine;

namespace Script
{
    public class EnemyMove : MonoBehaviour
    {
        private Transform _targetTransform;
        [SerializeField] private float speed;

        private void Awake()
        {
            GameObject player = GameObject.Find("Player");
            if (player)
            {
                _targetTransform = player.transform;
            }
        }


        // Update is called once per frame
        void Update()
        {
            Vector3 direction = _targetTransform.position - transform.position;
            Vector3 normalDir = direction.normalized;
            transform.position += normalDir * (Time.deltaTime * speed);
        }
    }
}