using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RVMCore.Forms
{
    [ToolboxBitmap(typeof(System.Windows.Forms.ProgressBar))]
    public class ProgressBarKai : System.Windows.Forms.ProgressBar
    {
        public delegate void StateChangedHandler(object source, vState State);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        static extern uint SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        private vState _State = vState.Normal;

        public enum vState { Normal, Pause, Error }

        private const int WM_USER = 0x400;
        private const int PBM_SETSTATE = WM_USER + 16;

        private const int PBST_NORMAL = 0x0001;
        private const int PBST_ERROR = 0x0002;
        private const int PBST_PAUSED = 0x0003;

        [Category("Behavior")]
        [Description("Event raised when the state of the Control is changed.")]
        public event StateChangedHandler StateChanged;

        [Category("Behavior")]
        [Description("This property allows the user to set the state of the ProgressBar.")]
        [DefaultValue(vState.Normal)]
        public vState State
        {
            get
            {
                if (Environment.OSVersion.Version.Major < 6)
                    return vState.Normal;
                if (this.Style == System.Windows.Forms.ProgressBarStyle.Blocks) return _State;
                else return vState.Normal;
            }
            set
            {
                _State = value;
                if (this.Style == System.Windows.Forms.ProgressBarStyle.Blocks)
                    ChangeState(_State);
            }
        }
        [Category("Appearance")]
        [Description("This is the text you want to print to ProgressBar.")]
        [DefaultValue("")]
        public override string Text { get; set; }
        [Category("Appearance")]
        [Description("This is the text you want to print to ProgressBar.")]
        public override Font Font { get => base.Font; set => base.Font = value; }

        private void ChangeState(vState State)
        {
            if (Environment.OSVersion.Version.Major > 5)
            {
                SendMessage(this.Handle, PBM_SETSTATE, PBST_NORMAL, 0);

                switch (State)
                {
                    case vState.Pause:
                        SendMessage(this.Handle, PBM_SETSTATE, PBST_PAUSED, 0);
                        break;
                    case vState.Error:
                        SendMessage(this.Handle, PBM_SETSTATE, PBST_ERROR, 0);
                        break;
                    default:
                        SendMessage(this.Handle, PBM_SETSTATE, PBST_NORMAL, 0);
                        break;
                }
                if (StateChanged != null)
                    StateChanged(this, State);
            }
        }
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == 15)
                ChangeState(_State);
            base.WndProc(ref m);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var mSize = e.Graphics.MeasureString(this.Text, this.Font);

            var mPos = new PointF((this.Height - mSize.Height) / 2, (this.Width - mSize.Width) / 2);

            e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(Color.Black), mPos);
        }
    }
}

