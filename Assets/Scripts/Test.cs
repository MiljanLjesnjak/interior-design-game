using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    Quaternion init = Quaternion.identity;

    void Update()
    {
        Quaternion q1 = Quaternion.identity;

        if (Input.GetKeyUp(KeyCode.LeftArrow))
            q1 = GameObject.Find("Coffee Table 3").transform.rotation;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Transform table = GameObject.Find("Coffee Table 3").transform;

            Quaternion q2 = table.rotation;

            Debug.Log(Quaternion.Angle(q1, q2));


            if (Mathf.Abs(Quaternion.Dot(q1, q2)) >= 1 - 0.1f)
                Debug.LogError("2");

        }


    }
}
