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

namespace ViewSonic
{
    /// <summary>
    /// AttributesAdjust.xaml 的互動邏輯
    /// </summary>
    public partial class AttributesAdjust : Window
    {
        
        public AttributesAdjust()
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
            //初始化與擷取所選取物件的屬性
            FillColorSelector_Combo.ItemsSource = typeof(Brushes).GetProperties();
            BorderColorSelector_Combo.ItemsSource = typeof(Brushes).GetProperties();
            var ColorList = typeof(Brushes).GetProperties().ToList();
            int DefaultFillColor = ColorList.FindIndex(x => x.Name == EditAttributeData.FillColorName);
            int DefaultBorderColor = ColorList.FindIndex(x => x.Name == EditAttributeData.StrokeColorName);
            BorderThick_txt.Text = EditAttributeData.StrokeThick.ToString();
            FillColorSelector_Combo.SelectedIndex = DefaultFillColor;
            BorderColorSelector_Combo.SelectedIndex = DefaultBorderColor;

        }

        private void Save_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditAttributeData.FillColorName = FillColorSelector_Combo.SelectedItem.GetType().GetProperty("Name").GetValue(FillColorSelector_Combo.SelectedItem).ToString();
                EditAttributeData.StrokeColorName = BorderColorSelector_Combo.SelectedItem.GetType().GetProperty("Name").GetValue(BorderColorSelector_Combo.SelectedItem).ToString();
                EditAttributeData.StrokeThick = int.Parse(BorderThick_txt.Text);
            }
            catch
            {
                this.Close();
            }
            this.Close();
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
