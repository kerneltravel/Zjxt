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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.Windows.Markup;

namespace 中医证治智能系统
{
    /// <summary>
    /// Interaction logic for selectbs.xaml
    /// </summary>
    public partial class selectbs : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 创建集合实例
        ObservableCollection<WesternMedicineInfo> listSymptom = new ObservableCollection<WesternMedicineInfo>();

        // 创建对 PassValuesHandler 方法的引用的类
        public delegate void PassValuesHandler(object sender, PassValuesEventArgs e);
        // 声明事件
        public event PassValuesHandler PassValuesEvent;
        // 创建事件数据类
        public class PassValuesEventArgs : EventArgs
        {
            private string _name;
            private string _number;
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public string Number
            {
                get { return _number; }
                set { _number = value; }
            }

            public PassValuesEventArgs(string name, string number)
            {
                this.Name = name;
                this.Number = number;
            }
        }
        public selectbs()
        {
            InitializeComponent();
            lv.ItemsSource = listSymptom;
            string sql = String.Format("select * from t_info_zxmx as t1 inner join t_info_zxxx as t2 on t1.zxbh=t2.zxbh where t2.zxlxbh='56'");  // 新的症象分类，病史为56号
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            listSymptom.Clear();
            while (dr.Read())
            {

                listSymptom.Add(new WesternMedicineInfo("病史", dr["zxbh"].ToString(), dr["zxmc"].ToString()));
            }
            dr.Close();
            conn.Close();
        }
        /// <summary>
        /// 功能：创建症象信息类
        /// 说明：1.SymptomTypes-->症象类型名称 SymptomNumber-->症象编号 SymptomName-->症象名称
        ///       2.
        /// </summary>
        public class WesternMedicineInfo : INotifyPropertyChanged
        {
            #region INotifyPropertyChanged 成员
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, e);
                }
            }
            #endregion

            private string _WesternMedicineType;
            private string _WesternMedicineNumber;
            private string _WesternMedicineName;

            public string WesternMedicineType
            {
                get { return _WesternMedicineType; }
                set { _WesternMedicineType = value; OnPropertyChanged(new PropertyChangedEventArgs("WesternMedicineType")); }
            }

            public string WesternMedicineNumber
            {
                get { return _WesternMedicineNumber; }
                set { _WesternMedicineNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("WesternMedicineNumber")); }
            }

            public string WesternMedicineName
            {
                get { return _WesternMedicineName; }
                set { _WesternMedicineName = value; OnPropertyChanged(new PropertyChangedEventArgs("WesternMedicineName")); }
            }

            public WesternMedicineInfo(string WesternMedicinetype, string WesternMedicinenumber, string WesternMedicinename)
            {
                _WesternMedicineType = WesternMedicinetype;
                _WesternMedicineNumber = WesternMedicinenumber;
                _WesternMedicineName = WesternMedicinename;
            }
        }

        private void back_search_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MessageBox.Show("没有病史被选择！");
        }

        private void select_search_Click(object sender, RoutedEventArgs e)
        {
            WesternMedicineInfo basicbingji = lv.SelectedItem as WesternMedicineInfo;
            if (basicbingji != null && basicbingji is WesternMedicineInfo)
            {
                PassValuesEventArgs args = new PassValuesEventArgs(basicbingji.WesternMedicineName.ToString(), basicbingji.WesternMedicineNumber.ToString());
                PassValuesEvent(this, args);
            }
            this.Close();
        }

        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
