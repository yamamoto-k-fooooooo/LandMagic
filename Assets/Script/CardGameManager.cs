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

    //�f�b�L
    List<Card.CardInfo> deckList = new List<Card.CardInfo>();
    //�f�b�L����
    const int deckNum = 25;

    private void Awake()
    {
        codeHandManager = GetComponent<HandManager>();
    }
    void Start()
    {
        //�f�b�L�𐶐�
        CreateDeck();
        //4������D�ɂ���
        shuffleAndDraw(4);

        //�}���K��
        //UI��\������y/n�Ŋm�F����
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

            //�������J�[�h�����I�u�W�F�N�g�ɂ���
            var cardObject = CardInstantiate(card);

            codeHandManager.AddCardToHand(card, cardObject);
        }

        for (int count = 0; count < drawNum; count++)
        {
            deckList.RemoveAt(count);
        }
    }

    //�V���b�t�����Ĉ��� or ��������
    void shuffleAndDraw(int drawNum, bool handBack = false)
    {
        //���������ꍇ
        if (handBack)
        {
            var handList = codeHandManager.GetHandList();
            for(int count = 0; count < handList.Count; count++)
            {
                Card.CardInfo card = handList[count];
                deckList.Add(card);
            }
            handList.Clear();

            //��D���f�b�L�ɖ߂����o

        }
        DeckShuffle();
        Draw(drawNum);
    }

    //�J�[�h�����󂯎��A�I�u�W�F�N�g�Ƃ��ĕԂ�
    public GameObject CardInstantiate(Card.CardInfo cardInfo)
    {
        GameObject makingCard = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

        Image image = makingCard.GetComponent<Image>();

        //�F��ύX
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
}
