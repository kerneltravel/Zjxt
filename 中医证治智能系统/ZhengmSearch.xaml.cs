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
    /// Interaction logic for ZhengmSearch.xaml
    /// </summary>
    public partial class ZhengmSearch : Window
    {
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 创建集合实例
        ObservableCollection<ZhengmInfo> listInfo = new ObservableCollection<ZhengmInfo>();
        // 全局变量
        ZhengmInfo Info_Edit = new ZhengmInfo("", "", "", "");

        /// <summary>
        /// 功能：创建检索信息类
        /// 说明：BmNumber -- > 病名编号 BmName -- > 病名名称 BasiczmNumber -- > 基本证名编号  BasiczmName -- > 基本证名名称
        /// </summary>
        public class ZhengmInfo : INotifyPropertyChanged
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
            private string _bmnumber;
            private string _bmname;
            private string _basiczmnumber;
            private string _basiczmname;

            public string BmNumber
            {
                get { return _bmnumber; }
                set { _bmnumber = value; OnPropertyChanged(new PropertyChangedEventArgs("BmNumber")); }
            }

            public string BmName
            {
                get { return _bmname; }
                set { _bmname = value; OnPropertyChanged(new PropertyChangedEventArgs("BmName")); }
            }

            public string BasiczmNumber
            {
                get { return _basiczmnumber; }
                set { _basiczmnumber = value; OnPropertyChanged(new PropertyChangedEventArgs("BasiczmNumber")); }
            }

            public string BasiczmName
            {
                get { return _basiczmname; }
                set { _basiczmname = value; OnPropertyChanged(new PropertyChangedEventArgs("BasiczmName")); }
            }

            public ZhengmInfo(string bmnumber, string bmname, string basiczmnumber, string basiczmname)
            {
                _bmnumber = bmnumber;
                _bmname = bmname;
                _basiczmnumber = basiczmnumber;
                _basiczmname = basiczmname;
            }
        }

        public ZhengmSearch()
        {
            InitializeComponent();
            // 指定 listview 数据源
            lv.ItemsSource = listInfo;
        }

        /// <summary>
        /// 功能：返回
        /// </summary>
        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：调用【病名信息管理】窗口,【选定】病名名称
        /// </summary>
        private void btn_bmmc_Click(object sender, RoutedEventArgs e)
        {
            DiseaseInfoAdmin diseaseinfoadmin = new DiseaseInfoAdmin();
            diseaseinfoadmin.PassValuesEvent += new DiseaseInfoAdmin.PassValuesHandler(ReceiveValues);
            diseaseinfoadmin.Show();
        }

        /// <summary>
        /// 功能：实现病名名称的读取和显示
        /// </summary>
        private void ReceiveValues(object sender, DiseaseInfoAdmin.PassValuesEventArgs e)
        {
            bmmc.Text = e.Name;
            listInfo.Clear();
            string sql = String.Format("select t1.bmbh, t2.bmmc, t1.jbzmbh, t3.jbzmmc from (t_info_jbzmbm as t1 inner join t_info_bm as t2 on t1.bmbh = t2.bmbh) inner join t_info_jbzm as t3 on t3.jbzmbh = t1.jbzmbh where t1.bmbh = '{0}' order by t1.bmbh", e.Number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listInfo.Add(new ZhengmInfo(dr["bmbh"].ToString(), dr["bmmc"].ToString(), dr["jbzmbh"].ToString(), dr["jbzmmc"].ToString())); 
            }
            dr.Close();
            conn.Close();
        }

        /// <summary>
        /// 功能：显示全部 
        /// </summary>
        private void display_Click(object sender, RoutedEventArgs e)
        {
            listInfo.Clear();
            string sql = String.Format("select t1.bmbh, t2.bmmc, t1.jbzmbh, t3.jbzmmc from (t_info_jbzmbm as t1 inner join t_info_bm as t2 on t1.bmbh = t2.bmbh) inner join t_info_jbzm as t3 on t3.jbzmbh = t1.jbzmbh order by t1.bmbh");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listInfo.Add(new ZhengmInfo(dr["bmbh"].ToString(), dr["bmmc"].ToString(), dr["jbzmbh"].ToString(), dr["jbzmmc"].ToString()));
            }
            dr.Close();
            conn.Close();
        }


    }
}
