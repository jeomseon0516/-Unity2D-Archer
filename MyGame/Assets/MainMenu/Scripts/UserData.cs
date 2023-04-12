using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USERDATA
{
    static public class URL
    {
        static public string url = "https://script.google.com/macros/s/AKfycbwH9NtpHxpdVkzHcKZkynlBzDMXiGclCotvnnALR3JJnH8uxtHjLgRPxElZHMgBgAVe/exec";
    }

    [System.Serializable]
    public class MemberForm
    {
        public int pk;
        public string name;
        public int gender;

        public MemberForm(int pk, string name, int gender)
        {
            this.pk = pk;
            this.name = name;
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

    [System.Serializable]
    public class GetOkData
    {
        public bool isOk;
        public string message;

        public GetOkData(bool isOk, string message)
        {
            this.isOk = isOk;
            this.message = message;
        }
    }
}
