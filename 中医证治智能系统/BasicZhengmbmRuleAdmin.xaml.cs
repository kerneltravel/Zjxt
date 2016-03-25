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
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.Windows.Markup;

namespace 中医证治智能系统
{
    /// <summary>
    /// Interaction logic for BasicZhengmbmRuleAdmin.xaml
    /// </summary>
    public partial class BasicZhengmbmRuleAdmin : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 创建集合实例
        ObservableCollection<BasicZhengmbmInfo> listZhengmbm = new ObservableCollection<BasicZhengmbmInfo>();
        // 全局变量，用于暂时存储主证编号
        public string m_jbzmbh = "";
        // 全局变量，用于暂时存储辅证编号
        public string m_bmbh = "";
        // 用于判断是否进行了增加操作
        bool IsAdd = false;
        // 用于判断是否重复添加
        bool IsRepeat = false;
        
        // 创建集合实例
        ObservableCollection<BasicZhengmbmInfo> listZhengmbm1 = new ObservableCollection<BasicZhengmbmInfo>();
        // 全局变量，用于暂时存储主证编号
        public string m_jbzmbh1 = "";
        // 全局变量，用于暂时存储辅证编号
        public string m_bmbh1 = "";
        // 用于判断是否进行了增加操作
        bool IsAdd1 = false;
        // 用于判断是否重复添加
        bool IsRepeat1 = false;

        /// <summary>
        /// 功能：类的构造函数，用于初始化
        /// </summary>
        public BasicZhengmbmRuleAdmin()
        {
            InitializeComponent();
            // 指定 listview 数据源
            lv.ItemsSource = listZhengmbm;
            lv1.ItemsSource = listZhengmbm1;
            save.IsEnabled = false;
            save1.IsEnabled = false;
        }

        /// <summary>
        /// 功能：创建基本证名信息类
        /// 说明：1.JibzmNumber --> 基本证名编号  JibzmName --> 基本证名名称  BingmNumber --> 病名编号
        ///         BingmName --> 病名名称 
        /// </summary>
        public class BasicZhengmbmInfo : INotifyPropertyChanged
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

            private string _JibzmNumber;
            private string _JibzmName;
            private string _BingmNumber;
            private string _BingmName;

            public string JibzmNumber
            {
                get { return _JibzmNumber; }
                set { _JibzmNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("JibzmNumber")); }
            }

            public string JibzmName
            {
                get { return _JibzmName; }
                set { _JibzmName = value; OnPropertyChanged(new PropertyChangedEventArgs("JibzmName")); }
            }

            public string BingmNumber
            {
                get { return _BingmNumber; }
                set { _BingmNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("BingmNumber")); }
            }

            public string BingmName
            {
                get { return _BingmName; }
                set { _BingmName = value; OnPropertyChanged(new PropertyChangedEventArgs("BingmName")); }
            }


            public BasicZhengmbmInfo(string jibzmnumber, string jibzmname, string bingmnumber, string bingmname)
            {
                _JibzmNumber = jibzmnumber;
                _JibzmName = jibzmname;
                _BingmNumber = bingmnumber;
                _BingmName = bingmname;
            }
        }

        // 判断是否重复添加
        public void Is_Repeat()
        {
            string sql = String.Format("select count(*) from t_info_jbzmbm where jbzmbh = '{0}' and bmbh='{1}' ", m_jbzmbh, m_bmbh);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count == 1)
            {
                MessageBox.Show("该记录已存在！", "消息", MessageBoxButton.OK, MessageBoxImage.Question);
                IsRepeat = true;

            }
            else
                IsRepeat = false;
            conn.Close();
        }

        // 判断是否重复添加
        public void Is_Repeat1()
        {
            string sql = String.Format("select count(*) from t_info_jbzmbm where jbzmbh = '{0}' and bmbh='{1}' ", m_jbzmbh1, m_bmbh1);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count == 1)
            {
                MessageBox.Show("该记录已存在！", "消息", MessageBoxButton.OK, MessageBoxImage.Question);
                IsRepeat = true;

            }
            else
                IsRepeat = false;
            conn.Close();
        }

        /// <summary>
        /// 功能：调用【基本证名信息管理】窗口,【选定】病名名称
        /// </summary>
        private void btn_jbzmmc_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdd)
            {
                MessageBox.Show("请先保存或取消！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                BasicZhengm basiczhengm = new BasicZhengm();
                basiczhengm.PassValuesEvent += new BasicZhengm.PassValuesHandler(ReceiveValues);
                basiczhengm.Show();
            }
        }

        /// <summary>
        /// 功能：实现基本证名名称的读取和显示
        /// </summary>
        private void ReceiveValues(object sender, BasicZhengm.PassValuesEventArgs e)
        {
            listZhengmbm.Clear(); // 先清空集合
            jbzmmc.Text = e.Name;
            m_jbzmbh = e.Number;
            // 读取主证编号
            string sql = String.Format("select t2.bmbh, bmmc from t_info_jbzmbm as t1 inner join t_info_bm as t2 on t1.bmbh = t2.bmbh  where jbzmbh = '{0}'", m_jbzmbh);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listZhengmbm.Add(new BasicZhengmbmInfo(m_jbzmbh, jbzmmc.Text, dr["bmbh"].ToString(), dr["bmmc"].ToString()));
            }
            dr.Close();
            conn.Close();
            lv.SelectedIndex = 0;
        }

        /// <summary>
        /// 功能：证名合并规则管理【返回】操作
        /// </summary>
        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：调用【病名信息管理】窗口,【选定】病名名称
        /// </summary>
        private void btn_bmmc_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdd)
            {
                MessageBox.Show("表未处于添加状态！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                DiseaseInfoAdmin diseaseinfoadmin = new DiseaseInfoAdmin();
                diseaseinfoadmin.PassValuesEvent += new DiseaseInfoAdmin.PassValuesHandler(ReceiveValues1);
                diseaseinfoadmin.Show();
            }
        }

        /// <summary>
        /// 功能：实现病名名称的读取和显示
        /// </summary>
        private void ReceiveValues1(object sender, DiseaseInfoAdmin.PassValuesEventArgs e)
        {
            if (lv.SelectedIndex == lv.Items.Count - 1)
            {
                bmmc.Text = e.Name;
                m_bmbh = e.Number;
                BasicZhengmbmInfo basiczhengmbminfo = lv.SelectedItem as BasicZhengmbmInfo;
                if (basiczhengmbminfo != null && basiczhengmbminfo is BasicZhengmbmInfo)
                {
                    basiczhengmbminfo.BingmNumber = m_bmbh;
                    basiczhengmbminfo.BingmName = bmmc.Text;
                }
            }
        }

        /// <summary>
        /// 功能：【增加】
        /// </summary>
        private void add_Click(object sender, RoutedEventArgs e)
        {
            if (jbzmmc.Text != "")
            {
                //listZhengmbm.Clear();
                IsAdd = true;
                save.IsEnabled = true;
                listZhengmbm.Add(new BasicZhengmbmInfo(m_jbzmbh, jbzmmc.Text, "", ""));
                lv.SelectedIndex = lv.Items.Count - 1;
            }
            else
            {
                MessageBox.Show("请输入证名名称！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 功能：【取消】
        /// 说明：1.从数据库将数据读取一遍即可！
        /// </summary>
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            // 在增加操作下才可执行
            if (IsAdd)
            {
                listZhengmbm.Clear();
                // 读取主证编号
                string sql = String.Format("select t3.jbzmmc, t2.bmbh, t2.bmmc from (t_info_jbzmbm as t1 inner join t_info_bm as t2 on t1.bmbh = t2.bmbh) inner join t_info_jbzm as t3 on t1.jbzmbh = t3.jbzmbh where t1.jbzmbh = '{0}'", m_jbzmbh);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    listZhengmbm.Add(new BasicZhengmbmInfo(m_jbzmbh, dr["jbzmmc"].ToString(), dr["bmbh"].ToString(), dr["bmmc"].ToString()));
                }
                dr.Close();
                conn.Close();
                lv.SelectedIndex = 0;
                bmmc.Text = "";
                save.IsEnabled = false;
                IsAdd = false;
            }
        }

        /// <summary>
        /// 功能：【删除】
        /// </summary>
        private void delete_Click(object sender, RoutedEventArgs e)
        {
            // 在非【增加】操作下才可执行
            if (!IsAdd && lv.SelectedIndex != -1)
            {
                string jbzmbh = "";
                string bmbh = "";
                BasicZhengmbmInfo basiczhengmbminfo = lv.SelectedItem as BasicZhengmbmInfo;
                if (basiczhengmbminfo != null && basiczhengmbminfo is BasicZhengmbmInfo)
                {
                    jbzmbh = basiczhengmbminfo.JibzmNumber;
                    bmbh = basiczhengmbminfo.BingmNumber;
                }
                if (MessageBox.Show("您确认要删除该记录吗？", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    try
                    {
                        string sql = String.Format("delete from t_info_jbzmbm where jbzmbh = '{0}' and bmbh = '{1}'", jbzmbh, bmbh);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("删除失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        conn.Close();
                        bmmc.Text = "";
                        listZhengmbm.Remove(basiczhengmbminfo);
                    }
                }
            }
        }

        /// <summary>
        /// 功能：【保存】
        /// </summary>
        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (jbzmmc.Text == "")
            {
                MessageBox.Show("请选择基本证名名称！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Is_Repeat();
                if (!IsRepeat)
                {
                    try
                    {
                        string sql = String.Format("insert into t_info_jbzmbm (jbzmbh,bmbh) values ('{0}', '{1}')",
                                       m_jbzmbh, m_bmbh);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("保存失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    finally
                    {
                        conn.Close();
                        save.IsEnabled = false;
                        IsAdd = false;
                    }
                }
            }
        }

        /// <summary>
        /// 功能:【显示全部】
        /// </summary>
        private void display_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdd)
            {
                listZhengmbm.Clear();
                string sql = String.Format("select t3.jbzmbh, t3.jbzmmc, t2.bmbh, t2.bmmc from (t_info_jbzmbm as t1 inner join t_info_bm as t2 on t1.bmbh = t2.bmbh) inner join t_info_jbzm as t3 on t1.jbzmbh = t3.jbzmbh");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    listZhengmbm.Add(new BasicZhengmbmInfo(dr["jbzmbh"].ToString(), dr["jbzmmc"].ToString(), dr["bmbh"].ToString(), dr["bmmc"].ToString()));
                }
                dr.Close();
                conn.Close();
                lv.SelectedIndex = 0;
            }
        }

        /**************************************** 基本证名-病名 *****************************************/
        /***********************************************************************************************/
        /**************************************** 病名-基本证名 *****************************************/

        /// <summary>
        /// 功能：调用【病名信息管理】窗口,【选定】病名名称
        /// </summary>
        private void btn_bmmc1_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdd1)
            {
                MessageBox.Show("请先保存或取消！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                DiseaseInfoAdmin diseaseinfoadmin = new DiseaseInfoAdmin();
                diseaseinfoadmin.PassValuesEvent += new DiseaseInfoAdmin.PassValuesHandler(ReceiveValues2);
                diseaseinfoadmin.Show();
            }
        }

        /// <summary>
        /// 功能：实现病名名称的读取和显示
        /// </summary>
        private void ReceiveValues2(object sender, DiseaseInfoAdmin.PassValuesEventArgs e)
        {
            listZhengmbm1.Clear(); // 先清空集合
            bmmc1.Text = e.Name;
            m_bmbh1 = e.Number;
            // 读取主证编号
            string sql = String.Format("select t1.jbzmbh, jbzmmc from t_info_jbzmbm as t1 inner join t_info_jbzm as t2 on t1.jbzmbh = t2.jbzmbh  where bmbh = '{0}'", m_bmbh1);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listZhengmbm1.Add(new BasicZhengmbmInfo(dr["jbzmbh"].ToString(), dr["jbzmmc"].ToString(), m_bmbh1, bmmc1.Text));
            }
            dr.Close();
            conn.Close();
            lv1.SelectedIndex = 0;
        }

        /// <summary>
        /// 功能：调用【基本证名信息管理】窗口,【选定】病名名称
        /// </summary>
        private void btn_jbzmmc1_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdd1)
            {
                MessageBox.Show("表未处于添加状态！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                BasicZhengm basiczhengm = new BasicZhengm();
                basiczhengm.PassValuesEvent += new BasicZhengm.PassValuesHandler(ReceiveValues3);
                basiczhengm.Show();
            }
        }

        /// <summary>
        /// 功能：实现基本证名名称的读取和显示
        /// </summary>
        private void ReceiveValues3(object sender, BasicZhengm.PassValuesEventArgs e)
        {
            if (lv1.SelectedIndex == lv1.Items.Count - 1)
            {
                jbzmmc1.Text = e.Name;
                m_jbzmbh1 = e.Number;
                BasicZhengmbmInfo basiczhengmbminfo = lv1.SelectedItem as BasicZhengmbmInfo;
                if (basiczhengmbminfo != null && basiczhengmbminfo is BasicZhengmbmInfo)
                {
                    basiczhengmbminfo.JibzmNumber = m_jbzmbh1;
                    basiczhengmbminfo.JibzmName = jbzmmc1.Text;
                }
            }
        }

        /// <summary>
        /// 功能：【增加】
        /// </summary>
        private void add1_Click(object sender, RoutedEventArgs e)
        {
            if (bmmc1.Text != "")
            {
                //listZhengmbm.Clear();
                IsAdd1 = true;
                save1.IsEnabled = true;
                listZhengmbm1.Add(new BasicZhengmbmInfo("", "", m_bmbh1, bmmc1.Text));
                lv1.SelectedIndex = lv1.Items.Count - 1;
            }
            else
            {
                MessageBox.Show("请输入病名名称！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 功能：【取消】
        /// 说明：1.从数据库将数据读取一遍即可！
        /// </summary>
        private void cancel1_Click(object sender, RoutedEventArgs e)
        {
            // 在增加操作下才可执行
            if (IsAdd1)
            {
                listZhengmbm1.Clear();
                // 读取主证编号
                string sql = String.Format("select t1.jbzmbh, jbzmmc from t_info_jbzmbm as t1 inner join t_info_jbzm as t2 on t1.jbzmbh = t2.jbzmbh  where bmbh = '{0}'", m_bmbh1);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    listZhengmbm1.Add(new BasicZhengmbmInfo(dr["jbzmbh"].ToString(), dr["jbzmmc"].ToString(), m_bmbh1, bmmc1.Text));
                }
                dr.Close();
                conn.Close();
                lv1.SelectedIndex = 0;
                bmmc1.Text = "";
                save1.IsEnabled = false;
                IsAdd1 = false;
            }
        }

        /// <summary>
        /// 功能：【删除】
        /// </summary>
        private void delete1_Click(object sender, RoutedEventArgs e)
        {
            // 在非【增加】操作下才可执行
            if (!IsAdd1 && lv1.SelectedIndex != -1)
            {
                string jbzmbh = "";
                string bmbh = "";
                BasicZhengmbmInfo basiczhengmbminfo = lv1.SelectedItem as BasicZhengmbmInfo;
                if (basiczhengmbminfo != null && basiczhengmbminfo is BasicZhengmbmInfo)
                {
                    jbzmbh = basiczhengmbminfo.JibzmNumber;
                    bmbh = basiczhengmbminfo.BingmNumber;
                }
                if (MessageBox.Show("您确认要删除该记录吗？", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    try
                    {
                        string sql = String.Format("delete from t_info_jbzmbm where jbzmbh = '{0}' and bmbh = '{1}'", jbzmbh, bmbh);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("删除失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        conn.Close();
                        bmmc1.Text = "";
                        listZhengmbm1.Remove(basiczhengmbminfo);
                    }
                }
            }
        }

        /// <summary>
        /// 功能：【保存】
        /// </summary>
        private void save1_Click(object sender, RoutedEventArgs e)
        {
            if (bmmc1.Text == "")
            {
                MessageBox.Show("请选择病名名称！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Is_Repeat1();
                if (!IsRepeat1)
                {
                    try
                    {
                        string sql = String.Format("insert into t_info_jbzmbm (jbzmbh,bmbh) values ('{0}', '{1}')",
                                       m_jbzmbh1, m_bmbh1);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("保存失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    finally
                    {
                        conn.Close();
                        save1.IsEnabled = false;
                        IsAdd1 = false;
                    }
                }
            }
        }

        /// <summary>
        /// 功能:【显示全部】
        /// </summary>
        private void display1_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdd1)
            {
                listZhengmbm1.Clear();
                string sql = String.Format("select t3.jbzmbh, t3.jbzmmc, t2.bmbh, t2.bmmc from (t_info_jbzmbm as t1 inner join t_info_bm as t2 on t1.bmbh = t2.bmbh) inner join t_info_jbzm as t3 on t1.jbzmbh = t3.jbzmbh");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    listZhengmbm1.Add(new BasicZhengmbmInfo(dr["jbzmbh"].ToString(), dr["jbzmmc"].ToString(), dr["bmbh"].ToString(), dr["bmmc"].ToString()));
                }
                dr.Close();
                conn.Close();
                lv1.SelectedIndex = 0;
            }
        }
    }
}
  