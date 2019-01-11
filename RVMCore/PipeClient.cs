using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using Newtonsoft.Json;

namespace RVMCore
{
    /// <summary>
    /// A named pipe client object. for sending information to <see cref="NamedPipeServerStream"/> equipped process.
    /// </summary>
    /// <typeparam name="T">The Mid object type. This type must can be serialize to Json objects.</typeparam>
    public class PipeClient<T>
    {
        /// <summary>
        /// Send a Mid object typeof <typeparamref name="T"/> to a named pipe server.
        /// </summary>
        /// <param name="SendObj"></param>
        /// <param name="PipeName"></param>
        /// <param name="TimeOut"></param>
        public void Send(T SendObj, string PipeName, int TimeOut = 1000)
        {
            try
            {
                NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", PipeName,
                    PipeDirection.Out, PipeOptions.Asynchronous);

                // The connect function will indefinitely wait for the pipe to become available
                // If that is not acceptable specify a maximum waiting time (in ms)
                pipeStream.Connect(TimeOut);
                Debug.WriteLine("[Client] Pipe connection established");
                string SendStr = JsonConvert.SerializeObject(SendObj);
                byte[] _buffer = Encoding.UTF8.GetBytes(SendStr);
                pipeStream.BeginWrite(_buffer, 0, _buffer.Length, AsyncSend, pipeStream);
                //pipeStream.Write(_buffer, 0, _buffer.Length);
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
                Debug.WriteLine("[Client] Pipe successfully send data.");
                pipeStream.Flush();
                pipeStream.Close();
                pipeStream.Dispose();
            }
            catch (Exception oEX)
            {
                Debug.WriteLine(oEX.Message);
            }
        }

        /// <summary>
        /// Create mid object instance using a Json string. 
        /// <para>*This method is for test purpose only.</para>
        /// </summary>
        [Obsolete("Do NOT use this in production.", false)]
        public static T GetMidObjectFromString(string json)
        {
            if (!json.IsNullOrEmptyOrWhiltSpace())
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch
                {
                    return default;
                }
            }
            else
                return default;
        }
    }
}
