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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using ABB.Robotics.Controllers.Configuration;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers;
using RobotStudio.Services.RobApi;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Data;
using RobotStudio.Services.RobApi.RobApi1;
using static System.Net.Mime.MediaTypeNames;


namespace Painting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WriteableBitmap wb;
        ABBRobot Robot;
        NetworkScanner Netscaner { get; set; }
        ControllerInfoCollection Controllers { get; set; }
        bool paint;
        List<Point3d> positions;

        public MainWindow()
        {
            InitializeComponent();

            Robot = new ABBRobot();
            positions = new List<Point3d>();

            wb = new WriteableBitmap((int)img.Width, (int)img.Height, 96, 96, PixelFormats.Bgra32, null); 
            img.Source = wb;

            NetScan();
        }

        private void NetScan()
        {
            Netscaner = new NetworkScanner();
            Netscaner.Scan();
            Controllers = Netscaner.Controllers;

            foreach (ControllerInfo c in Controllers)
            {
                cbox_Controllers.Items.Add(c);
            }
        }

        private void btnLoadImg_Click(object sender, RoutedEventArgs e)
        {
            rtbPoints.Document.Blocks.Clear();


            if (ImageConversion.CreateMapFromImg(ref wb, "img.bmp"))
            {
                img.Source = wb;

                var SPoint = ImageConversion.FindStartPoint();

                positions = ImageConversion.CreateWay(SPoint);

                foreach (var pos in positions)
                {
                    rtbPoints.AppendText("\r" + pos.ToString());
                }

                lbItems.Content = positions.Count.ToString() + " items";
            }
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            wb.Clear(Colors.White);
            rtbPoints.Document.Blocks.Clear();
            lbItems.Content = "0";
        }
        private void btnToRapid_Click(object sender, RoutedEventArgs e)
        {
            if (Robot != null)
            {

                foreach (var pos in positions)
                {
                    var item = "\r" + pos.X + ", " + pos.Y + ", " + pos.Z;
                    rtbPoints.AppendText(item);
                }
                Robot.Move(positions);

                positions.Clear();
            }
        }

        private void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            wb.Clear(Colors.White);
            
            var x = (int)e.GetPosition(img).X;
            var y = (int)e.GetPosition(img).Y;

            if (Robot != null)
            {
                paint = true;

                positions.Add(new Point3d(x, y, 0));
                rtbPoints.Document.Blocks.Clear();

            }
        }
        private void img_MouseMove(object sender, MouseEventArgs e)
        {
            var x = (int)e.GetPosition(img).X;
            var y = (int)e.GetPosition(img).Y;

            if (paint)
            {
                wb.FillEllipseCentered(x, y, 1, 1, Colors.Blue);
                positions.Add(new Point3d(x, y, 0));
            }

            lbXY.Content = x + ", " + y;
        }
        private void img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var x = (int)e.GetPosition(img).X;
            var y = (int)e.GetPosition(img).Y;

            if (Robot != null)
            {
                paint = false;
                positions.Add(new Point3d(x, y, -50));

                foreach (var pos in positions)
                {
                    var item = "\r" + pos.X + ", " + pos.Y + ", " + pos.Z;
                    rtbPoints.AppendText(item);
                }

                lbItems.Content = positions.Count.ToString() + " items";

                Robot.Move(positions);

                positions.Clear();
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e) => Robot.StartExec();
        private void btnStop_Click(object sender, RoutedEventArgs e) => Robot.StopExec();

        private void cbox_Controllers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var comboBoxControllers = sender as ComboBox;
                Robot.Connect((ControllerInfo)comboBoxControllers?.SelectedItem);
                lbSystem.Content = Robot.SelectedController.SystemName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
