using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
 

public class MiniConnectManager 
{
    public string _ip;
    public int _port;
    public Object content;
    private int fps;
    private float fpsTime;
    public static string ip;
    public static bool IsRobot;
    public Object itemPreafb;
    public bool log;
    public static long now;
    public static int port;
    private int showFps;
    public int startId;
    public int userCount;

    [DebuggerHidden]
    private IEnumerator BuildItems()
    {
        return new  c__Iterator87 { f__this = this };
    }

    public void OnReceveRadio(Connect c, object obj)
    {
        ErlKVMessage message = obj as ErlKVMessage;
        if (message.getValue("report") != null)
        {
            this.fps++;
        }
    }

    private void Start()
    {
        IsRobot = !this.log;
        ip = this._ip;
        port = this._port;
        DataAccess.getInstance().defaultHandle = new ReceiveFun(this.OnReceveRadio);
        //base.StartCoroutine(this.BuildItems());
    }

   /* private void Update()
    {
        this.fpsTime += Time.deltaTime;
        if (this.fpsTime > 1f)
        {
            this.fpsTime = 0f;
            this.showFps = this.fps;
            this.fps = 0;
        }
        now = ServerTimeKit.getMillisTime();
    }*/

    [CompilerGenerated]
    private sealed class  c__Iterator87 : IEnumerator, IDisposable, IEnumerator<object>
    {
        internal object current;
        internal int PC;
        internal MiniConnectManager f__this;
        internal int end__0;
        internal int i__3;
       // internal MiniConnectItem item__5;
        internal Object obj__4;
        internal float x__2;
        internal float y__1;

        [DebuggerHidden]
        public void Dispose()
        {
            this.PC = -1;
        }

        public bool MoveNext()
        {
           /* uint num = (uint) this.PC;
            this.PC = -1;
            switch (num)
            {
                case 0:
                    this.end__0 = this.f__this.startId + this.f__this.userCount;
                    this.y__1 = 414f;
                    this.x__2 = -158f;
                    this.i__3 = this.f__this.startId;
                    break;

                case 1:
                    this.i__3++;
                    break;

                default:
                    goto  Label_0171;
            }
            if (this.i__3 < this.end__0)
            {
                this.obj__4 = NGUITools.AddChild(this.f__this.content, this.f__this.itemPreafb);
                this.obj__4.name = this.i__3.ToString();
                this.obj__4.SetActive(true);
                this.item__5 = this.obj__4.GetComponent<MiniConnectItem>();
                this.item__5.Init(MiniConnectManager.ip, MiniConnectManager.port, this.i__3);
                this.obj__4.transform.localPosition = new Vector3(this.x__2, this.y__1, 0f);
                this.x__2 *= -1f;
                if (this.x__2 < 0f)
                {
                    this.y__1 -= 80f;
                }
                this.current = 1;
                this.PC = 1;
                return true;
            }
            this.PC = -1;
        Label_0171: */
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.current;
            }
        }
    }
}

