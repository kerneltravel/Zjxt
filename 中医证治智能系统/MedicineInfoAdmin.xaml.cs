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
    /// Interaction logic for MedicineInfoAdmin.xaml
    /// </summary>
    public partial class MedicineInfoAdmin : Window
    {
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        UserInfo User_Edit = new UserInfo("", "", "");
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
                get{ return _number;} 
                set{ _number = value ;} 
            }
            public PassValuesEventArgs(string name, string number)
            {
                this.Name = name;
                this.Number = number;
            }


            
        }

        bool IsAdd = false;
        bool IsModify = false;
        bool IsValid = true;
        bool IsRepeat = false;
        bool Modify = false;
        public MedicineInfoAdmin()
        {
            InitializeComponent();
            save_input.IsEnabled = false;

            Text_Readonly();
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
            private string _yaowuNumber;
            private string _yaowuName;
            private string _Beizu;

            public string YaowuNumber
            {
                get { return _yaowuNumber; }
                set { _yaowuNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("YaowuNumber")); }
            }

            public string YaowuName
            {
                get { return _yaowuName; }
                set { _yaowuName = value; OnPropertyChanged(new PropertyChangedEventArgs("YaowuName")); }
            }

            public string Beizu
            {
                get { return _Beizu; }
                set { _Beizu = value; OnPropertyChanged(new PropertyChangedEventArgs("Beizu")); }
            }

            public UserInfo(string yaowunumber, string yaowuname, string Beizu)
            {
                _yaowuNumber = yaowunumber;
                _yaowuName = yaowuname;
                _Beizu = Beizu;
            }
        }
        public void Text_Readonly()
        {

            text_ywmc.IsReadOnly = true;
            text_bz.IsReadOnly = true;
        }
        public void Text_Editable()
        {

            text_ywmc.IsReadOnly = false;
            text_bz.IsReadOnly = false;

        }
        private void text_ywmc_LostFocus(object sender, RoutedEventArgs e)
        {
            User_Edit.YaowuName = text_ywmc.Text;
        }
        private void text_bz_LostFocus(object sender, RoutedEventArgs e)
        {
            User_Edit.Beizu = text_bz.Text;
        }
        private void search_Click(object sender, RoutedEventArgs e)
        {


            if (text_box_ywmc.Text == "")
            {
                string sql = String.Format("select * from t_info_yw");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    listCustomer.Add(new UserInfo(dr["ywbh"].ToString(), dr["ywmc"].ToString(), dr["bz"].ToString()));
                }
                //lv.Items.Clear(); //关键点，指定 ItemsSource 时，该项必须得清空?
                lv.ItemsSource = listCustomer;
                dr.Close();
                conn.Close();


            }
            else
            {
                string sql = String.Format("select * from t_info_yw where ywmc like'%{0}%'", text_box_ywmc.Text);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                listCustomer.Clear();
                while (dr.Read())
                {

                    listCustomer.Add(new UserInfo(dr["ywbh"].ToString(), dr["ywmc"].ToString(), dr["bz"].ToString()));
                }
                lv.ItemsSource = listCustomer;
                dr.Close();
                conn.Close();


            }

        }

        private void back_search_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void delete_search_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("执行删除操作将对数据库造成很大影响，强烈建议不要执行此操作，您确定要删除该药名吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (MessageBox.Show("再次警告，您确定要删除该药名吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (MessageBox.Show("第三次警告，您确定要删除该药名吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        UserInfo userinfo = lv.SelectedItem as UserInfo;
                        if (userinfo != null && userinfo is UserInfo)
                        {
                            listCustomer.Remove(userinfo);
                        }
                        try
                        {
                            string sql = String.Format("delete from t_info_yw where ywbh = '{0}'", userinfo.YaowuNumber);
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

        private void select_search_Click(object sender, RoutedEventArgs e)
        {
            UserInfo userinfo = lv.SelectedItem as UserInfo;
            if (userinfo != null && userinfo is UserInfo)
            {
                PassValuesEventArgs args = new PassValuesEventArgs(userinfo.YaowuName,userinfo.YaowuNumber);
                // 要判断 PassValuesEvent 是否为空，即判断该窗口是否被调用
                if (PassValuesEvent != null)
                {
                    PassValuesEvent(this, args);
                }
            }
            this.Close();
        }

        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserInfo userinfo = lv.SelectedItem as UserInfo;
            if (userinfo != null && userinfo is UserInfo)
            {

                text_ywmc.Text = userinfo.YaowuName;
                text_ywbh.Text = userinfo.YaowuNumber;
                text_bz.Text = userinfo.Beizu;
            }
            User_Edit = userinfo;
        }

        private void modify_search_Click(object sender, RoutedEventArgs e)
        {
            Text_Editable();
            Modify = true;
            save_input.IsEnabled = true;
            tabControl1.SelectedIndex = 1;
            if (lv.SelectedItem != null)
            {
                User_Edit = lv.SelectedItem as UserInfo;
            }
            text_ywmc.Text = User_Edit.YaowuName;
            text_ywbh.Text = User_Edit.YaowuNumber;
            text_bz.Text = User_Edit.Beizu;


        }

        private void back_input_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cancel_input_Click(object sender, RoutedEventArgs e)
        {

            save_input.IsEnabled = false;
            text_ywmc.Text = "";
            text_ywbh.Text = "";
            text_bz.Text = "";

        }

        private void add_input_Click(object sender, RoutedEventArgs e)
        {
            Text_Editable();
            if (IsAdd == false && IsModify == false)
            {
                text_ywmc.Text = "";

                text_bz.Text = "";
                save_input.IsEnabled = true;
                string sql = String.Format("select max(ywbh) from t_info_yw");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    text_ywbh.Text = String.Format("{0:00000000}", Convert.ToInt64(dr[0]) + 1);
                    User_Edit = new UserInfo(String.Format("{0:00000000}", Convert.ToInt64(dr[0]) + 1), "", "");
                    listCustomer.Add(User_Edit);
                }
                dr.Close();
                conn.Close();
                Keyboard.Focus(text_ywmc); // 设置焦点
                lv.SelectedIndex = lv.Items.Count - 1; // 设置增加项被选中
                IsAdd = true;
            }
        }

        private void save_input_Click(object sender, RoutedEventArgs e)
        {
            if (text_ywmc.Text == "")
            {
                MessageBox.Show("基本病机名称不能为空！");
                IsValid = false;
                Keyboard.Focus(text_ywmc);
            }
            else
            {
                IsValid = true;
                Is_Repeat();
            }
            if (IsAdd == true && IsValid == true && IsRepeat == false)
            {
                save_input.IsEnabled = false;
                IsAdd = false;
                try
                {
                    string sql = String.Format("INSERT INTO t_info_yw (ywmc,bz,ywbh) VALUES ('{0}', '{1}', '{2}')", text_ywmc.Text, text_bz.Text, User_Edit.YaowuNumber);

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
                    listCustomer.Remove(User_Edit);
                    text_bz.Text = "";
                    text_ywmc.Text = "";
                    text_ywbh.Text = "";
                    lv.SelectedIndex = lv.Items.Count - 1;
                }
                finally
                {
                    conn.Close();

                }
            }
            else if (IsModify == true && IsValid == true && IsRepeat == false && text_ywbh.Text != "")
            {
                save_input.IsEnabled = false;
                IsModify = false;
                try
                {
                    string sql_update = String.Format("UPDATE t_info_yw SET ywmc = '{0}', bz = '{1}' WHERE ywbh='{2}' ", text_ywmc.Text, text_bz.Text, User_Edit.YaowuNumber);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql_update, conn);
                    int count = comm.ExecuteNonQuery();
                    if (count > 0)
                    {
                        MessageBox.Show("修改成功！", "操作", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("修改失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                finally
                {
                    conn.Close();
                    Text_Readonly();
                }
            }
            else if (Modify == true && IsValid == true && IsRepeat == false && text_ywbh.Text != "")
            {
                save_input.IsEnabled = false;
                Modify = false;
                try
                {
                    string sql_update = String.Format("UPDATE t_info_yw SET ywmc = '{0}', bz = '{1}' WHERE ywbh='{2}' ", text_ywmc.Text, text_bz.Text, User_Edit.YaowuNumber);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql_update, conn);
                    int count = comm.ExecuteNonQuery();
                    if (count > 0)
                    {
                        MessageBox.Show("修改成功！", "操作", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("修改失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                finally
                {
                    conn.Close();
                    Text_Readonly();
                }
            }
            if (IsRepeat == true)
            {
                text_ywmc.Text = "";
                Keyboard.Focus(text_ywmc);
            }
        }
        public void Is_Repeat()
        {
            string username = text_ywmc.Text.Trim();
            string sql = String.Format("select count(*) from t_info_yw where ywmc = '{0}'and ywbh!='{1}'", username, User_Edit.YaowuNumber);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count == 1)
            {
                MessageBox.Show("药物名称不能重复！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                IsRepeat = true;
            }
            else
                IsRepeat = false;
            conn.Close();
        }

        private void modify_input_Click(object sender, RoutedEventArgs e)
        {
            save_input.IsEnabled = true;
            IsModify = true;
            Text_Editable();
        }
    }
}
