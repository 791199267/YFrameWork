using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramwork
{
    public class Singleton<T> where T : new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();
                return instance;
            }
        }
    }

    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject gameObj = new GameObject();
                    instance = gameObj.AddComponent<T>();
                    gameObj.name = typeof(T).Name;
                    GameObject.DontDestroyOnLoad(gameObj);
                }
                return instance;
            }
        }
    }
}
