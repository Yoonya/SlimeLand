using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//버튼을 이용해 캐릭터를 이동시킨다.
public class CharacterMove : MonoBehaviour
{
    public RectTransform character; //캐릭터
    public RectTransform direct;
    public Transform camera; //카메라가 캐릭터를 따라다닌다.
    public float speed; // 스피드
    public float angle = 0f;
    private Vector3 moveVector; //플레이어 이동벡터
    public bool isCanMove = true;
    public bool isUp = false;
    public bool isDown = false;
    public bool isRight = false;
    public bool isLeft = false;
    public bool isMove = false;

    public GameObject dashBtn;
    public float time= 0f;
    public float dashTime = 0f; //0.2초동안 대쉬
    public float dashSpeed = 5f;
    public bool isDash = false;

    private CharacterStatus characterStatus;
    private JoyStick joyStick;
    public Animator characterAnimator;

    void Start()
    {
        characterStatus = FindObjectOfType<CharacterStatus>();
        joyStick = FindObjectOfType<JoyStick>();

        moveVector = Vector3.zero; //플레이어 이동벡터 초기화
    }


    // Update is called once per frame
    void Update()
    {
        speed = characterStatus.spd;
        //터치패드 입력받기
        if(isMove)
            HandleInput();

        camera.position = new Vector3(character.anchoredPosition.x, character.anchoredPosition.y, -10);
        //일정시간마다 대쉬를 쓸수있도록
        time += Time.deltaTime;

        if (characterStatus.equipCombo == 2) //바람슬라임
        {
            if (time >= 5f)
            {
                time = 5f;
                dashBtn.GetComponent<Button>().interactable = true;
            }

            dashBtn.GetComponent<Image>().fillAmount = time / 5.0f;
        }
        else
        {
            if (time >= 10f)
            {
                time = 10f;
                dashBtn.GetComponent<Button>().interactable = true;
            }
            dashBtn.GetComponent<Image>().fillAmount = time / 10.0f;
        }

        if (isCanMove)
        {
            if(isMove)
                Move();
            /*
            //임시 키보드
            if (Input.GetKeyDown(KeyCode.W))
                isUp = true;
            if (Input.GetKeyUp(KeyCode.W))
                isUp = false;
            if (Input.GetKeyDown(KeyCode.S))
                isDown = true;
            if (Input.GetKeyUp(KeyCode.S))
                isDown = false;
            if (Input.GetKeyDown(KeyCode.A))
                isLeft = true;
            if (Input.GetKeyUp(KeyCode.A))
                isLeft = false;
            if (Input.GetKeyDown(KeyCode.D))
                isRight = true;
            if (Input.GetKeyUp(KeyCode.D))
                isRight = false;

            if (isUp)
                UpButton();
            if (isDown)
                DownButton();
            if (isRight)
                RightButton();
            if (isLeft)
                LeftButton();
            
            if (!isUp && !isDown && !isRight && !isLeft)
                isMove = false;

            if (Input.GetKeyDown(KeyCode.Space) && dashBtn.GetComponent<Button>().interactable) //임시
            {
                DashButton();
            }
*/

            if (isDash) //대쉬상태
                dashTime += Time.deltaTime;
            if (dashTime > 0.2f) //대쉬 상태 초기화
            {
                dashTime = 0f;
                isDash = false;
            }

            if (isMove)
                characterAnimator.SetInteger("MoveIndex", 1);
            else
                characterAnimator.SetInteger("MoveIndex", 0);
        }

    }

    public void HandleInput()
    {
        moveVector = PoolInput();
    }

    public Vector3 PoolInput()
    {
        float h = joyStick.GetHorizontalValue();
        float v = joyStick.GetVerticalValue();
        
        //diret의 각도를 구한다.
        angle = Mathf.Atan2(h * -1, v) * Mathf.Rad2Deg;

        direct.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        Vector3 moveDir = new Vector3(h, v, 0).normalized;

        if (h > 0)
            isRight = true;
        else
            isRight = false;
        if (h < 0)
            isLeft = true;
        else
            isLeft = false;
        if (v > 0)
            isUp = true;
        else
            isUp = false;
        if (v < 0)
            isDown = true;
        else
            isDown = false;

        return moveDir;
    }

    public void Move()
    {
        if (isDash)
            character.Translate(moveVector * speed * dashSpeed * Time.deltaTime);
        else
            character.Translate(moveVector * speed * Time.deltaTime);
    }

    //대쉬 사용 버튼
    public void DashButton()
    {
        AudioManager.instance.PlaySFX("PlayerDash");
        isDash = true;
        time = 0f; //초기화
        dashBtn.GetComponent<Button>().interactable = false;
    }
    /* 버튼과 키보드
    public void UpButton()
    {
        direct.localEulerAngles = new Vector3(0, 0, 0);
        if(isDash)
            character.anchoredPosition = new Vector2(character.anchoredPosition.x, character.anchoredPosition.y + speed * dashSpeed * Time.deltaTime);
        else
            character.anchoredPosition = new Vector2(character.anchoredPosition.x, character.anchoredPosition.y + speed * Time.deltaTime);
        camera.position = new Vector3(character.anchoredPosition.x, character.anchoredPosition.y, -10);
        isMove = true;
    }

    public void DownButton()
    {
        direct.localEulerAngles = new Vector3(0, 0, 180);
        if (isDash)
            character.anchoredPosition = new Vector2(character.anchoredPosition.x, character.anchoredPosition.y - speed * dashSpeed * Time.deltaTime);
        else
            character.anchoredPosition = new Vector2(character.anchoredPosition.x, character.anchoredPosition.y - speed * Time.deltaTime);
        camera.position = new Vector3(character.anchoredPosition.x, character.anchoredPosition.y, -10);
        isMove = true;
    }

    public void RightButton()
    {
        direct.localEulerAngles = new Vector3(0, 0, 270);
        if (isDash)
            character.anchoredPosition = new Vector2(character.anchoredPosition.x + speed * dashSpeed * Time.deltaTime, character.anchoredPosition.y);
        else
            character.anchoredPosition = new Vector2(character.anchoredPosition.x + speed * Time.deltaTime, character.anchoredPosition.y);
        camera.position = new Vector3(character.anchoredPosition.x, character.anchoredPosition.y, -10);
        isMove = true;
    }

    public void LeftButton()
    {
        direct.localEulerAngles = new Vector3(0, 0, 90);
        if(isDash)
            character.anchoredPosition = new Vector2(character.anchoredPosition.x - speed * dashSpeed * Time.deltaTime, character.anchoredPosition.y);
        else
            character.anchoredPosition = new Vector2(character.anchoredPosition.x - speed * Time.deltaTime, character.anchoredPosition.y);
        camera.position = new Vector3(character.anchoredPosition.x, character.anchoredPosition.y, -10);
        isMove = true;
    }

    public void UpButtonDown()
    {
        isUp = true;
    }
    public void UpButtonUp()
    {
        isUp = false;
    }
    public void DownButtonDown()
    {
        isDown = true;
    }
    public void DownButtonUp()
    {
        isDown = false;
    }
    public void RightButtonDown()
    {
        isRight = true;
    }
    public void RightButtonUp()
    {
        isRight = false;
    }
    public void LeftButtonDown()
    {
        isLeft = true;
    }
    public void LeftButtonUp()
    {
        isLeft = false;
    }
    */
}
