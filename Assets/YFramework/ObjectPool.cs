using GameFramwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    private Dictionary<string, List<GameObject>> _poolDict = new Dictionary<string, List<GameObject>>();

    public GameObject GetObject(string name)
    {
        GameObject gameObject = null;
        if (_poolDict.ContainsKey(name) && _poolDict[name].Count > 0)
        {
            gameObject = _poolDict[name][0];
            _poolDict[name].RemoveAt(0);
        }
        else
        {
            gameObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(name));
            gameObject.name = name;
        }
        if (gameObject != null) gameObject.SetActive(true);
        return gameObject;
    }

    public void PushObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            Debug.LogError("对象池放入了一个空物体!");
            return;
        }

        gameObject.SetActive(false);
        var name = gameObject.name;
        if (!_poolDict.ContainsKey(name))
        {
            _poolDict.Add(name, new List<GameObject>());
        }
        _poolDict[name].Add(gameObject);
    }
}
