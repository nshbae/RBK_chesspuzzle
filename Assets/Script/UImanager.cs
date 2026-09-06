using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class UImanager : MonoBehaviour
{
    public MainManager MM;
    public TMP_Text coinText;
    public TMP_Text rubyText;
    //public int score;

    public TMP_Text stageText;
    public int stageNo;

    public TMP_Text instructionText;
    public string[] gameoverMsg;
    public string[] clearMsg;

    public Animator clearPanel;
    public TMP_Text starText;
    public TMP_Text clearText;
    public GameObject[] starGroups;

    //public TMP_Text moveCountText;

    public Animator helpPanel;

    public GameObject gridNum;

    public TMP_Text totalMoveText;
    public Image MoveCountSliderFill;
    public Image MoveCountSliderBGfill;
    public Slider MoveCounterSlider;
    public GameObject[] sliderStars;
    public Color[] starColor;

    public Animator rubyPanelAnim;
    public TMP_Text rubyGetText;

    playerStat statBeforePop;

    public Animator ppSlotController;
    public Sprite[] playerSlotSprite;
    public Image[] playerSlots;

    public GameObject NextStageBtn;
    public Button NewMapBtn;
    public Button AnsBtn;

    public GameObject newMapBtnPar;
    public GameObject AnsBtnPar;

    public string[] tutorialInstr;

    private void Start()
    {
        coinText.text = PlayerPrefs.GetInt("coin").ToString();
        rubyText.text = PlayerPrefs.GetInt("ruby").ToString();
        MM.SM.BGMsor.volume = PlayerPrefs.GetFloat("musicVol");
        MM.SM.BSFsor.volume = PlayerPrefs.GetFloat("sfxVol");
        Application.targetFrameRate = PlayerPrefs.GetInt("fps");
        CloseClearPop();
        gridNum.SetActive(false);
        if(MM.Map.CT == ChapterType.story)
        {
            newMapBtnPar.SetActive(false);
            AnsBtnPar.SetActive(false);
        }
    }
    public void AddCoin(int i)
    {
        coinText.text = (PlayerPrefs.GetInt("coin") + i).ToString();
        MM.SVM.SaveCoin();
    }

    public void AddRuby(int i)
    {
        rubyText.text = (int.Parse(rubyText.text) + i).ToString();
        MM.SVM.SaveRuby();
    }

    public void showRuby(int i)
    {
        if (i > 0)
        {
            rubyGetText.text = i.ToString();
            Invoke("showRuby_delay", 0.1f);
            MM.SM.readyGetRuby();
        }
    }

    public void showRuby_delay()
    {
        rubyPanelAnim.SetBool("show", true);
    }

    public void SetStageNo(int i)
    {
        stageNo = i;
        stageText.text = "스테이지 " + (i + 1).ToString();
        PlayerPrefs.SetInt("nowStage", i);
        if(MM.Map.nowChapter == Chapter.tutorial)
        {
            instructionText.text = tutorialInstr[i];
        }
    }

    public void ShowStuckMsg()
    {
        GameOverMsg();
    }

    public void ShowMsg(string msg)
    {
        instructionText.text = msg;
    }

    public void clearInstMsg()
    {
        instructionText.text = "";
    }

    public void GameOverMsg()
    {
        instructionText.text = "더이상 움직일 수 없습니다.\n" + "\"" + SplitEnterGetInd(gameoverMsg[(int)MM.Map.nowChapter]) + "\"";
    }

    string SplitEnterGetInd(string netString, int index)
    {
        string[] tmp = netString.Split('|');
        return tmp[index];
    }


    string SplitEnterGetInd(string netString)
    {
        string[] tmp = netString.Split('|');
        return SplitEnterGetInd(netString, Random.Range(0, tmp.Length));
    }

    public void ShowClearPop(int stars, float nowbonus, float maxbonus)
    {
        MM.SM.playStageClear(stars);
        //clearText.color = starColor[stars];
        starGroups[stars].SetActive(true);
        if(MM.Map.CT == ChapterType.custom)
        {
            bonusInstr(stars, nowbonus, maxbonus);
        }
        else
        {
            if(PlayerPrefs.GetInt("kill")<50 || Random.Range(0f, 1f) > 0.05f)
            {
                switch (stars)
                {
                    case 0:
                        //clearText.text = "획득 코인 : " + MM.Map.collectedCoin + " x " + maxbonus + " = " + maxbonus * MM.Map.collectedCoin;
                        clearText.text = "시간은 두 장소 사이의 가장 먼 거리이다.";
                        break;
                    case 1:
                        clearText.text = "때로는 기회를 놓치는 것이 기회일 수 있다.";
                        //clearText.text = SplitEnterGetInd(clearMsg[(int)MM.Map.nowChapter], 0);
                        break;
                    case 2:
                        clearText.text = "어려운 일은 시간이 해결해준다.";
                        //clearText.text = SplitEnterGetInd(clearMsg[(int)MM.Map.nowChapter], 1);
                        break;
                    case 3:
                        clearText.text = "시간은 인간이 쓸 수 있는 것중에\n가장 값진 것이다.";
                        //clearText.text = SplitEnterGetInd(clearMsg[(int)MM.Map.nowChapter], 2);
                        break;
                    case 4:
                        clearText.text = "시간을 지배할 수 있는 사람은\n인생을 지배할 수 있는 사람이다.";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                clearText.text = "즐겁게 플레이하시고 계신다면\n플레이스토어에 후기를 남겨주세요!";
            }
        }

        if (MM.Map.CT == ChapterType.story)
        {
            MM.SVM.SaveProgress((int)MM.Map.nowChapter, MM.Map.stageCount, stars);
        }
        if(MM.Map.CT == ChapterType.story && MM.TM.MapList.Count == MM.Map.stageCount + 1)
        {
            NextStageBtn.SetActive(false);
        }
        else if (MM.Map.CT == ChapterType.custom)
        {
            int totStage = 0;
            /*
            for(int i = 0; i < MM.Map.diffCurve.Length; i++)
            {
                totStage += MM.Map.diffCurve[i].PhaseLength;
            }
            if(totStage == MM.Map.stageCount + 1)
            {
                NextStageBtn.SetActive(false);
            }*/
        }

        clearPanel.SetBool("isShow", true);
    }

    public void bonusInstr(int stars, float nowbonus, float maxbonus)
    {
        string diff = "";
        switch (PlayerPrefs.GetInt("diff"))
        {
            case 0:
                diff = "쉬움";
                break;
            case 1:
                diff = "보통";
                break;
            case 2:
                diff = "어려움";
                break;
            case 3:
                diff = "부조리";
                break;
            default:
                break;
        }
        string leng = "";
        switch (PlayerPrefs.GetInt("length"))
        {
            case 0:
                leng = "짧음";
                break;
            case 1:
                leng = "보통";
                break;
            case 2:
                leng = "김";
                break;
            case 3:
                leng = "평생";
                break;
            default:
                break;
        }
        clearText.text = "획득 코인 : " + MM.Map.collectedCoin + " x " + nowbonus + " = " + nowbonus * MM.Map.collectedCoin + "\n" + leng + ", " + diff + "의 최대 배수는 " + maxbonus + "입니다.";
    }

    public void CloseClearPop()
    {
        clearPanel.SetBool("isShow", false);
        for (int i = 0; i < starGroups.Length; i++)
        {
            starGroups[i].SetActive(false);
        }
        rubyPanelAnim.SetBool("show", false);
    }

    public void LoadNextStage()
    {
        MM.Map.nextStage();
    }

    public void ShowHelpPop()
    {
        statBeforePop = MM.PM.nowStat;
        MM.PM.nowStat = playerStat.pause;
        if(PlayerPrefs.GetInt("coin") < 50)
        {
            AnsBtn.interactable = false;
            if (PlayerPrefs.GetInt("coin") < 5)
            {
                NewMapBtn.interactable = false;
            }
        }
        helpPanel.SetBool("isShow", true);
    }
    public void CloseHelpPop()
    {
        helpPanel.SetBool("isShow", false);
        MM.PM.nowStat = statBeforePop;
    }

    public void updateMoveCount(int move)
    {
        //moveCountText.text = move.ToString();
        int remain = (int)MoveCounterSlider.maxValue - move;
        MoveCounterSlider.value = remain >= 0 ? remain : 0;
        if(remain < 0)
        {
            setSliderStarColor(0);
        }
        else if (remain < 3)
        {
            setSliderStarColor(1);
        }
        else if (remain < 6)
        {
            setSliderStarColor(2);
        }
        else if (remain < 9)
        {
            setSliderStarColor(3);
        }
    }
    public void regenMap()
    {
        AddCoin(-5);
        MM.Map.GenerateMap();
    }

    public void ShowSolution()
    {
        AddCoin(-50);
        MM.Map.LoadSavedMap();
        gridNum.SetActive(true);
        List<Move> tmp = MM.Map.solution;
        string ans = "";
        for (int i = 1; i < tmp.Count; i++)
        {
            ans += ((char)(MM.Map.getIndexX(tmp[i].endPos) + (int)'A') + (MM.Map.getIndexY(tmp[i].endPos) + 1).ToString() + " ");
            if (tmp.Count > 14 && i == (int)(tmp.Count / 2))
                ans += "\n";
        }
        ans += ((char)(MM.Map.getIndexX(tmp[tmp.Count - 1].startPos) + (int)'A') + (MM.Map.getIndexY(tmp[tmp.Count - 1].startPos) + 1).ToString() + " ");
        instructionText.text = ans;
    }

    public void LoadHome()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void setSliderMoves(int moves)
    {
        MoveCountSliderFill.pixelsPerUnitMultiplier = 0.108f * (moves + 6);
        MoveCountSliderBGfill.pixelsPerUnitMultiplier = 0.108f * (moves + 6);
        MoveCounterSlider.maxValue = (moves + 6);
        MoveCounterSlider.value = MoveCounterSlider.maxValue;
        Vector2 origin = sliderStars[0].GetComponent<RectTransform>().anchoredPosition;
        float blockWidth = 1200 / (moves + 6);
        sliderStars[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(origin.x + blockWidth * 3, origin.y);
        sliderStars[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(origin.x + blockWidth * 6, origin.y);
        totalMoveText.text = (moves + 6).ToString();
        setSliderStarColor(4);

        ppSlotController.SetInteger("slot", MM.PM.playerCycle.Length);
        for(int i = 0; i < MM.PM.playerCycle.Length; i++)
        {
            playerSlots[i].sprite = playerSlotSprite[(int)MM.PM.playerCycle[i]];
        }
    }

    void setSliderStarColor(int starCount)
    {
        switch (starCount)
        {
            case 0:
                sliderStars[0].GetComponent<Image>().color = starColor[0];
                sliderStars[1].GetComponent<Image>().color = starColor[0];
                sliderStars[2].GetComponent<Image>().color = starColor[0];
                break;
            case 1:
                sliderStars[0].GetComponent<Image>().color = starColor[1];
                sliderStars[1].GetComponent<Image>().color = starColor[0];
                sliderStars[2].GetComponent<Image>().color = starColor[0];
                break;
            case 2:
                sliderStars[0].GetComponent<Image>().color = starColor[2];
                sliderStars[1].GetComponent<Image>().color = starColor[2];
                sliderStars[2].GetComponent<Image>().color = starColor[0];
                break;
            case 3:
                sliderStars[0].GetComponent<Image>().color = starColor[3];
                sliderStars[1].GetComponent<Image>().color = starColor[3];
                sliderStars[2].GetComponent<Image>().color = starColor[3];
                break;
            case 4:
                sliderStars[0].GetComponent<Image>().color = starColor[4];
                sliderStars[1].GetComponent<Image>().color = starColor[4];
                sliderStars[2].GetComponent<Image>().color = starColor[4];
                break;
            default:
                break;
        }
    }

    public void Undo()
    {
        if(MM.HM.Undo().stageMoves != -1)
        {
            MM.Map.LoadHistory(MM.HM.Undo());
        }
    }

    public void Redo()
    {
        if (MM.HM.Redo().stageMoves != -1)
        {
            MM.Map.LoadHistory(MM.HM.Redo());
        }
    }
}
