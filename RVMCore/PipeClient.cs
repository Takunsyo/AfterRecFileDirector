using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace RVMCore
{
    public class PipeClient<T>
    {
        public void Send(T SendObj, string PipeName, int TimeOut = 1000)
        {
            try
            {
                NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", PipeName, PipeDirection.Out, PipeOptions.Asynchronous);

                // The connect function will indefinitely wait for the pipe to become available
                // If that is not acceptable specify a maximum waiting time (in ms)
                pipeStream.Connect(TimeOut);
                Debug.WriteLine("[Client] Pipe connection established");
                string SendStr = JsonConvert.SerializeObject(SendObj);
                byte[] _buffer = Encoding.UTF8.GetBytes(SendStr);
                //pipeStream.BeginWrite(_buffer, 0, _buffer.Length, AsyncSend, pipeStream);
                pipeStream.Write(_buffer, 0, _buffer.Length);
            }
            catch (TimeoutException oEX)
            {
                Debug.WriteLine(oEX.Message);
            }
        }

        private void AsyncSend(IAsyncResult iar)
        {
            try
            {
                // Get the pipe
                NamedPipeClientStream pipeStream = (NamedPipeClientStream)iar.AsyncState;

                // End the write
                pipeStream.EndWrite(iar);
                pipeStream.Flush();
                pipeStream.Close();
                pipeStream.Dispose();
            }
            catch (Exception oEX)
            {
                Debug.WriteLine(oEX.Message);
            }
        }
    }
}
