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
    /// Interaction logic for PatientinfoSearch.xaml
    /// </summary>
    public partial class PatientinfoSearch : Window
    {// 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 用于判断是否进行了 Add 操作
        bool IsAdd = false;
        // 用于判断是否进行了 Modify 操作
        bool IsModify = false;
        // 用于判断在进行保存操作时是否能将数据保存至数据库
        bool IsValid = true;
        // 全局变量，用于保存所选行数
        int Selected_Items = -1;
        // 全局变量，用于用户添加、修改功能存储信息
        PatientInfo Patient_Edit = new PatientInfo("", "", "", "", "", "", "");
        // 创建集合实例
        ObservableCollection<PatientInfo> listPatient = new ObservableCollection<PatientInfo>();
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


        public PatientinfoSearch()
        {
            InitializeComponent();
            save_yw.IsEnabled = false;
            Text_Readonly();
            lv.SelectedItem = -1;
        }

        public class PatientInfo : INotifyPropertyChanged
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
            private string _Name;
            private string _Gender;
            private string _Age;
            private string _Tel;
            private string _Addr;
            private string _Email;

            public string BrNumber
            {
                get { return _brNumber; }
                set { _brNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("BrNumber")); }
            }

            public string Name
            {
                get { return _Name; }
                set { _Name = value; OnPropertyChanged(new PropertyChangedEventArgs("Name")); }
            }

            public string Gender
            {
                get { return _Gender; }
                set { _Gender = value; OnPropertyChanged(new PropertyChangedEventArgs("Gender")); }
            }
            public string Age
            {
                get { return _Age; }
                set { _Age = value; OnPropertyChanged(new PropertyChangedEventArgs("Age")); }
            }
            public string Tel
            {
                get { return _Tel; }
                set { _Tel = value; OnPropertyChanged(new PropertyChangedEventArgs("Tel")); }
            }
            public string Addr
            {
                get { return _Addr; }
                set { _Addr = value; OnPropertyChanged(new PropertyChangedEventArgs("_Addr")); }
            }
            public string Email
            {
                get { return _Email; }
                set { _Email = value; OnPropertyChanged(new PropertyChangedEventArgs("Email")); }
            }
            public PatientInfo(string brnumber, string name, string gender, string age, string tel, string addr, string email)
            {
                _brNumber = brnumber;
                _Name = name;
                _Gender = gender;
                _Age = age;
                _Tel = tel;
                _Addr = addr;
                _Email = email;
            }
        }
        public void Text_Clear()
        {
            brbh.Text = "";
            xm.Text = "";
            gender.Text = "";
            age.Text = "";
            number.Text = "";
            email.Text = "";
            address.Text = "";
        }

        /// <summary>
        /// 功能：设置文本只读
        /// </summary>
        public void Text_Readonly()
        {
            xm.IsReadOnly = true;
            gender.IsReadOnly = true;
            age.IsReadOnly = true;
            number.IsReadOnly = true;
            email.IsReadOnly = true;
            address.IsReadOnly = true;
        }
        /// 功能：设置文本可读
        /// </summary>
        public void Text_Editable()
        {
            xm.IsReadOnly = false;
            gender.IsReadOnly = false;
            age.IsReadOnly = false;
            number.IsReadOnly = false;
            email.IsReadOnly = false;
            address.IsReadOnly = false;
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            if (brid.Text == "" && brname.Text == "")
            {
                string sql = String.Format("select * from t_br_info");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    listPatient.Add(new PatientInfo(dr["brid"].ToString(), dr["xm"].ToString(), dr["xb"].ToString(), dr["nl"].ToString(), dr["lxdh"].ToString(), dr["jtzz"].ToString(), dr["Email"].ToString()));
                }
                lv.Items.Clear(); //关键点，指定 ItemsSource 时，该项必须得清空?
                lv.ItemsSource = listPatient;
                dr.Close();
                conn.Close();
                lv.SelectedIndex = 0;
            }
            else if (brname.Text != "")
            {
                string sql = String.Format("select * from t_br_info where xm like'%{0}%'", brname.Text);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                listPatient.Clear();
                while (dr.Read())
                {

                    listPatient.Add(new PatientInfo(dr["brid"].ToString(), dr["xm"].ToString(), dr["xb"].ToString(), dr["nl"].ToString(), dr["lxdh"].ToString(), dr["jtzz"].ToString(), dr["Email"].ToString()));
                }
                lv.ItemsSource = listPatient;
                dr.Close();
                conn.Close();


            }
            else if (brid.Text != "")
            {
                string sql = String.Format("select * from t_br_info where brid='{0}'", brid.Text);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                listPatient.Clear();
                while (dr.Read())
                {

                    listPatient.Add(new PatientInfo(dr["brid"].ToString(), dr["xm"].ToString(), dr["xb"].ToString(), dr["nl"].ToString(), dr["lxdh"].ToString(), dr["jtzz"].ToString(), dr["Email"].ToString()));
                }
                lv.ItemsSource = listPatient;
                dr.Close();
                conn.Close();
            }
        }

        private void back_search_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv.SelectedIndex != lv.Items.Count - 1 && IsAdd)
            {
                if (xm.Text == "")
                {
                    MessageBox.Show("姓名不能为空！");
                    lv.SelectedIndex = lv.Items.Count - 1;
                }
                else
                {
                    if (gender.Text == "")
                    {
                        MessageBox.Show("性别不能为空！");
                        lv.SelectedIndex = lv.Items.Count - 1;
                    }
                    else
                    {
                        if (age.Text == "")
                        {
                            MessageBox.Show("年龄不能为空！");
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
                if (xm.Text == "")
                {
                    MessageBox.Show("姓名不能为空！");
                    lv.SelectedIndex = Selected_Items;
                    Keyboard.Focus(xm);
                }
                else
                {
                    if (gender.Text == "")
                    {
                        MessageBox.Show("性别不能为空！");
                        lv.SelectedIndex = Selected_Items;
                        Keyboard.Focus(gender);
                    }
                    else
                    {
                        if (age.Text == "")
                        {
                            MessageBox.Show("年龄不能为空！");
                            lv.SelectedIndex = Selected_Items;
                            Keyboard.Focus(age);
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
                Patient_Edit = lv.SelectedItem as PatientInfo;
                Selected_Items = lv.SelectedIndex;
                if (Patient_Edit != null && Patient_Edit is PatientInfo)
                {
                    string sql = String.Format("select * from t_br_info where brid= '{0}'", Patient_Edit.BrNumber);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        brbh.Text = dr["brid"].ToString();
                        xm.Text = dr["xm"].ToString();
                        gender.Text = dr["xb"].ToString();
                        age.Text = dr["nl"].ToString();
                        number.Text = dr["lxdh"].ToString();
                        email.Text = dr["Email"].ToString();
                        address.Text = dr["jtzz"].ToString();
                    }
                    dr.Close();
                    conn.Close();
                }
            }
        }

        private void add_yw_Click(object sender, RoutedEventArgs e)
        {
            Text_Editable();
            if (IsAdd == false && IsModify == false)
            {
                Text_Clear();
                save_yw.IsEnabled = true;
                string sql = String.Format("select max(brid) from t_br_info");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    brbh.Text = (Convert.ToInt64(dr[0]) + 1).ToString();
                    Patient_Edit = new PatientInfo((Convert.ToInt64(dr[0]) + 1).ToString(), "", "", "", "", "", "");
                    listPatient.Add(Patient_Edit);
                }
                dr.Close();
                conn.Close();
                Keyboard.Focus(xm); // 设置焦点
                lv.ItemsSource = listPatient;
                lv.SelectedIndex = lv.Items.Count - 1; // 设置增加项被选中
                IsAdd = true;
            }

        }

        private void xm_LostFocus(object sender, RoutedEventArgs e)
        {
            Patient_Edit.Name = xm.Text;
        }

        private void age_LostFocus(object sender, RoutedEventArgs e)
        {
            Patient_Edit.Age = age.Text;
        }

        private void number_LostFocus(object sender, RoutedEventArgs e)
        {
            Patient_Edit.Tel = number.Text;
        }

        private void email_LostFocus(object sender, RoutedEventArgs e)
        {
            Patient_Edit.Email = email.Text;
        }

        private void address_LostFocus(object sender, RoutedEventArgs e)
        {
            Patient_Edit.Addr = address.Text;
        }

        private void delete_yw_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定要删除该项吗？", "确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                PatientInfo userinfo = lv.SelectedItem as PatientInfo;
                if (userinfo != null && userinfo is PatientInfo)
                {
                    listPatient.Remove(userinfo);
                }
                Text_Clear();
                try
                {
                    string sql = String.Format("delete from t_br_info where brid = '{0}'", userinfo.BrNumber);
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

        private void select_search_Click(object sender, RoutedEventArgs e)
        {
            PatientInfo userinfo = lv.SelectedItem as PatientInfo;
            if (userinfo != null && userinfo is PatientInfo)
            {
                PassValuesEventArgs args = new PassValuesEventArgs(userinfo.Name.ToString(), userinfo.BrNumber.ToString());
                // 要判断 PassValuesEvent 是否为空，即判断该窗口是否被调用
                if (PassValuesEvent != null)
                {
                    PassValuesEvent(this, args);
                }
            }
            this.Close();
        }

        private void xb_closed(object sender, EventArgs e)
        {
            Patient_Edit.Gender = gender.Text;
        }

        private void modify_yw_Click(object sender, RoutedEventArgs e)
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
                    save_yw.IsEnabled = true;
                }
            }

        }

        private void save_yw_Click(object sender, RoutedEventArgs e)
        {
            if (xm.Text == "")
            {
                MessageBox.Show("姓名不能为空！");
                IsValid = false;
                Keyboard.Focus(xm);
            }
            else if (gender.Text == "")
            {
                MessageBox.Show("性别不能为空！");
                IsValid = false;
                Keyboard.Focus(gender);
            }
            else if (age.Text == "")
            {
                MessageBox.Show("年龄不能为空！");
                IsValid = false;
                Keyboard.Focus(age);
            }
            else
            {
                IsValid = true;

            }
            // 添加用户保存
            if (IsAdd == true && IsValid == true)
            {
                save_yw.IsEnabled = false;
                IsAdd = false;
                try
                {
                    string sql = String.Format("INSERT INTO t_br_info (xm, xb, nl, lxdh, jtzz, Email, bz) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')", xm.Text, gender.Text, age.Text, number.Text, address.Text, email.Text, "");
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
                    listPatient.Remove(Patient_Edit);
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
            else if (IsModify == true && IsValid == true)
            {
                save_yw.IsEnabled = false;
                IsModify = false;
                try
                {
                    string sql_update = String.Format("UPDATE t_br_info SET xm = '{0}', xb = '{1}',nl='{2}' ,lxdh='{3}',jtzz='{4}',Email='{5}' WHERE brid = '{6}'", xm.Text, gender.Text, age.Text, number.Text, address.Text, email.Text, Patient_Edit.BrNumber);
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

        }

        private void cancel_yw_Click(object sender, RoutedEventArgs e)
        {
            save_yw.IsEnabled = false;
            Patient_Edit = lv.SelectedItem as PatientInfo;
            Selected_Items = lv.SelectedIndex;
            if (Patient_Edit != null && Patient_Edit is PatientInfo)
            {
                string sql = String.Format("select * from t_br_info where brid= '{0}'", Patient_Edit.BrNumber);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    brbh.Text = dr["brid"].ToString();
                    xm.Text = dr["xm"].ToString();
                    gender.Text = dr["xb"].ToString();
                    age.Text = dr["nl"].ToString();
                    number.Text = dr["lxdh"].ToString();
                    email.Text = dr["Email"].ToString();
                    address.Text = dr["jtzz"].ToString();
                }
                dr.Close();
                conn.Close();
            }
        }
    }
}
