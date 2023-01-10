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
        // PrefsSettings.SaveListJson<ListPerson>(listPerson, "PERSON_JSON");

        ListPerson l = PrefsSettings.LoadJson<ListPerson>("PERSON_JSON");

        print(l.listP[1].name);
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
