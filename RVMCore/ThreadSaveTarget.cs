using System;
using System.Threading;

namespace RVMCore
{
    //internal class ThreadSaveTarget:  IDisposable
    //{
    //    private Thread thread;

    //    private CancellationTokenSource TokenSource;

    //    public CancellationToken Token => TokenSource.Token;

    //    public ManualResetEvent ResetEvent { get; }

    //    public void Start()
    //    {
    //        if (thread is null) throw new ObjectDisposedException(nameof(thread) + "is disposed or never set a value!");
    //        thread.Start();
    //    }

    //    public void SetWork(ThreadStart threadStart)
    //    {
    //        if(TokenSource is null || ResetEvent is null)
    //            throw new ObjectDisposedException("Object is disposed");
    //        this.thread = new Thread(threadStart);
    //    }

    //    public void Cancel()
    //    {
    //        this.TokenSource.Cancel();
    //        this.Dispose(true);
    //    }
        
    //    public void Pause()=> ResetEvent.Reset();

    //    public void Resume()=> ResetEvent.Set();

    //    public void Join() => thread?.Join();

    //    public bool Join(int timeout) => thread?.Join(timeout) ?? false;

    //    public bool Join(TimeSpan timeout)=> thread?.Join(timeout) ??false;

    //    public bool IsAlive => thread?.IsAlive ?? false;

    //    public ThreadState ThreadState => thread?.ThreadState ?? ThreadState.Unstarted;

    //    public bool IsBackground
    //    {
    //        get
    //        {
    //            return this.thread?.IsBackground ?? false;
    //        }
    //        set
    //        {
    //            if(!(thread is null))
    //            {
    //                this.thread.IsBackground = value;
    //            }
    //            else {
    //                throw new ObjectDisposedException(nameof(thread) + "is disposed or never set a value!");
    //            }
    //        }
    //    }

    //    public ThreadSaveTarget(Action action)
    //    {
    //        if (action is null) throw new NullReferenceException("Paramater 'action' cannot be null.");
    //        var start = new ThreadStart(() => {
    //            action.Invoke();
    //            this.Dispose(true);
    //        });
    //        thread = new Thread(start);
    //        TokenSource = new CancellationTokenSource();
    //        ResetEvent = new ManualResetEvent(true);
    //    }

    //    public ThreadSaveTarget()
    //    {
    //        TokenSource = new CancellationTokenSource();
    //        ResetEvent = new ManualResetEvent(true);
    //    }
    //    #region IDisposable Support
    //    private bool disposedValue = false; 

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (!disposedValue)
    //        {
    //            if (disposing)
    //            {
    //                TokenSource.Dispose();
    //                ResetEvent.Dispose();
    //            }
    //            this.thread = null;
    //            disposedValue = true;
    //        }
    //    }

    //    void IDisposable.Dispose()
    //    {
    //        Dispose(true);
    //    }
    //    #endregion
    //}
}
