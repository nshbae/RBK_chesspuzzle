using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct StageSounds
{
    public AudioClip BGM;
    public AudioClip BSF;
    public AudioClip[] footsteps;
    public AudioClip SP_SF;
}

public class SoundManager : MonoBehaviour
{
    public MainManager MM;
    public StageSounds[] SS;
    //public AudioClip[] BGM;
    //public AudioClip[] BSF;
    //public AudioClip[] RockSteps;
    //public AudioClip[] WoodBoardSteps;
    public AudioSource BGMsor;
    public AudioSource BSFsor;
    public AudioSource footStepSor;
    public AudioSource SFsor;
    public AudioClip[] stageClearSound;
    public AudioClip getRubySound;
    //public AudioClip[] SP_SF;

    public AudioSource killSoundSor;
    public AudioClip[] pawnKillClips;
    public AudioClip[] eliteKillClips;
    int killsoundInedx;

    bool stepStop;

    public AudioSource DialogueSource;

    // Start is called before the first frame update
    public void playBGM()
    {
        int i = 0;
        i = (int)MM.Map.nowChapter;
        BGMsor.clip = SS[i].BGM;
        BGMsor.Play();
        BSFsor.clip = SS[i].BSF;
        BSFsor.Play();
        killsoundInedx = 0;
    }

    public void startRockSteps()
    {
        stepStop = false;
        continueSteps();
    }

    void continueSteps()
    {
        if (!stepStop && MM.PM.nowStat!=playerStat.pause)
        {
            footStepSor.clip = SS[(int)MM.Map.nowChapter].footsteps[Random.Range(0, SS[(int)MM.Map.nowChapter].footsteps.Length)];
            footStepSor.Play();
            Invoke("continueSteps", footStepSor.clip.length);
        }
    }

    public void stopSteps()
    {
        stepStop = true;
    }

    public void playStageClear(int starcount)
    {
        SFsor.clip = stageClearSound[starcount];
        SFsor.Play();
    }

    public void playSP()
    {
        SFsor.clip = SS[(int)MM.Map.nowChapter].SP_SF;
        SFsor.Play();
    }

    public void readyGetRuby()
    {
        //Invoke("playGetRuby", 0.5f);
    }

    void playGetRuby()
    {
        SFsor.clip = getRubySound;
        SFsor.Play();
    }

    public void killSoundPlay(bool isElite)
    {
        if (isElite)
        {
            killSoundSor.clip = eliteKillClips[killsoundInedx];
        }
        else
        {
            killSoundSor.clip = pawnKillClips[killsoundInedx];
        }
        killSoundSor.Play();
        killsoundInedx++;
        if(killsoundInedx >= pawnKillClips.Length)
        {
            killsoundInedx = 0;
        }
    }

    public void DialogueSoundPlay()
    {
        DialogueSource.Play();
    }
}
