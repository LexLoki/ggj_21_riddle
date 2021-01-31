using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CharadesDAO
{

    private static CharadesDAO _instance = null;
    public static CharadesDAO Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new CharadesDAO();
            }
            return _instance;
        }
    }

    private Charade[] _data;

    public void LoadCharadeData(TextAsset textAsset)
    {
        _data = JsonUtility.FromJson<CharadeData>(textAsset.text).data;
    }

    public List<string> GetItensList()
    {
        List<string> itens = new List<string>();
        Dictionary<string, bool> itemSet = new Dictionary<string, bool>();
        foreach(Charade charade in _data)
        {
            if (!itemSet.ContainsKey(charade.answer))
            {
                itemSet.Add(charade.answer, true);
                itens.Add(charade.answer);
            }
        }
        return itens;
    }

    public List<Charade> GetRiddlesForItem(string item)
    {
        List<Charade> riddles = new List<Charade>();
        foreach(Charade charade in _data)
        {
            if (item.Equals(charade.answer))
            {
                riddles.Add(charade);
            }
        }
        return riddles;
    }

    public Charade GetRandomRiddleForItem(string item)
    {
        List<Charade> riddles = GetRiddlesForItem(item);
        return riddles[Random.Range(0, riddles.Count)];
    }

    public List<Charade> GenerateRandomRiddlesList(List<string> itens)
    {
        return itens.ConvertAll(GetRandomRiddleForItem);
    }

    public List<string> GenerateRandomItensList(int n)
    {
        List<string> generated = new List<string>();
        List<string> itensList = GetItensList();
        int index;
        string item;
        while(n-- > 0)
        {
            index = Random.Range(0, itensList.Count);
            item = itensList[index];
            itensList.RemoveAt(index);
            generated.Add(item);
        }
        return generated;
    }


}
