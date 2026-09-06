using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShareData
{
    public int playerpos;
    public int playerind;
    public playerPiece[] pp;
    public int Moves;
    public int Chapter;
    public Block[] blockMap;
    public Enemy[] enemyMap;
    public int[] solution;
    public int key;
}

public class ShareManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public string dat2string(ShareData dat)
    {
        string ans = "";
        ans += (char)(dat.playerpos + 33);
        ans += '|';
        ans += (char)(dat.playerind + 33);
        ans += '|';
        return ans;
    }
}
