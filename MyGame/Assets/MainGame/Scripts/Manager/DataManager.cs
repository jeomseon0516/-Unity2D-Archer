using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

[System.Serializable]
class ItemForm
{
    public string A;
    public string B;

    ItemForm(string _A, string _B)
    {
        A = _A;
        B = _B;
    }
}

[System.Serializable]
class DataForm
{
    public string name;
    public string age;

    ItemForm item;
    public DataForm(string _name, string _age)
    {
        name = _name;
        age = _age;
    }
    public DataForm() {}
}
public class DataManager : SingletonTemplate<DataManager>
{
    private string _userName;
    private int _value;
    protected override void Init() 
    {
        _value = 0;

        var jsonData = Resources.Load<TextAsset>("SaveFile/Data");
        DataForm form = JsonUtility.FromJson<DataForm>(jsonData.ToString());

        _value = int.Parse(form.age);
        _userName = form.name;

        print(jsonData);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ++_value;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            --_value;
        if (Input.GetKeyDown(KeyCode.Space))
            SaveData("이몽룡", _value.ToString());
    }
    public void SaveData(string name, string age)
    {
        DataForm form = new DataForm(name, age);
        
        string jsonData = JsonUtility.ToJson(form);

        FileStream fileStream = new FileStream(
            Application.dataPath + "/MainGame/Resources/SaveFile/Data.json", FileMode.Create);

        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }
    private DataManager() {}
}
