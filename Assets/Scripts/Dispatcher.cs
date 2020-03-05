using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dispatcher : MonoBehaviour
{
    public GameObject shipPrefab;
    public static Ship currentShip;

    string dictKey;
    bool isWorkingInstance = true;

    static Dictionary<string, int> shipsLeftToAllocate = new Dictionary<string, int>();
    static Dictionary<string, Text> lableDict = new Dictionary<string, Text>();
    static List<Dispatcher> allShips = new List<Dispatcher>();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        isWorkingInstance = name.Contains("(Clone)");
        dictKey = name.Replace("(Clone)", null);
        allShips.Add(this);
        foreach (var item in allShips)
        {
            Debug.Log(item);
        }
        var floorsNumStr = dictKey.Replace("Ship-", null);
        var shipsToAllocate = 5 - int.Parse(floorsNumStr);
        if (!shipsLeftToAllocate.ContainsKey(dictKey))
        {
            shipsLeftToAllocate.Add(dictKey, shipsToAllocate);
            FillLabelsDict();
        }
        ReFreshLabel();
    }

    protected void OnShipClick()
    {   
        if (isWorkingInstance)
        {
            if (currentShip == null)
            {
                currentShip = GetComponentInChildren<Ship>();
            }
            else if (currentShip.IsPositionCorrect)
            {
                if (!currentShip.WAsLocatedOnse())
                {
                    shipsLeftToAllocate[dictKey]--;
                    ReFreshLabel();
                }
                currentShip = null;
            }
        }
        else if (currentShip == null&&shipsLeftToAllocate[dictKey]>0) // Обычный шаблон
        {
            var shipObjToPlay = Instantiate(shipPrefab, transform.parent.transform);
            currentShip = shipObjToPlay.GetComponentInChildren<Ship>();
        }
    }
    void FillLabelsDict()
    {
        var LabelObj = GameObject.Find(dictKey+"(Label)");
        var Label = LabelObj.GetComponent<Text>();
        lableDict.Add(dictKey, Label);
        /*var Labels = transform.parent.GetComponentsInChildren<Text>();
        foreach (var Label in Labels)
        {
            if (!Label.name.Contains("Label"))
            {
                continue;
            }
            Debug.Log(Label);
        }*/
    }
    void ReFreshLabel()
    {
        lableDict[dictKey].text = shipsLeftToAllocate[dictKey]+"x";
    }
}
