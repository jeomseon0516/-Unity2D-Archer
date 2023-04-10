using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class MemberForm
{
    public string Name;
    public int Age;
    public int Pk;

    public MemberForm(string userName, int age, int pk)
    {
        Name = userName;
        Age = age;
        Pk = pk;
    }
}

public class ExampleManager : MonoBehaviour
{
    private string URL = "https://script.google.com/macros/s/AKfycbwH9NtpHxpdVkzHcKZkynlBzDMXiGclCotvnnALR3JJnH8uxtHjLgRPxElZHMgBgAVe/exec";

    IEnumerator Start()
    {
        print("Start");
        // 요청을 하기위한 작업
        MemberForm meneberForm = new MemberForm("변사또", 45, 5);

        WWWForm form = new WWWForm();
        //form.AddField("Order", )
        form.AddField("Name", meneberForm.Name);
        form.AddField("Age",  meneberForm.Age);
        form.AddField("Pk",   meneberForm.Pk);

        // 응답에 대한 작업
        using (UnityWebRequest request = UnityWebRequest.Post(URL, form))
        {
            yield return request.SendWebRequest();
            print(request.downloadHandler.text);
        }
    }
}
