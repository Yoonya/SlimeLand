using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; //�����ڿ����� ����

    [SerializeField] Sound[] sfx = null; //ȿ����
    [SerializeField] Sound[] bgm = null; //bgm

    [SerializeField] AudioSource[] sfxPlayers = null;//ȿ����
    [SerializeField] AudioSource bgmPlayer = null;//bgm


    void Awake()
    {
        instance = this; //�����ڿ����� ����
    }

    public void PlayBGM(string bgmName) //�̸��� �޾� �� ���� ����Ǿ��ִ��� Ȯ��
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgmName == bgm[i].name)
            {
                bgmPlayer.clip = bgm[i].clip;
                bgmPlayer.Play();
            }
        }
    }

    public bool IsPlayingBGM() //���� ��������� Ȯ��
    {
        return bgmPlayer.isPlaying;
    }

    public bool IsPlayingSFX() //���� ��������� Ȯ��
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            if (sfxPlayers[i].isPlaying)
                return true;
        }
        return false;
    }

    public void StopBGM() //��� ��ž
    {
        bgmPlayer.Stop();
    }

    public void PauseBGM() //��� �Ͻ�����
    {
        bgmPlayer.Pause();
    }

    public void ReplayBGM() //��� ����
    {
        bgmPlayer.Play();
    }

    public void PlaySFX(string sfxName) //�̸��� �޾� �� ���� ����Ǿ��ִ��� Ȯ��
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (sfxName == sfx[i].name)
            {
                for (int j = 0; j < sfxPlayers.Length; j++) //ȿ���� �ҽ� ���� �������� �ִ��� Ȯ��
                {
                    if (!sfxPlayers[j].isPlaying) //��� ������ Ȯ��
                    {
                        sfxPlayers[j].clip = sfx[i].clip;
                        sfxPlayers[j].Play(); //play
                        return;
                    }
                }
                return;
            }
        }
    }
}
