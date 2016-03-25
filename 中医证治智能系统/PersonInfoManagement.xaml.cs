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
    /// Interaction logic for PersonalInfoAdmin.xaml
    /// </summary>
    public partial class PersonalInfoAdmin : Window
    {
        string nu;
        string ID;
        bool valid = true;
        bool IsRepeat = false;

        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        static public SqlConnection conn = new SqlConnection(connString);
        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            string sql = String.Format("select qxzmc from t_info_qxz");
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            combobox_Competence.ItemsSource = ds.Tables[0].DefaultView;  //关键点why？
            combobox_Competence.DisplayMemberPath = "qxzmc";
            combobox_Competence.SelectedValuePath = "subid";
        }
        private void personalinfoadmin_Loaded(object sender, RoutedEventArgs e)
        {


        }
        public PersonalInfoAdmin()
        {
            InitializeComponent();
            string y = Application.Current.Properties["user_name"].ToString();



            string sql = String.Format("select * from t_xtgl_czry where name='{0}'", y);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nu = dr["rybm"].ToString();
                ID = dr["id"].ToString();
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
            Text_Readonly();

        }

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

        // 功能：设置文本可读
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

        private void button_return_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void button_change_Click(object sender, RoutedEventArgs e)
        {
            Text_Editable();

        }

        private void button_save1_Click(object sender, RoutedEventArgs e)
        {

            if (textbox_name.Text == "")
            {
                MessageBox.Show("姓名不能为空！");
                valid = false;
                Keyboard.Focus(textbox_name);
            }
            else if (textbox_login.Text == "")
            {
                MessageBox.Show("用户不能为空！");
                valid = false;
                Keyboard.Focus(textbox_login);
            }
            else if (textbox_password.Password == "")
            {
                MessageBox.Show("密码不能为空！");
                valid = false;
                Keyboard.Focus(textbox_password);

            }
            else
            {
                valid = true;
                Is_Repeat();
            }
            // 添加用户保存
            if (valid == true && IsRepeat == false)
            {
                // 前面三项满足后才能保存至数据库


                try
                {
                    string sql = String.Format("UPDATE t_xtgl_czry SET name = '{0}', login = '{1}',pass='{2}' ,qx='{3}',sex='{4}',Email='{5}',mobile='{6}' , dh='{7}',birthday='{8}',address='{9}',bz='{10}' WHERE rybm = '{11}'", textbox_name.Text, textbox_login.Text, textbox_password.Password, combobox_Competence.Text, combobox_sex.Text,
                                   textbox_email.Text, textbox_mobilephone.Text, textbox_fixedphone.Text, datepicker_birthday.Text, textbox_address.Text, textbox_remark.Text, nu);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    if (count > 0)
                    {
                        MessageBox.Show("保存成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                finally
                {
                    conn.Close();
                    Text_Readonly();
                }
            }
        }
        public void Is_Repeat()
        {

            string username = textbox_name.Text.Trim();
            string sql = String.Format("select count(*) from t_xtgl_czry where name = '{0}' and  id!='{1}'", username, ID);
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

        private void button_cancel_Click_1(object sender, RoutedEventArgs e)
        {
            string y = Application.Current.Properties["user_name"].ToString();



            string sql = String.Format("select * from t_xtgl_czry where name='{0}'", y);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nu = dr["rybm"].ToString();
                ID = dr["id"].ToString();
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

        private void combobox_Competence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
