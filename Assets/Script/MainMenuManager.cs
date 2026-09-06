using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public DeckEditManager DEM;
    public Animator CustomChapterSelectAnim;
    public Animator StoryChapterSelectAnim;
    public Animator NameAsk;
    public TMP_Text LengthBonusInstrText;
    public Button StoryStartBtn;
    public Button CustomStartBtn;
    public GameObject StoryStagePop;
    public GameObject[] starGrids;
    public GameObject[] stageBtns;
    public GameObject stageSelector;
    public GameObject[] InstStarGrid;

    public TMP_Text[] DataNCR;
    public TMP_Text killText;

    public TMP_InputField nameField;

    public GameObject hellCost;

    public AudioSource BGMsor;

    GameObject[] chStarGrid;
    public float chBtnExtraDist;
    public GameObject[] storyChapterBtns;
    public GameObject storyChapterSelector;
    public GameObject[] customChapterBtns;
    public GameObject customChapterSelector;
    public GameObject[] lengthBtns;
    public GameObject lengthSelector;
    public GameObject[] diffBtns;
    public GameObject diffSelector;

    public GameObject creditPanel;

    public int[] LengthUnlockPrice;
    public GameObject[] LengthLocks;
    public int[] DiffUnlockPrice;
    public GameObject[] DiffLocks;
    public GameObject CustomLock;
    public price[] PPunlockPrice;
    public GameObject[] PPLocks;
    public GameObject PPLock;
    public Animator PPanim;
    public GameObject DevPanel;

    private int a;

    [System.Serializable]
    public struct price
    {
        public bool isRuby;
        public int amount;
    }

    // Start is called before the first frame update
    void Start()
    {
        InstStarGrid = new GameObject[20];
        chStarGrid = new GameObject[storyChapterBtns.Length];
        if (!PlayerPrefs.HasKey("playerName"))
        {
            NameAsk.SetBool("isShow", true);
        }
        else
        {
            SetData();
        }
        hellCost.SetActive(false);
        setChapterStar();
        //storyChapterSelector.transform.position = storyChapterBtns[PlayerPrefs.GetInt("nowChapter")].transform.position;
        customChapterSelector.transform.position = storyChapterBtns[PlayerPrefs.GetInt("nowChapter")].transform.position;

        lengthSelector.transform.position = lengthBtns[PlayerPrefs.GetInt("length")].transform.position;

        diffSelector.transform.position = diffBtns[PlayerPrefs.GetInt("diff")].transform.position;
        ShowBonus();
        killText.text = PlayerPrefs.GetInt("kill").ToString();
        SetLock();
        a = 0;
    }

    public void LoadGame()
    {
        if (PlayerPrefs.GetInt("testMode") == 1)
        {
            PlayerPrefs.SetInt("coin", PlayerPrefs.GetInt("coin") - 50);
        }
        SceneManager.LoadScene("MainGame");
    }

    public void LoadTutorial()
    {
        PlayerPrefs.SetInt("nowChapter", 0);
        PlayerPrefs.SetInt("nowStage", 0);
        SceneManager.LoadScene("MainGame");
    }

    public void SetMode(int testMode)
    {
        PlayerPrefs.SetInt("testMode", testMode);
        if(testMode == 2)
        {
            PlayerPrefs.SetInt("nowStage", 0);
        }
    }

    public void OpenChapterSelect(bool isStory)
    {
        if (isStory)
        {
            StoryChapterSelectAnim.SetBool("isShow", true);
        }
        else
        {
            CustomChapterSelectAnim.SetBool("isShow", true);
        }
    }

    public void CloseStageSelect()
    {
        if (StoryStagePop.activeSelf)
        {
            StoryStagePop.SetActive(false);
        }
        else
        {
            if (StoryChapterSelectAnim.GetBool("isShow"))
            {
                StoryChapterSelectAnim.SetBool("isShow", false);
            }
            else
            {
                CustomChapterSelectAnim.SetBool("isShow", false);
            }
        }
    }


    public void SetChapter(int chap)
    {
        PlayerPrefs.SetInt("nowChapter", chap);
        if(PlayerPrefs.GetInt("testMode") == 1)//지옥모드
        {
            /*
            PlayerPrefs.SetInt("nowStage", 0);
            if (PlayerPrefs.GetInt("coin") >= 50)
            {
                StoryStartBtn.interactable = true;
                storyChapterSelector.transform.position = storyChapterBtns[chap].transform.position;
                storyChapterSelector.SetActive(true);
            }*/
        }
        else if (PlayerPrefs.GetInt("testMode") == 0)//스토리 모드
        {
            string[] nowData = (PlayerPrefs.GetString("chapProg").Split(']'))[chap].Split(',');
            bool unlockLast = false;
            for (int i = 0; i < 20; i++)
            {
                Destroy(InstStarGrid[i]);
                stageBtns[i].GetComponent<Button>().interactable = true;
                stageBtns[i].GetComponentInChildren<TMP_Text>().color = stageBtns[i].GetComponent<Button>().colors.normalColor;
                int temp = int.Parse(nowData[i]);

                if (temp > 0)
                {
                    InstStarGrid[i] = Instantiate(starGrids[temp], stageBtns[i].transform.position, Quaternion.identity);
                    InstStarGrid[i].transform.parent = stageBtns[i].transform;
                    InstStarGrid[i].transform.localScale = Vector3.one;
                }
                else if (temp < 0 && !unlockLast)
                {
                    unlockLast = true;
                }
                else if (temp != 0)
                {
                    stageBtns[i].GetComponent<Button>().interactable = false;
                    stageBtns[i].GetComponentInChildren<TMP_Text>().color = stageBtns[i].GetComponent<Button>().colors.disabledColor;
                }
            }
            StoryStagePop.SetActive(true);
            storyChapterSelector.transform.position = storyChapterBtns[chap].transform.position;
        }
        else if (PlayerPrefs.GetInt("testMode") == 2)//커스텀 모드
        {
            customChapterSelector.transform.position = customChapterBtns[chap].transform.position;
        }

    }
    /*
    public void SetStageInstr(string instr)
    {
        InstrText.text = instr;
    }*/

    public void SetStageNo(int no)
    {
        PlayerPrefs.SetInt("nowStage", no);
        StoryStartBtn.interactable = true;
        stageSelector.transform.position = stageBtns[no].transform.position;
    }

    public void NameOk()
    {
        PlayerPrefs.SetString("playerName", nameField.text);
        NameAsk.SetBool("isShow", false);
        InitializeData();
        SetData();
    }

    public void InitializeData()
    {
        PlayerPrefs.SetInt("coin", 0);
        PlayerPrefs.SetInt("ruby", 0);
        PlayerPrefs.SetString("chapProg", "-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]");
        PlayerPrefs.SetInt("diff", 0);
        PlayerPrefs.SetInt("length", 0);
        PlayerPrefs.SetFloat("musicVol", 0.3f);
        PlayerPrefs.SetFloat("sfxVol", 0.18f);
        PlayerPrefs.SetInt("fps", 60);
        PlayerPrefs.SetInt("kill", 0);
        PlayerPrefs.SetInt("lockLength", 0);
        PlayerPrefs.SetInt("lockDiff", 0);
        PlayerPrefs.SetInt("lockPP", 0);
        PlayerPrefs.SetString("deck", "0,1,2");
        SetData();
    }

    public void setUpdatedData()
    {
        if (!PlayerPrefs.HasKey("musicVol"))
        {
            PlayerPrefs.SetFloat("musicVol", 0.3f);
            PlayerPrefs.SetFloat("sfxVol", 0.18f);
            PlayerPrefs.SetInt("fps", 60);
        }

        if (!PlayerPrefs.HasKey("lockDiff"))
        {
            PlayerPrefs.SetInt("diff", 0);
            PlayerPrefs.SetInt("length", 0);
            PlayerPrefs.SetInt("kill", 0);
            PlayerPrefs.SetInt("lockLength", 0);
            PlayerPrefs.SetInt("lockDiff", 0);
            PlayerPrefs.SetInt("coin", 0);
            PlayerPrefs.SetInt("ruby", 0);
            PlayerPrefs.SetString("chapProg", "-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]");
        }

        if (!PlayerPrefs.HasKey("lockPP"))
        {
            PlayerPrefs.SetInt("lockPP", 0);
            PlayerPrefs.SetString("deck", "0,1,2");
        }
    }

    public void SetData()
    {
        setUpdatedData();
        DataNCR[0].text = PlayerPrefs.GetString("playerName");
        DataNCR[1].text = PlayerPrefs.GetInt("coin").ToString();
        DataNCR[2].text = PlayerPrefs.GetInt("ruby").ToString();
        BGMsor.volume = PlayerPrefs.GetFloat("musicVol");
        Application.targetFrameRate = PlayerPrefs.GetInt("fps");
    }

    public void setChapterStar()
    {
        string[] nowData;
        for(int i = 0; i < storyChapterBtns.Length; i++)
        {
            nowData = (PlayerPrefs.GetString("chapProg").Split(']'))[i].Split(',');
            int stars = nowData.Length == 20 ? 3 : 0;
            for(int j = 0; j < nowData.Length; j++)
            {
                if (nowData[j] != "" && int.Parse(nowData[j]) < stars)
                {
                    stars = int.Parse(nowData[j]);
                }
            }
            if(stars > 0)
            {
                chStarGrid[i] = Instantiate(starGrids[stars], storyChapterBtns[i].transform.position + Vector3.down * chBtnExtraDist + Vector3.right * chBtnExtraDist, Quaternion.identity);
                chStarGrid[i].transform.parent = storyChapterBtns[i].transform;
                chStarGrid[i].transform.localScale = Vector3.one;
            }
        }

    }

    public void SetLength(int length)
    {
        PlayerPrefs.SetInt("length", length);
        lengthSelector.transform.position = lengthBtns[length].transform.position;
        ShowBonus();
    }
    public void SetDifficulty(int sum)
    {
        PlayerPrefs.SetInt("diff", sum);
        diffSelector.transform.position = diffBtns[PlayerPrefs.GetInt("diff")].transform.position;
        ShowBonus();
    }


    public void ShowBonus()
    {
        //보너스 계수 표기
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
                lengthBonus = 4f;
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
                diffBonus = 4f;
                break;
            default:
                break;
        }
        if (Mathf.Abs(diffBonus - lengthBonus) > 1f)
        {
            LengthBonusInstrText.text = "최대 코인 보너스 x" + (diffBonus > lengthBonus ? lengthBonus + 1.5f : diffBonus + 1.5f);
        }
        else if (Mathf.Abs(diffBonus - lengthBonus) == 1f)
        {
            if(diffBonus ==4 || lengthBonus == 4)
            {
                LengthBonusInstrText.text = "최대 코인 보너스 x" + (diffBonus > lengthBonus ? lengthBonus + 1.5f : diffBonus + 1.5f);
            }
            else
            {
                LengthBonusInstrText.text = "최대 코인 보너스 x" + (diffBonus > lengthBonus ? lengthBonus + 1f : diffBonus + 1f);
            }
        }
        else
        {
            if (diffBonus == 4 && lengthBonus == 4)
            {
                LengthBonusInstrText.text = "최대 코인 보너스 x" + (diffBonus > lengthBonus ? lengthBonus + 2f : diffBonus + 2f);
            }
            else
            {
                LengthBonusInstrText.text = "최대 코인 보너스 x" + (diffBonus > lengthBonus ? lengthBonus : diffBonus);
            }
        }

    }

    public void ShowCredit()
    {
        creditPanel.GetComponent<Animator>().SetBool("isShow", true);
    }

    public void CloseCredit()
    {
        creditPanel.GetComponent<Animator>().SetBool("isShow", false);
    }

    public void PayCoin(int index)
    {
        if (PlayerPrefs.GetInt("coin") >= LengthUnlockPrice[index])
        {
            PlayerPrefs.SetInt("coin", PlayerPrefs.GetInt("coin") - LengthUnlockPrice[index]);
            PlayerPrefs.SetInt("lockLength", PlayerPrefs.GetInt("lockLength") + (int)Mathf.Pow(10, index));
            Debug.Log(PlayerPrefs.GetInt("lockLength"));
            SetLock();
        }
    }
    public void PayRuby(int index)
    {
        if (PlayerPrefs.GetInt("ruby") >= DiffUnlockPrice[index])
        {
            PlayerPrefs.SetInt("ruby", PlayerPrefs.GetInt("ruby") - DiffUnlockPrice[index]);
            PlayerPrefs.SetInt("lockDiff", PlayerPrefs.GetInt("lockDiff") + (int)Mathf.Pow(10, index));
            Debug.Log(PlayerPrefs.GetInt("lockDiff"));
            SetLock();
        }
    }

    public void unlockPP(int index)
    {
        if (PPunlockPrice[index].isRuby)
        {
            if(PlayerPrefs.GetInt("ruby") >= PPunlockPrice[index].amount)
            {
                PlayerPrefs.SetInt("ruby", PlayerPrefs.GetInt("ruby") - PPunlockPrice[index].amount);
                PlayerPrefs.SetInt("lockPP", PlayerPrefs.GetInt("lockPP") + (int)Mathf.Pow(10, index));
                SetLock();
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("coin") >= PPunlockPrice[index].amount)
            {
                PlayerPrefs.SetInt("coin", PlayerPrefs.GetInt("coin") - PPunlockPrice[index].amount);
                PlayerPrefs.SetInt("lockPP", PlayerPrefs.GetInt("lockPP") + (int)Mathf.Pow(10, index));
                SetLock();
            }
        }
    }

    public void SetLock()
    {
        int tmp = PlayerPrefs.GetInt("lockLength");
        int tmp2 = PlayerPrefs.GetInt("lockDiff");
        int tmp3 = PlayerPrefs.GetInt("lockPP");
        for (int i = 0; i < 3; i++)
        {
            if (tmp % 10 == 1)
            {
                LengthLocks[i].SetActive(false);
            }
            if (tmp2 % 10 == 1)
            {
                DiffLocks[i].SetActive(false);
                if (i == 1)
                {
                    PPLock.SetActive(false);
                }
            }
            tmp /= 10;
            tmp2 /= 10;
        }

        for (int i = 0; i < PPLocks.Length; i++)
        {
            if (tmp3 % 10 == 1)
            {
                PPLocks[i].SetActive(false);
            }
            tmp3 /= 10;
        }

        string[] nowData = (PlayerPrefs.GetString("chapProg").Split(']'))[0].Split(',');
        for (int i = 0; i < 20; i++)
        {
            int temp = int.Parse(nowData[i]);
            //Debug.Log("stage " + i + ", data " + temp);
            if (i == 8 && temp >= 0)
            {
                CustomLock.SetActive(false);
            }
        }
        SetData();
    }

    public void openPPedit()
    {
        PPanim.SetBool("isShow", true);
        Invoke("delayedDeckUpdate", 0.1f);
    }

    void delayedDeckUpdate()
    {
        DEM.updateDeck();
    }

    public void closePPedit()
    {
        PPanim.SetBool("isShow", false);
    }

    public void Debug1(int amount)
    {
        PlayerPrefs.SetInt("coin", PlayerPrefs.GetInt("coin") + amount);
        SetData();
    }
    public void Debug2(int amount)
    {
        PlayerPrefs.SetInt("ruby", PlayerPrefs.GetInt("ruby") + amount);
        SetData();
    }

    public void resetData()
    {
        PlayerPrefs.DeleteKey("playerName");
        Start();
    }

    public void ActivateDev()
    {
        if (a == 9)
        {
            DevPanel.SetActive(true);
        } else if (a > 9)
        {
            DevPanel.SetActive(false);
            a = 0;
        }
        a++;
    }

    public void skipTut()
    {
        PlayerPrefs.SetString("chapProg", "3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]");
        Start();
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
