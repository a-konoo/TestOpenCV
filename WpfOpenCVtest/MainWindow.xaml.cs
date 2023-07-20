using System;
using System.Collections.Generic;
using System.Drawing;
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
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.WpfExtensions;
using Window = System.Windows.Window;

namespace WpfOpenCVtest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var src_mat = new Mat(@"C:\Temp\graph.png"))
            {
                var gray_mat = new Mat();
                Cv2.CvtColor(src_mat, gray_mat, ColorConversionCodes.BGR2GRAY);

                var bw_mat = new Mat();
                Cv2.Threshold(gray_mat, bw_mat, 192, 255, ThresholdTypes.BinaryInv);

                OpenCvSharp.Point[][] contours;
                OpenCvSharp.HierarchyIndex[] hindex;
                Cv2.FindContours(bw_mat, out contours, out hindex, RetrievalModes.Tree,
                    ContourApproximationModes.ApproxSimple);
                var x1List = new List<int>();
                var x2List = new List<int>();
                var y1List = new List<int>();
                var y2List = new List<int>();
                foreach (var cntr in contours)
                {
                    var ret = Cv2.BoundingRect(cntr);
                    x1List.Add(ret.TopLeft.X);
                    x2List.Add(ret.BottomRight.X);
                    y1List.Add(ret.TopLeft.Y);
                    y2List.Add(ret.BottomRight.Y);
                }
                var minX = x1List.Min<int>();
                var maxX = x2List.Max<int>();
                var minY = y1List.Min<int>();
                var maxY = y2List.Max<int>();

                //Cv2.DrawContours(src_mat, contours, -1, new Scalar(0, 0, 255), 2);
                //Cv2.Rectangle(src_mat, new OpenCvSharp.Point(minX, minY), 
                //    new OpenCvSharp.Point(maxX, maxY), new Scalar(0, 255, 0), 3);
                var mat2 = src_mat.Clone(new OpenCvSharp.Rect(minX, minY, maxX - minX, maxY - minY));
                // 余白除去後の画像
                //var dst_bitmap = BitmapSourceConverter.ToBitmapSource(mat2);

                var gray_mat_Inner = new Mat();
                Cv2.CvtColor(mat2, gray_mat_Inner, ColorConversionCodes.BGR2GRAY);

                var bw_mat_Inner = new Mat();
                Cv2.Threshold(gray_mat_Inner, bw_mat_Inner, 192, 255, ThresholdTypes.BinaryInv);
                OpenCvSharp.Point[][] contourInners;
                OpenCvSharp.HierarchyIndex[] hindexInners;
                var lines = Cv2.HoughLinesP(bw_mat_Inner, 1, Math.PI / 360, 100, 30, 0);
                /*
                Cv2.FindContours(bw_mat_Inner, out contourInners, out hindexInners, RetrievalModes.Tree,
                    ContourApproximationModes.ApproxSimple);
                */
                var x1List2 = new List<int>();
                var x2List2 = new List<int>();
                var y1List2 = new List<int>();
                var y2List2 = new List<int>();
                
                foreach (var line in lines)
                {
                    var x1 = line.P1.X;
                    var x2 = line.P2.X;
                    if (Math.Abs(x2-x1) < 3)
                    {
                        // 縦線扱い
                        Cv2.Line(mat2, line.P1.X, line.P1.Y, line.P2.X, line.P2.Y, new Scalar(0, 255, 0), 3);
                    }
                }
                /*
                var minX2 = x1List2.Min<int>();
                var maxX2 = x2List2.Max<int>();
                var minY2 = y1List2.Min<int>();
                var maxY2 = y2List2.Max<int>();
                var minXX2 = x1List2.Where(x => x > minX2).Min();
                var maxXX2 = x2List2.Where(x2 => x2 < maxX2).Max();
                var minYY2 = y1List2.Where(x => x > y1List2.Min<int>()).Min();
                var maxYY2 = y2List2.Where(x2 => x2 < y2List2.Max<int>()).Max();
                */
                // 余白除去後の画像
                //Cv2.Rectangle(mat2, new OpenCvSharp.Point(minXX2, minYY2), 
                //    new OpenCvSharp.Point(maxXX2, maxYY2), new Scalar(0, 255, 0), 3);
                var dst_bitmap = BitmapSourceConverter.ToBitmapSource(mat2);
                imgDst.Source = dst_bitmap;
            }
        }
    }
}
