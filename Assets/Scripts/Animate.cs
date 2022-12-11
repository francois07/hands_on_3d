using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyMathTools;

public class Animate : MonoBehaviour
{
  [SerializeField] float _TranslationSpeed;
  [SerializeField] float m_radius;
  Vector3 m_initialPos;

  // Start is called before the first frame update
  void Start()
  {
    m_initialPos = transform.position;
  }

  // Update is called once per frame
  void Update()
  {
    transform.localPosition = (transform.localPosition - Vector3.right * _TranslationSpeed * Time.deltaTime);

    if (transform.localPosition.x > 2000)
    {
      transform.localPosition = new Vector3(0, 0, 0);
    }
  }
}
