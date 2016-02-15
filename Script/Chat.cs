using System;

public class Chat
{
    public string areaInfo;
    public int channelType;
    public string content;
    public string friendReceiveName;
    public string friendReceiveUid;
    public int friendReceiveVip;
    public ErlArray goods;
    public int isShow;
    public int job;
    public string name;
    public int sender;
    public int stime;
    public string uid;
    public int vip;

    public Chat(string uid, string name, int vip, int channelType, int sender, int stime, int isShow, int job, string content, ErlArray goods, string friendReceiveUid, string friendReceiveName, int friendReceiveVip, string areaInfo)
    {
        this.uid = uid;
        this.name = name;
        this.vip = vip;
        this.channelType = channelType;
        this.sender = sender;
        this.stime = stime;
        this.job = job;
        this.content = content;
        this.isShow = isShow;
        this.goods = goods;
        this.friendReceiveUid = friendReceiveUid;
        this.friendReceiveName = friendReceiveName;
        this.friendReceiveVip = friendReceiveVip;
        this.areaInfo = areaInfo;
    }
}

