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
    /// Interaction logic for ClinicalcaseSearch.xaml
    /// </summary>
    public partial class ClinicalcaseSearch : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        ClinicalInfo Clinical_Edit = new ClinicalInfo("", "", "");
        int Selected_Items = -1;
        ObservableCollection<ClinicalInfo> listClinical = new ObservableCollection<ClinicalInfo>();

        public ClinicalcaseSearch()
        {
            InitializeComponent();
            lv.ItemsSource = listClinical;
            lv.SelectedIndex = -1;

        }
        public class ClinicalInfo : INotifyPropertyChanged
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
            private string _brNumber;
            private string _blNumber;
            private string _time;


            public string brNumber
            {
                get { return _brNumber; }
                set { _brNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("BrNumber")); }
            }

            public string BlNumber
            {
                get { return _blNumber; }
                set { _blNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("BlNumber")); }
            }

            public string Time
            {
                get { return _time; }
                set { _time = value; OnPropertyChanged(new PropertyChangedEventArgs("Time")); }
            }



            public ClinicalInfo(string brnumber, string blnumber, string time)
            {
                _brNumber = brnumber;
                _blNumber = blnumber;
                _time = time;

            }
        }

        // 创建对 PassValuesHandler 方法的引用的类
        public delegate void PassValuesHandler(object sender, PassValuesEventArgs e);
        // 声明事件
        public event PassValuesHandler PassValuesEvent;
        // 创建事件数据类
        public class PassValuesEventArgs : EventArgs
        {
            private string _brNumber;
            private string _blNumber;
            public string brNumber
            {
                get { return _brNumber; }
                set { _brNumber = value; }
            }
            public string blNumber
            {
                get { return _blNumber; }
                set { _blNumber = value; }
            }

            public PassValuesEventArgs(string brnumber, string blnumber)
            {
                this.brNumber = brnumber;
                this.blNumber = blnumber;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PatientinfoSearch patient = new PatientinfoSearch();
            patient.Show();
            patient.PassValuesEvent += new PatientinfoSearch.PassValuesHandler(ReceiveValues);
        }

        private void ReceiveValues(object sender, PatientinfoSearch.PassValuesEventArgs e)
        {
            listClinical.Clear();
            string sql = String.Format("select t1.blbh ,t_br_info.brid, t1.jzsj ,t_br_info.xm from t_bl as t1 inner join t_br_info on t1.jsxx = t_br_info.brid  where brid = '{0}' and bllx='0'", e.Number);
            // string sql = String.Format("select t1.ywmc ,t3.jbcfmc,  t2.ywjl, t2.dw from (t_info_jbcfxx as t3 inner join t_info_cfzc as t2 on t1.ywbh =  t2.ywbh ) inner join t_info_jbcfxx as t3 on t3.jbcfbh =  t2.cfbh  where jbcfbh = '{0}'", e.Number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                this.name.Text = dr["xm"].ToString();
                Clinical_Edit = new ClinicalInfo(e.Number, dr["blbh"].ToString(), dr["jzsj"].ToString());
            }

            dr.Close();
            conn.Close();
            listClinical.Add(Clinical_Edit);
            lv.SelectedIndex = 0;
        }

        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Clinical_Edit = lv.SelectedItem as ClinicalInfo;
            Selected_Items = lv.SelectedIndex;
            if (Clinical_Edit != null && Clinical_Edit is ClinicalInfo)
            {
                string sql = String.Format("select bz from t_bl where blbh='{0}'", Clinical_Edit.BlNumber);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    cf.Text = dr[0].ToString();
                }
                dr.Close();
                conn.Close();

            }
        }

        private void select_search_Click(object sender, RoutedEventArgs e)
        {
            ClinicalInfo userinfo = lv.SelectedItem as ClinicalInfo;
            if (userinfo != null && userinfo is ClinicalInfo)
            {
                PassValuesEventArgs args = new PassValuesEventArgs(Clinical_Edit.brNumber.ToString(), Clinical_Edit.BlNumber.ToString());
                if (PassValuesEvent != null)
                {
                    PassValuesEvent(this, args);
                }
            }
            this.Close();
        }

        private void back_search_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            listClinical.Clear();
            cf.Text = "";
            if (blbh.Text != "")
            {
                string sql = String.Format("select jsxx,jzsj from t_bl where blbh like '{0}%' and bllx='0'", blbh.Text);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
     
                    Clinical_Edit = new ClinicalInfo(dr["jsxx"].ToString(), blbh.Text, dr["jzsj"].ToString());
                    listClinical.Add(Clinical_Edit);
                }
                dr.Close();
                conn.Close();
                lv.SelectedIndex = -1;
            }
            else
            {
                string sql = String.Format("select t1.blbh ,t_br_info.brid, t1.jzsj from t_bl as t1 inner join t_br_info on t1.jsxx = t_br_info.brid where t1.bllx='0'");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    Clinical_Edit = new ClinicalInfo(dr["brid"].ToString(), dr["blbh"].ToString(), dr["jzsj"].ToString());
                    listClinical.Add(Clinical_Edit);
                }
                dr.Close();
                conn.Close();
                lv.SelectedIndex = -1;
            }

        }


    }
}
