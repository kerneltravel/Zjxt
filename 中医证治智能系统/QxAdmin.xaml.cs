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
    /// Interaction logic for QxAdmin.xaml
    /// </summary>
    public partial class QxAdmin : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 全局变量，用于存储权限组ID
        string id = "";
        // 全局变量，用于判断是否重复添加
        bool isrepeat = false;
        // 全局变量，用于判断是否进行添加
        bool IsAdd = false;
        // 全局变量，用于判断是否进行修改
        bool IsModify = false;
        // 全局变量，用于存储添加或修改前的选中的权限组位置
        int position = -1;
        
        // 初始化
        public QxAdmin()
        {
            InitializeComponent();
            // 刷新权限组
            Freshqx();
            // 初始化选项组选项
            listbox.SelectedIndex = 0;
            txt_qxzmc.IsEnabled = false;
        }

        /// <summary>
        /// 功能：返回
        /// </summary>
        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：刷新权限组名称
        /// </summary>
        private void Freshqx()
        {
            listbox.Items.Clear();
            string sql = String.Format("select qxzmc from t_info_qxz");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listbox.Items.Add(dr[0].ToString());
            }
            dr.Close();
            conn.Close();
        }

        /// <summary>
        /// 功能：判断是否重复添加
        /// </summary>
        private void Isrepeat()
        {
            if (IsAdd)
            {
                if (txt_qxzmc.Text.ToString() == "")
                {
                    MessageBox.Show("权限组名称不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    string sql = String.Format("select count(*) from t_info_qxz where qxzmc = '{0}'", txt_qxzmc.Text.ToString());
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = (int)comm.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("权限组名称不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        isrepeat = true;
                    }
                    else
                        isrepeat = false;
                    conn.Close();
                }                
            }
            if (IsModify)
            {
                if (txt_qxzmc.Text.ToString() == "")
                {

                }
                else
                {
                    string sql = String.Format("select count(*) from t_info_qxz where qxzmc = '{0}' and qxzmc != '{1}'", txt_qxzmc.Text.ToString(), listbox.Items[listbox.SelectedIndex].ToString());
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = (int)comm.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("该权限组名称已存在！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        isrepeat = true;
                    }
                    else
                        isrepeat = false;
                    conn.Close();   
                }                            
            }
        }

        /// <summary>
        /// 功能：清除权限
        /// </summary>
        private void Clearqx() 
        {
            for (int i = 0; i < listbox_1.Items.Count; i++ )
            {
                CheckBox checkbox = listbox_1.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = false;
                }
            }
            for (int i = 0; i < listbox_2.Items.Count; i++)
            {
                CheckBox checkbox = listbox_2.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = false;
                }
            }
            for (int i = 0; i < listbox_3.Items.Count; i++)
            {
                CheckBox checkbox = listbox_3.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = false;
                }
            }
            for (int i = 0; i < listbox_4.Items.Count; i++)
            {
                CheckBox checkbox = listbox_4.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = false;
                }
            }
            for (int i = 0; i < listbox_5.Items.Count; i++)
            {
                CheckBox checkbox = listbox_5.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = false;
                }
            }
            for (int i = 0; i < listbox_6.Items.Count; i++)
            {
                CheckBox checkbox = listbox_6.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = false;
                }
            }
        }

        /// <summary>
        /// 功能：查询匹配项
        /// </summary>
        /// <param name="Qxck"></param>
        private void Searchqx(string Qxck)
        {
            string qxck = Qxck;
            for (int i = 0; i < listbox_1.Items.Count; i++)
            {
                CheckBox checkbox = listbox_1.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    if (qxck  == checkbox.Content.ToString())
                    {
                        checkbox.IsChecked = true;
                    }
                }
            }
            for (int i = 0; i < listbox_2.Items.Count; i++)
            {
                CheckBox checkbox = listbox_2.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    if (qxck == checkbox.Content.ToString())
                    {
                        checkbox.IsChecked = true;
                    }
                }
            }
            for (int i = 0; i < listbox_3.Items.Count; i++)
            {
                CheckBox checkbox = listbox_3.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    if (qxck == checkbox.Content.ToString())
                    {
                        checkbox.IsChecked = true;
                    }
                }
            }
            for (int i = 0; i < listbox_4.Items.Count; i++)
            {
                CheckBox checkbox = listbox_4.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    if (qxck == checkbox.Content.ToString())
                    {
                        checkbox.IsChecked = true;
                    }
                }
            }
            for (int i = 0; i < listbox_5.Items.Count; i++)
            {
                CheckBox checkbox = listbox_5.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    if (qxck == checkbox.Content.ToString())
                    {
                        checkbox.IsChecked = true;
                    }
                }
            }
            for (int i = 0; i < listbox_6.Items.Count; i++)
            {
                CheckBox checkbox = listbox_6.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    if (qxck == checkbox.Content.ToString())
                    {
                        checkbox.IsChecked = true;
                    }
                }
            }
        }

        /// <summary>
        /// 功能：实现从数据库读取数据并完成 checkbox 的填写
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        private void Qxzgl(string Sql)
         {
            string sql = Sql;
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                string qxck = dr[0].ToString();
                Searchqx(qxck);
            }
            dr.Close();
            conn.Close();
        }

        /// <summary>
        /// 功能：在添加情况下保存权限组
        /// </summary>
        private void Save_Add()
        {
            // 保存权限组名称[t_info_qxz]
            string sql = String.Format("insert into t_info_qxz (qxzmc) values ('{0}') ", txt_qxzmc.Text.ToString());
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = comm.ExecuteNonQuery();
            conn.Close();
            // 读取ID
            string ID = "";
            sql = String.Format("select max(subid) from t_info_qxz");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                ID = dr[0].ToString();
            }
            dr.Close();
            conn.Close();
            // 保存权限组明细[t_info_qxzmx]
            for (int i = 0; i < listbox_1.Items.Count; i++)
            {
                CheckBox checkbox = listbox_1.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox && checkbox.IsChecked == true)
                {
                    //MessageBox.Show(checkbox.Content.ToString());
                    sql = String.Format("insert into t_info_qxzmx ( id, qxck) values ('{0}','{1}') ", ID, checkbox.Content.ToString());
                    conn.Open(); 
                    comm = new SqlCommand(sql, conn);
                    count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            for (int i = 0; i < listbox_2.Items.Count; i++)
            {
                CheckBox checkbox = listbox_2.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox && checkbox.IsChecked == true)
                {
                    sql = String.Format("insert into t_info_qxzmx ( id, qxck) values ('{0}','{1}') ", ID, checkbox.Content.ToString());
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            for (int i = 0; i < listbox_3.Items.Count; i++)
            {
                CheckBox checkbox = listbox_3.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox && checkbox.IsChecked == true)
                {
                    sql = String.Format("insert into t_info_qxzmx ( id, qxck) values ('{0}','{1}') ", ID, checkbox.Content.ToString());
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            for (int i = 0; i < listbox_4.Items.Count; i++)
            {
                CheckBox checkbox = listbox_4.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox && checkbox.IsChecked == true)
                {
                    sql = String.Format("insert into t_info_qxzmx ( id, qxck) values ('{0}','{1}') ", ID, checkbox.Content.ToString());
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            for (int i = 0; i < listbox_5.Items.Count; i++)
            {
                CheckBox checkbox = listbox_5.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox && checkbox.IsChecked == true)
                {
                    sql = String.Format("insert into t_info_qxzmx ( id, qxck) values ('{0}','{1}') ", ID, checkbox.Content.ToString());
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            for (int i = 0; i < listbox_6.Items.Count; i++)
            {
                CheckBox checkbox = listbox_6.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox && checkbox.IsChecked == true)
                {
                    sql = String.Format("insert into t_info_qxzmx ( id, qxck) values ('{0}','{1}') ", ID, checkbox.Content.ToString());
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            // 保存完成后将 IsAdd = false
            IsAdd = false;
        }

        /// <summary>
        /// 功能：在修改情况下保存权限组
        /// </summary>
        private void Save_Modify()
        {
            // 读取ID
            string ID = "";
            string sql = String.Format("select subid from t_info_qxz where qxzmc = '{0}'",listbox.Items[listbox.SelectedIndex].ToString());
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                ID = dr[0].ToString();
            }
            dr.Close();
            conn.Close();
            // 保存权限组名称[t_info_qxz]
            sql = String.Format("update t_info_qxz set qxzmc = '{0}' where subid = '{1}'", txt_qxzmc.Text.ToString(), ID);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            int count = comm.ExecuteNonQuery();
            conn.Close();
            // 删除权限组明细[t_info_qxzmx]
            sql = String.Format("delete from t_info_qxzmx where id = '{0}'", ID);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            count = comm.ExecuteNonQuery();
            conn.Close();
            // 保存权限组明细[t_info_qxzmx]
            for (int i = 0; i < listbox_1.Items.Count; i++)
            {
                CheckBox checkbox = listbox_1.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox && checkbox.IsChecked == true)
                {
                    //MessageBox.Show(checkbox.Content.ToString());
                    sql = String.Format("insert into t_info_qxzmx ( id, qxck) values ('{0}','{1}') ", ID, checkbox.Content.ToString());
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            for (int i = 0; i < listbox_2.Items.Count; i++)
            {
                CheckBox checkbox = listbox_2.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox && checkbox.IsChecked == true)
                {
                    sql = String.Format("insert into t_info_qxzmx ( id, qxck) values ('{0}','{1}') ", ID, checkbox.Content.ToString());
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            for (int i = 0; i < listbox_3.Items.Count; i++)
            {
                CheckBox checkbox = listbox_3.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox && checkbox.IsChecked == true)
                {
                    sql = String.Format("insert into t_info_qxzmx ( id, qxck) values ('{0}','{1}') ", ID, checkbox.Content.ToString());
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            for (int i = 0; i < listbox_4.Items.Count; i++)
            {
                CheckBox checkbox = listbox_4.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox && checkbox.IsChecked == true)
                {
                    sql = String.Format("insert into t_info_qxzmx ( id, qxck) values ('{0}','{1}') ", ID, checkbox.Content.ToString());
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            for (int i = 0; i < listbox_5.Items.Count; i++)
            {
                CheckBox checkbox = listbox_5.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox && checkbox.IsChecked == true)
                {
                    sql = String.Format("insert into t_info_qxzmx ( id, qxck) values ('{0}','{1}') ", ID, checkbox.Content.ToString());
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            for (int i = 0; i < listbox_6.Items.Count; i++)
            {
                CheckBox checkbox = listbox_6.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox && checkbox.IsChecked == true)
                {
                    sql = String.Format("insert into t_info_qxzmx ( id, qxck) values ('{0}','{1}') ", ID, checkbox.Content.ToString());
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            // 保存完成后将 IsModify = false
            IsModify = false;
        }

        /// <summary>
        /// 功能：权限组选择变化时触发事件
        /// </summary>
        private void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsAdd || IsModify)
            {
                if (listbox.SelectedIndex != position)
                MessageBox.Show("请先完成当前操作或选择返回！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                listbox.SelectedIndex = position;
            }
            else
            {
                if (listbox.SelectedIndex != -1)
                {
                    checkbox1_1.IsChecked = false;
                    checkbox1_2.IsChecked = false;
                    checkbox2_1.IsChecked = false;
                    checkbox2_2.IsChecked = false;
                    checkbox3_1.IsChecked = false;
                    checkbox3_2.IsChecked = false;
                    checkbox4_1.IsChecked = false;
                    checkbox4_2.IsChecked = false;
                    checkbox5_1.IsChecked = false;
                    checkbox5_2.IsChecked = false;
                    checkbox6_1.IsChecked = false;
                    checkbox6_2.IsChecked = false;
                    checkbox1_2.IsChecked = false;
                    Clearqx();
                    string qxzmc = listbox.Items[listbox.SelectedIndex].ToString();
                    txt_qxzmc.Text = qxzmc;
                    string sql = String.Format("select qxck from t_info_qxz t1 inner join t_info_qxzmx t2 on t1.subid = t2.id where t1.qxzmc = '{0}'"
                        , qxzmc);
                    Qxzgl(sql);
                } 
            }                              
        }
       
        /*****************************************************/ 
        /*****************设置全选和全不选*********************/
        /*****************************************************/ 
        private void checkbox1_1_Checked(object sender, RoutedEventArgs e)
        {
            checkbox1_2.IsChecked = false;
            for (int i = 0; i < listbox_1.Items.Count; i++)
            {
                CheckBox checkbox = listbox_1.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = true;
                }
            }
        }

        private void checkbox1_2_Checked(object sender, RoutedEventArgs e)
        {
            checkbox1_1.IsChecked = false;
            for (int i = 0; i < listbox_1.Items.Count; i++)
            {
                CheckBox checkbox = listbox_1.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = false;
                }
            }
        }

        private void checkbox2_1_Checked(object sender, RoutedEventArgs e)
        {
            checkbox2_2.IsChecked = false;
            for (int i = 0; i < listbox_2.Items.Count; i++)
            {
                CheckBox checkbox = listbox_2.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = true;
                }
            }
        }

        private void checkbox2_2_Checked(object sender, RoutedEventArgs e)
        {
            checkbox2_1.IsChecked = false;
            for (int i = 0; i < listbox_2.Items.Count; i++)
            {
                CheckBox checkbox = listbox_2.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = false;
                }
            }
        }

        private void checkbox3_1_Checked(object sender, RoutedEventArgs e)
        {
            checkbox3_2.IsChecked = false;
            for (int i = 0; i < listbox_3.Items.Count; i++)
            {
                CheckBox checkbox = listbox_3.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = true;
                }
            }
        }

        private void checkbox3_2_Checked(object sender, RoutedEventArgs e)
        {
            checkbox3_1.IsChecked = false;
            for (int i = 0; i < listbox_3.Items.Count; i++)
            {
                CheckBox checkbox = listbox_3.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = false;
                }
            }
        }

        private void checkbox4_1_Checked(object sender, RoutedEventArgs e)
        {
            checkbox4_2.IsChecked = false;
            for (int i = 0; i < listbox_4.Items.Count; i++)
            {
                CheckBox checkbox = listbox_4.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = true;
                }
            }
        }

        private void checkbox4_2_Checked(object sender, RoutedEventArgs e)
        {
            checkbox4_1.IsChecked = false;
            for (int i = 0; i < listbox_4.Items.Count; i++)
            {
                CheckBox checkbox = listbox_4.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = false;
                }
            }
        }

        private void checkbox5_1_Checked(object sender, RoutedEventArgs e)
        {
            checkbox5_2.IsChecked = false;
            for (int i = 0; i < listbox_5.Items.Count; i++)
            {
                CheckBox checkbox = listbox_5.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = true;
                }
            }
        }

        private void checkbox5_2_Checked(object sender, RoutedEventArgs e)
        {
            checkbox5_1.IsChecked = false;
            for (int i = 0; i < listbox_5.Items.Count; i++)
            {
                CheckBox checkbox = listbox_5.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = false;
                }
            }
        }

        private void checkbox6_1_Checked(object sender, RoutedEventArgs e)
        {
            checkbox6_2.IsChecked = false;
            for (int i = 0; i < listbox_6.Items.Count; i++)
            {
                CheckBox checkbox = listbox_6.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = true;
                }
            }
        }

        private void checkbox6_2_Checked(object sender, RoutedEventArgs e)
        {
            checkbox6_1.IsChecked = false;
            for (int i = 0; i < listbox_6.Items.Count; i++)
            {
                CheckBox checkbox = listbox_6.Items[i] as CheckBox;
                if (checkbox != null && checkbox is CheckBox)
                {
                    checkbox.IsChecked = false;
                }
            }
        }

        /// <summary>
        /// 功能：【增加】
        /// </summary>
        private void add_Click(object sender, RoutedEventArgs e)
        {            
            txt_qxzmc.IsEnabled = true;            
            listbox.Items.Add("");
            listbox.SelectedIndex = listbox.Items.Count - 1;
            IsAdd = true;
            position = listbox.SelectedIndex;
            txt_qxzmc.Text = "";
        }

        /// <summary>
        /// 功能：【修改】
        /// </summary>
        private void modify_Click(object sender, RoutedEventArgs e)
        {
            position = listbox.SelectedIndex;
            txt_qxzmc.IsEnabled = true;
            IsModify = true;
        }

        /// <summary>
        /// 功能：【保存】
        /// </summary>
        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdd)
            {
                Isrepeat();
                if (!isrepeat)
                {
                    Save_Add();                    
                    Freshqx();
                    txt_qxzmc.IsEnabled = false;
                }
            }
            if (IsModify)
            {
                Isrepeat();
                if (!isrepeat)
                {
                    Save_Modify();
                    Freshqx();
                    txt_qxzmc.IsEnabled = false;
                }
            }           
        }

        /// <summary>
        /// 功能：【删除】
        /// </summary>
        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (listbox.SelectedIndex == -1)
            {
                MessageBox.Show("请选择要删除的规则！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // 读取ID
                string ID = "";
                string sql = String.Format("select subid from t_info_qxz where qxzmc = '{0}'", txt_qxzmc.Text.ToString());
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    ID = dr[0].ToString();
                }
                dr.Close();
                conn.Close();
                // 删除【t_info_qxz】
                sql = String.Format("delete from t_info_qxz where qxzmc = '{0}'", txt_qxzmc.Text.ToString());
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count = comm.ExecuteNonQuery();
                conn.Close();
                // 删除【t_info_qxzmx】
                sql = String.Format("delete from t_info_qxzmx where id = '{0}'", ID);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                conn.Close();
                // 刷新
                Freshqx();
                // 焦点上移
                listbox.SelectedIndex = listbox.Items.Count - 1;
            }           
        }
    }
}
