using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Vibration.Vibrate(100);
    }


    private IEnumerator TestALO() {
        yield return new WaitForSeconds(3f);

        print("ALo");
    }
}
