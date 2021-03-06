﻿using System;
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
    /// Interaction logic for XiRuleAdmin.xaml
    /// </summary>
    public partial class RukeRule : Window
    {// 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);

        XiruleInfo xi_Edit = new XiruleInfo("", "", "", "");
        bool click = false;
        bool Add = false;
        bool IsValid = true;
        bool IsRepeat = false;
        int Selected_Items = -1;
        // 创建集合类并实例化
        ObservableCollection<XiruleInfo> listXirule = new ObservableCollection<XiruleInfo>();
        XiruleInfo ywAdd = new XiruleInfo("", "", "", "");
        public RukeRule()
        {
            InitializeComponent();
            lv.ItemsSource = listXirule;
            lv.SelectedIndex = 0;
            save.IsEnabled = false;
        }

        // 创建 listview 显示信息类
        public class XiruleInfo : INotifyPropertyChanged
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
            private string _rkNumber;
            private string _rkName;
            private string _zxNumber;
            private string _zxName;

            public string RkNumber
            {
                get { return _rkNumber; }
                set { _rkNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("RkNumber")); }
            }

            public string RkName
            {
                get { return _rkName; }
                set { _rkName = value; OnPropertyChanged(new PropertyChangedEventArgs("RkName")); }
            }

            public string ZxNumber
            {
                get { return _zxNumber; }
                set { _zxNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("ZxNumber")); }
            }

            public string ZxName
            {
                get { return _zxName; }
                set { _zxName = value; OnPropertyChanged(new PropertyChangedEventArgs("ZxName")); }
            }



            public XiruleInfo(string rknumber, string rkname, string zxnumber, string zxname)
            {
                _rkNumber = rknumber;
                _rkName = rkname;
                _zxNumber = zxnumber;
                _zxName = zxname;

            }
        }
        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {

            string sql = String.Format("select rkmc from t_info_rk");
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            combobox_rk.ItemsSource = ds.Tables[0].DefaultView;
            combobox_rk.DisplayMemberPath = "rkmc";
            combobox_rk.SelectedValuePath = "rkbh";

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }




        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (combobox_rk.Text != null)
            {
                listXirule.Clear();
                string sql = String.Format("select t_info_zxmx.zxmc,  t_rule_rk.rkbh, t_rule_rk.zxbh from (t_info_rk as t1 inner join t_rule_rk on t1.rkbh =  t_rule_rk.rkbh ) inner join t_info_zxmx on t_rule_rk.zxbh =  t_info_zxmx.zxbh  where rkmc = '{0}'", combobox_rk.Text);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {

                    xi_Edit = new XiruleInfo(dr["rkbh"].ToString(), combobox_rk.Text, dr["zxbh"].ToString(), dr["zxmc"].ToString());
                    listXirule.Add(xi_Edit);
                }
                dr.Close();
                conn.Close();
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            Add = true;
            save.IsEnabled = true;
            if (combobox_rk.Text == "")
            {
                MessageBox.Show("请输入系！", "comfirm", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ywAdd = new XiruleInfo(xi_Edit.RkNumber, xi_Edit.RkName, "", "");
                listXirule.Add(ywAdd);
                lv.SelectedIndex = lv.Items.Count - 1;

            }
        }

        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            xi_Edit = lv.SelectedItem as XiruleInfo;
            Selected_Items = lv.SelectedIndex;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (lv.SelectedItem == null)
            {
                MessageBox.Show("请先选择系！", "comfirm", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else if (Add == false)
            {
                MessageBox.Show("表未处于添加状态！", "comfirm", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else
            {
                Info_Symptom info = new Info_Symptom();
                info.Show();
                click = true;
                info.PassValuesEvent += new Info_Symptom.PassValuesHandler(ReceiveValues);
            }
        }
        private void ReceiveValues(object sender, Info_Symptom.PassValuesEventArgs e)
        {
            string sql = String.Format("select zxbh from t_info_zxmx where zxmc='{0}'", e.Name);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                this.zxmc.Text = e.Name;
                XiruleInfo userinfo = lv.SelectedItem as XiruleInfo;
                if (userinfo != null && userinfo is XiruleInfo)
                {
                    userinfo.ZxName = e.Name;
                    userinfo.ZxNumber = dr["zxbh"].ToString();
                }
            }
            dr.Close();
            conn.Close();
        }

        private void zxmc_LostFocus(object sender, RoutedEventArgs e)
        {
            xi_Edit.ZxName = zxmc.Text;

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            listXirule.Clear();
            zxmc.Text = "";
            combobox_rk.Text = "";
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("您确认要删除吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                XiruleInfo userinfo = lv.SelectedItem as XiruleInfo;
                if (userinfo != null && userinfo is XiruleInfo)
                {
                    listXirule.Remove(userinfo);
                }
                try
                {
                    string sql = String.Format("delete from t_rule_rk where rkbh = '{0}'and zxbh='{1}'", userinfo.RkNumber, userinfo.ZxNumber);
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

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (zxmc.Text == "")
            {
                MessageBox.Show("请选择症象！");
                IsValid = false;
                Keyboard.Focus(zxmc);
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
                XiruleInfo userinfo = lv.SelectedItem as XiruleInfo;
                if (userinfo != null && userinfo is XiruleInfo)
                {
                    try
                    {
                        string sql = String.Format("INSERT INTO t_rule_rk (rkbh,zxbh,bz) VALUES ('{0}', '{1}', '{2}')", userinfo.RkNumber, userinfo.ZxNumber, "");

                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                        if (count > 0)
                        {
                            MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("添加失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        listXirule.Remove(userinfo);
                        combobox_rk.Text = "";
                        zxmc.Text = "";
                        lv.SelectedIndex = lv.Items.Count - 1;
                    }
                    finally
                    {
                        conn.Close();

                    }
                }

            }
        }
        public void Is_Repeat()
        {
            string username = zxmc.Text.Trim();
            string sql = String.Format("select count(*) from t_rule_rk where rkbh = '{0}'and zxbh='{1}'", xi_Edit.RkNumber, xi_Edit.ZxNumber);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
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
    }
}

