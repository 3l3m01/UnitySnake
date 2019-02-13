using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool 
{
    private List<GameObject> pool;
    private LinkedList<GameObject> available;


    public GameObject prefab;

    public GameObjectPool(GameObject prefab)
    {
        pool = new List<GameObject>();
        available = new LinkedList<GameObject>();
        this.prefab = prefab;
    }

    public GameObject GetObject()
    {
        if (available.Count > 0)
        {
            GameObject go = available.Last.Value;
            available.RemoveLast();
            go.SetActive(true);
            return go;
        }
        else
        {
            GameObject go = GameObject.Instantiate(prefab);
            pool.Add(go);
            go.SetActive(true);
            return go;
        }
    }

    public void ReleaseObject(GameObject go)
    {
        if (pool.Contains(go)) // We only release objects that this pool gave out
        {
            go.SetActive(false);
            available.AddLast(go);
        }
    }

    public void ClearAll()
    {
        foreach (GameObject go in pool)
        {
            if (!available.Contains(go))
            {
                ReleaseObject(go);
            }
        }
    }

}   
