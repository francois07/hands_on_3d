using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySpawner : MonoBehaviour
{
  [Header("Buildings")]
  [SerializeField] GameObject[] m_buildings;
  [SerializeField] Material[] m_buildingMaterials;

  [Header("Gaps")]
  [SerializeField] GameObject[] m_gaps;
  [SerializeField] Material[] m_gapsMaterials;

  [Header("Map")]
  [SerializeField] Texture2D m_heightMap;
  [SerializeField] float m_padding;
  [SerializeField] int m_resolution;

  // Start is called before the first frame update
  void Start()
  {
    int n = m_buildings.Length;

    for (int i = 0; i < m_heightMap.height; i += m_resolution)
    {
      for (int j = 0; j < m_heightMap.width; j += m_resolution)
      {
        float hm = 100.0f - (100.0f * m_heightMap.GetPixel(i, j).grayscale);

        if (hm > 1)
        {
          int rand = (int)Random.Range(1, 255);

          if (rand >= hm)
          {
            GameObject randBulding = Instantiate(m_buildings[rand % n], new Vector3(i * m_padding, 0, j * m_padding), Quaternion.identity);
            randBulding.transform.localScale = new Vector3(rand % 3 + 1, (rand % 5 + 1) + hm * (5 / 100), rand % 3 + 1);
            randBulding.GetComponent<MeshRenderer>().material = m_buildingMaterials[rand % m_buildingMaterials.Length];
          }
        }
      }
    }

    for (int i = 0; i < m_heightMap.height; i += m_resolution)
    {
      for (int j = 0; j < m_heightMap.width; j += m_resolution)
      {
        Vector3 spawnpos = new Vector3(i * m_padding + Random.Range(0, 50), 0, j * m_padding + Random.Range(0, 50));
        int rand = (int)Random.Range(0, m_gaps.Length);

        if (!Physics.CheckSphere(spawnpos, 10))
        {
          GameObject randGap = Instantiate(m_gaps[rand], spawnpos, Quaternion.identity);
          // randGap.GetComponent<MeshRenderer>().material = m_buildingMaterials[rand];
        }
      }
    }
  }

  // Update is called once per frame
  void Update()
  {

  }
}
