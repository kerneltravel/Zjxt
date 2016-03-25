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
        string jsd1 = "";
        string ID1 = "";
        string ID2 = "";
        public display_bl(string id,string old)
        {
            InitializeComponent();
            ID1 = id;
            ID2 = old;
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
            sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '6'", ID1);  //主诉
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 = diagnosisresult1 + "主诉：" + dr["xxbh"].ToString().Trim();

            }
            dr.Close();
            conn1.Close();
            sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '9'", ID1);  //主诉时间
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 = diagnosisresult1 + "(" + dr["xxbh"].ToString() + ")";

            }
            dr.Close();
            conn1.Close();
            diagnosisresult1 += "\n" + "现病史：";
            sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '7'", ID1);  //初起症象
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                chuqizhengxiang1 = dr["xxbh"].ToString();
            }
            dr.Close();
            conn1.Close();
            if (chuqizhengxiang1 == "")
            {
                sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '8'", ID1);  //现症象
                conn1.Open();
                comm = new SqlCommand(sql, conn1);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    diagnosisresult1 = diagnosisresult1 +"  "+ dr["xxbh"].ToString();
                }
                dr.Close();
                conn1.Close();
            }
            else
            {

                diagnosisresult1 = diagnosisresult1 + "    " + "\n" + "初起症：" + chuqizhengxiang1 + "    " + "现症：";
                sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '8'", ID1);  //现症象
                conn1.Open();
                comm = new SqlCommand(sql, conn1);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    diagnosisresult1 = diagnosisresult1 + dr["xxbh"].ToString();
                }
                dr.Close();
                conn1.Close();
            }
            sql = String.Format("select * from t_bl_mx  where id = '{0}' and xxdllx = '0' and xxxllx = 'b'", ID1);  //西医病名名称
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 = diagnosisresult1 + "\n" + "西医诊断：" + dr["xxbh"].ToString();
            }
            dr.Close();
            conn1.Close();
            sql = String.Format("select * from t_bl_mx  where id = '{0}' and xxdllx = '0' and xxxllx = 'a'", ID1);  //既往史
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 = diagnosisresult1 + "\n" + "既往史：" + dr["xxbh"].ToString() + "\n";
            }
            dr.Close();
            conn1.Close();
            diagnosisresult1 += "\n" +"诊断结论：                                                              ";
            sql = String.Format("select b.bmmc from t_bl_mx a, t_info_bm b where id = '{0}' and a.xxbh = b.bmbh and xxdllx = '2' and xxxllx = '0'", ID1);  //病名编号
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                bingming1 = dr["bmmc"].ToString();
            }
            dr.Close();
            conn1.Close();
            diagnosisresult1 += "\n" + "病名：" + bingming1 + "（中医）";
            sql = String.Format("select * from t_bl_mx  where id = '{0}' and xxdllx = '0' and xxxllx = 'b'", ID1);  //西医病名名称
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                bingming1 = dr["xxbh"].ToString();
            }
            dr.Close();
            conn1.Close();
            diagnosisresult1 += bingming1 + "（西医）";
            diagnosisresult1 += "\n" + "证名：";
            sql = String.Format("select b.jbzmmc from t_bl_mx a, t_info_jbzm b where id = '{0} ' and a.xxbh = b.jbzmbh and xxdllx = '2' and xxxllx = '2'", ID1);  //证名编号
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["jbzmmc"].ToString();
            }
            dr.Close();
            conn1.Close();
            sql = String.Format("select b.xyzmmc from t_bl_mx a, t_rule_xyzz b where id = ' {0}' and a.xxbh = b.xybmbh and xxdllx = '0' and xxxllx = '5'", ID1);  //西医证名编号
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["xyzmmc"].ToString();
            }
            dr.Close();
            conn1.Close();
            diagnosisresult1 += "\n" + "治法：";
            sql = String.Format("select b.zf from t_bl_mx a, t_info_jbzm b where id = '{0}' and a.xxbh = b.jbzmbh and xxdllx = '2' and xxxllx = '2'", ID1);  //中医治法
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["zf"].ToString();
            }
            dr.Close();
            conn1.Close();
            sql = String.Format("select c.xyzf from t_bl_mx a, t_rule_xyzz b ,t_rule_xycf c where id = '{0}' and a.xxbh = b.xybmbh  and b.xyzmbh = c.xyzmbh and xxdllx = '0' and xxxllx = '5'", ID1);  //西医治法
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["xyzf"].ToString();
            }
            dr.Close();
            conn1.Close();
            diagnosisresult1 += "\n" + "主方名：";
            sql = String.Format("select b.jbcfmc from t_bl_mx a, t_info_jbcfxx b where id = '{0}' and a.xxbh = b.jbcfbh and xxdllx ='2' and xxxllx = '4'", ID1);  //处方编号
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["jbcfmc"].ToString();
            }
            dr.Close();
            conn1.Close();
            sql = String.Format("select c.xycfmc from t_bl_mx a, t_rule_xyzz b ,t_rule_xycf c where id = '{0}' and a.xxbh = b.xybmbh  and b.xyzmbh = c.xyzmbh and xxdllx = '0' and xxxllx = '5'", ID1);  //西医处方编号("select b.jbcfmc from t_bl_mx a, t_info_jbcfxx b where id = '{0}' and a.xxbh = b.jbcfbh and xxdllx ='2' and xxxllx = '4'", id);  //处方编号
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["xycfmc"].ToString();
            }
            dr.Close();
            conn1.Close();
            diagnosisresult1 += "\n" + "处方：";
            sql = String.Format("select b.ywmc,a.ywjl,a.dw from t_zd_ywzc a, t_info_yw b where id = '{0}' and a.ywbh = b.ywbh", ID1);
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["ywmc"].ToString() + " " + Convert.ToInt64(dr["ywjl"]).ToString() + dr["dw"].ToString()+"  ";
            }
            dr.Close();
            conn1.Close();
            diagnosisresult1 += "\n" + "服法：";
            sql = String.Format("select xxbh from  t_bl_mx a where a.id = '{0}' and a.xxdllx = '2' and a.xxxllx = '5'", ID1);
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult1 += dr["xxbh"].ToString();
            }
            dr.Close();
            conn1.Close();
            sql = String.Format("select c.xycfmc,c.xyff from t_bl_mx a, t_rule_xyzz b ,t_rule_xycf c where id = '{0}' and a.xxbh = b.xybmbh  and b.xyzmbh = c.xyzmbh and xxdllx = '0' and xxxllx = '5'", ID1);  //西医治法
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            dr = comm.ExecuteReader();
            if (dr.Read())
            {
                diagnosisresult1 += " 处方二：";
            }
            while (dr.Read())
            {
                diagnosisresult1 += dr["xycfmc"].ToString();
            }
            if (dr.Read())
            {
                diagnosisresult1 += "\n" + "服法：";
            }
            while (dr.Read())
            {
                diagnosisresult1 += dr["xyff"].ToString();
            }
            dr.Close();
            conn1.Close();
            diagnosisresult1 += "\n" + "                                                                   医师：__________";

            jieguo.Text = diagnosisresult1;
            sql = String.Format("update t_bl set bz = ' {0} ' where id = '{1}'", jieguo.Text.ToString(), ID1);
            conn1.Open();
            comm = new SqlCommand(sql, conn1);
            int count1 = comm.ExecuteNonQuery();
            if (count1 > 0)
            {

                MessageBox.Show("更新备注成功");
            }
            conn1.Close();
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

       

        private void zuofei_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            String sql = String.Format("update t_bl set bz = '{0}', bllx='{1}' where id = '{2}'", jieguo.Text.ToString(),"1", ID1);
            conn1.Open();
            SqlCommand comm = new SqlCommand(sql, conn1);
            int count1 = comm.ExecuteNonQuery();
            if (count1 > 0)
            {

                MessageBox.Show("更新备注成功");
            }
            conn1.Close();
            this.Close();
        }

        private void cankao_Click(object sender, RoutedEventArgs e)
        {
            if (ID2 == "")
            {

            }
            else
            {
                display_bl1 result = new display_bl1(ID2);
                result.Show();
            }
        }

    }
}

