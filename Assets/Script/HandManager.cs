using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] GameObject CardLayout;
    CardLayoutGroup codeCardLayoutGroup;
    [SerializeField] GameObject HandArea;
    RectTransform codeHandAreaRectTransform;

    //手札
    List<Card.CardInfo> handList = new List<Card.CardInfo>();
    //手札オブジェクト
    public List<GameObject> handGameObjectList = new List<GameObject>();
    /// <summary>
    /// 手札枚数毎のカードそれぞれのpositionとrotationのキャッシュ
    /// 手札枚数、左から〇番目のカード、positionとrotation
    /// </summary>
    Dictionary<int, Dictionary<int, pos_rot>> handNumLocalPosRotCache = new Dictionary<int, Dictionary<int, pos_rot>>();

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
        handGameObjectList.Add(cardObject);
        cardObject.transform.name += $"_{handGameObjectList.Count}";

        cardObject.transform.parent = CardLayout.transform;
        cardObject.GetComponent<Card>().Initialization();
        //手札表示を整える
        HandLayoutCalclate();
        if (!handNumLocalPosRotCache.ContainsKey(handGameObjectList.Count))
        {
            Dictionary<int, pos_rot> dict = new Dictionary<int, pos_rot>();
            for(int count = 0; count < handGameObjectList.Count; count++)
            {
                dict.Add(count, new pos_rot(handGameObjectList[count].transform.localPosition, handGameObjectList[count].transform.localRotation));
            }
            handNumLocalPosRotCache.Add(handGameObjectList.Count, dict);
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

    //public bool InPlayArea(float vectorY)
    //{
    //    if (vectorY > codeHandAreaRectTransform.anchoredPosition.y - codeHandAreaRectTransform.sizeDelta.y)
    //    {
    //        //使用できる状況か
    //        if (true)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}


    //手札の順番入れ替えチェック
    public int HandSwapCheck(float vectorX, int cardNum)
    {
        //左隣チェック
        if(0 < cardNum)
        {
            float leftCardPositionX = handNumLocalPosRotCache[handGameObjectList.Count][cardNum - 1].position.x;
            if (vectorX < leftCardPositionX)
            {
                //左にあったカードを右へ移動
                var leftCard = handGameObjectList[cardNum - 1];
                leftCard.transform.localPosition = handNumLocalPosRotCache[handGameObjectList.Count][cardNum].position;
                leftCard.transform.localRotation = handNumLocalPosRotCache[handGameObjectList.Count][cardNum].rotation;
                //list内も並び替え
                handGameObjectList.Replace(cardNum, cardNum - 1);

                //HandNumを更新
                return cardNum - 1;
            }
        }
        //右隣チェック
        if(cardNum < handGameObjectList.Count - 1)
        {
            float rightCardPositionX = handNumLocalPosRotCache[handGameObjectList.Count][cardNum + 1].position.x;
            if (vectorX > rightCardPositionX)
            {
                //右にあったカードを左へ移動
                var rightCard = handGameObjectList[cardNum + 1];
                rightCard.transform.localPosition = handNumLocalPosRotCache[handGameObjectList.Count][cardNum].position;
                rightCard.transform.localRotation = handNumLocalPosRotCache[handGameObjectList.Count][cardNum].rotation;
                //list内も並び替え
                handGameObjectList.Replace(cardNum, cardNum + 1);

                //HandNumを更新
                return cardNum + 1;
            }
        }

        return cardNum;
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
            return;

        var cache = self[index1];
        self[index1] = self[index2];
        self[index2] = cache;
    }
}

