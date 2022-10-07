using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    //�ε��� �� ����
    public static string nextScene;

    [SerializeField] Image progressBar; //���� ��

    private void Start()
    {
        Time.timeScale = 1;
        SetResolution();
        StartCoroutine(LoadScene()); //�ε�� ����
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene"); //�ε������� �̵�
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false; //���� �ε��� ������ �ڵ����� �ҷ��� ������ �̵��� ������ ����->false�� 90%�� �ε��ϰ� ���

        float timer = 0.0f;
        while (!op.isDone) //�ε��� ������ ������
        {
            yield return null;

            timer += Time.deltaTime;
            if (op.progress < 0.9f) //90%��� ���̱� ������
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else //����ũ �ε�
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer); //0.9���� 1���� 1�� ���� ä�쵵��
                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true; //������ �ҷ�����
                    yield break;
                }
            }
        }

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
