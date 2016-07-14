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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace 中医证治智能系统
{
    /// <summary>
    /// Interaction logic for ClassiccaseSearch.xaml
    /// </summary>
    public partial class ClassiccaseSearch : Window
    {
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 创建集合实例
        ObservableCollection<ClassiccaseInfo> listInfo = new ObservableCollection<ClassiccaseInfo>();
        // 全局变量
        ClassiccaseInfo Info_Edit = new ClassiccaseInfo("", "", "");
        // 创建对 PassValuesHandler 方法的引用的类
        public delegate void PassValuesHandler(object sender, PassValuesEventArgs e);
        // 声明事件
        public event PassValuesHandler PassValuesEvent;
        // 创建事件数据类
        public class PassValuesEventArgs : EventArgs
        {
            private string _name;
            private string _id;
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
            public string Number
            {
                get { return _id; }
                set { _id = value; }
            }

            public PassValuesEventArgs(string name, string id)
            {
                this.Name = name;
                this.Number = id;
            }
        }
        public ClassiccaseSearch()
        {
            InitializeComponent();
            // 指定 listview 数据源
            lv.ItemsSource = listInfo;
            // 初始化默认选择项
            radio_info.IsChecked = true;
        }

        /// <summary>
        /// 功能：创建检索信息类
        /// 说明：Title -- > 检索标题 Approximate -- > 近似度
        /// </summary>
        public class ClassiccaseInfo : INotifyPropertyChanged
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
            private string _id;
            private string _title;
            private string _approximate;

            public string Number
            {
                get { return _id; }
                set { _id = value; OnPropertyChanged(new PropertyChangedEventArgs("Number")); }
            }

            public string Title
            {
                get { return _title; }
                set { _title = value; OnPropertyChanged(new PropertyChangedEventArgs("Title")); }
            }

            public string Approximate
            {
                get { return _approximate; }
                set { _approximate = value; OnPropertyChanged(new PropertyChangedEventArgs("Approximate")); }
            }

            public ClassiccaseInfo(string id, string title, string approximate)
            {
                _id = id;
                _title = title;
                _approximate = approximate;
            }
        }

        /// <summary>
        /// 功能：返回
        /// </summary>
        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：检索
        /// </summary>
        private void search_Click(object sender, RoutedEventArgs e)
        {
            listInfo.Clear();
            bz.Text = "";
            if (radio_info.IsChecked == true)
            {
                id.Text = "";
                if (info.Text != "")
                {
                    string sql = String.Format("select * from t_bl  where jsxx LIKE '%{0}%' and bllx = '1' order by jsxx", info.Text.ToString());
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        listInfo.Add(new ClassiccaseInfo(dr["blbh"].ToString(), dr["jsxx"].ToString(), dr["zt"].ToString()));
                    }
                    dr.Close();
                    conn.Close();                   
                }
                else 
                {
                    string sql = String.Format("select * from t_bl where bllx = '1' order by jsxx");
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        listInfo.Add(new ClassiccaseInfo(dr["blbh"].ToString(), dr["jsxx"].ToString(), dr["zt"].ToString()));
                    }
                    dr.Close();
                    conn.Close();
                }
            }
            if (radio_id.IsChecked == true)
            {
                info.Text = "";
                if (id.Text != "")
                {
                    string sql = String.Format("select * from t_bl  where blbh = '{0}' and bllx = '1' order by jsxx", id.Text.ToString());
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        listInfo.Add(new ClassiccaseInfo(dr["blbh"].ToString(), dr["jsxx"].ToString(), dr["zt"].ToString()));
                    }
                    dr.Close();
                    conn.Close();
                }
                else
                {
                    string sql = String.Format("select * from t_bl where bllx = '1' order by jsxx");
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        listInfo.Add(new ClassiccaseInfo(dr["blbh"].ToString(), dr["jsxx"].ToString(), dr["zt"].ToString()));
                    }
                    dr.Close();
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 功能：ListView 选择变化时触发事件
        /// </summary>
        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClassiccaseInfo info = lv.SelectedItem as ClassiccaseInfo;
            if (info != null && info is ClassiccaseInfo)
            {
                title.Text = info.Title;
                string sql = String.Format("select * from t_bl  where blbh = '{0}'", info.Number);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    bz.Text = dr["bz"].ToString();
                }
                dr.Close();
                conn.Close();
            }  
        }

        /// <summary>
        /// 功能：保存
        /// </summary>
        private void save_Click(object sender, RoutedEventArgs e)
        {
            ClassiccaseInfo info = lv.SelectedItem as ClassiccaseInfo;
            if (info != null && info is ClassiccaseInfo)
            {
                string sql = String.Format("update t_bl set jsxx = '{0}' where blbh = '{1}'", title.Text, info.Number);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                int count = comm.ExecuteNonQuery();
                conn.Close();
                refresh(); // 刷新
            }
        }

        /// <summary>
        /// 功能：删除
        /// </summary>
        private void delete_Click(object sender, RoutedEventArgs e)
        {
            ClassiccaseInfo info = lv.SelectedItem as ClassiccaseInfo;
            if (info != null && info is ClassiccaseInfo)
            {
                if (MessageBox.Show("您确认要删除吗？", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    string sql = String.Format("delete from t_bl where blbh = '{0}'", info.Number);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    conn.Close();
                    refresh();
                    bz.Text = "";
                }
            }
        }

        /// <summary>
        /// 功能：选定
        /// </summary>
        private void select_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ClassiccaseInfo userinfo = lv.SelectedItem as ClassiccaseInfo;
            if (userinfo != null && userinfo is ClassiccaseInfo)
            {
                PassValuesEventArgs args = new PassValuesEventArgs(userinfo.Title.ToString(), userinfo.Number.ToString());
                if (PassValuesEvent != null)
                {
                    PassValuesEvent(this, args);
                }
            }
        }

        /// <summary>
        /// 功能：刷新
        /// </summary>
        private void refresh()
        {
            listInfo.Clear();
            string sql = String.Format("select * from t_bl where bllx = '1' order by jsxx");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listInfo.Add(new ClassiccaseInfo(dr["blbh"].ToString(), dr["jsxx"].ToString(), dr["zt"].ToString()));
            }
            dr.Close();
            conn.Close();
        }
    }
}
