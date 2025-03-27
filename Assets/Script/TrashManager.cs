using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashManager : MonoBehaviour
{
    [SerializeField] CardGameManager codeCardGameManager;

    //Id��CardInfo��ۑ�
    Dictionary<int, CardInfo> trashDict = new Dictionary<int, CardInfo>();
    //cardType����CardInfo��ۑ�
    Dictionary<int, List<CardInfo>> type_CardInfoDict = new Dictionary<int, List<CardInfo>> ();
    //Id��GameObject��ۑ�
    Dictionary<int, GameObject> trashCardObjectDict = new Dictionary<int, GameObject>();


    public void AddCardToTrash(CardInfo card, GameObject cardObject)
    {
        trashDict.Add(card.id, card);
        if (type_CardInfoDict.ContainsKey(card.cardType))
        {
            type_CardInfoDict[card.cardType].Add(card);
        }
        else
        {
            type_CardInfoDict.Add(card.cardType, new List<CardInfo>() { card });
        }
        trashCardObjectDict.Add(card.id, cardObject);

        cardObject.transform.parent = transform;
        cardObject.transform.localPosition = new Vector3(0, trashDict.Count * 0.01f, 0);
        float randomRotY = Random.Range(-3, 3);
        cardObject.transform.localRotation = Quaternion.Euler(0, randomRotY, 0);
    }

    //EventTrigger > ClickEvent����Ă΂��
    public void OpenTrashList()
    {
        codeCardGameManager.OpenCardListDisplay(type_CardInfoDict);
    }
}
