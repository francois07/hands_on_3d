using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class SplineTest : MonoBehaviour
{
    [SerializeField] Transform[] m_splineCtrlPts;
    LTSpline m_spline;

    // Start is called before the first frame update
    void Awake()
    {
        m_spline = new LTSpline(m_splineCtrlPts.Select((Transform t) => t.position).ToArray());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (m_spline == null || m_spline.pts.Length == 0) return;

        Gizmos.color = Color.green;

        int nPts = 100;
        Vector3 currPos = m_spline.interp(0);

        for (int i = 0; i < nPts; i++)
        {
            Vector3 pos = m_spline.interp((float)(i + 1) / (nPts - 1));
            Gizmos.DrawSphere(pos, .125f);

            if (i < nPts - 1)
            {
                Gizmos.DrawLine(currPos, pos);
            }

            currPos = pos;
        }
    }
}
