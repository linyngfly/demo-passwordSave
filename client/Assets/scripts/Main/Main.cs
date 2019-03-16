using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public static Main instance = null;
    public Transform baseDirTrsm;
    public Transform dirPrefab;
    public Transform filePrefab;
    public Sprite img_online;
    public Sprite img_local;
    [HideInInspector]
    public DirData baseDirData = null;
    [HideInInspector]
    public ToggleGroup baseToggleGroup;
    RectTransform baseDirRect = null;
    int maxDepth = 0;
    float resetTime = 0f;
    int connectSvrTimes = 0;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        SocketWait.instance.Hide();
        baseToggleGroup = GetComponent<ToggleGroup>();
        baseDirRect = transform.Find("records/Viewport/baseDir").GetComponent<RectTransform>();

        transform.Find("up/account").GetComponent<Text>().text = PlayerData.account;
        Image tmpImg = transform.Find("up/net").GetComponent<Image>();
        if (PlayerData.isLocal)
        {
            tmpImg.sprite = img_local;
            string data = PlayerPrefs.GetString(PlayerData.GetLocalDataKey(PlayerData.account));
            if (data == "")
            {
                baseDirData = new DirData();
            }
            else
            {
                baseDirData = JsonUtility.FromJson<DirData>(PlayerData.DecryData(data));
            }
            InitData(true);

        }
        else
        {
            tmpImg.sprite = img_online;
            SocketWait.instance.Show("连接服务器中");
            SocketClient.OnOpen(SVR_OnConnectorOpen);
            SocketClient.OnClose(SVR_OnConnectorClose);
            Connect_svr();
        }
    }


    void InitData(bool isChange)
    {
        if (!isChange)
        {
            return;
        }


        foreach (Transform child in baseDirTrsm)
        {
            Destroy(child.gameObject);
        }
        if (FilePanel.instance != null)
        {
            Destroy(FilePanel.instance.gameObject);
        }
        if (FileInfoPanel.instance != null)
        {
            Destroy(FileInfoPanel.instance.gameObject);
        }
        if (DirPanel.instance != null)
        {
            Destroy(DirPanel.instance.gameObject);
        }


        for (int i = 0; i < baseDirData.dirs.Count; i++)
        {
            DirData oneDirData = baseDirData.dirs[i];
            Transform dirObj = Instantiate(dirPrefab, baseDirTrsm);
            dirObj.GetComponent<Toggle>().group = baseToggleGroup;
            oneDirData.attachedObj = dirObj;
            oneDirData.parentDir = baseDirData;
            DirPrefab tmpDirPrefab = dirObj.GetComponent<DirPrefab>();
            tmpDirPrefab.SetDirName(oneDirData.dirName);
            tmpDirPrefab.SetPos(0);
            tmpDirPrefab.attachedDirData = oneDirData;
        }
        baseDirData.isFold = false;

        for (int i = 0; i < baseDirData.files.Count; i++)
        {
            FileData oneFileData = baseDirData.files[i];
            Transform fileObj = Instantiate(filePrefab, baseDirTrsm);
            fileObj.GetComponent<Toggle>().group = baseToggleGroup;
            oneFileData.attachedObj = fileObj;
            oneFileData.parentDir = baseDirData;
            fileObj.GetComponent<FilePrefab>().Init(oneFileData);
        }
        ResetSize();
    }

    /// <summary>
    /// 新建最外层文件
    /// </summary>
    public void Btn_baseNewFile()
    {
        Transform fileObj = Instantiate(filePrefab, baseDirTrsm);
        fileObj.GetComponent<Toggle>().group = baseToggleGroup;
        FileData tmp = new FileData();
        baseDirData.files.Add(tmp);
        tmp.parentDir = baseDirData;
        tmp.keys.AddRange(new List<string> { "名称", "账号", "密码", "备注" });
        tmp.values.AddRange(new List<string> { "默认名", "", "", "" });
        tmp.attachedObj = fileObj;
        fileObj.GetComponent<FilePrefab>().Init(tmp);
        SaveData();
        ResetSize();
    }

    /// <summary>
    /// 新建最外层文件夹
    /// </summary>
    public void Btn_baseNewDir()
    {
        Transform dirObj = Instantiate(dirPrefab, baseDirTrsm);
        dirObj.GetComponent<Toggle>().group = baseToggleGroup;
        DirData tmp = new DirData();
        baseDirData.dirs.Add(tmp);
        tmp.parentDir = baseDirData;
        tmp.dirName = "默认分组";
        tmp.attachedObj = dirObj;
        DirPrefab tmpDirPrefab = dirObj.GetComponent<DirPrefab>();
        tmpDirPrefab.SetDirName(tmp.dirName);
        tmpDirPrefab.SetPos(0);
        tmpDirPrefab.attachedDirData = tmp;
        dirObj.SetSiblingIndex(tmp.GetDirIndex());
        SaveData();
        ResetSize();
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    public void SaveData()
    {
        if (PlayerData.isLocal)
        {
            PlayerPrefs.SetString(PlayerData.GetLocalDataKey(PlayerData.account), PlayerData.EncryData(JsonUtility.ToJson(baseDirData)));
        }
        else
        {
            SocketWait.instance.Show("上传数据中");
            string data_str = PlayerData.EncryData(JsonUtility.ToJson(baseDirData));
            Proto_updatedata_req msg = new Proto_updatedata_req();
            msg.data = data_str;
            SocketClient.SendMsg("connector.main.updateData", msg);
        }
    }


    /// <summary>
    /// layout手动刷新
    /// </summary>
    public void ResetSize()
    {
        StopCoroutine("UpdateLayout");
        resetTime = 0f;
        StartCoroutine("UpdateLayout");
    }

    IEnumerator UpdateLayout()
    {
        while (true)
        {
            resetTime += Time.deltaTime;
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseDirRect);
            yield return new WaitForEndOfFrame();
            if (resetTime >= 3f)
            {
                StopCoroutine("UpdateLayout");
            }
        }
    }

    private void Update()
    {
        SocketClient.ReadMsg();
    }

    private bool IsFocusOnInputText()
    {
        GameObject tmp = EventSystem.current.currentSelectedGameObject;
        if (tmp == null)
        {
            return false;
        }
        if (tmp.GetComponent<InputField>() != null)
        {
            return true;
        }
        return false;
    }


    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (IsFocusOnInputText())
            {
                return;
            }
            if (UnlockPanel.instance == null)
            {
                Btn_lock();
            }
        }
    }

    void FindDepth(DirData dirData, int nowDep)
    {
        if (dirData.isFold)
        {
            if (nowDep > maxDepth)
            {
                maxDepth = nowDep;
            }
            return;
        }
        if (dirData.dirs.Count == 0)
        {
            if (dirData.files.Count > 0)
            {
                nowDep++;
            }
            if (nowDep > maxDepth)
            {
                maxDepth = nowDep;
            }
            return;
        }
        nowDep++;
        for (int i = 0; i < dirData.dirs.Count; i++)
        {
            FindDepth(dirData.dirs[i], nowDep);
        }
    }

    public void Btn_ToLoginScene()
    {
        SocketClient.DisConnect();
        SceneManager.LoadScene("login");
    }

    public void Btn_lock()
    {
        Object obj = Resources.Load("Panels/unlockPanel");
        Instantiate(obj, transform);
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    public void Btn_changePassword()
    {
        Object obj = Resources.Load("Panels/changePasswordPanel");
        Instantiate(obj, transform);
    }

    public void ChangePasswordYes(string password)
    {
        if (PlayerData.isLocal)
        {
            PlayerData.SetPassword(password);
            PlayerPrefs.SetString(PlayerData.GetLocalPasswordKey(PlayerData.account), PlayerData.EncryPassword(password));
            PlayerPrefs.SetString(PlayerData.GetLocalDataKey(PlayerData.account), PlayerData.EncryData(JsonUtility.ToJson(baseDirData)));
        }
        else
        {
            PlayerData.SetPassword(password);
            SocketWait.instance.Show("修改密码中");
            string data_str = PlayerData.EncryData(JsonUtility.ToJson(baseDirData));
            Proto_updatedata_req msg = new Proto_updatedata_req();
            msg.data = data_str;
            msg.password = PlayerData.EncryPassword(password);
            SocketClient.SendMsg("connector.main.changePassword", msg);
        }
    }

    public DirPanel GetDirPanel()
    {
        if (DirPanel.instance == null)
        {
            Object obj = Resources.Load("Panels/dirPanel");
            Instantiate(obj, transform);
        }
        return DirPanel.instance;
    }

    public FileInfoPanel GetFileInfoPanel()
    {
        if (FileInfoPanel.instance == null)
        {
            Object obj = Resources.Load("Panels/fileInfoPanel");
            Instantiate(obj, transform);
        }
        return FileInfoPanel.instance;
    }
    public FilePanel GetFilePanel()
    {
        if (FilePanel.instance == null)
        {
            Object obj = Resources.Load("Panels/filePanel");
            Instantiate(obj, transform);
        }
        return FilePanel.instance;
    }

    // 服务器连接成功
    void SVR_OnConnectorOpen(string msg)
    {
        SocketClient.AddHandler("connector.main.entry", SVR_EntryBack);
        SocketClient.AddHandler("connector.main.updateData", SVR_UpdateDataBack);
        SocketClient.AddHandler("connector.main.changePassword", SVR_ChangePasswordBack);
        SocketClient.AddHandler("onKicked", SVR_OnKicked);
        Proto_entry_req data = new Proto_entry_req();
        data.uid = PlayerData.uid;
        data.token = PlayerData.token;
        SocketClient.SendMsg("connector.main.entry", data);
        connectSvrTimes = 0;
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    void Connect_svr()
    {
        SocketClient.Connect(PlayerData.host, PlayerData.port);
    }

    IEnumerator Connect_svr_later()
    {
        SocketWait.instance.Show("连接服务器中");
        connectSvrTimes++;
        yield return new WaitForSeconds(2f);
        Connect_svr();
    }

    // 服务器连接失败
    void SVR_OnConnectorClose(string msg)
    {
        Debug.Log("connectTimes:" + connectSvrTimes);
        if (connectSvrTimes >= 3)
        {
            SocketWait.instance.Show("网络错误，请重新登录");
            StartCoroutine(BackToLogin());
        }
        else
        {
            StartCoroutine(Connect_svr_later());
        }
    }


    void Wrong_data_back()
    {
        SocketClient.DisConnect();
        SocketWait.instance.Show("数据错误，请重新登录");
        StartCoroutine(BackToLogin());
    }

    void SVR_EntryBack(string msg)
    {
        Proto_entry_res res = JsonUtility.FromJson<Proto_entry_res>(msg);
        if (res.code == 0)
        {
            bool isChange = false;
            if (res.data == "")
            {
                if (baseDirData == null)
                {
                    baseDirData = new DirData();
                    isChange = true;
                }
                else if (baseDirData.files.Count != 0 || baseDirData.dirs.Count != 0)
                {
                    baseDirData = new DirData();
                    isChange = true;
                }
                else
                {
                    isChange = false;
                }
            }
            else
            {
                string getData = PlayerData.DecryData(res.data);
                if (baseDirData == null)
                {
                    isChange = true;
                    baseDirData = JsonUtility.FromJson<DirData>(getData);
                }
                else
                {
                    string localData = JsonUtility.ToJson(baseDirData);
                    if (localData != getData)
                    {
                        isChange = true;
                        baseDirData = JsonUtility.FromJson<DirData>(getData);
                    }
                }

            }
            InitData(isChange);
            SocketWait.instance.Hide();
        }
        else
        {
            Wrong_data_back();
        }
    }

    void SVR_UpdateDataBack(string msg)
    {
        Proto_entry_res res = JsonUtility.FromJson<Proto_entry_res>(msg);
        if (res.code == 0)
        {
            SocketWait.instance.Hide();
        }
        else
        {
            Wrong_data_back();
        }
    }

    void SVR_ChangePasswordBack(string msg)
    {
        Proto_entry_res res = JsonUtility.FromJson<Proto_entry_res>(msg);
        if (res.code == 0)
        {
            SocketWait.instance.Hide();
        }
        else
        {
            Wrong_data_back();
        }
    }
    void SVR_OnKicked(string msg)
    {
        SocketClient.DisConnect();
        SocketWait.instance.Show("账号在其他地方登录了");
        StartCoroutine(BackToLogin());
    }

    IEnumerator BackToLogin()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("login");
    }


    private void OnDisable()
    {
        SocketClient.OffOpen();
        SocketClient.OffClose();
        SocketClient.RemoveHandler("connector.main.entry");
        SocketClient.RemoveHandler("connector.main.updateData");
        SocketClient.RemoveHandler("connector.main.changePassword");
        SocketClient.RemoveHandler("onKicked");
        SocketClient.DisConnect();
    }
}