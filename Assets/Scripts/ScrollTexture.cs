using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
  [SerializeField] Material m_Material;
  [SerializeField] float m_Speed;

  // Start is called before the first frame update
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {
    m_Material.mainTextureOffset += new Vector2(0, m_Speed * Time.deltaTime);
  }
}
