using Microsoft.Win32;
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
using System.Diagnostics;

namespace ImageViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        int scaleValue = 0;
        double[] scaleList = { 1, 2, 4, 6, 8, 12 };
        double scale;
        double angle = 0;
        const double MaxScale = 10;
        const double MinScale = 1;
        double RectDownX;
        double RectDownY;
        double DownX;
        double DownY;

        public MainWindow()
        {
            InitializeComponent();
            ScrollViewImage.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            ScrollViewImage.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            RectDownX = 0;
            RectDownY = 0;
            DownX = 0;
            DownY = 0;
            scaleValue = 0;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string fileName;
            if ((fileName = OpenDialog("jpgファイル(*.jpg)|*.jpg")) != null){
                SetPoto(fileName);
            }
            MagnificationView();

        }
        // 全体
        private void Overall_Click(object sender, RoutedEventArgs e)
        {
            OverallView();
        }

        // 等倍
        private void Magnification_Click(object sender, RoutedEventArgs e)
        {
            MagnificationView();
        }

        // 拡大
        private void Expansion_Click(object sender, RoutedEventArgs e)
        {
            if (ViewImage.Source == null)
            {
                return;
            }
            ScrollViewImage.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            ScrollViewImage.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

            scaleValue++;
            if (scaleValue >= scaleList.Count())
            {
                scaleValue = scaleList.Count() - 1;
            }
            ImageTransform(scaleList[scaleValue]);
        }

        // 縮小
        private void Shrinking_Click(object sender, RoutedEventArgs e)
        {
            if (ViewImage.Source == null)
            {
                return;
            }
            scaleValue--;
            if (scaleValue <= 0)
            {
                ScrollViewImage.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                ScrollViewImage.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                scaleValue = 0;
            }
            ImageTransform(scaleList[scaleValue]);

        }

        // 右回転
        private void RightRotation_Click(object sender, RoutedEventArgs e)
        {
            if (ViewImage.Source == null)
            {
                return;
            }
            angle += 90;
            if (angle >= 360)
            {
                angle = 0;
            }
            ImageTransform(scaleList[scaleValue]);
        }

        // 左回転
        private void LeftRotation_Click(object sender, RoutedEventArgs e)
        {
            if (ViewImage.Source == null)
            {
                return;
            }
            angle -= 90;
            if (angle <= -360)
            {
                angle = 0;
            }
            ImageTransform(scaleList[scaleValue]);

        }

        /// <summary>
        ///  ダイアログ表示
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private string OpenDialog(string filter)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "ファイルを開く";
            dialog.Filter = filter;
            if ((bool)dialog.ShowDialog())
            {
                return dialog.FileName;
            }
            return null;
        }

        /// <summary>
        /// 画像設定
        /// </summary>
        /// <param name="filePath"></param>
        private void SetPoto(string filePath)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.None;
            image.UriSource = new Uri(filePath);
            image.EndInit();
            image.Freeze();
            
            ViewImage.Source = image;
        }

        /// <summary>
        /// 等倍表示
        /// </summary>
        private void OverallView()
        {
            if (ViewImage.Source == null)
            {
                return;
            }

            ScrollViewImage.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            ScrollViewImage.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

            ViewImage.Width = ViewImage.Source.Width;
            ViewImage.Height = ViewImage.Source.Height;
        }

        /// <summary>
        /// 全体表示
        /// </summary>
        private void MagnificationView()
        {
            if (ViewImage.Source == null)
            {
                return;
            }

            ScrollViewImage.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            ScrollViewImage.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scaleValue = 0;
            ImageTransform(scaleList[scaleValue]);
        }

        /// <summary>
        /// ウィンドウのサイズ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewImage.Width = ScrollGrid.ActualWidth;
            ScrollViewImage.Height = ScrollGrid.ActualHeight;
            ScrollViewImage.UpdateLayout();
            if (scaleValue != 0)
            {
                return;
            }

            ImageTransform(scaleList[scaleValue]);
        }

        /// <summary>
        /// 画像変換
        /// </summary>
        private void ImageTransform(double s)
        {
            Debug.WriteLine(" ScrollViewImage.ExtentWidth" + ScrollViewImage.ExtentWidth + " ScrollViewImage.ExtentHeight" + ScrollViewImage.ExtentHeight);
            scale = s;
            Debug.WriteLine("scale" + scale);
            Set();

            rt.Angle = angle;
            st.ScaleX = s;
            st.ScaleY = s;

            ViewImage.UpdateLayout();


            ScrollViewImage.ScrollToHorizontalOffset(ScrollViewImage.ScrollableWidth / 2);
            ScrollViewImage.ScrollToVerticalOffset(ScrollViewImage.ScrollableHeight / 2);

            Debug.WriteLine(" ScrollViewImage.ExtentWidth" + ScrollViewImage.ExtentWidth + " ScrollViewImage.ExtentHeight" + ScrollViewImage.ExtentHeight);
            Debug.WriteLine("");
        }

        /// <summary>
        /// ウィンドウのロード完了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewImage_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewImage.Width = ScrollViewImage.ActualWidth;
            ScrollViewImage.Height = ScrollViewImage.ActualHeight;

        }

        /// <summary>
        /// 高さ固定で横サイズ自動決定
        /// </summary>
        private void Set()
        {
            if (angle == 90 || angle == 270)
            {
                ViewImage.Width = ScrollViewImage.ActualHeight;
                ViewImage.Height = double.NaN;
            }
            else
            {
                ViewImage.Height = ScrollViewImage.ActualHeight;
                ViewImage.Width = double.NaN;
            }


        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 画像上の座標取得（拡大前の座標）
            Point p = e.GetPosition(ViewImage);
            if (p.X < 0 || p.X > ViewImage.ActualWidth
                || p.Y < 0 || p.Y > ViewImage.ActualHeight)
            {
                return;
            }
            double offsetWidth = 0;
            double offsetHeight = 0;

            if (ScrollViewImage.ActualWidth >= ScrollViewImage.ExtentWidth)
            {
                offsetWidth = (ScrollViewImage.ActualWidth - ViewImage.ActualWidth) / 2;
            }
            if (ScrollViewImage.ActualHeight >= ScrollViewImage.ExtentHeight)
            {
                offsetHeight = (ScrollViewImage.ActualHeight - ViewImage.ActualHeight) / 2;
            }

            RectDownX = p.X * scale + offsetWidth;
            RectDownY = p.Y * scale + offsetHeight;
            DownX = p.X * scale;
            DownY = p.Y * scale;

            // 矩形初期化
            ViewRectangle.Margin = new Thickness(RectDownX, RectDownY, 0, 0);
            ViewRectangle.Width = 0;
            ViewRectangle.Height = 0;
            ViewRectangle.Visibility = Visibility.Visible;

            Debug.WriteLine("downX:" + DownX + " downY:" + DownY);
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(ViewImage);
            // 矩形非表示
            ViewRectangle.Width = 0;
            ViewRectangle.Height = 0;
            ViewRectangle.Visibility = Visibility.Hidden;
            Debug.WriteLine("upX:" + (p.X * scale) + " upY:" + (p.Y * scale));
            ExpandingWithDrag(p);


        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (ViewRectangle.Visibility != Visibility.Visible)
            {
                return;
            }
            // 矩形サイズ設定

            double offsetWidth = 0;
            double offsetHeight = 0;

            if (ScrollViewImage.ActualWidth >= ScrollViewImage.ExtentWidth)
            {
                offsetWidth = (ScrollViewImage.ActualWidth - ViewImage.ActualWidth) / 2;
            }
            if (ScrollViewImage.ActualHeight >= ScrollViewImage.ExtentHeight)
            {
                offsetHeight = (ScrollViewImage.ActualHeight - ViewImage.ActualHeight) / 2;
            }
            Point p = e.GetPosition(ViewImage);
            double rectX = p.X * scale + offsetWidth;
            double rectY = p.Y * scale + offsetHeight;

            if (rectX - RectDownX >= 0 && rectY - RectDownY >= 0)
            {
                // マウスダウン座標から見て右下
                ViewRectangle.Margin = new Thickness(RectDownX, RectDownY, 0, 0);
                ViewRectangle.Width = rectX - RectDownX;
                ViewRectangle.Height = rectY - RectDownY;
            }
            else if (rectX - RectDownX < 0 && rectY - RectDownY >= 0)
            {
                // マウスダウン座標から見て左下
                ViewRectangle.Margin = new Thickness(rectX, RectDownY, 0, 0);
                ViewRectangle.Width = RectDownX - rectX;
                ViewRectangle.Height = rectY - RectDownY;
            }
            else if (rectX - RectDownX >= 0 && rectY - RectDownY < 0)
            {
                // マウスダウン座標から見て右上
                ViewRectangle.Margin = new Thickness(RectDownX, rectY, 0, 0);
                ViewRectangle.Width = rectX - RectDownX;
                ViewRectangle.Height = RectDownY - rectY;
            }
            else
            {
                // マウスダウン座標から見て左上
                ViewRectangle.Margin = new Thickness(rectX, rectY, 0, 0);
                ViewRectangle.Width = Math.Abs(RectDownX - rectX);
                ViewRectangle.Height = Math.Abs(RectDownY - rectY);
            }

        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            // 矩形非表示
            ViewRectangle.Width = 0;
            ViewRectangle.Height = 0;
            ViewRectangle.Visibility = Visibility.Hidden;
        }

        private void ExpandingWithDrag(Point p)
        {
            double upX = p.X * scale;
            double upY = p.Y * scale;
            double width = DownX > upX ? DownX - upX : upX - DownX; 
            double height = DownY > upY ? DownY - upY : upY - DownY;

            Debug.WriteLine("width:" + width + " height:" + height);

            double s;
            if (width > height)
            {
                s = ScrollViewImage.ExtentWidth / width;
            }
            else
            {
                s = ScrollViewImage.ExtentHeight / height;
            }

            if (s >= scaleList[scaleList.Count()- 1])
            {
                s = scaleList[scaleList.Count() - 1];
                return;
            }
            ScrollViewImage.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            ScrollViewImage.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            SetScaleValue(s);
            ImageTransform(s);

        }

        private void SetScaleValue(double s)
        {
            for (int i = scaleList.Count() - 1; i >= 0; i--)
            {
                if (s >= scaleList[i])
                {
                    scaleValue = i;
                    break;
                }
            }
        }
    }
}
