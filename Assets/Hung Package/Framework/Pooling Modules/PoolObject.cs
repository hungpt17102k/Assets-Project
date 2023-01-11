using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject
{
    GameObject gameObject;
    Transform transform;

    bool hasPoolObjectComponent;
    IPoolObject poolObjectInterface;

    public PoolObject(GameObject objectInstance)
    {
        gameObject = objectInstance;
        transform = gameObject.transform;

        if (gameObject.GetComponent<IPoolObject>() != null)
        {
            hasPoolObjectComponent = true;
            poolObjectInterface = gameObject.GetComponent<IPoolObject>();
        }

        gameObject.SetActive(false);
    }

    public void Reuse()
    {
        gameObject.SetActive(true);

        if (hasPoolObjectComponent)
        {
            poolObjectInterface.OnObjectReuse();
        }
    }

    public void Reuse(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);

        if (hasPoolObjectComponent)
        {
            poolObjectInterface.OnObjectReuse();
        }

    }

    public void Reuse(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        gameObject.SetActive(true);

        if (hasPoolObjectComponent)
        {
            poolObjectInterface.OnObjectReuse();
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void SetParent(Transform parent)
    {
        transform.parent = parent;
    }

}
