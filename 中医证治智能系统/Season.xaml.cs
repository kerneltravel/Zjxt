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
using System.Windows.Navigation;
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
    /// Interaction logic for Season.xaml
    /// </summary>
    public partial class Season : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        bool isexit = false;
        public Season()
        {
            InitializeComponent();
            DateTime dt = DateTime.Now;
            tx1.Text = dt.Year.ToString() + "/01/01";
            tx2.Text = dt.Year.ToString() + "/12/31";
            string sql = String.Format("select count(*) from t_season where nd = '{0}'", dt.Year.ToString());
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int num = (int)comm.ExecuteScalar();
            if (num > 4)
            {
                isexit = true;
            }
            conn.Close();
            if(isexit==true)
            {
                sql = String.Format("select * from t_season where seasonname = '春季' and nd = '{0}'", dt.Year.ToString());
                conn.Open();
                comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    date1.Text = dr["fromdate"].ToString();
                    date2.Text = dr["todate"].ToString();
                }
                dr.Close();
                conn.Close();
                sql = String.Format("select * from t_season where seasonname = '夏季' and nd = '{0}'", dt.Year.ToString());
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    date3.Text = dr["fromdate"].ToString();
                    date4.Text = dr["todate"].ToString();
                }
                dr.Close();
                conn.Close();
                sql = String.Format("select * from t_season where seasonname = '秋季' and nd = '{0}'", dt.Year.ToString());
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    date5.Text = dr["fromdate"].ToString();
                    date6.Text = dr["todate"].ToString();
                }
                dr.Close();
                conn.Close();
                sql = String.Format("select * from t_season where seasonname = '冬季' and nd = '{0}' and fromdate='{1}'", dt.Year.ToString(), tx1.Text);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    date7.Text = dr["todate"].ToString();
                }
                dr.Close();
                conn.Close();
                sql = String.Format("select * from t_season where seasonname = '冬季' and nd = '{0}' and todate='{1}'", dt.Year.ToString(), tx2.Text);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    date8.Text = dr["fromdate"].ToString();
                }
                dr.Close();
                conn.Close();
            }
            else
            {
                date1.Text = dt.Year.ToString() + "/3/10";
                date2.Text = dt.Year.ToString() + "/7/9";
                date3.Text = dt.Year.ToString() + "/7/10";
                date4.Text = dt.Year.ToString() + "/9/10";
                date5.Text = dt.Year.ToString() + "/9/11";
                date6.Text = dt.Year.ToString() + "/11/10";
                date7.Text = dt.Year.ToString() + "/3/9";
                date8.Text = dt.Year.ToString() + "/11/11";
                sql = String.Format("insert t_season (nd,seasonname,fromdate,todate) values ('{2}','春季','{0}','{1}')", date1.Text, date2.Text, dt.Year.ToString());
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count = comm.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("更新成功");
                }
                conn.Close();
                sql = String.Format("insert t_season (nd,seasonname,fromdate,todate) values ('{2}','夏季','{0}','{1}')", date3.Text, date4.Text, dt.Year.ToString());
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("更新成功");
                }
                conn.Close();
                sql = String.Format("insert t_season (nd,seasonname,fromdate,todate) values ('{2}','秋季','{0}','{1}')", date5.Text, date6.Text, dt.Year.ToString());
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("更新成功");
                }
                conn.Close();
                sql = String.Format("insert t_season (nd,seasonname,fromdate,todate) values ('{2}','冬季','{0}','{1}')", date8.Text, tx2.Text, dt.Year.ToString());
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("更新成功");
                }
                conn.Close();
                sql = String.Format("insert t_season (nd,seasonname,fromdate,todate) values ('{2}','冬季','{0}','{1}')", tx1.Text, date7.Text, dt.Year.ToString());
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("更新成功");
                }
                conn.Close();
            }
            
        }

        private void Button_Click_save(object sender, RoutedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            tx1.Text=dt.Year.ToString()+"/01/01";
            tx2.Text=dt.Year.ToString()+"/12/31";
            date1.DisplayDate = Convert.ToDateTime(date1.SelectedDate);
            DateTime dt1 = date1.DisplayDate;
            date2.DisplayDate = Convert.ToDateTime(date2.SelectedDate);
            DateTime dt2 = date2.DisplayDate;
            date3.DisplayDate = Convert.ToDateTime(date3.SelectedDate);
            DateTime dt3 = date3.DisplayDate;
            date4.DisplayDate = Convert.ToDateTime(date4.SelectedDate);
            DateTime dt4 = date4.DisplayDate;
            date5.DisplayDate = Convert.ToDateTime(date5.SelectedDate);
            DateTime dt5 = date5.DisplayDate;
            date6.DisplayDate = Convert.ToDateTime(date6.SelectedDate);
            DateTime dt6 = date6.DisplayDate;
            date7.DisplayDate = Convert.ToDateTime(date7.SelectedDate);
            DateTime dt7 = date7.DisplayDate;
            date8.DisplayDate = Convert.ToDateTime(date8.SelectedDate);
            DateTime dt8 = date8.DisplayDate;
            if((dt.Year.ToString()!=dt1.Year.ToString())||(dt.Year.ToString()!=dt2.Year.ToString())||(dt.Year.ToString()!=dt3.Year.ToString())||(dt.Year.ToString()!=dt4.Year.ToString())||(dt.Year.ToString()!=dt5.Year.ToString())||(dt.Year.ToString()!=dt6.Year.ToString())||(dt.Year.ToString()!=dt7.Year.ToString())||(dt.Year.ToString()!=dt8.Year.ToString()))
            {
                MessageBox.Show("只能定义今年的季节,所以只能选择今年之内的日期,请您重新选择！");
            }
            else
            {
                string sql = String.Format("update t_season set fromdate = '{0}',todate = '{1}' where nd = '{2}' and seasonname ='春季'",date1.Text,date2.Text,dt.Year.ToString());
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                int count = comm.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("更新成功");
                }
                conn.Close();
                sql = String.Format("update t_season set fromdate = '{0}',todate = '{1}' where nd = '{2}' and seasonname ='夏季'", date3.Text, date4.Text, dt.Year.ToString());
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("更新成功");
                }
                conn.Close();
                sql = String.Format("update t_season set fromdate = '{0}',todate = '{1}' where nd = '{2}' and seasonname ='秋季'", date5.Text, date6.Text, dt.Year.ToString());
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("更新成功");
                }
                conn.Close();
                sql = String.Format("update t_season set fromdate = '{0}' where nd = '{2}' and seasonname ='冬季'and todate = '{1}'", date8.Text,tx2.Text , dt.Year.ToString());
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("更新成功");
                }
                conn.Close();
                sql = String.Format("update t_season set todate = '{1}' where fromdate = '{0}'and nd = '{2}' and seasonname ='冬季'", tx1.Text, date7.Text, dt.Year.ToString());
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("更新成功");
                }
                conn.Close();
            }
            



        }

        private void date1_CalendarClosed(object sender, RoutedEventArgs e)
        {
            date1.DisplayDate =Convert.ToDateTime(date1.SelectedDate) ;
            DateTime dt = date1.DisplayDate;
            date7.SelectedDate=dt.AddDays(-1);

        }

        private void date2_CalendarClosed(object sender, RoutedEventArgs e)
        {
            date2.DisplayDate = Convert.ToDateTime(date2.SelectedDate);
            DateTime dt = date2.DisplayDate;
            date3.SelectedDate = dt.AddDays(1);
        }

        private void date4_CalendarClosed(object sender, RoutedEventArgs e)
        {
            date4.DisplayDate = Convert.ToDateTime(date4.SelectedDate);
            DateTime dt = date4.DisplayDate;
            date5.SelectedDate = dt.AddDays(1);
        }

        private void date6_CalendarClosed(object sender, RoutedEventArgs e)
        {
            date6.DisplayDate = Convert.ToDateTime(date6.SelectedDate);
            DateTime dt = date6.DisplayDate;
            date8.SelectedDate = dt.AddDays(1);
        }

        private void date3_CalendarClosed(object sender, RoutedEventArgs e)
        {
            date3.DisplayDate = Convert.ToDateTime(date3.SelectedDate);
            DateTime dt = date3.DisplayDate;
            date2.SelectedDate = dt.AddDays(-1);
        }

        private void date5_CalendarClosed(object sender, RoutedEventArgs e)
        {
            date5.DisplayDate = Convert.ToDateTime(date5.SelectedDate);
            DateTime dt = date5.DisplayDate;
            date4.SelectedDate = dt.AddDays(-1);
        }

        private void date8_CalendarClosed(object sender, RoutedEventArgs e)
        {
            date8.DisplayDate = Convert.ToDateTime(date8.SelectedDate);
            DateTime dt = date8.DisplayDate;
            date6.SelectedDate = dt.AddDays(-1);
        }

        private void date7_CalendarClosed(object sender, RoutedEventArgs e)
        {
            date7.DisplayDate = Convert.ToDateTime(date7.SelectedDate);
            DateTime dt = date7.DisplayDate;
            date1.SelectedDate = dt.AddDays(1);
        }

        private void Button_Click_close(object sender, RoutedEventArgs e)
        {        
            this.Close();        
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            date1.Text = dt.Year .ToString()+"/3/10";
            date2.Text = dt.Year.ToString() + "/7/9";
            date3.Text = dt.Year.ToString() + "/7/10";
            date4.Text = dt.Year.ToString() + "/9/10";
            date5.Text = dt.Year.ToString() + "/9/11";
            date6.Text = dt.Year.ToString() + "/11/10";
            date7.Text = dt.Year.ToString() + "/3/9";
            date8.Text = dt.Year.ToString() + "/11/11";
        }

    }
}
