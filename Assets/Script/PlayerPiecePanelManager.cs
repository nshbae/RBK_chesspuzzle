using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPiecePanelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Color bright;
    public Color dark;
    public Color brightPiece;
    public Color darkPiece;

    public Image[] Blocks;
    public Image[] Pieces;

    // Update is called once per frame
    public void UpdatePanelStat(int pp)
    {
        for(int i = 0; i < Blocks.Length; i++)
        {
            if(i == pp)
            {
                Blocks[i].color = bright;
                Pieces[i].color = darkPiece;
            }
            else
            {
                Blocks[i].color = dark;
                Pieces[i].color = brightPiece;
            }
        }
    }
}
