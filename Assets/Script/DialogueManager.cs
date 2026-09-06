using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public enum ImagePos
{
    left,
    right,
    BG
}

[System.Serializable]
public enum ImageCode
{
    일반,
    생각,
    고민,
    짜증,
    버럭,
    흥미,
    미소,
    빵긋,
    공포,
    당황,
    놀람,
    우울,
    울먹,
    정색,
    혐오,
    딴청,
    지루,
    자신,
    부끄
}

[System.Serializable]
public struct characterImageData
{
    public string name;
    public Sprite[] ImageArr;
    /*
    public Sprite idle;
    public Sprite angry;
    public Sprite curious;
    public Sprite happy;
    public Sprite trust;
    public Sprite horror;
    public Sprite surprise;
    public Sprite sad;
    public Sprite disgust;*/
}

[System.Serializable]
public struct BGimages
{
    public Sprite[] BGImageArr;
}

[System.Serializable]
public struct Dialogue
{
    public string name;
    public Sprite charSprite;
    public ImagePos pos;
    public string dialogueString;

    public Dialogue(string name, Sprite charSprite, ImagePos pos, string dialogueString)
    {
        this.name = name;
        this.charSprite = charSprite;
        this.pos = pos;
        this.dialogueString = dialogueString;
    }
}

[System.Serializable]
public struct DialoguePhase
{
    public int stageNo;
    public bool isEnd;
    public List<Dialogue> dialogueContent;

    public DialoguePhase(int stageNo, bool isEnd)
    {
        this.stageNo = stageNo;
        this.isEnd = isEnd;
        this.dialogueContent = new List<Dialogue>();
    }
}

public class DialogueManager : MonoBehaviour
{
    public MainManager MM;
    public Animator anim;
    public int nowPhaseInd;
    public int nowDialogueInd;
    public GameObject NameBox;

    public Image[] ImgPos;//left, right, BG순
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    public characterImageData[] characterImageDict;
    public BGimages[] BGimageDict;
    public string[] TotalData;//프롤로그, 화산, 도서관, 사막, 숲, 동굴, 바다, 우주, 설원 순서
    public List<DialoguePhase> nowPhaseList;
    public GameObject blackBG;

    public int diaTxtState; //0숨김, 1표기중, 2표기완료
    public string finishedText;
    private float timer;
    public float scrollSpeed;

    // Start is called before the first frame update
    void Start()
    {
        diaTxtState = 0;
        timer = 0f;
        nowPhaseInd = 0;
        nowDialogueInd = 0;
        SetNowData();
        if(MM.Map.CT != ChapterType.test)
            CheckDialoguePhase(PlayerPrefs.GetInt("nowStage"), false);//컴포넌트 순서상 MapManager 뒤에 있어야 발동
    }

    void SetNowData()
    {
        for (int i = 0; i < TotalData.Length; i++)//엔터 지우기
        {
            TotalData[i] = TotalData[i].Replace("\n", "");
            TotalData[i] = TotalData[i].Replace("\r", "");
        }

        int nowChapter = (int)MM.Map.nowChapter;
        if (TotalData.Length > nowChapter)
        {
            string[] phaseData = TotalData[nowChapter].Split('[');
            nowPhaseList = new List<DialoguePhase>();

            for (int i = 1; i < phaseData.Length; i++)//각각의 phase에 대해
            {
                string[] DialogueData = phaseData[i].Split(']');
                DialoguePhase temp = new DialoguePhase();
                temp.dialogueContent = new List<Dialogue>();
                for (int j = 0; j < DialogueData.Length - 1; j++)//각각의 dialogue에 대해 (마지막 dialogue는 주석)
                {
                    string[] splitData = DialogueData[j].Split('|');
                    if (j == 0)//0번 dialogue에는 phase의 정보가 들어있음
                    {
                        temp.stageNo = int.Parse(splitData[0]);
                        temp.isEnd = (splitData[1] == "1");
                    }
                    else //일반 dialogue에 대한 데이터 처리
                    {
                        Dialogue tmp = new Dialogue();
                        Sprite thisSprite = characterImageDict[0].ImageArr[0];
                        foreach (var k in characterImageDict)//이름으로 딕셔너리에서 찾고 이미지 코드 비교해서 thisSprite 세팅 
                        {
                            if (" " + k.name == splitData[0])
                            {
                                thisSprite = k.ImageArr[(int)Enum.Parse(typeof(ImageCode), splitData[1])];
                                break;
                            }
                        }
                        if (splitData[0] == " 배경")
                        {
                            tmp = new Dialogue(splitData[0], BGimageDict[nowChapter].BGImageArr[int.Parse(splitData[1])], ImagePos.BG, splitData[2]);
                        }
                        else if (splitData.Length == 3)//이미지 위치 정보가 생략된 경우 자동으로 left으로 배정
                        {
                            tmp = new Dialogue(splitData[0], thisSprite, ImagePos.left, splitData[2]);
                        }
                        else if (splitData.Length == 4)
                        {
                            tmp = new Dialogue(splitData[0], thisSprite, ImagePos.right, splitData[3]);
                        }
                        temp.dialogueContent.Add(tmp);
                    }
                }
                nowPhaseList.Add(temp);
            }
        }
    }

    public bool CheckDialoguePhase(int stageNo, bool isCheckingAfterStageEnd)
    {
        if(MM.Map.CT == ChapterType.story)
        {
            for (int i = nowPhaseInd; i < nowPhaseList.Count; i++)
            {
                while (nowPhaseList[nowPhaseInd].stageNo < stageNo)
                {
                    nowPhaseInd++;
                }

                {
                    //Debug.Log(i + "번째 phase isEnd는 " + nowPhaseList[i].isEnd);
                    if (nowPhaseList[i].stageNo == stageNo && isCheckingAfterStageEnd == nowPhaseList[i].isEnd)
                    {
                        anim.SetBool("Active", true);
                        nowDialogueInd = 0;
                        SetDialogue(nowPhaseList[nowPhaseInd].dialogueContent[nowDialogueInd]);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void SetDialogue(Dialogue nowD)
    {
        MM.PM.nowStat = playerStat.pause;
        //이름 설정, 다이알로그 텍스트 설정
        if (nowD.name == " 해설자")
        {
            NameBox.SetActive(false);
            //dialogueText.text = nowD.dialogueString;
            dialogueText.text = "";
            finishedText = nowD.dialogueString;
            diaTxtState = 1;
        }
        else
        {
            if (nowD.name == " 배경")
            {
                nameText.text = nowD.dialogueString;
                dialogueText.text = "";
                diaTxtState = 2;
            }
            else
            {
                nameText.text = nowD.name;
                //dialogueText.text = nowD.dialogueString;
                dialogueText.text = "";
                finishedText = nowD.dialogueString;
                diaTxtState = 1;
            }
            if (!NameBox.activeSelf)
            {
                NameBox.SetActive(true);
            }
        }

        //이미지 설정
        for (int j = 0; j < ImgPos.Length; j++)
        {
            if (j == (int)nowD.pos && nowD.name != " 해설자")
            {
                ImgPos[j].sprite = nowD.charSprite;
                if (nowD.charSprite == null)
                {
                    ImgPos[j].enabled = false;
                }
                else
                {
                    ImgPos[j].SetNativeSize();
                    ImgPos[j].enabled = true;
                }
            }
            else if (j != 2)
            {
                ImgPos[j].enabled = false;
            }
        }
    }

    public void FixedUpdate()
    {
        if(diaTxtState == 1)
        {
            timer += Time.fixedDeltaTime;
            if (!MM.SM.DialogueSource.isPlaying)
            {
                MM.SM.DialogueSoundPlay();
            }
            if(timer > scrollSpeed)
            {
                timer = 0f;
                dialogueText.text = finishedText.Substring(0, dialogueText.text.Length + 1);
                if (dialogueText.text == finishedText)
                {
                    diaTxtState = 2;
                }
            }
        }
    }

    public void DialogueNext()
    {
        if(diaTxtState == 1)
        {
            dialogueText.text = finishedText;
            diaTxtState = 2;
        }else if (diaTxtState == 2)
        {
            nowDialogueInd++;
            if (nowDialogueInd < nowPhaseList[nowPhaseInd].dialogueContent.Count)
            {
                SetDialogue(nowPhaseList[nowPhaseInd].dialogueContent[nowDialogueInd]);
            }
            else
            {
                DialogueSkip();
            }
        }
    }

    public void DialogueSkip()
    {
        diaTxtState = 0;
        nowPhaseInd++;
        anim.SetBool("Active", false);
        MM.PM.ResetPlayer();
        if (nowPhaseList[nowPhaseInd - 1].isEnd)
        {
            MM.UM.LoadNextStage();
        }
    }
}
