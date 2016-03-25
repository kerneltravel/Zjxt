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
    /// Interaction logic for display_bl1.xaml
    /// </summary>
        
    public partial class display_bl1 : Window
    {
        string j;
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        public display_bl1(string id)
        {
            InitializeComponent();
            string sql = String.Format("select * from t_bl where id ='{0}' ", id);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                title.Text = dr["jsxx"].ToString();
                jieguo.Text = dr["bz"].ToString();
                j = dr["zt"].ToString();
            }
            dr.Close();
            conn.Close();
            sql = String.Format("select explain from t_info_jsd where jsd = '{0}'", j);
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
    }
}
