using System;

namespace Script
{
    public class Singleton<T> where T : class
    {
        private static T CreateInstanceOfT()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }

        private static readonly Lazy<T> Lazy = new Lazy<T>(CreateInstanceOfT);

        public static T Instance => Lazy.Value;
    }
}