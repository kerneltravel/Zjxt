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
    /// Interaction logic for BingmRuleAdmin.xaml
    /// </summary>
    public partial class BingmRuleAdmin : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 创建 List 集合实例
        List<Node> nodes = new List<Node>();
        // 创建哈希表实例
        Hashtable httree = new Hashtable(1000);
        // 全局变量，用于暂时存储【病名信息管理】中的病名编号
        public string m_bmbh = "";
        // 全局变量，用于暂时存储条件编号
        public string m_tjbh = "";
        // 全局变量，用于暂时存储病名类型
        public string bmlx = "";
        // 全局变量，判断是否可以添加保存
        bool IsAdd = false;
        // 全局变量，判断是否重复添加
        bool IsRepeat = false;

        public BingmRuleAdmin()
        {
            InitializeComponent();
            // 初始化默认选择项
            bmmc.IsEnabled = false;
            bmmc_1.IsEnabled = false;
            comb_ffs.SelectedIndex = 0;
            comb_tjs.SelectedIndex = 0;
            comb_zbs.SelectedIndex = 0;
            comb_tjlx.SelectedIndex = 0;
            comb_tjfz.SelectedIndex = -1;
            comb_zlfz.SelectedIndex = -1;

        }

        /// <summary>
        /// 功能：创建节点类
        /// </summary>
        public class Node
        {
            public Node()
            {
                this.Nodes = new List<Node>();
                this.ParentID = -1;
            }
            public long ID { get; set; }
            public string Name { get; set; }
            public long ParentID { get; set; }
            public List<Node> Nodes { get; set; }
        }

        // 创建树
        public void BuildENTree()
        {
            httree.Clear();
            Node parentnode1 = new Node();
            List<Node> outputList = new List<Node>();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = new Node();
                newnode.ID = nodes[i].ID;
                newnode.Name = nodes[i].Name;
                newnode.ParentID = nodes[i].ParentID;
                httree.Add(newnode.ID, newnode);
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                Node parentnode = (Node)httree[newnode.ParentID];
                if (parentnode != null)
                {
                    parentnode.Nodes.Add(newnode);
                    if ((parentnode1.ID != parentnode.ID) && parentnode.ParentID == -1)
                    {
                        outputList.Add(parentnode);
                        parentnode1.ID = parentnode.ID;
                    }
                }
            }
            this.treeview.ItemsSource = outputList;
        }

        // 创建子树
        public void BuildENTree1()
        {
            httree.Clear();
            Node parentnode1 = new Node();
            List<Node> outputList = new List<Node>();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = new Node();
                newnode.ID = nodes[i].ID;
                newnode.Name = nodes[i].Name;
                newnode.ParentID = nodes[i].ParentID;
                httree.Add(newnode.ID, newnode);
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                Node parentnode = (Node)httree[newnode.ParentID];
                if (parentnode != null)
                {
                    parentnode.Nodes.Add(newnode);
                    if ((parentnode1.ID != parentnode.ID) && parentnode.ParentID == -1)
                    {
                        outputList.Add(parentnode);
                        parentnode1.ID = parentnode.ID;
                    }
                }
            }
            this.treeview1.ItemsSource = outputList;
        }

        /// <summary>
        /// 功能：判断重复添加
        /// </summary>
        public void Is_Repeat()
        {
            // 外感
            if (bmlx == "0")
            {
                // 症象
                if (comb_tjlx.SelectedIndex == 1)
                {
                    string sql = String.Format("select count(*) from t_rule_wg_bm where ff = '{0}' and blgz = '{1}' and tjzb ='{2}'and zxbh ='{3}' and bmbh = '{4}'"
                          , comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex, m_tjbh, m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = (int)comm.ExecuteScalar();
                    if (count > 0)
                    {
                        IsRepeat = true;
                    }
                    else
                    {
                        IsRepeat = false;
                    }
                    conn.Close();
                }
                // 基本病机
                if (comb_tjlx.SelectedIndex == 2)
                {
                    string sql = String.Format("select count(*) from t_rule_wg_bm where ff = '{0}' and blgz = '{1}' and jbbjbh ='{2}' and tjzb ='{3}'"
                          , comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex, m_tjbh, m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = (int)comm.ExecuteScalar();
                    if (count > 0)
                    {
                        IsRepeat = true;
                    }
                    else
                    {
                        IsRepeat = false;
                    }
                    conn.Close();
                }               
            }
            // 内伤
            if (bmlx == "1")
            {
                // 症象
                if (comb_tjlx.SelectedIndex == 1)
                {
                    string sql = String.Format("select count(*) from t_rule_ns_bm where ff = '{0}' and blgz = '{1}' and tjzb ='{2}'and zxbh ='{3}' and bmbh = '{4}'"
                          , comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex, m_tjbh, m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = (int)comm.ExecuteScalar();
                    if (count > 0)
                    {
                        IsRepeat = true;
                    }
                    else
                    {
                        IsRepeat = false;
                    }
                    conn.Close();
                }                
            }            
        }

        /// <summary>
        /// 功能：定义函数，实现数字转换为汉字
        /// </summary>
        public string numbertochinese(string number)
        {
            string chinese = "";
            switch (number)
            {
                case "1":
                    chinese = "一";
                    break;
                case "2":
                    chinese = "二";
                    break;
                case "3":
                    chinese = "三";
                    break;
                case "4":
                    chinese = "四";
                    break;
                case "5":
                    chinese = "五";
                    break;
                case "6":
                    chinese = "六";
                    break;
                case "7":
                    chinese = "七";
                    break;
                case "8":
                    chinese = "八";
                    break;
                case "9":
                    chinese = "九";
                    break;
                case "10":
                    chinese = "十";
                    break;
                case "11":
                    chinese = "十一";
                    break;
                case "12":
                    chinese = "十二";
                    break;
            }
            return chinese;
        }

        /// <summary>
        /// 功能：返回
        /// </summary>
        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：刷新列表
        /// 说明：1.哈希表的 ID 不能重复
        /// </summary>
        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            comb_ffs.SelectedIndex = 0;
            comb_tjs.SelectedIndex = 0;
            comb_zbs.SelectedIndex = 0;
            comb_tjlx.SelectedIndex = 0;
            comb_tjfz.SelectedIndex = -1;
            comb_zlfz.SelectedIndex = -1;
            comb_tjlx.Items.Clear();
            comb_tjlx.Items.Add("--请选择条件类型--");
            comb_tjlx.SelectedIndex = 0;
            if (bmlx == "0") // 外感
            {
                comb_tjlx.Items.Add("症象");
                comb_tjlx.Items.Add("基本病机");
            }
            if (bmlx == "1") // 内伤
                comb_tjlx.Items.Add("症象");
            comb_ffs.Items.Clear();
            comb_ffs.Items.Add("--请选择方法数--");
            comb_ffs.SelectedIndex = 0;
            comb_tjs.Items.Clear();
            comb_tjs.Items.Add("--请选择条件数--");
            comb_tjs.SelectedIndex = 0;
            comb_zbs.Items.Clear();
            comb_zbs.Items.Add("--请选择组别数--");
            comb_zbs.SelectedIndex = 0;
            tjmc.Clear();
            // 读取病名编号和病名类型
            string sql = String.Format("select bmbh, bmlx from t_info_bm where bmmc = '{0}'", bmmc.Text.Trim());
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                m_bmbh = dr["bmbh"].ToString();
                bmlx = dr["bmlx"].ToString();
            }
            dr.Close();
            conn.Close();

            // 外感
            if (bmlx == "0")
            {
                // 清空
                nodes.Clear();
                // 判断是否存在该病名的推理规则
                sql = String.Format("select count(*) from t_rule_wg_bm where bmbh = '{0}'", m_bmbh);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count = (int)comm.ExecuteScalar();
                if (count == 0) {
                    // 清空treeview，先清空结点，再调用创建树函数
                    nodes.Clear();
                    BuildENTree();
                    MessageBox.Show("不存在该病名的推理规则，请录入！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                conn.Close();
                // 若存在该病名的推理规则，显示 treeview
                if (count > 0)
                {
                    // 将数据库数据写入 List 集合
                    // 一级树写入           
                    sql = String.Format("select distinct ff from t_rule_wg_bm where bmbh = '{0}'", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node 
                        { 
                            ID = Convert.ToInt32(dr["ff"]), 
                            Name = bmmc.Text.Trim() + "的推理规则方法" + numbertochinese(dr["ff"].ToString()) + "（规则：所有条件均成立）" 
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 二级树写入
                    sql = String.Format("select ff, blgz, gzfz from t_rule_wg_bm where bmbh = '{0}' group by ff, blgz, gzfz", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString()),
                            Name = "条件" + numbertochinese(dr["blgz"].ToString()) + "（规则：任" + numbertochinese(dr["gzfz"].ToString()) + "组成立）",
                            ParentID = Convert.ToInt32(dr["ff"])
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 三级树写入
                    sql = String.Format("select ff, blgz, tjzb, znfz from t_rule_wg_bm where bmbh = '{0}' group by ff, blgz, tjzb, znfz ", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString()),
                            Name = "组别" + numbertochinese(dr["tjzb"].ToString()) + "（规则：任" + numbertochinese(dr["znfz"].ToString()) + "症成立）",
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 四级树写入
                    // 写入症象信息
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_wg_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["zxbh"].ToString()),
                            Name = dr["zxbh"].ToString() + "  " + dr[4].ToString() + "【症象】",
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();
                    // 写入基本病机信息
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from [zjxtserver].[dbo].[t_info_jbbj] as t1 inner join [zjxtserver].[dbo].[t_rule_wg_bm] as t2 on t2.jbbjbh = t1.jbbjbh where bmbh = '{0}' group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh ", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["jbbjbh"].ToString()),
                            Name = dr["jbbjbh"].ToString() + "  " + dr[4].ToString() + "【基本病机】",
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 五级树写入
                    // 写入症象信息
                    sql = String.Format("select t2.id, t1.blgz, t1.tjzb, t1.ff, t1.zxbh, t2.zxmc from t_rule_wg_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' group by t2.id, t1.blgz, t1.tjzb, t1.ff, t1.zxbh, t2.zxmc ", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["zxbh"].ToString() + dr["id"].ToString()),
                            Name = dr["zxmc"].ToString(),
                            ParentID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["zxbh"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 调用创建树函数
                    BuildENTree();
                    // 在下拉框显示方法数
                    comb_ffs.Items.Clear();
                    comb_ffs.Items.Add("--请选择方法数--");
                    comb_ffs.SelectedIndex = 0;
                    sql = String.Format("select distinct ff from t_rule_wg_bm where bmbh = '{0}' order by ff", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        comb_ffs.Items.Add("方法" + numbertochinese(dr["ff"].ToString()));
                    }
                    dr.Close();
                    conn.Close();
                }
            }

            // 内伤
            if (bmlx == "1")
            {
                // 清空
                nodes.Clear();
                // 判断是否存在该病名的推理规则
                sql = String.Format("select count(*) from t_rule_ns_bm where bmbh = '{0}'", m_bmbh);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count = (int)comm.ExecuteScalar();
                if (count == 0) {
                    // 清空treeview，先清空结点，再调用创建树函数
                    nodes.Clear();
                    BuildENTree();
                    MessageBox.Show("不存在该病名的推理规则，请录入！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                conn.Close();
                // 若存在该病名的推理规则，显示 treeview
                if (count > 0)
                {
                    // 将数据库数据写入 List 集合
                    // 一级树写入           
                    sql = String.Format("select distinct ff from t_rule_ns_bm where bmbh = '{0}'", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node { ID = Convert.ToInt32(dr["ff"]), Name = bmmc.Text.Trim() + "的推理规则方法" + numbertochinese(dr["ff"].ToString()) + "（规则：所有条件均成立）" });
                    }
                    dr.Close();
                    conn.Close();

                    // 二级树写入
                    sql = String.Format("select ff, blgz, gzfz from t_rule_ns_bm where bmbh = '{0}' group by ff, blgz, gzfz", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString()),
                            Name = "条件" + numbertochinese(dr["blgz"].ToString()) + "（规则：任" + numbertochinese(dr["gzfz"].ToString()) + "组成立）",
                            ParentID = Convert.ToInt32(dr["ff"])
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 三级树写入
                    sql = String.Format("select ff, blgz, tjzb, znfz from t_rule_ns_bm where bmbh = '{0}' group by ff, blgz, tjzb, znfz ", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString()),
                            Name = "组别" + numbertochinese(dr["tjzb"].ToString()) + "（规则：任" + numbertochinese(dr["znfz"].ToString()) + "症成立）",
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 四级树写入
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_ns_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["zxbh"].ToString()),
                            Name = dr["zxbh"].ToString() + "  " + dr[4].ToString() + "【症象】",
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 五级树写入
                    sql = String.Format("select t2.id, t1.blgz, t1.tjzb, t1.ff, t1.zxbh, t2.zxmc from t_rule_ns_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' group by t2.id, t1.blgz, t1.tjzb, t1.ff, t1.zxbh, t2.zxmc ", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["zxbh"].ToString() + dr["id"].ToString()),
                            Name = dr["zxmc"].ToString(),
                            ParentID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["zxbh"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 调用创建树函数
                    BuildENTree();
                    // 在下拉框显示方法数
                    comb_ffs.Items.Clear();
                    comb_ffs.Items.Add("--请选择方法数--");
                    comb_ffs.SelectedIndex = 0;
                    sql = String.Format("select distinct ff from t_rule_ns_bm where bmbh = '{0}' order by ff", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        comb_ffs.Items.Add("方法" + numbertochinese(dr["ff"].ToString()));
                    }
                    dr.Close();
                    conn.Close();
                }
            }
            // 显示【病名规则录入】中的病名名称
            bmmc_1.Text = bmmc.Text;            
        }

        /// <summary>
        /// 功能：调用【病名信息管理】窗口,【选定】病名名称
        /// </summary>
        private void btn_bmmc_Click(object sender, RoutedEventArgs e)
        {
            DiseaseInfoAdmin diseaseinfoadmin = new DiseaseInfoAdmin();
            diseaseinfoadmin.PassValuesEvent += new DiseaseInfoAdmin.PassValuesHandler(ReceiveValues);
            diseaseinfoadmin.Show();
        }

        /// <summary>
        /// 功能：实现病名名称的读取和显示
        /// </summary>
        private void ReceiveValues(object sender, DiseaseInfoAdmin.PassValuesEventArgs e)
        {
            bmmc.Text = e.Name;
            // 显示【病名规则录入】中的病名名称
            bmmc_1.Text = bmmc.Text;   
            comb_ffs.SelectedIndex = 0;
            comb_tjs.SelectedIndex = 0;
            comb_zbs.SelectedIndex = 0;
            comb_tjlx.SelectedIndex = 0;
            comb_tjfz.SelectedIndex = -1;
            comb_zlfz.SelectedIndex = -1;
            // 读取病名编号和病名类型
            string sql = String.Format("select bmbh, bmlx from t_info_bm where bmmc = '{0}'",bmmc.Text.Trim());
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                m_bmbh = dr["bmbh"].ToString();
                bmlx = dr["bmlx"].ToString();
            }
            dr.Close();
            conn.Close();

            // 外感
            if (bmlx == "0")
            {
                comb_tjlx.Items.Clear();
                comb_tjlx.Items.Add("--请选择条件类型--");
                comb_tjlx.SelectedIndex = 0;
                comb_tjlx.Items.Add("症象");
                comb_tjlx.Items.Add("基本病机");
                // 清空
                nodes.Clear();
                // 判断是否存在该病名的推理规则
                sql = String.Format("select count(*) from t_rule_wg_bm where bmbh = '{0}'",m_bmbh);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count = (int)comm.ExecuteScalar();
                if (count == 0) {
                    // 清空treeview，先清空结点，再调用创建树函数
                    nodes.Clear();
                    BuildENTree();
                    MessageBox.Show("不存在该病名的推理规则，请录入！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                conn.Close();
                // 若存在该病名的推理规则，显示 treeview
                if (count > 0)
                {
                    // 将数据库数据写入 List 集合
                    // 一级树写入           
                    sql = String.Format("select distinct ff from t_rule_wg_bm where bmbh = '{0}'", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node { 
                            ID = Convert.ToInt32(dr["ff"]), 
                            Name = bmmc.Text.Trim() + "的推理规则方法" + numbertochinese(dr["ff"].ToString()) + "（规则：所有条件均成立）" });
                    }
                    dr.Close();
                    conn.Close();

                    // 二级树写入
                    sql = String.Format("select ff, blgz, gzfz from t_rule_wg_bm where bmbh = '{0}' group by ff, blgz, gzfz", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString()),
                            Name = "条件" + numbertochinese(dr["blgz"].ToString()) + "（规则：任" + numbertochinese(dr["gzfz"].ToString()) + "组成立）",
                            ParentID = Convert.ToInt32(dr["ff"])
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 三级树写入
                    sql = String.Format("select ff, blgz, tjzb, znfz from t_rule_wg_bm where bmbh = '{0}' group by ff, blgz, tjzb, znfz ", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString()),
                            Name = "组别" + numbertochinese(dr["tjzb"].ToString()) + "（规则：任" + numbertochinese(dr["znfz"].ToString()) + "症成立）",
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 四级树写入
                    // 写入症象信息
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_wg_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["zxbh"].ToString()),
                            Name = dr["zxbh"].ToString() + "  " + dr[4].ToString() + "【症象】",
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();
                    // 写入基本病机信息
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from [zjxtserver].[dbo].[t_info_jbbj] as t1 inner join [zjxtserver].[dbo].[t_rule_wg_bm] as t2 on t2.jbbjbh = t1.jbbjbh where bmbh = '{0}' group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh ", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["jbbjbh"].ToString()),
                            Name = dr["jbbjbh"].ToString() + "  " + dr[4].ToString() + "【基本病机】",
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 五级树写入
                    // 写入症象信息
                    sql = String.Format("select t2.id, t1.blgz, t1.tjzb, t1.ff, t1.zxbh, t2.zxmc from t_rule_wg_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' group by t2.id, t1.blgz, t1.tjzb, t1.ff, t1.zxbh, t2.zxmc ", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["zxbh"].ToString() + dr["id"].ToString()),
                            Name = dr["zxmc"].ToString(),
                            ParentID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["zxbh"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();
                    
                    // 调用创建树函数
                    BuildENTree();
                    // 在下拉框显示方法数
                    comb_ffs.Items.Clear();
                    comb_ffs.Items.Add("--请选择方法数--");
                    comb_ffs.SelectedIndex = 0;
                    sql = String.Format("select distinct ff from t_rule_wg_bm where bmbh = '{0}' order by ff", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        comb_ffs.Items.Add("方法" + numbertochinese(dr["ff"].ToString()));
                    }
                    dr.Close();
                    conn.Close();
                }                
            }

            // 内伤
            if (bmlx == "1")
            {
                comb_tjlx.Items.Clear();
                comb_tjlx.Items.Add("--请选择条件类型--");
                comb_tjlx.SelectedIndex = 0;
                comb_tjlx.Items.Add("症象");
                // 清空
                nodes.Clear();
                // 判断是否存在该病名的推理规则
                sql = String.Format("select count(*) from t_rule_ns_bm where bmbh = '{0}'", m_bmbh);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count = (int)comm.ExecuteScalar();
                if (count == 0) {
                    // 清空treeview，先清空结点，再调用创建树函数
                    nodes.Clear();
                    BuildENTree();
                    MessageBox.Show("不存在该病名的推理规则，请录入！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                conn.Close();
                // 若存在该病名的推理规则，显示 treeview
                if (count > 0)
                {
                    // 将数据库数据写入 List 集合
                    // 一级树写入           
                    sql = String.Format("select distinct ff from t_rule_ns_bm where bmbh = '{0}'", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node 
                        { 
                            ID = Convert.ToInt32(dr["ff"]), 
                            Name = bmmc.Text.Trim() + "的推理规则方法" + numbertochinese(dr["ff"].ToString()) + "（规则：所有条件均成立）" 
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 二级树写入
                    sql = String.Format("select ff, blgz, gzfz from t_rule_ns_bm where bmbh = '{0}' group by ff, blgz, gzfz", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString()),
                            Name = "条件" + numbertochinese(dr["blgz"].ToString()) + "（规则：任" + numbertochinese(dr["gzfz"].ToString()) + "组成立）",
                            ParentID = Convert.ToInt32(dr["ff"])
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 三级树写入
                    sql = String.Format("select ff, blgz, tjzb, znfz from t_rule_ns_bm where bmbh = '{0}' group by ff, blgz, tjzb, znfz ", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString()),
                            Name = "组别" + numbertochinese(dr["tjzb"].ToString()) + "（规则：任" + numbertochinese(dr["znfz"].ToString()) + "症成立）",
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 四级树写入
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_ns_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["zxbh"].ToString()),
                            Name = dr["zxbh"].ToString() + "  " + dr[4].ToString() + "【症象】",
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 五级树写入
                    sql = String.Format("select t2.id, t1.blgz, t1.tjzb, t1.ff, t1.zxbh, t2.zxmc from t_rule_ns_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' group by t2.id, t1.blgz, t1.tjzb, t1.ff, t1.zxbh, t2.zxmc ", m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["zxbh"].ToString() + dr["id"].ToString()),
                            Name = dr["zxmc"].ToString(),
                            ParentID = Convert.ToInt64(dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString() + dr["zxbh"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 调用创建树函数
                    BuildENTree();
                }
                // 在下拉框显示方法数
                comb_ffs.Items.Clear();
                comb_ffs.Items.Add("--请选择方法数--");
                comb_ffs.SelectedIndex = 0;
                sql = String.Format("select distinct ff from t_rule_ns_bm where bmbh = '{0}' order by ff", m_bmbh);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    comb_ffs.Items.Add("方法" + numbertochinese(dr["ff"].ToString()));
                }
                dr.Close();
                conn.Close();
            }        
        }

        /// <summary>
        /// 功能：方法数下拉框关闭触发事件
        /// </summary>
        private void comb_ffs_DropDownClosed(object sender, EventArgs e)
        {
            comb_tjs.Items.Clear();
            comb_tjs.Items.Add("--请选择条件数--");
            comb_tjs.SelectedIndex = 0;
            comb_zbs.Items.Clear();
            comb_zbs.Items.Add("--请选择组别数--");
            comb_zbs.SelectedIndex = 0;
            // 外感
            if (bmlx == "0")
            {
                // 在方法数选定的前提下
                if (comb_ffs.SelectedIndex > 0)
                {
                    string sql = String.Format("select distinct blgz from t_rule_wg_bm where ff = '{0}' and bmbh = '{1}'", comb_ffs.SelectedIndex, m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        comb_tjs.Items.Add("条件" + numbertochinese(dr["blgz"].ToString()));
                    }
                    dr.Close();
                    conn.Close();
                }
            }
            // 内伤
            if (bmlx == "1")
            {
                // 在方法数选定的前提下
                if (comb_ffs.SelectedIndex > 0)
                {
                    string sql = String.Format("select distinct blgz from t_rule_ns_bm where ff = '{0}' and bmbh = '{1}'", comb_ffs.SelectedIndex, m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        comb_tjs.Items.Add("条件" + numbertochinese(dr["blgz"].ToString()));
                    }
                    dr.Close();
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 功能：条件数下拉框关闭触发事件
        /// </summary>
        private void comb_tjs_DropDownClosed(object sender, EventArgs e)
        {
            comb_tjfz.SelectedIndex = -1;
            comb_zbs.Items.Clear();
            comb_zbs.Items.Add("--请选择组别数--");
            comb_zbs.SelectedIndex = 0;
            // 外感
            if (bmlx == "0")
            {
                if (comb_tjs.SelectedIndex > 0)
                {
                    string sql = String.Format("select gzfz, tjzb from t_rule_wg_bm where ff = '{0}' and blgz = '{1}' and bmbh = '{2}' group by gzfz, tjzb", comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        comb_zbs.Items.Add("组别" + numbertochinese(dr["tjzb"].ToString()));
                        comb_tjfz.SelectedIndex = Convert.ToInt32(dr["gzfz"]) - 1;
                    }
                    dr.Close();
                    conn.Close();
                } 
            }
            // 内伤
            if (bmlx == "1")
            {
                if (comb_tjs.SelectedIndex > 0)
                {
                    string sql = String.Format("select gzfz, tjzb from t_rule_ns_bm where ff = '{0}' and blgz = '{1}' and bmbh = '{2}' group by gzfz, tjzb", comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        comb_zbs.Items.Add("组别" + numbertochinese(dr["tjzb"].ToString()));
                        comb_tjfz.SelectedIndex = Convert.ToInt32(dr["gzfz"]) - 1;
                    }
                    dr.Close();
                    conn.Close();
                }
            }
            
        }

        /// <summary>
        /// 功能：组别数下拉框关闭触发事件
        /// </summary>
        private void comb_zbs_DropDownClosed(object sender, EventArgs e)
        {
            comb_zlfz.SelectedIndex = -1;
            // 外感
            if (bmlx == "0")
            { 
                if (comb_zbs.SelectedIndex > 0)
                {
                    string sql = String.Format("select znfz from t_rule_wg_bm where ff = '{0}' and blgz = '{1}' and tjzb = '{2}' and bmbh = '{3}' group by znfz", comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex, m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        comb_zlfz.SelectedIndex = Convert.ToInt32(dr["znfz"]) - 1;
                    }
                    dr.Close();
                    conn.Close();
                    // 刷新
                    // 四级树写入
                    nodes.Clear();
                    // 写入症象信息
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_wg_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh"
                        , m_bmbh ,comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString()),
                            Name = dr["zxbh"].ToString() + "  " + dr[4].ToString()
                        });
                    }
                    dr.Close();
                    conn.Close();
                    // 写入基本病机信息
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from [zjxtserver].[dbo].[t_info_jbbj] as t1 inner join [zjxtserver].[dbo].[t_rule_wg_bm] as t2 on t2.jbbjbh = t1.jbbjbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh "
                        , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["jbbjbh"].ToString()),
                            Name = dr["jbbjbh"].ToString() + "  " + dr[4].ToString()
                        });
                    }
                    dr.Close();
                    conn.Close();
                    // 五级树写入
                    sql = String.Format("select t2.id, t1.ff, t1.zxbh, t2.zxmc from t_rule_wg_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.id, t1.ff, t1.zxbh, t2.zxmc "
                        , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["zxbh"].ToString() + dr["id"].ToString()),
                            Name = dr["zxmc"].ToString(),
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();
                    // 五级树写入
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from t_info_jbbj as t1 inner join t_rule_wg_bm as t2 on t2.jbbjbh = t1.jbbjbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh "
                        , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["jbbjbh"].ToString()),
                            Name = dr[4].ToString(),
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["jbbjbh"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();
                    // 调用创建树函数
                    BuildENTree1();
                } 

            }
            // 内伤
            if (bmlx == "1")
            {
                if (comb_zbs.SelectedIndex > 0)
                {
                    string sql = String.Format("select znfz from t_rule_ns_bm where ff = '{0}' and blgz = '{1}' and tjzb = '{2}' and bmbh = '{3}' group by znfz", comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex, m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        comb_zlfz.SelectedIndex = Convert.ToInt32(dr["znfz"]) - 1;
                    }
                    dr.Close();
                    conn.Close();
                    // 刷新
                    // 四级树写入
                    nodes.Clear();
                    // 写入症象信息
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_ns_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh"
                        , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString()),
                            Name = dr["zxbh"].ToString() + "  " + dr[4].ToString()
                        });
                    }
                    dr.Close();
                    conn.Close();                   
                    // 五级树写入
                    sql = String.Format("select t2.id, t1.ff, t1.zxbh, t2.zxmc from t_rule_ns_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.id, t1.ff, t1.zxbh, t2.zxmc "
                        , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["zxbh"].ToString() + dr["id"].ToString()),
                            Name = dr["zxmc"].ToString(),
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();                   
                    // 调用创建树函数
                    BuildENTree1();
                }
            }
            
        }

        /// <summary>
        /// 功能:方法数增加
        /// </summary>
        private void btn_ffs_Click(object sender, RoutedEventArgs e)
        {
            comb_tjs.Items.Clear();
            comb_tjs.Items.Add("--请选择条件数--");
            comb_tjs.SelectedIndex = 0;
            comb_zbs.Items.Clear();
            comb_zbs.Items.Add("--请选择组别数--");
            comb_zbs.SelectedIndex = 0;
            if (bmmc_1.Text != "")
            {
                // 外感
                if (bmlx == "0")
                {
                    string sql = String.Format("select max(ff) from t_rule_wg_bm where bmbh = '{0}'",m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr[0].ToString() == "")
                        {
                            // 防止重复连续添加
                            if (comb_ffs.Items.Count == 1)
                            {
                                if (dr[0].ToString() != "1")
                                {
                                    comb_ffs.Items.Add("方法" + numbertochinese("1"));
                                }
                            }
                        }
                        else
                        {
                            // 防止重复连续添加
                            if (Convert.ToInt32(dr[0].ToString()) >= comb_ffs.Items.Count - 1)
                            {
                                comb_ffs.Items.Add("方法" + numbertochinese((Convert.ToInt32(dr[0].ToString()) + 1).ToString()));
                            }
                        }
                    }
                    dr.Close();
                    conn.Close();
                    comb_ffs.SelectedIndex = comb_ffs.Items.Count - 1;
                }
                // 内伤
                if (bmlx == "1")
                {
                    string sql = String.Format("select max(ff) from t_rule_ns_bm where bmbh = '{0}'", m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr[0].ToString() == "")
                        {
                            // 防止重复连续添加
                            if (comb_ffs.Items.Count == 1)
                            {
                                if (dr[0].ToString() != "1")
                                {
                                    comb_ffs.Items.Add("方法" + numbertochinese("1"));
                                }
                            }
                        }
                        else
                        {
                            // 防止重复连续添加
                            if (Convert.ToInt32(dr[0].ToString()) >= comb_ffs.Items.Count - 1)
                            {
                                comb_ffs.Items.Add("方法" + numbertochinese((Convert.ToInt32(dr[0].ToString()) + 1).ToString()));
                            }
                        }
                    }
                    dr.Close();
                    conn.Close();
                    comb_ffs.SelectedIndex = comb_ffs.Items.Count - 1;
                }                
            }   
        }

        /// <summary>
        /// 功能:条件数增加
        /// </summary>
        private void btn_tjs_Click(object sender, RoutedEventArgs e)
        {
            comb_zbs.Items.Clear();
            comb_zbs.Items.Add("--请选择组别数--");
            comb_zbs.SelectedIndex = 0;
            if (comb_ffs.SelectedIndex > 0)
            {
                // 外感
                if (bmlx == "0")
                {
                    string sql = String.Format("select max(blgz) from t_rule_wg_bm where bmbh = '{0}' and ff = '{1}'" ,m_bmbh ,comb_ffs.SelectedIndex);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr[0].ToString() == "")
                        {
                            // 防止重复连续添加
                            if (comb_tjs.Items.Count == 1)
                            {
                                if (dr[0].ToString() != "1")
                                {
                                    comb_tjs.Items.Add("条件" + numbertochinese("1"));
                                }
                            }
                        }
                        else
                        {
                            // 防止重复连续添加
                            if (Convert.ToInt32(dr[0].ToString()) >= comb_tjs.Items.Count - 1)
                            {
                                comb_tjs.Items.Add("条件" + numbertochinese((Convert.ToInt32(dr[0].ToString()) + 1).ToString()));
                            }
                        }
                    }
                    dr.Close();
                    conn.Close();
                    comb_tjs.SelectedIndex = comb_tjs.Items.Count - 1;
                }
                // 内伤
                if (bmlx == "1")
                {
                    string sql = String.Format("select max(blgz) from t_rule_ns_bm where bmbh = '{0}' and ff = '{1}'", m_bmbh, comb_ffs.SelectedIndex);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr[0].ToString() == "")
                        {
                            // 防止重复连续添加
                            if (comb_tjs.Items.Count == 1)
                            {
                                if (dr[0].ToString() != "1")
                                {
                                    comb_tjs.Items.Add("条件" + numbertochinese("1"));
                                }
                            }
                        }
                        else
                        {
                            // 防止重复连续添加
                            if (Convert.ToInt32(dr[0].ToString()) >= comb_tjs.Items.Count - 1)
                            {
                                comb_tjs.Items.Add("条件" + numbertochinese((Convert.ToInt32(dr[0].ToString()) + 1).ToString()));
                            }
                        }
                    }
                    dr.Close();
                    conn.Close();
                    comb_tjs.SelectedIndex = comb_tjs.Items.Count - 1;
                }
            }
            else
            {
                MessageBox.Show("请先选择方法数！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 功能:组别数增加
        /// </summary>
        private void btn_zbs_Click(object sender, RoutedEventArgs e)
        {
            if (comb_tjs.SelectedIndex > 0)
            {
                // 外感
                if (bmlx == "0")
                {
                    string sql = String.Format("select max(tjzb) from t_rule_wg_bm where ff = '{0}' and blgz = '{1}' and bmbh = '{2}'", comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr[0].ToString() == "")
                        {
                            // 防止重复连续添加
                            if (comb_zbs.Items.Count == 1)
                            {
                                if (dr[0].ToString() != "1")
                                {
                                    comb_zbs.Items.Add("组别" + numbertochinese("1"));
                                }
                            }
                        }
                        else
                        {
                            // 防止重复连续添加
                            if (Convert.ToInt32(dr[0].ToString()) >= comb_zbs.Items.Count - 1)
                            {
                                comb_zbs.Items.Add("组别" + numbertochinese((Convert.ToInt32(dr[0].ToString()) + 1).ToString()));
                            }
                        }
                    }
                    dr.Close();
                    conn.Close();
                    comb_zbs.SelectedIndex = comb_zbs.Items.Count - 1;
                }
                // 内伤
                if (bmlx == "1")
                {
                    string sql = String.Format("select max(tjzb) from t_rule_ns_bm where ff = '{0}' and blgz = '{1}' and bmbh = '{2}'", comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr[0].ToString() == "")
                        {
                            // 防止重复连续添加
                            if (comb_zbs.Items.Count == 1)
                            {
                                if (dr[0].ToString() != "1")
                                {
                                    comb_zbs.Items.Add("组别" + numbertochinese("1"));
                                }
                            }
                        }
                        else
                        {
                            // 防止重复连续添加
                            if (Convert.ToInt32(dr[0].ToString()) >= comb_zbs.Items.Count - 1)
                            {
                                comb_zbs.Items.Add("组别" + numbertochinese((Convert.ToInt32(dr[0].ToString()) + 1).ToString()));
                            }
                        }
                    }
                    dr.Close();
                    conn.Close();
                    comb_zbs.SelectedIndex = comb_zbs.Items.Count - 1;
                }
                
            }
            else
            {
                MessageBox.Show("请先选择条件数！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 功能：调用【症象信息管理】窗口,【选定】病名名称
        /// </summary>
        private void btn_tjmc_Click(object sender, RoutedEventArgs e)
        {
            if (bmmc_1.Text == "")
            {
                MessageBox.Show("请先选择病名！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (comb_ffs.SelectedIndex == 0)
                {
                    MessageBox.Show("请先选择方法数！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (comb_tjs.SelectedIndex == 0)
                    {
                        MessageBox.Show("请先选择条件数！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        if (comb_zbs.SelectedIndex == 0)
                        {
                            MessageBox.Show("请先选择组别数！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if (comb_tjfz.Text == "")
                            {
                                MessageBox.Show("请先录入该条件下的条件阀值！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                if (comb_zlfz.Text == "")
                                {
                                    MessageBox.Show("请先录入该条件下的组内阀值！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    if (comb_tjlx.SelectedIndex == 0)
                                    {
                                        MessageBox.Show("请选择条件类型！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                    else
                                    {
                                        // 症象
                                        if (comb_tjlx.SelectedIndex == 1)
                                        {
                                            Info_Symptom symptom = new Info_Symptom();
                                            symptom.PassValuesEvent += new Info_Symptom.PassValuesHandler(ReceiveValues);
                                            symptom.Show();
                                        }
                                        // 基本病机
                                        if (comb_tjlx.SelectedIndex == 2)
                                        {
                                            BasicBingji basicbingji = new BasicBingji();
                                            basicbingji.PassValuesEvent += new BasicBingji.PassValuesHandler(ReceiveValues1);
                                            basicbingji.Show();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }           
        }

        /// <summary>
        /// 功能：实现基本病机名称的读取和显示
        /// </summary>
        private void ReceiveValues1(object sender, BasicBingji.PassValuesEventArgs e)
        {
            tjmc.Text = e.Name;
            m_tjbh = e.Number;
            IsAdd = true;
        }

        /// <summary>
        /// 功能：实现症象名称的读取和显示
        /// </summary>
        private void ReceiveValues(object sender, Info_Symptom.PassValuesEventArgs e)
        {
            tjmc.Text = e.Name;
            m_tjbh = e.Number;
            IsAdd = true;
        }

        /// <summary>
        /// 功能：【添加】功能
        /// </summary>
        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdd)
            {
                Is_Repeat();
                if (IsRepeat)
                {
                    MessageBox.Show("该条件已添加！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // 外感
                    if (bmlx == "0")
                    {
                        // 症象
                        if (comb_tjlx.SelectedIndex == 1)
                        {
                            string sql = String.Format("insert into t_rule_wg_bm ( ff, blgz, zxbh, tjzb, znfz, gzfz, bmbh, tjlx) values( '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')"
                                , comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, m_tjbh, comb_zbs.SelectedIndex, comb_zlfz.Text, comb_tjfz.Text, m_bmbh, comb_tjlx.SelectedIndex.ToString());
                            conn.Open();
                            SqlCommand comm = new SqlCommand(sql, conn);
                            int count = comm.ExecuteNonQuery();
                            conn.Close();
                            IsAdd = false;
                            // 刷新
                            // 四级树写入
                            nodes.Clear();
                            // 写入症象信息
                            sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_wg_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh"
                                , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            SqlDataReader dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                nodes.Add(new Node
                                {
                                    ID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString()),
                                    Name = dr["zxbh"].ToString() + "  " + dr[4].ToString()
                                });
                            }
                            dr.Close();
                            conn.Close();
                            // 写入基本病机信息
                            sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from [zjxtserver].[dbo].[t_info_jbbj] as t1 inner join [zjxtserver].[dbo].[t_rule_wg_bm] as t2 on t2.jbbjbh = t1.jbbjbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh "
                                , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                nodes.Add(new Node
                                {
                                    ID = Convert.ToInt32(dr["ff"].ToString() + dr["jbbjbh"].ToString()),
                                    Name = dr["jbbjbh"].ToString() + "  " + dr[4].ToString()
                                });
                            }
                            dr.Close();
                            conn.Close();
                            // 五级树写入
                            sql = String.Format("select t2.id, t1.ff, t1.zxbh, t2.zxmc from t_rule_wg_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.id, t1.ff, t1.zxbh, t2.zxmc "
                                , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                nodes.Add(new Node
                                {
                                    ID = Convert.ToInt64(dr["ff"].ToString() + dr["zxbh"].ToString() + dr["id"].ToString()),
                                    Name = dr["zxmc"].ToString(),
                                    ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString())
                                });
                            }
                            dr.Close();
                            conn.Close();
                            // 五级树写入
                            sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from t_info_jbbj as t1 inner join t_rule_wg_bm as t2 on t2.jbbjbh = t1.jbbjbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh "
                                , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                nodes.Add(new Node
                                {
                                    ID = Convert.ToInt32(dr["jbbjbh"].ToString()),
                                    Name = dr[4].ToString(),
                                    ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["jbbjbh"].ToString())
                                });
                            }
                            dr.Close();
                            conn.Close();
                            // 调用创建树函数
                            BuildENTree1();
                        }
                        // 基本病机
                        if(comb_tjlx.SelectedIndex == 2)
                        {
                            string sql = String.Format("insert into t_rule_wg_bm ( ff, blgz, jbbjbh, tjzb, znfz, gzfz, bmbh, tjlx) values( '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')"
                               , comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, m_tjbh, comb_zbs.SelectedIndex, comb_zlfz.Text, comb_tjfz.Text, m_bmbh, comb_tjlx.SelectedIndex.ToString());
                            conn.Open();
                            SqlCommand comm = new SqlCommand(sql, conn);
                            int count = comm.ExecuteNonQuery();
                            conn.Close();
                            IsAdd = false;
                            // 刷新
                            // 四级树写入
                            nodes.Clear();
                            // 写入症象信息
                            sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_wg_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh"
                                , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            SqlDataReader dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                nodes.Add(new Node
                                {
                                    ID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString()),
                                    Name = dr["zxbh"].ToString() + "  " + dr[4].ToString()
                                });
                            }
                            dr.Close();
                            conn.Close();
                            // 写入基本病机信息
                            sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from [zjxtserver].[dbo].[t_info_jbbj] as t1 inner join [zjxtserver].[dbo].[t_rule_wg_bm] as t2 on t2.jbbjbh = t1.jbbjbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh "
                                , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                nodes.Add(new Node
                                {
                                    ID = Convert.ToInt32(dr["ff"].ToString() + dr["jbbjbh"].ToString()),
                                    Name = dr["jbbjbh"].ToString() + "  " + dr[4].ToString()
                                });
                            }
                            dr.Close();
                            conn.Close();
                            // 五级树写入
                            sql = String.Format("select t2.id, t1.ff, t1.zxbh, t2.zxmc from t_rule_wg_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.id, t1.ff, t1.zxbh, t2.zxmc "
                                , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                nodes.Add(new Node
                                {
                                    ID = Convert.ToInt64(dr["ff"].ToString() + dr["zxbh"].ToString() + dr["id"].ToString()),
                                    Name = dr["zxmc"].ToString(),
                                    ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString())
                                });
                            }
                            dr.Close();
                            conn.Close();
                            // 五级树写入
                            sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from t_info_jbbj as t1 inner join t_rule_wg_bm as t2 on t2.jbbjbh = t1.jbbjbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh "
                                , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                nodes.Add(new Node
                                {
                                    ID = Convert.ToInt32(dr["jbbjbh"].ToString()),
                                    Name = dr[4].ToString(),
                                    ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["jbbjbh"].ToString())
                                });
                            }
                            dr.Close();
                            conn.Close();
                            // 调用创建树函数
                            BuildENTree1();
                        }                       
                    }
                    // 内伤
                    if (bmlx == "1")
                    {
                        string sql = String.Format("insert into t_rule_ns_bm ( ff, blgz, zxbh, tjzb, znfz, gzfz, bmbh) values( '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')"
                               , comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, m_tjbh, comb_zbs.SelectedIndex, comb_zlfz.Text, comb_tjfz.Text, m_bmbh);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                        conn.Close();
                        IsAdd = false;
                        // 刷新
                        // 四级树写入
                        nodes.Clear();
                        sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_ns_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh"
                            , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        SqlDataReader dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            nodes.Add(new Node
                            {
                                ID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString()),
                                Name = dr["zxbh"].ToString() + "  " + dr[4].ToString() + "【症象】"
                            });
                        }
                        dr.Close();
                        conn.Close();

                        // 五级树写入
                        sql = String.Format("select t2.id, t1.ff, t1.zxbh, t2.zxmc from t_rule_ns_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}' group by t2.id, t1.ff, t1.zxbh, t2.zxmc "
                            , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            nodes.Add(new Node
                            {
                                ID = Convert.ToInt64(dr["ff"].ToString() + dr["zxbh"].ToString() + dr["id"].ToString()),
                                Name = dr["zxmc"].ToString(),
                                ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString())
                            });
                        }
                        dr.Close();
                        conn.Close();
                        // 调用创建树函数
                        BuildENTree1();
                    }                             
                }
            }
        }

        /// <summary>
        /// 功能：【删除】功能
        /// </summary>
        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            Node node = treeview1.SelectedItem as Node;
            if (node != null && node is Node)
            {
                // 外感
                if(bmlx == "0")
                {
                    // 症象
                    string sql = String.Format("delete from t_rule_wg_bm where ff = '{0}' and blgz = '{1}' and zxbh = '{2}' and tjzb = '{3}' and bmbh = '{4}'"
                        , comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, node.ID.ToString().Substring(1), comb_zbs.SelectedIndex, m_bmbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    conn.Close();
                    // 基本病机
                    sql = String.Format("delete from t_rule_wg_bm where ff = '{0}' and blgz = '{1}' and jbbjbh = '{2}' and tjzb = '{3}' and bmbh = '{4}'"
                        , comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, node.ID.ToString().Substring(1), comb_zbs.SelectedIndex, m_bmbh);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count = comm.ExecuteNonQuery();
                    conn.Close();
                    // 刷新
                    // 四级树写入
                    nodes.Clear();
                    // 写入症象信息
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_wg_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh"
                        , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString()),
                            Name = dr["zxbh"].ToString() + "  " + dr[4].ToString()
                        });
                    }
                    dr.Close();
                    conn.Close();
                    // 写入基本病机信息
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from [zjxtserver].[dbo].[t_info_jbbj] as t1 inner join [zjxtserver].[dbo].[t_rule_wg_bm] as t2 on t2.jbbjbh = t1.jbbjbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh "
                        , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["jbbjbh"].ToString()),
                            Name = dr["jbbjbh"].ToString() + "  " + dr[4].ToString()
                        });
                    }
                    dr.Close();
                    conn.Close();
                    // 五级树写入
                    sql = String.Format("select t2.id, t1.ff, t1.zxbh, t2.zxmc from t_rule_wg_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.id, t1.ff, t1.zxbh, t2.zxmc "
                        , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["zxbh"].ToString() + dr["id"].ToString()),
                            Name = dr["zxmc"].ToString(),
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();
                    // 五级树写入
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from t_info_jbbj as t1 inner join t_rule_wg_bm as t2 on t2.jbbjbh = t1.jbbjbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh "
                        , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["jbbjbh"].ToString()),
                            Name = dr[4].ToString(),
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["jbbjbh"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();
                    // 调用创建树函数
                    BuildENTree1();
                }
                // 内伤
                if(bmlx == "1")
                {
                    // 删除
                    string sql = String.Format("delete from t_rule_ns_bm where ff = '{0}' and blgz = '{1}' and zxbh = '{2}' and tjzb = '{3}'"
                        , comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, node.ID.ToString().Substring(1), comb_zbs.SelectedIndex);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    conn.Close();
                    // 刷新
                    // 四级树写入
                    nodes.Clear();
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_ns_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh"
                        , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString()),
                            Name = dr["zxbh"].ToString() + "  " + dr[4].ToString() + "【症象】"
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 五级树写入
                    sql = String.Format("select t2.id, t1.ff, t1.zxbh, t2.zxmc from t_rule_ns_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}' group by t2.id, t1.ff, t1.zxbh, t2.zxmc "
                        , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["zxbh"].ToString() + dr["id"].ToString()),
                            Name = dr["zxmc"].ToString(),
                            ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();
                    // 调用创建树函数
                    BuildENTree1();
                }
            }
        }

        /// <summary>
        /// 功能：组别数选择变化时触发事件
        /// </summary>
        private void comb_zbs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 外感
            if (bmlx == "0")
            {                   
                // 刷新
                // 四级树写入
                nodes.Clear();
                // 写入症象信息
                string sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_wg_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh"
                    , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    nodes.Add(new Node
                    {
                        ID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString()),
                        Name = dr["zxbh"].ToString() + "  " + dr[4].ToString()
                    });
                }
                dr.Close();
                conn.Close();
                // 写入基本病机信息
                sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from [zjxtserver].[dbo].[t_info_jbbj] as t1 inner join [zjxtserver].[dbo].[t_rule_wg_bm] as t2 on t2.jbbjbh = t1.jbbjbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh "
                    , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    nodes.Add(new Node
                    {
                        ID = Convert.ToInt32(dr["ff"].ToString() + dr["jbbjbh"].ToString()),
                        Name = dr["jbbjbh"].ToString() + "  " + dr[4].ToString()
                    });
                }
                dr.Close();
                conn.Close();
                // 五级树写入
                sql = String.Format("select t2.id, t1.ff, t1.zxbh, t2.zxmc from t_rule_wg_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.id, t1.ff, t1.zxbh, t2.zxmc "
                    , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    nodes.Add(new Node
                    {
                        ID = Convert.ToInt64(dr["ff"].ToString() + dr["zxbh"].ToString() + dr["id"].ToString()),
                        Name = dr["zxmc"].ToString(),
                        ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString())
                    });
                }
                dr.Close();
                conn.Close();
                // 五级树写入
                sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from t_info_jbbj as t1 inner join t_rule_wg_bm as t2 on t2.jbbjbh = t1.jbbjbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh "
                    , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    nodes.Add(new Node
                    {
                        ID = Convert.ToInt32(dr["jbbjbh"].ToString()),
                        Name = dr[4].ToString(),
                        ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["jbbjbh"].ToString())
                    });
                }
                dr.Close();
                conn.Close();
                // 调用创建树函数
                BuildENTree1();
            }
            // 内伤
            if (bmlx == "1")
            {            
                // 刷新
                // 四级树写入
                nodes.Clear();
                // 写入症象信息
                string sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_ns_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh"
                    , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    nodes.Add(new Node
                    {
                        ID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString()),
                        Name = dr["zxbh"].ToString() + "  " + dr[4].ToString()
                    });
                }
                dr.Close();
                conn.Close();
                // 五级树写入
                sql = String.Format("select t2.id, t1.ff, t1.zxbh, t2.zxmc from t_rule_ns_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' and  ff = '{1}' and blgz = '{2}' and tjzb = '{3}'  group by t2.id, t1.ff, t1.zxbh, t2.zxmc "
                    , m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex, comb_zbs.SelectedIndex);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    nodes.Add(new Node
                    {
                        ID = Convert.ToInt64(dr["ff"].ToString() + dr["zxbh"].ToString() + dr["id"].ToString()),
                        Name = dr["zxmc"].ToString(),
                        ParentID = Convert.ToInt32(dr["ff"].ToString() + dr["zxbh"].ToString())
                    });
                }
                dr.Close();
                conn.Close();
                // 调用创建树函数
                BuildENTree1();
            }
        }

        /// <summary>
        /// 功能：全部展开
        /// </summary>
        private void expand_all_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in treeview.Items)
            {
                DependencyObject dObject = treeview.ItemContainerGenerator.ContainerFromItem(item);
                ((TreeViewItem)dObject).ExpandSubtree();
            }
        }

        /// <summary>
        /// 功能：全部收缩
        /// </summary>
        private void collapse_all_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in treeview.Items)
            {
                DependencyObject dObject = treeview.ItemContainerGenerator.ContainerFromItem(item);
                CollapseTreeviewItems(((TreeViewItem)dObject));
            }
        }

        /// <summary>
        /// 功能：收缩
        /// </summary>
        /// <param name="Item"></param>
        private void CollapseTreeviewItems(TreeViewItem Item)
        {
            Item.IsExpanded = false;
            foreach (var item in Item.Items)
            {
                DependencyObject dObject = treeview.ItemContainerGenerator.ContainerFromItem(item);
                if (dObject != null)
                {
                    ((TreeViewItem)dObject).IsExpanded = false;
                    if (((TreeViewItem)dObject).HasItems)
                    {
                        CollapseTreeviewItems(((TreeViewItem)dObject));
                    }
                }
            }
        }

        /// <summary>
        /// 功能：条件阀值下拉框关闭触发事件
        /// </summary>
        private void comb_tjfz_DropDownClosed(object sender, EventArgs e)
        {
            // 外感
            if (bmlx == "0")
            {
                if (comb_ffs.SelectedIndex != 0 && comb_tjs.SelectedIndex != 0 && comb_zbs.SelectedIndex != 0)
                {
                    String sql = String.Format("update t_rule_wg_bm set gzfz = '{0}' where bmbh = '{1}' and ff = '{2}' and blgz = '{3}'"
                        , comb_tjfz.Text, m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            // 内伤
            if (bmlx == "1")
            {
                if (comb_ffs.SelectedIndex != 0 && comb_tjs.SelectedIndex != 0 && comb_zbs.SelectedIndex != 0)
                {
                    String sql = String.Format("update t_rule_ns_bm set gzfz = '{0}' where bmbh = '{1}' and ff = '{2}' and blgz = '{3}'"
                        , comb_tjfz.Text, m_bmbh, comb_ffs.SelectedIndex, comb_tjs.SelectedIndex);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }           
        }

        /// <summary>
        /// 功能：组内阀值下拉框关闭触发事件
        /// </summary>
        private void comb_zlfz_DropDownClosed(object sender, EventArgs e)
        {
            // 外感
            if (bmlx == "0")
            {
                if (comb_ffs.SelectedIndex != 0 && comb_tjs.SelectedIndex != 0 && comb_zbs.SelectedIndex != 0)
                {
                    String sql = String.Format("update t_rule_wg_bm set znfz = '{0}' where bmbh = '{1}' and ff = '{2}' and tjzb = '{3}'"
                        , comb_zlfz.Text, m_bmbh, comb_ffs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            // 内伤
            if (bmlx == "1")
            {
                if (comb_ffs.SelectedIndex != 0 && comb_tjs.SelectedIndex != 0 && comb_zbs.SelectedIndex != 0)
                {
                    String sql = String.Format("update t_rule_ns_bm set znfz = '{0}' where bmbh = '{1}' and ff = '{2}' and tjzb = '{3}'"
                        , comb_zlfz.Text, m_bmbh, comb_ffs.SelectedIndex, comb_zbs.SelectedIndex);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
    }
}
