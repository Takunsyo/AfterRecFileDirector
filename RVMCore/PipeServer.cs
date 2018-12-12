using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using Newtonsoft.Json;

namespace RVMCore
{
    /// <summary>
    /// A pipe Server using <see cref="NamedPipeServerStream"/> *Test object for Async callback usage.
    /// </summary>
    /// <typeparam name="T">The Message object type for Server to listen.</typeparam>
    public class PipeServer<T> : IDisposable
    {
        public delegate void MessageHandler(T message);
        public event MessageHandler PipeMessage;
        string _pipeName;
        private NamedPipeServerStream mPipe;

        public PipeServer(string PipeName )
        {
            if (PipeName.IsNullOrEmptyOrWhiltSpace())
                throw new ArgumentNullException("PipeName cannot be null or empty space.");
            this._pipeName = PipeName;
        }

        public bool StartListen()
        {
            if(mPipe ==null) 
            try
            {
                // Create the new async pipe 
                mPipe = new NamedPipeServerStream(_pipeName, 
                    PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                // Wait for a connection
                mPipe.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), mPipe);
                    return true;
            }
            catch (Exception oEX)
            {
                Debug.WriteLine(oEX.Message);
            }
            return false;
        }

        private void WaitForConnectionCallBack(IAsyncResult iar)
        {
            NamedPipeServerStream pipe = (NamedPipeServerStream)iar.AsyncState;
            try
            {
                // Get the pipe
                // End waiting for the connection
                pipe.EndWaitForConnection(iar);
                byte[] buffer = new byte[255];
                // Read the incoming message
                pipe.Read(buffer, 0, 255);
                // Convert byte buffer to string
                string stringData = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                Debug.WriteLine(stringData);

                // Pass message back to calling form
                T message = JsonConvert.DeserializeObject<T>(stringData);
                PipeMessage.Invoke(message);
            }
            catch(Exception ex)
            {
                ex.Message.ErrorLognConsole();
                return;
            }
            finally
            {
                // Kill original sever and create new wait server
                pipe.Close();
                pipe = null;
                pipe = new NamedPipeServerStream(_pipeName, 
                    PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                // Recursively wait for the connection again and again....
                pipe.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipe);
            }
        }

        public void StopListen()
        {
            if (this.mPipe != null)
            this.mPipe.Close();
        }

        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            if (this.mPipe != null)
                mPipe.Dispose(); this.IsDisposed = true;
        }
    }
}
