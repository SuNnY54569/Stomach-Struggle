using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadomRotation : MonoBehaviour
{
    private void Start()
    {
        // สุ่มการหมุนในแกน Z
        RandomizeRotationZ();
    }

    private void RandomizeRotationZ()
    {
        // สุ่มค่าการหมุนในแกน Z (ระหว่าง 0 ถึง 360 องศา)
        float randomZRotation = Random.Range(0f, 360f);

        // ตั้งค่าการหมุนในแกน Z ให้กับวัตถุ
        transform.rotation = Quaternion.Euler(0, 0, randomZRotation);
    }
}
