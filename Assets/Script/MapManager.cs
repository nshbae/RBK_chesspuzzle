using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct reachableList
{
    public List<int> reachableBlockList;
    public List<wormHoleData> whDataList;
}


[System.Serializable]
public struct Move
{
    public playerPiece pp;
    public int startPos;
    public int endPos;
    public bool isPortal;
    public int portalIn;
    public int portalOut;
    public List<int> usedPath;

    public Move(playerPiece pp, int startPos, int endPos, bool isPortal, int portalIn, int portalOut)
    {
        this.pp = pp;
        this.startPos = startPos;
        this.endPos = endPos;
        this.isPortal = isPortal;
        this.portalIn = portalIn;
        this.portalOut = portalOut;
        usedPath = new List<int>();
    }
}

[System.Serializable]
public struct wormHoleData
{
    public int whIn;
    public int whOut;
    public List<int> reachableBlocks;

    public wormHoleData(int uwhI, int uwhO)
    {
        this.whIn = uwhI;
        this.whOut = uwhO;
        this.reachableBlocks = new List<int>();
    }
}


[System.Serializable]
public struct ChapterSprites
{
    public Sprite GroundSpriteW;
    public Sprite GroundSpriteB;
    public Sprite ObstacleW;
    public Sprite ObstacleB;
    public Sprite[] specialW;
    public Sprite[] specialB;
}


public enum Chapter
{
    tutorial,
    volcano,
    library,
    desert,
    forest,
    cave,
    sea,
    space,
    snow
}
public enum Block
{
    obstacle,
    ground,
    enemy,
    special,
    special1,
    special2,
    special3
}

public enum Enemy
{
    empty,
    pawn,
    rook,
    bishop,
    knight,
    rb,
    bk,
    kr
}

[System.Serializable]
public struct DiffPhase
{
    public int PhaseLength;
    public float MovesI;
    public float extraBlockRatioI;
    public float specialBlockRatioI;
    public float enemyCountI;
    public float eliteRatioI;
    public float MovesF;
    public float extraBlockRatioF;
    public float specialBlockRatioF;
    public float enemyCountF;
    public float eliteRatioF;
}

[System.Serializable]
public enum ChapterType
{
    custom,
    test,
    story
}


public class MapManager : MonoBehaviour
{
    public ChapterType CT;
    public float[] testDiff;
    public Chapter nowChapter;
    // Start is called before the first frame update
    public int stageCount;
    public float Moves;
    public float enemyCount;

    public MainManager MM;

    public SpriteRenderer[] groundGrid;
    public SpriteRenderer[] enemyGrid;
    //public Sprite[] GroundSprite;
    //public Sprite[] ObstacleSprite;
    //public Sprite[] SpecialSprite;
    public ChapterSprites[] ChSprite; 
    public Sprite[] EnemySprite;
    public Color[] EnemyColors;
    public int row;
    public int col;
    public Block[] groundMap;
    public Enemy[] enemyMap;
    public List<Move> solution;

    Block[] groundMap_save;
    Enemy[] enemyMap_save;
    int[] startPosInd;

    public float extraBlockRatio;
    public float specialBlockRatio;
    public float eliteRatio;

    public DiffPhase[] diffCurve;
    public int nowDiffPhase;

    public int collectedCoin;
    public int collectedRuby;
    public float starBonus;

    public GameObject BGsprite;
    public Sprite[] BGspriteDict;

    private void Start()
    {
        MM.PPM.SetPPM();
        if (CT!=ChapterType.test)
        {
            if (PlayerPrefs.GetInt("testMode") == 0)
                CT = ChapterType.story;
            else if (PlayerPrefs.GetInt("testMode") == 2)
                CT = ChapterType.custom;
            if (PlayerPrefs.HasKey("nowChapter"))
            {
                nowChapter = (Chapter)PlayerPrefs.GetInt("nowChapter");
                stageCount = PlayerPrefs.GetInt("nowStage");
            }
        }

        if (PlayerPrefs.GetInt("testMode") == 0)
        {
            CT = ChapterType.story;
        }

        groundMap = new Block[64];
        enemyMap = new Enemy[64];

        groundMap_save = new Block[64];
        enemyMap_save = new Enemy[64];
        startPosInd = new int[2];

        nowDiffPhase = 0;
        MM.UM.SetStageNo(stageCount);
        SetCustomDiff();
        GenerateMap();
        BGsprite.GetComponent<SpriteRenderer>().sprite = BGspriteDict[(int)nowChapter];
        //fullEmptyMap();
        MM.SM.playBGM();
    }
    public void SetCustomDiff()
    {
        int diff = 0;
        switch (PlayerPrefs.GetInt("diff"))
        {
            case 0:
                diff = 0;
                break;
            case 1:
                diff = 2;
                break;
            case 2:
                diff = 5;
                break;
            case 3:
                diff = 8;
                break;
            default:
                break;
        }
        int[] arr = new int[] { 0, 0, 0, 0 };//난이도 분배 배열 - 추가이동, 추가블록, 특수블록, 엘리트
        switch (diff)
        {
            case 0:
                arr = new int[] { 0, 0, 0, 0 };
                break;
            case 2:
                arr = new int[] { 0, 1, 0, 0 };
                diff--;
                break;
            case 5:
                arr = new int[] { 0, 1, 1, 0 };
                diff -= 2;
                break;
            case 8:
                arr = new int[] { 0, 1, 1, 1 };
                diff -= 3;
                break;
            default:
                break;
        }
        for (int i = 0; i < diff; i++)
        {
            int ind;
            do
            {
                ind = Random.Range(0, 3);
            } while (arr[ind + 1] >= 3);
            arr[ind + 1] = arr[ind + 1] + 1;
        }
        Debug.Log("추가이동, 추가블록, 특수블록, 엘리트" + arr[0] + ", " + arr[1] + ", " + arr[2] + ", " + arr[3]);

        enemyCount = PlayerPrefs.GetInt("length");
        switch (PlayerPrefs.GetInt("length"))
        {
            case 0:
                enemyCount = 3;
                Moves = 4;
                break;
            case 1:
                enemyCount = 6;
                Moves = 9;
                break;
            case 2:
                enemyCount = 10;
                Moves = 14;
                break;
            case 3:
                enemyCount = 15;
                Moves = 21;
                break;
            default:
                enemyCount = 3;
                Moves = 4;
                break;
        }
        /*switch (arr[0])//추가 이동
        {
            case 0:
                Moves = PlayerPrefs.GetInt("length") + 1;
                break;
            case 1:
                Moves = PlayerPrefs.GetInt("length") + 2;
                break;
            case 2:
                Moves = PlayerPrefs.GetInt("length") + 3;
                break;
            case 3:
                Moves = PlayerPrefs.GetInt("length") + 4;
                break;
            default:
                Moves = PlayerPrefs.GetInt("length");
                break;
        }*/
        switch (arr[1])//추가 블록
        {
            case 0:
                extraBlockRatio = 1f;
                break;
            case 1:
                extraBlockRatio = 0.6f;
                break;
            case 2:
                extraBlockRatio = 0.4f;
                break;
            case 3:
                extraBlockRatio = 0.1f;
                break;
            default:
                extraBlockRatio = 1f;
                break;
        }
        switch (arr[2])// 특수블록
        {
            case 0:
                specialBlockRatio = 0f;
                break;
            case 1:
                specialBlockRatio = 0.3f;
                break;
            case 2:
                specialBlockRatio = 0.6f;
                break;
            case 3:
                specialBlockRatio = 0.9f;
                break;
            default:
                specialBlockRatio = 0f;
                break;
        }
        switch (arr[3])//엘리트
        {
            case 0:
                eliteRatio = 0f;
                break;
            case 1:
                eliteRatio = 0.2f;
                break;
            case 2:
                eliteRatio = 0.5f;
                break;
            case 3:
                eliteRatio = 1f;
                break;
            default:
                eliteRatio = 0f;
                break;
        }

    }
    /*
    public void SetStartDiff()
    {
        if (CT != ChapterType.test)
        {
            int netLength = 0;
            for (int i = 0; i < diffCurve.Length; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
                    netLength += diffCurve[j].PhaseLength;
                }
                if(stageCount - netLength <= 0)
                {
                    nowDiffPhase = i;
                    break;
                }
            }
            netLength = 0;
            for (int j = 0; j < nowDiffPhase; j++)
            {
                netLength += diffCurve[j].PhaseLength;
            }
            float temp = (stageCount - netLength) / (float)(diffCurve[nowDiffPhase].PhaseLength - 1);
            //Debug.Log((stageCount - netLength) + ", " + diffCurve[nowDiffPhase].PhaseLength);

            Moves = diffCurve[nowDiffPhase].MovesI + (diffCurve[nowDiffPhase].MovesF - diffCurve[nowDiffPhase].MovesI) * temp;
            //Debug.Log(diffCurve[nowDiffPhase].MovesI + "+" + diffCurve[nowDiffPhase].MovesF + "-" + diffCurve[nowDiffPhase].MovesI + "*" + temp);
            extraBlockRatio = diffCurve[nowDiffPhase].extraBlockRatioI + (diffCurve[nowDiffPhase].extraBlockRatioF - diffCurve[nowDiffPhase].extraBlockRatioI) * temp;
            specialBlockRatio = diffCurve[nowDiffPhase].specialBlockRatioI + (diffCurve[nowDiffPhase].specialBlockRatioF - diffCurve[nowDiffPhase].specialBlockRatioI) * temp;
            enemyCount = diffCurve[nowDiffPhase].enemyCountI + (diffCurve[nowDiffPhase].enemyCountF - diffCurve[nowDiffPhase].enemyCountI) * temp;
            eliteRatio = diffCurve[nowDiffPhase].eliteRatioI + (diffCurve[nowDiffPhase].eliteRatioF - diffCurve[nowDiffPhase].eliteRatioI) * temp;
        }
        else
        {
            Moves = testDiff[0];
            extraBlockRatio = testDiff[1];
            specialBlockRatio = testDiff[2];
            enemyCount = testDiff[3];
            eliteRatio = testDiff[4];
        }
    }
    */
    public void upDifficulty()
    {
        stageCount++;
        if(CT == ChapterType.custom)
        {
            SetCustomDiff();
        }
        /*
        if (CT != ChapterType.test)
        {
            int length = diffCurve[nowDiffPhase].PhaseLength;
            int phaseCount = stageCount;
            for(int i = 0; i < nowDiffPhase; i++)
            {
                phaseCount -= diffCurve[i].PhaseLength;
            }
            if (phaseCount >= length)
            {
                nowDiffPhase++;
                phaseCount = 0;
            }
            if(nowDiffPhase >= diffCurve.Length)
            {
                nowDiffPhase--;
                MM.UM.ShowMsg(" 최고 난이도에 도달했습니다.\n더 어려운 맵은 지옥모드에서 도전하세요.");
            }
            else
            {
                Moves += (diffCurve[nowDiffPhase].MovesF - diffCurve[nowDiffPhase].MovesI) / length;
                extraBlockRatio += (diffCurve[nowDiffPhase].extraBlockRatioF - diffCurve[nowDiffPhase].extraBlockRatioI) / length;
                specialBlockRatio += (diffCurve[nowDiffPhase].specialBlockRatioF - diffCurve[nowDiffPhase].specialBlockRatioI) / length;
                enemyCount += (diffCurve[nowDiffPhase].enemyCountF - diffCurve[nowDiffPhase].enemyCountI) / length;
                eliteRatio += (diffCurve[nowDiffPhase].eliteRatioF - diffCurve[nowDiffPhase].eliteRatioI) / length;
            }
        }*/
    }
    public int getIndexX(int index)
    {
        return index % row;
    }
    public int getIndexY(int index)
    {
        return Mathf.FloorToInt(index / row);
    }
    public void GenerateMap()
    {
        MM.PM.RandomizePP();
        MM.PM.ResetPlayer();
        /*
        if (MM.UM.instructionText.text != "" && MM.UM.instructionText.text[0] != ' ')//지우지 않을 메세지는 시작에 스페이스 바 포함.
            MM.UM.clearInstMsg();*/
        if (CT == ChapterType.custom)//커스텀 맵이면 무작위 맵 생성
        {
            MM.UM.setSliderMoves((int)Moves);
            GenerateMap(Random.Range(0, MM.PM.playerCycle.Length));
        }
        else if (CT == ChapterType.story)//스토리 맵이면 저장된 맵 생성
        {
            if (MM.TM)
            {
                MapData MD = MM.TM.GetMap(stageCount);
                MM.PM.playerCycle = new playerPiece[MD.playerCycle.Length];
                for(int i = 0; i < MD.playerCycle.Length; i++)
                {
                    MM.PM.playerCycle[i] = MD.playerCycle[i];
                }

                MM.PM.playerPos = MD.playerpos;
                MM.PM.nowCycle = MD.playerInd;
                MM.PM.UpdatePlayerPos();
                Moves = (float)MD.Moves;
                MM.UM.setSliderMoves((int)Moves);

                for (int i = 0; i < groundMap.Length; i++)
                {
                    groundMap[i] = MD.BlockData[i];
                    enemyMap[i] = MD.EnemyData[i];
                }

                SaveGeneratedMap(MM.PM.playerPos, MM.PM.nowCycle);
                MM.PM.ResetPlayer();
                updateMap();
            }
            else
            {
                GenerateMap(Random.Range(0, MM.PM.playerCycle.Length));
            }
        }
        if(CT != ChapterType.test)
            MM.DM.CheckDialoguePhase(stageCount, false);
        starBonus = 1;

        MM.HM.ClearHistory();
        MM.HM.AddHistory();
    }
    public void GenerateMap(int finind)
    {
        MM.UM.gridNum.SetActive(false);
        MM.UM.ShowMsg("");
        MM.UM.updateMoveCount(0);
        EmptyMap();
        Block[] tempMap = new Block[64];
        for (int i = 0; i < groundMap.Length; i++)
        {
            tempMap[i] = Block.ground;
        }

        //space맵은 시작시 웜홀 설치
        if(nowChapter == Chapter.space)
        {
            if (specialBlockRatio > 0f)
            {
                List<int> wormholes = pickNonDupGroundBlocks(tempMap, 2);
                foreach (var i in wormholes)
                {
                    tempMap[i] = Block.special;
                    groundMap[i] = Block.special;
                    Debug.Log("sp0 " + i.ToString());
                }
            }
            if (specialBlockRatio >= 0.3f)
            {
                List<int> wormholes = pickNonDupGroundBlocks(tempMap, 2);
                foreach (var i in wormholes)
                {
                    tempMap[i] = Block.special1;
                    groundMap[i] = Block.special1;
                    Debug.Log("sp1 " + i.ToString());
                }
            }
            if (specialBlockRatio >= 0.7f)
            {
                List<int> wormholes = pickNonDupGroundBlocks(tempMap, 2);
                foreach (var i in wormholes)
                {
                    tempMap[i] = Block.special2;
                    groundMap[i] = Block.special2;
                    Debug.Log("sp2 " + i.ToString());
                }
            }
        }

        solution = new List<Move>();
        int finishind = finind;
        int finishPos = -1;
        do
        {
            finishPos = Random.Range(0, 64);
        } while ((int)groundMap[finishPos] >= (int)Block.special);

        groundMap[finishPos] = Block.ground;
        for (int i = (int)Moves + finishind; i > finishind; i--)
        {
            playerPiece nowpp = MM.PM.playerCycle[i % MM.PM.playerCycle.Length];
            groundMap[finishPos] = Block.enemy;
            tempMap[finishPos] = Block.enemy;
            enemyMap[finishPos] = Enemy.pawn;
            placeEnemy(finishPos, nowpp);
            int prev = finishPos;
            reachableList reachable = MM.PPM.PieceList(nowpp, finishPos, tempMap, enemyMap, false);
            Move thisMove = new Move();
            if (CT != ChapterType.test || nowpp == playerPiece.knight)
                thisMove = PickNormalMove(nowpp, reachable, prev);
            else
            {
                //thisMove = PickNormalMove(MM.PM.playerCycle[i % 3], reachable, prev);
                thisMove = PickWeightedMove(nowpp, reachable, prev);
            }
            finishPos = thisMove.endPos;
            if(finishPos == -1)
            {
                Debug.Log("fuck");
                GenerateMap(finind);
                return;
            }
            foreach (int j in thisMove.usedPath)
            {
                if((int)groundMap[j] < (int)Block.enemy)
                {
                    groundMap[j] = Block.ground;
                }
            }
            solution.Insert(0, thisMove);
        }
        groundMap[finishPos] = Block.ground;
        tempMap[finishPos] = Block.ground;
        string tmp = "";
        foreach (Move i in solution)
        {
            tmp += ((char)(getIndexX(i.endPos) + (int)'A') + (getIndexY(i.endPos) + 1).ToString() + " ");
            //tmp += (i.ToString() + ", ");
        }
        MM.PM.playerPos = finishPos;
        MM.PM.nowCycle = (finishind+1) % MM.PM.playerCycle.Length;
        MM.PM.UpdatePlayerPos();

        reduceEnemy();
        AddExtraBlocks();
        AddSpecialBlocks();

        SaveGeneratedMap(finishPos, finishind);

        tmp += ((playerPiece)finind).ToString();
        MM.PM.ResetPlayer();
        updateMap();

        Debug.Log(tmp);
    }

    Move PickNormalMove(playerPiece pp, reachableList reachable, int nowPos)
    {
        Move ansM = new Move(pp, -1, -1, false, -1, -1);
        ansM.usedPath = new List<int>();

        if (reachable.reachableBlockList.Count == 0)
        {
            Debug.Log("empty reachable List!");
            return ansM;
        }

        int finishPos;
        bool useWH = false;
        List<int> tot = new List<int>();
        foreach (var i in reachable.reachableBlockList)
        {
            tot.Add(i);
        }
        foreach (var i in reachable.whDataList)
        {
            foreach(var j in i.reachableBlocks)
            {
                if (!tot.Contains(j))
                {
                    tot.Add(j);
                }
            }
        }

        int pI = -1;
        int pO = -1;
        if (reachable.whDataList.Count == 0)
        {
            finishPos = reachable.reachableBlockList[Random.Range(0, reachable.reachableBlockList.Count)];
        }
        else
        {
            int temp = 0;
            bool haveWHpath = false;
            for (int i = 0; i < 100; i++)
            {
                temp = Random.Range(0, tot.Count);
                if (!reachable.reachableBlockList.Contains(temp) && groundMap[temp] < Block.special)
                {
                    haveWHpath = true;
                    break;
                }
            }
            if (!haveWHpath)
            {
                int breaker = 0;
                do
                {
                    temp = Random.Range(0, reachable.reachableBlockList.Count);
                    breaker++;
                } while (groundMap[temp] >= Block.special && breaker <= 999);
                if (breaker >= 999)
                {
                    Debug.Log("error code 001 - too much loop");
                }
            }

            useWH = temp >= reachable.reachableBlockList.Count ? true : false;
            finishPos = tot[temp];
        }

        for (int j = 0; j < 100; j++)
        {
            if (groundMap[finishPos] < Block.enemy)
                break;

            if (reachable.whDataList.Count == 0)
            {
                finishPos = reachable.reachableBlockList[Random.Range(0, reachable.reachableBlockList.Count)];
            }
            else
            {
                int temp = Random.Range(0, tot.Count);
                useWH = temp >= reachable.reachableBlockList.Count ? true : false;
                finishPos = tot[temp];
            }

            if (j == 99)
            {
                Debug.Log("fail to generate path.");
                return ansM;
            }
        }
        Debug.Log(pp + ", " + useWH);

        if (!useWH)
        {
            //string logTmp = "";
            foreach (var i in requiredPathBlock(nowPos, finishPos, true, pp))
            {
                ansM.usedPath.Add(i);
                //logTmp += i.ToString() + ", ";
            }
            //Debug.Log("사용한 칸 " + logTmp);
        }
        else
        {
            foreach (var i in reachable.whDataList)
            {
                foreach (var j in i.reachableBlocks)
                {
                    if (j == finishPos)
                    {
                        pI = i.whIn;
                        pO = i.whOut;
                        foreach (var k in requiredPathBlock(nowPos, pI, true, pp))
                        {
                            ansM.usedPath.Add(k);
                        }
                        foreach (var k in requiredPathBlock(pO, finishPos, true, pp))
                        {
                            if (!ansM.usedPath.Contains(k))
                            {
                                ansM.usedPath.Add(k);
                            }
                        }
                        break;
                    }
                }
            }
        }
        //Debug.Log(reachable + ", ");
        Move result = new Move(pp, nowPos, finishPos, useWH, pI, pO);
        result.usedPath = new List<int>();
        foreach(var i in ansM.usedPath)
        {
            result.usedPath.Add(i);
        }
        return result;
    }

    Move PickWeightedMove(playerPiece pp, reachableList reachable, int nowPos)
    {

        Move ansM = new Move(pp, -1, -1, false, -1, -1);
        ansM.usedPath = new List<int>();

        if (reachable.reachableBlockList.Count == 0)
        {
            Debug.Log("empty reachable List!");
            return ansM;
        }

        int finishPos;
        bool useWH = false;
        List<int> tot = new List<int>();
        foreach (var i in reachable.reachableBlockList)
        {
            tot.Add(i);
        }
        foreach (var i in reachable.whDataList)
        {
            foreach (var j in i.reachableBlocks)
            {
                if (!tot.Contains(j))
                {
                    tot.Add(j);
                }
            }
        }

        int pI = -1;
        int pO = -1;
        if (reachable.whDataList.Count == 0)
        {
            int tmp = Random.Range(0, reachable.reachableBlockList.Count);
            int tmp2 = Random.Range(0, reachable.reachableBlockList.Count);
            finishPos = reachable.reachableBlockList[tmp > tmp2 ? tmp : tmp2];
        }
        else
        {
            int temp = 0;
            bool haveWHpath = false;
            for (int i = 0; i < 100; i++)
            {
                temp = Random.Range(0, tot.Count);
                if (!reachable.reachableBlockList.Contains(temp) && groundMap[temp] < Block.special)
                {
                    haveWHpath = true;
                    break;
                }
            }
            if (!haveWHpath)
            {
                int breaker = 0;
                do
                {
                    int tmp = Random.Range(0, reachable.reachableBlockList.Count);
                    int tmp2 = Random.Range(0, reachable.reachableBlockList.Count);
                    temp = tmp > tmp2 ? tmp : tmp2;
                    breaker++;
                } while (groundMap[temp] >= Block.special && breaker<999);
                if(breaker >= 999)
                {
                    Debug.Log("error code 001 - too much loop");
                }
            }

            useWH = (temp >= reachable.reachableBlockList.Count ? true : false);
            finishPos = tot[temp];
        }

        for (int j = 0; j < 100; j++)
        {
            if (groundMap[finishPos] < Block.enemy)
                break;

            if (reachable.whDataList.Count == 0)
            {
                finishPos = reachable.reachableBlockList[Random.Range(0, reachable.reachableBlockList.Count)];
            }
            else
            {
                int temp = Random.Range(0, tot.Count);
                useWH = temp >= reachable.reachableBlockList.Count ? true : false;
                finishPos = tot[temp];
            }

            if (j == 99)
            {
                Debug.Log("fail to generate path.");
                return ansM;
            }
        }
        Debug.Log(pp + ", " + useWH);

        if (!useWH)
        {
            //string logTmp = "";
            foreach (var i in requiredPathBlock(nowPos, finishPos, true, pp))
            {
                ansM.usedPath.Add(i);
                //logTmp += i.ToString() + ", ";
            }
            //Debug.Log("사용한 칸 " + logTmp);
        }
        else
        {
            foreach (var i in reachable.whDataList)
            {
                foreach (var j in i.reachableBlocks)
                {
                    if (j == finishPos)
                    {
                        pI = i.whIn;
                        pO = i.whOut;
                        foreach (var k in requiredPathBlock(nowPos, pI, true, pp))
                        {
                            ansM.usedPath.Add(k);
                        }
                        foreach (var k in requiredPathBlock(pO, finishPos, true, pp))
                        {
                            if (!ansM.usedPath.Contains(k))
                            {
                                ansM.usedPath.Add(k);
                            }
                        }
                        break;
                    }
                }
            }
        }
        //Debug.Log(reachable + ", ");
        Move result = new Move(pp, nowPos, finishPos, useWH, pI, pO);
        result.usedPath = new List<int>();
        foreach (var i in ansM.usedPath)
        {
            result.usedPath.Add(i);
        }
        return result;
    }
    void SaveGeneratedMap(int finishPos, int finishind)
    {
        for (int i = 0; i < row * col; i++)
        {
            groundMap_save[i] = groundMap[i];
            enemyMap_save[i] = enemyMap[i];
        }
        startPosInd[0] = finishPos;
        startPosInd[1] = finishind;
    }
    
    List<int> requiredPathBlock(int startPos, int finPos, bool includeEnd, playerPiece used_pp)
    {
        List<int> ans = new List<int>();
        Block[] empty = new Block[row*col];
        Enemy[] empty_e = new Enemy[row * col];
        for(int i = 0; i < empty.Length; i++)
        {
            empty[i] = Block.ground;
            empty_e[i] = Enemy.empty;
        }
        playerPiece pp = used_pp;//룩, 비숍, 나이트 이외의 말 추가시 최적화 필요
        if(used_pp >= playerPiece.rook && used_pp <= playerPiece.unicorn && used_pp != playerPiece.empty)
        {
            ans = MM.PPM.RookList(startPos, empty, empty_e, includeEnd).reachableBlockList;
            for (int i = 0; i < ans.Count; i++)
            {
                if (ans[i] == finPos)
                {
                    pp = playerPiece.rook;
                    break;
                }
            }
            ans = MM.PPM.BishopList(startPos, empty, empty_e, includeEnd).reachableBlockList;
            for (int i = 0; i < ans.Count; i++)
            {
                if (ans[i] == finPos)
                {
                    pp = playerPiece.bishop;
                    break;
                }
            }
            ans = MM.PPM.KnightList(startPos, empty, empty_e, includeEnd).reachableBlockList;
            for (int i = 0; i < ans.Count; i++)
            {
                if (ans[i] == finPos)
                {
                    pp = playerPiece.knight;
                    break;
                }
            }
        }
        if (includeEnd)
        {
            ans = requiredPathBlockwithEnd(pp, startPos, finPos);
        }
        else
        {
            ans = requiredPathBlock(pp, startPos, finPos);
        }
        return ans;
    }

    List<int> requiredPathBlock(playerPiece pp, int startPos, int finPos)
    {
        Debug.Log(pp.ToString());
        List<int> ans = new List<int>();
        switch (pp)
        {
            case playerPiece.rook:
                if(finPos > startPos)
                {
                    if(finPos >= startPos + row)//down
                    {
                        for (int i= startPos; i < finPos; i += row)
                        {
                            ans.Add(i);
                        }
                    }
                    else//right
                    {
                        for (int i = startPos; i < finPos; i++)
                        {
                            ans.Add(i);
                        }
                    }
                }
                else
                {
                    if (finPos <= startPos - row)//up
                    {
                        for (int i = startPos; i > finPos; i -= row)
                        {
                            ans.Add(i);
                        }
                    }
                    else//left
                    {
                        for (int i = startPos; i > finPos; i--)
                        {
                            ans.Add(i);
                        }
                    }
                }
                break;
            case playerPiece.bishop:
                if (finPos > startPos)
                {
                    if (getIndexX(finPos) > getIndexX(startPos))//dr
                    {
                        for (int i = startPos; i < finPos; i += (row+1))
                        {
                            ans.Add(i);
                        }
                    }
                    else//dl
                    {
                        for (int i = startPos; i < finPos; i+= (row-1))
                        {
                            ans.Add(i);
                        }
                    }
                }
                else
                {
                    if (getIndexX(finPos) > getIndexX(startPos))//ur
                    {
                        for (int i = startPos; i > finPos; i -= (row-1))
                        {
                            ans.Add(i);
                        }
                    }
                    else//ul
                    {
                        for (int i = startPos; i > finPos; i-= (row+1))
                        {
                            ans.Add(i);
                        }
                    }
                }
                break;
            case playerPiece.knight:
            case playerPiece.deer:
            case playerPiece.jumpking:
                ans.Add(startPos);
                break;
            default:
                break;
        }
        string tmp = "path block";
        foreach(var i in ans)
        {
            tmp += i.ToString() + ", ";
        }
        Debug.Log(tmp);
        return ans;
    }
    
    List<int> requiredPathBlockwithEnd(playerPiece pp, int startPos, int finPos)
    {
        List<int> ans = new List<int>();
        ans = requiredPathBlock(pp, startPos, finPos);
        //ans.Add(startPos);
        ans.Add(finPos);
        return ans;
    }

    void placeEnemy(int index, playerPiece pp)
    {
        if(Random.Range(0f,1f) > eliteRatio)
        {
            enemyMap[index] = Enemy.pawn;
        }
        else
        {
            float temp = Random.Range(0f, 1f);
            switch (pp)
            {
                case playerPiece.rook:
                    if(temp>0.5)
                        enemyMap[index] = Enemy.bishop;
                    else
                        enemyMap[index] = Enemy.knight;
                    break;
                case playerPiece.bishop:
                    if (temp > 0.5)
                        enemyMap[index] = Enemy.rook;
                    else
                        enemyMap[index] = Enemy.knight;
                    break;
                case playerPiece.knight:
                    if (temp > 0.5)
                        enemyMap[index] = Enemy.bishop;
                    else
                        enemyMap[index] = Enemy.rook;
                    break;
                default:
                    break;
            }
        }
    }

    void reduceEnemy()
    {
        for(int i = 0; i < Moves - enemyCount; i++)
        {
            int tmp= 0;
            do
            {
                tmp = PickRndTypeBlock(groundMap, Block.enemy);
                if (tmp == -1)
                    return;
            }
            while (tmp == solution[solution.Count - 1].startPos);
            groundMap[tmp] = Block.ground;
            enemyMap[tmp] = Enemy.empty;
        }
    }

    void AddExtraBlocks()
    {
        int extraBlockNum = (int) (GetObstacleNum() * extraBlockRatio);
        for (int i = 0; i < extraBlockNum; i++)
        {
            int index = PickAdjObsBlock(groundMap);
            if (index == -1)
                return;
            if(nowChapter == Chapter.library)
            {
                if (Random.Range(0.0f, 1.0f) < specialBlockRatio)//specialBlockRatio의 확률로
                    groundMap[index] = Block.ground;//필요시 special, special1으로 바꿀 것
                else
                    groundMap[index] = Block.ground;
            }
            else
            {
                groundMap[index] = Block.ground;
            }
        }
    }

    int GetObstacleNum()
    {
        int obstacleNum = 0;
        for (int i = 0; i < groundMap.Length; i++)
        {
            if (groundMap[i] == Block.obstacle)
                obstacleNum++;
        }
        return obstacleNum;
    }
    int GetGroundNum()
    {
        int groundNum = 0;
        for (int i = 0; i < groundMap.Length; i++)
        {
            if (groundMap[i] == Block.ground)
                groundNum++;
        }
        return groundNum;
    }

    int PickAdjObsBlock(Block[] t_map)
    {
        int ans = 0;
        bool NotAdj = true;

        int count = 0;
        for(int i = 0; i < groundMap.Length; i++)
        {
            if (groundMap[i] == Block.obstacle)
            {
                count++;
            }
        }
        if(count == 0)
        {
            Debug.Log("no obstacle detected");
            return -1;
        }
        do
        {
            ans = Random.Range(0, t_map.Length);
            if (t_map[ans] == Block.obstacle)
            {
                if ((ans > 1 && t_map[ans - 1] != Block.obstacle) || (ans < row * col - 1 && t_map[ans + 1] != Block.obstacle) || (ans > row && t_map[ans - row] != Block.obstacle) || (ans < row * col - row && t_map[ans + row] != Block.obstacle))
                    NotAdj = false;
            }
        }
        while (NotAdj);
        //Debug.Log(ans);
        return ans;
    }

    int PickRndTypeBlock(Block[] t_map, Block type)
    {
        int count = 0;
        for(int i = 0; i < groundMap.Length; i++)
        {
            if (groundMap[i] == type)
            {
                count++;
            }
        }
        if (count == 0)
        {
            Debug.Log("no enemy detected");
            return -1;
        }
        int ans = Random.Range(0, t_map.Length);
        while(t_map[ans] != type)
        {
            ans = Random.Range(0, t_map.Length);
        }
        return ans;
    }

    List<int> pickNonDupGroundBlocks(Block[] t_map, int count)
    {
        List<int> arr = new List<int>();
        if (t_map.Length < count)
        {
            return arr;
        }
        for (int i = 0; i < count; i++)
        {
            int temp = -1;
            int breaker = 0;
            do
            {
                temp = Random.Range(0, t_map.Length);
                breaker++;
            } while ((t_map[temp] != Block.ground || arr.Contains(temp))&& breaker<999);

            if (breaker >= 999)
            {
                Debug.Log("error code 001 - too much loop");
            }
            arr.Add(temp);
        }
        return arr;
    }

    void AddSpecialBlocks()
    {
        switch (nowChapter)
        {
            case Chapter.volcano:
                /*int obsSpNum = (int)(GetObstacleNum() * specialBlockRatio/2);
                for (int i = 0; i < obsSpNum; i++)
                    groundMap[PickAdjObsBlock(groundMap)] = Block.special;
                int gndSpNum = (int)(GetGroundNum() * specialBlockRatio);
                for (int i = 0; i < gndSpNum; i++)
                    groundMap[PickRndTypeBlock(groundMap, Block.ground)] = Block.special;*/
                
                List<int>[] usedTurn = new List<int>[row*col];
                for(int i = 0; i < usedTurn.Length; i++)
                {
                    usedTurn[i] = new List<int>(0);
                }
                for(int i = 0; i < solution.Count-1; i++)
                {
                    List<int> usedBlock = solution[i].usedPath;
                    for(int j = 0; j < usedBlock.Count; j++)
                    {
                        if (!usedTurn[usedBlock[j]].Contains(i % 4))
                        {
                            usedTurn[usedBlock[j]].Add(i % 4);
                            //Debug.Log(usedBlock[j] + "블록 " + i + "턴에 사용 " + i % 4 + "번째 용암 불가");
                        }
                    }
                }
                for(int i = 0; i < usedTurn.Length; i++)//모든 칸에 대해
                {
                    if (groundMap[i] == Block.ground)//그 칸이 일반 땅이면
                    {
                        if(usedTurn[i].Count < 4)//4배수 중 한번이라도 안쓰이는 턴이 있으면
                        {
                            for(int j = 0; j < 4; j++)//0-3 순서대로 확인해서
                            {
                                if (!usedTurn[i].Contains(j))//4배수중 안쓰이는 턴을 확인하고
                                {
                                    if (Random.Range(0.0f, 1.0f) < specialBlockRatio)//specialBlockRatio의 확률로
                                    {
                                        if(j < 2)//
                                            groundMap[i] = (Block)(4 - j);//위상에 맞는 special block으로 바꿔준다.
                                        else//
                                            groundMap[i] = (Block)(8 - j);
                                        //Debug.Log(i + "칸은 4배수 + " + j + "번째 미사용, 코드 " + (int)groundMap[i] + "를 할당");
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                return;
            case Chapter.library:
                int[] stepedTimes = new int[row * col];
                for (int i = 0; i < solution.Count; i++)
                {
                    List<int> usedBlock = solution[i].usedPath.GetRange(0, solution[i].usedPath.Count - 1);
                    for (int j = 0; j < usedBlock.Count; j++)
                    {
                        stepedTimes[usedBlock[j]]++;
                    }
                }
                for(int i = 0; i < stepedTimes.Length; i++)
                {
                    if(groundMap[i] == Block.ground)
                    {
                        if (stepedTimes[i] > 0 && stepedTimes[i] <= 2)
                        {
                            if (Random.Range(0.0f, 1.0f) < specialBlockRatio)//specialBlockRatio의 확률로
                            {
                                if (stepedTimes[i] == 1)
                                {
                                    groundMap[i] = Block.special;
                                }
                                else
                                {
                                    groundMap[i] = Block.special1;
                                }
                            }
                        }
                    }
                }
                return;
            case Chapter.space:
                //Generate Map에서 이미 생성함
                return;
            default:
                return;
        }
    }

    Sprite getGroundSprite(int i)
    {
        if ((getIndexX(i) + getIndexY(i)) % 2 == 0)
            return ChSprite[(int)nowChapter].GroundSpriteW;
        else
            return ChSprite[(int)nowChapter].GroundSpriteB;
    }

    Sprite getObstacleSprite(int i)
    {
        if ((getIndexX(i) + getIndexY(i)) % 2 == 0)
            return ChSprite[(int)nowChapter].ObstacleW;
        else
            return ChSprite[(int)nowChapter].ObstacleB;
    }

    Sprite getSpecialSprite(int i)
    {
        if ((getIndexX(i) + getIndexY(i)) % 2 == 0)
            return ChSprite[(int)nowChapter].specialW[(int)groundMap[i] - 3];
        else
            return ChSprite[(int)nowChapter].specialB[(int)groundMap[i] - 3];
    }

    void updateMap()
    {
        for (int i = 0; i < groundMap.Length; i++)
        {
            if(groundMap[i] == Block.obstacle)
            {
                groundGrid[i].sprite = getObstacleSprite(i);
            }
            else if (groundMap[i] == Block.ground || groundMap[i] == Block.enemy)
            {
                groundGrid[i].sprite = getGroundSprite(i);
            }
            else if (groundMap[i] >= Block.special)
            {
                groundGrid[i].sprite = getSpecialSprite(i);
            }
            SetEnemyColor();
            enemyGrid[i].sprite = EnemySprite[(int)enemyMap[i]];
        }
    }

    void SetEnemyColor()
    {
        for(int i = 0; i < enemyGrid.Length; i++)
        {
            enemyGrid[i].color = EnemyColors[(int)nowChapter];
        }
    }

    void EmptyMap()
    {
        for (int i = 0; i < groundMap.Length; i++)
        {
            groundMap[i] = Block.obstacle;
            enemyMap[i] = Enemy.empty;
        }
    }

    public void removeEnemyAtInd(int ind)
    {
        PlayerPrefs.SetInt("kill", PlayerPrefs.GetInt("kill") + 1);
        if(enemyMap[ind] == Enemy.pawn)
        {
            collectCoin(1);
            MM.SM.killSoundPlay(false);
        }
        else
        {
            collectCoin(3);
            MM.SM.killSoundPlay(true);
        }
        groundMap[ind] = Block.ground;
        enemyMap[ind] = Enemy.empty;
        enemyGrid[ind].sprite = EnemySprite[(int)Enemy.empty];

        int remain = 0;
        for (int i = 0; i < groundMap.Length; i++)
        {
            if (groundMap[i] == Block.enemy)
                remain++;
        }
        if(remain == 0)
        {
            MM.PM.nowStat = playerStat.pause;
            float maxBonus = CalcBonus();

            switch (MM.PM.stageMoves - (int)Moves)
            {
                case int n when (n <= -3):
                    starBonus = 5f;
                    if (starBonus > maxBonus)
                        starBonus = maxBonus;
                    MM.UM.ShowClearPop(4, starBonus, maxBonus);
                    collectRuby(((int)Moves - MM.PM.stageMoves) * 2);
                    break;
                case int n when (n <= 0):
                    starBonus = 3f;
                    if (starBonus > maxBonus)
                        starBonus = maxBonus;
                    MM.UM.ShowClearPop(3, starBonus, maxBonus);
                    if ((int)Moves - MM.PM.stageMoves > 0)
                    {
                        collectRuby((int)Moves - MM.PM.stageMoves);
                    }
                    break;
                case int n when (n <= 3):
                    starBonus = 2f;
                    if (starBonus > maxBonus)
                        starBonus = maxBonus;
                    MM.UM.ShowClearPop(2, starBonus, maxBonus);
                    break;
                case int n when (n <= 6):
                    starBonus = 1f;
                    if (starBonus > maxBonus)
                        starBonus = maxBonus;
                    MM.UM.ShowClearPop(1, starBonus, maxBonus);
                    break;
                default:
                    starBonus = 0.5f;
                    if (starBonus > maxBonus)
                        starBonus = maxBonus;
                    MM.UM.ShowClearPop(0, starBonus, maxBonus);
                    break;
            }
            if (CT != ChapterType.test)
            {
                if (!MM.DM.CheckDialoguePhase(MM.Map.stageCount, true))
                {
                    //MM.Map.nextStage();
                }
            }
        }
    }

    public float CalcBonus()
    {
        //보너스 계수 계산
        float lengthBonus = 1f;
        switch (PlayerPrefs.GetInt("length"))
        {
            case 0:
                lengthBonus = 1f;
                break;
            case 1:
                lengthBonus = 2f;
                break;
            case 2:
                lengthBonus = 3f;
                break;
            case 3:
                lengthBonus = 5f;
                break;
            default:
                break;
        }
        float diffBonus = 1f;
        switch (PlayerPrefs.GetInt("diff"))
        {
            case 0:
                diffBonus = 1f;
                break;
            case 1:
                diffBonus = 2f;
                break;
            case 2:
                diffBonus = 3f;
                break;
            case 3:
                diffBonus = 5f;
                break;
            default:
                break;
        }
        if(Mathf.Abs(diffBonus - lengthBonus) > 1f)
        {
            return diffBonus > lengthBonus ? lengthBonus + 1.5f : diffBonus + 1.5f;
        }
        else if (Mathf.Abs(diffBonus - lengthBonus) == 1f)
        {
            if (diffBonus == 4 || lengthBonus == 4)
            {
                return diffBonus > lengthBonus ? lengthBonus + 1.5f : diffBonus + 1.5f;
            }
            else
            {
                return diffBonus > lengthBonus ? lengthBonus + 1f : diffBonus + 1f;
            }
        }
        else
        {
            if (diffBonus == 4 && lengthBonus == 4)
            {
                return diffBonus > lengthBonus ? lengthBonus + 2f : diffBonus + 2f;
            }
            else
            {
                return diffBonus > lengthBonus ? lengthBonus : diffBonus;
            }
        }
    }

    public void nextStage()
    {
        MM.UM.SetStageNo(MM.UM.stageNo + 1);
        upDifficulty();
        MM.PM.nowStat = playerStat.idle;
        GenerateMap();
    }

    public void UpdateVolcanoSP()
    {
        bool playSPsound = false;
        for (int i = 0; i < groundGrid.Length; i++)
        {
            if(groundMap[i] >= Block.special)
            {
                switch (groundMap[i])
                {
                    case Block.special:
                        groundMap[i] = Block.special1;
                        playSPsound = true;
                        break;
                    case Block.special1:
                        groundMap[i] = Block.special2;
                        break;
                    case Block.special2:
                        groundMap[i] = Block.special3;
                        break;
                    case Block.special3:
                        groundMap[i] = Block.special;
                        break;
                }
                updateMap();
                /*
                if ((getIndexX(i) + getIndexY(i)) % 2 == 1)//검은 칸인 경우 +4
                {
                    if (groundGrid[i].sprite == ChSprite[(int)nowChapter].specialB[0])
                    {
                        groundGrid[i].sprite = ChSprite[(int)nowChapter].specialB[1];
                        playSPsound = true;
                    }
                    else if (groundGrid[i].sprite == ChSprite[(int)nowChapter].specialB[1])
                    {
                        groundGrid[i].sprite = ChSprite[(int)nowChapter].specialB[2];
                    }
                    else if (groundGrid[i].sprite == ChSprite[(int)nowChapter].specialB[2])
                    {
                        groundGrid[i].sprite = ChSprite[(int)nowChapter].specialB[3];
                    }
                    else if (groundGrid[i].sprite == ChSprite[(int)nowChapter].specialB[3])
                    {
                        groundGrid[i].sprite = ChSprite[(int)nowChapter].specialB[0];
                    }
                }
                else
                {
                    if (groundGrid[i].sprite == ChSprite[(int)nowChapter].specialW[0])
                    {
                        groundGrid[i].sprite = ChSprite[(int)nowChapter].specialW[1];
                        playSPsound = true;
                    }
                    else if (groundGrid[i].sprite == ChSprite[(int)nowChapter].specialW[1])
                    {
                        groundGrid[i].sprite = ChSprite[(int)nowChapter].specialW[2];
                    }
                    else if (groundGrid[i].sprite == ChSprite[(int)nowChapter].specialW[2])
                    {
                        groundGrid[i].sprite = ChSprite[(int)nowChapter].specialW[3];
                    }
                    else if (groundGrid[i].sprite == ChSprite[(int)nowChapter].specialW[3])
                    {
                        groundGrid[i].sprite = ChSprite[(int)nowChapter].specialW[0];
                    }
                }
                */
            }
        }
        if (playSPsound)
        {
            MM.SM.playSP();
        }
    }

    public void UpdateLibrarySP(int startPos, int finPos, playerPiece used_pp)
    {
        bool playSPsound = false;
        List<int> pathBlockList = requiredPathBlock(startPos, finPos, false, used_pp);
        //Debug.Log(used_pp.ToString() + ",  " + pathBlockList.Count);
        for (int i = 0; i < pathBlockList.Count; i++)
        {
            if(groundMap[pathBlockList[i]] == Block.special1)
            {
                groundMap[pathBlockList[i]] = Block.special;
                if ((getIndexX(pathBlockList[i]) + getIndexY(pathBlockList[i])) % 2 == 1)//검은 칸인 경우
                {
                    groundGrid[pathBlockList[i]].sprite = ChSprite[(int)Chapter.library].specialB[0];
                }
                else
                {
                    groundGrid[pathBlockList[i]].sprite = ChSprite[(int)Chapter.library].specialW[0];
                }
                playSPsound = true;
            }
            else if (groundMap[pathBlockList[i]] == Block.special)
            {
                groundMap[pathBlockList[i]] = Block.obstacle;
                if ((getIndexX(pathBlockList[i]) + getIndexY(pathBlockList[i])) % 2 == 1)//검은 칸인 경우
                {
                    groundGrid[pathBlockList[i]].sprite = ChSprite[(int)Chapter.library].ObstacleB;
                }
                else
                {
                    groundGrid[pathBlockList[i]].sprite = ChSprite[(int)Chapter.library].ObstacleW;
                }
                playSPsound = true;
            }
        }
        if (playSPsound)
        {
            MM.SM.playSP();
        }
    }

    public void LoadSavedMap()
    {
        //MM.UM.gridNum.SetActive(false);
        MM.PM.ResetPlayer();
        //MM.UM.clearInstMsg();
        if (MM.Map.nowChapter == Chapter.tutorial)
        {
            MM.UM.instructionText.text = MM.UM.tutorialInstr[stageCount];
        }
        MM.PM.stageMoves = 0;
        MM.UM.updateMoveCount(0);
        collectedCoin = 0;
        collectedRuby = 0;
        MM.UM.setSliderMoves((int)Moves);
        EmptyMap();

        for(int i = 0; i < row * col; i++)
        {
            groundMap[i] = groundMap_save[i];
            enemyMap[i] = enemyMap_save[i];
        }
        
        if(CT == ChapterType.story)
        {
            MapData MD = MM.TM.GetMap(stageCount);
            MM.PM.playerCycle = new playerPiece[MD.playerCycle.Length];
            for (int i = 0; i < MD.playerCycle.Length; i++)
            {
                MM.PM.playerCycle[i] = MD.playerCycle[i];
            }

            MM.PM.playerPos = MD.playerpos;
            MM.PM.nowCycle = MD.playerInd;
        }
        else
        {
            MM.PM.playerPos = startPosInd[0];
            if(MM.PM.playerCycle.Length != 1)
            {
                MM.PM.nowCycle = (startPosInd[1] + 1) % 3;
            }
            else
            {
                MM.PM.nowCycle = 0;
            }
        }
        MM.PM.UpdatePlayerPos();

        MM.PM.nowStat = playerStat.idle;
        MM.HM.ClearHistory();
        MM.HM.AddHistory();
        updateMap();
    }

    public void LoadHistory(moveHistory mh)
    {
        MM.PM.ResetPlayer();
        //MM.UM.clearInstMsg();
        if (MM.Map.nowChapter == Chapter.tutorial)
        {
            MM.UM.instructionText.text = mh.instruction;
        }
        MM.PM.stageMoves = mh.stageMoves;
        MM.UM.updateMoveCount(mh.stageMoves);
        collectedCoin = mh.collectedCoin;
        collectedRuby = mh.collectedRuby;
        //MM.UM.setSliderMoves((int)Moves - mh.stageMoves);
        EmptyMap();

        for (int i = 0; i < row * col; i++)
        {
            groundMap[i] = mh.groundMap[i];
            enemyMap[i] = mh.enemyMap[i];
        }

        MM.PM.playerPos = mh.playerPos;
        MM.PM.nowCycle = mh.nowCycle;

        MM.PM.UpdatePlayerPos();

        MM.PM.nowStat = playerStat.idle;
        updateMap();
        Debug.Log("history loaded");
    }


    public void collectCoin(int coin)
    {
        collectedCoin += coin;
    }

    public void collectRuby(int ruby)
    {
        collectedRuby += ruby;
        MM.UM.showRuby(ruby);
    }

    public void gainCoin()
    {
        List<playerPiece> arr = new List<playerPiece>();
        for(int i = 0; i < MM.PM.playerCycle.Length; i++)
        {
            if(!arr.Contains(MM.PM.playerCycle[i]))
                arr.Add(MM.PM.playerCycle[i]);
        }
        float PPtype = 0.3f * arr.Count;
        if (PPtype > 0.9f)
            PPtype = 1.0f;

        if(CT == ChapterType.custom)
        {
            MM.UM.AddCoin((int)(collectedCoin * starBonus * PPtype));
        }else if (CT == ChapterType.story)
        {
            MM.UM.AddCoin((int)(collectedCoin));
        }
        MM.UM.AddRuby((int)(collectedRuby * PPtype));
        starBonus = 1;
        collectedCoin = 0;
        collectedRuby = 0;
    }
}
