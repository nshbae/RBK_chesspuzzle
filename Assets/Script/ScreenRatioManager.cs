using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenRatioManager : MonoBehaviour
{
    public CanvasScaler mainCanvasScaler;
    public GameObject Map;

    public float h, w;
    public bool isSquare;
    // Start is called before the first frame update
    void Awake()
    {
        h = Screen.height;
        w = Screen.width;
        if(h/w >= 2.0556f)//길쭉
        {
            isSquare = false; 
            SetRatio(isSquare);
        }
        else//정사각
        {
            isSquare = true;
            SetRatio(isSquare);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(h != Screen.height)
        {
            h = Screen.height;

            if ((h / w) >= 2.0556f)//길쭉으로 변화
            {
                isSquare = false;
                SetRatio(isSquare);
            }
            if ((h / w) < 2.0556f)//정사각으로 변화
            {
                isSquare = true;
                SetRatio(isSquare);
            }
        }
        if(w != Screen.width)
        {
            w = Screen.width;

            if ((h / w) >= 2.0556f)//길쭉으로 변화
            {
                isSquare = false;
                SetRatio(isSquare);
            }
            if ((h / w) < 2.0556f)//정사각으로 변화
            {
                isSquare = true;
                SetRatio(isSquare);
            }
        }
    }

    void SetRatio(bool isSq)
    {
        if (isSq)
        {
            mainCanvasScaler.matchWidthOrHeight = 1f;
            Map.transform.localScale = Vector3.one * 0.61f;
        }
        else
        {
            mainCanvasScaler.matchWidthOrHeight = 0f;
            Map.transform.localScale = Vector3.one * 0.61f / (h/w) * 2.0556f;
        }
    }
}
