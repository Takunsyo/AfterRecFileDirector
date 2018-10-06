using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace RVMCore
{
    /// <summary>
    /// A Mediaplayer based on MCI.
    /// </summary>
    public class MediaPlayer2 : IDisposable
    {
        public const int MM_MCINOTIFY = 953;

        private string fileName;
        /// <summary>
        ///     Gets the full path of current playing media.
        ///     </summary>
        public string MediaFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    return string.Empty;
                return fileName;
            }
        }

        private Size playerSize;
        private IntPtr container;
        private string mediaName;
        private IntPtr notifyForm = IntPtr.Zero;
        private int _Length = -1;

        private int _Volume = 1000;
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string command, StringBuilder returnValue, int returnLength, IntPtr winHandle);

        [Flags]
        public enum PlayState
        {
            Opened,
            Playing,
            Paused,
            Stopped,
            Closed
        }

        private PlayState _State = PlayState.Closed;
        /// <summary>
        ///     Gets the state of current player.
        ///     </summary>
        public PlayState State
        {
            get
            {
                return _State;
            }
            private set
            {
                _State = value;
            }
        }

        /// <summary>
        ///     Initialize a new instance of the <see cref="MediaPlayer"/> class 
        ///     with specific Player<see cref="Control"/>'s handle and size, and specific <see cref="Form"/>'s 
        ///     handle to accept the notice message from MCI.
        ///     </summary>
        ///     <param name="hwnd">A <see cref="Control"/>'s Handle, So MCI can put video on it.</param>
        ///     <param name="size">A <see cref="Control"/>'s Size to adjust the player screen's size.</param>
        ///     <param name="notifyHwnd">A <see cref="Form"/>'s Handle, to accept notice message from MCI
        ///     , if not needed set it to <see cref="IntPtr.Zero"/>.</param>
        public MediaPlayer2(IntPtr hwnd, Size size, IntPtr notifyHwnd)
        {
            container = hwnd;
            playerSize = size;
            notifyForm = notifyHwnd;
            State = PlayState.Closed;
            System.Security.Cryptography.MD5CryptoServiceProvider MD5Provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] buf = BitConverter.GetBytes(container.ToInt32());
            buf = MD5Provider.ComputeHash(buf);
            mediaName = System.Text.Encoding.Default.GetString(buf);
        }

        /// <summary>
        ///     Stop playing and unload media.
        ///     </summary>
        private void ClosePlayer()
        {
            if (this.State.HasFlag(PlayState.Opened))
            {
                string playCommand = "Close " + mediaName;
                mciSendString(playCommand, null, 0, IntPtr.Zero);
                _Length = -1;
                this.State = PlayState.Closed;
            }
        }

        /// <summary>
        ///     Open the file in <see cref="fileName"/> and load it.
        ///     </summary>
        private void OpenMediaFile()
        {
            ClosePlayer();
            string playCommand = string.Format("Open \"{0}\" type mpegvideo alias {1} parent {2} style child", fileName, mediaName, container.ToInt32());
            mciSendString(playCommand, null, 0, IntPtr.Zero);
            playCommand = string.Format("put {0} window at {1} {2} {3} {4}", mediaName, 0, 0, playerSize.Width, playerSize.Height);
            mciSendString(playCommand, null, 0, IntPtr.Zero);
            playCommand = string.Format("setaudio {0} volume to {1}", mediaName, _Volume);
            mciSendString(playCommand, null, 0, IntPtr.Zero);
            playCommand = string.Format("set {0} time format milliseconds", mediaName);
            mciSendString(playCommand, null, 0, IntPtr.Zero);
            this.State = PlayState.Opened;
        }

        /// <summary>
        ///     Start to play media.
        ///     </summary>
        private void PlayMediaFile()
        {
            if (this.State.HasFlag(PlayState.Opened))
            {
                string playCommand = string.Format("play {0} notify", mediaName);
                mciSendString(playCommand, null, 0, notifyForm);
                // Dim playCommand = String.Format("resume {0} notify", mediaName)
                // mciSendString(playCommand, Nothing, 0, notifyForm)
                this.State = PlayState.Playing | PlayState.Opened;
            }
        }

        /// <summary>
        ///     Open a file and play it.
        ///     </summary>
        public void Play(string fileName, bool PlayImidity = true)
        {
            this.fileName = fileName;
            OpenMediaFile();
            if (PlayImidity)
                PlayMediaFile();
        }
        /// <summary>
        ///     Open a file and play it , but last file path of current player's playing media will be returen in OldFile paramater.
        ///     </summary>
        ///     <param name="OldFile">returns current player's playing media full path.</param>
        ///     <param name="NewFile">new media's full path to play.</param>
        public void Play(ref string OldFile, string NewFile, bool PlayImidity = true)
        {
            OldFile = this.fileName;
            Play(NewFile, PlayImidity);
        }

        /// <summary>
        ///     Resume play from pausing.
        ///     </summary>
        public void Play()
        {
            if (this.State.HasFlag(PlayState.Opened))
            {
                string playCommand = string.Format("resume {0}", mediaName);
                mciSendString(playCommand, null, 0, notifyForm);
                this.State = PlayState.Playing | PlayState.Opened;
            }
        }

        /// <summary>
        ///     Pause media, use <see cref="Play()"/> to resume play.
        ///     </summary>
        public void Pause()
        {
            if (this.State.HasFlag(PlayState.Opened))
            {
                string playCommand = string.Format("pause {0}", mediaName);
                mciSendString(playCommand, null, 0, notifyForm);
                this.State = PlayState.Paused | PlayState.Opened;
            }
        }

        /// <summary>
        ///     Stop playing and unload media.
        ///     </summary>
        public void Stop()
        {
            ClosePlayer();
        }

        /// <summary>
        ///     Gets the media length for current player in milliseconds.
        ///     </summary>
        public int Length
        {
            get
            {
                if (this.State.HasFlag(PlayState.Opened))
                {
                    if (_Length <= 0)
                    {
                        StringBuilder Result = new StringBuilder(255);
                        string playCommand = string.Format("status {0} length", mediaName);
                        mciSendString(playCommand, Result, Result.Capacity, IntPtr.Zero);
                        // On Error Resume Next
                        int tmp = 0;
                        if (int.TryParse(Result.ToString(), out tmp))
                            _Length = tmp;
                    }
                    return _Length;
                }
                else
                    return -1;
            }
        }

        /// <summary>
        ///     Gets or Sets playing position for current player in milliseconds.
        ///     </summary>
        public int Position
        {
            get
            {
                if (this.State.HasFlag(PlayState.Opened))
                {
                    StringBuilder Result = new StringBuilder(255);
                    string playCommand = string.Format("status {0} position", mediaName);
                    mciSendString(playCommand, Result, Result.Capacity, IntPtr.Zero);
                    // On Error Resume Next
                    int tmp = 0;
                    if (int.TryParse(Result.ToString(), out tmp))
                        return tmp;
                    else
                        return -1;
                }
                else
                    return -1;
            }
            set
            {
                if (this.State.HasFlag(PlayState.Opened))
                {
                    if (value < 0)
                    if (this.State.HasFlag(PlayState.Playing))
                    {
                        string playCommand = string.Format("play {0} from {1}", mediaName, value.ToString());
                        mciSendString(playCommand, null, 0, IntPtr.Zero);
                    }
                    else
                    {
                        string playCommand = string.Format("seek {0} to {1}", mediaName, value.ToString());
                        mciSendString(playCommand, null, 0, IntPtr.Zero);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets or Sets volume for current player in range of 1 to 1000.
        ///     </summary>
        ///     <exception cref="InvalidOperationException"></exception>
        public int Volume
        {
            get
            {
                return _Volume;
            }
            set
            {
                if (value < 0 | value > 1000)
                    throw new InvalidOperationException();
                _Volume = value;
                if (this.State.HasFlag(PlayState.Opened))
                {
                    string playCommand = string.Format("setaudio {0} volume to {1}", mediaName, value.ToString());
                    mciSendString(playCommand, null, 0, IntPtr.Zero);
                }
            }
        }

        /// <summary>
        ///     Gets or Sets the view size of the current player.
        ///     </summary>
        public Size ScreenSize
        {
            get
            {
                return playerSize;
            }
            set
            {
                playerSize = value;
                var playCommand = string.Format("put {0} window at {1} {2} {3} {4}", mediaName, 0, 0, playerSize.Width, playerSize.Height);
                mciSendString(playCommand, null, 0, IntPtr.Zero);
            }
        }

        private bool disposedValue; // 要检测冗余调用

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (State.HasFlag(PlayState.Opened))
                        Stop();
                }
            }
            disposedValue = true;
        }

        // TODO: 仅当以上 Dispose(disposing As Boolean)拥有用于释放未托管资源的代码时才替代 Finalize()。
        // Protected Overrides Sub Finalize()
        // ' 请勿更改此代码。将清理代码放入以上 Dispose(disposing As Boolean)中。
        // Dispose(False)
        // MyBase.Finalize()
        // End Sub

        // Visual Basic 添加此代码以正确实现可释放模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(disposing As Boolean)中。
            Dispose(true);
        }
    }

}
