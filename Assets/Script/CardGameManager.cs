using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CardGameManager : MonoBehaviour
{
    public enum CardType
    {
        white,
        blue,
        black,
        red,
        green
    }

    //デッキ
    List<Card> deckList = new List<Card>();
    //デッキ枚数
    const int deckNum = 25;

    //手札
    List<Card> handList = new List<Card>();

    // Start is called before the first frame update
    void Start()
    {
        //デッキを生成
        CreateDeck();
        //4枚を手札にする
        shuffleAndDraw(4);

        foreach (Card card in handList)
        {
            Debug.Log($"手札 : {card.cardType}");
        }
        foreach (Card card in deckList)
        {
            Debug.Log($"デッキ : {card.cardType}");
        }

        //マリガン
        //UIを表示してy/nで確認する
    }

    void CreateDeck()
    {
        for(int count = 0; count < deckNum; count++)
        {
            int cardType = count % 5;
            Card card = new Card(cardType, 0);
            deckList.Add(card);
        }
    }
    void Shuffle()
    {
        deckList = deckList.OrderBy(a => Guid.NewGuid()).ToList();

        foreach(Card card in deckList)
        {
            Debug.Log($"{card.cardType}");
        }
    }

    void Draw(int drawNum)
    {
        for(int count = 0; count < drawNum; count++)
        {
            Card card = deckList[count];
            handList.Add(card);
        }
        for (int count = 0; count < drawNum; count++)
        {
            deckList.RemoveAt(count);
        }
    }

    //シャッフルして引く or 引き直す
    void shuffleAndDraw(int drawNum, bool handBack = false)
    {
        //手札を全てデッキに戻す
        if (handBack)
        {
            for(int count = 0; count < handList.Count; count++)
            {
                Card card = handList[count];
                deckList.Add(card);
            }
            handList.Clear();
        }
        Shuffle();
        Draw(drawNum);
    }
}
