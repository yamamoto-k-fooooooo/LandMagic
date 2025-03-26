using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] CardGameManager codeCardGameManager;
    [SerializeField] GameObject CardLayout;
    [SerializeField] CardLayoutGroup codeCardLayoutGroup;
    [SerializeField] RectTransform codeHandAreaRectTransform;

    //��D
    Dictionary<int, CardInfo> handDict = new Dictionary<int, CardInfo>();

    //��D�I�u�W�F�N�g
    Dictionary<int, GameObject> handCardObjectDict = new Dictionary<int, GameObject>();
    /// <summary>
    /// ��D�������̃J�[�h���ꂼ���position��rotation�̃L���b�V��
    /// ��D�����A������Z�Ԗڂ̃J�[�h�Aposition��rotation
    /// </summary>
    Dictionary<int, Dictionary<int, pos_rot>> handNumLocalPosRotCache = new Dictionary<int, Dictionary<int, pos_rot>>();

    [HideInInspector]
    public GameObject bigDisplayHandObject = null;

    class pos_rot
    {
        public Vector2 position;
        public Quaternion rotation;

        public pos_rot(Vector2 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    public bool draggingBool = false;


    public Dictionary<int, CardInfo> GetHandDict()
    {
        return handDict;
    }

    public void AddCardToHand(CardInfo card, GameObject cardObject)
    {
        handDict.Add(card.id, card);
        handCardObjectDict.Add(card.id, cardObject);
        cardObject.transform.name += $"_{handCardObjectDict.Count}";

        cardObject.transform.parent = CardLayout.transform;
        cardObject.GetComponent<Card>().Initialization(card);
        //��D�\���𐮂���
        HandLayoutCalclate();
        if (!handNumLocalPosRotCache.ContainsKey(handCardObjectDict.Count))
        {
            List<GameObject> handObjectList = codeCardLayoutGroup.GetChildrenWithoutGrandchildren();
            Dictionary<int, pos_rot> dict = new Dictionary<int, pos_rot>();
            for(int count = 0; count < handObjectList.Count; count++)
            {
                dict.Add(count, new pos_rot(handObjectList[count].transform.localPosition, handObjectList[count].transform.localRotation));
            }          

            handNumLocalPosRotCache.Add(handCardObjectDict.Count, dict);
        }
    }


    //��D�̕\�����X�V
    public void HandLayoutCalclate()
    {
        int handNum = handDict.Count;
        //��D��6���ȉ��̏ꍇ�͐�`�ɍL����悤�ɂ���
        if (handNum <= 6)
        {
            codeCardLayoutGroup.startAngle = 90 + 10 * (handNum - 1);
            codeCardLayoutGroup.endAngle = 90 - 10 * (handNum - 1);
        }
        codeCardLayoutGroup.Calclate();
    }

    public bool InPlayArea(float vectorY)
    {
        if (vectorY > codeHandAreaRectTransform.anchoredPosition.y - codeHandAreaRectTransform.sizeDelta.y)
        {
            //�g�p�ł���󋵂�
            if (true)
            {
                return true;
            }
        }
        return false;
    }


    //��D�̏��ԓ���ւ��`�F�b�N
    public int HandSwapCheck(float vectorX, int cardNum)
    {
        //���׃`�F�b�N
        if(0 < cardNum)
        {
            float leftCardPositionX = handNumLocalPosRotCache[handCardObjectDict.Count][cardNum - 1].position.x;
            if (vectorX < leftCardPositionX)
            {
                //���ɂ������J�[�h���E�ֈړ�
                Transform leftCard = CardLayout.transform.GetChild(cardNum - 1);
                leftCard.localPosition = handNumLocalPosRotCache[handCardObjectDict.Count][cardNum].position;
                leftCard.localRotation = handNumLocalPosRotCache[handCardObjectDict.Count][cardNum].rotation;

                //HandNum���X�V
                return cardNum - 1;
            }
        }
        //�E�׃`�F�b�N
        if(cardNum < handCardObjectDict.Count - 1)
        {
            float rightCardPositionX = handNumLocalPosRotCache[handCardObjectDict.Count][cardNum + 1].position.x;
            if (vectorX > rightCardPositionX)
            {
                //�E�ɂ������J�[�h�����ֈړ�
                Transform rightCard = CardLayout.transform.GetChild(cardNum);
                rightCard.localPosition = handNumLocalPosRotCache[handCardObjectDict.Count][cardNum].position;
                rightCard.localRotation = handNumLocalPosRotCache[handCardObjectDict.Count][cardNum].rotation;

                //HandNum���X�V
                return cardNum + 1;
            }
        }
        return cardNum;
    }

    public void HandObjectRevertDisplay()
    {
        if(bigDisplayHandObject == null)
        {
            return;
        }
        bigDisplayHandObject.GetComponent<Card>().RevertDisplay();
    }

    public void CardPlay(int cardNum)
    {
        CardInfo card = handDict[cardNum];
        handDict.Remove(card.id);

        GameObject cardObject = handCardObjectDict[cardNum];
        cardObject.transform.parent = CardLayout.transform.parent.transform;
        handCardObjectDict.Remove(cardNum);

        //���o
        Destroy(cardObject);

        codeCardGameManager.CardPlay(card);
        HandLayoutCalclate();
    }

    public bool IsSelecting()
    {
        return codeCardGameManager.selectingBool;
    }
    public bool SelectedHandObject(int cardId)
    {
        if (codeCardGameManager.selectedCardIdHash.Contains(cardId))
        {
            codeCardGameManager.selectedCardIdHash.Remove(cardId);
            return false;
        }
        else
        {
            codeCardGameManager.selectedCardIdHash.Add(cardId);
            return true;
        }
    }

    public IEnumerator DiscardHand(int num)
    {
        //�w�薇������I��
        yield return codeCardGameManager.SelectCheck(num);
        HashSet<int> CardIdHash = codeCardGameManager.selectedCardIdHash;
        //�̂ĎD��
        foreach (int selectedHandId in CardIdHash)
        {
            CardInfo card = handDict[selectedHandId];
            handDict.Remove(selectedHandId);
            codeCardGameManager.GotoTrash(card);

            GameObject cardObject = handCardObjectDict[selectedHandId];
            cardObject.transform.parent = CardLayout.transform.parent.transform;
            handCardObjectDict.Remove(selectedHandId);

            //���o
            Destroy(cardObject);
        }
        codeCardGameManager.SelectedHashClear();
        HandLayoutCalclate();
    }
}

