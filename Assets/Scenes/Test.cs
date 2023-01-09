using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{
    [Title("Alo", horizontalLine: false, bold: true)]
    public Transform cube;

    // Start is called before the first frame update
    void Start()
    {
        cube.DOMoveX(2f, 3f);
    }


    private IEnumerator TestALO() {
        yield return new WaitForSeconds(3f);

        print("ALo");
    }
}
