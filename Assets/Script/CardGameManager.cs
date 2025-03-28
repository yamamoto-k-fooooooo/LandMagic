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

    //手札のカード
    [SerializeField] GameObject handCardObject;
    [SerializeField] Sprite[] uiCardMaterials;
    //カラー毎のフィールド
    [SerializeField] GameObject[] ColorArea;
    //ステージに出すカード
    [SerializeField] GameObject stageCardObject;
    [SerializeField] Material[] stageCardMaterial;
    //カード一覧表
    [SerializeField] GameObject CardListDisplay;
    [SerializeField] TextMeshProUGUI[] codeCardListText;

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
    public bool selectingBool = false;
    public HashSet<int> selectedCardIdHash = new HashSet<int>();

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
            GameObject handObject = HandObjectInstantiate(card);

            codeHandManager.AddCardToHand(card, handObject);
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
    public GameObject HandObjectInstantiate(CardInfo cardInfo)
    {
        GameObject makingCard = Instantiate(handCardObject, Vector3.zero, Quaternion.identity);

        Image image = makingCard.GetComponent<Image>();
        image.sprite = uiCardMaterials[cardInfo.cardType];

#if DEBUG && UNITY_EDITOR
        string name = "";
        //色を変更
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
    //カード情報を受け取り、オブジェクトとして返す
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
        //マテリアル(イラスト)を変更
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
        //色を取得
        int color = card.cardType;

        GameObject parentObject = ColorArea[color];
        //出現オブジェクトを生成
        //var stageCardObject = Instantiate(this.stageCardObject, Vector3.one, Quaternion.identity, parentObject.transform);
        var stageCardObject = StageObjectInstantiate(card, parentObject);

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
        //場に出た時の効果
        DoCardEffect(color);
    }

    //ETB効果の実行
    void DoCardEffect(int color)
    {
        switch (color)
        {
            //サルベージ
            case (int)CardType.white:
                break;
            //ハンデス
            case (int)CardType.black:
                DiscardHand(1);
                break;
            //除去
            case (int)CardType.red:
                break;
            //ドロー
            case (int)CardType.green:
                Draw(1);
                break;
        }
    }


    public void OpenCardListDisplay(Dictionary<int, List<CardInfo>> type_cardInfoDict)
    {
        CardListDisplay.SetActive(true);
        CardListDisplay.transform.localScale = Vector3.zero;

        //カード毎の枚数を更新
        //foreachでcardType毎に枚数を数える
        foreach(int key in type_cardInfoDict.Keys)
        {
            codeCardListText[key].text = $"{type_cardInfoDict[key].Count}";
        }

        //アニメーション
        CardListDisplay.transform.DOScale(Vector3.one, 0.1f);
    }

    /// <summary>
    /// 指定枚数分を選択されるまで待機
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public IEnumerator SelectCheck(int num)
    {
        codeSelectCheckButton.SelectNumSet(num);

        selectingBool = true;
        while (selectingBool)
        {
            //〇枚選ぶ
            yield return null;
        }
    }
    //SelectCheckButtonから呼ばれる
    public void SelectComplete()
    {
        selectingBool = false;
        SelectCheckButton.SetActive(false);
    }
    public void SelectedHashClear()
    {
        selectedCardIdHash.Clear();
    }

    //手札を〇枚捨てる
    void DiscardHand(int num)
    {
        StartCoroutine(codeHandManager.DiscardHand(num));
    }

    //捨て札にカードを送る
    public void GotoTrash(CardInfo card)
    {
        var cardObject = StageObjectInstantiate(card);
        codeTrashManager.AddCardToTrash(card, cardObject);
    }
}