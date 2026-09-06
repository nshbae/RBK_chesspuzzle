using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionManager : MonoBehaviour
{
    public Animator optionController;
    public AudioSource BGMsor;
    public AudioSource sfxSor;

    public Button musicMuteBtn;
    public Button sfxMuteBtn;
    public Slider musicVolSlider;
    public Slider sfxVolSlider;
    public Button[] fpsBtn;

    public Sprite musicMute;
    public Sprite musicNorm;
    public Sprite sfxMute;
    public Sprite sfxNorm;

    public Sprite frame;
    public Sprite whiteRect;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetFloat("musicVol") == 0f)
            musicMuteBtn.GetComponent<Image>().sprite = musicMute;
        else
            musicMuteBtn.GetComponent<Image>().sprite = musicNorm;

        if (PlayerPrefs.GetFloat("sfxVol") == 0f)
            sfxMuteBtn.GetComponent<Image>().sprite = sfxMute;
        else
            sfxMuteBtn.GetComponent<Image>().sprite = sfxNorm;

        musicVolSlider.value = PlayerPrefs.GetFloat("musicVol");
        sfxVolSlider.value = PlayerPrefs.GetFloat("sfxVol");
        switch (PlayerPrefs.GetInt("fps"))
        {
            case 30:
                ActivateColor(0);
                break;
            case 60:
                ActivateColor(1);
                break;
            case 120:
                ActivateColor(2);
                break;
            default:
                break;
        }
    }

    void ActivateColor(int i)
    {
        fpsBtn[0].GetComponent<Image>().sprite = frame;
        fpsBtn[0].GetComponentInChildren<TMP_Text>().color = Color.white;
        fpsBtn[1].GetComponent<Image>().sprite = frame;
        fpsBtn[1].GetComponentInChildren<TMP_Text>().color = Color.white;
        fpsBtn[2].GetComponent<Image>().sprite = frame;
        fpsBtn[2].GetComponentInChildren<TMP_Text>().color = Color.white;

        fpsBtn[i].GetComponent<Image>().sprite = whiteRect;
        fpsBtn[i].GetComponentInChildren<TMP_Text>().color = Color.black;
    }

    public void clickMuteMusic()
    {
        if (PlayerPrefs.GetFloat("musicVol") == 0)
        {
            PlayerPrefs.SetFloat("musicVol", 1f);
            musicMuteBtn.GetComponent<Image>().sprite = musicNorm;
            musicVolSlider.value = 1f;
        }
        else
        {
            PlayerPrefs.SetFloat("musicVol", 0f);
            musicMuteBtn.GetComponent<Image>().sprite = musicMute;
            musicVolSlider.value = 0f;
        }
        BGMsor.volume = musicVolSlider.value;
    }

    public void clickMuteSfx()
    {
        if (PlayerPrefs.GetFloat("sfxVol") == 0)
        {
            PlayerPrefs.SetFloat("sfxVol", 1f);
            sfxMuteBtn.GetComponent<Image>().sprite = sfxNorm;
            sfxVolSlider.value = 1f;
        }
        else
        {
            PlayerPrefs.SetFloat("sfxVol", 0f);
            sfxMuteBtn.GetComponent<Image>().sprite = sfxMute;
            sfxVolSlider.value = 0f;
        }
        if (sfxSor)
        {
            sfxSor.volume = sfxVolSlider.value;
        }
    }

    public void changeMusicVol()
    {
        if(musicVolSlider.value == 0)
        {
            PlayerPrefs.SetFloat("musicVol", 0f);
            musicMuteBtn.GetComponent<Image>().sprite = musicMute;
        }
        else
        {
            PlayerPrefs.SetFloat("musicVol", musicVolSlider.value);
            if(musicMuteBtn.GetComponent<Image>().sprite == musicMute)
            {
                musicMuteBtn.GetComponent<Image>().sprite = musicNorm;
            }
        }
        BGMsor.volume = musicVolSlider.value;
    }

    public void changeSfxVol()
    {
        if (sfxVolSlider.value == 0)
        {
            PlayerPrefs.SetFloat("sfxVol", 0f);
            sfxMuteBtn.GetComponent<Image>().sprite = sfxMute;
        }
        else
        {
            PlayerPrefs.SetFloat("sfxVol", sfxVolSlider.value);
            if (sfxMuteBtn.GetComponent<Image>().sprite == sfxMute)
            {
                sfxMuteBtn.GetComponent<Image>().sprite = sfxNorm;
            }
        }
        if (sfxSor)
        {
            sfxSor.volume = sfxVolSlider.value;
        }
    }

    public void clickFps(int i)
    {
        PlayerPrefs.SetInt("fps", i);
        switch (i)
        {
            case 30:
                ActivateColor(0);
                break;
            case 60:
                ActivateColor(1);
                break;
            case 120:
                ActivateColor(2);
                break;
            default:
                break;
        }
        Application.targetFrameRate = i;
    }

    public void OpenOption()
    {
        optionController.SetBool("isShow", true);
    }

    public void CloseOption()
    {
        optionController.SetBool("isShow", false);
    }
}
