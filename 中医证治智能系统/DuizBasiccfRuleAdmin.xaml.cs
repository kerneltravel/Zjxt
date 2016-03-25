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
    /// Interaction logic for DuizBasiccfRuleAdmin.xaml
    /// </summary>
    public partial class DuizBasiccfRuleAdmin : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 创建集合实例
        ObservableCollection<BasicZhengmCfInfo> listBasicZhengmCf = new ObservableCollection<BasicZhengmCfInfo>();
        // 全局变量，用于存储病名类信息
        BasicZhengmCfInfo BasicZhengmCf_Edit = new BasicZhengmCfInfo("", "", "", "");
        string zhengNumber;
        string chuNumber;
        string cfNumber;
        bool click = false;
        bool Add = false;
        bool IsValid = true;

        public DuizBasiccfRuleAdmin()
        {
            InitializeComponent();
            save.IsEnabled = false;
            lv.ItemsSource = listBasicZhengmCf;
            lv.SelectedIndex = 0;
        }
        public class BasicZhengmCfInfo : INotifyPropertyChanged
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

            private string _BasicZhengmNumber;
            private string _BasicZhengmName;
            private string _ChufNumber;
            private string _ChufName;


            public string BasicZhengmNumber
            {
                get { return _BasicZhengmNumber; }
                set { _BasicZhengmNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("BasicZhengmNumber")); }
            }

            public string BasicZhengmName
            {
                get { return _BasicZhengmName; }
                set { _BasicZhengmName = value; OnPropertyChanged(new PropertyChangedEventArgs("BasicZhengmName")); }
            }

            public string ChufNumber
            {
                get { return _ChufNumber; }
                set { _ChufNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("ChufNumber")); }
            }

            public string ChufName
            {
                get { return _ChufName; }
                set { _ChufName = value; OnPropertyChanged(new PropertyChangedEventArgs("ChufName")); }
            }


            public BasicZhengmCfInfo(string basiczhengmnumber, string basiczhengmname, string chufnumber, string chufname)
            {
                _BasicZhengmNumber = basiczhengmnumber;
                _BasicZhengmName = basiczhengmname;
                _ChufNumber = chufnumber;
                _ChufName = chufname;

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            click = true;
            BasicZhengm basiczm = new BasicZhengm();
            basiczm.PassValuesEvent += new BasicZhengm.PassValuesHandler(ReceiveValues);
            basiczm.Show();

        }
        private void ReceiveValues(object sender, BasicZhengm.PassValuesEventArgs e)
        {
            zhengNumber = e.Number;
            listBasicZhengmCf.Clear();
            string sql1 = String.Format("select * from t_info_jbzm where jbzmbh='{0}'", e.Number);
            conn.Open();
            SqlCommand comm1 = new SqlCommand(sql1, conn);
            SqlDataReader dr1 = comm1.ExecuteReader();

            while (dr1.Read())
            {
                this.zmmc.Text = dr1["jbzmmc"].ToString();
            }
            dr1.Close();
            conn.Close();
            string sql = String.Format("select t_rule_dzjbcf.zmbh, t_info_jbzm.jbzmmc,t_rule_dzjbcf.cfbh,t1.jbcfmc from(t_info_jbcfxx as t1 inner join t_rule_dzjbcf on t1.jbcfbh=t_rule_dzjbcf.cfbh)inner join t_info_jbzm on t_info_jbzm.jbzmbh=t_rule_dzjbcf.zmbh  where jbzmbh='{0}'", e.Number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();

            while (dr.Read())
            {
                BasicZhengmCf_Edit = new BasicZhengmCfInfo(e.Number, dr["jbzmmc"].ToString(), dr["cfbh"].ToString(), dr["jbcfmc"].ToString());
                listBasicZhengmCf.Add(BasicZhengmCf_Edit);

            }
            dr.Close();
            conn.Close();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            if (click == false && zmmc.Text == "")
            {
                MessageBox.Show("请输入证名名称！", "comfirm", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else if (Add == false)
            {
                MessageBox.Show("表未处于添加状态！", "comfirm", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {

                JibenCF jibencf = new JibenCF();
                jibencf.Show();
                jibencf.PassValuesEvent += new JibenCF.PassValuesHandler(ReceiveValues1);


            }




        }
        private void ReceiveValues1(object sender, JibenCF.PassValuesEventArgs e)
        {
            Keyboard.Focus(cfmc);
            chuNumber = e.Number;
            string sql = String.Format("select jbcfmc from t_info_jbcfxx where jbcfbh='{0}'", e.Number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                this.cfmc.Text = dr["jbcfmc"].ToString();

            }
            dr.Close();
            conn.Close();
        }
        private void cfmc_LostFocus(object sender, RoutedEventArgs e)
        {
            BasicZhengmCf_Edit.ChufName = cfmc.Text;
            BasicZhengmCf_Edit.ChufNumber = chuNumber;

        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            BasicZhengmCfInfo userinfo = lv.SelectedItem as BasicZhengmCfInfo;
            Add = true;
            save.IsEnabled = true;
            if (zmmc.Text == "")
            {
                MessageBox.Show("请先输入基本症名！", "comfirm", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (userinfo == null)
            {
                BasicZhengmCf_Edit = new BasicZhengmCfInfo(zhengNumber, zmmc.Text, "", "");
                listBasicZhengmCf.Add(BasicZhengmCf_Edit);
                lv.SelectedIndex = lv.Items.Count - 1;

            }
            else
            {
                Add = false;
                MessageBox.Show("该基本症名对应的处方已经存在！", "comfirm", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (cfmc.Text == "")
            {
                MessageBox.Show("处方不能为空！");
                IsValid = false;
                Keyboard.Focus(cfmc);
            }
            else
            {
                IsValid = true;


            }
            if (Add == true && IsValid == true)
            {

                save.IsEnabled = false;
                Add = false;
                string sql = String.Format("select jbcfbh from t_info_jbcfxx where jbcfmc='{0}'", cfmc.Text);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    cfNumber = dr["jbcfbh"].ToString();
                }
                dr.Close();
                conn.Close();
                try
                {
                    string sql1 = String.Format("INSERT INTO t_rule_dzjbcf (zmbh,cfbh,dzcfzt,bz) VALUES ('{0}', '{1}','{2}','{3}')", BasicZhengmCf_Edit.BasicZhengmNumber, cfNumber, "", "");

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
                    listBasicZhengmCf.Remove(BasicZhengmCf_Edit);
                    zmmc.Text = "";
                    cfmc.Text = "";

                    lv.SelectedIndex = lv.Items.Count - 1;
                }
                finally
                {
                    conn.Close();

                }
            }

        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            listBasicZhengmCf.Clear();
            zmmc.Text = "";
            cfmc.Text = "";

        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("您确认要删除该记录吗吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                BasicZhengmCfInfo userinfo = lv.SelectedItem as BasicZhengmCfInfo;
                if (userinfo != null && userinfo is BasicZhengmCfInfo)
                {
                    listBasicZhengmCf.Remove(userinfo);
                }


                try
                {
                    string sql = String.Format("delete from t_rule_dzjbcf where zmbh = '{0}'", userinfo.BasicZhengmNumber);
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

        private void back_Copy_Click(object sender, RoutedEventArgs e)
        {
            string sql = String.Format("select t_rule_dzjbcf.zmbh, t_info_jbzm.jbzmmc,t_rule_dzjbcf.cfbh,t1.jbcfmc from(t_info_jbcfxx as t1 inner join t_rule_dzjbcf on t1.jbcfbh=t_rule_dzjbcf.cfbh)inner join t_info_jbzm on t_info_jbzm.jbzmbh=t_rule_dzjbcf.zmbh ");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();

            while (dr.Read())
            {
                BasicZhengmCf_Edit = new BasicZhengmCfInfo(dr["zmbh"].ToString(), dr["jbzmmc"].ToString(), dr["cfbh"].ToString(), dr["jbcfmc"].ToString());
                listBasicZhengmCf.Add(BasicZhengmCf_Edit);

            }
            dr.Close();
            conn.Close();
        }
    }
}
