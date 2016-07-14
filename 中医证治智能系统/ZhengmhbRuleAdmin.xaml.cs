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
    /// Interaction logic for ZhengmhbRuleAdmin.xaml
    /// </summary>
    public partial class ZhengmhbRuleAdmin : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 创建集合实例
        ObservableCollection<ZhengmhbInfo> listZhengmhb= new ObservableCollection<ZhengmhbInfo>();
        // 全局变量，用于暂时存储主证编号
        public string m_zzbh = "";
        // 全局变量，用于暂时存储辅证编号
        public string m_fzbh = "";
        // 用于判断是否进行了增加操作
        bool IsAdd = false;
        // 用于判断点击的是否为【主证名称】按钮
        bool IsZzmc = false;
        // 用于判断点击的是否为【辅证名称】按钮
        bool IsFzmc = false;
        // 用于判断是否重复添加
        bool IsRepeat = false;

        /// <summary>
        /// 功能：类的构造函数，用于初始化
        /// </summary>
        public ZhengmhbRuleAdmin()
        {
            InitializeComponent();
            // 指定 listview 数据源
            lv.ItemsSource = listZhengmhb;
            comb_fzzt.SelectedIndex = 0;
            save.IsEnabled = false;
        }

        /// <summary>
        /// 功能：创建基本证名信息类
        /// 说明：1.ZhuzNumber --> 主证编号  ZhuzName --> 主证名称  FuzNumber --> 辅证编号
        ///         FuzName --> 辅证名称  FuzState --> 辅证状态
        /// </summary>
        public class ZhengmhbInfo : INotifyPropertyChanged
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

            private string _ZhuzNumber;
            private string _ZhuzName;
            private string _FuzNumber;
            private string _FuzName;
            private string _FuzState;

            public string ZhuzNumber
            {
                get { return _ZhuzNumber; }
                set { _ZhuzNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("ZhuzNumber")); }
            }

            public string ZhuzName
            {
                get { return _ZhuzName; }
                set { _ZhuzName = value; OnPropertyChanged(new PropertyChangedEventArgs("ZhuzName")); }
            }

            public string FuzNumber
            {
                get { return _FuzNumber; }
                set { _FuzNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("FuzNumber")); }
            }

            public string FuzName
            {
                get { return _FuzName; }
                set { _FuzName = value; OnPropertyChanged(new PropertyChangedEventArgs("FuzName")); }
            }

            public string FuzState
            {
                get { return _FuzState; }
                set { _FuzState = value; OnPropertyChanged(new PropertyChangedEventArgs("FuzState")); }
            }

            public ZhengmhbInfo(string zhuznumber, string zhuzname, string fuznumber, string fuzname, string fuzstate)
            {
                _ZhuzNumber = zhuznumber;
                _ZhuzName = zhuzname;
                _FuzNumber = fuznumber;
                _FuzName = fuzname;
                _FuzState = fuzstate;
            }
        }

        // 判断是否重复添加
        public void Is_Repeat()
        {
            string sql = String.Format("select count(*) from t_rule_zmhb where zzbh = '{0}' and fzbh='{1}' ", m_zzbh, m_fzbh);
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
        private void btn_zzmc_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdd)
            {
                MessageBox.Show("请先保存或取消！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                IsZzmc = true;
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
            if (IsZzmc)
            {
                listZhengmhb.Clear(); // 先清空集合
                zzmc.Text = e.Name;
                m_zzbh = e.Number;
                // 读取主证编号
                string sql = String.Format("select fzbh, t2.jbzmmc, fzzt from t_rule_zmhb as t1 inner join t_info_jbzm as t2 on t1.fzbh = t2.jbzmbh  where zzbh = '{0}'", m_zzbh);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    string fzstate = "";
                    if (dr["fzzt"].ToString() == "1")
                    {
                        fzstate = "删除";
                    }
                    if (dr["fzzt"].ToString() == "2")
                    {
                        fzstate = "留用";
                    }
                    listZhengmhb.Add(new ZhengmhbInfo(m_zzbh, zzmc.Text, dr["fzbh"].ToString(), dr["jbzmmc"].ToString(), fzstate));
                }
                dr.Close();
                conn.Close();
                lv.SelectedIndex = 0;
                IsZzmc = false;
            }
            if (IsFzmc)
            {
                if (lv.SelectedIndex == lv.Items.Count - 1)
                {
                    fzmc.Text = e.Name;
                    m_fzbh = e.Number;
                    ZhengmhbInfo zhengmhbinfo = lv.SelectedItem as ZhengmhbInfo;
                    if (zhengmhbinfo != null && zhengmhbinfo is ZhengmhbInfo)
                    {
                        zhengmhbinfo.FuzNumber = m_fzbh;
                        zhengmhbinfo.FuzName = fzmc.Text;
                        zhengmhbinfo.FuzState = comb_fzzt.Text;
                    }
                    IsFzmc = false;
                }
            }            
        }

        /// <summary>
        /// 功能：证名合并规则管理【返回】操作
        /// </summary>
        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：ListView 选择变化触发事件
        /// </summary>
        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// 功能：选择辅证名称
        /// </summary>
        private void btn_fzmc_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdd)
            {
                MessageBox.Show("表未处于添加状态！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                IsFzmc = true;
                BasicZhengm basiczhengm = new BasicZhengm();
                basiczhengm.PassValuesEvent += new BasicZhengm.PassValuesHandler(ReceiveValues);
                basiczhengm.Show();

            }
        }

        /// <summary>
        /// 功能：【增加】
        /// </summary>
        private void add_Click(object sender, RoutedEventArgs e)
        {
            if (zzmc.Text != "")
            {
                //listZhengmhb.Clear();
                IsAdd = true;
                save.IsEnabled = true;
                listZhengmhb.Add(new ZhengmhbInfo(m_zzbh, zzmc.Text, "", "", ""));
                lv.SelectedIndex = lv.Items.Count - 1;
            }
            else
            {
                MessageBox.Show("请输入主证名称！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 功能：辅证状态下拉框关闭触发事件
        /// </summary>
        private void comb_fzzt_DropDownClosed(object sender, EventArgs e)
        {
            if (IsAdd && lv.SelectedIndex == lv.Items.Count - 1)
            {
                ZhengmhbInfo zhengmhbinfo = lv.SelectedItem as ZhengmhbInfo;
                if (zhengmhbinfo != null && zhengmhbinfo is ZhengmhbInfo)
                {
                    zhengmhbinfo.FuzState = comb_fzzt.Text;
                }
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
                string zzbh = "";
                string fzbh = "";
                ZhengmhbInfo zhengmhbinfo = lv.SelectedItem as ZhengmhbInfo;
                if (zhengmhbinfo != null && zhengmhbinfo is ZhengmhbInfo)
                {
                    zzbh = zhengmhbinfo.ZhuzNumber;
                    fzbh = zhengmhbinfo.FuzNumber;
                }
                if (MessageBox.Show("您确认要删除本条规则吗？", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    try
                    {
                        string sql = String.Format("delete from t_rule_zmhb where zzbh = '{0}' and fzbh = '{1}'", zzbh, fzbh);
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
                        zzmc.Text = "";
                        listZhengmhb.Remove(zhengmhbinfo);
                    }
                }               
            }
        }

        /// <summary>
        /// 功能：【保存】
        /// </summary>
        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (fzmc.Text == "")
            {
                MessageBox.Show("请选择辅证名称！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Is_Repeat();
                if (!IsRepeat)
                {
                    try
                    {
                        string sql = String.Format("insert into t_rule_zmhb (zzbh,fzbh,fzzt) values ('{0}', '{1}', '{2}')",
                                       m_zzbh, m_fzbh, comb_fzzt.SelectedIndex.ToString());
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
        /// 功能：【取消】
        /// 说明：1.从数据库将数据读取一遍即可！
        /// </summary>
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            // 在增加操作下才可执行
            if(IsAdd)
            {
                listZhengmhb.Clear();
                // 读取主证编号
                string sql = String.Format("select fzbh, t2.jbzmmc, fzzt from t_rule_zmhb as t1 inner join t_info_jbzm as t2 on t1.fzbh = t2.jbzmbh  where zzbh = '{0}'", m_zzbh);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    string fzstate = "";
                    if (dr["fzzt"].ToString() == "1")
                    {
                        fzstate = "删除";
                    }
                    if (dr["fzzt"].ToString() == "2")
                    {
                        fzstate = "留用";
                    }
                    listZhengmhb.Add(new ZhengmhbInfo(m_zzbh, zzmc.Text, dr["fzbh"].ToString(), dr["jbzmmc"].ToString(), fzstate));
                }
                dr.Close();
                conn.Close();
                lv.SelectedIndex = 0;
                save.IsEnabled = false;
                IsAdd = false;
            }
        }

        /// <summary>
        /// 功能:【显示全部】
        /// </summary>
        private void display_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdd)
            {
                listZhengmhb.Clear();
                string sql = String.Format(" select zzbh, t2.jbzmmc, fzbh,  t3.jbzmmc, fzzt from (t_rule_zmhb as t1 inner join t_info_jbzm as t2 on t1.zzbh = t2.jbzmbh) inner join t_info_jbzm as t3 on t1.fzbh = t3.jbzmbh ");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    string fzstate = "";
                    if (dr["fzzt"].ToString() == "1")
                    {
                        fzstate = "删除";
                    }
                    if (dr["fzzt"].ToString() == "2")
                    {
                        fzstate = "留用";
                    }
                    listZhengmhb.Add(new ZhengmhbInfo(dr["zzbh"].ToString(), dr[1].ToString(), dr["fzbh"].ToString(), dr[3].ToString(), fzstate));
                }
                dr.Close();
                conn.Close();
                lv.SelectedIndex = 0;
            }
        }


    }
}
