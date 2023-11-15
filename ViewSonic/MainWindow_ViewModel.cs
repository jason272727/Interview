using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace ViewSonic
{
    public class MainWindow_ViewModel : ViewModelBase
    {

        public ICommand UserCommand { get; }
        private MainWindow_Model _model;
        private List<Button> ButtonList = new List<Button>();
        
        public string UserAction
        {
            get { return _model.UserAction; }
            set { _model.UserAction = value; OnPropertyChanged(); }
        }
      
        public int ResizingPoint
        {
            get { return _model.ResizingPoint; }
            set { _model.ResizingPoint = value; OnPropertyChanged(); }
        }
       
        public Rectangle Current_Rectangle
        {
            get { return _model.rectangle; }
            set { _model.rectangle = value; OnPropertyChanged(); }
        }
        public Polygon Current_Triangle
        {
            get { return _model.Triangle; }
            set { _model.Triangle = value; OnPropertyChanged(); }
        }
        public Ellipse Current_Ellipse
        {
            get { return _model.ellipse; }
            set { _model.ellipse = value; OnPropertyChanged(); }
        }
       
        public MainWindow_ViewModel()
        {
            _model = new MainWindow_Model();
            UserCommand = new RelayCommand(GetCommand, CanDoIt);
        }
        private void GetCommand(object parameter)
        {
            UserAction = parameter.ToString();
            switch (UserAction)
            {
                case "Select":
                    SetButton("Select_btn");
                    break;

                case "Erase":
                    SetButton("Erase_btn");
                    break;

                case "Rectangle":
                    SetButton("Rectangle_btn");
                    break;

                case "Triangle":
                    SetButton("Triangle_btn");
                    break;

                case "Ellipse":
                    SetButton("Ellipse_btn");
                    break;
            }
        }
        /// <summary>
        /// 初始化，預設"選擇"模式
        /// </summary>
        /// <param name="parameter">Button List</param>
        public void Init(List<Button> parameter)
        {
            ButtonList = parameter;
            ButtonList[1].IsEnabled = false;
            UserAction = "Select";
        }
        /// <summary>
        /// 載入圖片
        /// </summary>
        /// <param name="CurrentCanvas"></param>
        public void LoadImage(Canvas CurrentCanvas)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image File (*.jpg;*.png)|*.jpg;*.png";
            bool? result = openFile.ShowDialog();
            if (result == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFile.FileName, UriKind.Relative));
                ImageBrush brush = new ImageBrush(bitmap);
                CurrentCanvas.Background = brush;
            }
        }
        /// <summary>
        /// 匯出畫布
        /// </summary>
        /// <param name="CurrentCanvas"></param>
        public void ExportCanvas(Canvas CurrentCanvas)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Image File (*.png)|*.png";
            bool? result = saveFile.ShowDialog();
            if (result == true)
            {              
                Size size = new Size(CurrentCanvas.RenderSize.Width, CurrentCanvas.RenderSize.Height); //取得Canvas的size
                CurrentCanvas.Measure(size); //測量UIElement的大小，以利於放置在正確的地方
                CurrentCanvas.Arrange(new Rect(size));//實際調整元素大小、位置
                double bpi = 96d;
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)size.Width, (int)size.Height, bpi, bpi, PixelFormats.Default);  //Canvas轉換成Bitmap               
                rtb.Render(CurrentCanvas);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                var CropBitmap = new CroppedBitmap(rtb, new Int32Rect(0, 0, (int)size.Width, (int)size.Height)); //擷取Canvas範圍
                BitmapEncoder PngEncoder = new PngBitmapEncoder();
                PngEncoder.Frames.Add(BitmapFrame.Create(CropBitmap));//把擷取的影像加入至BitmapEncoder編碼
                using (var file = System.IO.File.OpenWrite(saveFile.FileName))
                {
                    PngEncoder.Save(file);
                }
            }
        }
        /// <summary>
        /// 當使用者點選功能按鈕時，會在這裡做介面Update，主要是要讓使用者知道目前在甚麼功能模式下。
        /// </summary>
        /// <param name="ButtonName"></param>
        private void SetButton(string ButtonName)
        {
            for (int i = 0; i < ButtonList.Count; i++)
            {
                if (ButtonList[i].Name == ButtonName)
                {
                    ButtonList[i].IsEnabled = false;
                }
                else
                {
                    ButtonList[i].IsEnabled = true;
                }
            }
        }
        /// <summary>
        /// 滑鼠在畫布上的點擊事件處理
        /// </summary>
        /// <param name="MouseDownData">滑鼠在畫布上點選時，所產生之對應資料</param>
        public void ButtonDown(UserCanvas_Model MouseDownData)
        {
            switch (UserAction)
            {
                case "Erase":
                    #region 刪除圖形
                    MouseDownData.CurrentCanvas.Children.Remove(MouseDownData.SelectShape);
                    #endregion
                    break;

                case "Rectangle":
                    #region 建立四邊形
                    Point RectangleStartPoint = MouseDownData.StartPoint;
                    Rectangle rectangle = new Rectangle
                    {
                        Width = 0,
                        Height = 0,
                        Stroke = Brushes.Black,
                        StrokeThickness = 3,
                        Fill = Brushes.White
                    };
                    Canvas.SetLeft(rectangle, RectangleStartPoint.X);
                    Canvas.SetTop(rectangle, RectangleStartPoint.Y);
                    Current_Rectangle = rectangle;
                    MouseDownData.CurrentCanvas.Children.Add(rectangle);
                    #endregion
                    break;

                case "Triangle":
                    #region 建立三角形
                    Point TriangleStartPoint = MouseDownData.StartPoint;
                    Polygon polygon = new Polygon
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 3,
                        Fill = Brushes.White
                    };
                    Current_Triangle = polygon;
                    MouseDownData.CurrentCanvas.Children.Add(polygon);
                    #endregion
                    break;

                case "Ellipse":
                    #region 建立圓形
                    Point EllipseStartPoint = MouseDownData.StartPoint;
                    Ellipse ellipse = new Ellipse
                    {
                        Width = 0,
                        Height = 0,
                        Stroke = Brushes.Black,
                        StrokeThickness = 3,
                        Fill = Brushes.White
                    };
                    Canvas.SetLeft(ellipse, EllipseStartPoint.X);
                    Canvas.SetTop(ellipse, EllipseStartPoint.Y);
                    Current_Ellipse = ellipse;
                    MouseDownData.CurrentCanvas.Children.Add(ellipse);
                    #endregion
                    break;
            }
        }
        /// <summary>
        /// 滑鼠移動事件
        /// </summary>
        /// <param name="MouseMoveData">UI相關資訊</param>
        public void MouseMove(UserCanvas_Model MouseMoveData)
        {
            Point StartPoint = MouseMoveData.StartPoint;
            Point EndPoint = MouseMoveData.EndPoint;
            switch (UserAction)
            {
                case "Select":
                    #region 有關圖形的移動，或是形狀、大小的調整處理
                    #region 計算移動距離
                    double X_distance = EndPoint.X - StartPoint.X;
                    double Y_distance = EndPoint.Y - StartPoint.Y;
                    #endregion
                    if (MouseMoveData.SelectAction == "Dragging") //圖形拖動
                    {
                        //判斷是不是三角形(多邊形)
                        if (MouseMoveData.SelectShape is Polygon)
                        {
                            #region 取得三角形上的每個點
                            Point Point1 = (MouseMoveData.SelectShape as Polygon).Points[0];
                            Point Point2 = (MouseMoveData.SelectShape as Polygon).Points[1];
                            Point Point3 = (MouseMoveData.SelectShape as Polygon).Points[2];
                            #endregion
                            #region 目前的位置 + or - 移動距離
                            (MouseMoveData.SelectShape as Polygon).Points[0] = new Point(Point1.X + X_distance, Point1.Y + Y_distance);
                            (MouseMoveData.SelectShape as Polygon).Points[1] = new Point(Point2.X + X_distance, Point2.Y + Y_distance);
                            (MouseMoveData.SelectShape as Polygon).Points[2] = new Point(Point3.X + X_distance, Point3.Y + Y_distance);
                            #endregion
                        }
                        else
                        {
                            #region 如果不是三角形，就直接更動位置
                            Canvas.SetLeft(MouseMoveData.SelectShape, Canvas.GetLeft(MouseMoveData.SelectShape) + X_distance);
                            Canvas.SetTop(MouseMoveData.SelectShape, Canvas.GetTop(MouseMoveData.SelectShape) + Y_distance);
                            #endregion
                        }
                    }
                    else if (MouseMoveData.SelectAction == "Resizing")//圖形大小調整
                    {
                        if (MouseMoveData.SelectShape is Polygon) //是否為三角形
                        {
                            #region 取得三角形上的每個點
                            Point Point1 = (MouseMoveData.SelectShape as Polygon).Points[0];
                            Point Point2 = (MouseMoveData.SelectShape as Polygon).Points[1];
                            Point Point3 = (MouseMoveData.SelectShape as Polygon).Points[2];
                            #endregion
                            switch (ResizingPoint)
                            {
                                #region 針對滑鼠移動的點進位置的更新
                                case 0:
                                    (MouseMoveData.SelectShape as Polygon).Points[0] = new Point(Point1.X + X_distance, Point1.Y + Y_distance);
                                    break;

                                case 1:
                                    (MouseMoveData.SelectShape as Polygon).Points[1] = new Point(Point2.X + X_distance, Point2.Y + Y_distance);
                                    break;

                                case 2:
                                    (MouseMoveData.SelectShape as Polygon).Points[2] = new Point(Point3.X + X_distance, Point3.Y + Y_distance);
                                    break;
                                    #endregion
                            }
                        }
                        else
                        {
                            #region 如果不是三角形，就直接縮放圖形
                            (MouseMoveData.SelectShape as FrameworkElement).Width = Math.Max(0, EndPoint.X - Canvas.GetLeft(MouseMoveData.SelectShape));
                            (MouseMoveData.SelectShape as FrameworkElement).Height = Math.Max(0, EndPoint.Y - Canvas.GetTop(MouseMoveData.SelectShape));
                            #endregion
                        }
                    }
                    #endregion
                    break;

                case "Rectangle":
                    #region 以滑鼠拖拉的方式，建立四邊形
                    Current_Rectangle.Width = Math.Abs(EndPoint.X - StartPoint.X);
                    Current_Rectangle.Height = Math.Abs(EndPoint.Y - StartPoint.Y);
                    double rectangle_left = StartPoint.X < EndPoint.X ? StartPoint.X : EndPoint.X;
                    double rectangle_top = StartPoint.Y < EndPoint.Y ? StartPoint.Y : EndPoint.Y;
                    Canvas.SetLeft(Current_Rectangle, rectangle_left);
                    Canvas.SetTop(Current_Rectangle, rectangle_top);
                    #endregion
                    break;

                case "Triangle":
                    #region 以滑鼠拖拉的方式，建立三角形
                    Current_Triangle.Points.Clear();
                    Current_Triangle.Points.Add(StartPoint);
                    Current_Triangle.Points.Add(new Point(EndPoint.X, StartPoint.Y));
                    Current_Triangle.Points.Add(new Point((StartPoint.X + EndPoint.X) / 2, EndPoint.Y));
                    #endregion
                    break;

                case "Ellipse":
                    #region 以滑鼠拖拉的方式，建立圓形
                    Current_Ellipse.Width = Math.Abs(EndPoint.X - StartPoint.X);
                    Current_Ellipse.Height = Math.Abs(EndPoint.Y - StartPoint.Y);
                    double ellipse_left = StartPoint.X < EndPoint.X ? StartPoint.X : EndPoint.X;
                    double ellipse_top = StartPoint.Y < EndPoint.Y ? StartPoint.Y : EndPoint.Y;
                    Canvas.SetLeft(Current_Ellipse, ellipse_left);
                    Canvas.SetTop(Current_Ellipse, ellipse_top);
                    #endregion
                    break;
            }
        }

        public bool CanDoIt(object parameter)
        {
            //預設永遠可以執行
            return true;
        }

        /// <summary>
        /// 判斷使用者滑鼠是不是點在圖形邊線上(主要判斷使用者是不是想要調整圖形大小)
        /// </summary>
        public bool CheckResize(UserCanvas_Model CheckData)
        {
            if (CheckData.SelectShape is Polygon)
            {
                #region 取得三角形上三個頂點
                double Point1_X = (CheckData.SelectShape as Polygon).Points[0].X;
                double Point1_Y = (CheckData.SelectShape as Polygon).Points[0].Y;
                double Point2_X = (CheckData.SelectShape as Polygon).Points[1].X;
                double Point2_Y = (CheckData.SelectShape as Polygon).Points[1].Y;
                double Point3_X = (CheckData.SelectShape as Polygon).Points[2].X;
                double Point3_Y = (CheckData.SelectShape as Polygon).Points[2].Y;
                #endregion
                #region 使用者點選範圍內的+-15
                if (CheckData.UserPoint.X < Point1_X + 15 && CheckData.UserPoint.Y < Point1_Y + 15 && CheckData.UserPoint.X > Point1_X - 15 && CheckData.UserPoint.Y > Point1_Y - 15)
                {
                    ResizingPoint = 0;
                    return true;
                }
                else if (CheckData.UserPoint.X < Point2_X + 15 && CheckData.UserPoint.Y < Point2_Y + 15 && CheckData.UserPoint.X > Point2_X - 15 && CheckData.UserPoint.Y > Point2_Y - 15)
                {
                    ResizingPoint = 1;
                    return true;
                }
                else if (CheckData.UserPoint.X < Point3_X + 15 && CheckData.UserPoint.Y < Point3_Y + 15 && CheckData.UserPoint.X > Point3_X - 15 && CheckData.UserPoint.Y > Point3_Y - 15)
                {
                    ResizingPoint = 2;
                    return true;
                }
                else
                {
                    return false;
                }
                #endregion
            }
            else
            {
                #region 取得左上與右下座標
                double left = Canvas.GetLeft(CheckData.SelectShape);
                double top = Canvas.GetTop(CheckData.SelectShape);
                double right = left + CheckData.SelectShape.RenderSize.Width;
                double bottom = top + CheckData.SelectShape.RenderSize.Height;
                #endregion
                #region 使用者點選範圍內的+-15
                if (CheckData.UserPoint.X < left + 15 || CheckData.UserPoint.Y < top + 15 || CheckData.UserPoint.X > right - 15 || CheckData.UserPoint.Y > bottom - 15)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                #endregion
            }
        }
        /// <summary>
        /// 針對圖形屬性修改
        /// </summary>
        /// <param name="UserSelectShape"></param>
        public void EditSelectShapeAttribute(UserCanvas_Model UserSelectShape)
        {
            #region 取得目前使用者點選物件的屬性
            //有關顏色的資訊在這邊只能取出顏色代碼，所以還需要透過GetColorName()來轉換得知代碼所代表的顏色，以利於後續操作的便利性。
            var FillColorCode = UserSelectShape.SelectShape.GetType().GetProperty("Fill").GetValue(UserSelectShape.SelectShape).ToString();
            var BorderColorCode = UserSelectShape.SelectShape.GetType().GetProperty("Stroke").GetValue(UserSelectShape.SelectShape).ToString();
            EditAttributeData.StrokeThick = int.Parse(UserSelectShape.SelectShape.GetType().GetProperty("StrokeThickness").GetValue(UserSelectShape.SelectShape).ToString());            
            EditAttributeData.FillColorName = GetColorName(FillColorCode);
            EditAttributeData.StrokeColorName = GetColorName(BorderColorCode);
            #endregion
            AttributesAdjust EditAttribute = new AttributesAdjust();          
            EditAttribute.ShowDialog();
            UpdateShapeAttribute(UserSelectShape.SelectShape);
        }
        /// <summary>
        /// 色碼轉換(Color Code -> Color Name)
        /// </summary>
        /// <param name="ColorCode"></param>
        /// <returns></returns>
        private string GetColorName(string ColorCode)
        {
            Color color = (Color)ColorConverter.ConvertFromString(ColorCode);
            foreach (var property in typeof(Colors).GetProperties())
            {
                if ((Color)property.GetValue(null) == color)
                {
                    return property.Name;
                }
            }
            return "Null";
        }
        /// <summary>
        /// 更新所選取物件的屬性
        /// </summary>
        /// <param name="SelectShape">所選取的物件(直接把EditSelectShapeAttribute的UIElement帶入近來)</param>
        private void UpdateShapeAttribute(UIElement SelectShape)
        {
            #region 把變動的顏色進行轉譯(Color Code -> Brushes)
            Color FillColor = (Color)ColorConverter.ConvertFromString(EditAttributeData.FillColorName);
            Color StrokeColor = (Color)ColorConverter.ConvertFromString(EditAttributeData.StrokeColorName);
            Brush FillColorBrush = new SolidColorBrush(FillColor);
            Brush StrokeColorBrush = new SolidColorBrush(StrokeColor);
            #endregion
            switch (SelectShape.GetType().Name)
            {
                #region 屬性更新
                case "Rectangle":
                    Rectangle rectangle = (Rectangle)SelectShape;
                    rectangle.Fill = FillColorBrush;
                    rectangle.Stroke = StrokeColorBrush;
                    rectangle.StrokeThickness = EditAttributeData.StrokeThick;
                    break;

                case "Polygon":
                    Polygon triangle = (Polygon)SelectShape;
                    triangle.Fill = FillColorBrush;
                    triangle.Stroke = StrokeColorBrush;
                    triangle.StrokeThickness = EditAttributeData.StrokeThick;
                    break;

                case "Ellipse":
                    Ellipse ellipse = (Ellipse)SelectShape;
                    ellipse.Fill = FillColorBrush;
                    ellipse.Stroke = StrokeColorBrush;
                    ellipse.StrokeThickness = EditAttributeData.StrokeThick;
                    break;
                    #endregion
            }
        }
    }
}