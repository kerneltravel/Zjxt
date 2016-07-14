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
    /// Interaction logic for display_lczd.xaml
    /// </summary>
    public partial class display_bl : Window
    {
        // 定义连接字符串
        static public string connString1 = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn1 = new SqlConnection(connString1);
        //显示诊断结果
        string diagnosisresult1 = "";

        string chuqizhengxiang1 = "";
        string bingming1 = "";
        string bingming2 = "";
        string jsd1 = "";
        string ID1 = "";
        string ID2 = "";
        public display_bl(string id,string old)
        {
            InitializeComponent();
            ID1 = id;
            ID2 = old;

            // 病例编号
            string sql = String.Format("select blbh from t_bl where id ='{0}' ", ID1);
            conn1.Open();
            SqlCommand comm = new SqlCommand(sql, conn1);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 = "病历编号:" + dr[0].ToString() + "\n";
            }
            dr.Close();
            conn1.Close();
            diagnosisresult1 += "----------------------------------------------------------------------------------" + "\n";
            
            // 检索信息
            sql = String.Format("select jsxx from t_bl where id ='{0}' ", ID1);
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 = diagnosisresult1 + "检索信息:" + dr[0].ToString() + "\n";
                title.Text = dr[0].ToString();
            }
            dr.Close();
            conn1.Close();
            diagnosisresult1 += "----------------------------------------------------------------------------------" + "\n";
            
            // 主诉
            sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '6'", ID1);  
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 = diagnosisresult1 + "主诉：" + dr["xxbh"].ToString().Trim();
            }
            dr.Close();
            conn1.Close();
            
            // 主诉时间
            sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '9'", ID1);  
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 = diagnosisresult1 + "(" + dr["xxbh"].ToString() + ")";
            }
            dr.Close();
            conn1.Close();
            
            // 现病史：
            // 初起症 + 现症
            diagnosisresult1 += "\n" + "现病史：";
            sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '7'", ID1);  // 初起症象
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                chuqizhengxiang1 = dr["xxbh"].ToString() + "  ";
            }
            dr.Close();
            conn1.Close();
            if (chuqizhengxiang1 == "")
            {
                sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '8'", ID1);  // 现症象
                conn1.Open();
                comm = new SqlCommand(sql, conn1);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    diagnosisresult1 = diagnosisresult1 + dr["xxbh"].ToString() + "  ";
                }
                dr.Close();
                conn1.Close();
            }
            else
            {
                diagnosisresult1 = diagnosisresult1 + "    " + "\n" + "  " + "初起症：" + chuqizhengxiang1 + "    " + "\n" + "  "  + "现症：";
                sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '8'", ID1);  //现症象
                conn1.Open();
                comm = new SqlCommand(sql, conn1);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    diagnosisresult1 = diagnosisresult1 + dr["xxbh"].ToString() + "  ";
                }
                dr.Close();
                conn1.Close();
            }

            // 西医诊断：西医病名名称（若无，则默认不显示）
            String xyzd = "";
            Boolean is_xyzd = false;
            sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = 'b'", ID1); 
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                xyzd = xyzd + dr["xxbh"].ToString() + "  ";
                is_xyzd = true;
            }
            if (is_xyzd) // 存在西医诊断
            {
                diagnosisresult1 = diagnosisresult1 + "\n" + "西医诊断：" + xyzd;
                is_xyzd = false;
            } 
            dr.Close();
            conn1.Close();

            // 既往史：既往史名称
            diagnosisresult1 = diagnosisresult1 + "\n" + "既往史：";
            sql = String.Format("select * from t_bl_mx  where id = '{0}' and xxdllx = '0' and xxxllx = 'a'", ID1);  
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 = diagnosisresult1 + dr["xxbh"].ToString() + "  ";
            }
            dr.Close();
            conn1.Close();

            // 诊断结论:
            //    1、病名：中医病名名称+西医病名名称
            //    2、证名：
            //    3、治法：
            //    4、主方名：
            //    5、处方：
            //    6、服法：
            //    7、处方二：
            //    8、服法：
            diagnosisresult1 += "\n" +"诊断结论：                                                              ";
            /// 1.病名：
            /// 1.1中医病名名称
            sql = String.Format("select b.bmmc from t_bl_mx a, t_info_bm b where id = '{0}' and a.xxbh = b.bmbh and xxdllx = '2' and xxxllx = '0'", ID1);  
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                bingming1 = dr["bmmc"].ToString()+ "（中医）" + "  ";
            }
            dr.Close();
            conn1.Close();
            diagnosisresult1 += "\n" + "  " + "病名：" + bingming1;
            /// 1.2西医病名名称
            sql = String.Format("select * from t_bl_mx  where id = '{0}' and xxdllx = '0' and xxxllx = 'b'", ID1);  
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                bingming2 = dr["xxbh"].ToString() + "（西医）" + "  ";
            }
            dr.Close();
            conn1.Close();
            diagnosisresult1 += bingming2;
            /// 2.证名名称：
            /// 2.1基本证名名称
            diagnosisresult1 += "\n" + "  " + "证名：";
            sql = String.Format("select distinct(b.jbzmmc) from t_bl_mx a, t_info_jbzm b where id = '{0} ' and a.xxbh = b.jbzmbh and xxdllx = '2' and xxxllx = '2'", ID1);  
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["jbzmmc"].ToString() + "  ";
            }
            dr.Close();
            conn1.Close();
            /// 2.2西医证名名称（根据西医病名编号可得）
            sql = String.Format("select b.xyzmmc from t_bl_mx a, t_rule_xyzz b where id = ' {0}' and a.xxbh = b.xybmbh and xxdllx = '0' and xxxllx = '5'", ID1);  
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["xyzmmc"].ToString() + "  ";
            }
            dr.Close();
            conn1.Close();

            diagnosisresult1 += "\n";

            /// 3.治法：
            /// 3.1中医治法（根据中医基本证名得出对应的治法）
            diagnosisresult1 += "\n" + "  " + "治法：";
            sql = String.Format("select distinct(b.zf) from t_bl_mx a, t_info_jbzm b where id = '{0}' and a.xxbh = b.jbzmbh and xxdllx = '2' and xxxllx = '2'", ID1);  
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["zf"].ToString() + "  ";
            }
            dr.Close();
            conn1.Close();
            /// 3.2西医治法（根据西医病名编号得出西医证名编号，再得出西医治法）
            sql = String.Format("select c.xyzf from t_bl_mx a, t_rule_xyzz b ,t_rule_xycf c where id = '{0}' and a.xxbh = b.xybmbh  and b.xyzmbh = c.xyzmbh and xxdllx = '0' and xxxllx = '5'", ID1); 
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["xyzf"].ToString() + "  ";
            }
            dr.Close();
            conn1.Close();
            /// 4.主方名：
            /// 4.1基本处方名称（根据基本处方编号得出基本处方名称）
            diagnosisresult1 += "\n" + "  " + "主方名：";
            sql = String.Format("select b.jbcfmc from t_bl_mx a, t_info_jbcfxx b where id = '{0}' and a.xxbh = b.jbcfbh and xxdllx ='2' and xxxllx = '4'", ID1);
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["jbcfmc"].ToString() + "  ";
            }
            dr.Close();
            conn1.Close();
            /// 4.2西医处方名称（根据西医病名编号得出西医证名编号，再得出西医处方名称）
            sql = String.Format("select c.xycfmc from t_bl_mx a, t_rule_xyzz b ,t_rule_xycf c where id = '{0}' and a.xxbh = b.xybmbh  and b.xyzmbh = c.xyzmbh and xxdllx = '0' and xxxllx = '5'", ID1);  
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["xycfmc"].ToString() + "  ";
            }
            dr.Close();
            conn1.Close();
            /// 5.处方：（根据病例编号在药物组成表查找）
            diagnosisresult1 += "\n" + "  " + "处方：";
            sql = String.Format("select b.ywmc,a.ywjl,a.dw from t_zd_ywzc a, t_info_yw b where id = '{0}' and a.ywbh = b.ywbh", ID1);
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["ywmc"].ToString() + " " + Convert.ToInt64(dr["ywjl"]).ToString() + dr["dw"].ToString() + "  ";
            }
            dr.Close();
            conn1.Close();
            /// 6.服法：（根据病例编号直接在病例明细表中查找服法，重复的服法只需写一次）
            diagnosisresult1 += "\n" + "  " + "服法：";
            sql = String.Format("select distinct(xxbh) from t_bl_mx a where a.id = '{0}' and a.xxdllx = '2' and a.xxxllx = '5'", ID1);
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["xxbh"].ToString();
            }
            dr.Close();
            conn1.Close();
            /// 7.处方二：（根据西医病名编号查找西医证名编号，然后根据西医证名编号查找西医处方组成以及西医服法）
            /// （默认不显示，若存在则显示）
            String cf2 = "";
            Boolean is_cf2 = false;
            sql = String.Format("select c.xycfmc from t_bl_mx a, t_rule_xyzz b ,t_rule_xycf c where id = '{0}' and a.xxbh = b.xybmbh  and b.xyzmbh = c.xyzmbh and xxdllx = '0' and xxxllx = '5'", ID1); 
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                is_cf2 = true;
                cf2 += dr["xycfmc"].ToString() + "  ";
            }
            if (is_cf2) // 存在处方二
            {
                diagnosisresult1 += "\n" + "  " + "处方二：" + cf2;
                is_cf2 = false;
            }
            dr.Close();
            conn1.Close();

            /// 8.服法：
            /// （默认不显示，若存在则显示）
            String ff = "";
            Boolean is_ff = false;
            sql = String.Format("select c.xyff from t_bl_mx a, t_rule_xyzz b ,t_rule_xycf c where id = '{0}' and a.xxbh = b.xybmbh  and b.xyzmbh = c.xyzmbh and xxdllx = '0' and xxxllx = '5'", ID1);
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                is_ff = true;
                ff += dr["xyff"].ToString() + "  ";
            }
            if (is_ff)
            {
                diagnosisresult1 += "\n" + "  " + "服法：" + ff;
                is_ff = false;
            }
            dr.Close();
            conn1.Close();
            
            // 结尾处
            diagnosisresult1 += "\n" + "\n" + "\n" + "                                                                                                    医师：__________";
            // 显示结果
            jieguo.Text = diagnosisresult1;
            
            //// 更新备注
            //sql = String.Format("update t_bl set bz = ' {0} ' where id = '{1}'", jieguo.Text.ToString(), ID1);
            //conn1.Open();
            //comm = new SqlCommand(sql, conn1);
            //int count1 = comm.ExecuteNonQuery();
            //if (count1 > 0)
            //{
            //    MessageBox.Show("更新备注成功");
            //}
            //conn1.Close();

            sql = String.Format("select zt from t_bl where id = '{0}'", ID1);
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                jsd1 = dr["zt"].ToString();
            }
            dr.Close();
            conn1.Close();
            sql = String.Format("select explain from t_info_jsd where jsd = '{0}'", jsd1);
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                jinsidu.Text += dr["explain"].ToString();
            }
            dr.Close();
            conn1.Close();
        }

        /// <summary>
        /// 功能：返回
        /// </summary>
        private void zuofei_Click(object sender, RoutedEventArgs e)
        {        
            this.Close();
        }

        /// <summary>
        /// 功能：保存修改
        /// </summary>
        private void save_Click(object sender, RoutedEventArgs e)
        {
            String sql = String.Format("update t_bl set bz = '{0}', bllx='{1}' where id = '{2}'", jieguo.Text.ToString(),"1", ID1);
            conn1.Open();
            SqlCommand comm = new SqlCommand(sql, conn1);
            int count1 = comm.ExecuteNonQuery();
            //if (count1 > 0)
            //{
            //    MessageBox.Show("更新备注成功");
            //}
            conn1.Close();
            this.Close();
        }

        /// <summary>
        /// 功能：参考病例
        /// </summary>
        private void cankao_Click(object sender, RoutedEventArgs e)
        {
            if (ID2 == ""){}
            else
            {
                display_bl1 result = new display_bl1(ID2);
                result.Show();
            }
        }

    }
}

