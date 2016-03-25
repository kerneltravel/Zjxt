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
    /// Interaction logic for FanywRuleAdmin.xaml
    /// </summary>
    public partial class FanywRuleAdmin : Window
    {
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        FanywInfo Fan_Edit = new FanywInfo("", "", "", "", "");
        ObservableCollection<FanywInfo> listHufanyw = new ObservableCollection<FanywInfo>();
        string a, b, c;
        bool click = false;
        bool Add = false;
        bool IsValid = true;
        bool IsRepeat = false;
        string[] d = new string[100];
        public FanywRuleAdmin()
        {
            InitializeComponent();
            save.IsEnabled = false;
            lv.ItemsSource = listHufanyw;
            lv.SelectedIndex = 0;
        }
        public class FanywInfo : INotifyPropertyChanged
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
            private string _groupNumber;
            private string _mainywNumber;
            private string _mainywName;
            private string _hufanywNumber;
            private string _hufanywName;

            public string GroupNumber
            {
                get { return _groupNumber; }
                set { _groupNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("GroupNumber")); }
            }

            public string MainywNumber
            {
                get { return _mainywNumber; }
                set { _mainywNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("MainywNumber")); }
            }

            public string MainywName
            {
                get { return _mainywName; }
                set { _mainywName = value; OnPropertyChanged(new PropertyChangedEventArgs("MainywName")); }
            }

            public string HufanywNumber
            {
                get { return _hufanywNumber; }
                set { _hufanywNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("HufanywNumber")); }
            }
            public string HufanywName
            {
                get { return _hufanywName; }
                set { _hufanywName = value; OnPropertyChanged(new PropertyChangedEventArgs("HufanywName")); }
            }
            public FanywInfo(string groupnumber, string mainywnumber, string mainywname, string hufanywnumber, string hufanywname)
            {
                _groupNumber = groupnumber;
                _mainywNumber = mainywnumber;
                _mainywName = mainywname;
                _hufanywNumber = hufanywnumber;
                _hufanywName = hufanywname;

            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            click = true;

            MedicineInfoAdmin ywinfo = new MedicineInfoAdmin();
            ywinfo.PassValuesEvent += new MedicineInfoAdmin.PassValuesHandler(ReceiveValues);
            ywinfo.Show();
        }
        private void ReceiveValues(object sender, MedicineInfoAdmin.PassValuesEventArgs e)
        {

            a = e.Number;
            listHufanyw.Clear();
            string sql1 = String.Format("select * from t_info_yw where ywbh='{0}'", e.Number);
            conn.Open();
            SqlCommand comm1 = new SqlCommand(sql1, conn);
            SqlDataReader dr1 = comm1.ExecuteReader();

            while (dr1.Read())
            {
                this.mainywmc.Text = dr1["ywmc"].ToString();
            }
            dr1.Close();
            conn.Close();
            string sql = String.Format("select t1.zbh, t1.ywbh,t_info_yw.ywmc from t_info_yw inner join t_rule_fyw as t1 on t_info_yw.ywbh=t1.ywbh where t1.ywbh='{0}'", e.Number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                a = dr["zbh"].ToString();
                b = e.Number;
                c = dr["ywmc"].ToString();


            }
            dr.Close();
            conn.Close();
            string sql2 = String.Format("select t1.hfywbh,t_info_yw.ywmc from t_info_yw inner join t_rule_fyw as t1 on t_info_yw.ywbh=t1.hfywbh where t1.ywbh='{0}'", e.Number);
            conn.Open();
            SqlCommand comm2 = new SqlCommand(sql2, conn);
            SqlDataReader dr2 = comm2.ExecuteReader();
            while (dr2.Read())
            {

                Fan_Edit = new FanywInfo(a, b, c, dr2["hfywbh"].ToString(), dr2["ywmc"].ToString());
                listHufanyw.Add(Fan_Edit);

            }
            dr2.Close();
            conn.Close();
        }
        private void hufanywmc_LostFocus(object sender, RoutedEventArgs e)
        {
            Fan_Edit.HufanywName = hufanywmc.Text;
            Fan_Edit.HufanywNumber = a;

        }
        private void display_Click(object sender, RoutedEventArgs e)
        {
            int i = 0; int j = 0;
            string sql2 = String.Format("select t_info_yw.ywmc from t_info_yw inner join t_rule_fyw as t1 on t_info_yw.ywbh=t1.hfywbh group by t1.zbh, t1.ywbh, t1.hfywbh ,ywmc");
            conn.Open();
            SqlCommand comm2 = new SqlCommand(sql2, conn);
            SqlDataReader dr2 = comm2.ExecuteReader();
            while (dr2.Read() && i < 100)
            {

                d[i] = dr2["ywmc"].ToString();
                i++;

            }
            dr2.Close();
            conn.Close();
            string sql = String.Format("select t1.zbh, t1.ywbh,t_info_yw.ywmc,t1.hfywbh,t_info_yw.ywmc from t_info_yw inner join t_rule_fyw as t1 on t_info_yw.ywbh=t1.ywbh group by t1.zbh, t1.ywbh, t1.hfywbh ,ywmc ");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read() && j < i)
            {
                Fan_Edit = new FanywInfo(dr["zbh"].ToString(), dr["ywbh"].ToString(), dr["ywmc"].ToString(), dr["hfywbh"].ToString(), d[j]);
                j++;
                listHufanyw.Add(Fan_Edit);



            }
            dr.Close();
            conn.Close();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (click == false && mainywmc.Text == "")
            {
                MessageBox.Show("请先选择主药物名称！", "comfirm", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else if (Add == false)
            {
                MessageBox.Show("表未处于添加状态！", "comfirm", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {

                MedicineInfoAdmin ywinfo = new MedicineInfoAdmin();
                ywinfo.PassValuesEvent += new MedicineInfoAdmin.PassValuesHandler(ReceiveValues1);
                ywinfo.Show();


            }

        }

        private void ReceiveValues1(object sender, MedicineInfoAdmin.PassValuesEventArgs e)
        {
            Keyboard.Focus(hufanywmc);
            a = e.Number;
            string sql = String.Format("select ywmc from t_info_yw where ywbh='{0}'", e.Number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                this.hufanywmc.Text = dr["ywmc"].ToString();

            }
            dr.Close();
            conn.Close();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            FanywInfo userinfo = lv.SelectedItem as FanywInfo;
            Add = true;
            save.IsEnabled = true;
            if (mainywmc.Text == "")
            {
                MessageBox.Show("请先输入主药物名称！", "comfirm", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (userinfo != null)
            {
                Fan_Edit = new FanywInfo(userinfo.GroupNumber, userinfo.MainywNumber, userinfo.MainywName, "", "");
                listHufanyw.Add(Fan_Edit);
                lv.SelectedIndex = lv.Items.Count - 1;
            }
            else
            {
                string sql = String.Format("select max(zbh) from t_rule_fyw");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    Fan_Edit = new FanywInfo(String.Format("{0:0000}", Convert.ToInt64(dr[0]) + 1), a, mainywmc.Text, "", "");
                    listHufanyw.Add(Fan_Edit);
                }
                dr.Close();
                conn.Close();
                lv.SelectedIndex = lv.Items.Count - 1;

            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            mainywmc.Text = "";
            hufanywmc.Text = "";
            listHufanyw.Clear();

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (hufanywmc.Text == "")
            {
                MessageBox.Show("请选择互反药物名称！");
                IsValid = false;
                Keyboard.Focus(hufanywmc);
            }
            else
            {
                IsValid = true;
                Is_Repeat();
            }
            if (Add == true && IsValid == true && IsRepeat == false)
            {
                save.IsEnabled = false;
                Add = false;
                string sql = String.Format("select ywbh from t_info_yw where ywmc='{0}'", hufanywmc.Text);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    b = dr["ywbh"].ToString();
                }
                dr.Close();
                conn.Close();
                try
                {
                    string sql1 = String.Format("INSERT INTO t_rule_fyw(zbh,ywbh,hfywbh,bz) VALUES ('{0}', '{1}', '{2}','{3}')", Fan_Edit.GroupNumber, Fan_Edit.MainywNumber, b, "");

                    conn.Open();
                    SqlCommand comm1 = new SqlCommand(sql1, conn);
                    int count = comm1.ExecuteNonQuery();
                    if (count > 0)
                    {
                        MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("添加失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    listHufanyw.Remove(Fan_Edit);
                    mainywmc.Text = "";
                    hufanywmc.Text = "";
                    lv.SelectedIndex = lv.Items.Count - 1;
                }
                finally
                {
                    conn.Close();

                }
            }
        }
        public void Is_Repeat()
        {
            string username = hufanywmc.Text.Trim();
            string sql = String.Format("select ywbh from t_info_yw where ywmc = '{0}'", username);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                c = dr["ywbh"].ToString();
            }
            dr.Close();
            conn.Close();
            string sql1 = String.Format("select count(*) from t_rule_fyw where hfywbh = '{0}'and ywbh='{1}'", c, Fan_Edit.MainywNumber);
            conn.Open();
            SqlCommand comm1 = new SqlCommand(sql1, conn);
            int count = (int)comm1.ExecuteScalar();
            if (count == 1)
            {
                MessageBox.Show("该记录已存在！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                IsRepeat = true;
            }
            else
            {
                IsRepeat = false;
            }
            conn.Close();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("您确认要删除该记录吗吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                FanywInfo userinfo = lv.SelectedItem as FanywInfo;
                if (userinfo != null && userinfo is FanywInfo)
                {
                    listHufanyw.Remove(userinfo);
                }


                try
                {
                    string sql = String.Format("delete from t_rule_fyw where hfywbh = '{0}'", userinfo.HufanywNumber);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    if (count > 0)
                    {
                        MessageBox.Show("删除成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("删除失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }

}


