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
    /// Interaction logic for ZhengmldSearch.xaml
    /// </summary>
    public partial class ZhengmldSearch : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 创建 List 集合实例
        List<Node> nodes = new List<Node>();
        // 创建哈希表实例
        Hashtable httree = new Hashtable(1000);
        // 全局变量，用于暂时存储【基本病名信息管理】中的病名编号
        public string m_jbzmbh = ""; // 基本证名编号
        public string bmnumber = ""; // 病名编号
        public string bmlx = "";     // 病名类型
        public string bmname = "";   // 病名名称
        public string m_jbzmlx = ""; // 基本证名类型
        int count;

        public ZhengmldSearch()
        {
            InitializeComponent();
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
        /// 功能：调用【基本证名信息管理】窗口,【选定】病名名称
        /// </summary>
        private void btn_zmmc_Click(object sender, RoutedEventArgs e)
        {
            BasicZhengm basiczhengm = new BasicZhengm();
            basiczhengm.PassValuesEvent += new BasicZhengm.PassValuesHandler(ReceiveValues);
            basiczhengm.Show();
        }

        private void ReceiveValues(object sender, BasicZhengm.PassValuesEventArgs e)
        {
            zmmc.Text = e.Name;
            m_jbzmbh = e.Number;
            // 判断是否存在该病名的推理规则
            string sql = String.Format("select count(*) from t_info_jbzmbm where jbzmbh = '{0}'", m_jbzmbh);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            count = (int)comm.ExecuteScalar();
            if (count == 0)
                MessageBox.Show("不存在该证名所属的病名，请录入！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            conn.Close();
            // 根据基本证名编号读取病名编号
            sql = String.Format("select bmbh from t_info_jbzmbm where jbzmbh = '{0}'", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                bmnumber = dr[0].ToString();
            }
            dr.Close();
            conn.Close();
            // 根据病名编号读取病名名称和病名类型
            sql = String.Format("select bmmc, bmlx from t_info_bm where bmbh = '{0}'", bmnumber);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                bmname = dr[0].ToString();
                bmlx = dr[1].ToString();
            }
            dr.Close();
            conn.Close();

            /**
             * 目录一：该证名所属的内伤/外感病名
             **/

            // 外感病名
            if (bmlx == "0")
            {
                nodes.Clear();
                // 判断是否存在该病名的推理规则
                sql = String.Format("select count(*) from t_rule_wg_bm where bmbh = '{0}'", bmnumber);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = (int)comm.ExecuteScalar();
                if (count == 0)
                    MessageBox.Show("不存在该病名的推理规则，请录入！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                conn.Close();
                // 若存在该病名的推理规则，显示 treeview
                if (count > 0)
                { 
                    // 一级树写入
                    nodes.Add(new Node { 
                            ID = Convert.ToInt64(bmnumber), 
                            Name = "该证名所属的外感病名：" + bmname }
                        );
                    // 二级树写入【方法】
                    sql = String.Format("select distinct ff from t_rule_wg_bm where bmbh = '{0}'", bmnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(bmnumber + dr["ff"]),
                            Name = bmname + "的推理规则方法" + numbertochinese(dr["ff"].ToString()) + "（规则：该方法下所有条件均成立）",
                            ParentID = Convert.ToInt64(bmnumber)
                        });
                    }
                    dr.Close();
                    conn.Close();
                    // 三级树写入【条件】
                    sql = String.Format("select ff, blgz, gzfz from t_rule_wg_bm where bmbh = '{0}' group by ff, blgz, gzfz", bmnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["blgz"].ToString()),
                            Name = "条件" + numbertochinese(dr["blgz"].ToString()) + "（规则：本条件中任" + numbertochinese(dr["gzfz"].ToString()) + "组成立）",
                            ParentID = Convert.ToInt64(bmnumber + dr["ff"])
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 四级树写入【组别】
                    sql = String.Format("select ff, blgz, tjzb, znfz,gzfz from t_rule_wg_bm where bmbh = '{0}' group by ff, blgz, tjzb, znfz,gzfz ", bmnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString()),
                            Name = "组别" + numbertochinese(dr["tjzb"].ToString()) + "（规则：该组内任" + numbertochinese(dr["znfz"].ToString()) + "症成立）",
                            ParentID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["blgz"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 五级树写入
                    // 写入症象信息
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_wg_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh", bmnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["zxbh"].ToString()),
                            Name = dr["zxbh"].ToString() + "  " + dr[4].ToString() + "【症象】",
                            ParentID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();
                    // 写入基本病机信息
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from [zjxtserver].[dbo].[t_info_jbbj] as t1 inner join [zjxtserver].[dbo].[t_rule_wg_bm] as t2 on t2.jbbjbh = t1.jbbjbh where bmbh = '{0}' group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh ", bmnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["jbbjbh"].ToString()),
                            Name = dr["jbbjbh"].ToString() + "  " + dr[4].ToString() + "【基本病机】",
                            ParentID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 六级树写入
                    // 写入症象信息
                    sql = String.Format("select t2.id, t1.ff, t1.zxbh, t2.zxmc from t_rule_wg_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' group by t2.id, t1.ff, t1.zxbh, t2.zxmc ", bmnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["id"].ToString()),
                            Name = dr["zxmc"].ToString(),
                            ParentID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["zxbh"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    //// 调用创建树函数
                    //BuildENTree();
                }
            }

            // 内伤
            if (bmlx == "1")
            {
                // 清空
                nodes.Clear();
                // 判断是否存在该病名的推理规则
                sql = String.Format("select count(*) from t_rule_ns_bm where bmbh = '{0}'", bmnumber);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = (int)comm.ExecuteScalar();
                if (count == 0)
                    MessageBox.Show("不存在该病名的推理规则，请录入！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                conn.Close();
                // 若存在该病名的推理规则，显示 treeview
                if (count > 0)
                {
                    // 一级树写入
                    nodes.Add(new Node { ID = Convert.ToInt64(bmnumber), Name = "该证名所属的内伤病名：" + bmname });
                    // 二级树写入
                    sql = String.Format("select distinct ff from t_rule_ns_bm where bmbh = '{0}'", bmnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(bmnumber + dr["ff"]),
                            Name = bmname + "的推理规则方法" + numbertochinese(dr["ff"].ToString()) + "（规则：该方法下所有条件均成立）",
                            ParentID = Convert.ToInt64(bmnumber)
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 三级树写入
                    sql = String.Format("select ff, blgz, gzfz from t_rule_ns_bm where bmbh = '{0}' group by ff, blgz, gzfz", bmnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["blgz"].ToString()),
                            Name = "条件" + numbertochinese(dr["blgz"].ToString()) + "（规则：本条件中任" + numbertochinese(dr["gzfz"].ToString()) + "组成立）",
                            ParentID = Convert.ToInt64(bmnumber + dr["ff"])
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 四级树写入
                    sql = String.Format("select ff, blgz, tjzb, znfz from t_rule_ns_bm where bmbh = '{0}' group by ff, blgz, tjzb, znfz ", bmnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString()),
                            Name = "组别" + numbertochinese(dr["tjzb"].ToString()) + "（规则：该组内任" + numbertochinese(dr["znfz"].ToString()) + "症成立）",
                            ParentID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["blgz"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 五级树写入
                    sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_ns_bm as t2 on t2.zxbh = t1.zxbh where bmbh = '{0}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh,t2.id", bmnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["zxbh"].ToString()),
                            Name = dr["zxbh"].ToString() + "  " + dr[4].ToString() + "【症象】",
                            ParentID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();

                    // 六级树写入
                    sql = String.Format("select t2.id, t1.ff, t1.zxbh, t2.zxmc from t_rule_ns_bm as t1 inner join t_info_zxmx as t2 on t1.zxbh = t2.zxbh where bmbh = '{0}' group by t2.id, t1.ff, t1.zxbh, t2.zxmc ", bmnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        nodes.Add(new Node
                        {
                            ID = Convert.ToInt64(dr["ff"].ToString() + dr["id"].ToString()),
                            Name = dr["zxmc"].ToString(),
                            ParentID = Convert.ToInt64(bmnumber + dr["ff"].ToString() + dr["zxbh"].ToString())
                        });
                    }
                    dr.Close();
                    conn.Close();
                    //// 调用创建树函数
                    //BuildENTree();
                }
            }

            /**
             * 目录二：该证名的推理规则方法
             **/
            
            // 根据基本证名编号读取基本证名类型
            sql = String.Format("select jbzmlx from t_info_jbzm where jbzmbh = '{0}'", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                m_jbzmlx = dr["jbzmlx"].ToString();

            }
            dr.Close();
            conn.Close();
            // 判断是否存在该病名的推理规则
            sql = String.Format("select count(*) from t_rule_jbzm where jbzmbh = '{0}'", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            count = (int)comm.ExecuteScalar();
            if (count == 0)
                MessageBox.Show("不存在该病名的推理规则，请录入！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            conn.Close();
            // 若存在该病名的推理规则，显示 treeview
            if (count > 0)
            {
                //一级树写入
                sql = String.Format("select distinct ff from t_rule_jbzm where jbzmbh = '{0}'", m_jbzmbh);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    nodes.Add(new Node {
                        ID = Convert.ToInt64(m_jbzmbh + dr["ff"]), 
                        Name = "该证名的推理规则方法" + numbertochinese(dr["ff"].ToString()) + "（规则：该方法下所有条件均成立）" 
                    });
                }
                dr.Close();
                conn.Close();

                // 二级树写入【条件】（blgz）
                sql = String.Format("select ff, blgz, gzfz from t_rule_jbzm where jbzmbh = '{0}' group by ff, blgz, gzfz", m_jbzmbh);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    nodes.Add(new Node
                    {
                        ID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["blgz"].ToString()),
                        Name = "条件" + numbertochinese(dr["blgz"].ToString()) + "（规则：本条件中任" + numbertochinese(dr["gzfz"].ToString()) + "组成立）",
                        ParentID = Convert.ToInt64(m_jbzmbh + dr["ff"])
                    });
                }
                dr.Close();
                conn.Close();

                // 三级树写入【组别】（tjzb）
                sql = String.Format("select ff, blgz, tjzb, znfz from t_rule_jbzm where jbzmbh = '{0}' group by ff, blgz, tjzb, znfz ", m_jbzmbh);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    nodes.Add(new Node
                    {
                        ID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString()),
                        Name = "组别" + numbertochinese(dr["tjzb"].ToString()) + "（规则：该组内任" + numbertochinese(dr["znfz"].ToString()) + "症成立）",
                        ParentID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["blgz"].ToString()),
                    });
                }
                dr.Close();
                conn.Close();

                // 四级树写入【病名】（四种病名）
                // 先取出条件类型并存入数组
                //【tjlx】0：症象 1：基本病机 2：复合病机 3：病名
                string[] m_tjlx = new string[5] { "", "", "", "", "" };
                int i = 0;
                sql = String.Format("select distinct tjlx from t_rule_jbzm where jbzmbh = '{0}'", m_jbzmbh);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    m_tjlx[i] = dr["tjlx"].ToString();
                    i++;
                }
                dr.Close();
                conn.Close();

                for (i = 0; m_tjlx[i] != ""; i++)
                {
                    switch (m_tjlx[i])
                    {
                        case "0": //【症象】
                            {
                                sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_jbzm as t2 on t2.zxbh = t1.zxbh where jbzmbh = '{0}' group by t2.ff, t2.blgz, t2.tjzb, t2.zxbh", m_jbzmbh);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                dr = comm.ExecuteReader();
                                while (dr.Read())
                                {
                                    nodes.Add(new Node
                                    {
                                        ID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["zxbh"].ToString()),
                                        Name = dr["zxbh"].ToString() + "  " + dr[4].ToString() + "【症象】",
                                        ParentID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                                    });
                                }
                                dr.Close();
                                conn.Close();
                            }
                            break;
                        case "1": //【基本病机】
                            {
                                sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh, min(t1.jbbjmc) from t_info_jbbj as t1 inner join t_rule_jbzm as t2 on t2.jbbjbh = t1.jbbjbh where jbzmbh = '{0}' group by t2.ff, t2.blgz, t2.tjzb, t2.jbbjbh", m_jbzmbh);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                dr = comm.ExecuteReader();
                                while (dr.Read())
                                {
                                    nodes.Add(new Node
                                    {
                                        ID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["jbbjbh"].ToString()),
                                        Name = dr["jbbjbh"].ToString() + "  " + dr[4].ToString() + "【基本病机】",
                                        ParentID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                                    });
                                }
                                dr.Close();
                                conn.Close();
                            }
                            break;
                        case "2": //【复合病机】
                            {
                                sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.fhbjbh, min(t1.fhbjmc) from t_info_fhbj as t1 inner join t_rule_jbzm as t2 on t2.fhbjbh = t1.fhbjbh where jbzmbh = '{0}' group by t2.ff, t2.blgz, t2.tjzb, t2.fhbjbh", m_jbzmbh);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                dr = comm.ExecuteReader();
                                while (dr.Read())
                                {
                                    nodes.Add(new Node
                                    {
                                        ID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["fhbjbh"].ToString()),
                                        Name = dr["fhbjbh"].ToString() + "  " + dr[4].ToString() + "【复合病机】",
                                        ParentID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                                    });
                                }
                                dr.Close();
                                conn.Close();
                            }
                            break;
                        case "3": //【病名】
                            {
                                sql = String.Format("select t2.ff, t2.blgz, t2.tjzb, t2.bmbh, t1.bmlx, min(t1.bmmc) from t_info_bm as t1 inner join t_rule_jbzm as t2 on t2.bmbh = t1.bmbh where jbzmbh = '{0}' group by t2.ff, t2.blgz, t2.tjzb, t2.bmbh, t1.bmlx", m_jbzmbh);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                dr = comm.ExecuteReader();
                                while (dr.Read())
                                {
                                    if (dr["bmlx"].ToString() == "0")
                                    {
                                        nodes.Add(new Node
                                        {
                                            ID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["bmbh"].ToString()),
                                            Name = dr["bmbh"].ToString() + "  " + dr[5].ToString() + "【外感病名】",
                                            ParentID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                                        });
                                    }
                                    if (dr["bmlx"].ToString() == "1")
                                    {
                                        nodes.Add(new Node
                                        {
                                            ID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["bmbh"].ToString()),
                                            Name = dr["bmbh"].ToString() + "  " + dr[5].ToString() + "【内伤病名】",
                                            ParentID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["blgz"].ToString() + dr["tjzb"].ToString())
                                        });
                                    }
                                }
                                dr.Close();
                                conn.Close();
                            }
                            break;
                    }
                }

                // 五级树写入【同一编号不同名称】
                m_tjlx = new string[5] { "", "", "", "", "" };
                i = 0;
                sql = String.Format("select distinct tjlx from t_rule_jbzm where jbzmbh = '{0}'", m_jbzmbh);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    m_tjlx[i] = dr["tjlx"].ToString();
                    i++;
                }
                dr.Close();
                conn.Close();

                for (i = 0; m_tjlx[i] != ""; i++)
                {
                    switch (m_tjlx[i])
                    {
                        case "0": //【症象】
                            {
                                sql = String.Format("select t1.id, t2.ff, t2.blgz, t2.tjzb, t2.zxbh, min(t1.zxmc) from t_info_zxmx as t1 inner join t_rule_jbzm as t2 on t2.zxbh = t1.zxbh where jbzmbh = '{0}' group by t1.id, t2.ff, t2.blgz, t2.tjzb, t2.zxbh"
                                    , m_jbzmbh);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                dr = comm.ExecuteReader();
                                while (dr.Read())
                                {
                                    nodes.Add(new Node
                                    {
                                        ID = Convert.ToInt64(dr["ff"].ToString() + dr["id"].ToString()),
                                        Name = dr["zxbh"].ToString() + "  " + dr[5].ToString(),
                                        ParentID = Convert.ToInt64(m_jbzmbh + dr["ff"].ToString() + dr["zxbh"].ToString())
                                    });
                                }
                                dr.Close();
                                conn.Close();
                            }
                            break;
                        case "1": //【基本病机】
                            {
                                // 同一编号对应唯一名称，不需任何添加
                            }
                            break;
                        case "2": //【复合病机】
                            {
                                // 同一编号对应唯一名称，不需任何添加
                            }
                            break;
                        case "3": //【病名】
                            {
                                // 同一编号对应唯一名称，不需任何添加
                            }
                            break;
                    }
                }
                //// 调用创建树函数
                //BuildENTree();
            }

            /**
             * 目录三：该证名所对应的治法
             **/
            // 说明：该目录下本只有一级树，因只有一级树是无法显示的（目前暂未解决），故分为两级树进行显示
            // 一级树
            sql = String.Format("select zf from t_info_jbzm where jbzmbh = '{0}'", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64("101" + m_jbzmbh),  // ID 取 101 + m_jbzmbh
                    Name = "该证名所对应的治法为： " + dr["zf"].ToString()
                });
            }
            dr.Close();
            conn.Close();
            // 二级树
            sql = String.Format("select zf from t_info_jbzm where jbzmbh = '{0}'", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64( "102" + m_jbzmbh),  // ID 取 102 + m_jbzmbh
                    Name = dr["zf"].ToString(),
                    ParentID = Convert.ToInt64("101" + m_jbzmbh)
                });
            }
            dr.Close();
            conn.Close();

            /**
             * 目录四：该证名所对应的处方
             **/
            // 一级树【处方】
            sql = String.Format("select t1.jbcfmc from t_info_jbcfxx t1 inner join t_rule_dzjbcf t2 on t1.jbcfbh = t2.cfbh  where t2.zmbh = '{0}'", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64("103" + m_jbzmbh),  // ID 取 103 + m_jbzmbh
                    Name = "该证名所对应的处方为： " + dr["jbcfmc"].ToString()
                });
            }
            dr.Close();
            conn.Close();

            // 二级树【处方的组成】+【处方所对应的修改方法】（仅显示标题）
            //【处方的组成】
            nodes.Add(new Node
            {
                ID = Convert.ToInt64("104" + m_jbzmbh),  // ID 取 104 + m_jbzmbh
                Name = "该处方的组成",
                ParentID = Convert.ToInt64("103" + m_jbzmbh)
            });
            //【处方所对应的修改方法】
            nodes.Add(new Node
            {
                ID = Convert.ToInt64("105" + m_jbzmbh),  // ID 取 105 + m_jbzmbh
                Name = "该处方所对应的修改方法",
                ParentID = Convert.ToInt64("103" + m_jbzmbh)
            });

            // 三级树【具体处方组成】+【具体处方所对应的修改方法】（显示具体内容）
            // 【具体处方组成】
            sql = String.Format("select t2.cfbh, t2.ywbh, t1.ywmc, t2.ywjl, t2.dw from ( t_info_yw t1 inner join t_info_cfzc t2 on t2.ywbh = t1.ywbh ) inner join t_rule_dzjbcf t3 on t3.cfbh = t2.cfbh where t3.zmbh = '{0}'", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64(dr["cfbh"].ToString() + dr["ywbh"].ToString()), // 处方编号+药物编号
                    Name = dr["ywmc"].ToString() + " " + string.Format("{0:0}",Convert.ToDecimal(dr["ywjl"].ToString())) + dr["dw"].ToString(), // 药物剂量只取整数
                    ParentID = Convert.ToInt64("104" + m_jbzmbh)
                });
            }
            dr.Close();
            conn.Close();
            // 【具体处方所对应的修改方法】
            sql = String.Format("select distinct t1.ff from t_rule_xgcfgz as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id where t2.zmbh = '{0}' order by t1.ff", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64("105" + m_jbzmbh + dr["ff"].ToString()),
                    Name = "修改方法" + numbertochinese(dr["ff"].ToString()),
                    ParentID = Convert.ToInt64("105" + m_jbzmbh)
                });
            }
            dr.Close();
            conn.Close();

            // 四级树【修改条件】+【替换】
            //【修改条件】
            sql = String.Format("select distinct t1.ff, t1.gzfz from t_rule_xgcfgz as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id where t2.zmbh = '{0}' order by t1.ff", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64("105" + m_jbzmbh + dr["ff"].ToString() + dr["gzfz"].ToString()),
                    Name = "修改条件：当以下条件中成立的条件的条件权值之和大于等于 " + dr["gzfz"].ToString() + " 时，本方法成立",
                    ParentID = Convert.ToInt64("105" + m_jbzmbh + dr["ff"].ToString())
                });
            }
            dr.Close();
            conn.Close();
            //【替换】
            sql = String.Format("select t1.ff, xgfs, jbcfbh, jbcfmc from (t_rule_xgcf as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id)"
                                    + "inner join t_info_jbcfxx as t3 on t3.jbcfbh = t1.xgcfbh where t2.zmbh = '{0}'", m_jbzmbh);
            string xgfs = "";
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                if (dr["xgfs"].ToString() == "=")
                    xgfs = "替换";
                else
                    xgfs = "添加";
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64("105" + m_jbzmbh + dr["ff"].ToString() + "1"),  // 这种设置ID的前提条件是一种修改方法只对应一种替换，否则无效
                    Name = xgfs + "为：" + dr["jbcfmc"].ToString(),
                    ParentID = Convert.ToInt64("105" + m_jbzmbh + dr["ff"].ToString())
                });
            }
            dr.Close();
            conn.Close();

            // 五级树【修改条件】子目录 +【替换】子目录
            //【修改条件】子目录
            // 用于存储条件编号和其他编号
            string[][] tjbh = new string[15][];
            tjbh[0] = new string[6] { "", "", "", "", "", "" };
            tjbh[1] = new string[6] { "", "", "", "", "", "" };
            tjbh[2] = new string[6] { "", "", "", "", "", "" };
            tjbh[3] = new string[6] { "", "", "", "", "", "" };
            tjbh[4] = new string[6] { "", "", "", "", "", "" };
            tjbh[5] = new string[6] { "", "", "", "", "", "" };
            tjbh[6] = new string[6] { "", "", "", "", "", "" };
            tjbh[7] = new string[6] { "", "", "", "", "", "" };
            tjbh[8] = new string[6] { "", "", "", "", "", "" };
            tjbh[9] = new string[6] { "", "", "", "", "", "" };
            tjbh[10] = new string[6] { "", "", "", "", "", "" };
            tjbh[11] = new string[6] { "", "", "", "", "", "" };
            tjbh[12] = new string[6] { "", "", "", "", "", "" };
            tjbh[13] = new string[6] { "", "", "", "", "", "" };
            tjbh[14] = new string[6] { "", "", "", "", "", "" };
            // 用于存储条件名称
            string[] tjmc = new string[15] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            int m = 0;
            int n = 0;
            // 读取条件编号
            sql = String.Format("select t1.ff, tjbh = (case when zxbh is not null then zxbh when jbbjbh is not null then jbbjbh when fhbjbh is not null then fhbjbh when bmbh is not null then bmbh when jbzmbh is not null then jbzmbh end),"
            + "zxbh, jbbjbh, fhbjbh, bmbh, jbzmbh from t_rule_xgcfgz as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id where t2.zmbh = '{0}' order by t1.ff", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                tjbh[m][0] = dr["tjbh"].ToString();
                tjbh[m][1] = dr["zxbh"].ToString();
                tjbh[m][2] = dr["jbbjbh"].ToString();
                tjbh[m][3] = dr["fhbjbh"].ToString();
                tjbh[m][4] = dr["bmbh"].ToString();
                tjbh[m][5] = dr["jbzmbh"].ToString();
                m++;
            }
            dr.Close();
            conn.Close();
            m = 0;
            // 根据条件编号读取条件名称
            while (tjbh[m][0] != "")
            {
                string temp_tjlx = "";
                if (tjbh[m][1] != "")
                {
                    sql = String.Format("select zxmc from t_info_zxmx where zxbh = '{0}'", tjbh[m][1]);
                    temp_tjlx = "1";
                }
                if (tjbh[m][2] != "")
                {
                    sql = String.Format("select jbbjmc from t_info_jbbj where jbbjbh = '{0}'", tjbh[m][2]);
                    temp_tjlx = "2";
                }
                if (tjbh[m][3] != "")
                {
                    sql = String.Format("select fhbjmc from t_info_fhbj where fhbjbh = '{0}'", tjbh[m][3]);
                    temp_tjlx = "3";
                }
                if (tjbh[m][4] != "")
                {
                    sql = String.Format("select bmmc from t_info_bm where bmbh = '{0}'", tjbh[m][4]);
                    temp_tjlx = "4";
                }
                if (tjbh[m][5] != "")
                {
                    sql = String.Format("select jbzmmc from t_info_jbzm where jbzmbh = '{0}'", tjbh[m][5]);
                    temp_tjlx = "5";
                }
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    switch (temp_tjlx)
                    { 
                        case "1":
                            tjmc[n] = dr[0].ToString() + "【症象】";
                            break;
                        case "2":
                            tjmc[n] = dr[0].ToString() + "【基本病机】";
                            break;
                        case "3":
                            tjmc[n] = dr[0].ToString() + "【复合病机】";
                            break;
                        case "4":
                            tjmc[n] = dr[0].ToString() + "【病名】";
                            break;
                        case "5":
                            tjmc[n] = dr[0].ToString() + "【基本证名】";
                            break;
                    }                
                    //j++;
                }
                dr.Close();
                conn.Close();
                m++;
                n++; // 没有写在while里，防止同一编号的不同名称重复写入
            }
            sql = String.Format("select t1.ff, tjbh = (case when zxbh is not null then zxbh when jbbjbh is not null then jbbjbh when fhbjbh is not null then fhbjbh when bmbh is not null then bmbh when jbzmbh is not null then jbzmbh end),"
                       + "znfz, gzfz from t_rule_xgcfgz as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id where t2.zmbh = '{0}' order by t1.ff", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            n = 0;
            while (dr.Read())
            {
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64("105" + m_jbzmbh + dr["ff"].ToString() + dr["tjbh"].ToString()),  // 这种设置ID的前提条件是一种修改方法只对应一种替换，否则无效
                    Name = dr["tjbh"].ToString() + " " + tjmc[n] + " （条件权值：" + dr["znfz"].ToString() + "）",
                    ParentID = Convert.ToInt64("105" + m_jbzmbh + dr["ff"].ToString() + dr["gzfz"].ToString())
                });
                n++;
            }
            dr.Close();
            conn.Close();
            //【替换】药物具体处方组成
            sql = String.Format("select distinct t1.ff, jbcfbh, jbcfmc, t4.ywbh, t5.ywmc, t4.ywjl, t4.dw from (((([zjxtserver].[dbo].[t_rule_xgcf] as t1 inner join [zjxtserver].[dbo].[t_rule_dzjbcf] as t2 on t1.id = t2.id)"
                                +"inner join [zjxtserver].[dbo].[t_info_jbcfxx] as t3 on t3.jbcfbh = t1.xgcfbh) inner join "
                                +"[zjxtserver].[dbo].[t_info_cfzc] as t4 on t4.cfbh = t1.xgcfbh)) "
                                +"inner join [zjxtserver].[dbo].[t_info_yw] as t5 on t4.ywbh = t5.ywbh where t2.zmbh  = '{0}'", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64(dr["ff"].ToString() + dr["jbcfbh"].ToString() + dr["ywbh"].ToString()),
                    Name = dr["ywmc"].ToString() + " " + string.Format("{0:0}", Convert.ToDecimal(dr["ywjl"].ToString())) + dr["dw"].ToString(), // 药物剂量只取整数
                    ParentID = Convert.ToInt64("105" + m_jbzmbh + dr["ff"].ToString() + "1")
                });
            }
            dr.Close();
            conn.Close();

            /**
             * 目录五：该处方所对应的服法
             **/
            // 说明：该目录下本只有一级树，因只有一级树是无法显示的（目前暂未解决），故分为两级树进行显示
            // 一级树
            sql = String.Format("select t1.ff, t1.jbcfbh from t_info_jbcfxx t1 inner join t_rule_dzjbcf t2 on t1.jbcfbh = t2.cfbh where t2.zmbh = '{0}'", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64("501" + m_jbzmbh),  // ID 取 501 + m_jbzmbh
                    Name = "该处方所对应的服法为： " + dr["ff"].ToString()
                });
            }
            dr.Close();
            conn.Close();           
            // 二级树
            sql = String.Format("select t1.ff, t1.jbcfbh from t_info_jbcfxx t1 inner join t_rule_dzjbcf t2 on t1.jbcfbh = t2.cfbh where t2.zmbh = '{0}'", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64("502" + m_jbzmbh + dr["jbcfbh"].ToString()), 
                    Name = dr["ff"].ToString(),
                    ParentID = Convert.ToInt64("501" + m_jbzmbh)
                });
            }
            dr.Close();
            conn.Close();

            /**
             * 目录六：该证名所对应的药物修改方法
             **/
            // 一级树【该证名所对应的药物修改方法】
            nodes.Add(new Node
            {
                ID = Convert.ToInt64("601" + m_jbzmbh),  // ID 取 501 + m_jbzmbh
                Name = "该证名所对应的药物修改方法" 
            });
            // 二级树【修改方法】
            sql = String.Format("select distinct t1.tjzb from t_rule_xgywgz as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id where t2.zmbh = '{0}' order by t1.tjzb", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64("602" + m_jbzmbh + dr["tjzb"].ToString()),
                    Name = "修改方法" + numbertochinese(dr["tjzb"].ToString()),
                    ParentID = Convert.ToInt64("601" + m_jbzmbh)
                });
            }
            dr.Close();
            conn.Close();
            // 三级树【修改条件】+【添加】/【删除】
            //【修改条件】
            sql = String.Format("select distinct t1.ff, t1.gzfz, t1.tjzb from t_rule_xgywgz as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id where t2.zmbh = '{0}' order by t1.ff", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64("603" + m_jbzmbh + dr["gzfz"].ToString() + dr["tjzb"].ToString()),
                    Name = "修改条件：当以下条件中成立的条件的条件权值之和大于等于 " + dr["gzfz"].ToString() + " 时，本方法成立",
                    ParentID = Convert.ToInt64("602" + m_jbzmbh + dr["tjzb"].ToString())
                });
            }
            dr.Close();
            conn.Close();
            //【添加】/【删除】
            sql = String.Format("select t1.ff, xgfs, ywmc, t3.ywbh, ywjl, dw from (t_rule_xgyw as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id)"
                                   + "inner join t_info_yw as t3 on t3.ywbh = t1.ywbh where t2.zmbh = '{0}' order by t1.ff", m_jbzmbh);
            xgfs = "";
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                if (dr["xgfs"].ToString() == "-")
                    xgfs = "去掉";
                else
                    xgfs = "增加";
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64("603" + dr["ff"].ToString() + dr["ywbh"].ToString()),  // 这种设置ID的前提条件是一种修改方法只对应一种替换，否则无效
                    Name = xgfs + "：" + dr["ywmc"].ToString() + " " + string.Format("{0:0}", Convert.ToDecimal(dr["ywjl"].ToString())) + dr["dw"].ToString(),
                    ParentID = Convert.ToInt64("602" + m_jbzmbh + dr["ff"].ToString())
                });
            }
            dr.Close();
            conn.Close();
            // 四级树【修改条件】下的具体内容
            string[][] tjbh_1 = new string[15][];
            tjbh[0] = new string[6] { "", "", "", "", "", "" };
            tjbh[1] = new string[6] { "", "", "", "", "", "" };
            tjbh[2] = new string[6] { "", "", "", "", "", "" };
            tjbh[3] = new string[6] { "", "", "", "", "", "" };
            tjbh[4] = new string[6] { "", "", "", "", "", "" };
            tjbh[5] = new string[6] { "", "", "", "", "", "" };
            tjbh[6] = new string[6] { "", "", "", "", "", "" };
            tjbh[7] = new string[6] { "", "", "", "", "", "" };
            tjbh[8] = new string[6] { "", "", "", "", "", "" };
            tjbh[9] = new string[6] { "", "", "", "", "", "" };
            tjbh[10] = new string[6] { "", "", "", "", "", "" };
            tjbh[11] = new string[6] { "", "", "", "", "", "" };
            tjbh[12] = new string[6] { "", "", "", "", "", "" };
            tjbh[13] = new string[6] { "", "", "", "", "", "" };
            tjbh[14] = new string[6] { "", "", "", "", "", "" };
            // 用于存储条件名称
            string[] tjmc_1 = new string[15] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            int m_1 = 0;
            int n_1 = 0;
            // 读取条件编号
            sql = String.Format("select t1.tjzb, tjbh = (case when zxbh is not null then zxbh when jbbjbh is not null then jbbjbh when fhbjbh is not null then fhbjbh when bmbh is not null then bmbh when jbzmbh is not null then jbzmbh end),"
            + "zxbh, jbbjbh, fhbjbh, bmbh, jbzmbh from t_rule_xgywgz as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id where t2.zmbh = '{0}' order by t1.tjzb", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                tjbh[m_1][0] = dr["tjbh"].ToString();
                tjbh[m_1][1] = dr["zxbh"].ToString();
                tjbh[m_1][2] = dr["jbbjbh"].ToString();
                tjbh[m_1][3] = dr["fhbjbh"].ToString();
                tjbh[m_1][4] = dr["bmbh"].ToString();
                tjbh[m_1][5] = dr["jbzmbh"].ToString();
                m_1++;
            }
            dr.Close();
            conn.Close();
            m_1 = 0;
            // 根据条件编号读取条件名称
            while (tjbh[m_1][0] != "")
            {
                string temp_tjlx = "";
                if (tjbh[m_1][1] != "")
                {
                    sql = String.Format("select zxmc from t_info_zxmx where zxbh = '{0}'", tjbh[m_1][1]);
                    temp_tjlx = "1"; // 症象
                }
                if (tjbh[m_1][2] != "")
                {
                    sql = String.Format("select jbbjmc from t_info_jbbj where jbbjbh = '{0}'", tjbh[m_1][2]);
                    temp_tjlx = "2"; // 基本病机
                }
                if (tjbh[m_1][3] != "")
                {
                    sql = String.Format("select fhbjmc from t_info_fhbj where fhbjbh = '{0}'", tjbh[m_1][3]);
                    temp_tjlx = "3"; // 复合病机
                }
                if (tjbh[m_1][4] != "")
                {
                    sql = String.Format("select bmmc from t_info_bm where bmbh = '{0}'", tjbh[m_1][4]);
                    temp_tjlx = "4"; // 病名
                }
                if (tjbh[m_1][5] != "")
                {
                    sql = String.Format("select jbzmmc from t_info_jbzm where jbzmbh = '{0}'", tjbh[m_1][5]);
                    temp_tjlx = "5"; // 基本证名
                }
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    switch (temp_tjlx)
                    {
                        case "1":
                            tjmc[n_1] = dr[0].ToString() + "【症象】";
                            break;
                        case "2":
                            tjmc[n_1] = dr[0].ToString() + "【基本病机】";
                            break;
                        case "3":
                            tjmc[n_1] = dr[0].ToString() + "【复合病机】";
                            break;
                        case "4":
                            tjmc[n_1] = dr[0].ToString() + "【病名】";
                            break;
                        case "5":
                            tjmc[n_1] = dr[0].ToString() + "【基本证名】";
                            break;
                    }                  
                    //j++;
                }
                dr.Close();
                conn.Close();
                m_1++;
                n_1++; // 没有写在while里，防止同一编号的不同名称重复写入
            }
            // 添加到 ListView 
            sql = String.Format("select t1.tjzb, tjbh = (case when zxbh is not null then zxbh when jbbjbh is not null then jbbjbh when fhbjbh is not null then fhbjbh when bmbh is not null then bmbh when jbzmbh is not null then jbzmbh end),"
                    + "znfz, gzfz from t_rule_xgywgz as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id where t2.zmbh = '{0}' order by t1.tjzb", m_jbzmbh);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            n_1 = 0;
            while (dr.Read())
            {
                nodes.Add(new Node
                {
                    ID = Convert.ToInt64("604" + dr["tjzb"].ToString() + dr["tjbh"].ToString()), 
                    Name = dr["tjbh"].ToString() + " " + tjmc[n_1] + " （条件权值：" + dr["znfz"].ToString() + "）",
                    ParentID = Convert.ToInt64("603" + m_jbzmbh + dr["gzfz"].ToString() + dr["tjzb"].ToString())
                });
                n_1++;
            }
            dr.Close();
            conn.Close();

            // 调用创建树函数
            BuildENTree();
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
    }
}