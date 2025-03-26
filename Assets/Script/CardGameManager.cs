using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class CardGameManager : MonoBehaviour
{
    HandManager codeHandManager;

    public enum CardType
    {
        white,
        blue,
        black,
        red,
        green
    }

    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject[] ColorArea;
    [SerializeField] GameObject StageCardObject;

    //デッキ
    List<CardInfo> deckList = new List<CardInfo>();
    //デッキ枚数
    const int deckNum = 25;

    /// <summary>
    /// 自分の場に出ているカードの数
    /// CardType, 数
    /// </summary>
    Dictionary<int, int> MyFieldCardNum = new Dictionary<int, int>()
    {
        { 0,0 },
        { 1,0 },
        { 2,0 },
        { 3,0 },
        { 4,0 }
    };

    private void Awake()
    {
        codeHandManager = GetComponent<HandManager>();
    }
    void Start()
    {
        //デッキを生成
        CreateDeck();
        //4枚を手札にする
        shuffleAndDraw(4);

        //マリガン
        //UIを表示してy/nで確認する
        //shuffleAndDraw(4, true);
    }

    private void Update()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.D))
        {
            Draw(1);
        }
#endif
    }

    void CreateDeck()
    {
        for(int count = 0; count < deckNum; count++)
        {

            int cardType = count % 5;
            CardInfo card = new CardInfo(count, cardType, 0);
            deckList.Add(card);
        }
    }
    void DeckShuffle()
    {
        deckList = deckList.OrderBy(a => Guid.NewGuid()).ToList();
    }

    void Draw(int drawNum)
    {
        if(drawNum <= 0)
        {
            return;
        }

        CardInfo card = null;
        for (int count = 0; count < drawNum; count++)
        {
            card = deckList[count];

            //引いたカード情報をオブジェクトにする
            GameObject cardObject = CardInstantiate(card);

            codeHandManager.AddCardToHand(card, cardObject);
        }

        deckList.RemoveRange(0, drawNum);
    }

    //シャッフルして引く or 引き直す
    void shuffleAndDraw(int drawNum, bool handBack = false)
    {
        //引き直す場合
        if (handBack)
        {
            var handDict = codeHandManager.GetHandDict();
            for(int count = 0; count < handDict.Count; count++)
            {
                CardInfo card = handDict[count];
                deckList.Add(card);
            }
            handDict.Clear();

            //手札をデッキに戻す演出

        }
        DeckShuffle();
        Draw(drawNum);
    }

    //カード情報を受け取り、オブジェクトとして返す
    public GameObject CardInstantiate(CardInfo cardInfo)
    {
        GameObject makingCard = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

        Image image = makingCard.GetComponent<Image>();
        string name = "";
        //色を変更
        switch (cardInfo.cardType)
        {
            case (int)CardType.white:
                image.color = new Color(1, 1, 150/255f, 1);
                name = "_white";
                break;
            case (int)CardType.blue:
                image.color = new Color(100/255f, 1, 1, 1);
                name = "_blue";
                break;
            case (int)CardType.black:
                image.color = new Color(135/255f, 135/255f, 135/255f, 1);
                name = "_black";
                break;
            case (int)CardType.red:
                image.color = new Color(1, 100/255f, 100/255f, 1);
                name = "_red";
                break;
            case (int)CardType.green:
                image.color = new Color(100/255f, 1, 100/255f, 1);
                name = "_green";
                break;
        }
        makingCard.name += name;

        return makingCard;
    }

    public void CardPlay(CardInfo card)
    {
        //色を取得
        int color = card.cardType;
        //出現させるpositionを取得
        Vector3 position = Vector3.zero;

        GameObject parentObject = ColorArea[color];

        //出現オブジェクトを生成
        var stageCardObject = Instantiate(StageCardObject, Vector3.one, Quaternion.identity, parentObject.transform);
        stageCardObject.transform.localPosition = Vector3.zero;
        //大きさ調整
        var localScale = stageCardObject.transform.localScale;
        var parentLossyScale = stageCardObject.transform.parent.lossyScale;
        stageCardObject.transform.localScale
            = new Vector3(
                localScale.x / parentLossyScale.x,
                localScale.y / parentLossyScale.y,
                localScale.z / parentLossyScale.z);


        //出現演出するならこの箇所？


        //場のカードとして加算
        MyFieldCardNum[color]++;
        //勝利条件を満たしているか
        int winCount = 0;
        for(int colorType = 0;colorType < MyFieldCardNum.Count; colorType++)
        {
            if(MyFieldCardNum[colorType] > 0)
            {
                winCount++;
            }
            else
            {
                break;
            }
        }
        if(winCount >= 5)
        {
            //勝利
            Debug.Log("勝利");
            return;
        }
    }
}