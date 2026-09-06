using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playerPiece
{
    rook,
    bishop,
    knight,
    king,
    empty,
    queen,
    crow,
    dragon,
    unicorn,
    random,
    deer,
    jumpking
}
public enum moveType
{
    jump = 0,
    up = -8,
    right = 1,
    down = 8,
    left = -1,
    upright = -7,
    downright = 9,
    downleft = 7,
    upleft = -9
}

[System.Serializable]
public struct chessVec
{
    public int x;
    public int y;

    public chessVec(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class playerPieceManager : MonoBehaviour
{

    [System.Serializable]
    public struct blockScanData
    {
        public bool isAdd;
        public bool keepScan;
        public wormHoleData whd;

        public blockScanData(bool isAdd, bool keepScan)
        {
            this.isAdd = isAdd;
            this.keepScan = keepScan;
            whd = new wormHoleData(-1, -1);
        }
    }


    public MainManager MM;
    MapManager map;
    int row;
    int col;
    public void SetPPM()
    {
        map = MM.Map;
        row = map.row;
        col = map.col;
    }
    bool isInMapLength(int index)
    {
        if (index < 0 || index >= map.groundMap.Length)
        {
            return false;
        }
        return true;
    }

    public int getIndexX(int index)
    {
        return index % row;
    }
    public int getIndexY(int index)
    {
        return Mathf.FloorToInt(index / row);
    }

    public bool isKillable(playerPiece pp, Enemy enem)
    {
        bool ans = false;
        if(enem == Enemy.pawn)
        {
            return true;
        }
        switch (pp)
        {
            case playerPiece.rook:
                switch (enem)
                {
                    case Enemy.rook:
                        return false;
                        break;
                    case Enemy.bishop:
                        return true;
                        break;
                    case Enemy.knight:
                        return true;
                        break;
                    default:
                        break;
                }
                break;
            case playerPiece.bishop:
                switch (enem)
                {
                    case Enemy.rook:
                        return true;
                        break;
                    case Enemy.bishop:
                        return false;
                        break;
                    case Enemy.knight:
                        return true;
                        break;
                    default:
                        break;
                }
                break;
            case playerPiece.knight:
            case playerPiece.jumpking:
                switch (enem)
                {
                    case Enemy.rook:
                        return true;
                        break;
                    case Enemy.bishop:
                        return true;
                        break;
                    case Enemy.knight:
                        return false;
                        break;
                    default:
                        break;
                }
                break;
            case playerPiece.king:
                return true;
            case playerPiece.queen:
            case playerPiece.deer:
                switch (enem)
                {
                    case Enemy.rook:
                        return false;
                        break;
                    case Enemy.bishop:
                        return false;
                        break;
                    case Enemy.knight:
                        return true;
                        break;
                    default:
                        break;
                }
                return true;
            case playerPiece.crow:
                switch (enem)
                {
                    case Enemy.rook:
                        return true;
                        break;
                    case Enemy.bishop:
                        return false;
                        break;
                    case Enemy.knight:
                        return false;
                        break;
                    default:
                        break;
                }
                return true;
            case playerPiece.dragon:
                switch (enem)
                {
                    case Enemy.rook:
                        return false;
                        break;
                    case Enemy.bishop:
                        return true;
                        break;
                    case Enemy.knight:
                        return false;
                        break;
                    default:
                        break;
                }
                return true;
            case playerPiece.unicorn:
                switch (enem)
                {
                    case Enemy.rook:
                        return true;
                        break;
                    case Enemy.bishop:
                        return true;
                        break;
                    case Enemy.knight:
                        return true;
                        break;
                    default:
                        break;
                }
                return true;
            default:
                break;
        }
        return ans;
    }

    public reachableList PieceList(playerPiece pp, int playerind, Block[] t_map, Enemy[] e_map, bool includeEnemy)
    {
        reachableList ans = new reachableList();
        ans.reachableBlockList = new List<int>();
        ans.whDataList = new List<wormHoleData>();
        reachableList tmp = KingList(playerind, t_map, e_map, includeEnemy);
        switch (pp)
        {
            case playerPiece.rook:
                tmp = RookList(playerind, t_map, e_map, includeEnemy);
                break;
            case playerPiece.bishop:
                tmp = BishopList(playerind, t_map, e_map, includeEnemy);
                break;
            case playerPiece.knight:
                tmp = KnightList(playerind, t_map, e_map, includeEnemy);
                break;
            case playerPiece.king:
                tmp = KingList(playerind, t_map, e_map, includeEnemy);
                break;
            case playerPiece.queen:
                tmp = QueenList(playerind, t_map, e_map, includeEnemy);
                break;
            case playerPiece.crow:
                tmp = CrowList(playerind, t_map, e_map, includeEnemy);
                break;
            case playerPiece.dragon:
                tmp = DragonList(playerind, t_map, e_map, includeEnemy);
                break;
            case playerPiece.unicorn:
                tmp = UnicornList(playerind, t_map, e_map, includeEnemy);
                break;
            case playerPiece.deer:
                tmp = DeerList(playerind, t_map, e_map, includeEnemy);
                break;
            case playerPiece.jumpking:
                tmp = JumpKingList(playerind, t_map, e_map, includeEnemy);
                break;
            default:
                Debug.Log("player piece가 정해지지 않았습니다.");
                break;
        }

        foreach (var i in tmp.reachableBlockList)
        {
            ans.reachableBlockList.Add(i);
        }
        foreach (var i in tmp.whDataList)
        {
            ans.whDataList.Add(i);
        }
        return ans;
    }

    public blockScanData checkBlock(playerPiece pp, int scanpoint, moveType mt, Block[] t_map, Enemy[] e_map, bool includeEnemy)
    {
        blockScanData ans = new blockScanData(false, false);
        switch (t_map[scanpoint])
        {
            case Block.ground:
                ans.isAdd = true;
                ans.keepScan = true;
                break;
            case Block.enemy:
                if (includeEnemy)
                    if (isKillable(pp,e_map[scanpoint]))
                        ans.isAdd = true;
                break;
            case Block.obstacle:
                break;
            case Block.special:
            default:
                if (map.nowChapter == Chapter.volcano)
                {
                    if (map.groundGrid[scanpoint].sprite == map.ChSprite[(int)Chapter.volcano].specialB[1] || map.groundGrid[scanpoint].sprite == map.ChSprite[(int)Chapter.volcano].specialW[1])
                        ans.keepScan = false;
                    else
                    {
                        ans.isAdd = true;
                        ans.keepScan = true;
                    }
                }
                else if (map.nowChapter == Chapter.library)
                {
                    ans.isAdd = true;
                    ans.keepScan = true;
                }
                //library는 special 걍 다 add 해도 됨
                else if (map.nowChapter == Chapter.space)
                {
                    ans.keepScan = false;
                    for (int j = 0; j < map.groundMap.Length; j++)
                    {
                        if (j != scanpoint && map.groundMap[j] == map.groundMap[scanpoint])
                        {
                            ans.whd = new wormHoleData(scanpoint, j);
                            foreach (var k in wormHoleList((int)mt, j, t_map, true, pp))
                            {
                                ans.whd.reachableBlocks.Add(k);
                            }
                            break;
                        }
                    }
                }
                break;
        }
        return ans;
    }

    public reachableList RookList(int playerind, Block[] t_map, Enemy[] e_map, bool includeEnemy)
    {
        reachableList rl = new reachableList();
        rl.reachableBlockList = new List<int>();
        rl.whDataList = new List<wormHoleData>();
        List<int> ans = new List<int>();

        int playerY = getIndexY(playerind);

        int scanRange = row > col ? row : col;
        scanRange *= 2;

        int scanpoint;
        bool[] isScanning = { true, true, true, true }; // up right down left, clockwise order
        for (int i = 1; i < scanRange; i++)
        {
            //up
            scanpoint = playerind - row * i;
            if (isScanning[0] && isInMapLength(scanpoint))
            {
                blockScanData tmp = checkBlock(playerPiece.rook, scanpoint, moveType.up, t_map, e_map, includeEnemy);
                if (tmp.isAdd)
                    ans.Add(scanpoint);
                if (!tmp.keepScan)
                    isScanning[0] = false;
                if (tmp.whd.whIn != -1)
                {
                    rl.whDataList.Add(tmp.whd);
                }
            }

            //right
            scanpoint = playerind + i;
            if (isScanning[1] && isInMapLength(scanpoint) && playerY == getIndexY(scanpoint))
            {
                blockScanData tmp = checkBlock(playerPiece.rook, scanpoint, moveType.right, t_map, e_map, includeEnemy);
                if (tmp.isAdd)
                    ans.Add(scanpoint);
                if (!tmp.keepScan)
                    isScanning[1] = false;
                if (tmp.whd.whIn != -1)
                {
                    rl.whDataList.Add(tmp.whd);
                }
            }

            //down
            scanpoint = playerind + row * i;
            if (isScanning[2] && isInMapLength(scanpoint))
            {
                blockScanData tmp = checkBlock(playerPiece.rook, scanpoint, moveType.down, t_map, e_map, includeEnemy);
                if (tmp.isAdd)
                    ans.Add(scanpoint);
                if (!tmp.keepScan)
                    isScanning[2] = false;
                if (tmp.whd.whIn != -1)
                {
                    rl.whDataList.Add(tmp.whd);
                }
            }

            //left
            scanpoint = playerind - i;
            if (isScanning[3] && isInMapLength(scanpoint) && playerY == getIndexY(scanpoint))
            {
                blockScanData tmp = checkBlock(playerPiece.rook, scanpoint, moveType.left, t_map, e_map, includeEnemy);
                if (tmp.isAdd)
                    ans.Add(scanpoint);
                if (!tmp.keepScan)
                    isScanning[3] = false;
                if (tmp.whd.whIn != -1)
                {
                    rl.whDataList.Add(tmp.whd);
                }
            }
        }
        foreach (var i in ans)
        {
            rl.reachableBlockList.Add(i);
        }
        return rl;
    }

    public reachableList BishopList(int playerind, Block[] t_map, Enemy[] e_map, bool includeEnemy)
    {
        reachableList rl = new reachableList();
        rl.reachableBlockList = new List<int>();
        rl.whDataList = new List<wormHoleData>();
        List<int> ans = new List<int>();

        int playerY = getIndexY(playerind);

        int scanRange = row < col ? row : col;
        scanRange *= 2;

        int scanpoint;
        bool[] isScanning = { true, true, true, true }; // ur dr dl ul, clockwise order
        for (int i = 1; i < scanRange; i++)
        {
            //ur
            scanpoint = playerind - row * i + i;
            if (isScanning[0] && isInMapLength(scanpoint) && playerY == getIndexY(scanpoint) + i)
            {
                blockScanData tmp = checkBlock(playerPiece.bishop, scanpoint, moveType.upright, t_map, e_map, includeEnemy);
                if (tmp.isAdd)
                    ans.Add(scanpoint);
                if (!tmp.keepScan)
                    isScanning[0] = false;
                if (tmp.whd.whIn != -1)
                {
                    rl.whDataList.Add(tmp.whd);
                }
            }

            //dr
            scanpoint = playerind + row * i + i;
            if (isScanning[1] && isInMapLength(scanpoint) && playerY == getIndexY(scanpoint) - i)
            {
                blockScanData tmp = checkBlock(playerPiece.bishop, scanpoint, moveType.downright, t_map, e_map, includeEnemy);
                if (tmp.isAdd)
                    ans.Add(scanpoint);
                if (!tmp.keepScan)
                    isScanning[1] = false;
                if (tmp.whd.whIn != -1)
                {
                    rl.whDataList.Add(tmp.whd);
                }
            }

            //dl
            scanpoint = playerind + row * i - i;
            if (isScanning[2] && isInMapLength(scanpoint) && playerY == getIndexY(scanpoint) - i)
            {
                blockScanData tmp = checkBlock(playerPiece.bishop, scanpoint, moveType.downleft, t_map, e_map, includeEnemy);
                if (tmp.isAdd)
                    ans.Add(scanpoint);
                if (!tmp.keepScan)
                    isScanning[2] = false;
                if (tmp.whd.whIn != -1)
                {
                    rl.whDataList.Add(tmp.whd);
                }
            }

            //ul
            scanpoint = playerind - row * i - i;
            if (isScanning[3] && isInMapLength(scanpoint) && playerY == getIndexY(scanpoint) + i)
            {
                blockScanData tmp = checkBlock(playerPiece.bishop, scanpoint, moveType.upleft, t_map, e_map, includeEnemy);
                if (tmp.isAdd)
                    ans.Add(scanpoint);
                if (!tmp.keepScan)
                    isScanning[3] = false;
                if (tmp.whd.whIn != -1)
                {
                    rl.whDataList.Add(tmp.whd);
                }
            }
        }
        foreach (var i in ans)
        {
            rl.reachableBlockList.Add(i);
        }
        return rl;
    }

    private reachableList pointList(int playerind, Block[] t_map, Enemy[] e_map, bool includeEnemy, List<chessVec> cvList, playerPiece pp)
    {
        reachableList rl = new reachableList();
        rl.reachableBlockList = new List<int>();
        rl.whDataList = new List<wormHoleData>();
        List<int> ans = new List<int>();

        int playerX = getIndexX(playerind);
        int playerY = getIndexY(playerind);


        foreach (chessVec i in cvList)
        {
            int scanpoint = playerind + i.y * row + i.x;
            if (isInMapLength(scanpoint) && playerY + i.y == getIndexY(scanpoint))
            {
                blockScanData tmp = checkBlock(pp, scanpoint, moveType.jump, t_map, e_map, includeEnemy);
                if (tmp.isAdd)
                    ans.Add(scanpoint);
                if (tmp.whd.whIn != -1)
                    rl.whDataList.Add(tmp.whd);
            }
        }

        foreach (var i in ans)
        {
            rl.reachableBlockList.Add(i);
        }
        return rl;
    }

    public reachableList KnightList(int playerind, Block[] t_map, Enemy[] e_map, bool includeEnemy)
    {
        List<chessVec> scanVect = new List<chessVec> {new chessVec(1, -2),
                                                      new chessVec(1, 2),
                                                      new chessVec(-1, -2),
                                                      new chessVec(-1, 2),
                                                      new chessVec(2, -1),
                                                      new chessVec(2, 1),
                                                      new chessVec(-2, -1),
                                                      new chessVec(-2, 1)};
        return pointList(playerind, t_map, e_map, includeEnemy, scanVect, playerPiece.knight);
    }

    public reachableList KingList(int playerind, Block[] t_map, Enemy[] e_map, bool includeEnemy)
    {
        List<chessVec> scanVect = new List<chessVec> {new chessVec(-1, -1),
                                                      new chessVec(0, -1),
                                                      new chessVec(1, -1),
                                                      new chessVec(-1, 0),
                                                      new chessVec(1, 0),
                                                      new chessVec(-1, 1),
                                                      new chessVec(0, 1),
                                                      new chessVec(1, 1)};
        return pointList(playerind, t_map, e_map, includeEnemy, scanVect, playerPiece.king);
    }

    public reachableList DeerList(int playerind, Block[] t_map, Enemy[] e_map, bool includeEnemy)
    {
        List<chessVec> scanVect = new List<chessVec> {new chessVec(-1, -1),
                                                      new chessVec(0, -1),
                                                      new chessVec(1, -1),
                                                      new chessVec(-1, 0),
                                                      new chessVec(1, 0),
                                                      new chessVec(-1, 1),
                                                      new chessVec(0, 1),
                                                      new chessVec(1, 1),
                                                      new chessVec(-2, -2),
                                                      new chessVec(0, -2),
                                                      new chessVec(2, -2),
                                                      new chessVec(-2, 0),
                                                      new chessVec(2, 0),
                                                      new chessVec(-2, 2),
                                                      new chessVec(0, 2),
                                                      new chessVec(2, 2)};
        return pointList(playerind, t_map, e_map, includeEnemy, scanVect, playerPiece.deer);
    }

    public reachableList JumpKingList(int playerind, Block[] t_map, Enemy[] e_map, bool includeEnemy)
    {
        List<chessVec> scanVect = new List<chessVec> {new chessVec(-2, -2),
                                                      new chessVec(-2, -1),
                                                      new chessVec(-2, 0),
                                                      new chessVec(-2, 1),
                                                      new chessVec(-2, 2),
                                                      new chessVec(-1, -2),
                                                      new chessVec(-1, 2),
                                                      new chessVec(0, -2),
                                                      new chessVec(0, 2),
                                                      new chessVec(1, -2),
                                                      new chessVec(1, 2),
                                                      new chessVec(2, -2),
                                                      new chessVec(2, -1),
                                                      new chessVec(2, 0),
                                                      new chessVec(2, 1),
                                                      new chessVec(2, 2)};
        return pointList(playerind, t_map, e_map, includeEnemy, scanVect, playerPiece.jumpking);
    }

    public reachableList QueenList(int playerind, Block[] t_map, Enemy[] e_map, bool includeEnemy)
    {
        reachableList rl = new reachableList();
        rl.reachableBlockList = new List<int>();
        rl.whDataList = new List<wormHoleData>();
        List<int> ans = new List<int>();

        reachableList RookL = RookList(playerind, t_map, e_map, includeEnemy);
        reachableList BishopL = BishopList(playerind, t_map, e_map, includeEnemy);

        foreach (var i in RookL.reachableBlockList){
            ans.Add(i);
        }
        foreach (var i in BishopL.reachableBlockList)
        {
            if(!ans.Contains(i))
                ans.Add(i);
        }
        foreach(var i in ans)
        {
            rl.reachableBlockList.Add(i);
        }


        foreach (var i in RookL.whDataList)
        {
            rl.whDataList.Add(i);
        }
        foreach (var i in BishopL.whDataList)
        {
            if (!rl.whDataList.Contains(i))
                rl.whDataList.Add(i);
        }

        return rl;
    }

    public reachableList CrowList(int playerind, Block[] t_map, Enemy[] e_map, bool includeEnemy)
    {
        reachableList rl = new reachableList();
        rl.reachableBlockList = new List<int>();
        rl.whDataList = new List<wormHoleData>();
        List<int> ans = new List<int>();

        reachableList KnightL = KnightList(playerind, t_map, e_map, includeEnemy);
        reachableList BishopL = BishopList(playerind, t_map, e_map, includeEnemy);

        foreach (var i in KnightL.reachableBlockList)
        {
            ans.Add(i);
        }
        foreach (var i in BishopL.reachableBlockList)
        {
            if (!ans.Contains(i))
                ans.Add(i);
        }
        foreach (var i in ans)
        {
            rl.reachableBlockList.Add(i);
        }


        foreach (var i in KnightL.whDataList)
        {
            rl.whDataList.Add(i);
        }
        foreach (var i in BishopL.whDataList)
        {
            if (!rl.whDataList.Contains(i))
                rl.whDataList.Add(i);
        }

        return rl;
    }

    public reachableList DragonList(int playerind, Block[] t_map, Enemy[] e_map, bool includeEnemy)
    {
        reachableList rl = new reachableList();
        rl.reachableBlockList = new List<int>();
        rl.whDataList = new List<wormHoleData>();
        List<int> ans = new List<int>();

        reachableList RookL = RookList(playerind, t_map, e_map, includeEnemy);
        reachableList KnightL = KnightList(playerind, t_map, e_map, includeEnemy);

        foreach (var i in RookL.reachableBlockList)
        {
            ans.Add(i);
        }
        foreach (var i in KnightL.reachableBlockList)
        {
            if (!ans.Contains(i))
                ans.Add(i);
        }
        foreach (var i in ans)
        {
            rl.reachableBlockList.Add(i);
        }


        foreach (var i in RookL.whDataList)
        {
            rl.whDataList.Add(i);
        }
        foreach (var i in KnightL.whDataList)
        {
            if (!rl.whDataList.Contains(i))
                rl.whDataList.Add(i);
        }

        return rl;
    }
    public reachableList UnicornList(int playerind, Block[] t_map, Enemy[] e_map, bool includeEnemy)
    {
        reachableList rl = new reachableList();
        rl.reachableBlockList = new List<int>();
        rl.whDataList = new List<wormHoleData>();
        List<int> ans = new List<int>();

        reachableList QueenL = QueenList(playerind, t_map, e_map, includeEnemy);
        reachableList KnightL = KnightList(playerind, t_map, e_map, includeEnemy);

        foreach (var i in QueenL.reachableBlockList)
        {
            ans.Add(i);
        }
        foreach (var i in KnightL.reachableBlockList)
        {
            if (!ans.Contains(i))
                ans.Add(i);
        }
        foreach (var i in ans)
        {
            rl.reachableBlockList.Add(i);
        }


        foreach (var i in QueenL.whDataList)
        {
            rl.whDataList.Add(i);
        }
        foreach (var i in KnightL.whDataList)
        {
            if (!rl.whDataList.Contains(i))
                rl.whDataList.Add(i);
        }

        return rl;
    }

    List<int> wormHoleList(int wayPlus, int wormHolePos, Block[] t_map, bool includeEnem, playerPiece pp)//obstacle과 special은 무조건 미포함
    {
        List<int> ans = new List<int>();
        int scanLength = (row > col ? row : col) * 2;
        ans.Add(wormHolePos);
        for (int i = 1; i < scanLength; i++)
        {
            int scanPoint = wormHolePos + wayPlus * i;
            if (wayPlus == 1)//right
            {
                if (isInMapLength(scanPoint) && getIndexY(wormHolePos) == getIndexY(scanPoint))
                {
                    if (t_map[scanPoint] == Block.ground)
                    {
                        ans.Add(scanPoint);
                    }
                    else if (t_map[scanPoint] == Block.enemy && includeEnem && isKillable(pp, map.enemyMap[scanPoint]))
                    {
                        ans.Add(scanPoint);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (wayPlus == -1)//left
            {
                if (isInMapLength(scanPoint) && getIndexY(wormHolePos) == getIndexY(scanPoint))
                {
                    if (t_map[scanPoint] == Block.ground)
                    {
                        ans.Add(scanPoint);
                    }
                    else if (t_map[scanPoint] == Block.enemy && includeEnem && isKillable(pp, map.enemyMap[scanPoint]))
                    {
                        ans.Add(scanPoint);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (wayPlus == row)//down
            {
                if (isInMapLength(scanPoint))
                {
                    if (t_map[scanPoint] == Block.ground)
                    {
                        ans.Add(scanPoint);
                    }
                    else if (t_map[scanPoint] == Block.enemy && includeEnem && isKillable(pp, map.enemyMap[scanPoint]))
                    {
                        ans.Add(scanPoint);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (wayPlus == -row)//up
            {
                if (isInMapLength(scanPoint))
                {
                    if (t_map[scanPoint] == Block.ground)
                    {
                        ans.Add(scanPoint);
                    }
                    else if (t_map[scanPoint] == Block.enemy && includeEnem && isKillable(pp, map.enemyMap[scanPoint]))
                    {
                        ans.Add(scanPoint);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (wayPlus == -row + 1)//ur
            {
                if (isInMapLength(scanPoint) && getIndexY(wormHolePos) == getIndexY(scanPoint) + i)
                {
                    if (t_map[scanPoint] == Block.ground)
                    {
                        ans.Add(scanPoint);
                    }
                    else if (t_map[scanPoint] == Block.enemy && includeEnem && isKillable(pp, map.enemyMap[scanPoint]))
                    {
                        ans.Add(scanPoint);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (wayPlus == row + 1)//dr
            {
                if (isInMapLength(scanPoint) && getIndexY(wormHolePos) == getIndexY(scanPoint) - i)
                {
                    if (t_map[scanPoint] == Block.ground)
                    {
                        ans.Add(scanPoint);
                    }
                    else if (t_map[scanPoint] == Block.enemy && includeEnem && isKillable(pp, map.enemyMap[scanPoint]))
                    {
                        ans.Add(scanPoint);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (wayPlus == row - 1)//dl
            {
                if (isInMapLength(scanPoint) && getIndexY(wormHolePos) == getIndexY(scanPoint) - i)
                {
                    if (t_map[scanPoint] == Block.ground)
                    {
                        ans.Add(scanPoint);
                    }
                    else if (t_map[scanPoint] == Block.enemy && includeEnem && isKillable(pp, map.enemyMap[scanPoint]))
                    {
                        ans.Add(scanPoint);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (wayPlus == -row - 1)//ul
            {
                if (isInMapLength(scanPoint) && getIndexY(wormHolePos) == getIndexY(scanPoint) + i)
                {
                    if (t_map[scanPoint] == Block.ground)
                    {
                        ans.Add(scanPoint);
                    }
                    else if (t_map[scanPoint] == Block.enemy && includeEnem && isKillable(pp, map.enemyMap[scanPoint]))
                    {
                        ans.Add(scanPoint);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (wayPlus == 0)//for jumpers
            {
                //already added the outpoint of wormhole
            }
        }
        /*string whlLog = "";
        foreach(var i in ans)
        {
            whlLog += i + " ";
        }
        Debug.Log(whlLog + " " + includeEnem);*/
        return ans;
    }

}
