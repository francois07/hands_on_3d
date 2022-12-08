using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    int i = 3;
    // Update is called once per frame
    void Update()
    {
        if (Time.time > i)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = Vector3.up * 3 + (Vector3.forward * Random.Range(-1, 1));
            sphere.gameObject.AddComponent<SphereCollider>();
            sphere.gameObject.AddComponent<Rigidbody>();
            i += 3;
        }
    }
}
