using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardInfo
{
    //ID
    public int id { get; private set; }
    //色
    public int cardType { get; private set; }
    //効果
    public int cardEffect { get; private set; }

    public CardInfo(int id, int type, int effect)
    {
        this.id = id;
        cardType = type;
        cardEffect = effect;
    }
}
public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    //カード情報
    CardInfo codeCardInfo;

    //大きさ
    Vector3 cardLocalScale;

    HandManager codeHandManager;
    RectTransform codeRectTransform;
    RectTransform codeCardLayoutRectTransform;
    [SerializeField] Image codeImage;

    //手札の何番目にあるか
    int handNum = 0;

    bool playedBool = false;

    public void Initialization(CardInfo card)
    {
        cardLocalScale = transform.localScale;
        codeCardInfo = card;
        codeHandManager = GameObject.FindGameObjectWithTag("CardGameManager").GetComponent<HandManager>();
        codeRectTransform = GetComponent<RectTransform>();
        codeCardLayoutRectTransform = codeRectTransform.parent as RectTransform;
    }

    //EventTrigger > PointerEnterで呼ぶ
    //カーソルが合うと最前面、大きく表示する
    void CardBigDisplay()
    {
        if (playedBool || transform.localScale != cardLocalScale)
        {
            return;
        }
        //既に他のカードが拡大表示されている
        if(codeHandManager.bigDisplayHandObject != null && codeHandManager.bigDisplayHandObject != gameObject)
        {
            if (codeHandManager.draggingBool)
            {
                return;
            }
        }

        handNum = gameObject.transform.GetSiblingIndex();
        //最前列へ
        transform.SetAsLastSibling();
        //少し上に動かして見やすくする
        transform.localPosition = new Vector3(
            transform.localPosition.x, 
            transform.localPosition.y + 80, 
            transform.localPosition.z
           );
        //表示をまっすぐに
        transform.localRotation = Quaternion.identity;
        //拡大
        transform.localScale = new Vector3(
            cardLocalScale.x * 1.5f,
            cardLocalScale.y * 1.5f,
            cardLocalScale.z
           );

        codeHandManager.bigDisplayHandObject = gameObject;
    }
    //EventTrigger > PointerExitで呼ぶ
    //カーソルが外れると表示を戻す
    public void CardRevertDisplay()
    {
        if (codeHandManager.draggingBool || playedBool)
        {
            return;
        }
        RevertDisplay();
        codeHandManager.HandLayoutCalclate();
    }
    public void RevertDisplay()
    {
        transform.SetSiblingIndex(handNum);
        transform.localScale = cardLocalScale;
        codeHandManager.bigDisplayHandObject = null;
    }

    // ドラッグ中の処理
    public void OnDrag(PointerEventData eventData)
    {
        if (playedBool)
        {
            return;
        }

        //拡大表示
        if (codeHandManager.bigDisplayHandObject != null)
        {
            //他に拡大表示している手札がある場合は戻す
            if (codeHandManager.bigDisplayHandObject != gameObject)
            {
                codeHandManager.HandObjectRevertDisplay();
                CardBigDisplay();
            }
        }
        else
        {
            CardBigDisplay();
        }

        // eventData.positionから、親に従うlocalPositionへの変換を行う
        // オブジェクトの位置をlocalPositionに変更する
        Vector2 localPosition = GetLocalPosition(eventData.position);
        codeRectTransform.anchoredPosition = localPosition;

        //プレイゾーンにあるか
        if (codeHandManager.InPlayArea(localPosition.y))
        {

        }
        else
        {
            //手札並び替えチェック
            handNum = codeHandManager.HandSwapCheck(localPosition.x, handNum);
        }
    }
    // ScreenPositionからlocalPositionへの変換関数
    private Vector2 GetLocalPosition(Vector2 screenPosition)
    {
        Vector2 result = Vector2.zero;

        // screenPositionを親の座標系(parentRectTransform)に対応するよう変換する.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(codeCardLayoutRectTransform, screenPosition, Camera.main, out result);

        return result;
    }

    // ドラッグ終了時の処理
    public void OnEndDrag(PointerEventData eventData)
    {
        codeHandManager.draggingBool = false;
        if (playedBool)
        {
            return;
        }
        else if (codeHandManager.IsSelecting())
        {
            CardRevertDisplay();
            return;
        }
        // eventData.positionから、親に従うlocalPositionへの変換を行う
        // オブジェクトの位置をlocalPositionに変更する
        Vector2 localPosition = GetLocalPosition(eventData.position);

        //プレイゾーンに出したか
        if (codeHandManager.InPlayArea(localPosition.y))
        {
            playedBool = true;
            codeHandManager.CardPlay(codeCardInfo.id);
            return;
        }

        CardRevertDisplay();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        codeHandManager.draggingBool = true;
    }

    //EventTrigger > PointerClickで呼ぶ
    public void Selecterd()
    {
        if (codeHandManager.IsSelecting() && !codeHandManager.draggingBool)
        {
            //フェードさせる
            if (codeHandManager.SelectedHandObject(codeCardInfo.id))
            {
                codeImage.DOFade(0.7f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }
            //フェードを戻す
            else
            {
                codeImage.DOKill();
                var color = codeImage.color;
                codeImage.color = new Color(color.r, color.g, color.b, 1);
            }
        }
    }
}
