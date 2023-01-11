using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolController : MonoBehaviour
{
    private static PoolController poolController;

    private Dictionary<string, Queue<PoolObject>> poolDictionary = new Dictionary<string, Queue<PoolObject>>();

    [Header("List of object to pool")]
    public List<PoolItem> ItemsToPool;

    //---------------------------Unity Functions----------------------------------
    private void Awake() {
        poolController = this;

        CreatePool();
    }

    private void OnDestroy() {
        poolDictionary.Clear();
    }

    //-----------------------------Pool Controller Functions--------------------------------
    
    //====================Private Functions==================
    private Transform CreateObjectHolder(string name)
    {
        GameObject objectHolder = new GameObject(name + "_Holder");

        return objectHolder.transform;
    }

    private void CreatePool()
    {
        foreach(PoolItem p in ItemsToPool)
        {
            poolDictionary.Add(p.itemName, new Queue<PoolObject>());

            Transform objectHolderTrans = CreateObjectHolder(p.itemName);

            for(int i = 0; i < p.amountToPool; i++)
            {
                PoolObject newObject = new PoolObject(Instantiate(p.objectToPool));
                poolDictionary[p.itemName].Enqueue(newObject);

                GameObject objPool = newObject.GetGameObject(); 

                objPool.name = p.itemName + "_" + (i + 1);
                objPool.transform.SetParent(objectHolderTrans);
            }
        }
    }

    //====================Public Functions==================
    public static GameObject ReuseObject(PoolKey key, Vector3 position, Quaternion rotation)
    {
        string poolKey = key.ToString();

        if (poolController.poolDictionary.ContainsKey(poolKey))
        {
            PoolObject objectToReuse = poolController.poolDictionary[poolKey].Dequeue();
            poolController.poolDictionary[poolKey].Enqueue(objectToReuse);

            objectToReuse.Reuse(position, rotation);

            return objectToReuse.GetGameObject();
        }
        else
        {
            return null;
        }
    }

}

[System.Serializable]
public class PoolItem
{
    [Tooltip("Name with no space")]
    public string itemName;
    public GameObject objectToPool;
    public int amountToPool;
}