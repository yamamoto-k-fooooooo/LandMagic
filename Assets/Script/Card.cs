using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public class CardInfo
    {
        //�F
        public int cardType { get; private set; }
        //����
        public int cardEffect { get; private set; }

        public CardInfo(int type, int effect)
        {
            cardType = type;
            cardEffect = effect;
        }
    }
    //�傫��
    Vector3 cardLocalScale;

    HandManager codeHandManager;
    RectTransform codeRectTransform;
    RectTransform codeCardLayoutRectTransform;

    //��D�̉��Ԗڂɂ��邩
    int handNum = 0;

    bool playedBool = false;

    public void Initialization()
    {
        cardLocalScale = transform.localScale;
        codeHandManager = GameObject.Find("CardGameManager").GetComponent<HandManager>();
        codeRectTransform = GetComponent<RectTransform>();
        codeCardLayoutRectTransform = codeRectTransform.parent as RectTransform;
    }

    //EventTrigger > PointerEnter�ŌĂ�
    //�J�[�\���������ƍőO�ʁA�傫���\������
    void CardBigDisplay()
    {
        if (playedBool || transform.localScale != cardLocalScale)
        {
            return;
        }
        //���ɑ��̃J�[�h���g��\������Ă���
        if(codeHandManager.bigDisplayHandObject != null && codeHandManager.bigDisplayHandObject != gameObject)
        {
            if (codeHandManager.draggingBool)
            {
                return;
            }
        }

        handNum = gameObject.transform.GetSiblingIndex();
        //�őO���
        transform.SetAsLastSibling();
        //������ɓ������Č��₷������
        transform.localPosition = new Vector3(
            transform.localPosition.x, 
            transform.localPosition.y + 80, 
            transform.localPosition.z
           );
        //�\�����܂�������
        transform.localRotation = Quaternion.identity;
        //�g��
        transform.localScale = new Vector3(
            cardLocalScale.x * 1.5f,
            cardLocalScale.y * 1.5f,
            cardLocalScale.z
           );

        codeHandManager.bigDisplayHandObject = gameObject;
    }
    //EventTrigger > PointerExit�ŌĂ�
    //�J�[�\�����O���ƕ\����߂�
    public void CardRevertDisplay()
    {
        if (codeHandManager.draggingBool || playedBool)
        {
            return;
        }
        RevertDisplay();
        codeHandManager.HandLayoutCalclate();
    }
    public void RevertDisplay()
    {
        transform.SetSiblingIndex(handNum);
        transform.localScale = cardLocalScale;
        codeHandManager.bigDisplayHandObject = null;
    }

    // �h���b�O���̏���
    public void OnDrag(PointerEventData eventData)
    {
        if (playedBool)
        {
            return;
        }

        //�g��\��
        if (codeHandManager.bigDisplayHandObject != null)
        {
            //���Ɋg��\�����Ă����D������ꍇ�͖߂�
            if (codeHandManager.bigDisplayHandObject != gameObject)
            {
                codeHandManager.HandObjectRevertDisplay();
                CardBigDisplay();
            }
        }
        else
        {
            CardBigDisplay();
        }

        // eventData.position����A�e�ɏ]��localPosition�ւ̕ϊ����s��
        // �I�u�W�F�N�g�̈ʒu��localPosition�ɕύX����
        Vector2 localPosition = GetLocalPosition(eventData.position);
        codeRectTransform.anchoredPosition = localPosition;

        //�v���C�]�[���ɂ��邩
        if (codeHandManager.InPlayArea(localPosition.y))
        {

        }
        else
        {
            //��D���ёւ��`�F�b�N
            handNum = codeHandManager.HandSwapCheck(localPosition.x, handNum);
        }
    }
    // ScreenPosition����localPosition�ւ̕ϊ��֐�
    private Vector2 GetLocalPosition(Vector2 screenPosition)
    {
        Vector2 result = Vector2.zero;

        // screenPosition��e�̍��W�n(parentRectTransform)�ɑΉ�����悤�ϊ�����.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(codeCardLayoutRectTransform, screenPosition, Camera.main, out result);

        return result;
    }

    // �h���b�O�I�����̏���
    public void OnEndDrag(PointerEventData eventData)
    {
        if (playedBool)
        {
            return;
        }
        codeHandManager.draggingBool = false;

        // eventData.position����A�e�ɏ]��localPosition�ւ̕ϊ����s��
        // �I�u�W�F�N�g�̈ʒu��localPosition�ɕύX����
        Vector2 localPosition = GetLocalPosition(eventData.position);

        //�v���C�]�[���ɏo������
        if (codeHandManager.InPlayArea(localPosition.y))
        {
            playedBool = true;
            codeHandManager.CardPlay(handNum);
            return;
        }

        CardRevertDisplay();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        codeHandManager.draggingBool = true;
    }
}
