using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

namespace USERDATA
{
    static public class CommonMacro
    {
        static public string url = "https://script.google.com/macros/s/AKfycbwH9NtpHxpdVkzHcKZkynlBzDMXiGclCotvnnALR3JJnH8uxtHjLgRPxElZHMgBgAVe/exec";
        static public string eMailCheck = @"^([a-zA-Z0-9]+)@([a-zA-Z0-9-]+)(\.[a-zA-Z0-9]+){1,}$";
        static public string conditionsKorean = @"^[ㄱ-ㅎ가-힣ㅏ-ㅣ]";

        static public string GetSecurityPassword(string password)
        {
            SHA256 sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(password));
            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }

            return stringBuilder.ToString();
        }
    }

    [System.Serializable]
    public class MemberForm
    {
        public int pk;
        public string name;
        public int gender;

        public MemberForm(int pk, string name, int gender)
        {
            this.pk     = pk;
            this.name   = name;
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
            this.pk       = pk;
            this.id       = id;
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
            this.userInfo   = userInfo;
        }
    }

    // pk를 받아온다.
    [System.Serializable]
    public class GetSuccessData
    {
        public int pk;
        public bool isSuccess;
        public string message;

        public GetSuccessData(int pk, bool isSuccess, string message)
        {
            this.pk        = pk;
            this.isSuccess = isSuccess;
            this.message   = message;
        }
    }
}
