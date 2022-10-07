using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Image bgImg;
    public Image joystickImg;
    public Vector3 inputVector;

    private CharacterMove characterMove;

    private void Start()
    {
        characterMove = FindObjectOfType<CharacterMove>();
    }

    public virtual void OnDrag(PointerEventData ped) //배경 이미지가 터치받으면 조이스틱이 터치받은곳으로 이동
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

            inputVector = new Vector3(pos.x * 2, pos.y * 2, 0);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            //조이스틱 이동
            joystickImg.rectTransform.anchoredPosition = new Vector3(inputVector.x * (bgImg.rectTransform.sizeDelta.x / 3),
                inputVector.y * (bgImg.rectTransform.sizeDelta.y / 3));
        }
    }

    public virtual void OnPointerDown(PointerEventData ped) //터치하고 있을 때
    {
        OnDrag(ped);
        characterMove.isMove = true;
    }

    public virtual void OnPointerUp(PointerEventData ped)//터치 안할때 원위치
    {
        inputVector = Vector3.zero;
        joystickImg.rectTransform.anchoredPosition = Vector3.zero;
        characterMove.isUp = false;
        characterMove.isDown = false;   
        characterMove.isRight = false;
        characterMove.isLeft = false;
        characterMove.isMove = false;
        characterMove.isDash = false;
    }

    public float GetHorizontalValue() //플레이어 컨트롤 x값을 받기위함
    {
        return inputVector.x;
    }

    public float GetVerticalValue()//플레이어 컨트롤 y값을 받기위함
    {
        return inputVector.y;
    }
}