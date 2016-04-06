using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
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

namespace 中医证治智能系统
{
    /// <summary>
    /// Interaction logic for XiInfoAdmin.xaml
    /// </summary>
    public partial class XiInfoAdmin : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        XiInfo Xi_Edit = new XiInfo("", "", "");
        ObservableCollection<XiInfo> listXi = new ObservableCollection<XiInfo>();
        bool IsAdd = false;
        bool IsModify = false;
        bool IsValid = true;
        bool IsRepeat = false;
        bool Modify = false;
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

        public XiInfoAdmin()
        {
            InitializeComponent();
            save_input.IsEnabled = false;
        }

        /// <summary>
        /// 功能：创建系信息类
        /// </summary>
        public class XiInfo : INotifyPropertyChanged
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
            private string _Beizu;

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

            public string Beizu
            {
                get { return _Beizu; }
                set { _Beizu = value; OnPropertyChanged(new PropertyChangedEventArgs("Beizu")); }
            }

            public XiInfo(string xinumber, string xiname, string Beizu)
            {
                _xiNumber = xinumber;
                _xiName = xiname;
                _Beizu = Beizu;
            }
        }

        /// <summary>
        /// 功能：查找
        /// </summary>
        private void search_Click(object sender, RoutedEventArgs e)
        {
            if (text_box_xmc.Text == "")
            {
                // 先清空目录
                listXi.Clear();
                string sql = String.Format("select * from x_t_info_x");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    listXi.Add(new XiInfo(dr["xbh"].ToString(), dr["xmc"].ToString(), dr["bz"].ToString()));
                }
                //lv.Items.Clear(); //关键点，指定 ItemsSource 时，该项必须得清空?
                lv.ItemsSource = listXi;
                dr.Close();
                conn.Close();
            }
            else
            {
                // 先清空目录
                listXi.Clear();
                string sql = String.Format("select * from x_t_info_x where xmc like'%{0}%'", text_box_xmc.Text);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                listXi.Clear();
                while (dr.Read())
                {

                    listXi.Add(new XiInfo(dr["xbh"].ToString(), dr["xmc"].ToString(), dr["bz"].ToString()));
                }
                lv.ItemsSource = listXi;
                dr.Close();
                conn.Close();
            }
        }

        /// <summary>
        /// 功能：【系信息管理】中的【选定】按钮功能
        /// </summary>
        private void select_search_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            XiInfo xi = lv.SelectedItem as XiInfo;
            if (xi != null && xi is XiInfo)
            {
                PassValuesEventArgs args = new PassValuesEventArgs(xi.XiName.ToString(), xi.XiNumber.ToString());
                // 要判断 PassValuesEvent 是否为空，即判断该窗口是否被调用
                if (PassValuesEvent != null)
                {
                    PassValuesEvent(this, args);
                }
            }
        }

        /// <summary>
        /// 功能：删除
        /// </summary>
        private void delete_search_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("执行删除操作将对数据库造成很大影响，强烈建议不要执行此操作，您确定要删除该系吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (MessageBox.Show("再次警告，您确定要删除该系吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (MessageBox.Show("第三次警告，您确定要删除该系吗？", "confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        XiInfo xi = lv.SelectedItem as XiInfo;
                        if (xi != null && xi is XiInfo)
                        {
                            listXi.Remove(xi);
                        }
                        try
                        {
                            string sql = String.Format("delete from x_t_info_x where xbh = '{0}'", xi.XiNumber);
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

        /// <summary>
        /// 功能：修改
        /// </summary>
        private void modify_search_Click(object sender, RoutedEventArgs e)
        {
            Text_Editable();
            Modify = true;
            save_input.IsEnabled = true;
            tabControl1.SelectedIndex = 1;
            if (lv.SelectedItem != null)
            {
                Xi_Edit = lv.SelectedItem as XiInfo;
            }
            text_xmc.Text = Xi_Edit.XiName;
            text_xbh.Text = Xi_Edit.XiNumber;
            text_bz.Text = Xi_Edit.Beizu;
        }

        /// <summary>
        /// 功能：设置文本编辑权限
        /// </summary>
        public void Text_Editable()
        {

            text_xmc.IsReadOnly = false;
            text_bz.IsReadOnly = false;

        }

        /// <summary>
        /// 功能：返回
        /// </summary>
        private void back_search_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：【系信息录入】中的增加
        /// </summary>
        private void add_input_Click(object sender, RoutedEventArgs e)
        {
            Text_Editable();
            if (IsAdd == false && IsModify == false)
            {
                text_xmc.Text = "";
                text_bz.Text = "";
                save_input.IsEnabled = true;
                string sql = String.Format("select max(xbh) from x_t_info_x");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    text_xbh.Text = String.Format("{0:000}", Convert.ToInt64(dr[0]) + 1);
                    Xi_Edit = new XiInfo(String.Format("{0:000}", Convert.ToInt64(dr[0]) + 1), "", "");
                    listXi.Add(Xi_Edit);
                }
                dr.Close();
                conn.Close();
                Keyboard.Focus(text_xmc); // 设置焦点
                lv.SelectedIndex = lv.Items.Count - 1; // 设置增加项被选中
                IsAdd = true;
            }
        }

        /// <summary>
        /// 功能：【系信息录入】中的修改
        /// </summary>
        private void modify_input_Click(object sender, RoutedEventArgs e)
        {
            if (text_xbh.Text != "")
            {
                save_input.IsEnabled = true;
                IsModify = true;
                Text_Editable();
            }
        }

        /// <summary>
        /// 功能：【系信息录入】中的保存
        /// </summary>
        private void save_input_Click(object sender, RoutedEventArgs e)
        {
            if (text_xmc.Text == "")
            {
                MessageBox.Show("系名称不能为空！");
                IsValid = false;
                Keyboard.Focus(text_xmc);
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
                    string sql = String.Format("INSERT INTO x_t_info_x (xmc,bz,xbh) VALUES ('{0}', '{1}', '{2}')", text_xmc.Text, text_bz.Text, Xi_Edit.XiNumber);

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
                    listXi.Remove(Xi_Edit);
                    text_bz.Text = "";
                    text_xmc.Text = "";
                    text_xbh.Text = "";
                    lv.SelectedIndex = lv.Items.Count - 1;
                }
                finally
                {
                    conn.Close();
                    // 刷新目录
                    refresh();
                }
            }
            else if (IsModify == true && IsValid == true && IsRepeat == false && text_xbh.Text != "")
            {
                save_input.IsEnabled = false;
                IsModify = false;
                try
                {
                    string sql_update = String.Format("UPDATE x_t_info_x SET xmc = '{0}', bz = '{1}' WHERE xbh='{2}' ", text_xmc.Text, text_bz.Text, Xi_Edit.XiNumber);
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
            else if (Modify == true && IsValid == true && IsRepeat == false && text_xbh.Text != "")
            {
                save_input.IsEnabled = false;
                Modify = false;
                try
                {
                    string sql_update = String.Format("UPDATE x_t_info_x SET xmc = '{0}', bz = '{1}' WHERE xbh='{2}' ", text_xmc.Text, text_bz.Text, Xi_Edit.XiNumber);
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
                text_xmc.Text = "";
                Keyboard.Focus(text_xmc);
            }
        }

        /// <summary>
        /// 功能：【系信息录入】中的取消
        /// </summary>
        private void cancel_input_Click(object sender, RoutedEventArgs e)
        {
            save_input.IsEnabled = false;
            IsAdd = false;
            text_xmc.Text = "";
            text_xbh.Text = "";
            text_bz.Text = "";
            // 刷新目录
            refresh();
        }

        /// <summary>
        /// 功能：【系信息录入】中的返回
        /// </summary>
        private void back_input_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：判断是否重复
        /// </summary>
        public void Is_Repeat()
        {
            string username = text_xmc.Text.Trim();
            string sql = String.Format("select count(*) from x_t_info_x where xmc = '{0}'and xbh!='{1}'", username, Xi_Edit.XiNumber);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count == 1)
            {
                MessageBox.Show("病机名称不能重复！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                IsRepeat = true;
            }
            else
                IsRepeat = false;
            conn.Close();
        }

        /// <summary>
        /// 功能：刷新目录
        /// </summary>
        private void refresh()
        {
            if (text_box_xmc.Text == "")
            {
                // 先清空目录
                listXi.Clear();
                string sql = String.Format("select * from x_t_info_x");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    listXi.Add(new XiInfo(dr["xbh"].ToString(), dr["xmc"].ToString(), dr["bz"].ToString()));
                }
                //lv.Items.Clear(); //关键点，指定 ItemsSource 时，该项必须得清空?
                lv.ItemsSource = listXi;
                dr.Close();
                conn.Close();
            }
            else
            {
                // 先清空目录
                listXi.Clear();
                string sql = String.Format("select * from x_t_info_x where xmc like'%{0}%'", text_box_xmc.Text);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                listXi.Clear();
                while (dr.Read())
                {
                    listXi.Add(new XiInfo(dr["xbh"].ToString(), dr["xmc"].ToString(), dr["bz"].ToString()));
                }
                lv.ItemsSource = listXi;
                dr.Close();
                conn.Close();
            }
        }

        /// <summary>
        /// 功能：设置文本读写权限
        /// </summary>
        public void Text_Readonly()
        {
            text_xmc.IsReadOnly = true;
            text_bz.IsReadOnly = true;
        }

        /// <summary>
        /// 功能：listview选择某项时对应的信息显示
        /// </summary>
        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            XiInfo xi = lv.SelectedItem as XiInfo;
            if (xi != null && xi is XiInfo)
            {
                text_xmc.Text = xi.XiName;
                text_xbh.Text = xi.XiNumber;
                text_bz.Text = xi.Beizu;
            }
            Xi_Edit = xi;
        }

        /// <summary>
        /// 功能：系名称编辑框失去焦点时对应的listview显示
        /// </summary>
        private void text_xmc_LostFocus(object sender, RoutedEventArgs e)
        {
            if(text_xbh.Text != "")
                Xi_Edit.XiName = text_xmc.Text;
        }

        /// <summary>
        /// 功能：备注编辑框失去焦点时对应的listview显示
        /// </summary>
        private void text_bz_LostFocus(object sender, RoutedEventArgs e)
        {
            if (text_xbh.Text != "")
                Xi_Edit.Beizu = text_bz.Text;
        }
    }
}
