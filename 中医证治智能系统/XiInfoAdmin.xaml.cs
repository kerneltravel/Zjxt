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
    /// Interaction logic for XiInfoAdmin.xaml
    /// </summary>
    public partial class XiInfoAdmin : Window
    {// 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        int i = -1;
        static string j;
        bool IsAdd = false;

        bool IsEdit = false;

        bool valid = true;

        bool IsRepeat = false;
        UserInfo user_add = new UserInfo("", "", "");

        UserInfo useredit = new UserInfo("", "", "");

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
            private string _xiNumber;
            private string _xiName;
            private string _beiZu;

            public string XiNumber
            {
                get { return _xiNumber; }
                set { _xiNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("XiNumber")); }
            }
            public string XiName
            {
                get { return _xiName; }
                set { _xiName = value; OnPropertyChanged(new PropertyChangedEventArgs("XiName")); }
            }

            public string BeiZu
            {
                get { return _beiZu; }
                set { _beiZu = value; OnPropertyChanged(new PropertyChangedEventArgs("BeiZu")); }
            }

            public UserInfo(string xinumber, string xiname, string beizu)
            {
                _xiNumber = xinumber;
                _xiName = xiname;
                _beiZu = beizu;
            }
        }

        ObservableCollection<UserInfo> listCustomer1 = new ObservableCollection<UserInfo>();
        public void Text_Readonly()
        {
            text_box_xmc.IsReadOnly = true;
            text_box_bz.IsReadOnly = true;

        }
        public void Text_Editable()
        {
            text_box_xmc.IsReadOnly = false;
            text_box_bz.IsReadOnly = false;
        }
        public void Text_Clear()
        {
            text_box_bz.Text = "";
            text_box_xmc.Text = "";
            text_block_xbh.Text = j;

        }
        private void text_box_xmc_LostFocus(object sender, RoutedEventArgs e)
        {
            //if(  )
            user_add.XiName = text_box_xmc.Text;
            useredit.XiName = text_box_xmc.Text;
        }
        private void text_box_bz_LostFocus(object sender, RoutedEventArgs e)
        {
            user_add.BeiZu = text_box_bz.Text;
            useredit.BeiZu = text_box_bz.Text;
        }

        public XiInfoAdmin()
        {
            InitializeComponent();
            button_save.IsEnabled = false;

            string sql = String.Format("select * from t_info_x");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listCustomer1.Add(new UserInfo(dr["xbh"].ToString(), dr["xmc"].ToString(), dr["bz"].ToString()));

            }
            lv1.ItemsSource = listCustomer1;
            dr.Close();
            conn.Close();
            // 设置默认选中第一项
            lv1.SelectedIndex = 0;
        }

        private void button_add_Click(object sender, RoutedEventArgs e)
        {


            string sql = String.Format("select max(xbh) from t_info_x");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read() && button_add.IsEnabled)
            {
                user_add = new UserInfo(String.Format("{0:000}", Convert.ToInt64(dr[0]) + 1), "", "");
                j = String.Format("{0:000}", Convert.ToInt64(dr[0]) + 1);
                Text_Clear();
                Text_Editable();
                listCustomer1.Add(user_add);
            }
            dr.Close();
            conn.Close();
            Keyboard.Focus(text_box_xmc); // 设置焦点
            lv1.SelectedIndex = lv1.Items.Count - 1; // 设置增加项被选中
            IsAdd = true;
            button_save.IsEnabled = true;
            button_add.IsEnabled = false;
        }
        //public void Text_Clear()
        //{
        //    text_box_bz.Text = "";
        //    text_box_xmc.Text = "";
        //    text_block_xbh.Text = j;

        //}

        private void lv1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv1.SelectedIndex != lv1.Items.Count - 1 && IsAdd)
            {
                if (text_box_xmc.Text == "")
                {
                    MessageBox.Show("系名不能为空！");
                    lv1.SelectedIndex = lv1.Items.Count - 1;
                    Keyboard.Focus(text_box_xmc);
                }
                else
                {
                    MessageBox.Show("请先保存！");
                    lv1.SelectedIndex = i;
                }
            }
            else if (lv1.SelectedIndex != i && IsEdit)
            {
                if (text_box_xmc.Text == "")
                {
                    MessageBox.Show("系名不能为空！");
                    lv1.SelectedIndex = i;
                    Keyboard.Focus(text_box_xmc);
                }
                else
                {
                    MessageBox.Show("请先保存！");
                    lv1.SelectedIndex = i;
                }
            }
            else
            {
                useredit = lv1.SelectedItem as UserInfo;
                i = lv1.SelectedIndex;
                if (useredit != null && useredit is UserInfo)
                {
                    string sql = String.Format("select * from t_info_x where xbh = '{0}'", useredit.XiNumber);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        if (IsEdit != true) Text_Readonly();
                        text_box_xmc.Text = dr["xmc"].ToString();
                        text_box_bz.Text = dr["bz"].ToString();
                        text_block_xbh.Text = dr["xbh"].ToString();
                    }
                    dr.Close();
                    conn.Close();
                }
            }
        }

        private void button_back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button_modify_Click(object sender, RoutedEventArgs e)
        {
            if (lv1.SelectedIndex == -1)
            {
                MessageBox.Show("请预先选择需要修改的行！");
            }
            else
            {
                IsEdit = true;
                Text_Editable();
                button_save.IsEnabled = true;
            }
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            if (text_box_xmc.Text == "")
            {
                MessageBox.Show("系名不能为空！");
                valid = false;
                Keyboard.Focus(text_box_xmc);
            }
            else
            {
                valid = true;
                Is_Repeat();
            }
            if (IsAdd == true && valid == true && IsRepeat == false)
            {
                // 前面三项满足后才能保存至数据库
                button_add.IsEnabled = true;
                button_save.IsEnabled = false;
                lv1.SelectedIndex = lv1.Items.Count - 1;
                IsAdd = false;
                try
                {
                    string sql = String.Format("INSERT INTO t_info_x (xbh,xmc,bz) VALUES ('{0}', '{1}', '{2}')",
                                   user_add.XiNumber, text_box_xmc.Text, text_box_bz.Text);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    if (count > 0)
                    {
                        MessageBox.Show("保存成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("保存失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    listCustomer1.Remove(user_add);
                    Text_Clear();
                    lv1.SelectedIndex = lv1.Items.Count - 1;

                }
                finally
                {
                    conn.Close();
                    Text_Readonly();
                }
            }
            else if (IsEdit == true && valid == true && IsRepeat == false)
            {

                IsEdit = false;
                button_save.IsEnabled = false;
                try
                {
                    string sql_update = String.Format("UPDATE t_info_x SET xmc='{0}', bz ='{1}' WHERE xbh='{2}'", text_box_xmc.Text, text_box_bz.Text, useredit.XiNumber);

                    conn.Open();
                    // 创建 Command 对象
                    SqlCommand comm = new SqlCommand(sql_update, conn);
                    int count = comm.ExecuteNonQuery();
                    if (count > 0)
                    {
                        MessageBox.Show("操作成功！", "操作", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception)
                {

                    MessageBox.Show("操作失败！");

                }
                finally
                {
                    conn.Close();
                    Text_Readonly();
                }
            }
            if (IsRepeat == true)
            {
                Keyboard.Focus(text_box_xmc);

            }
        }

        public void Is_Repeat()
        {

            string username = text_box_xmc.Text.Trim();
            string sql = String.Format("select count(*) from t_info_x where xmc = '{0}' and xbh!='{1}' ", username, useredit.XiNumber);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count == 1)
            {
                MessageBox.Show("姓名不能重复！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                IsRepeat = true;

            }
            else
                IsRepeat = false;
            conn.Close();

        }
        private void button_delete_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("执行删除操作将对数据库造成很大影响，强烈建议不要执行此操作，您确定要删除该病名吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (MessageBox.Show("再次警告，您确定要删除该病名吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (MessageBox.Show("第三次警告，您确定要删除该病名吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        UserInfo userinfo = lv1.SelectedItem as UserInfo;
                        if (userinfo != null && userinfo is UserInfo)
                        {
                            listCustomer1.Remove(userinfo);
                        }
                        Text_Clear();
                        try
                        {
                            string sql = String.Format("delete from t_info_x where xbh = '{0}'", userinfo.XiNumber);
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

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            button_save.IsEnabled = false;
            Text_Readonly();
            if (IsEdit == true)
            {
                //string number = text_block_xbh.Text;
                string sql = String.Format("select * from t_info_x where xbh = '{0}'", useredit.XiNumber);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    text_box_xmc.Text = dr["xmc"].ToString();
                    text_box_bz.Text = dr["bz"].ToString();
                    useredit.XiName = text_box_xmc.Text;

                }
                dr.Close();
                conn.Close();

            }
            IsEdit = false;


        }
    }
}
