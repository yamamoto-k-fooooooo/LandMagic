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

    //�f�b�L
    List<CardInfo> deckList = new List<CardInfo>();
    //�f�b�L����
    const int deckNum = 25;

    /// <summary>
    /// �����̏�ɏo�Ă���J�[�h�̐�
    /// CardType, ��
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

            //�������J�[�h�����I�u�W�F�N�g�ɂ���
            GameObject cardObject = CardInstantiate(card);

            codeHandManager.AddCardToHand(card, cardObject);
        }

        deckList.RemoveRange(0, drawNum);
    }

    //�V���b�t�����Ĉ��� or ��������
    void shuffleAndDraw(int drawNum, bool handBack = false)
    {
        //���������ꍇ
        if (handBack)
        {
            var handDict = codeHandManager.GetHandDict();
            for(int count = 0; count < handDict.Count; count++)
            {
                CardInfo card = handDict[count];
                deckList.Add(card);
            }
            handDict.Clear();

            //��D���f�b�L�ɖ߂����o

        }
        DeckShuffle();
        Draw(drawNum);
    }

    //�J�[�h�����󂯎��A�I�u�W�F�N�g�Ƃ��ĕԂ�
    public GameObject CardInstantiate(CardInfo cardInfo)
    {
        GameObject makingCard = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

        Image image = makingCard.GetComponent<Image>();
        string name = "";
        //�F��ύX
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
        //�F���擾
        int color = card.cardType;
        //�o��������position���擾
        Vector3 position = Vector3.zero;

        GameObject parentObject = ColorArea[color];

        //�o���I�u�W�F�N�g�𐶐�
        var stageCardObject = Instantiate(StageCardObject, Vector3.one, Quaternion.identity, parentObject.transform);
        stageCardObject.transform.localPosition = Vector3.zero;
        //�傫������
        var localScale = stageCardObject.transform.localScale;
        var parentLossyScale = stageCardObject.transform.parent.lossyScale;
        stageCardObject.transform.localScale
            = new Vector3(
                localScale.x / parentLossyScale.x,
                localScale.y / parentLossyScale.y,
                localScale.z / parentLossyScale.z);


        //�o�����o����Ȃ炱�̉ӏ��H


        //��̃J�[�h�Ƃ��ĉ��Z
        MyFieldCardNum[color]++;
        //���������𖞂����Ă��邩
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
            //����
            Debug.Log("����");
            return;
        }
    }
}