using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardInfo
{
    //ID
    public int id { get; private set; }
    //�F
    public int cardType { get; private set; }
    //����
    public int cardEffect { get; private set; }

    public CardInfo(int id, int type, int effect)
    {
        this.id = id;
        cardType = type;
        cardEffect = effect;
    }
}
public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    //�J�[�h���
    CardInfo codeCardInfo;

    //�傫��
    Vector3 cardLocalScale;

    HandManager codeHandManager;
    RectTransform codeRectTransform;
    RectTransform codeCardLayoutRectTransform;
    [SerializeField] Image codeImage;

    //��D�̉��Ԗڂɂ��邩
    int handNum = 0;

    bool playedBool = false;

    public void Initialization(CardInfo card)
    {
        cardLocalScale = transform.localScale;
        codeCardInfo = card;
        codeHandManager = GameObject.FindGameObjectWithTag("CardGameManager").GetComponent<HandManager>();
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
        codeHandManager.draggingBool = false;
        if (playedBool)
        {
            return;
        }
        else if (codeHandManager.IsSelecting())
        {
            CardRevertDisplay();
            return;
        }
        // eventData.position����A�e�ɏ]��localPosition�ւ̕ϊ����s��
        // �I�u�W�F�N�g�̈ʒu��localPosition�ɕύX����
        Vector2 localPosition = GetLocalPosition(eventData.position);

        //�v���C�]�[���ɏo������
        if (codeHandManager.InPlayArea(localPosition.y))
        {
            playedBool = true;
            codeHandManager.CardPlay(codeCardInfo.id);
            return;
        }

        CardRevertDisplay();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        codeHandManager.draggingBool = true;
    }

    //EventTrigger > PointerClick�ŌĂ�
    public void Selecterd()
    {
        if (codeHandManager.IsSelecting() && !codeHandManager.draggingBool)
        {
            //�t�F�[�h������
            if (codeHandManager.SelectedHandObject(codeCardInfo.id))
            {
                codeImage.DOFade(0.7f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }
            //�t�F�[�h��߂�
            else
            {
                codeImage.DOKill();
                var color = codeImage.color;
                codeImage.color = new Color(color.r, color.g, color.b, 1);
            }
        }
    }
}
