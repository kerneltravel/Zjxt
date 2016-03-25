using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

namespace 中医证治智能系统
{
    /// <summary>
    /// Interaction logic for display_fywcl.xaml
    /// </summary>
    public partial class display_fywcl : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        int vexsCount = 0; // 药物个数
        VertexNode[] vertexNodeList; // 顶点数组
        EdgeNode edgeNode; 
        string number; // 病历编号

        /// <summary>
        /// 药物结点
        /// </summary>
        public class VertexNode
        {
            public string vertexNumber { get; set; } // 药物编号
            public string vertexName { get; set; } // 药物名称
            public EdgeNode firstedge { get; set; } // 药物结点头指针

        }
        
        /// <summary>
        /// 反药物结点
        /// </summary>
        public class EdgeNode {
            public string adjvexNumber { get; set; } // 反药物编号
            public string adjvexName { get; set; } // 反药物名称
            public EdgeNode next { get; set; } // 指向下一个反药物结点
        }

        public display_fywcl(string number)
        {
            InitializeComponent();
            this.number = number;

            initNode(); // 初始化结点
            selectTip(); // 选择提示
        }

        /// <summary>
        /// 功能：初始化结点
        /// </summary>
        private void initNode()
        {
            vexsCount = getCount(); // 药物顶点个数

            // 初始化药物顶点，建立顶点表
            vertexNodeList = new VertexNode[vexsCount];
            string sql = "";
            int i = 0;
            sql = String.Format("SELECT distinct (b.[ywbh]), b.ywmc FROM temp_fywcl a ,t_info_yw b where a.ywbh = b.ywbh");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                vertexNodeList[i] = new VertexNode();
                vertexNodeList[i].vertexNumber = dr["ywbh"].ToString(); // 药物编号
                vertexNodeList[i].vertexName = dr["ywmc"].ToString(); // 药物名称
                vertexNodeList[i].firstedge = null;
                i++;
            }
            dr.Close();
            conn.Close();

            // 初始化反药物结点，建立邻接表
            sql = String.Format("SELECT a.hfywbh, a.ywbh, b.ywmc FROM temp_fywcl a ,t_info_yw b where a.hfywbh = b.ywbh");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                EdgeNode edgeNode_1 = new EdgeNode();

                edgeNode_1.adjvexNumber = dr["hfywbh"].ToString();
                edgeNode_1.adjvexName = dr["ywmc"].ToString();
                edgeNode_1.next = vertexNodeList[getPosition(dr["ywbh"].ToString(), vertexNodeList)].firstedge;
                vertexNodeList[getPosition(dr["ywbh"].ToString(), vertexNodeList)].firstedge = edgeNode_1;
            }
            dr.Close();
            conn.Close();
        }

        /// <summary>
        /// 功能：返回药物结点在vertexNodeList中的位置
        /// </summary>
        /// <param name="ywbh"></param>
        /// <param name="vertexNodeList"></param>
        /// <returns></returns>
        public int getPosition(string ywbh, VertexNode[] vertexNodeList)
        {
            for (int i = 0; i < vexsCount; i++)
            {
                if (vertexNodeList[i].vertexNumber == ywbh)
                {
                    return i;
                }
            }
                return -1;
        }

        /**
         *  功能：选择提示
         */
        public void selectTip() {
            string sql = "";
            string tipStr; // 提示信息
            string ywStr; // 药物名称
            string fywStr; // 反药物名称
            for (int i = 0; i < vexsCount; i++) {
                tipStr = "提示：选择1,请点击【是】；选择2，请点击【否】";
                ywStr = "";
                fywStr = "";
                ywStr = "1、"+vertexNodeList[i].vertexName;
                if (vertexNodeList[i].firstedge != null)
                {
                    EdgeNode mEdgeNode = new EdgeNode();
                    mEdgeNode = vertexNodeList[i].firstedge;
                    fywStr = "2、"+mEdgeNode.adjvexName;
                    while (mEdgeNode.next != null)
                    {
                        mEdgeNode = mEdgeNode.next;
                        fywStr += mEdgeNode.adjvexName + '\t';
                    }
                }
                if (MessageBox.Show(tipStr + '\n' + ywStr + '\n' + fywStr, "互反药物选择", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    // 选择反药物，删除药物
                    sql = String.Format("delete	t_zd_ywzc from t_zd_ywzc a, temp_fywcl b where a.id = '{0}' and a.ywbh = b.ywbh", number);
                }
                else
                {
                    // 选择药物，删除反药物
                    sql = String.Format("delete	t_zd_ywzc from t_zd_ywzc a, temp_fywcl b where a.id = '{0}' and a.ywbh = b.hfywbh", number);
                }
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                int count = comm.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// 功能：取得药物种类个数
        /// </summary>
        /// <returns></returns>
        private int getCount(){
            int count = 0;
            string sql;
            sql = String.Format("SELECT COUNT(distinct [ywbh] ) FROM temp_fywcl");  
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                count = Convert.ToInt16(dr[0].ToString());
            }
            dr.Close();
            conn.Close();
            return count;
        }
    }
}
