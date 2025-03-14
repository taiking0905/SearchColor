using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace SearchColor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Boolean qflag = false;
        private DispatcherTimer _timer;
        public MainWindow()
        {
            InitializeComponent();
            LoadColorsFromCsv();
            SetWindowProperties();
            InitializeTimer();
        }
        // ウィンドウのプロパティを設定
        private void SetWindowProperties()
        {
            this.Topmost = true;
            this.Left = 0;
            this.Top = 0;
            this.Activated += Window_Activated;
            this.Deactivated += Window_Deactivated;
        }
        // タイマーを初期化
        private void InitializeTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            var animation = new System.Windows.Media.Animation.ColorAnimation
            {
                From = System.Windows.Media.Color.FromRgb(240, 240, 240), // カスタムの明るいグレー
                To = Colors.White, // 白
                Duration = TimeSpan.FromMilliseconds(300)
            };
            MainGrid.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 240, 240));
            (MainGrid.Background as SolidColorBrush)?.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            var animation = new System.Windows.Media.Animation.ColorAnimation
            {
                From = Colors.White,
                To = System.Windows.Media.Color.FromRgb(240, 240, 240),
                Duration = TimeSpan.FromMilliseconds(300)
            };
            MainGrid.Background = new SolidColorBrush(Colors.White);
            (MainGrid.Background as SolidColorBrush)?.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Q)
                {
                    if (qflag)
                        qflag = false;
                    else
                        qflag = true;
                    //Console.WriteLine("Qキーが押されました。qflag = true");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"例外が発生しました: {ex.Message}");
            }
        }


        // タイマーのTickイベントでマウス位置を取得
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!qflag)
            {
                // 画面全体のマウス位置を取得
                var mousePosition = System.Windows.Forms.Cursor.Position;

                // Bitmapの作成
                Bitmap bmp = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                // Graphicsの作成
                Graphics g = Graphics.FromImage(bmp);

                // マウス座標にある画面をコピー
                g.CopyFromScreen(mousePosition.X, mousePosition.Y, 0, 0, new System.Drawing.Size(1, 1));

                // ピクセルの色を取得
                System.Drawing.Color pixelColor = bmp.GetPixel(0, 0);

                // Graphics解放
                g.Dispose();

                CustomColorConverter.RGBToHSV(pixelColor.R, pixelColor.G, pixelColor.B, out double h, out double s, out double v);

                // マウスの位置を表示
                maustext.Text = $"Mouse Position (Screen): X = {mousePosition.X:F0}, Y = {mousePosition.Y:F0}" +
                     $"RGB({pixelColor.R:F0}, {pixelColor.G:F0}, {pixelColor.B:F0})";

                // H (Hue) の値を更新
                if (HueText != null) // UIが初期化される前に呼ばれる場合のガード
                    HueText.Text = $"色相(Hue): {h:F1}";
                if (HueSlider != null)
                    HueSlider.Value = h;

                // S (Saturation) の値を更新
                if (SaturationText != null)
                    SaturationText.Text = $"彩度(Saturation): {s:F3}";
                if (SaturationSlider != null)
                    SaturationSlider.Value = s;


                // V (Value) の値を更新
                if (ValueText != null)
                    ValueText.Text = $"明度(Value): {v:F3}";
                if (ValueSlider != null)
                    if (SaturationSlider != null)
                        ValueSlider.Value = v;


                mainColor.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(pixelColor.R, pixelColor.G, pixelColor.B));
            }

        }


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // H (Hue) の値を更新
            if (HueText != null) // UIが初期化される前に呼ばれる場合のガード
                HueText.Text = $"色相(Hue): {HueSlider.Value:F1}";

            // S (Saturation) の値を更新
            if (SaturationText != null)
                SaturationText.Text = $"彩度(Saturation): {SaturationSlider.Value:F3}";

            // V (Value) の値を更新
            if (ValueText != null)
                ValueText.Text = $"明度(Value): {ValueSlider.Value:F3}";

            // HSVの値を取得
            double h = HueSlider.Value;        // Hue (0-360)
            double s = SaturationSlider.Value; // Saturation (0-1)
            double v = ValueSlider.Value;      // Value (0-1)


            // HSV→RGB変換
            System.Windows.Media.Color color = CustomColorConverter.HSVToRGB(h, s, v);

            // Rectangleに色を適用
            mainColor.Fill = new SolidColorBrush(color);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

            Close();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // HSVの値を取得
            double h = HueSlider.Value;        // Hue (0-360)
            double s = SaturationSlider.Value; // Saturation (0-1)
            double v = ValueSlider.Value;      // Value (0-1)

            System.Windows.Media.Color color = CustomColorConverter.HSVToRGB(h, s, v);

            // RGB値を16進数に変換
            string newColorText = $"#{color.R:X2}{color.G:X2}{color.B:X2}";

            ColorFileManager.SaveColor(newColorText);

            // クリップボードにコピー
            System.Windows.Clipboard.SetText(newColorText);

            // ウィンドウを閉じる
            Close();
        }


        private void LoadColorsFromCsv()
        {
            // CSVファイルのパス
            string csvFilePath = "oldcolor.csv";


            // CSVファイルが存在しない場合、新しいファイルを作成
            if (!File.Exists(csvFilePath))
            {
                // CSVファイルが存在しない場合、新しいファイルを作成
                if (!File.Exists(csvFilePath))
                {
                    // 初期色データを設定（例: 空白の色データを3つ入れる）
                    string[] defaultColors = new string[] { "#FF0000", "#00FF00", "#0000FF" };
                    File.WriteAllLines(csvFilePath, defaultColors);
                }
            }

            // CSVファイルから色を読み取る
            if (File.Exists(csvFilePath))
            {
                string[] colors = File.ReadAllLines(csvFilePath);

                // 各Rectangleに色を適用
                if (colors.Length > 0) oldColor1.Fill = ConvertHexToBrush(colors[2]);
                if (colors.Length > 1) oldColor2.Fill = ConvertHexToBrush(colors[1]);
                if (colors.Length > 2) oldColor3.Fill = ConvertHexToBrush(colors[0]);
            }

        }

        private SolidColorBrush ConvertHexToBrush(string hex)
        {
            // 前後の空白を除去してからColorに変換
            return new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex.Trim()));
        }

    }

}

