using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RVMCore.Forms;

namespace RVMCore.MirakurunWarpper
{
    /// <summary>
    /// MirakurunViewer.xaml 的交互逻辑
    /// </summary>
    public partial class MirakurunViewer : Window
    {
        MirakurunViewerView mView;
        public MirakurunViewer()
        {
            InitializeComponent();
            mView = new MirakurunViewerView();
            this.DataContext = mView;
            
        }

        public void listbox_dblClick(object sender, EventArgs e)
        {
            mView.ChangeChannel();
            MediaPlayer player = new MediaPlayer();
            player.Open(mView.ViewUri);
            VideoDrawing drawing = new VideoDrawing();
            drawing.Rect = new Rect(0, 0, 300, 200);
            drawing.Player = player;
            player.Play();
            DrawingBrush brush = new DrawingBrush(drawing);
            this.PlayerBack.Background = brush;
        }
    }
}
