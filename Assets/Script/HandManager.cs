using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] CardGameManager codeCardGameManager;
    [SerializeField] GameObject CardLayout;
    [SerializeField] CardLayoutGroup codeCardLayoutGroup;
    [SerializeField] RectTransform codeHandAreaRectTransform;

    //手札
    Dictionary<int, CardInfo> handDict = new Dictionary<int, CardInfo>();

    //手札オブジェクト
    Dictionary<int, GameObject> handCardObjectDict = new Dictionary<int, GameObject>();
    /// <summary>
    /// 手札枚数毎のカードそれぞれのpositionとrotationのキャッシュ
    /// 手札枚数、左から〇番目のカード、positionとrotation
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
        //手札表示を整える
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


    //手札の表示を更新
    public void HandLayoutCalclate()
    {
        int handNum = handDict.Count;
        //手札が6枚以下の場合は扇形に広げるようにする
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
            //使用できる状況か
            if (true)
            {
                return true;
            }
        }
        return false;
    }


    //手札の順番入れ替えチェック
    public int HandSwapCheck(float vectorX, int cardNum)
    {
        //左隣チェック
        if(0 < cardNum)
        {
            float leftCardPositionX = handNumLocalPosRotCache[handCardObjectDict.Count][cardNum - 1].position.x;
            if (vectorX < leftCardPositionX)
            {
                //左にあったカードを右へ移動
                Transform leftCard = CardLayout.transform.GetChild(cardNum - 1);
                leftCard.localPosition = handNumLocalPosRotCache[handCardObjectDict.Count][cardNum].position;
                leftCard.localRotation = handNumLocalPosRotCache[handCardObjectDict.Count][cardNum].rotation;

                //HandNumを更新
                return cardNum - 1;
            }
        }
        //右隣チェック
        if(cardNum < handCardObjectDict.Count - 1)
        {
            float rightCardPositionX = handNumLocalPosRotCache[handCardObjectDict.Count][cardNum + 1].position.x;
            if (vectorX > rightCardPositionX)
            {
                //右にあったカードを左へ移動
                Transform rightCard = CardLayout.transform.GetChild(cardNum);
                rightCard.localPosition = handNumLocalPosRotCache[handCardObjectDict.Count][cardNum].position;
                rightCard.localRotation = handNumLocalPosRotCache[handCardObjectDict.Count][cardNum].rotation;

                //HandNumを更新
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

        //演出
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
        //指定枚数分を選ぶ
        yield return codeCardGameManager.SelectCheck(num);
        HashSet<int> CardIdHash = codeCardGameManager.selectedCardIdHash;
        //捨て札へ
        foreach (int selectedHandId in CardIdHash)
        {
            CardInfo card = handDict[selectedHandId];
            handDict.Remove(selectedHandId);
            codeCardGameManager.GotoTrash(card);

            GameObject cardObject = handCardObjectDict[selectedHandId];
            cardObject.transform.parent = CardLayout.transform.parent.transform;
            handCardObjectDict.Remove(selectedHandId);

            //演出
            Destroy(cardObject);
        }
        codeCardGameManager.SelectedHashClear();
        HandLayoutCalclate();
    }
}

