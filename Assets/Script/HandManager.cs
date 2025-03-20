using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    CardGameManager codeCardGameManager;
    [SerializeField] GameObject CardLayout;
    CardLayoutGroup codeCardLayoutGroup;
    //RectTransform codeHandMovePointRectTransform;



    private void Start()
    {
        codeCardGameManager = GetComponent<CardGameManager>();
        codeCardLayoutGroup = CardLayout.GetComponent<CardLayoutGroup>();
    }

    public void DrawCardDisplay(GameObject card, int handNum)
    {
        card.transform.parent = CardLayout.transform;

        //èD‚ª6–‡ˆÈ‰º‚Ìê‡‚ÍîŒ`‚ÉL‚°‚é‚æ‚¤‚É‚·‚é
        if(handNum <= 6)
        {
            codeCardLayoutGroup.startAngle = 90 - 10 * (handNum - 1);
            codeCardLayoutGroup.endAngle = 90 + 10 * (handNum - 1);
        }
    }
}
