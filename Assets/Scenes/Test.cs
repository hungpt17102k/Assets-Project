using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{
    [Title("Alo", horizontalLine: false, bold: true)]
    public ListPerson listPerson = new ListPerson();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update() {
        if(Input.anyKeyDown)
        {
            PoolController.ReuseObject(PoolKey.Cube, Vector3.down, Quaternion.identity);
        }
    }
}

[System.Serializable]
public class ListPerson
{
    public List<Person> listP;

    public ListPerson()
    {
        listP = new List<Person>();
    }
}

[System.Serializable]
public class Person
{
    public string name;
    public string age;
}
