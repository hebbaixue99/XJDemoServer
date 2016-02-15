using System;
using System.Collections.Generic;

public class TimerManager
{
    private static TimerManager _instance;
    private List<Timer> timerList = new List<Timer>();

    public void clearAllTimer()
    {
        if ((this.timerList != null) || (this.timerList.Count > 0))
        {
            for (int i = 0; i < this.timerList.Count; i++)
            {
                if (this.timerList[i] != null)
                {
                    this.timerList[i].stop();
                    this.timerList[i] = null;
                }
            }
        }
        this.timerList.Clear();
    }

    public Timer getTimer(long delay)
    {
        Timer item = new Timer(delay, 0);
        this.timerList.Add(item);
        return item;
    }

    public Timer getTimer(long delay, int count)
    {
        Timer item = new Timer(delay, count);
        this.timerList.Add(item);
        return item;
    }

    public void removeTimer(Timer timer)
    {
        if ((timer != null) && ((this.timerList != null) && this.timerList.Contains(timer)))
        {
            timer.stop();
            this.timerList.Remove(timer);
        }
    }

    public void update()
    {
        for (int i = 0; i < this.timerList.Count; i++)
        {
            if (this.timerList[i].isDispose())
            {
                this.timerList.RemoveAt(i);
            }
            else
            {
                this.timerList[i].update();
            }
        }
    }

    public static TimerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TimerManager();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
}

