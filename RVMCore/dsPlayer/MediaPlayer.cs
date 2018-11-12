using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RVMCore.DirectShowLib
{
    public class MediaPlayer  : IDisposable
    {
        private const int WMGraphNotify = 0x0400 + 13;
        private IntPtr notifyTarget;
        private IntPtr owner;

        //private FilterGraph fg;
        private IGraphBuilder graphBuilder;
        private IMediaControl mediaControl;
        private IVideoWindow videoWindow;
        private IMediaEventEx mediaEventEx;
        private IMediaSeeking mediaSeeking;
        private IMediaPosition mediaPosition;
        private IBasicAudio basicAudio;
        private IBasicVideo basicVideo;
        private IVideoFrameStep frameStep;

        #region"Properties"
        /// <summary>
        ///     Gets the full path of current playing media.
        ///     </summary>
        public string MediaFilePath { get { return this.FilePath; } }
        public string FilePath { get; set; } = "";
        /// <summary>
        ///     Gets the state of current player.
        ///     </summary>
        public PlayState State { get;private set; } = PlayState.Closed;
        /// <summary>
        ///     Gets the media length for current player in milliseconds.
        ///     </summary>
        public int Length { get; private set; }
        
        /// <summary>
        ///     Gets or Sets playing position for current player in milliseconds.
        ///     </summary>
        public int Position {
            get
            {
                if (this.State.HasFlag(PlayState.Opened))
                {
                    double time;
                    int hr=mediaPosition.get_CurrentPosition(out time);
                    DsError.ThrowExceptionForHR(hr);
                    return (int)(time*1000);
                }
                return 0;
            }
            set
            {
                if (this.State.HasFlag(PlayState.Opened))
                {
                    if (value < 0) return;
                    double time = (double)(value/1000)/this.Position;
                    int hr = mediaPosition.put_CurrentPosition(time);
                    //DsError.ThrowExceptionForHR(hr);
                }
            }
        } //need to be program.
        /// <summary>
        ///     Gets or Sets volume for current player in range of 1 to 100.
        ///     </summary>
        ///     <exception cref="InvalidOperationException"></exception>
        public int Volume {
            get
            {
                int vol;
                this.basicAudio.get_Volume(out vol);
                vol += 5000;
                return vol/100;
            }
            set
            {
                if (value >= 0 && value <= 100) {
                    int vol = value * 100 -5000;
                    this.basicAudio.put_Volume(vol);
                }
                else { throw new InvalidOperationException("Value is out of range."); }
            }
        }

        public bool FullScreen {
            get
            {
                OABool vol;
                int hr = this.videoWindow.get_FullScreenMode(out vol);
                DsError.ThrowExceptionForHR(hr);
                return vol.Equals(OABool.True) ? true : false;
            }
            set
            {
                OABool vol = value ? OABool.True : OABool.False;
                int hr = this.videoWindow.put_FullScreenMode(vol);
                DsError.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        ///     Gets or Sets the view size of the current player.
        ///     </summary>
        public Size ScreenSize
        {
            get
            {
                int x,y = 0;
                videoWindow.get_Width(out x);
                videoWindow.get_Height(out y);
                return new Size(x,y);
            }
            set
            {
                int pre_h = value.Height;
                int pre_w = value.Width;
                int ori_h = this.VideoSize.Height;
                int ori_w = this.VideoSize.Width;
                int tmp_w = pre_h * ori_w / ori_h;
                int tmp_h = pre_w * ori_h / ori_w;
                if (tmp_w > pre_w)
                {
                    pre_h = tmp_h;
                    tmp_w = pre_h * ori_w / ori_h;
                
                }
                videoWindow.SetWindowPosition(0, 0, tmp_w , tmp_h   );
            }
        }
        private Size _ScreenSize;

        public Size VideoSize { get; private set; }

        public double AspetRatio
        {
            get
            {
                return this.VideoSize.Width / this.VideoSize.Height;
            }
        }
        #endregion

        public MediaPlayer(IntPtr hwnd, Size size)
        {
            this._ScreenSize = size;
            this.owner = hwnd;
            this.notifyTarget = hwnd;
            //this.hr = graphBuilder.RenderFile(null, null);
            //videoWindow.put_WindowStyle(WindowStyle.Child);
            //videoWindow.SetWindowPosition(0,0,size.Width,size.Height);
            //videoWindow.put_Owner(hwnd);
        }


        private int InitVideoWindow(int nMultiplier, int nDivider)
        {
            int hr = 0;
            int lHeight, lWidth;
            if (this.basicVideo == null)
                return 0;
            // Read the default video size
            hr = this.basicVideo.GetVideoSize(out lWidth, out lHeight);
            if (hr == DsResults.E_NoInterface)
                return 0;
            // Account for requests of normal, half, or double size
            lWidth = lWidth * nMultiplier / nDivider;
            lHeight = lHeight * nMultiplier / nDivider;
            Control.FromHandle(owner).ClientSize = new Size(lWidth, lHeight + 75);
            Application.DoEvents();
            hr = this.videoWindow.SetWindowPosition(0, 30, lWidth, lHeight);
            return hr;
        }

        private void CloseInterfaces()
        {
            int hr = 0;
            try
            {
                lock (this)
                {
                    // Relinquish ownership (IMPORTANT!) after hiding video window
                    hr = this.videoWindow.put_Visible(OABool.False);
                    DsError.ThrowExceptionForHR(hr);
                    hr = this.videoWindow.put_Owner(IntPtr.Zero);
                    DsError.ThrowExceptionForHR(hr);
                    if (this.mediaEventEx != null)
                    {
                        hr = this.mediaEventEx.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);
                        DsError.ThrowExceptionForHR(hr);
                    }
                    // Release and zero DirectShow interfaces
                    if (this.mediaEventEx != null)
                        this.mediaEventEx = null;
                    if (this.mediaSeeking != null)
                        this.mediaSeeking = null;
                    if (this.mediaPosition != null)
                        this.mediaPosition = null;
                    if (this.mediaControl != null)
                        this.mediaControl = null;
                    if (this.basicAudio != null)
                        this.basicAudio = null;
                    if (this.basicVideo != null)
                        this.basicVideo = null;
                    if (this.videoWindow != null)
                        this.videoWindow = null;
                    if (this.frameStep != null)
                        this.frameStep = null;
                    if (this.graphBuilder != null)
                        Marshal.ReleaseComObject(this.graphBuilder); this.graphBuilder = null;
                    GC.Collect();
                }
            }
            catch
            {
                //what to do?
            }
        }

        /// <summary>
        ///     Stop playing and unload media.
        ///     </summary>
        private void ClosePlayer()
        {
            if (this.State.HasFlag(PlayState.Opened))
            {
                this.mediaControl.Stop();
                CloseInterfaces();
                this.Length = -1;
                this.State = PlayState.Closed;
            }
        }
        /// <summary>
        /// Open the file in <see cref="FilePath"/> and load it.
        /// </summary>
        private void OpenMediaFile()
        {
            ClosePlayer();
            int hr = 0;
            this.graphBuilder = (IGraphBuilder)new FilterGraph();
            hr = graphBuilder.RenderFile(this.FilePath, null);
            DsError.ThrowExceptionForHR(hr);
            this.mediaControl = (IMediaControl)this.graphBuilder;
            this.mediaEventEx = (IMediaEventEx)this.graphBuilder;
            this.mediaSeeking = (IMediaSeeking)this.graphBuilder;
            this.mediaPosition = (IMediaPosition)this.graphBuilder;

            this.videoWindow = this.graphBuilder as IVideoWindow;
            this.basicVideo = this.graphBuilder as IBasicVideo;
            int x, y;
            this.basicVideo.GetVideoSize(out x, out y);
            this.VideoSize = new Size(x, y);
            this.basicAudio = (IBasicAudio)this.graphBuilder;
            hr = this.mediaEventEx.SetNotifyWindow(notifyTarget, WMGraphNotify, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);
            hr = this.videoWindow.put_Owner(owner);
            DsError.ThrowExceptionForHR(hr);
            hr = this.videoWindow.put_WindowStyle(WindowStyle.Child |
                    WindowStyle.ClipSiblings | WindowStyle.ClipChildren);
            DsError.ThrowExceptionForHR(hr);
            videoWindow.SetWindowPosition(0, 0, Control.FromHandle(this.owner).Width, Control.FromHandle(this.owner).Height);
            double time;
            mediaPosition.get_Duration(out time);
            this.Length = (int)(time*1000);

            this.State = PlayState.Opened;
        }
        /// <summary>
        ///     Start to play media.
        ///     </summary>
        private void PlayMediaFile()
        {
            if (this.State.HasFlag(PlayState.Opened))
            {
                this.mediaControl.Run();
                this.State = PlayState.Playing | PlayState.Opened;
            }
        }
        /// <summary>
        ///     Open a file and play it.
        ///     </summary>
        public void Play(string fileName, bool PlayImidity = true)
        {
            this.FilePath = fileName;
            OpenMediaFile();
            if (PlayImidity)
                PlayMediaFile();
        }
        /// <summary>
        ///     Open a file and play it , but last file path of current player's playing media will be returen in OldFile paramater.
        ///     </summary>
        ///     <param name="OldFile">returns current player's playing media full path.</param>
        ///     <param name="NewFile">new media's full path to play.</param>
        public void Play(out string OldFile, string NewFile, bool PlayImidity = true)
        {
            OldFile = this.FilePath;
            Play(NewFile, PlayImidity);
        }
        /// <summary>
        ///     Resume play from pausing.
        ///     </summary>
        public void Play()
        {
            if (this.State.HasFlag(PlayState.Opened))
            {
                mediaControl.Run();
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
                int hr = 0;
                hr = this.mediaControl.Pause();
                DsError.ThrowExceptionForHR(hr);
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

        public void Dispose()
        {
            this.ClosePlayer();
        }
    }
    [Flags]
    public enum PlayState
    {
        Closed  =1,
        Opened  =2,
        Playing =4,
        Paused  =8,
        Stopped =16
    }

}
