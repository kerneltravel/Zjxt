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
    public partial class display_lczd : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        //显示诊断结果
        string diagnosisresult = "";
        string brid = "";
        string time = "";
        string chuqizhengxiang = "";
        string bingming = "";
        string jsd = "";
        string ID = "";
        public display_lczd(string id)
        {
            InitializeComponent();
            ID = id;
            string sql = String.Format("select blbh from t_bl where id ='{0}' ", id);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult = "病历编号" + dr[0].ToString() + "\n";
            }
            dr.Close();
            conn.Close();
            diagnosisresult += "----------------------------------------------------------------------------------" + "\n";
            sql = String.Format("select * from t_bl where id ='{0}'and bllx='0' ", id);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                brid = dr["jsxx"].ToString();
                time = dr["jzsj"].ToString();
            }
            dr.Close();
            conn.Close();
            sql = String.Format("select * from t_br_info where brid ='{0}' ", brid);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult = diagnosisresult + "姓名：" + dr["xm"].ToString() + "性别：" + dr["xb"].ToString() + "年龄：" + dr["nl"].ToString() + "就诊时间：" + time + "\n";

            }
            dr.Close();
            conn.Close();
            diagnosisresult += "----------------------------------------------------------------------------------" + "\n";
            sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '6'", id);  //主诉
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult = diagnosisresult + "主诉：" + dr["xxbh"].ToString();

            }
            dr.Close();
            conn.Close();
            sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '9'", id);  //主诉时间
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult = diagnosisresult + "(" + dr["xxbh"].ToString() + ")";

            }
            dr.Close();
            conn.Close();
            diagnosisresult += "\n" + "现病史：";
            sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '7'", id);  //初起症象
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                chuqizhengxiang = dr["xxbh"].ToString();
            }
            dr.Close();
            conn.Close();
            if (chuqizhengxiang == "")
            {
                sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '8'", id);  //现症象
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    //diagnosisresult = diagnosisresult + "                                    " + dr["xxbh"].ToString() + "\n";
                    diagnosisresult = diagnosisresult + dr["xxbh"].ToString() + "\n";
                }
                dr.Close();
                conn.Close();
            }
            else
            {

                diagnosisresult = diagnosisresult + "                                    " + "\n" + "初起症：" + chuqizhengxiang + "                 " + "\n" + "现症：";
                sql = String.Format("select * from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '8'", id);  //现症象
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    diagnosisresult = diagnosisresult + dr["xxbh"].ToString() + "\n";
                }
                dr.Close();
                conn.Close();
            }
            sql = String.Format("select * from t_bl_mx  where id = '{0}' and xxdllx = '0' and xxxllx = 'b'", id);  //西医病名名称
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult = diagnosisresult + "西医诊断：" + dr["xxbh"].ToString() + "\n";
            }
            dr.Close();
            conn.Close();
            sql = String.Format("select * from t_bl_mx  where id = '{0}' and xxdllx = '0' and xxxllx = 'a'", id);  //既往史
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult = diagnosisresult + "既往史：" + dr["xxbh"].ToString() + "\n";
            }
            dr.Close();
            conn.Close();
            diagnosisresult += "诊断结论：                                                              ";
            sql = String.Format("select b.bmmc from t_bl_mx a, t_info_bm b where id = '{0}' and a.xxbh = b.bmbh and xxdllx = '2' and xxxllx = '0'", id);  //病名编号
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                bingming = dr["bmmc"].ToString();
            }
            dr.Close();
            conn.Close();
            diagnosisresult += "\n" + "    病名：" + bingming + " （中医） " + "\n";
            sql = String.Format("select * from t_bl_mx  where id = '{0}' and xxdllx = '0' and xxxllx = 'b'", id);  //西医病名名称
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                bingming = dr["xxbh"].ToString();
            }
            dr.Close();
            conn.Close();
            diagnosisresult += bingming + " （西医） ";
            diagnosisresult += "\n" + "证名：";
            sql = String.Format("select b.jbzmmc from t_bl_mx a, t_info_jbzm b where id = '{0} ' and a.xxbh = b.jbzmbh and xxdllx = '2' and xxxllx = '2'", id);  //证名编号
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult += dr["jbzmmc"].ToString();
            }
            dr.Close();
            conn.Close();
            sql = String.Format("select b.xyzmmc from t_bl_mx a, t_rule_xyzz b where id = ' {0}' and a.xxbh = b.xybmbh and xxdllx = '0' and xxxllx = '5'", id);  //西医证名编号
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult += dr["xyzmmc"].ToString();
            }
            dr.Close();
            conn.Close();
            diagnosisresult += "\n" + "治法：";
            sql = String.Format("select b.zf from t_bl_mx a, t_info_jbzm b where id = '{0}' and a.xxbh = b.jbzmbh and xxdllx = '2' and xxxllx = '2'", id);  //中医治法
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult += dr["zf"].ToString();
            }
            dr.Close();
            conn.Close();
            sql = String.Format("select c.xyzf from t_bl_mx a, t_rule_xyzz b ,t_rule_xycf c where id = '{0}' and a.xxbh = b.xybmbh  and b.xyzmbh = c.xyzmbh and xxdllx = '0' and xxxllx = '5'", id);  //西医治法
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult += dr["xyzf"].ToString();
            }
            dr.Close();
            conn.Close();
            diagnosisresult += "\n" + "主方名：";
            sql = String.Format("select b.jbcfmc from t_bl_mx a, t_info_jbcfxx b where id = '{0}' and a.xxbh = b.jbcfbh and xxdllx ='2' and xxxllx = '4'", id);  //处方编号
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult += dr["jbcfmc"].ToString();
            }
            dr.Close();
            conn.Close();
            sql = String.Format("select c.xycfmc from t_bl_mx a, t_rule_xyzz b ,t_rule_xycf c where id = '{0}' and a.xxbh = b.xybmbh  and b.xyzmbh = c.xyzmbh and xxdllx = '0' and xxxllx = '5'", id);  //西医处方编号("select b.jbcfmc from t_bl_mx a, t_info_jbcfxx b where id = '{0}' and a.xxbh = b.jbcfbh and xxdllx ='2' and xxxllx = '4'", id);  //处方编号
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult += dr["xycfmc"].ToString();
            }
            dr.Close();
            conn.Close();
            diagnosisresult += "\n" + "处方：";
            sql = String.Format("select b.ywmc,a.ywjl,a.dw from t_zd_ywzc a, t_info_yw b where id = '{0}' and a.ywbh = b.ywbh", id);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult += dr["ywmc"].ToString() + " " + Convert.ToInt64(dr["ywjl"]).ToString() + dr["dw"].ToString()+"  ";
            }
            dr.Close();
            conn.Close();
            diagnosisresult += "\n" + "服法：";
            sql = String.Format("select xxbh from  t_bl_mx a where a.id = '{0}' and a.xxdllx = '2' and a.xxxllx = '5'", id);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                diagnosisresult += dr["xxbh"].ToString() + "\n";
            }
            dr.Close();
            conn.Close();
            sql = String.Format("select c.xycfmc,c.xyff from t_bl_mx a, t_rule_xyzz b ,t_rule_xycf c where id = '{0}' and a.xxbh = b.xybmbh  and b.xyzmbh = c.xyzmbh and xxdllx = '0' and xxxllx = '5'", id);  //西医治法
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            if (dr.Read())
            {
                diagnosisresult += "处方二：";
            }
            while (dr.Read())
            {
                diagnosisresult += dr["xycfmc"].ToString() + "\n";
            }
            if (dr.Read())
            {
                diagnosisresult += "服法：";
            }
            while (dr.Read())
            {
                diagnosisresult += dr["xyff"].ToString() + "\n";
            }
            dr.Close();
            conn.Close();
            diagnosisresult += "                                            医师：__________";

            jieguo.Text = diagnosisresult;
            sql = String.Format("update t_bl set bz = ' {0} ' where id = '{1}'", jieguo.Text.ToString(), id);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            int count1 = comm.ExecuteNonQuery();
            if (count1 > 0)
            {

                MessageBox.Show("更新备注成功");
            }
            conn.Close();
            sql = String.Format("select zt from t_bl where id = '{0}'", id);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                jsd = dr["zt"].ToString();
            }
            dr.Close();
            conn.Close();
            sql = String.Format("select explain from t_info_jsd where jsd = '{0}'", jsd);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                jinsidu.Text += dr["explain"].ToString();
            }
            dr.Close();
            conn.Close();
        }

        private void dayin_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pDialog = new PrintDialog();
            if (pDialog.ShowDialog() == true)
            {
                pDialog.PrintVisual(jieguo, "诊断结果");
            }
        }

        private void zuofei_Click(object sender, RoutedEventArgs e)
        {
            string sql = String.Format("update t_bl set bllx = '4' where id = '{0}'", ID);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = comm.ExecuteNonQuery();
            if (count > 0)
            {
                MessageBox.Show("作废成功");
            }
            conn.Close();

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            String sql = String.Format("update t_bl set bz = ' {0} ' where id = '{1}'", jieguo.Text.ToString(), ID);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count1 = comm.ExecuteNonQuery();
            if (count1 > 0)
            {

                MessageBox.Show("更新备注成功");
            }
            conn.Close();
            this.Close();
        }

    }
}
