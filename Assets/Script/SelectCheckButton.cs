using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCheckButton : MonoBehaviour
{
    [SerializeField] Button codeButton;
    [SerializeField] CardGameManager codeCardGameManager;

    int selectNum = 0;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        codeButton.interactable = selectNum == codeCardGameManager.selectedCardIdHash.Count;
    }

    public void SelectNumSet(int num)
    {
        gameObject.SetActive(true);
        selectNum = num;
    }
}
