using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MemberForm
{
    public int pk;
    public string name;
    public int age;
    public string gender;

    public MemberForm(int pk, string name, int age, string gender)
    {
        this.pk     = pk;
        this.name   = name;
        this.age    = age;
        this.gender = gender;
    }
}

public class UserInfo
{
    public int pk;
    public string id;
    public string password;

    public UserInfo(int pk, string id, string password)
    {
        this.pk = pk;
        this.id = id;
        this.password = password;
    }
}

public class UserData
{
    public MemberForm memberForm;
    public UserInfo userInfo;

    public UserData(MemberForm memberForm, UserInfo userInfo)
    {
        this.memberForm = memberForm;
        this.userInfo = userInfo;
    }
}

public class ExampleManager : MonoBehaviour
{
    private string URL = "https://script.google.com/macros/s/AKfycbwH9NtpHxpdVkzHcKZkynlBzDMXiGclCotvnnALR3JJnH8uxtHjLgRPxElZHMgBgAVe/exec";

    IEnumerator Start()
    {
        // 요청을 하기위한 작업
        //MemberForm memberForm = new MemberForm("변사또", 45, 5);

        //WWWForm form = new WWWForm();
        ////form.AddField("Order", )
        //form.AddField("Name", memberForm.name);
        //form.AddField("Age",  memberForm.age);
        //form.AddField("Pk",   memberForm.pk);
        MemberForm memberForm = new MemberForm(5, "변사또", 5, "남자");
        UserInfo userInfo     = new UserInfo(5, "byeun2018@gmail.com", "1591");
        UserData userData     = new UserData(memberForm, userInfo);

        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("Order", "sing up");
        wwwForm.AddField("MemberForm", JsonUtility.ToJson(memberForm));
        wwwForm.AddField("UserInfo",   JsonUtility.ToJson(userInfo));

        // 응답에 대한 작업
        using (UnityWebRequest request = UnityWebRequest.Post(URL, wwwForm))
        {
            yield return request.SendWebRequest();
            UserData jsonData = JsonUtility.FromJson<UserData>(request.downloadHandler.text);

            print(request.downloadHandler.text);
        }
    }

    public void NextScene()
    {
        SceneManager.LoadScene("ProgressScene");
    }
}
