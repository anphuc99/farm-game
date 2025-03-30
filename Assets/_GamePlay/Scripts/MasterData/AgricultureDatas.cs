using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AgricultureData
{
    public int itemID;
    public GameObject junior1;
    public GameObject junior2;
    public GameObject mature;
    public GameObject dead;
}

[CreateAssetMenu(fileName = "AgricultureData", menuName = "data/AgricultureData")]
public class AgricultureDatas : ScriptableObject
{
    public static AgricultureDatas Instance 
    {
        get
        {
            if(instance == null)
            {
                instance = Resources.Load<AgricultureDatas>("AgricultureData");
            }
            return instance;
        }
    }

    private static AgricultureDatas instance;

    public List<AgricultureData> items;
}
