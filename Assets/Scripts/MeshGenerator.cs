using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyMathTools;
using System.Linq;

namespace MeshGenerator
{
  public class MeshGenerator : MonoBehaviour
  {
    public delegate Vector3 ComputePositionDelegate(float kX, float kZ);
    public delegate Vector3 ComputeNormalDelegate(float kX, float kZ);
    public delegate Vector3 ComputeUVDelegate(float kX, float kZ);


    [Header("Plane Heightmap")]
    [SerializeField] Texture2D m_HeightMap;
    [SerializeField][Range(0, 500)] int m_Yscale;
    [SerializeField][Range(0, 10000)] int m_Hscale;

    [Header("Cube Heigtmaps")]
    [SerializeField] Texture2D[] m_HeightMaps;

    [Header("Spline")]
    [SerializeField] Transform[] m_SplineCtrlPts;
    [SerializeField] AnimationCurve m_Width;
    LTSpline m_Spline;

    [Header("Texture")]
    [SerializeField] int m_Speed;
    [SerializeField] Material m_Material;

    //[Header("TestHelix")]
    //[SerializeField] float testHelixRadius;

    // Start is called before the first frame update
    void Start()
    {
      MeshFilter meshFilter = GetComponent<MeshFilter>();
      // m_Spline = new LTSpline(m_SplineCtrlPts.Select((Transform t) => t.position).ToArray());

      // meshFilter.mesh = createNormalizedPlaneXZMesh(100, 100, (kX, kZ) =>
      // {
      //   Vector3 hm = applyHeightMap(new Vector3(kX, 0, kZ), 1);
      //   return new Vector3(-hm.y, hm.x, hm.z);
      // }, (kX, kZ) => Vector3.right);
      meshFilter.mesh = createNormalizedCubeMesh(100, 100, 25, 50, 1);


      // MeshCollider meshCollider = GetComponent<MeshCollider>();
      // meshCollider.sharedMesh = meshFilter.mesh;
    }

    // Update is called once per frame
    void Update()
    {
      // m_Material.mainTextureOffset += new Vector2(0, m_Speed * Time.deltaTime);
    }


    Mesh createNormalizedCubeMesh(int nSegmentsX, int nSegmentsZ, float length, float width, float height)
    {
      Mesh mesh = new Mesh();
      mesh.name = "Cube";
      mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

      Mesh sideFront = createNormalizedPlaneXZMesh(nSegmentsX, nSegmentsZ, (kX, kZ) =>
      {
        /* Avant avion */
        // Vector3 hm = applyHeightMap(m_HeightMaps[0], kX, kZ);
        // return new Vector3(hm.x * width, hm.z * height, -hm.y * 20);

        return new Vector3(kX * width, kZ * height, 0);
      });
      Mesh sideBack = createNormalizedPlaneXZMesh(nSegmentsX, nSegmentsZ, (kX, kZ) =>
      {
        /* Dos avion */
        // Vector3 hm = applyHeightMap(m_HeightMaps[0], kX, kZ);
        // return new Vector3(width * hm.x, (1 - hm.z) * height, length + hm.y * 20);
        return new Vector3(kX * width, (1 - kZ) * height, length);
      });

      Mesh sideTop = createNormalizedPlaneXZMesh(nSegmentsX, nSegmentsZ, (kX, kZ) =>
      {
        /* Dessus avion */
        Vector3 hm = applyHeightMap(m_HeightMaps[0], kX, kZ);
        return new Vector3(hm.x * width, height + hm.y * 2, hm.z * length);
      });
      Mesh sideBottom = createNormalizedPlaneXZMesh(nSegmentsX, nSegmentsZ, (kX, kZ) => new Vector3(width * (1 - kX), 0, kZ * length));

      Mesh sideLeft = createNormalizedPlaneXZMesh(nSegmentsX, nSegmentsZ, (kX, kZ) =>
      {
        /* Coté gauche avion */
        // Vector3 hm = applyHeightMap(m_HeightMaps[1], kX, kZ);
        // return new Vector3(-hm.y * 10, height * hm.z, length * (1 - hm.x));

        return new Vector3(0, height * kZ, length * (1 - kX));
      });
      Mesh sideRight = createNormalizedPlaneXZMesh(nSegmentsX, nSegmentsZ, (kX, kZ) => new Vector3(width, height * kZ, kX * length));

      Dictionary<string, Mesh> sides = new Dictionary<string, Mesh>
      {
        ["front"] = sideFront,
        ["back"] = sideBack,
        ["top"] = sideTop,
        ["bottom"] = sideBottom,
        ["right"] = sideRight,
        ["left"] = sideLeft
      };

      // Mesh[] sides = new Mesh[] { sideFront, sideTop, sideRight, sideLeft, sideBack, sideBottom };

      CombineInstance[] combine = new CombineInstance[sides.Count];

      for (int i = 0; i < combine.Length; i++)
      {
        combine[i].mesh = sides.Values.ToArray()[i];
      }

      mesh.CombineMeshes(combine, true, false, false);

      mesh.RecalculateNormals();
      mesh.RecalculateBounds();

      return mesh;
    }

    Mesh createNormalizedPlaneXZMesh(int nSegmentsX, int nSegmentsZ, ComputePositionDelegate computePos = null, ComputeNormalDelegate computeNorm = null, ComputeUVDelegate computeUV = null)
    {
      int nVertices = (nSegmentsX + 1) * (nSegmentsZ + 1);

      Mesh mesh = new Mesh();
      mesh.name = "StripXZ";
      mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

      Vector3[] vertices = new Vector3[nVertices];
      int[] triangles = new int[nSegmentsX * nSegmentsZ * 6];
      Vector3[] normals = new Vector3[nVertices];
      Vector2[] uv = new Vector2[nVertices];

      // Vertices generation
      for (int i = 0; i < nSegmentsZ + 1; i++)
      {
        float kZ = (float)i / nSegmentsZ;

        for (int j = 0; j < nSegmentsX + 1; j++)
        {
          float kX = ((float)j / nSegmentsX);
          int idx = i * (nSegmentsX + 1) + j;

          vertices[idx] = computePos != null ? computePos(kX, kZ) : (new Vector3(kX, 0, kZ));
          normals[idx] = computeNorm != null ? computeNorm(kX, kZ) : Vector3.up;
          uv[idx] = computeUV != null ? computeUV(kX, kZ) : new Vector2(kX, kZ);
        }
      }

      // Create GridXZ Mesh triangles
      for (int i = 0; i < nSegmentsZ; i++)
      {
        for (int j = 0; j < nSegmentsX; j++)
        {
          int p0 = j + i * (nSegmentsX + 1);
          int p1 = p0 + 1;
          int p2 = p0 + nSegmentsX + 1;
          int p3 = p2 + 1;

          int idx = (j * 6) + (6 * nSegmentsX) * i;

          triangles[idx] = p0;
          triangles[idx + 1] = p2;
          triangles[idx + 2] = p1;

          triangles[idx + 3] = p1;
          triangles[idx + 4] = p2;
          triangles[idx + 5] = p3;
        }
      }

      mesh.vertices = vertices;
      mesh.triangles = triangles;
      mesh.normals = normals;
      mesh.uv = uv;

      mesh.RecalculateBounds();
      mesh.RecalculateNormals();

      return mesh;
    }

    Vector3 computeSpherePosition(float radius, float kX, float kZ)
    {
      float coeff = Mathf.PI * 2;
      return CoordConvert.SphericalToCartesian(new Spherical(radius, coeff * kX, coeff * kZ));
    }

    Vector3 computeTorusPosition(float bigR, float smallR, float kX, float kZ)
    {
      float coeff = Mathf.PI * 2;

      Cylindrical omegaCyl = new Cylindrical(bigR, coeff * kX, 0);
      Vector3 omegaCar = CoordConvert.CylindricalToCartesian(omegaCyl);
      Vector3 pCar = omegaCar.normalized * smallR * Mathf.Cos(coeff * kZ) + Vector3.up * smallR * Mathf.Sin(coeff * kZ);

      return omegaCar + pCar;
    }

    Vector3 computeHelixPosition(float bigR, float smallR, int nTurns, float kX, float kZ)
    {
      float coeff = Mathf.PI * 2;

      Cylindrical omegaCyl = new Cylindrical(bigR, nTurns * coeff * kX, 0);
      Vector3 omegaCar = CoordConvert.CylindricalToCartesian(omegaCyl);
      Vector3 pCar = omegaCar.normalized * smallR * Mathf.Cos(coeff * kZ) + Vector3.up * smallR * Mathf.Sin(coeff * kZ);

      return omegaCar + pCar + Vector3.up * kX * smallR * 2 * nTurns;
    }

    Mesh createHeightmapPlane(int nSegmentsX, int nSegmentsZ, float length, float width, float scale)
    {
      Mesh plane = createNormalizedPlaneXZMesh(nSegmentsX, nSegmentsZ, (kX, kZ) =>
      {
        Vector3 scaled = new Vector3(kX * width, 0, kZ * length);

        float y = m_HeightMap.GetPixel(
                  (int)(kX * m_HeightMap.width),
                  (int)(kZ * m_HeightMap.width)
              ).grayscale;

        return scaled - Vector3.up * y * m_Yscale + Vector3.up * m_Yscale;
      });

      plane.RecalculateBounds();
      plane.RecalculateNormals();

      return plane;
    }

    Vector3 applyHeightMap(Texture2D hm, float kX, float kZ)
    {
      Vector3 scaled = new Vector3(kX, 0, kZ);
      float y = hm.GetPixel((int)(kX * m_HeightMap.width), (int)(kZ * m_HeightMap.width)).grayscale;

      Vector3 res = (scaled + Vector3.up * m_Yscale) - Vector3.up * y * m_Yscale;

      return res;
    }

    Vector3 computeSplinePosition(AnimationCurve width, float kX, float kZ)
    {
      Vector3 pt = m_Spline.interp(kZ);
      Vector3 tangent = (m_Spline.interp(kZ + 0.001f) - pt).normalized;
      Vector3 ortho = Vector3.Cross(tangent, Vector3.forward);

      return pt + ortho * (kX - 0.5f) * .5f * width.Evaluate(kZ);
    }

    Vector3 computeTreePosition(int height, int width, float kX, float kZ)
    {
      float coeff = Mathf.PI * 2;
      Cylindrical cyl = new Cylindrical(width * (1 - kZ) + Random.value, coeff * kX, coeff * kZ);

      return MyMathTools.CoordConvert.CylindricalToCartesian(cyl);
    }

    Vector3 computerNormalizedCube(int height, int width, float kX, float kZ)
    {
      float y;
      float part = 1.0f / 4.0f;
      float z = (1 - kZ) * part;

      if (kX <= part)
      {
        y = kX;
        return new Vector3(0, y, z);
      }
      else if (part <= kX && kX <= 2 * part)
      {
        y = part;
        return new Vector3(kX - part, y, z);
      }
      else if (2 * part <= kX && kX <= 3 * part)
      {
        y = 3 * part - kX;
        return new Vector3(part, y, z);
      }
      else
      {
        y = 0;
        return new Vector3(part - (1 - kX), y, z);
      }
    }
  }
}
