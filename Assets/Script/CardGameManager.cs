using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using DG.Tweening;
using TMPro;

public class CardGameManager : MonoBehaviour
{
    [SerializeField] HandManager codeHandManager;
    [SerializeField] GameObject SelectCheckButton;
    [SerializeField] SelectCheckButton codeSelectCheckButton;
    [SerializeField] TrashManager codeTrashManager;

    public enum CardType
    {
        white,
        blue,
        black,
        red,
        green
    }

    //��D�̃J�[�h
    [SerializeField] GameObject handCardObject;
    [SerializeField] Sprite[] uiCardMaterials;
    //�J���[���̃t�B�[���h
    [SerializeField] GameObject[] ColorArea;
    //�X�e�[�W�ɏo���J�[�h
    [SerializeField] GameObject stageCardObject;
    [SerializeField] Material[] stageCardMaterial;
    //�J�[�h�ꗗ�\
    [SerializeField] GameObject CardListDisplay;
    [SerializeField] TextMeshProUGUI[] codeCardListText;

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
    public bool selectingBool = false;
    public HashSet<int> selectedCardIdHash = new HashSet<int>();

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
            GameObject handObject = HandObjectInstantiate(card);

            codeHandManager.AddCardToHand(card, handObject);
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
    public GameObject HandObjectInstantiate(CardInfo cardInfo)
    {
        GameObject makingCard = Instantiate(handCardObject, Vector3.zero, Quaternion.identity);

        Image image = makingCard.GetComponent<Image>();
        image.sprite = uiCardMaterials[cardInfo.cardType];

#if DEBUG && UNITY_EDITOR
        string name = "";
        //�F��ύX
        switch (cardInfo.cardType)
        {
            case (int)CardType.white:
                name = "_white";
                break;
            case (int)CardType.blue:
                name = "_blue";
                break;
            case (int)CardType.black:
                name = "_black";
                break;
            case (int)CardType.red:
                name = "_red";
                break;
            case (int)CardType.green:
                name = "_green";
                break;
        }
        makingCard.name += name;
#endif

        return makingCard;
    }
    //�J�[�h�����󂯎��A�I�u�W�F�N�g�Ƃ��ĕԂ�
    public GameObject StageObjectInstantiate(CardInfo cardInfo, GameObject parent = null)
    {
        GameObject makingCard;
        if (parent == null)
        {
            makingCard = Instantiate(stageCardObject, Vector3.zero, Quaternion.identity);
        }
        else
        {
            makingCard = Instantiate(stageCardObject, Vector3.zero, Quaternion.identity, parent.transform);
        }
        //�}�e���A��(�C���X�g)��ύX
        makingCard.GetComponent<MeshRenderer>().material = stageCardMaterial[cardInfo.cardType];

#if DEBUG && UNITY_EDITOR
        string name = "";
        switch (cardInfo.cardType)
        {
            case (int)CardType.white:
                name = "_white";
                break;
            case (int)CardType.blue:
                name = "_blue";
                break;
            case (int)CardType.black:
                name = "_black";
                break;
            case (int)CardType.red:
                name = "_red";
                break;
            case (int)CardType.green:
                name = "_green";
                break;
        }
        makingCard.name += name;
#endif

        return makingCard;
    }

    public void CardPlay(CardInfo card)
    {
        //�F���擾
        int color = card.cardType;

        GameObject parentObject = ColorArea[color];
        //�o���I�u�W�F�N�g�𐶐�
        //var stageCardObject = Instantiate(this.stageCardObject, Vector3.one, Quaternion.identity, parentObject.transform);
        var stageCardObject = StageObjectInstantiate(card, parentObject);

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
        //��ɏo�����̌���
        DoCardEffect(color);
    }

    //ETB���ʂ̎��s
    void DoCardEffect(int color)
    {
        switch (color)
        {
            //�T���x�[�W
            case (int)CardType.white:
                break;
            //�n���f�X
            case (int)CardType.black:
                DiscardHand(1);
                break;
            //����
            case (int)CardType.red:
                break;
            //�h���[
            case (int)CardType.green:
                Draw(1);
                break;
        }
    }


    public void OpenCardListDisplay(Dictionary<int, List<CardInfo>> type_cardInfoDict)
    {
        CardListDisplay.SetActive(true);
        CardListDisplay.transform.localScale = Vector3.zero;

        //�J�[�h���̖������X�V
        //foreach��cardType���ɖ����𐔂���
        foreach(int key in type_cardInfoDict.Keys)
        {
            codeCardListText[key].text = $"{type_cardInfoDict[key].Count}";
        }

        //�A�j���[�V����
        CardListDisplay.transform.DOScale(Vector3.one, 0.1f);
    }

    /// <summary>
    /// �w�薇������I�������܂őҋ@
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public IEnumerator SelectCheck(int num)
    {
        codeSelectCheckButton.SelectNumSet(num);

        selectingBool = true;
        while (selectingBool)
        {
            //�Z���I��
            yield return null;
        }
    }
    //SelectCheckButton����Ă΂��
    public void SelectComplete()
    {
        selectingBool = false;
        SelectCheckButton.SetActive(false);
    }
    public void SelectedHashClear()
    {
        selectedCardIdHash.Clear();
    }

    //��D���Z���̂Ă�
    void DiscardHand(int num)
    {
        StartCoroutine(codeHandManager.DiscardHand(num));
    }

    //�̂ĎD�ɃJ�[�h�𑗂�
    public void GotoTrash(CardInfo card)
    {
        var cardObject = StageObjectInstantiate(card);
        codeTrashManager.AddCardToTrash(card, cardObject);
    }
}