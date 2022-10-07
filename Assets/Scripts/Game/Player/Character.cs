using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //외곽에는 못 나가도록 충돌처리하기 위하여
    private CharacterMove characterMove;
    private CharacterStatus characterStatus;
    private Vector2 movingLocation;

    // Start is called before the first frame update
    void Start()
    {
        characterMove = FindObjectOfType<CharacterMove>();
        characterStatus = FindObjectOfType<CharacterStatus>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //외곽에는 못나가도록 하기 위하여, speed속도만큼 튕겨져 나온다
        if (collision.CompareTag("Block"))
        {         
            if (characterMove.isUp)
            {
                characterMove.isUp = false;
                characterMove.dashTime = 0f;
                characterMove.isDash = false;
                characterMove.isCanMove = false;
                movingLocation = new Vector2(characterMove.character.anchoredPosition.x, characterMove.character.anchoredPosition.y - 0.15f);
                gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(characterMove.character.anchoredPosition, movingLocation, characterMove.speed);
                characterMove.camera.position = new Vector3(characterMove.character.anchoredPosition.x, characterMove.character.anchoredPosition.y, -10);
            }
            if (characterMove.isDown)
            {
                characterMove.isDown = false;
                characterMove.dashTime = 0f;
                characterMove.isDash = false;
                characterMove.isCanMove = false;
                movingLocation = new Vector2(characterMove.character.anchoredPosition.x, characterMove.character.anchoredPosition.y + 0.15f);
                gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(characterMove.character.anchoredPosition, movingLocation, characterMove.speed);
                characterMove.camera.position = new Vector3(characterMove.character.anchoredPosition.x, characterMove.character.anchoredPosition.y, -10);
            }
            if (characterMove.isRight)
            {
                characterMove.isRight = false;
                characterMove.dashTime = 0f;
                characterMove.isDash = false;
                characterMove.isCanMove = false;
                movingLocation = new Vector2(characterMove.character.anchoredPosition.x - 0.15f, characterMove.character.anchoredPosition.y);
                gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(characterMove.character.anchoredPosition, movingLocation, characterMove.speed);
                characterMove.camera.position = new Vector3(characterMove.character.anchoredPosition.x, characterMove.character.anchoredPosition.y, -10);
            }
            if (characterMove.isLeft)
            {
                characterMove.isLeft = false;
                characterMove.dashTime = 0f;
                characterMove.isDash = false;
                characterMove.isCanMove = false;
                movingLocation = new Vector2(characterMove.character.anchoredPosition.x + 0.15f, characterMove.character.anchoredPosition.y);
                gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(characterMove.character.anchoredPosition, movingLocation, characterMove.speed);
                characterMove.camera.position = new Vector3(characterMove.character.anchoredPosition.x, characterMove.character.anchoredPosition.y, -10);
            }

            if(!characterMove.isUp && !characterMove.isDown && !characterMove.isRight&& !characterMove.isLeft) //끼어있는거 내보내야지
            {
                if (gameObject.GetComponent<RectTransform>().anchoredPosition.y > 0f)
                {
                    movingLocation = new Vector2(characterMove.character.anchoredPosition.x, characterMove.character.anchoredPosition.y - 0.15f);
                    gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(characterMove.character.anchoredPosition, movingLocation, characterMove.speed);
                    characterMove.camera.position = new Vector3(characterMove.character.anchoredPosition.x, characterMove.character.anchoredPosition.y, -10);
                }
                if (gameObject.GetComponent<RectTransform>().anchoredPosition.y < 0f)
                {
                    movingLocation = new Vector2(characterMove.character.anchoredPosition.x, characterMove.character.anchoredPosition.y + 0.15f);
                    gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(characterMove.character.anchoredPosition, movingLocation, characterMove.speed);
                    characterMove.camera.position = new Vector3(characterMove.character.anchoredPosition.x, characterMove.character.anchoredPosition.y, -10);
                }
                if (gameObject.GetComponent<RectTransform>().anchoredPosition.x > 0f)
                {
                    movingLocation = new Vector2(characterMove.character.anchoredPosition.x - 0.15f, characterMove.character.anchoredPosition.y);
                    gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(characterMove.character.anchoredPosition, movingLocation, characterMove.speed);
                    characterMove.camera.position = new Vector3(characterMove.character.anchoredPosition.x, characterMove.character.anchoredPosition.y, -10);
                }
                if (gameObject.GetComponent<RectTransform>().anchoredPosition.x < 0f)
                {
                    movingLocation = new Vector2(characterMove.character.anchoredPosition.x + 0.15f, characterMove.character.anchoredPosition.y);
                    gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(characterMove.character.anchoredPosition, movingLocation, characterMove.speed);
                    characterMove.camera.position = new Vector3(characterMove.character.anchoredPosition.x, characterMove.character.anchoredPosition.y, -10);
                }
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision) //나가야 이동가능
    {
        if (collision.CompareTag("Block"))
        {
            characterMove.isCanMove = true;
        }
    }
}
