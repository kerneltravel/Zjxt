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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace 中医证治智能系统
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 定义连接字符串（全局变量）
        public static string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象（全局变量）
        public static SqlConnection conn = new SqlConnection(connString);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = combobox.Text.Trim();  // Trim()用于去除文本框中的前后空格
            string password = passwordbox.Password.Trim();
            Application.Current.Properties["user_name"] = username;

            // 获取用户名和密码匹配的行的数量的SQL语句
            string sql = String.Format("select count(*) from t_xtgl_czry where name = '{0}' and pass = '{1}'", username, password);
            try
            {
                // 打开与数据库的连接
                conn.Open();
                // 创建 Command 对象
                SqlCommand comm = new SqlCommand(sql, conn);
                // 执行查询语句，返回匹配的行数
                int num = (int)comm.ExecuteScalar();
                // 在User表中指定用户和密码的记录最多只有一行
                if (num == 1)
                {
                    // 如果有匹配的行，则表明用户名和密码正确                  
                    MainInterface maininterface = new MainInterface();
                    maininterface.Show();
                    this.Visibility = 0;
                    this.Close(); 
                }
                else
                {
                    MessageBox.Show("您输入的用户名或密码错误！", "登录失败", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    //combobox.Text = "";
                    passwordbox.Password = "";
                    Keyboard.Focus(passwordbox);

                }
            }
            catch (Exception)
            {
                MessageBox.Show("操作数据库失败！", "登录失败", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
            }
            finally
            {
                conn.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 设置初始化焦点位置
            Keyboard.Focus(combobox);
        }

        // 按下 Enter 键设置下一焦点位置
        private void combobox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Keyboard.Focus(passwordbox);
        }

        // 按下 Enter 键设置下一焦点位置
        private void passwordbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Keyboard.Focus(login);
        }

        private void quit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
  
        private void combobox_DropDownOpened(object sender, EventArgs e)
        {          
            string sql = String.Format("select name from t_xtgl_czry");
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            combobox.ItemsSource = ds.Tables[0].DefaultView;  //关键点why？
            combobox.DisplayMemberPath = "name";
            combobox.SelectedValuePath = "id";
        }
    }
}
