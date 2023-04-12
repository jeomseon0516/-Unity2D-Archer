using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
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
    }


    public void NextScene()
    {
        SceneManager.LoadScene("ProgressScene");
    }
}
