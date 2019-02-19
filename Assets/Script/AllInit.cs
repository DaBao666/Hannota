using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


public class AllInit : MonoBehaviour {
    private bool bFirst = true;
    public double r, R, h;
    public int totolPlate = 4;
    public int rMax;
    public int rMin;
    private bool drag = false;
    GameObject selectedRing = null;
    readonly int Left = 0;
    readonly int Mid = 1;
    readonly int Right = 2;
    private Stack<GameObject>[] gameObjectsStack = new Stack<GameObject>[] {new Stack<GameObject>(),new Stack<GameObject>(),new Stack<GameObject>()};

    private struct PillarStruct
    {
       public Vector3 postion;
       public float h;
    }

    private List<PillarStruct> GetPillarPos()
    {
        var PillarAPosAndH = GetPillarPosition("Pillars/PillarA");
        var PillarBPosAndH = GetPillarPosition("Pillars/PillarB");
        var PillarCPosAndH = GetPillarPosition("Pillars/PillarC");
        List<PillarStruct> PillarPosTmp = new List<PillarStruct>();
        PillarPosTmp.Add(PillarAPosAndH);
        PillarPosTmp.Add(PillarBPosAndH);
        PillarPosTmp.Add(PillarCPosAndH);
        return PillarPosTmp;
    }

    private PillarStruct GetPillarPosition (string url)
    {
        var obj = GameObject.Find(url);
        if (obj == null)
            return new PillarStruct();
        PillarStruct tmp = new PillarStruct();
        tmp.postion = obj.transform.position;
        tmp.h = (float)obj.GetComponent<Ring>().h;
        return tmp;
    }


    List<PillarStruct> PillarPos = new List<PillarStruct>();

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (totolPlate < 0)
        {
            return;
        }
        if (bFirst)
        {
            var CylinderPlaneSample = GameObject.Find("CylinderPlaneSample");
            Stack<GameObject> tmpStack = new Stack<GameObject>();
            if (CylinderPlaneSample == null)
            {
                return;
            }

            var stepR = (float)(CylinderPlaneSample.GetComponent<Collider>().bounds.size.x - 0.5f * CylinderPlaneSample.GetComponent<Collider>().bounds.size.x) / (totolPlate - 1);
            var stepH = 2.0f * CylinderPlaneSample.GetComponent<Collider>().bounds.size.y;
            (CylinderPlaneSample.GetComponent<AddAttribute>() as AddAttribute).ID = totolPlate;
            (CylinderPlaneSample.GetComponent<AddAttribute>() as AddAttribute).whichDir = Mid;
            PillarPos = GetPillarPos();

            var accumulation = 0f;
            var accumulationX = 1f;
            var rationR = Mathf.Pow(0.3f,1f/ (totolPlate-1));
            
            for (int i = totolPlate; i > 0 ; i--)
            {
                accumulationX *= rationR;
                //Debug.Log(rationR);
                accumulation = accumulation + stepH;
                var CylinderPlaneSingleOne = GameObject.Instantiate(CylinderPlaneSample);
                CylinderPlaneSingleOne.transform.position = CylinderPlaneSample.transform.position + new Vector3(0, accumulation, 0);
                CylinderPlaneSingleOne.transform.localScale =
                    new Vector3(CylinderPlaneSample.transform.localScale.x * accumulationX,
                    CylinderPlaneSample.transform.localScale.y,
                    CylinderPlaneSample.transform.localScale.z * accumulationX);
                (CylinderPlaneSingleOne.GetComponent<AddAttribute>() as AddAttribute).ID = i;
                (CylinderPlaneSingleOne.GetComponent<AddAttribute>() as AddAttribute).whichDir = Mid;
                tmpStack.Push(CylinderPlaneSingleOne);
            }
            gameObjectsStack[Mid] = tmpStack;
            
            CylinderPlaneSample.SetActive(false);
            bFirst = false;
        }

        if (Input.GetMouseButton(0) & !drag)//left click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  //camare2D.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Ring")
                {
                     if((hit.collider.gameObject.GetComponent<AddAttribute>() as AddAttribute).ID == (gameObjectsStack[(hit.collider.gameObject.GetComponent<AddAttribute>() as AddAttribute).whichDir].Peek().GetComponent<AddAttribute>() as AddAttribute).ID)
                    {
                        selectedRing = hit.collider.gameObject;
                        gameObjectsStack[(hit.collider.gameObject.GetComponent<AddAttribute>() as AddAttribute).whichDir].Pop();
                        drag = true;
                    }
                }
            }
        }

        if (Input.GetMouseButton(1) & drag)//right click => DragOff
        {
            if(selectedRing & FuncDragOff())
            {
                drag = false;
            }
            
        }

        if (drag & selectedRing) //Drag
        {
            Vector3 Pos = Camera.main.WorldToScreenPoint(selectedRing.transform.position);// 获取目标对象当前的世界坐标系位置，并将其转换为屏幕坐标系的点
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Pos.z);// 设置鼠标的屏幕坐标向量，用上面获得的Pos的z轴数据作为鼠标的z轴数据，使鼠标坐标与目标对象坐标处于同一层面上
            selectedRing.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }
    }

    private bool FuncDragOff() {
        float[] arraytmp = new float[]{
                Vector3.Distance(selectedRing.transform.position, PillarPos[Left].postion),
                Vector3.Distance(selectedRing.transform.position, PillarPos[Mid].postion),
                Vector3.Distance(selectedRing.transform.position, PillarPos[Right].postion)
                };
        int selectedWhich = Array.IndexOf(arraytmp, arraytmp.Min());
        
        selectedRing.transform.position = new Vector3(
            PillarPos[selectedWhich].postion.x,
            PillarPos[selectedWhich].h,
            PillarPos[selectedWhich].postion.z
        );

        int idstate;
        if (gameObjectsStack[selectedWhich].Count == 0)
            idstate = int.MaxValue;
        else
            idstate = (gameObjectsStack[selectedWhich].Peek().GetComponent<AddAttribute>() as AddAttribute).ID;

       int  selectedRingID = (selectedRing.GetComponent<AddAttribute>() as AddAttribute).ID;

        if(idstate > selectedRingID)
        {
            (selectedRing.GetComponent<AddAttribute>() as AddAttribute).whichDir = selectedWhich;
            gameObjectsStack[selectedWhich].Push(selectedRing);
            return true;
        }
        return false;


    }

    
}
