using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playerStat
{
    idle,
    choosing,
    moving,
    dead,
    pause
}

public class PlayerManager : MonoBehaviour
{
    public MainManager MM;
    //public int cycleLength;
    public playerPiece[] playerCycle;
    public Sprite[] playerPieceSprite;
    public GameObject player;
    public GameObject playersprite;
    public reachableList rl;
    public Move MoveData;

    public GameObject[] reachableMarkerPool;
    public Transform awaypoint;

    public int nowCycle;
    public int playerPos;
    public int prevPos;
    public float moveSpeed;
    //public bool choosingPath;
    public playerStat nowStat;
    public int stageMoves;

    public ParticleSystem killEffect;
    public bool[] PPLocks;

    // Start is called before the first frame update
    void Awake()
    {
        if (PlayerPrefs.HasKey("deck"))
        {
            string[] tmp = PlayerPrefs.GetString("deck").Split(',');
            switch (tmp.Length)
            {
                case 1:
                    playerCycle = new playerPiece[] { (playerPiece)int.Parse(tmp[0]) };
                    break;
                case 2:
                    playerCycle = new playerPiece[] { (playerPiece)int.Parse(tmp[0]), (playerPiece)int.Parse(tmp[1]) };
                    break;
                case 3:
                    playerCycle = new playerPiece[] { (playerPiece)int.Parse(tmp[0]), (playerPiece)int.Parse(tmp[1]), (playerPiece)int.Parse(tmp[2]) };
                    break;
            }
        }
        else
        {
            playerCycle = new playerPiece[] { playerPiece.rook, playerPiece.bishop, playerPiece.knight };
        }
        int tmp3 = PlayerPrefs.GetInt("lockPP");
        for (int i = 0; i < 6; i++)
        {
            if (tmp3 % 10 == 1)
            {
                if(i > 3)
                {
                    PPLocks[i + 6] = false;
                }
                else
                {
                    PPLocks[i + 5] = false;
                }
            }
            tmp3 /= 10;
        }
        RandomizePP();
        //playerCycle = new playerPiece[cycleLength];
        //SetPlayerPiece();
        UpdatePlayerPos();
        ResetPlayer();
        stageMoves = 0;
        rl = new reachableList();
        MoveData = new Move();
    }

    public void RandomizePP()
    {
        if (PlayerPrefs.HasKey("deck"))
        {
            string[] tmp = PlayerPrefs.GetString("deck").Split(',');
            switch (tmp.Length)
            {
                case 1:
                    playerCycle = new playerPiece[] { (playerPiece)int.Parse(tmp[0]) };
                    break;
                case 2:
                    playerCycle = new playerPiece[] { (playerPiece)int.Parse(tmp[0]), (playerPiece)int.Parse(tmp[1]) };
                    break;
                case 3:
                    playerCycle = new playerPiece[] { (playerPiece)int.Parse(tmp[0]), (playerPiece)int.Parse(tmp[1]), (playerPiece)int.Parse(tmp[2]) };
                    break;
            }
        }
        for (int i = 0; i < playerCycle.Length; i++)
        {
            if (playerCycle[i] == playerPiece.random)
            {
                int ran = 0;
                do
                {
                    ran = Random.Range(0, System.Enum.GetValues(typeof(playerPiece)).Length);
                } while (PPLocks[ran]);

                playerCycle[i] = (playerPiece)ran;
            }
        }
        UpdatePlayerPiece();
    }
    /*
    void SetPlayerPiece()
    {
        //setting first piece
        if (PlayerPrefs.HasKey("playerPiece0"))
        {
            playerCycle[0] = (playerPiece)PlayerPrefs.GetInt("playerPiece0");
        }
        else
        {
            playerCycle[0] = playerPiece.rook;
        }


        //setting second piece
        if (cycleLength > 1 && PlayerPrefs.HasKey("playerPiece1"))
        {
            playerCycle[1] = (playerPiece)PlayerPrefs.GetInt("playerPiece1");
        }
        else if (cycleLength > 1)
        {
            playerCycle[1] = playerPiece.bishop;
        }

        //setting third piece
        if (cycleLength > 2 && PlayerPrefs.HasKey("playerPiece2"))
        {
            playerCycle[2] = (playerPiece)PlayerPrefs.GetInt("playerPiece2");
        }
        else if (cycleLength > 2)
        {
            playerCycle[2] = playerPiece.knight;
        }
    }*/

    public void UpdatePlayerPiece()
    {
        playersprite.GetComponent<SpriteRenderer>().sprite = playerPieceSprite[(int)playerCycle[nowCycle]];
        MM.PPPM.UpdatePanelStat(nowCycle);
    }

    public void UpdatePlayerPos()
    {
        UpdatePlayerPiece();
        player.transform.position = MM.Map.groundGrid[playerPos].transform.position;
        prevPos = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if(nowStat == playerStat.moving)
        {
            SlidePlayer();
        }
    }

    public void ClickBoard()
    {
        if (nowStat == playerStat.idle)
        {
            nowStat = playerStat.choosing;
            checkReachable();
            nowCycle = (nowCycle + 1) % playerCycle.Length;
        }
        else if (nowStat == playerStat.choosing)
        {
            clickMarker();
        }
    }

    void checkReachable()
    {
        List<int> reachable;
        rl = MM.PPM.PieceList(playerCycle[nowCycle], playerPos, MM.Map.groundMap, MM.Map.enemyMap, true);

        reachable = rl.reachableBlockList;
        foreach (var i in rl.whDataList)
        {
            foreach (var j in i.reachableBlocks)
            {
                if (!reachable.Contains(j))
                    reachable.Add(j);
            }
        }

        for (int i = 0; i < reachable.Count; i++)
        {
            reachableMarkerPool[i].transform.position = MM.Map.groundGrid[reachable[i]].transform.position;
            reachableMarkerPool[i].GetComponent<MarkerData>().index = reachable[i];
        }
        removeMarker(reachable.Count);
        if(reachable.Count == 0)
        {
            MM.UM.ShowStuckMsg();
        }
    }

    void removeMarker(int startingInd)
    {
        for (int i = startingInd; i < reachableMarkerPool.Length; i++)
        {
            reachableMarkerPool[i].transform.position = awaypoint.position;
            reachableMarkerPool[i].GetComponent<MarkerData>().wormhole_index = -1;
        }
    }

    void removeMarker()
    {
        removeMarker(0);
    }

    void clickMarker()
    {
        RaycastHit hit;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D ray = Physics2D.Raycast(mousePos, Vector2.zero);
        if (ray.collider && ray.collider.gameObject.tag == "target")
        {
            if (ray.collider.gameObject.tag == "target")
            {
                prevPos = playerPos;
                playerPos = ray.collider.gameObject.GetComponent<MarkerData>().index;
                //SetMoveData(playerCycle[nowCycle], prevPos, playerPos);
                removeMarker();
                nowStat = playerStat.moving;
                stageMoves++;
                MM.UM.updateMoveCount(stageMoves);
                MM.SM.startRockSteps();
            }
        }
    }


    void SlidePlayer()
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, MM.Map.groundGrid[playerPos].transform.position, Time.deltaTime * moveSpeed);
        if (player.transform.position == MM.Map.groundGrid[playerPos].transform.position)
        {
            MM.SM.stopSteps();
            switch (MM.Map.nowChapter)
            {
                case Chapter.volcano:
                    MM.Map.UpdateVolcanoSP();
                    if (MM.Map.groundGrid[playerPos].sprite == MM.Map.ChSprite[(int)Chapter.volcano].specialB[1] || MM.Map.groundGrid[playerPos].sprite == MM.Map.ChSprite[(int)Chapter.volcano].specialW[1])
                    {
                        KillPlayer();
                        return;
                    }
                    break;
                case Chapter.library:
                    MM.Map.UpdateLibrarySP(prevPos, playerPos, playerCycle[(nowCycle - 1) < 0 ? playerCycle.Length - 1 : nowCycle - 1]);
                    break;
                default:
                    break;
            }
            nowStat = playerStat.idle;
            if (MM.Map.groundMap[playerPos] == Block.enemy)
            {
                MM.Map.removeEnemyAtInd(playerPos);
                killEffect.Play();
            }
            MM.HM.AddHistory();
            UpdatePlayerPiece();
        }
    }

    void KillPlayer()
    {
        nowStat = playerStat.dead;
        playersprite.SetActive(false);
        MM.UM.GameOverMsg();
    }

    public void ResetPlayer()
    {
        removeMarker();
        nowStat = playerStat.idle;
        playersprite.SetActive(true);
        stageMoves = 0;
    }
}