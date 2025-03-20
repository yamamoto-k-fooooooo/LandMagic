using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
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

    CardGameManager codeCardGameManager;
    //手札の何番目にあるか
    int handNum = 0;

    private void Start()
    {
        codeCardGameManager = GameObject.Find("CardGameManager").GetComponent<CardGameManager>();
    }

    //カーソルが合うと最前面、大きく表示する
    public void CardBigDisplay()
    {
        handNum = gameObject.transform.GetSiblingIndex();

        gameObject.transform.SetAsLastSibling();
        gameObject.transform.localPosition = new Vector3(
            gameObject.transform.localPosition.x, 
            gameObject.transform.localPosition.y + 50, 
            gameObject.transform.localPosition.z
           );
        gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1);
    }
    //カーソルが戻ると表示を戻す
    public void CardRevertDisplay()
    {
        gameObject.transform.SetSiblingIndex(handNum);
        gameObject.transform.localScale = Vector3.one;
        codeCardGameManager.HandLayoutCalclate();
    }
}
