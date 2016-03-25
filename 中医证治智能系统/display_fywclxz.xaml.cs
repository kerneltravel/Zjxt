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

namespace 中医证治智能系统
{
    /// <summary>
    /// Interaction logic for display_fywclxz.xaml
    /// </summary>
    public partial class display_fywclxz : Window
    {
        string selectIndex = "0";
        // 创建对 PassValuesHandler 方法的引用的类
        public delegate void PassValuesHandler(object sender, PassValuesEventArgs e);
        // 声明事件
        public event PassValuesHandler PassValuesEvent;
        // 创建事件数据类
        public class PassValuesEventArgs : EventArgs
        {
            private string _number;

            public string Number
            {
                get { return _number; }
                set { _number = value; }
            }

            public PassValuesEventArgs(string number)
            {
                this.Number = number;
            }
        }

        public display_fywclxz()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 选择反药物处理方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Manual.IsChecked == true)
                selectIndex = "1";
            if (Auto.IsChecked == true)
                selectIndex = "2";
            if (AllReserved.IsChecked == true)
                selectIndex = "3";
            PassValuesEventArgs args = new PassValuesEventArgs(selectIndex);
            this.Close();
            // 要判断 PassValuesEvent 是否为空，即判断该窗口是否被调用
            if (PassValuesEvent != null)
            {
                PassValuesEvent(this, args);
            }
        }

        /// <summary>
        /// 人工处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Manual_Click(object sender, RoutedEventArgs e)
        {
            if (Manual.IsChecked == true)
            {
                Auto.IsEnabled = false;
                AllReserved.IsEnabled = false;
            }
            else
            {
                Auto.IsEnabled = true;
                AllReserved.IsEnabled = true; 
            }
        }

        /// <summary>
        /// 系统自动处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Auto_Click(object sender, RoutedEventArgs e)
        {
            if (Auto.IsChecked == true)
            {
                Manual.IsEnabled = false;
                AllReserved.IsEnabled = false;
            }
            else
            {
                Manual.IsEnabled = true;
                AllReserved.IsEnabled = true;
            }
        }

        /// <summary>
        /// 全部保留
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllReserved_Click(object sender, RoutedEventArgs e)
        {
            if (AllReserved.IsChecked == true)
            {
                Manual.IsEnabled = false;
                Auto.IsEnabled = false;
            }
            else
            {
                Manual.IsEnabled = true;
                Auto.IsEnabled = true;
            }
        }
    }
}
