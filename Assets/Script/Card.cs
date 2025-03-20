using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public class CardInfo
    {
        //�F
        public int cardType { get; private set; }
        //����
        public int cardEffect { get; private set; }

        public CardInfo(int type, int effect)
        {
            cardType = type;
            cardEffect = effect;
        }
    }
}
