using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateProjectileByJudah : MonoBehaviour
{
    [SerializeField] private float rotSpeed = 700f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotSpeed * Time.deltaTime, 0);
    }

}
