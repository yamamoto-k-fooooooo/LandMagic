using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public class CardInfo
    {
        //色
        public int cardType { get; private set; }
        //効果
        public int cardEffect { get; private set; }

        public CardInfo(int type, int effect)
        {
            cardType = type;
            cardEffect = effect;
        }
    }
    //大きさ
    Vector3 cardLocalScale;

    HandManager codeHandManager;
    RectTransform codeRectTransform;
    RectTransform codeCardLayoutRectTransform;

    //手札の何番目にあるか
    int handNum = 0;

    public void Initialization()
    {
        cardLocalScale = transform.localScale;
        codeHandManager = GameObject.Find("CardGameManager").GetComponent<HandManager>();
        codeRectTransform = GetComponent<RectTransform>();
        codeCardLayoutRectTransform = codeRectTransform.parent as RectTransform;
    }

    //EventTrigger > PointerEnterで呼ぶ
    //カーソルが合うと最前面、大きく表示する
    void CardBigDisplay()
    {
        if (codeHandManager.draggingBool)
        {
            return;
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
    }
    //EventTrigger > PointerExitで呼ぶ
    //カーソルが外れると表示を戻す
    void CardRevertDisplay()
    {
        if (codeHandManager.draggingBool)
        {
            return;
        }
        transform.SetSiblingIndex(handNum);
        transform.localScale = cardLocalScale;
        codeHandManager.HandLayoutCalclate();
    }

    // ドラッグ中の処理
    public void OnDrag(PointerEventData eventData)
    {
        // eventData.positionから、親に従うlocalPositionへの変換を行う
        // オブジェクトの位置をlocalPositionに変更する
        Vector2 localPosition = GetLocalPosition(eventData.position);
        codeRectTransform.anchoredPosition = localPosition;

        //プレイゾーンにある時はフレームの色を変える
        //if (codeHandManager.InPlayArea(localPosition.y))
        //{

        //}
        //手札入れ替え
        handNum = codeHandManager.HandSwapCheck(localPosition.x, handNum);
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
        CardRevertDisplay();       
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        codeHandManager.draggingBool = true;
    }
}
