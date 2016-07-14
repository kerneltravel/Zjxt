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
    /// Interaction logic for ChufzcInfoAdmin.xaml
    /// </summary>
    public partial class ChufzcInfoAdmin : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        bool click = false;
        bool Add = false;
        bool IsValid = true;
        bool IsRepeat = false;
        int Selected_Items = -1;
        string MedicineNumber1;
        string ynumber;
        string MedicineNumber;
        ChufzcInfo chu_Edit = new ChufzcInfo("", "", "", "", "");
        // 创建集合类并实例化
        ObservableCollection<ChufzcInfo> listChufzc = new ObservableCollection<ChufzcInfo>();

        public ChufzcInfoAdmin()
        {
            InitializeComponent();
            lv.ItemsSource = listChufzc;
            lv.SelectedIndex = 0;
            save.IsEnabled = false;

        }
        // 创建 listview 显示信息类
        public class ChufzcInfo : INotifyPropertyChanged
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
            private string _cfNumber;
            private string _cfName;
            private string _ywName;
            private string _ywjl;
            private string _dw;

            public string CfNumber
            {
                get { return _cfNumber; }
                set { _cfNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("CfNumber")); }
            }

            public string CfName
            {
                get { return _cfName; }
                set { _cfName = value; OnPropertyChanged(new PropertyChangedEventArgs("CfName")); }
            }

            public string YwName
            {
                get { return _ywName; }
                set { _ywName = value; OnPropertyChanged(new PropertyChangedEventArgs("YwName")); }
            }

            public string Ywjl
            {
                get { return _ywjl; }
                set { _ywjl = value; OnPropertyChanged(new PropertyChangedEventArgs("Ywjl")); }
            }

            public string Dw
            {
                get { return _dw; }
                set { _dw = value; OnPropertyChanged(new PropertyChangedEventArgs("Dw")); }
            }

            public ChufzcInfo(string cfnumber, string cfname, string ywname, string ywjl, string dw)
            {
                _cfNumber = cfnumber;
                _cfName = cfname;
                _ywName = ywname;
                _ywjl = ywjl;
                _dw = dw;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            click = true;
            JibenCF jibencf = new JibenCF();
            jibencf.PassValuesEvent += new JibenCF.PassValuesHandler(ReceiveValues);
            jibencf.Show();
        }


        private void ReceiveValues(object sender, JibenCF.PassValuesEventArgs e)
        {
            listChufzc.Clear();
            string sql = String.Format("select t1.ywmc ,t_info_jbcfxx.jbcfmc,  t_info_cfzc.ywjl, t_info_cfzc.dw from (t_info_yw as t1 inner join t_info_cfzc on t1.ywbh =  t_info_cfzc.ywbh ) inner join t_info_jbcfxx on t_info_jbcfxx.jbcfbh =  t_info_cfzc.cfbh  where jbcfbh = '{0}'", e.Number);
            // string sql = String.Format("select t1.ywmc ,t3.jbcfmc,  t2.ywjl, t2.dw from (t_info_jbcfxx as t3 inner join t_info_cfzc as t2 on t1.ywbh =  t2.ywbh ) inner join t_info_jbcfxx as t3 on t3.jbcfbh =  t2.cfbh  where jbcfbh = '{0}'", e.Number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                this.cfmc.Text = dr["jbcfmc"].ToString();
                chu_Edit = new ChufzcInfo(e.Number, dr["jbcfmc"].ToString(), dr["ywmc"].ToString(), Convert.ToInt64(dr["ywjl"]).ToString(), dr["dw"].ToString());
                listChufzc.Add(chu_Edit);
            }
            dr.Close();
            conn.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (click == false && cfmc.Text == "")
            {
                MessageBox.Show("请先选择处方！", "comfirm", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else if (Add == false)
            {
                MessageBox.Show("表未处于添加状态！", "comfirm", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MedicineInfoAdmin medicine = new MedicineInfoAdmin();
                medicine.PassValuesEvent += new MedicineInfoAdmin.PassValuesHandler(ReceiveValues1);
                medicine.Show();

            }

        }


        private void ReceiveValues1(object sender, MedicineInfoAdmin.PassValuesEventArgs e)
        {
            Keyboard.Focus(ywmc);
            string sql = String.Format("select ywmc from t_info_yw where ywbh='{0}'", e.Number);
            // string sql = String.Format("select t1.ywmc ,t3.jbcfmc,  t2.ywjl, t2.dw from (t_info_jbcfxx as t3 inner join t_info_cfzc as t2 on t1.ywbh =  t2.ywbh ) inner join t_info_jbcfxx as t3 on t3.jbcfbh =  t2.cfbh  where jbcfbh = '{0}'", e.Number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                this.ywmc.Text = dr["ywmc"].ToString();

            }
            dr.Close();
            conn.Close();
        }
        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {

            Add = true;
            save.IsEnabled = true;
            if (cfmc.Text == "")
            {
                MessageBox.Show("请先输入基本处方！", "comfirm", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ChufzcInfo ywAdd = new ChufzcInfo(chu_Edit.CfNumber, chu_Edit.CfName, "", "", "");
                listChufzc.Add(ywAdd);
                lv.SelectedIndex = lv.Items.Count - 1;

            }

        }

        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            chu_Edit = lv.SelectedItem as ChufzcInfo;
            Selected_Items = lv.SelectedIndex;
            if (chu_Edit != null && chu_Edit is ChufzcInfo)
            {
                ywjl.Text = chu_Edit.Ywjl;
            }
        }
        private void ywmc_LostFocus(object sender, RoutedEventArgs e)
        {
            chu_Edit.YwName = ywmc.Text;
        }
        private void ywjl_LostFocus(object sender, RoutedEventArgs e)
        {
            chu_Edit.Ywjl = ywjl.Text;
        }
        private void dw_LostFocus(object sender, RoutedEventArgs e)
        {
            chu_Edit.Dw = dw.Text;
        }
        private void ywjl_TextChanged(object sender, TextChangedEventArgs e)
        {
            //屏蔽中文输入和非法字符粘贴输入
            TextBox textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);

            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (ywmc.Text == "")
            {
                MessageBox.Show("请选择药物！");
                IsValid = false;
                Keyboard.Focus(ywmc);
            }
            else if (ywjl.Text == "")
            {
                MessageBox.Show("请输入剂量！");
                IsValid = false;
                Keyboard.Focus(ywjl);
            }
            else if (dw.Text == "")
            {
                MessageBox.Show("请选择单位！");
                IsValid = false;

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
                string sql = String.Format("select ywbh from t_info_yw where ywmc='{0}'", ywmc.Text);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    MedicineNumber = dr["ywbh"].ToString();
                }
                dr.Close();
                conn.Close();
                try
                {
                    string sql1 = String.Format("INSERT INTO t_info_cfzc (cfbh,ywbh,ywjl,dw) VALUES ('{0}', '{1}', '{2}','{3}')", chu_Edit.CfNumber, MedicineNumber, ywjl.Text, dw.Text);

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
                    listChufzc.Remove(chu_Edit);
                    cfmc.Text = "";
                    ywmc.Text = "";
                    ywjl.Text = "";
                    dw.Text = "";
                    bz.Text = "";
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
            string username = ywmc.Text.Trim();
            string sql = String.Format("select ywbh from t_info_yw where ywmc = '{0}'", username);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                ynumber = dr["ywbh"].ToString();
            }
            dr.Close();
            conn.Close();
            string sql1 = String.Format("select count(*) from t_info_cfzc where cfbh = '{0}'and ywbh='{1}'", chu_Edit.CfNumber, ynumber);
            conn.Open();
            SqlCommand comm1 = new SqlCommand(sql1, conn);
            int count = (int)comm1.ExecuteScalar();
            if (count == 1)
            {
                MessageBox.Show("药物名称不能重复！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
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
            if (MessageBox.Show("执行删除操作将对数据库造成很大影响，强烈建议不要执行此操作，您确定要删除该处方吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (MessageBox.Show("再次警告，您确定要删除该处方吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (MessageBox.Show("第三次警告，您确定要删除该处方吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ChufzcInfo userinfo = lv.SelectedItem as ChufzcInfo;
                        if (userinfo != null && userinfo is ChufzcInfo)
                        {
                            listChufzc.Remove(userinfo);
                        }

                        string sql = String.Format("select ywbh from t_info_yw where ywmc='{0}'", userinfo.YwName);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        SqlDataReader dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            MedicineNumber1 = dr["ywbh"].ToString();
                        }
                        dr.Close();
                        conn.Close();
                        try
                        {
                            string sql1 = String.Format("delete from t_info_cfzc where ywbh = '{0}'", MedicineNumber1);
                            conn.Open();
                            SqlCommand comm1 = new SqlCommand(sql1, conn);
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

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            cfmc.Text = "";
            ywmc.Text = "";
            ywjl.Text = "";
            dw.Text = "";
            bz.Text = "";
            listChufzc.Clear();
        }





    }
}
