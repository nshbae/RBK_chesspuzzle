using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct moveHistory
{
    public int stageMoves;
    public Block[] groundMap;
    public Enemy[] enemyMap;
    public int playerPos;
    public int nowCycle;
    public int collectedCoin;
    public int collectedRuby;
    public string instruction;

    public moveHistory(int stageMoves, Block[] groundMap, Enemy[] enemyMap, int playerPos, int nowCycle, int collectedCoin, int collectedRuby, string instruction)
    {
        this.stageMoves = stageMoves;
        Block[] tmp = new Block[groundMap.Length];
        Enemy[] tmp2 = new Enemy[groundMap.Length];
        for (int i = 0; i < tmp.Length; i++)
        {
            tmp[i] = groundMap[i];
            tmp2[i] = enemyMap[i];
        }
        this.groundMap = tmp;
        this.enemyMap = tmp2;
        this.playerPos = playerPos;
        this.nowCycle = nowCycle;
        this.collectedCoin = collectedCoin;
        this.collectedRuby = collectedRuby;
        this.instruction = instruction;
    }
}

public class HIstoryManager : MonoBehaviour
{
    public MainManager MM;
    public List<moveHistory> moveHistoryList;
    // Start is called before the first frame update
    void Start()
    {
        //moveHistoryList = new List<moveHistory>();
    }

    public void ClearHistory()
    {
        moveHistoryList.Clear();
        //Debug.Log("cleared");
    }


    public void AddHistory()
    {
        for(int i = 0; i < moveHistoryList.Count; i++)
        {
            if(moveHistoryList[i].stageMoves >= MM.PM.stageMoves)
            {
                moveHistoryList.RemoveAt(i);
                i--;
                //Debug.Log("removed");
            }
        }
        string instr = "";
        if (MM.Map.nowChapter == Chapter.tutorial)
        {
            instr = MM.UM.tutorialInstr[MM.Map.stageCount];
        }
        moveHistory mh = new moveHistory(MM.PM.stageMoves, MM.Map.groundMap, MM.Map.enemyMap, MM.PM.playerPos, MM.PM.nowCycle, MM.Map.collectedCoin, MM.Map.collectedRuby, instr);
        moveHistoryList.Add(mh);
        //Debug.Log(MM.PM.stageMoves + ", " + (int)MM.Map.groundMap[0] + ", " + (int)MM.Map.enemyMap[0] + ", " + MM.PM.playerPos + ", " + MM.PM.nowCycle + ", " + MM.Map.collectedCoin + ", " + MM.Map.collectedRuby + ", " + instr);
    }

    public moveHistory Undo()
    {
        moveHistory mh = new moveHistory();
        if (MM.PM.stageMoves >= 1)
        {
            mh = moveHistoryList[(int)MM.PM.stageMoves - 1];
        }
        else
        {
            mh = new moveHistory(-1, null, null, -1, -1, -1, -1, "");
        }
        //Debug.Log("called for undo data");
        return mh;
    }
    public moveHistory Redo()
    {
        moveHistory mh = new moveHistory();
        if (MM.PM.stageMoves < moveHistoryList.Count)
        {
            mh = moveHistoryList[(int)MM.PM.stageMoves + 1];
        }
        else
        {
            mh = new moveHistory(-1, null, null, -1, -1, -1, -1, "");
        }
        return mh;
    }
}
