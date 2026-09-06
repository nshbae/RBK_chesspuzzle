using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapData
{
    public int playerpos;
    public int playerInd;
    public int Moves;
    public playerPiece[] playerCycle;
    public Block[] BlockData;
    public Enemy[] EnemyData;
}
public class TutorialManager : MonoBehaviour
{
    public MainManager MM;
    public List<string> MapList;
    public MapData GetMap(int i)
    {
        MapData ans = new MapData();
        string[] nowData = MapList[i].Split(']');
        string[] nowSubData = nowData[0].Split(',');
        ans.playerpos = int.Parse(nowSubData[0]);
        ans.playerInd = int.Parse(nowSubData[1]);
        ans.Moves = int.Parse(nowSubData[2]);

        nowSubData = nowData[1].Split(',');
        ans.playerCycle = new playerPiece[nowSubData.Length];
        for(int j = 0; j < ans.playerCycle.Length; j++)
        {
            ans.playerCycle[j] = (playerPiece)(int.Parse(nowSubData[j]));
        }

        nowSubData = nowData[2].Split(',');
        string[] nowSubData2 = nowData[3].Split(',');
        ans.BlockData = new Block[nowSubData.Length];
        ans.EnemyData = new Enemy[nowSubData.Length];
        for (int j = 0; j < nowSubData.Length; j++)
        {
            ans.BlockData[j] = (Block)int.Parse(nowSubData[j]);
            ans.EnemyData[j] = (Enemy)int.Parse(nowSubData2[j]);
        }

        return ans;
    }
}
