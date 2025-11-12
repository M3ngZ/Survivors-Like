using System;
using UnityEngine;
using UnityEngine.Events;

namespace Script
{
    /// <summary>
    /// 输入注册系统
    /// </summary>
    public class InputManager : SingletonMono<InputManager>
    {
        //水平向量有变化时 响应事件
        public event UnityAction<Vector2> OnMovementInput;

        //ESC等退出键按下
        public event UnityAction OnQuitPressed;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnQuitPressed?.Invoke();
            }

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            if (horizontal != 0 || vertical != 0)
            {
                OnMovementInput?.Invoke(Vector2.right * horizontal + Vector2.up * vertical);
            }
        }
    }
}