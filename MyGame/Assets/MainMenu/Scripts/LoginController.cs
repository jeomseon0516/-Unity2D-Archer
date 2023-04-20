using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using USERDATA;

public class LoginController : MonoBehaviour
{
    private MessageBoxController _mgBox;
    private InputField _id;
    private InputField _password;

    // Start is called before the first frame update
    private void Start()
    {
        transform.parent.Find("WaitAndMessageBox").TryGetComponent(out _mgBox);
        transform.Find("ID").TryGetComponent(out _id);
        transform.Find("Password").TryGetComponent(out _password);
    }
    public void OnLogin()
    {
        string id = _id.text;
        string password = _password.text;

        if (MessageBoxController.ErrorMessage(_mgBox, id.Length <= 0 || password.Length <= 0,
            "패스워드와 아이디를 모두 입력해주세요."))
            return;

        if (MessageBoxController.ErrorMessage(_mgBox, !Regex.IsMatch(id, CommonMacro.eMailCheck),
            "이메일을 정확하게 입력해주십시오."))
            return;

        if (MessageBoxController.ErrorMessage(_mgBox, Regex.IsMatch(password, CommonMacro.conditionsKorean),
            "패스워드에 한글을 입력할 수 없습니다."))
            return;

        password = CommonMacro.GetSecurityPassword(password);
        StartCoroutine(LoginUser(id, password));
    }
    private IEnumerator LoginUser(string id, string password)
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("order", "sign in");
        wwwForm.AddField("id", id);
        wwwForm.AddField("password", password);

        _mgBox.gameObject.SetActive(true);
        // 응답에 대한 작업
        using (UnityWebRequest request = UnityWebRequest.Post(CommonMacro.url, wwwForm))
        {
            yield return request.SendWebRequest();
            GetSuccessData jsonData = JsonUtility.FromJson<GetSuccessData>(request.downloadHandler.text);
            MessageBoxController.SetMgBox(_mgBox, (jsonData.isSuccess ? "성공 " : "실패 ") + "Message : " + jsonData.message,
                                          jsonData.isSuccess ? Color.blue : Color.red, true);
            if (jsonData.isSuccess)
                SceneManager.LoadScene("ProgressScene");
        }
    }
}
