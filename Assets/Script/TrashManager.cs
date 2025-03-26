using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashManager : MonoBehaviour
{
    Dictionary<int, CardInfo> trashDict = new Dictionary<int, CardInfo>();
    Dictionary<int, GameObject> trashCardObjectDict = new Dictionary<int, GameObject>();

    public void AddCardToTrash(CardInfo card, GameObject cardObject)
    {
        trashDict.Add(card.id, card);
        trashCardObjectDict.Add(card.id, cardObject);

        cardObject.transform.parent = transform;
        cardObject.transform.localPosition = new Vector3(0, trashDict.Count * 0.001f, 0);
        float randomRotY = Random.Range(-3, 3);
        cardObject.transform.localRotation = Quaternion.Euler(0, randomRotY, 0);
    }
}
