using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class PlayerData
{
    public static bool isLocal = false;

    public static int uid = 0;
    public static string host = "";
    public static int port = 0;
    public static int token = 0;

    public static string account = "";
    private static string password = "";
    private static string Key_IV = "";
    public static string PIN = "";

    public static string Default_Pin = "123456";


    public static void SetPassword(string _password)
    {
        password = _password;
        int len = password.Length > 5 ? 5 : password.Length;
        Key_IV = password.Substring(0, len) + EncryPassword(_password).Substring(0, 8 - len);
    }
    public static string GetPassword()
    {
        return password;
    }

    public static string EncryPassword(string _password)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(_password);//求Byte[]数组            
        byte[] Md5 = new MD5CryptoServiceProvider().ComputeHash(bytes);//求哈希值   
        string str = Convert.ToBase64String(Md5);
        return Convert.ToBase64String(Md5);
    }

    public static string EncryData(string data)
    {
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        byte[] inputByteArray = Encoding.UTF8.GetBytes(data);
        des.Key = Encoding.UTF8.GetBytes(Key_IV);// 密匙
        des.IV = Encoding.UTF8.GetBytes(Key_IV);// 初始化向量
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        string retB = Convert.ToBase64String(ms.ToArray());
        return retB;
    }

    public static string DecryData(string data)
    {
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        byte[] inputByteArray = Convert.FromBase64String(data);
        des.Key = Encoding.UTF8.GetBytes(Key_IV);
        des.IV = Encoding.UTF8.GetBytes(Key_IV);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        return Encoding.UTF8.GetString(ms.ToArray());
    }

    public static string GetLocalPasswordKey(string account)
    {
        return "password_" + account;
    }

    public static string GetLocalDataKey(string account)
    {
        return "data_" + account;
    }

    public static string GetPinKey()
    {
        string headStr = "";
        if (isLocal)
        {
            headStr = "pin_local_";
        }
        else
        {
            headStr = "pin_online_";
        }
        return headStr + account;
    }
}

[Serializable]
public class Proto_entry_req
{
    public int uid;
    public int token;
}

[Serializable]
public class Proto_entry_res
{
    public int code;
    public string data;
}

[Serializable]
public class Proto_updatedata_req
{
    public string password;
    public string data;
}