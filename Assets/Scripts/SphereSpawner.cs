using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour
{
  [SerializeField] Material m_material;

  // Start is called before the first frame update
  void Start()
  {

  }

  float rangeTimeSpawn = 0.1f; // Spawn a sphere every 0.1 second
  float timeSpawn = 0.1f; 		 // Time to spawn a sphere

  // Update is called once per frame
  void Update()
  {
    if (Time.time > timeSpawn)
    {
      int numSpheres = 10; // Number of spheres to spawn at the same time

      for (int j = 0; j < numSpheres; j++)
      {

        Vector3 sizeSphere = new Vector3(5, 5, 5); // Size of the sphere
        int secDestroy = 10;                       // Destroy the sphere after 10 second
        int height = 500;													 // Height of the sphere

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        sphere.transform.localScale = sizeSphere;
        sphere.GetComponent<Renderer>().material = m_material;
        Destroy(sphere, secDestroy);

        sphere.transform.position = new Vector3(Random.Range(0, 2500), height, Random.Range(0, 2500));
        // sphere.gameObject.AddComponent<SphereCollider>();
        sphere.gameObject.AddComponent<Rigidbody>();
      }

      timeSpawn += rangeTimeSpawn;
    }
  }
}
