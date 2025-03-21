using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (codeHandManager.draggingBool)
        {
            return;
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
    }
    //EventTrigger > PointerExit�ŌĂ�
    //�J�[�\�����O���ƕ\����߂�
    void CardRevertDisplay()
    {
        if (codeHandManager.draggingBool)
        {
            return;
        }
        transform.SetSiblingIndex(handNum);
        transform.localScale = cardLocalScale;
        codeHandManager.HandLayoutCalclate();
    }

    // �h���b�O���̏���
    public void OnDrag(PointerEventData eventData)
    {
        // eventData.position����A�e�ɏ]��localPosition�ւ̕ϊ����s��
        // �I�u�W�F�N�g�̈ʒu��localPosition�ɕύX����
        Vector2 localPosition = GetLocalPosition(eventData.position);
        codeRectTransform.anchoredPosition = localPosition;

        //�v���C�]�[���ɂ��鎞�̓t���[���̐F��ς���
        //if (codeHandManager.InPlayArea(localPosition.y))
        //{

        //}
        //��D����ւ�
        handNum = codeHandManager.HandSwapCheck(localPosition.x, handNum);
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
        codeHandManager.draggingBool = false;
        CardRevertDisplay();       
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        codeHandManager.draggingBool = true;
    }
}
