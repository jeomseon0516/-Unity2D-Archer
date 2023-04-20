using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using USERDATA;

public class RegisterController : MonoBehaviour
{
    private MessageBoxController _mgBox;
    private InputField _confirmPassword;
    private InputField _password;
    private InputField _id;
    private InputField _nickName;
    private Dropdown   _selectGender;

    private void Start()
    {
        transform.Find("ID").TryGetComponent(out _id);
        transform.Find("Password").TryGetComponent(out _password);
        transform.Find("ConfirmPassword").TryGetComponent(out _confirmPassword);
        transform.Find("NickName").TryGetComponent(out _nickName);
        transform.Find("SelectGender").TryGetComponent(out _selectGender);
        transform.parent.Find("WaitAndMessageBox").TryGetComponent(out _mgBox);
    }
    public void OnRegister()
    {
        string id        = _id.text;
        string password  = _password.text;
        string confirmPassword = _confirmPassword.text;
        string nickName  = _nickName.text;
        int selectGender = _selectGender.captionText.text == "man" ? 1 : 2;

        if (MessageBoxController.ErrorMessage(_mgBox, 
            id.Length <= 0 || password.Length <= 0 || confirmPassword.Length <= 0 || nickName.Length <= 0, 
            "회원 정보를 모두 입력해주십시오."))
            return;

        if (MessageBoxController.ErrorMessage(_mgBox, !Regex.IsMatch(id, CommonMacro.eMailCheck), 
            "이메일 양식을 정확하게 작성해주십시오."))
            return;

        if (MessageBoxController.ErrorMessage(_mgBox, Regex.IsMatch(password, CommonMacro.conditionsKorean), 
            "패스워드에 한글을 입력할 수 없습니다."))
            return;

        if (MessageBoxController.ErrorMessage(_mgBox, !password.Contains(confirmPassword), 
            "패스워드 확인을 다시 작성해주십시오."))
            return;

        // ..암호화 & 복호화
        password = CommonMacro.GetSecurityPassword(password);
        StartCoroutine(RegisterUser(new MemberForm(0, nickName, selectGender), new UserInfo(0, id, password)));
    }
    private IEnumerator RegisterUser(MemberForm memberForm, UserInfo userInfo)
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("order", "sign up");
        wwwForm.AddField("memberForm", JsonUtility.ToJson(memberForm));
        wwwForm.AddField("userInfo",   JsonUtility.ToJson(userInfo));

        _mgBox.gameObject.SetActive(true);
        // 응답에 대한 작업
        using (UnityWebRequest request = UnityWebRequest.Post(CommonMacro.url, wwwForm))
        {
            yield return request.SendWebRequest();
            GetSuccessData jsonData = JsonUtility.FromJson<GetSuccessData>(request.downloadHandler.text);
            MessageBoxController.SetMgBox(_mgBox, (jsonData.isSuccess ? "성공 " : "실패 ") + "Message : " + jsonData.message, 
                                          jsonData.isSuccess ? Color.blue : Color.red, true);
        }
    }
}