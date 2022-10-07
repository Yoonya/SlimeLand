using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class Lobby : MonoBehaviour
{
    public GameObject status;
    public GameObject shop;
    public GameObject ranking;
    public GameObject admob;

    private void Awake()
    {
        //�ڳ� ���� ����
        var bro = Backend.Initialize(true);
        if (bro.IsSuccess())
        {
            // �ʱ�ȭ ���� �� ����
            //Debug.Log("�ʱ�ȭ ����!");
        }
        else
        {
            // �ʱ�ȭ ���� �� ����
            //Debug.LogError("�ʱ�ȭ ����!");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayBGM("BGM0");
        admob.SetActive(true);
        SetResolution();
    }

    public void StartBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        admob.SetActive(false);
        StartCoroutine(LoadScene("Game"));
    }

    public void SlimeBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        status.SetActive(true);
    }

    public void RankingBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        ranking.SetActive(true);
    }

    public void ShopBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        shop.SetActive(true);
    }


    public void OptionBtn()
    {
        AudioManager.instance.PlaySFX("Button");
    }

    public void QuitBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        Application.Quit();
    }

    IEnumerator LoadScene(string nextScene)
    {
        yield return new WaitForSeconds(1);
        LoadingScene.LoadScene(nextScene);
    }

    // �ػ� �����ϴ� �Լ�
    public void SetResolution()
    {
        int setWidth = 1920; // ����� ���� �ʺ�
        int setHeight = 1080; // ����� ���� ����

        int deviceWidth = Screen.width; // ��� �ʺ� ����
        int deviceHeight = Screen.height; // ��� ���� ����

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution �Լ� ����� ����ϱ�

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // ����� �ػ� �� �� ū ���
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // ���ο� �ʺ�
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
        }
        else // ������ �ػ� �� �� ū ���
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // ���ο� ����
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
        }
    }
}
