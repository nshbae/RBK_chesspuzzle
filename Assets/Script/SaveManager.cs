using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public MainManager MM;
    public void SaveCoin()
    {
        PlayerPrefs.SetInt("coin", int.Parse(MM.UM.coinText.text));
    }

    public void SaveRuby()
    {
        PlayerPrefs.SetInt("ruby", int.Parse(MM.UM.rubyText.text));
    }
    public void SaveProgress(int chapter, int stage, int stars)
    {
        string Data = PlayerPrefs.GetString("chapProg");
        string[] temp = Data.Split(']');
        string[] tmp = temp[chapter].Split(',');
        if(int.Parse(tmp[stage]) < stars)//더 많은 별을 땄을 때만 데이터 업데이트
        {
            tmp[stage] = stars.ToString();

            Data = "";
            for (int i = 0; i < temp.Length; i++)
            {
                if (i == chapter)
                {
                    Data += tmp[0];
                    for (int j = 1; j < tmp.Length; j++)
                    {
                        Data += "," + tmp[j];
                    }
                    Data += "]";
                }
                else
                {
                    Data += temp[i] + "]";
                }
            }
            PlayerPrefs.SetString("chapProg", Data);
        }
    }
}
