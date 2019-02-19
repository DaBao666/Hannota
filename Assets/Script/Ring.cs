using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Ring : MonoBehaviour {
    public double r, R, h;
    public string dirction;
    private int startangle = 0, endangle = 360;
    private bool bFirst = true;
    // Use this for initialization
    void Start() {
        
    }

    // Update is called once per frame
    void Update () {
        if (bFirst)
        {
            var PillarsB = GameObject.Find("Pillars/PillarB");
            if (PillarsB == null)
            {
                return;
            }

            var GameObjectItem = GameObject.Find("GameObject").GetComponent<AllInit>() as AllInit;
            h = GameObjectItem.totolPlate * 0.2;
            GetRing();
            var offPositionCylinder = new Vector3(0, 0, 0);
            if (dirction == "Left")
            {
                offPositionCylinder = new Vector3(5, 0, 0);
            }
            else if (dirction == "Right")
            {
                offPositionCylinder = new Vector3(-5, 0, 0);
            }
            else if (dirction == "Mid")
            {
                offPositionCylinder = new Vector3(0, 0, 0);
            }
            gameObject.transform.position = PillarsB.transform.position - offPositionCylinder;
            bFirst = false;
        }
	}

    public void GetRing()
    {
        if (r < 0 | R < 0 | h < 0)
        {
            return;
        }

        List<Vector3> verticesallList = null;
        List<int> trianglesallList = null;
        GetInOutUpDown(ref verticesallList, ref trianglesallList);

        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = mf.mesh;
        mesh.Clear();
        mesh.vertices = verticesallList.ToArray();
        mesh.triangles = trianglesallList.ToArray();
        verticesallList.Clear();
        trianglesallList.Clear();
    }
   
    private void GetInOutUpDown(ref List<Vector3> verticesList, ref List<int> trianglesList)
    {
        int bins = 36;

        List<Vector3> verticesrList = new List<Vector3>();
        List<int> trianglesrList = new List<int>();
        GetVerticesAndTriangles(ref verticesrList, ref trianglesrList, bins, startangle, endangle, r);//内环

        List<Vector3> verticesRList = new List<Vector3>();
        List<int> trianglesRList = new List<int>();
        GetVerticesAndTriangles(ref verticesRList, ref trianglesRList, bins, startangle, endangle, R);//外环

        //整理内外环点
        verticesList = new List<Vector3>();
        verticesList.AddRange(verticesrList);
        verticesList.AddRange(verticesRList);

        //整理内外环三角形
        trianglesList = new List<int>();
        trianglesList.AddRange(trianglesrList);
        for (int i = 0; i < trianglesRList.Count; i++)
        {
            trianglesRList[i] = trianglesRList[i] + verticesrList.Count;
        }
        trianglesList.AddRange(trianglesRList);

        //整理底面三角形
        for (int i = 0; i < bins; i++)
        {
            if (i == bins - 1)
                break;
            int[] triangletmp = new int[] {
             i + 2 * bins,
            i,
            (i + 1) % bins,

              i + 2 * bins,
            (i + 1) % bins,
            i,


            (i + 1) % bins,
            (i + 1) % bins + 2 * bins,
            i + (2 * bins),

            (i + 1) % bins,
            i + (2 * bins),
            (i + 1) % bins + 2 * bins,
            };
            trianglesList.AddRange(triangletmp);
        }
       
        //整理顶面三角形
        for (int i = 0; i < bins; i++)
        {
            if (i == bins - 1)
                break;
            int[] triangletmp = new int[] {
            i + 3 * bins,
            i + bins,
            (i + 1) % bins + 3 * bins,

            i + bins,
            i + 3 * bins,
            (i + 1) % bins + 3 * bins,

            i + bins,
            (i + 1) % bins + bins,
            (i + 1) % bins + 3 * bins,

            (i + 1) % bins + bins,
            i + bins,
            (i + 1) % bins + 3 * bins,
        };
            trianglesList.AddRange(triangletmp);
        }
    }
    private void GetVerticesAndTriangles(ref List<Vector3> verticesList, ref List<int> trianglesList, int bins = 10, int startangle = 0, int endangle = 360, double r = 1)
    {
        Vector3[] vertices = new Vector3[2 * bins];
        int[] trianglesrsideonepositive = new int[3 * (bins - 1)];
        int[] trianglesrsidetwopositive = new int[3 * (bins - 1)];
        int[] trianglesrsideonenegative = new int[3 * (bins - 1)];
        int[] trianglesrsidetwonegative = new int[3 * (bins - 1)];

        for (int i = 0; i < bins; i++)
        {
            var x = (float)(Mathf.Cos( (Mathf.PI /180) * (endangle - startangle)  / (bins - 1) * i + (Mathf.PI / 180) * startangle) * r);
            var y = (float)h;
            var z = (float)(Mathf.Sin( (Mathf.PI / 180) * (endangle - startangle) / (bins -1) * i + (Mathf.PI / 180) * startangle) * r);

            vertices[i] = new Vector3(x, 0, z);///点
            vertices[i + bins] = new Vector3(x, y, z);

            if (i == bins - 1)
            {
                break;
            }
            int[] trianglesrtmp = new int[] {
                                                i, i + bins, (i + 1) % bins,///顺时针

                                                i + bins, i, (i + 1) % bins,///逆时针

                                                i + bins, (i + 1) % bins + bins, (i + 1) % bins,///顺时针

                                                i + bins, (i + 1) % bins, (i + 1) % bins + bins,///逆时针
            };
            trianglesList.AddRange(trianglesrtmp);
        }
        verticesList.AddRange(vertices);
    }

}
