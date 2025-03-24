using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    CardGameManager codeCardGameManager;
    [SerializeField] GameObject CardLayout;
    CardLayoutGroup codeCardLayoutGroup;
    [SerializeField] GameObject HandArea;
    RectTransform codeHandAreaRectTransform;

    //手札
    List<Card.CardInfo> handList = new List<Card.CardInfo>();
    //手札オブジェクト
    public List<GameObject> handCardObjectList = new List<GameObject>();
    /// <summary>
    /// 手札枚数毎のカードそれぞれのpositionとrotationのキャッシュ
    /// 手札枚数、左から〇番目のカード、positionとrotation
    /// </summary>
    Dictionary<int, Dictionary<int, pos_rot>> handNumLocalPosRotCache = new Dictionary<int, Dictionary<int, pos_rot>>();

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

    private void Awake()
    {
        codeCardGameManager = GetComponent<CardGameManager>();
        codeCardLayoutGroup = CardLayout.GetComponent<CardLayoutGroup>();
        codeHandAreaRectTransform = HandArea.GetComponent<RectTransform>();
    }

    public List<Card.CardInfo> GetHandList()
    {
        return handList;
    }

    public void AddCardToHand(Card.CardInfo card, GameObject cardObject)
    {
        handList.Add(card);
        handCardObjectList.Add(cardObject);
        cardObject.transform.name += $"_{handCardObjectList.Count}";

        cardObject.transform.parent = CardLayout.transform;
        cardObject.GetComponent<Card>().Initialization();
        //手札表示を整える
        HandLayoutCalclate();
        if (!handNumLocalPosRotCache.ContainsKey(handCardObjectList.Count))
        {
            Dictionary<int, pos_rot> dict = new Dictionary<int, pos_rot>();
            for(int count = 0; count < handCardObjectList.Count; count++)
            {
                dict.Add(count, new pos_rot(handCardObjectList[count].transform.localPosition, handCardObjectList[count].transform.localRotation));
            }
            handNumLocalPosRotCache.Add(handCardObjectList.Count, dict);
        }
    }


    //手札の表示を更新
    public void HandLayoutCalclate()
    {
        int handNum = handList.Count;
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
            float leftCardPositionX = handNumLocalPosRotCache[handCardObjectList.Count][cardNum - 1].position.x;
            if (vectorX < leftCardPositionX)
            {
                //左にあったカードを右へ移動
                var leftCard = handCardObjectList[cardNum - 1];
                leftCard.transform.localPosition = handNumLocalPosRotCache[handCardObjectList.Count][cardNum].position;
                leftCard.transform.localRotation = handNumLocalPosRotCache[handCardObjectList.Count][cardNum].rotation;
                //list内も並び替え
                handCardObjectList.Replace(cardNum, cardNum - 1);
                handList.Replace(cardNum, cardNum - 1);

                //HandNumを更新
                return cardNum - 1;
            }
        }
        //右隣チェック
        if(cardNum < handCardObjectList.Count - 1)
        {
            float rightCardPositionX = handNumLocalPosRotCache[handCardObjectList.Count][cardNum + 1].position.x;
            if (vectorX > rightCardPositionX)
            {
                //右にあったカードを左へ移動
                var rightCard = handCardObjectList[cardNum + 1];
                rightCard.transform.localPosition = handNumLocalPosRotCache[handCardObjectList.Count][cardNum].position;
                rightCard.transform.localRotation = handNumLocalPosRotCache[handCardObjectList.Count][cardNum].rotation;
                //list内も並び替え
                handCardObjectList.Replace(cardNum, cardNum + 1);
                handList.Replace(cardNum, cardNum + 1);

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
        Card.CardInfo card = handList[cardNum];
        handList.RemoveAt(cardNum);

        var cardObject = handCardObjectList[cardNum];
        cardObject.transform.parent = HandArea.transform;
        handCardObjectList.RemoveAt(cardNum);

        //演出
        Destroy(cardObject);

        codeCardGameManager.CardPlay(card);
        HandLayoutCalclate();
    }
}

public static class ListUtil
{
    /// <summary>
    /// Replace Index1 and index2
    /// </summary>
    public static void Replace<T>(this IList<T> self, int index1, int index2)
    {
        if (self.Count <= index1 || self.Count <= index2 || index1 == index2)
        {
            return;
        }

        var cache = self[index1];
        self[index1] = self[index2];
        self[index2] = cache;
    }
}

