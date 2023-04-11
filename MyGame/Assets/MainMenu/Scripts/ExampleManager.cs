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
    public int gender;

    public MemberForm(int pk, string name, int age, int gender)
    {
        this.pk     = pk;
        this.name   = name;
        this.age    = age;
        this.gender = gender;
    }
}

[System.Serializable]
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

[System.Serializable]
public class UserData
{
    public string memberForm;
    public string userInfo;

    public UserData(string memberForm, string userInfo)
    {
        this.memberForm = memberForm;
        this.userInfo = userInfo;
    }
}

public class ExampleManager : MonoBehaviour
{
    private string URL = "https://script.google.com/macros/s/AKfycbwH9NtpHxpdVkzHcKZkynlBzDMXiGclCotvnnALR3JJnH8uxtHjLgRPxElZHMgBgAVe/exec";

    private void Start()
    {
        // 요청을 하기위한 작업
        //MemberForm memberForm = new MemberForm("변사또", 45, 5);

        //WWWForm form = new WWWForm();
        ////form.AddField("Order", )
        //form.AddField("Name", memberForm.name);
        //form.AddField("Age",  memberForm.age);
        //form.AddField("Pk",   memberForm.pk);
        //pk가 0일경우 회원가입 또는 존재하지 않는 회원
        StartCoroutine(RegistUser(new MemberForm(0, "신재훈", 21, 3), new UserInfo(0, "ssam0708@gmail.com", "iloveshowta1337")));
    }

    private IEnumerator RegistUser(MemberForm memberForm, UserInfo userInfo)
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("order", "sign up");
        wwwForm.AddField("memberForm", JsonUtility.ToJson(memberForm));
        wwwForm.AddField("userInfo",   JsonUtility.ToJson(userInfo));

        // 응답에 대한 작업
        using (UnityWebRequest request = UnityWebRequest.Post(URL, wwwForm))
        {
            yield return request.SendWebRequest();
            print(request.downloadHandler.text);
        }
    }

    public void NextScene()
    {
        SceneManager.LoadScene("ProgressScene");
    }
}
