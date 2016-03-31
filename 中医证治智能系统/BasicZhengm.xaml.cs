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
    /// Interaction logic for BasicZhengm.xaml
    /// </summary>
    public partial class BasicZhengm : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 创建集合实例
        ObservableCollection<BasicZhengmInfo> listBasicZhengm = new ObservableCollection<BasicZhengmInfo>();
        // 全局变量，用于存储病名类信息
        BasicZhengmInfo BasicZhengm_Edit = new BasicZhengmInfo("", "", "", "", "", "", "", "");
        // 用于判断是否进行了增加操作
        bool IsAdd = false;
        // 用于判断是否进行了修改操作
        bool IsModify = false;
        // 用于判断基本证名名称重复
        bool IsRepeat = false;
        // 创建对 PassValuesHandler 方法的引用的类
        public delegate void PassValuesHandler(object sender, PassValuesEventArgs e);
        // 声明事件
        public event PassValuesHandler PassValuesEvent;
        // 创建事件数据类
        public class PassValuesEventArgs : EventArgs
        {
            private string _number;
            private string _name;
            public string Number
            {
                get { return _number; }
                set { _number = value; }
            }

            public PassValuesEventArgs(string number, string name)
            {
                this.Number = number;
                this.Name = name;
            }

            public string Name 
            {
                get { return _name; }
                set { _name = value; } 
            }
        }

        /// <summary>
        /// 功能：类的构造函数，用于初始化
        /// </summary>
        public BasicZhengm()
        {
            InitializeComponent();
            // 病名类型默认选中第一项【内伤】
            search_jbzmlx.SelectedIndex = 1;
            // 证名分级默认选中第一项【全部】
            search_zmjb.SelectedIndex = 0;
            input_zmjb.SelectedIndex = 0;
            // 证名分类默认选中第一项【全部】
            search_xsjb.SelectedIndex = 0;
            input_xsjb.SelectedIndex = 0;
            // 病机因素默认选中第一项【全部】
            search_bjys.SelectedIndex = 0;
            input_bjys.SelectedIndex = 0;
            // 指定 listview 数据源
            search_lv.ItemsSource = listBasicZhengm;
            // 初始化控件的可编辑性
            input_jbzmbh.IsReadOnly = true;
            input_save.IsEnabled = false;
            input_cancel.IsEnabled = false;
        }

        /// <summary>
        /// 功能：创建基本证名信息类
        /// 说明：1.BasicZhengmNumber --> 基本证名编号  BasicZhengmName --> 基本证名名称  BasicZhengmType --> 基本证名类型
        ///         Treatment --> 治法  Remark --> 备注
        /// </summary>
        public class BasicZhengmInfo : INotifyPropertyChanged
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

            private string _BasicZhengmNumber;
            private string _BasicZhengmName;
            private string _BasicZhengmType;
            private string _Zmjb;
            private string _Xsjb;
            private string _Bjys;
            private string _Treatment;
            private string _Remark;

            public string BasicZhengmNumber
            {
                get { return _BasicZhengmNumber; }
                set { _BasicZhengmNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("BasicZhengmNumber")); }
            }

            public string BasicZhengmName
            {
                get { return _BasicZhengmName; }
                set { _BasicZhengmName = value; OnPropertyChanged(new PropertyChangedEventArgs("BasicZhengmName")); }
            }

            public string BasicZhengmType
            {
                get { return _BasicZhengmType; }
                set { _BasicZhengmType = value; OnPropertyChanged(new PropertyChangedEventArgs("BasicZhengmType")); }
            }

            public string Zmjb
            {
                get { return _Zmjb; }
                set { _Zmjb = value; OnPropertyChanged(new PropertyChangedEventArgs("Zmjb")); }
            }

            public string Xsjb
            {
                get { return _Xsjb; }
                set { _Xsjb = value; OnPropertyChanged(new PropertyChangedEventArgs("Xsjb")); }
            }

            public string Bjys
            {
                get { return _Bjys; }
                set { _Bjys = value; OnPropertyChanged(new PropertyChangedEventArgs("Bjys")); }
            }

            public string Treatment
            {
                get { return _Treatment; }
                set { _Treatment = value; OnPropertyChanged(new PropertyChangedEventArgs("Treatment")); }
            }

            public string Remark
            {
                get { return _Remark; }
                set { _Remark = value; OnPropertyChanged(new PropertyChangedEventArgs("Remark")); }
            }

            public BasicZhengmInfo(string basiczhengmnumber, string basiczhengmname, string basiczhengmtype, string zmjb, string xsjb, string bjys, string treatment, string remark)
            {
                _BasicZhengmNumber = basiczhengmnumber;
                _BasicZhengmName = basiczhengmname;
                _BasicZhengmType = basiczhengmtype;
                _Zmjb = zmjb;
                _Xsjb = xsjb;
                _Bjys = bjys;
                _Treatment = treatment;
                _Remark = remark;
            }
        }

        /// <summary>
        /// 功能：根据对应数字返回相应的证名分级名称
        /// </summary>
        public string Get_Zmjb(string index)
        {
            string Zmjb = "";
            switch (index)
            { 
                case "1":
                    Zmjb = "一级特别证";
                    break;
                case "2":
                    Zmjb = "二级病名证";
                    break;
                case "3":
                    Zmjb = "三级脏腑证";
                    break;
                case "4":
                    Zmjb = "四级基础证";
                    break;
                case "5":
                    Zmjb = "五级近似证";
                    break;
            }
            return Zmjb;
        }

        /// <summary>
        /// 功能：根据对应数字返回相应的证名分类名称
        /// </summary>
        public string Get_Xsjb(string index)
        {
            string Xsjb = "";
            switch (index)
            {
                case "1":
                    Xsjb = "甲";
                    break;
                case "2":
                    Xsjb = "乙1";
                    break;
                case "3":
                    Xsjb = "乙2";
                    break;
                case "4":
                    Xsjb = "乙3";
                    break;
                case "5":
                    Xsjb = "乙4";
                    break;
            }
            return Xsjb;
        }

        /// <summary>
        /// 功能：防止输入基本证名名称重复
        /// </summary>
        public void Is_Repeat()
        {
            string basiczhengm_name = input_jbzmmc.Text.Trim();
            string sql = String.Format("select count(*) from t_info_jbzm where jbzmmc = '{0}' and jbzmbh != '{1}' ", basiczhengm_name, BasicZhengm_Edit.BasicZhengmNumber);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count > 0)
            {
                MessageBox.Show("基本证名名称不能重复！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                IsRepeat = true;
            }
            else
                IsRepeat = false;
            conn.Close();
        }

        /// <summary>
        /// 功能：基本证名信息检索中的查询功能
        /// </summary>
        private void search_Click(object sender, RoutedEventArgs e)
        {
            listBasicZhengm.Clear(); // 先清空集合
            string sql = "";
            string sql_count = "";
            if (search_jbzmlx.Text.Trim() == "外感")
            {
                sql = String.Format("select * from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%'", 0, search_jbzmmc.Text.Trim());
                sql_count = String.Format("select count(*) from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' ", 0, search_jbzmmc.Text.Trim());
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    listBasicZhengm.Add(new BasicZhengmInfo(dr["jbzmbh"].ToString(), dr["jbzmmc"].ToString(), "0", "", "", "", dr["zf"].ToString(), dr["bz"].ToString()));
                }
                dr.Close();
                conn.Close();

                // 显示记录数
                conn.Open();
                comm = new SqlCommand(sql_count, conn);
                int count = (int)comm.ExecuteScalar();
                conn.Close();
                Record_Name.Text = Convert.ToString(count) + "条";
            }
            if (search_jbzmlx.Text.Trim() == "内伤")
            {
                // 证名分级、证名分类、病机因素都为全部
                if (search_zmjb.SelectedIndex == 0 && search_xsjb.SelectedIndex == 0 && search_bjys.SelectedIndex == 0) 
                {
                    sql = String.Format("select * from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%'", 1, search_jbzmmc.Text.Trim());
                    sql_count = String.Format("select count(*) from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' ", 1, search_jbzmmc.Text.Trim());
                }
                // 证名分级不为全部，证名分类、病机因素为全部
                if (search_zmjb.SelectedIndex != 0 && search_xsjb.SelectedIndex == 0 && search_bjys.SelectedIndex == 0)
                {
                    sql = String.Format("select * from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and zmjb = '{2}'", 1, search_jbzmmc.Text.Trim(), search_zmjb.SelectedIndex.ToString());
                    sql_count = String.Format("select count(*) from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and zmjb = '{2}' ", 1, search_jbzmmc.Text.Trim(), search_zmjb.SelectedIndex.ToString());
                }
                // 证名分类不为全部，证名分级、病机因素都为全部
                if (search_zmjb.SelectedIndex == 0 && search_xsjb.SelectedIndex != 0 && search_bjys.SelectedIndex == 0)
                {
                    sql = String.Format("select * from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and xsjb = '{2}'", 1, search_jbzmmc.Text.Trim(), search_xsjb.SelectedIndex.ToString());
                    sql_count = String.Format("select count(*) from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and xsjb = '{2}' ", 1, search_jbzmmc.Text.Trim(), search_xsjb.SelectedIndex.ToString());
                }
                // 证名分级、证名分类为全部，病机因素不为全部
                if (search_zmjb.SelectedIndex == 0 && search_xsjb.SelectedIndex == 0 && search_bjys.SelectedIndex != 0)
                {
                    sql = String.Format("select * from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and bjys = '{2}'", 1, search_jbzmmc.Text.Trim(), search_bjys.SelectedIndex.ToString());
                    sql_count = String.Format("select count(*) from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and bjys = '{2}' ", 1, search_jbzmmc.Text.Trim(), search_bjys.SelectedIndex.ToString());
                }
                // 证名分级、证名分类不为全部，病机因素为全部
                if (search_zmjb.SelectedIndex != 0 && search_xsjb.SelectedIndex != 0 && search_bjys.SelectedIndex == 0)
                {
                    sql = String.Format("select * from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and zmjb = '{2}' and xsjb = '{3}'", 1, search_jbzmmc.Text.Trim(), search_zmjb.SelectedIndex.ToString(), search_xsjb.SelectedIndex.ToString());
                    sql_count = String.Format("select count(*) from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and zmjb = '{2}' and xsjb = '{3}' ", 1, search_jbzmmc.Text.Trim(), search_zmjb.SelectedIndex.ToString(), search_xsjb.SelectedIndex.ToString());    
                }
                // 证名分级、病机因素不为全部，证名分类为全部
                if (search_zmjb.SelectedIndex != 0 && search_xsjb.SelectedIndex == 0 && search_bjys.SelectedIndex != 0)
                {
                    sql = String.Format("select * from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and zmjb = '{2}' and bjys = '{3}'", 1, search_jbzmmc.Text.Trim(), search_zmjb.SelectedIndex.ToString(), search_bjys.SelectedIndex.ToString());
                    sql_count = String.Format("select count(*) from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and zmjb = '{2}' and bjys = '{3}' ", 1, search_jbzmmc.Text.Trim(), search_zmjb.SelectedIndex.ToString(), search_bjys.SelectedIndex.ToString());    
                }
                // 证名分级为全部，证名分类、病机因素不为全部
                if (search_zmjb.SelectedIndex == 0 && search_xsjb.SelectedIndex != 0 && search_bjys.SelectedIndex != 0)
                {
                    sql = String.Format("select * from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and xsjb = '{2}' and bjys = '{3}'", 1, search_jbzmmc.Text.Trim(), search_xsjb.SelectedIndex.ToString(), search_bjys.SelectedIndex.ToString());
                    sql_count = String.Format("select count(*) from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and xsjb = '{2}' and bjys = '{3}' ", 1, search_jbzmmc.Text.Trim(), search_xsjb.SelectedIndex.ToString(), search_bjys.SelectedIndex.ToString());    
                }
                // 证名分级、证名分类、病机因素都不为全部
                if (search_zmjb.SelectedIndex != 0 && search_xsjb.SelectedIndex != 0 && search_bjys.SelectedIndex != 0)
                {
                    sql = String.Format("select * from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and zmjb = '{2}' and xsjb = '{3}' and bjys = '{4}'", 1, search_jbzmmc.Text.Trim(), search_zmjb.SelectedIndex.ToString(), search_xsjb.SelectedIndex.ToString(), search_bjys.SelectedIndex.ToString());
                    sql_count = String.Format("select count(*) from t_info_jbzm where jbzmlx = '{0}' and jbzmmc like '%{1}%' and zmjb = '{2}' and xsjb = '{3}' and bjys = '{4}'", 1, search_jbzmmc.Text.Trim(), search_zmjb.SelectedIndex.ToString(), search_xsjb.SelectedIndex.ToString(), search_bjys.SelectedIndex.ToString());    
                }
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    listBasicZhengm.Add(new BasicZhengmInfo(dr["jbzmbh"].ToString(), dr["jbzmmc"].ToString(), "1", Get_Zmjb(dr["zmjb"].ToString()), Get_Xsjb(dr["xsjb"].ToString()), dr["bjys"].ToString(), dr["zf"].ToString(), dr["bz"].ToString()));
                }
                dr.Close();
                conn.Close();

                // 显示记录数               
                conn.Open();
                comm = new SqlCommand(sql_count, conn);
                int count = (int)comm.ExecuteScalar();
                conn.Close();
                Record_Name.Text = Convert.ToString(count) + "条";
            }
        }

        /// <summary>
        /// 功能：病名信息检索中【返回】操作
        /// </summary>
        private void search_back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：选择某项时基本证名信息录入界面显示相应的信息
        /// </summary>
        private void search_lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BasicZhengmInfo basiczhengminfo = search_lv.SelectedItem as BasicZhengmInfo;
            if (basiczhengminfo != null && basiczhengminfo is BasicZhengmInfo)
            {
                string sql = String.Format("select * from t_info_jbzm where jbzmbh = '{0}'", basiczhengminfo.BasicZhengmNumber);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["jbzmlx"].ToString() == "0")
                        input_jbzmlx.Text = "外感基本证名";
                    else
                        input_jbzmlx.Text = "内伤基本证名";
                    input_jbzmbh.Text = dr["jbzmbh"].ToString();
                    input_jbzmmc.Text = dr["jbzmmc"].ToString();
                    input_zf.Text = dr["zf"].ToString();
                    input_bz.Text = dr["bz"].ToString();
                }
                dr.Close();
                conn.Close();
            }
        }

        /// <summary>
        /// 功能：基本证名信息检索中【修改】按钮功能
        /// </summary>
        private void search_modify_Click(object sender, RoutedEventArgs e)
        {
            // 防止在增加功能中进行修改
            if (!IsAdd)
            {
                if (input_jbzmbh.Text.Trim() == "")
                {
                    MessageBox.Show("请选择具体项进行修改！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    tabcontrol.SelectedIndex = 1;
                    IsModify = true;
                    // 【修改】操作中，【保存】和【取消】应设置为可用
                    input_save.IsEnabled = true;
                    input_cancel.IsEnabled = true;
                    // 用于保存和重复检测
                    BasicZhengm_Edit.BasicZhengmNumber = input_jbzmbh.Text;
                }             
            }
        }

        /// <summary>
        /// 功能：基本证名信息检索中删除操作
        /// </summary>
        private void search_delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("执行删除操作将对数据库造成很大影响，强烈建议不要执行此操作，您确认要删除该病名吗？", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (MessageBox.Show("再次警告：您确认要删除该病名吗？", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    BasicZhengmInfo basiczhengminfo = search_lv.SelectedItem as BasicZhengmInfo;
                    if (basiczhengminfo != null && basiczhengminfo is BasicZhengmInfo)
                    {
                        listBasicZhengm.Remove(basiczhengminfo);
                    }
                    try
                    {
                        string sql = String.Format("delete from t_info_jbzm where jbzmbh = '{0}'", basiczhengminfo.BasicZhengmNumber);
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

        /////////////////////////////////// 基本证名信息检索 //////////////////////////////////////////
        //////////////////////////////////////  分割线  //////////////////////////////////////////////
        /////////////////////////////////// 基本证名信息录入 //////////////////////////////////////////


        /// <summary>
        /// 功能：返回主菜单
        /// </summary>
        private void input_back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：基本证名信息录入【增加】按钮
        /// 说明：1.在进行修改操作时，点击【增加】按钮是无效的
        ///       2.点击【增加】按钮时，应先将所有编辑框清空
        ///       3.应考虑增加不同的基本证名类型其最大编号是不同的，分情况搜索
        /// </summary>
        private void input_add_Click(object sender, RoutedEventArgs e)
        {
            // 防止在修改功能中进行增加
            if (!IsModify)
            {
                IsAdd = true;
                // 【增加】操作中，【保存】和【取消】应设置为可用
                input_save.IsEnabled = true;
                input_cancel.IsEnabled = true;
                //  清空编辑框
                input_jbzmbh.Text = "";
                input_jbzmmc.Text = "";
                input_zf.Text = "";
                input_bz.Text = "";

                if (input_jbzmlx.Text.Trim() == "")
                {
                    MessageBox.Show("请选择基本证名类型！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (input_jbzmlx.Text.Trim() == "外感基本证名")
                    {
                        string sql = String.Format("select max(jbzmbh) from t_info_jbzm where jbzmlx = 0");
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        SqlDataReader dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            BasicZhengm_Edit = new BasicZhengmInfo(String.Format("{0:00000000}", Convert.ToInt64(dr[0]) + 1), "", "", "", "","", "", "");
                            listBasicZhengm.Add(BasicZhengm_Edit);
                            input_jbzmbh.Text = BasicZhengm_Edit.BasicZhengmNumber;
                        }
                        dr.Close();
                        conn.Close();
                    }
                    if (input_jbzmlx.Text.Trim() == "内伤基本证名")
                    {
                        string sql = String.Format("select max(jbzmbh) from t_info_jbzm where jbzmlx = 1");
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        SqlDataReader dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            BasicZhengm_Edit = new BasicZhengmInfo(String.Format("{0:00000000}", Convert.ToInt64(dr[0]) + 1), "", "", "", "", "", "" ,"");
                            listBasicZhengm.Add(BasicZhengm_Edit);
                            input_jbzmbh.Text = BasicZhengm_Edit.BasicZhengmNumber;
                        }
                        dr.Close();
                        conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 功能：病名信息录入中【保存】功能
        /// </summary>
        private void input_save_Click(object sender, RoutedEventArgs e)
        {
            // 检测基本证名名称是否输入重复
            Is_Repeat();
            // 【增加】功能下【保存】功能
            if (IsAdd == true && IsRepeat == false)
            {
                if (input_jbzmlx.Text.Trim() == "")
                {
                    MessageBox.Show("请选择基本证名类型！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (input_jbzmlx.Text.Trim() == "外感基本证名")
                    {
                        if (input_jbzmmc.Text.Trim() == "")
                        {
                            MessageBox.Show("基本证名名称不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if(input_zf.Text.Trim() == "")
                            {
                                MessageBox.Show("治法不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                try
                                {                             
                                    string sql = String.Format("INSERT INTO t_info_jbzm ( jbzmbh, jbzmmc, jbzmlx, zf, bz, xsjb, bjys, zmjb) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')"
                                                                , BasicZhengm_Edit.BasicZhengmNumber, input_jbzmmc.Text, 0, input_zf.Text, input_bz.Text);
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
                                }
                                finally
                                {
                                    conn.Close();
                                    input_save.IsEnabled = false;
                                    input_cancel.IsEnabled = false;
                                    IsAdd = false;
                                }
                            }
                        }
                    }
                    if (input_jbzmlx.Text.Trim() == "内伤基本证名")
                    {
                        if (input_jbzmmc.Text.Trim() == "")
                        {
                            MessageBox.Show("基本证名名称不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if (input_zf.Text.Trim() == "")
                            {
                                MessageBox.Show("治法不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                try
                                {
                                    string sql = String.Format("INSERT INTO t_info_jbzm ( jbzmbh, jbzmmc, jbzmlx, zf, bz, xsjb, bjys, zmjb) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')"
                                                                , BasicZhengm_Edit.BasicZhengmNumber, input_jbzmmc.Text, 1, input_zf.Text, input_bz.Text, input_xsjb.SelectedIndex, input_bjys.SelectedIndex + 1, input_zmjb.SelectedIndex + 1);
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
                                }
                                finally
                                {
                                    conn.Close();
                                    input_save.IsEnabled = false;
                                    input_cancel.IsEnabled = false;
                                    IsAdd = false;
                                }
                            }
                        }
                    }
                }
            }
            // 【修改】功能下【保存】功能
            if (IsModify == true && IsRepeat == false)
            {
                if (input_jbzmlx.Text.Trim() == "")
                {
                    MessageBox.Show("请选择基本证名类型！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (input_jbzmlx.Text.Trim() == "外感基本证名")
                    {
                        if (input_jbzmmc.Text.Trim() == "")
                        {
                            MessageBox.Show("基本证名名称不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if (input_zf.Text.Trim() == "")
                            {
                                MessageBox.Show("治法不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                try
                                {
                                    string sql = String.Format("update t_info_jbzm set jbzmmc = '{0}', jbzmlx = '{1}', zf = '{2}', bz = '{3}' where jbzmbh = '{4}'"
                                                                , input_jbzmmc.Text, 0, input_zf.Text, input_bz.Text, BasicZhengm_Edit.BasicZhengmNumber);
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
                                }
                                finally
                                {
                                    conn.Close();
                                    input_save.IsEnabled = false;
                                    input_cancel.IsEnabled = false;
                                    IsModify = false;
                                }
                            }
                        }
                    }
                    if (input_jbzmlx.Text.Trim() == "内伤基本证名")
                    {
                        if (input_jbzmmc.Text.Trim() == "")
                        {
                            MessageBox.Show("基本证名名称不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if (input_zf.Text.Trim() == "")
                            {
                                MessageBox.Show("治法不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                try
                                {
                                    string sql = String.Format("update t_info_jbzm set jbzmmc = '{0}', jbzmlx = '{1}', zf = '{2}', bz = '{3}', zmjb = '{4}' ,xsjb = '{5}', bjys = '{6}'  where jbzmbh = '{7}'"
                                                                 , input_jbzmmc.Text, 1, input_zf.Text, input_bz.Text, input_zmjb.SelectedIndex + 1, input_xsjb.SelectedIndex, input_bjys.SelectedIndex + 1, BasicZhengm_Edit.BasicZhengmNumber);
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
                                }
                                finally
                                {
                                    conn.Close();
                                    input_save.IsEnabled = false;
                                    input_cancel.IsEnabled = false;
                                    IsModify = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 功能：基本证名信息录入中【修改】按钮功能
        /// </summary>
        private void input_modify_Click(object sender, RoutedEventArgs e)
        {
            // 防止在增加功能中进行修改
            if (!IsAdd)
            {
                if (input_jbzmbh.Text.Trim() == "")
                {
                    MessageBox.Show("请选择具体项进行修改！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    IsModify = true;
                    // 【修改】操作中，【保存】和【取消】应设置为可用
                    input_save.IsEnabled = true;
                    input_cancel.IsEnabled = true;
                    // 用于保存和重复检测
                    BasicZhengm_Edit.BasicZhengmNumber = input_jbzmbh.Text;
                }             
            }
        }

        /// <summary>
        /// 功能：基本证名信息录入中【取消】按钮功能
        /// </summary>
        private void input_cancel_Click(object sender, RoutedEventArgs e)
        {
            // 【增加】功能下【取消】
            if (IsAdd)
            {
                IsAdd = false;
                input_save.IsEnabled = false;
                input_cancel.IsEnabled = false;
                input_jbzmbh.Text = "";
                input_jbzmmc.Text = "";
                input_zf.Text = "";
                input_bz.Text = "";
            }
            // 【修改】功能下【取消】
            if (IsModify)
            {
                IsModify = false;
                input_save.IsEnabled = false;
                input_cancel.IsEnabled = false;
                // 恢复修改前的数据
                BasicZhengmInfo basiczhengminfo = search_lv.SelectedItem as BasicZhengmInfo;
                if (basiczhengminfo != null && basiczhengminfo is BasicZhengmInfo)
                {
                    string sql = String.Format("select * from t_info_jbzm where jbzmbh = '{0}'", basiczhengminfo.BasicZhengmNumber);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr["jbzmlx"].ToString() == "0")
                            input_jbzmlx.Text = "外感基本证名";
                        else
                            input_jbzmlx.Text = "内伤基本证名";
                        input_jbzmbh.Text = dr["jbzmbh"].ToString();
                        input_jbzmmc.Text = dr["jbzmmc"].ToString();
                        input_zf.Text = dr["zf"].ToString();
                        input_bz.Text = dr["bz"].ToString();
                    }
                    dr.Close();
                    conn.Close();                   
                }
            }
        }

        public static string number;

        /// <summary>
        /// 功能：【基本证名信息管理】中的【选定】按钮功能
        /// </summary>
        private void search_select_Click(object sender, RoutedEventArgs e)
        {
            BasicZhengmInfo basiczhengminfo = search_lv.SelectedItem as BasicZhengmInfo;
            if (basiczhengminfo != null && basiczhengminfo is BasicZhengmInfo)
            {
                PassValuesEventArgs args = new PassValuesEventArgs(basiczhengminfo.BasicZhengmNumber.ToString(), basiczhengminfo.BasicZhengmName.ToString());
                // 要判断 PassValuesEvent 是否为空，即判断该窗口是否被调用
                if (PassValuesEvent != null)
                {
                    PassValuesEvent(this, args);
                }
            }
            this.Close();
        }

        /// <summary>
        /// 功能：基本证名类型下拉关闭触发【检索】
        /// </summary>
        private void search_jbzmlx_DropDownClosed(object sender, EventArgs e)
        {
            if (search_jbzmlx.Text == "外感")
            {
                search_zmjb.IsEnabled = false;
                search_xsjb.IsEnabled = false;
                search_bjys.IsEnabled = false;
            }
            else
            {
                search_zmjb.IsEnabled = true;
                search_xsjb.IsEnabled = true;
                search_bjys.IsEnabled = true; 
            }
        }

        /// <summary>
        /// 功能：基本证名类型下拉关闭触发【录入】
        /// </summary>
        private void input_jbzmlx_DropDownClosed(object sender, EventArgs e)
        {
            if (input_jbzmlx.Text == "外感基本证名")
            {
                input_zmjb.IsEnabled = false;
                input_xsjb.IsEnabled = false;
                input_bjys.IsEnabled = false;
            }
            else
            {
                input_zmjb.IsEnabled = true;
                input_xsjb.IsEnabled = true;
                input_bjys.IsEnabled = true;
            }
        }
    }
}
