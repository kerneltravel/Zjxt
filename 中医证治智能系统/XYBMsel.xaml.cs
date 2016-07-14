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
    /// Interaction logic for XYBMsel.xaml
    /// </summary>
    public partial class XYBMsel : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        ObservableCollection<UserInfo> listCustomer = new ObservableCollection<UserInfo>();
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
        public XYBMsel()
        {
            InitializeComponent();
        }
        public class UserInfo : INotifyPropertyChanged
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
            private string _bingjiNumber;
            private string _bingjiName;
            private string _type;

            public string BingjiNumber
            {
                get { return _bingjiNumber; }
                set { _bingjiNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("BingjiNumber")); }
            }

            public string BingjiName
            {
                get { return _bingjiName; }
                set { _bingjiName = value; OnPropertyChanged(new PropertyChangedEventArgs("BingjiName")); }
            }

            public string type
            {
                get { return _type; }
                set { _type = value; OnPropertyChanged(new PropertyChangedEventArgs("type")); }
            }

            public UserInfo(string type, string bingjinumber, string bingjiname)
            {
                _bingjiNumber = bingjinumber;
                _bingjiName = bingjiname;
                _type = type;
            }
        }
        private void back_search_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MessageBox.Show("没有西医病名被选择！");
        }

        private void select_search_Click(object sender, RoutedEventArgs e)
        {
            UserInfo basicbingji = lv.SelectedItem as UserInfo;
            if (basicbingji != null && basicbingji is UserInfo)
            {
                PassValuesEventArgs args = new PassValuesEventArgs(basicbingji.BingjiName.ToString(), basicbingji.BingjiNumber.ToString());
                PassValuesEvent(this, args);
            }
            this.Close();
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            listCustomer.Clear();
            if (combobox_xybmlx_input.Text.Trim() == "")
            {
                string sql = String.Format("select t2.xybmlxmc,t1.xybmbh,t1.xybmmc from t_info_xybm as t1 inner join t_info_xybmlx as t2 on t1.xybmlxbh=t2.xybmlxbh");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    listCustomer.Add(new UserInfo(dr["xybmlxmc"].ToString(), dr["xybmbh"].ToString(), dr["xybmmc"].ToString()));
                }
                //lv.Items.Clear(); //关键点，指定 ItemsSource 时，该项必须得清空?
                lv.ItemsSource = listCustomer;
                dr.Close();
                conn.Close();


            }
            else if (text_box_bjmc.Text.Trim() == "")
            {
                string sql = String.Format("select t2.xybmlxmc,t1.xybmbh,t1.xybmmc from t_info_xybm as t1 inner join t_info_xybmlx as t2 on t1.xybmlxbh=t2.xybmlxbh where t2.xybmlxmc='{0}'", combobox_xybmlx_input.Text.Trim());
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    listCustomer.Add(new UserInfo(dr["xybmlxmc"].ToString(), dr["xybmbh"].ToString(), dr["xybmmc"].ToString()));
                }
                //lv.Items.Clear(); //关键点，指定 ItemsSource 时，该项必须得清空?
                lv.ItemsSource = listCustomer;
                dr.Close();
                conn.Close();

            }

            else
            {
                string sql = String.Format("select t2.xybmlxmc,t1.xybmbh,t1.xybmmc from t_info_xybm as t1 inner join t_info_xybmlx as t2 on t1.xybmlxbh=t2.xybmlxbh where t1.xybmmc like'%{0}%' and t2.xybmlxmc='{1}'", text_box_bjmc.Text, combobox_xybmlx_input.Text.Trim());
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                listCustomer.Clear();
                while (dr.Read())
                {

                    listCustomer.Add(new UserInfo(dr["xybmlxmc"].ToString(), dr["xybmbh"].ToString(), dr["xybmmc"].ToString()));
                }
                lv.ItemsSource = listCustomer;
                dr.Close();
                conn.Close();


            }
        }

        private void combobox_xybmlx_types_input_DropDownOpened(object sender, EventArgs e)
        {
            string sql = String.Format("select xybmlxmc from t_info_xybmlx");
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            combobox_xybmlx_input.ItemsSource = ds.Tables[0].DefaultView;
            combobox_xybmlx_input.DisplayMemberPath = "xybmlxmc";
            combobox_xybmlx_input.SelectedValuePath = "xybmlxbh";
        }

        private void combobox_xybmlx_types_input_DropDownClosed(object sender, EventArgs e)
        {

        }

        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

