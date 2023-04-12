using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using USERDATA;

public class RegistController : MonoBehaviour
{
    private InputField _confirmPassword;
    private InputField _password;
    private InputField _id;
    private InputField _nickName;
    private Dropdown   _selectGender;
    private MessageBoxController _mgBox;

    void Start()
    {
        transform.Find("ID").TryGetComponent(out _id);
        transform.Find("Password").TryGetComponent(out _password);
        transform.Find("ConfirmPassword").TryGetComponent(out _confirmPassword);
        transform.Find("NickName").TryGetComponent(out _nickName);
        transform.Find("SelectGender").TryGetComponent(out _selectGender);
        transform.parent.Find("WaitAndMessageBox").TryGetComponent(out _mgBox);
    }

    public void OnRegist()
    {
        string id        = _id.text;
        string password  = _password.text;
        string confirmPassword = _confirmPassword.text;
        string nickName  = _nickName.text;
        int selectGender = _selectGender.captionText.text == "man" ? 1 : 2;

        if (id.Length <= 0 || password.Length <= 0 || confirmPassword.Length <= 0 || nickName.Length <= 0)
        {
            _mgBox.gameObject.SetActive(true);
            MessageBoxController.SetMgBox(_mgBox, "회원 정보를 모두 입력해주십시오.", Color.red, true);
            return;
        }

        if (!Regex.IsMatch(id, @"^([a-zA-Z0-9]+)@([a-zA-Z0-9-]+)(\.[a-zA-Z0-9]+){1,}$"))
        {
            _mgBox.gameObject.SetActive(true);
            MessageBoxController.SetMgBox(_mgBox, "이메일 양식을 정확하게 작성해주십시오.", Color.red, true);
            return;
        }

        if (Regex.IsMatch(password, @"^[ㄱ-ㅎ가-힣ㅏ-ㅣ]"))
        {
            _mgBox.gameObject.SetActive(true);
            MessageBoxController.SetMgBox(_mgBox, "패스워드에 한글을 입력할 수 없습니다.", Color.red, true);
            return;
        }

        if (!password.Contains(confirmPassword))
        {
            _mgBox.gameObject.SetActive(true);
            MessageBoxController.SetMgBox(_mgBox, "패스워드 확인을 다시 작성해주십시오.", Color.red, true);
            return;
        }

        StartCoroutine(RegistUser(new MemberForm(0, nickName, selectGender), new UserInfo(0, id, password)));
    }


    private IEnumerator RegistUser(MemberForm memberForm, UserInfo userInfo)
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("order", "sign up");
        wwwForm.AddField("memberForm", JsonUtility.ToJson(memberForm));
        wwwForm.AddField("userInfo",   JsonUtility.ToJson(userInfo));

        _mgBox.gameObject.SetActive(true);
        // 응답에 대한 작업
        using (UnityWebRequest request = UnityWebRequest.Post(URL.url, wwwForm))
        {
            yield return request.SendWebRequest();
            GetOkData jsonData = JsonUtility.FromJson<GetOkData>(request.downloadHandler.text);
            MessageBoxController.SetMgBox(_mgBox, (jsonData.isOk ? "성공 " : "실패 ") + "Message : " + jsonData.message, 
                                          jsonData.isOk ? Color.blue : Color.red, true);
        }
    }
}
