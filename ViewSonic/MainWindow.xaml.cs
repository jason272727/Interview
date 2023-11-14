using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace ViewSonic
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {       
        private bool Dragging = false;
        private bool Resizing = false;
        private Point StartPoint_Model;                
        private UIElement SelectShape_Model;      
        List<Button> ButtonList = new List<Button>();
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindow_ViewModel ViewModel)
            {
                ButtonList.Add(Erase_btn);
                ButtonList.Add(Select_btn);
                ButtonList.Add(Rectangle_btn);
                ButtonList.Add(Triangle_btn);
                ButtonList.Add(Ellipse_btn);
                ViewModel.Init(ButtonList);
            }
        }
       
        /// <summary>
        /// 使用者在畫布上的點擊事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainWindow_ViewModel ViewModel)
            {
                StartPoint_Model = e.GetPosition(UserCanvas);//取得滑鼠點擊的初始位置
                switch (ViewModel.UserAction)
                {
                    case "Select":                       
                       
                        HitTestResult hitTestResult = VisualTreeHelper.HitTest(UserCanvas, StartPoint_Model); //嘗試判斷使用者點選位置，是否為圖形
                        if (hitTestResult != null && hitTestResult.VisualHit is UIElement) //判斷是否圖形和UIE物件
                        {
                            #region 如果使用者在Select模式下，點選了某個圖形物件，則把其物件與使用者點擊位置記錄下來，並將資料傳送至ViewModel做進一步判斷與處理
                            SelectShape_Model = (UIElement)hitTestResult.VisualHit; //暫存物件
                            UserCanvas_Model CheckClickBorder = new UserCanvas_Model
                            {
                                SelectShape = SelectShape_Model,
                                UserPoint = StartPoint_Model
                            };
                            Resizing = ViewModel.CheckResize(CheckClickBorder);//判斷使用者這否點擊在圖形邊上(如果是的話，代表使用者想要縮放此圖形)                           
                            #endregion
                            if (!Resizing)
                            {
                                #region 如果不是點擊在物件的邊框上，就代表單純要拖拉物件位置，所以進行以下設定。
                                Dragging = true;
                                SelectShape_Model.CaptureMouse();
                                #endregion
                            }
                        }
                        break;

                    case "Erase":
                        HitTestResult DelectShape = VisualTreeHelper.HitTest(UserCanvas, StartPoint_Model); //嘗試判斷使用者點選位置，是否為圖形
                        if (DelectShape != null && DelectShape.VisualHit is UIElement) //判斷是否圖形和UIE物件
                        {
                            #region 把所選取的物件資料記錄下來，並交由ViewModel來處理後續動作
                            UserCanvas_Model DeleteData = new UserCanvas_Model
                            {
                                CurrentCanvas = UserCanvas,
                                SelectShape = (UIElement)DelectShape.VisualHit
                            };
                            ViewModel.ButtonDown(DeleteData);
                            #endregion
                        }
                        break;

                    case "Rectangle":                       
                        Dragging = true;
                        #region 把所選取的物件資料記錄下來，並交由ViewModel來處理後續動作
                        UserCanvas_Model MouseDownCreateRectangle = new UserCanvas_Model
                        {                          
                            CurrentCanvas=UserCanvas,
                            StartPoint = e.GetPosition(UserCanvas)                            
                        };
                        ViewModel.ButtonDown(MouseDownCreateRectangle);
                        #endregion
                        break;

                    case "Triangle":
                        Dragging = true;
                        #region 把所選取的物件資料記錄下來，並交由ViewModel來處理後續動作
                        UserCanvas_Model MouseDownCreateTriangle = new UserCanvas_Model
                        {
                            CurrentCanvas = UserCanvas,
                            StartPoint = e.GetPosition(UserCanvas)
                        };
                        ViewModel.ButtonDown(MouseDownCreateTriangle);
                        #endregion
                        break;

                    case "Ellipse":
                        Dragging = true;
                        #region 把所選取的物件資料記錄下來，並交由ViewModel來處理後續動作
                        UserCanvas_Model MouseDownCreateEllipse = new UserCanvas_Model
                        {
                            CurrentCanvas = UserCanvas,
                            StartPoint = e.GetPosition(UserCanvas)
                        };
                        ViewModel.ButtonDown(MouseDownCreateEllipse);
                        #endregion
                        break;
                }
            }       
        }

       
        /// <summary>
        /// 滑鼠在畫布上拖拉事件處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserCanvas_MouseMove(object sender, MouseEventArgs e)
        {           
            if (DataContext is MainWindow_ViewModel ViewModel)
            {
                switch (ViewModel.UserAction)
                {
                    case "Select":                        
                        Point EndPoint = e.GetPosition(UserCanvas);                        
                        if (Dragging)
                        {
                            #region 代表使用者單純移動圖形，所以把該圖形的資訊記錄下來，並交由ViewModel做後續處理
                            UserCanvas_Model UserMoveShape = new UserCanvas_Model
                            {
                                SelectAction = "Dragging",
                                StartPoint = StartPoint_Model,
                                EndPoint = e.GetPosition(UserCanvas),
                                SelectShape = SelectShape_Model
                            };
                            ViewModel.MouseMove(UserMoveShape);
                            #endregion
                        }
                        else if (Resizing)
                        {
                            #region 代表使用者要縮放圖形，所以把該圖形的資訊記錄下來，並交由ViewModel做後續處理
                            UserCanvas_Model UserResizingShape = new UserCanvas_Model
                            {
                                SelectAction = "Resizing",
                                StartPoint = StartPoint_Model,
                                EndPoint = e.GetPosition(UserCanvas),
                                SelectShape = SelectShape_Model
                            };
                            ViewModel.MouseMove(UserResizingShape);
                            #endregion
                        }
                        StartPoint_Model = EndPoint;                      
                        break;

                    case "Rectangle":
                        if (Dragging)
                        {
                            #region 代表使用者正以滑鼠拖拉的方式，建立四邊形。所以把相關資訊記錄下來，並交由ViewModel做後續處理
                            UserCanvas_Model MouseMoveCreateRectangle = new UserCanvas_Model
                            {
                                SelectShape = SelectShape_Model,
                                StartPoint = StartPoint_Model,
                                EndPoint = e.GetPosition(UserCanvas)
                            };
                            ViewModel.MouseMove(MouseMoveCreateRectangle);
                            #endregion
                        }
                        break;

                    case "Triangle":
                        if (Dragging)
                        {
                            #region 代表使用者正以滑鼠拖拉的方式，建立三角形。所以把相關資訊記錄下來，並交由ViewModel做後續處理
                            UserCanvas_Model MouseMoveCreateTriangle = new UserCanvas_Model
                            {
                                StartPoint = StartPoint_Model,
                                EndPoint = e.GetPosition(UserCanvas)
                            };
                            ViewModel.MouseMove(MouseMoveCreateTriangle);
                            #endregion
                        }
                        break;

                    case "Ellipse":
                        if (Dragging)
                        {
                            #region 代表使用者正以滑鼠拖拉的方式，建立圓形。所以把相關資訊記錄下來，並交由ViewModel做後續處理
                            UserCanvas_Model MouseMoveCreateEllipse = new UserCanvas_Model
                            {
                                SelectShape = SelectShape_Model,
                                StartPoint = StartPoint_Model,
                                EndPoint = e.GetPosition(UserCanvas)
                            };
                            ViewModel.MouseMove(MouseMoveCreateEllipse);
                            #endregion
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 滑鼠放掉後的處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainWindow_ViewModel ViewModel)
            {
                if ((Dragging == true) && (ViewModel.UserAction != "Select")) //代表使用者用滑鼠拖拉方式建立物件
                {
                    Dragging = false;                   
                }
                else //代表使用者在縮放圖形
                {
                    if (SelectShape_Model != null)
                    {
                        SelectShape_Model.ReleaseMouseCapture();
                    }                    
                    Dragging = false;
                    Resizing = false;
                }
            }
         
        }
        /// <summary>
        /// 針對圖形點擊右鍵，代表針對該圖形進行屬性的變動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainWindow_ViewModel ViewModel)
            {
                Point UserSelectPoint = e.GetPosition(UserCanvas);
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(UserCanvas, UserSelectPoint); //嘗試判斷使用者點選位置，是否為圖形
                if (hitTestResult != null && hitTestResult.VisualHit is UIElement) //判斷是否圖形和UIE物件
                {
                    #region 把所選取的物件，交由給ViewModel做處理
                    UIElement UserSelectShape = (UIElement)hitTestResult.VisualHit;
                    UserCanvas_Model UserSelectShapeData = new UserCanvas_Model
                    {
                        SelectShape = UserSelectShape
                    };
                    ViewModel.EditSelectShapeAttribute(UserSelectShapeData);
                    #endregion
                }
            }
           
        }
    }
}
