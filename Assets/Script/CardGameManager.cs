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

    //デッキ
    List<Card.CardInfo> deckList = new List<Card.CardInfo>();
    //デッキ枚数
    const int deckNum = 25;

    //手札
    List<Card.CardInfo> handList = new List<Card.CardInfo>();

    void Start()
    {
        codeHandManager = GetComponent<HandManager>();

        //デッキを生成
        CreateDeck();
        //4枚を手札にする
        shuffleAndDraw(4);

        foreach (Card.CardInfo card in handList)
        {
            Debug.Log($"手札 : {card.cardType}");
        }
        foreach (Card.CardInfo card in deckList)
        {
            Debug.Log($"デッキ : {card.cardType}");
        }

        //マリガン
        //UIを表示してy/nで確認する
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
            Card.CardInfo card = new Card.CardInfo(cardType, 0);
            deckList.Add(card);
        }
    }
    void DeckShuffle()
    {
        deckList = deckList.OrderBy(a => Guid.NewGuid()).ToList();

        foreach(Card.CardInfo card in deckList)
        {
            Debug.Log($"{card.cardType}");
        }
    }

    void Draw(int drawNum)
    {
        if(drawNum <= 0)
        {
            return;
        }

        Card.CardInfo card = null;
        for (int count = 0; count < drawNum; count++)
        {
            card = deckList[count];

            //引いたカード情報をオブジェクトにする
            var cardObject = Hoge(card);
            handList.Add(card);

            codeHandManager.AddCardToHand(cardObject, handList.Count);
        }


        for (int count = 0; count < drawNum; count++)
        {
            deckList.RemoveAt(count);
        }
    }

    //シャッフルして引く or 引き直す
    void shuffleAndDraw(int drawNum, bool handBack = false)
    {
        //引き直す場合
        if (handBack)
        {
            for(int count = 0; count < handList.Count; count++)
            {
                Card.CardInfo card = handList[count];
                deckList.Add(card);
            }
            handList.Clear();
        }
        DeckShuffle();
        Draw(drawNum);
    }

    //カード情報を受け取り、オブジェクトとして返す
    public GameObject Hoge(Card.CardInfo cardInfo)
    {
        GameObject makingCard = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

        Image image = makingCard.GetComponent<Image>();

        //色を変更
        switch (cardInfo.cardType)
        {
            case (int)CardType.white:
                image.color = new Color(1, 1, 150/255f, 1);
                break;
            case (int)CardType.blue:
                image.color = new Color(100/255f, 1, 1, 1);
                break;
            case (int)CardType.black:
                image.color = new Color(135/255f, 135/255f, 135/255f, 1);
                break;
            case (int)CardType.red:
                image.color = new Color(1, 100/255f, 100/255f, 1);
                break;
            case (int)CardType.green:
                image.color = new Color(100/255f, 1, 100/255f, 1);
                break;
        }

        return makingCard;
    }

    //手札表示を更新
    public void HandLayoutCalclate()
    {
        codeHandManager.HandLayoutCalclate(handList.Count);
    }
}
