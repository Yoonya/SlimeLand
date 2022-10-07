using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    //로딩간 씬 관리
    public static string nextScene;

    [SerializeField] Image progressBar; //진행 바

    private void Start()
    {
        Time.timeScale = 1;
        SetResolution();
        StartCoroutine(LoadScene()); //로드씬 시작
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene"); //로딩씬으로 이동
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false; //씬의 로딩이 끝나면 자동으로 불러온 씬으로 이동할 것인지 설정->false시 90%만 로드하고 대기

        float timer = 0.0f;
        while (!op.isDone) //로딩이 끝나지 않으면
        {
            yield return null;

            timer += Time.deltaTime;
            if (op.progress < 0.9f) //90%대기 중이기 때문에
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else //페이크 로딩
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer); //0.9에서 1까지 1초 동안 채우도록
                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true; //나머지 불러오기
                    yield break;
                }
            }
        }

    }

    // 해상도 설정하는 함수
    public void SetResolution()
    {
        int setWidth = 1920; // 사용자 설정 너비
        int setHeight = 1080; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }

}
