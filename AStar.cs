using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar :MonoBehaviour
{
    int mapX = 10;  
    int mapY = 10;   
    int[,] mMap; //总地图
    List<int[]> mDisPoint;//障碍物

    Gride startPo;
    Gride endPo;
    List<Gride> openList = new List<Gride>();  //开放列表
    List<Gride> closeList = new List<Gride>(); //关闭列表
    void Start()
    {
        mDisPoint = new List<int[]>();
        mDisPoint.Add(new int[2]{2,2});
        mDisPoint.Add(new int[2]{2,3});
        mDisPoint.Add(new int[2]{2,4});
        mDisPoint.Add(new int[2]{3,2});
        mDisPoint.Add(new int[2]{3,4});
        mDisPoint.Add(new int[2]{4,2});
        mDisPoint.Add(new int[2]{4,3});
        mDisPoint.Add(new int[2]{4,4});

        mMap = new int[mapX, mapY];

        int[] arr = new int[2];
        for(int i = 0; i < mapX; i++){
            for (int j = 0; j < mapY; j++){
                arr[0] = i;
                arr[1] = j;
                if(IsDisPointHave(i, j)) mMap[i,j] = 0;
                else mMap[i,j] = 1;
            }
        }

        startPo = new Gride(0,0);
        endPo = new Gride(3, 5);

        FindPath(startPo, endPo);
        
        PrintMap();
    }  

    void FindPath(Gride _startPo, Gride _endPo)
    {
        openList.Add(_startPo);
        
        Gride gr;
        while((gr = GetMinGr()) != null)
        {
            CheckGride(gr); 
            if(openList.Count == 0 || IsOpenListHave(_endPo))
            {
                break;
            }
        }

        gr = GetEndGr();
        if(gr == null)
        {
            Debug.LogError("寻路失败");
            return;
        }
        while((gr = GetGrideFather(gr)) != null){
            mMap[gr.x, gr.y] = 9;
        }
    }

    /// <summary>
    /// 处理当前格子周围的格子
    /// </summary>
    /// <param name="gr"></param>
    void CheckGride(Gride gr){
        for(int i = gr.x - 1; i <= gr.x + 1; i++)
        {
            if(i< 0 || i> mapX) continue;
            for(int j = gr.y - 1; j <= gr.y + 1; j++)
            {
                if(j< 0 || j > mapY) continue;
                if (IsDisPointHave(i,j) || (i == gr.x && j == gr.y) || IsCloseListHave(gr)) continue;

                Gride gride = new Gride(i, j);
                ComputeGAndH(gride, gr);
                openList.Add(gride);
            }
        }
        openList.Remove(gr);        
        closeList.Add(gr);
    }

    /// <summary>
    /// 当前格子的g和h值以及是否刷新父节点
    /// </summary>
    /// <param name="curGr"></param>
    /// <param name="lastGr"></param>
    void ComputeGAndH(Gride curGr, Gride lastGr){
        int g = GetG(curGr, lastGr);

        if(curGr.father == null) {
            curGr.father = lastGr;
            curGr.g = g;
        }    
        else if(g + lastGr.g < curGr.g){
            curGr.g = g + lastGr.g;
            curGr.father = lastGr;
        }

        curGr.h = (Mathf.Abs(endPo.y - curGr.y) + Mathf.Abs(endPo.x - curGr.x))* 10; 
    }

    /// <summary>
    /// 上个格子到当前格子的g值
    /// </summary>
    /// <param name="curGr"></param>
    /// <param name="lastGr"></param>
    /// <returns></returns>
    int GetG(Gride curGr, Gride lastGr){
        if(curGr.x == lastGr.x || curGr.y == lastGr.y) return 10;
        else return 14;
    }

    /// <summary>
    /// 获取map中f值最小的
    /// </summary>
    /// <returns></returns>
    Gride GetMinGr(){
        Gride gr = null;
        foreach(Gride gride in openList){
            if(gr == null){
                gr = gride;
            }else if(gride.g + gride.h < gr.g+ gr.h){
                gr = gride;
            }
        }
        return gr;
    }

    Gride GetGrideFather(Gride gride){
        return gride.father;
    }
    Gride GetEndGr(){
        foreach(Gride gr in openList){
            if((gr.x == endPo.x) && (gr.y == endPo.y)){
                return gr;
            }
        }
        return null;
    }

    /// <summary>
    /// 开放列表是否包含
    /// </summary>
    /// <param name="gr"></param>
    /// <returns></returns>
    bool IsOpenListHave(Gride gr){
        foreach(Gride gride in openList){
            if(gride.x == gr.x && gride.y == gr.y) return true;
        }
        return false;
    }
    bool IsCloseListHave(Gride gr){
        foreach(Gride gride in closeList){
            if(gride.x == gr.x && gride.y == gr.y) return true;
        }
        return false;
    }
    //这个点是否是障碍物
    bool IsDisPointHave(int x, int y)
    {
        foreach(int[] arr in mDisPoint)
        {
            if(arr[0] == x && arr[1] == y)
            {
                return true;
            }
        }
        return false;
    }

    void PrintMap(){
        string str;
        for(int i = 0; i<mapX; i++)
        {
            str = "";
            for(int j = 0; j < mapY; j++)
            {
                if((i == startPo.x&& j == startPo.y) ||(i == endPo.x && j == endPo.y)) mMap[i, j] = 2;
                str = str + "  " + mMap[i,j];
            }
            Debug.Log(str + "   " + i);
        }
    }
}

class Gride{
    public int x;
    public int y;
    public int g;
    public int h;

    public Gride father;

    public Gride(){

    } 
    public Gride(int _x, int _y){
        x = _x;
        y = _y;
    }
}