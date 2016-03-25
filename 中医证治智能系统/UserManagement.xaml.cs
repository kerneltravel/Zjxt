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
    /// Interaction logic for UserManagement.xaml
    /// </summary>
    public partial class UserManagement : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 用于判断是否进行了 Add 操作
        bool IsAdd = false;
        // 用于判断是否进行了 Modify 操作
        bool IsModify = false;
        // 用于判断在进行保存操作时是否能将数据保存至数据库
        bool IsValid = true;
        // 用于判断用户姓名重复
        bool IsRepeat = false;
        // 全局变量，用于保存所选行数
        int Selected_Items = -1;
        // 全局变量，用于用户添加、修改功能存储信息
        UserInfo User_Edit = new UserInfo("", "", "");
        // 创建集合实例
        ObservableCollection<UserInfo> listCustomer = new ObservableCollection<UserInfo>();

        /// <summary>
        /// 功能：类的构造函数，用于初始化
        /// 说明：1.初始化窗口（默认）
        ///       2.初始化保存按钮不可用
        ///       3.从数据库读取用户信息，添加至集合，并显示在 listview 中
        ///       4.为 listview 指定数据源
        ///       5.设置 listview 默认选中第一项
        ///       6.初始化右边信息不可编辑
        /// </summary>
        public UserManagement()
        {
            InitializeComponent();
            button_save.IsEnabled = false;
            string sql = String.Format("select * from t_xtgl_czry");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listCustomer.Add(new UserInfo(dr["rybm"].ToString(), dr["name"].ToString(), dr["qx"].ToString()));
            }
            lv.Items.Clear(); //关键点，指定 ItemsSource 时，该项必须得清空?
            lv.ItemsSource = listCustomer;
            dr.Close();
            conn.Close();
            lv.SelectedIndex = 0;
            Text_Readonly();
        }

        /// <summary>
        /// 功能：创建用户对象类
        /// </summary>
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
            private string _employeeNumber;
            private string _Name;
            private string _Competence;

            public string EmployeeNumber
            {
                get { return _employeeNumber; }
                set { _employeeNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("EmployeeNumber")); }
            }

            public string Name
            {
                get { return _Name; }
                set { _Name = value; OnPropertyChanged(new PropertyChangedEventArgs("Name")); }
            }

            public string Competence
            {
                get { return _Competence; }
                set { _Competence = value; OnPropertyChanged(new PropertyChangedEventArgs("Competence")); }
            }

            public UserInfo(string empnumber, string name, string Competence)
            {
                _employeeNumber = empnumber;
                _Name = name;
                _Competence = Competence;
            }
        }
        
        /// <summary>
        /// 功能：设置文本只读
        /// </summary>
        public void Text_Readonly()
        {           
            textbox_name.IsReadOnly = true;
            textbox_login.IsReadOnly = true;
            textbox_password.IsEnabled = false;
            textbox_login.IsReadOnly = true;
            combobox_Competence.IsEnabled = false;
            combobox_sex.IsEnabled = false;
            textbox_email.IsReadOnly = true;
            textbox_mobilephone.IsReadOnly = true;
            textbox_fixedphone.IsReadOnly = true;
            datepicker_birthday.IsEnabled = false;
            textbox_address.IsReadOnly = true;
            textbox_remark.IsReadOnly = true;
        }

        /// <summary>
        /// 功能：设置文本可读
        /// </summary>
        public void Text_Editable()
        {
            textbox_name.IsReadOnly = false;
            textbox_login.IsReadOnly = false;
            textbox_password.IsEnabled = true;
            textbox_login.IsReadOnly = false;
            combobox_Competence.IsEnabled = true;
            combobox_sex.IsEnabled = true;
            textbox_email.IsReadOnly = false;
            textbox_mobilephone.IsReadOnly = false;
            textbox_fixedphone.IsReadOnly = false;
            datepicker_birthday.IsEnabled = true;
            textbox_address.IsReadOnly = false;
            textbox_remark.IsReadOnly = false;
        }

        /// <summary>
        /// 功能：清空文本
        /// </summary> 
        public void Text_Clear()
        {
            textbox_name.Text = "";
            textbox_login.Text = "";
            textbox_password.Password = "";
            textbox_login.Text = "";
            combobox_Competence.Text = "";
            combobox_sex.Text = "";
            textbox_email.Text = "";
            textbox_mobilephone.Text = "";
            textbox_fixedphone.Text = "";
            datepicker_birthday.Text = "";
            textbox_address.Text = "";
            textbox_remark.Text = "";
        }

        /// <summary>
        /// 功能：实现 combobox 下拉绑定数据库
        /// </summary>
        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            string sql = String.Format("select qxzmc from t_info_qxz");
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            combobox_Competence.ItemsSource = ds.Tables[0].DefaultView;
            combobox_Competence.DisplayMemberPath = "qxzmc";
            combobox_Competence.SelectedValuePath = "subid";
        }

        /// <summary>
        /// 功能：左侧 Item 选中后右侧显示详细信息
        /// 说明：1.一般情况下，右边显示的信息是不可编辑的，应设置为只读。
        ///       2.在增加功能和修改功能下，右边显示的信息是可编辑的。
        ///       3.在增加功能下，选中项始终为最后一项，即增加的一项。
        ///       4.在修改功能下，选中项为修改的项。
        ///       5.添加 lv.SelectedIndex != lv.Items.Count - 1 这条限制条件，避免提示信息在重新设置选中添加项时重复出现
        /// </summary>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv.SelectedIndex != lv.Items.Count - 1 && IsAdd)
            {
                if (textbox_name.Text == "")
                {
                    MessageBox.Show("姓名不能为空！");
                    lv.SelectedIndex = lv.Items.Count - 1;                    
                }
                else
                {
                    if (textbox_login.Text == "")
                    {                        
                        MessageBox.Show("用户不能为空！");
                        lv.SelectedIndex = lv.Items.Count - 1;                       
                    }
                    else
                    {
                        if (textbox_password.Password == "")
                        {
                            MessageBox.Show("密码不能为空！");
                            lv.SelectedIndex = lv.Items.Count - 1;
                        }
                        else
                        {
                            MessageBox.Show("请先保存！");                            
                            lv.SelectedIndex = lv.Items.Count - 1;
                        }
                    }
                }
            }
            else if (lv.SelectedIndex != Selected_Items && IsModify)
            {
                if (textbox_name.Text == "")
                {
                    MessageBox.Show("姓名不能为空！");
                    lv.SelectedIndex = Selected_Items;
                    Keyboard.Focus(textbox_name);
                }
                else
                {
                    if (textbox_login.Text == "")
                    {
                        MessageBox.Show("用户不能为空！");
                        lv.SelectedIndex = Selected_Items;
                        Keyboard.Focus(textbox_login);
                    }
                    else
                    {
                        if (textbox_password.Password == "")
                        {
                            MessageBox.Show("密码不能为空！");
                            lv.SelectedIndex = Selected_Items;
                            Keyboard.Focus(textbox_password);
                        }
                        else
                        {
                            MessageBox.Show("请先保存！");
                            lv.SelectedIndex = Selected_Items;
                        }
                    }
                }
            }
            else
            {
                User_Edit = lv.SelectedItem as UserInfo;
                Selected_Items = lv.SelectedIndex;
                if (User_Edit != null && User_Edit is UserInfo)
                {
                    string sql = String.Format("select * from t_xtgl_czry where rybm = '{0}'", User_Edit.EmployeeNumber);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        textbox_name.Text = dr["name"].ToString();
                        textbox_login.Text = dr["login"].ToString();
                        textbox_password.Password = dr["pass"].ToString();
                        textbox_login.Text = dr["login"].ToString();
                        combobox_Competence.Text = dr["qx"].ToString();
                        combobox_sex.Text = dr["sex"].ToString();
                        textbox_email.Text = dr["Email"].ToString();
                        textbox_mobilephone.Text = dr["mobile"].ToString();
                        textbox_fixedphone.Text = dr["dh"].ToString();
                        datepicker_birthday.Text = dr["birthday"].ToString();
                        textbox_address.Text = dr["address"].ToString();
                        textbox_remark.Text = dr["bz"].ToString();
                    }
                    dr.Close();
                    conn.Close();
                }
            }           
        }

        /// <summary>
        /// 功能：退出
        /// </summary>
        private void Button_Click_back(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：增加用户
        /// 说明：1.清空右边显示信息
        ///       2.设置右边信息可编辑
        ///       3.设置保存按钮可用
        ///       4.从数据库读取最大人员编号，加一赋值给添加项
        ///       5.不能重复点击添加
        /// </summary>
        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            if (IsAdd == false && IsModify == false)
            {
                Text_Clear();
                Text_Editable();
                button_save.IsEnabled = true;
                string sql = String.Format("select max(rybm) from t_xtgl_czry");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);         
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    User_Edit = new UserInfo(String.Format("{0:00000000}", Convert.ToInt64(dr[0]) + 1), "", "");
                    listCustomer.Add(User_Edit);
                }
                dr.Close();
                conn.Close();
                Keyboard.Focus(textbox_name); // 设置焦点
                lv.SelectedIndex = lv.Items.Count - 1; // 设置增加项被选中
                IsAdd = true;  
            }                                
        }

        /// <summary>
        /// 功能：保存用户
        /// 说明：1.在姓名、用户、密码三项没填完整的情况下，保存无效，提示不能为空
        ///       2.保存用户只在添加用户和修改用户前提下有效
        ///       3.用户名不可重复
        /// </summary>
        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            if (textbox_name.Text == "")
            {
                MessageBox.Show("姓名不能为空！");
                IsValid = false;
                Keyboard.Focus(textbox_name);
            }
            else if (textbox_login.Text == "")
            {
                MessageBox.Show("用户不能为空！");
                IsValid = false;
                Keyboard.Focus(textbox_login);
            }
            else if (textbox_password.Password == "")
            {
                MessageBox.Show("密码不能为空！");
                IsValid = false;
                Keyboard.Focus(textbox_password);
            }
            else
            {
                IsValid = true;
                Is_Repeat();
            }
            // 添加用户保存
            if (IsAdd == true && IsValid == true && IsRepeat == false)
            {                                       
                button_save.IsEnabled = false;               
                IsAdd = false;
                try
                {
                    string sql = String.Format("INSERT INTO t_xtgl_czry (name, login, pass, qx, sex, Email, mobile, dh, birthday, address, bz, rybm) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')",
                                    textbox_name.Text, textbox_login.Text, textbox_password.Password, combobox_Competence.Text, combobox_sex.Text, textbox_email.Text, 
                                    textbox_mobilephone.Text, textbox_fixedphone.Text, datepicker_birthday.Text, textbox_address.Text, textbox_remark.Text, User_Edit.EmployeeNumber);
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
                    Text_Clear();
                    lv.SelectedIndex = lv.Items.Count - 1;
                }
                finally
                {
                    conn.Close();
                    Text_Readonly();
                }
            }
            // 修改用户保存
            else if (IsModify == true && IsValid == true && IsRepeat == false)
            {
                button_save.IsEnabled = false;
                IsModify = false;
                try
                {
                    string sql_update = String.Format("UPDATE t_xtgl_czry SET name = '{0}', login = '{1}',pass='{2}' ,qx='{3}',sex='{4}',Email='{5}',mobile='{6}',dh='{7}',birthday='{8}',address='{9}',bz='{10}' WHERE rybm = '{11}'", textbox_name.Text, textbox_login.Text, textbox_password.Password, combobox_Competence.Text, combobox_sex.Text,
                                                      textbox_email.Text, textbox_mobilephone.Text, textbox_fixedphone.Text, datepicker_birthday.Text, textbox_address.Text, textbox_remark.Text, User_Edit.EmployeeNumber);
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
                textbox_name.Text = "";
                Keyboard.Focus(textbox_name);
            }                                             
        }

        /// <summary>
        /// 功能：将姓名显示在左侧 listview 对应框
        /// </summary>
        private void textbox_name_LostFocus(object sender, RoutedEventArgs e)
        {
            User_Edit.Name = textbox_name.Text;
        }

        /// <summary>
        /// 功能：将权限显示在左侧 listview 对应框
        /// </summary>
        private void combobox_Competence_LostFocus(object sender, RoutedEventArgs e)
        {
            User_Edit.Competence = combobox_Competence.Text;
        }

        /// <summary>
        /// 功能：删除用户
        /// </summary>
        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定要删除该项吗？", "确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                UserInfo userinfo = lv.SelectedItem as UserInfo;
                if (userinfo != null && userinfo is UserInfo)
                {
                    listCustomer.Remove(userinfo);
                }
                Text_Clear();
                try
                {
                    string sql = String.Format("delete from t_xtgl_czry where rybm = '{0}'", userinfo.EmployeeNumber);
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

        /// <summary>
        /// 功能：修改用户
        /// 说明：1.必须要有选中项
        ///       2.设置右边信息可编辑
        ///       3.设置保存按钮可用
        /// </summary>
        private void button_modify_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdd == false && IsModify == false)
            {
                if (lv.SelectedIndex == -1)
                {
                    MessageBox.Show("请预先选择需要修改的项！");
                }
                else
                {
                    IsModify = true;
                    Text_Editable();
                    button_save.IsEnabled = true;
                }  
            }                    
        }

        /// <summary>
        /// 功能：防止姓名重复
        /// </summary>
        public void Is_Repeat()
        {
            string username = textbox_name.Text.Trim();
            string sql = String.Format("select count(*) from t_xtgl_czry where name = '{0}' and rybm!='{1}' ", username, User_Edit.EmployeeNumber);
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
    }
}
