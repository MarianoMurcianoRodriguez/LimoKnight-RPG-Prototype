using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LimoKnight
{
    /// <summary>
    /// This singleton class will require T to be the same Type as the class that inherits this
    /// singleton and to have a parent holding it to not be destroy on posible loads to swapping scenes
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        private static Dictionary<string, string> _paths;

        private static T _instance;

        private string Path
        {
            get
            {
                GameObject obj = gameObject;
                string path = "/" + obj.name;
                while (obj.transform.parent != null)
                {
                    obj = obj.transform.parent.gameObject;
                    path = "/" + obj.name + path;
                }
                //Debug.Log("Path = " + path);
                return path;
            }
        }

        private static Dictionary<string, string> Paths
        {
            get
            {
                if (_paths == null)
                    _paths = new Dictionary<string, string>();
                return _paths;
            }
        }

        protected virtual void Awake()
        {
            DontDestroyOnLoad(transform.parent);
            Paths.Add((typeof(T)).ToString(), Path);
        }

        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                string path;

                //Debug.Log("Looking for " + typeof(T).ToString() + "...");

                if (Paths.TryGetValue(typeof(T).ToString(), out path))
                {
                    GameObject obj = GameObject.Find(path) as GameObject;
                    if (obj != null)
                    {
                        //Debug.Log("Found!!!");
                        _instance = obj.GetComponent<T>();
                    }
                    else
                    {
                        //Debug.Log("Not Found...");
                    }
                }
                return _instance;
            }
        }
    }

}



