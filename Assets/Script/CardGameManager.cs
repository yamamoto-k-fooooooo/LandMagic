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

    //�f�b�L
    List<Card> deckList = new List<Card>();
    //�f�b�L����
    const int deckNum = 25;

    //��D
    List<Card> handList = new List<Card>();

    // Start is called before the first frame update
    void Start()
    {
        //�f�b�L�𐶐�
        CreateDeck();
        //4������D�ɂ���
        shuffleAndDraw(4);

        foreach (Card card in handList)
        {
            Debug.Log($"��D : {card.cardType}");
        }
        foreach (Card card in deckList)
        {
            Debug.Log($"�f�b�L : {card.cardType}");
        }

        //�}���K��
        //UI��\������y/n�Ŋm�F����
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

    //�V���b�t�����Ĉ��� or ��������
    void shuffleAndDraw(int drawNum, bool handBack = false)
    {
        //��D��S�ăf�b�L�ɖ߂�
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
