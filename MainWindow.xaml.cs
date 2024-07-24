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

namespace signwriter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.SaveFileDialog dlgSave = new Microsoft.Win32.SaveFileDialog();

            dlgSave.Filter = "ビットマップファイル(*.bmp)|*.bmp|" +
            "JPEGファイル(*.jpg)|*,jpg|" +
            "PNGファイル(*.png)|*.png";
            dlgSave.AddExtension = true;

            if ((bool)dlgSave.ShowDialog())
            {

                string extension = System.IO.Path.GetExtension(dlgSave.FileName).ToUpper();

                Rect rectBounds = InkCanvas.Strokes.GetBounds();

                DrawingVisual dv = new DrawingVisual();
                DrawingContext dc = dv.RenderOpen();
                dc.PushTransform(new TranslateTransform(-rectBounds.X, -rectBounds.Y));

                dc.DrawRectangle(InkCanvas.Background, null, rectBounds);

                InkCanvas.Strokes.Draw(dc);
                dc.Close();

                RenderTargetBitmap rtb = new RenderTargetBitmap(
                    (int)rectBounds.Width, (int)rectBounds.Height,
                    96, 96,
                    PixelFormats.Default);
                rtb.Render(dv);

                BitmapEncoder enc = null;

                switch (extension)
                {
                    case ".BMP":
                        enc = new BmpBitmapEncoder();
                        break;
                    case ".JPG":
                        enc = new JpegBitmapEncoder();
                        break;
                    case ".PNG":
                        enc = new PngBitmapEncoder();
                        break;
                }

                if (enc != null)
                {
                    enc.Frames.Add(BitmapFrame.Create(rtb));
                    System.IO.Stream stream = System.IO.File.Create(dlgSave.FileName);
                    enc.Save(stream);
                    stream.Close();
                }

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InkCanvas.Strokes.Clear();
        }

        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            InkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }

        private void WriteButton_Click(object sender, RoutedEventArgs e)
        {
            InkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }
    }
}
