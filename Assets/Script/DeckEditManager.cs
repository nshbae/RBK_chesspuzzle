using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct clickedPP
{
    public bool isFromDeck;
    public playerPiece pp;

    public clickedPP(bool isFromDeck, playerPiece pp)
    {
        this.isFromDeck = isFromDeck;
        this.pp = pp;
    }
}
public class DeckEditManager : MonoBehaviour
{
    public MainMenuManager MMM;
    public float followSpeed;
    public Sprite[] PieceImage;
    public GameObject MouseFollower;
    public clickedPP nowClickedPP;
    public Animator describAnim;
    public string[] PPdescribtion;
    public TMP_Text PPdesc_text;
    public string[] PPflavor;
    public TMP_Text PPflavor_text;
    public TMP_Text desc_title;
    public string[] ppName;

    public GameObject[] deck;
    public Animator DeckAnim;
    public GameObject[] RefPoint;

    private Vector3 tmp;
    private string[] arr;

    public Sprite[] ppMoveImages;
    public Image ppMoveImage;

    public ScrollRect ScrollRect_ListGrid;

    public TMP_Text deckTypeInstr;

    // Start is called before the first frame update
    void Start()
    {
        MouseFollower.GetComponent<SpriteRenderer>().enabled = false;
        nowClickedPP = new clickedPP(false, playerPiece.empty);
        DeckAnim.keepAnimatorControllerStateOnDisable = true;
    }


    public void updateDeck()
    {
        arr = PlayerPrefs.GetString("deck").Split(',');
        switch (arr.Length)
        {
            case 1:
                DeckAnim.SetInteger("state", 0);
                break;
            case 2:
                DeckAnim.SetInteger("state", 3);
                break;
            case 3:
                DeckAnim.SetInteger("state", 7);
                break;
            default:
                DeckAnim.SetInteger("state", 7);
                break;
        }
        for (int i = 0; i < arr.Length; i++)
        {
            deck[i].GetComponentsInChildren<Image>()[1].sprite = PieceImage[int.Parse(arr[i])];
            deck[i].GetComponent<DeckPP_info>().info = new clickedPP(true, (playerPiece)int.Parse(arr[i]));
        }
        updateDeckTypeInstr();
    }

    // Update is called once per frame
    void Update()
    {
        if (!describAnim.GetBool("isShow"))
        {
            if (Input.GetMouseButtonDown(0))
            {
                TouchStart();
            }
            else if (nowClickedPP.pp != playerPiece.empty && Input.GetMouseButton(0))
            {
                TouchDrag();
                if (ScrollRect_ListGrid.vertical)
                {
                    ScrollRect_ListGrid.vertical = false;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                TouchEnd();
                ScrollRect_ListGrid.vertical = true;
            }
        }else if (describAnim.GetBool("isShow")&& MouseFollower.GetComponent<SpriteRenderer>().enabled)
        {
            TouchEnd();
        }
    }

    public void TouchStart()
    {
        RaycastHit hit;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D ray = Physics2D.Raycast(mousePos, Vector2.zero);
        if (ray.collider && ray.collider.gameObject.tag == "target")
        {
            clickedPP tmp = ray.collider.gameObject.GetComponent<DeckPP_info>().info;
            if (tmp.isFromDeck)//click deck
            {
                if (arr.Length > 1)
                {
                    MouseFollower.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
                    DragDeckPP(tmp.pp);
                    //Debug.Log(nowClickedPP.pp.ToString());
                    string newDeck = "";
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (ray.collider.gameObject != deck[i])
                        {
                            newDeck += arr[i];
                            newDeck += ',';
                        }
                    }
                    PlayerPrefs.SetString("deck", newDeck.Remove(newDeck.Length - 1));
                    updateDeck();
                }
            }
            else//click list
            {
                MouseFollower.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
                DragListPP(tmp.pp);
            }
        }
    }
    
    public void TouchDrag()
    {
        tmp = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
        MouseFollower.transform.position = Vector3.Lerp(MouseFollower.transform.position, tmp, Time.deltaTime * followSpeed);

        if (MouseFollower.transform.position.y < RefPoint[0].transform.position.y) //hover on deck
        {
            switch (DeckAnim.GetInteger("state"))
            {
                case 0:
                case 1:
                case 2:
                    if (MouseFollower.transform.position.x < RefPoint[0].transform.position.x)
                    {
                        DeckAnim.SetInteger("state", 1);
                    }
                    else
                    {
                        DeckAnim.SetInteger("state", 2);
                    }
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                    if (MouseFollower.transform.position.x < RefPoint[1].transform.position.x)
                    {
                        DeckAnim.SetInteger("state", 4);
                    }
                    else if (MouseFollower.transform.position.x >= RefPoint[1].transform.position.x && MouseFollower.transform.position.x < RefPoint[2].transform.position.x)
                    {
                        DeckAnim.SetInteger("state", 5);
                    }
                    else
                    {
                        DeckAnim.SetInteger("state", 6);
                    }
                    break;
                case 7:
                    PlayerPrefs.SetString("deck", arr[0] + ',' + arr[1]);
                    updateDeck();
                    break;
                default:
                    break;
            }
        }
        else if (MouseFollower.transform.position.y >= RefPoint[0].transform.position.y) //hover on list
        {
            updateDeck();
        }
    }
    
    public void TouchEnd()
    {
        if (MouseFollower.transform.position.y < RefPoint[0].transform.position.y) //drop to deck
        {
            switch (DeckAnim.GetInteger("state"))
            {
                case 1:
                    PlayerPrefs.SetString("deck", ((int)nowClickedPP.pp).ToString() + ',' + arr[0]);
                    updateDeck();
                    break;
                case 2:
                    PlayerPrefs.SetString("deck", arr[0] + ',' + ((int)nowClickedPP.pp).ToString());
                    updateDeck();
                    break;
                case 4:
                    PlayerPrefs.SetString("deck", ((int)nowClickedPP.pp).ToString() + ',' + arr[0] + ',' + arr[1]);
                    updateDeck();
                    break;
                case 5:
                    PlayerPrefs.SetString("deck", arr[0] + ',' + ((int)nowClickedPP.pp).ToString() + ',' + arr[1]);
                    updateDeck();
                    break;
                case 6:
                    PlayerPrefs.SetString("deck", arr[0] + ',' + arr[1] + ',' + ((int)nowClickedPP.pp).ToString());
                    updateDeck();
                    break;
            }
        }


        MouseFollower.GetComponent<SpriteRenderer>().enabled = false;
        MouseFollower.GetComponentInChildren<ParticleSystem>().Stop();
        nowClickedPP = new clickedPP(false, playerPiece.empty);
    }
    public void DragListPP(playerPiece index)
    {
        nowClickedPP = new clickedPP(false, index);
        MouseFollower.GetComponent<SpriteRenderer>().sprite = PieceImage[(int)nowClickedPP.pp];
        MouseFollower.GetComponent<SpriteRenderer>().enabled = true;
        MouseFollower.GetComponentInChildren<ParticleSystem>().Play();
    }
    public void DragDeckPP(playerPiece index)
    {
        if ((int)index < 4 || (int) index == 9 || ((int) index > 4 &&  (int) index < 9 && !MMM.PPLocks[(int)index - 5].activeSelf) || ((int)index > 9 && !MMM.PPLocks[(int)index - 6].activeSelf))
        {
            nowClickedPP = new clickedPP(true, index);
            MouseFollower.GetComponent<SpriteRenderer>().sprite = PieceImage[(int)nowClickedPP.pp];
            MouseFollower.GetComponent<SpriteRenderer>().enabled = true;
            MouseFollower.GetComponentInChildren<ParticleSystem>().Play();
        }
    }

    public void ShowDescribtion(int index)
    {
        PPdesc_text.text = PPdescribtion[index];
        desc_title.text = ppName[index];
        PPflavor_text.text = PPflavor[index];
        ppMoveImage.sprite = ppMoveImages[index];
        describAnim.SetBool("isShow", true);
    }

    public void CloseDescribtion()
    {
        describAnim.SetBool("isShow", false);
    }

    public void updateDeckTypeInstr()
    {
        List<playerPiece> arr2 = new List<playerPiece>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (!arr2.Contains((playerPiece)int.Parse(arr[i])) || arr[i] == ((int)playerPiece.random).ToString())
                arr2.Add((playerPiece)int.Parse(arr[i]));
        }
        float PPtype = 0.3f * arr2.Count;
        if (PPtype > 0.9f)
            PPtype = 1.0f;
        deckTypeInstr.text = "±â¹° Á¾·ù " + arr2.Count.ToString() + " - È¹µæ ÀçÈ­ " + (PPtype * 100).ToString("F0") + "% (¹ö¸²)";
    }
}
