using System;

public class Timer
{
    public int currentCount;
    public long currentTime;
    public long delayTime;
    private bool dispose;
    private TimerHandle onComplete;
    private TimerHandle onTimer;
    private int repeatCount;
    public bool running;
    private long startTime;

    public Timer(long delay, int repeatCount)
    {
        this.delayTime = delay;
        this.repeatCount = repeatCount;
    }

    public void addOnComplete(TimerHandle onComplete)
    {
        this.onComplete = onComplete;
    }

    public void addOnTimer(TimerHandle onTimer)
    {
        this.onTimer = onTimer;
    }

    private void Dispose()
    {
        this.dispose = true;
    }

    private int getCurrentCount()
    {
        return (int) ((this.currentTime - this.startTime) / this.delayTime);
    }

    public bool isDispose()
    {
        return this.dispose;
    }

    public void reset()
    {
        this.startTime = TimeKit.getMillisTime();
        this.currentCount = 0;
        this.running = true;
    }

    public void start()
    {
        this.start(false);
    }

    public void start(bool firstCall)
    {
        if (firstCall)
        {
            this.currentCount = -1;
        }
        else
        {
            this.currentCount = 0;
        }
        this.running = true;
        this.startTime = TimeKit.getMillisTime();
    }

    public void stop()
    {
        this.running = false;
        this.startTime = 0L;
        this.currentCount = 0;
        this.onTimer = null;
        this.onComplete = null;
        this.Dispose();
    }

    public void update()
    {
        if (this.running)
        {
            this.currentTime = TimeKit.getMillisTime();
            int num = this.getCurrentCount();
            if (num > this.currentCount)
            {
                this.currentCount = num;
                if (this.onTimer != null)
                {
                    this.onTimer();
                }
            }
            if ((this.repeatCount > 0) && (this.currentCount >= this.repeatCount))
            {
                this.stop();
                if (this.onComplete != null)
                {
                    this.onComplete();
                }
                this.dispose = true;
            }
            if ((this.onTimer == null) && (this.onComplete == null))
            {
                this.stop();
            }
        }
    }
}

