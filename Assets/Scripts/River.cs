using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class River : MonoBehaviour
{
  public delegate Vector3 ComputePositionDelegate(float kX, float kZ);

  [SerializeField] Transform[] m_RiverWayPoints;
  LTSpline m_Spline;

  [SerializeField] AnimationCurve m_RiverWidth;

  MeshFilter m_Mf;

  [SerializeField] Material m_Material;
  [SerializeField] float m_Speed;

  // Start is called before the first frame update
  void Start()
  {
    m_Mf = GetComponent<MeshFilter>();
    m_Spline = new LTSpline(m_RiverWayPoints.Select(item => item.position).ToArray());
    m_Mf.mesh = CreateNormalizedGridXZ(100, 100,
        (kX, kZ) =>
        {
          Vector3 pt = m_Spline.interp(kZ); //pt corresponds to the position on the spline at kZ
          Vector3 tangent = (m_Spline.interp(kZ + 0.001f) - pt).normalized; //tangent at pt 
          Vector3 ortho = Vector3.Cross(tangent, Vector3.down); //orthogonal vector to tangent

          return pt + ortho * (kX - 0.5f) * .5f * m_RiverWidth.Evaluate(kZ); // return the position of the vertex, Evaluate(kZ) returns the width of the river at kZ
        }
        );

  }

  // Update is called once per frame
  void Update()
  {
    m_Material.mainTextureOffset += new Vector2(0, m_Speed * Time.deltaTime);
  }

  private void OnDrawGizmos()
  {
    if (m_RiverWayPoints != null && m_RiverWayPoints.Length > 1)
    {
      Gizmos.color = Color.red;
      for (int i = 0; i < m_RiverWayPoints.Length - 1; i++)
      {
        Gizmos.DrawLine(m_RiverWayPoints[i].position, m_RiverWayPoints[i + 1].position);
      }
    }

    if (m_Spline != null)
    {
      Gizmos.color = Color.green;
      int nPos = 100;
      for (int i = 0; i < nPos; i++)
        Gizmos.DrawSphere(m_Spline.interp((float)i / (nPos - 1)), .01f);
    }
  }


  Mesh CreateNormalizedGridXZ(int nSegmentsX, int nSegmentsZ, ComputePositionDelegate computePos = null)
  {
    Mesh mesh = new Mesh();
    mesh.name = "NormalizedGrid";
    mesh.indexFormat = IndexFormat.UInt32;

    Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
    int[] triangles = new int[2 * nSegmentsX * nSegmentsZ * 3];
    Vector2[] uv = new Vector2[vertices.Length];

    //Vertices
    int index = 0;
    for (int i = 0; i < nSegmentsZ + 1; i++)
    {
      float kZ = (float)i / nSegmentsZ;
      for (int j = 0; j < nSegmentsX + 1; j++)
      {
        float kX = (float)j / nSegmentsX;
        vertices[index] = computePos != null ? computePos(kX, kZ) : new Vector3(kX, 0, kZ);
        uv[index++] = new Vector2(kX, kZ);
      }
    }

    //Triangles
    index = 0;
    for (int i = 0; i < nSegmentsZ; i++)
    {
      for (int j = 0; j < nSegmentsX; j++)
      {
        triangles[index++] = i * (nSegmentsX + 1) + j;
        triangles[index++] = (i + 1) * (nSegmentsX + 1) + j;
        triangles[index++] = (i + 1) * (nSegmentsX + 1) + j + 1;

        triangles[index++] = i * (nSegmentsX + 1) + j;
        triangles[index++] = (i + 1) * (nSegmentsX + 1) + j + 1;
        triangles[index++] = i * (nSegmentsX + 1) + j + 1;
      }
    }

    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.uv = uv;

    mesh.RecalculateNormals();
    mesh.RecalculateBounds();

    return mesh;
  }
}
