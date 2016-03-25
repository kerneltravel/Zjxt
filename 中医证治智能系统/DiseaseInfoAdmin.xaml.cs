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
    /// Interaction logic for DiseaseInfoAdmin.xaml
    /// </summary>
    public partial class DiseaseInfoAdmin : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 创建集合实例
        ObservableCollection<DiseaseInfo> listDisease = new ObservableCollection<DiseaseInfo>();
        // 全局变量，用于存储病名类信息
        DiseaseInfo Disease_Edit = new DiseaseInfo( "", "", "","","");
        // 用于判断是否进行了增加操作
        bool IsAdd = false;
        // 用于判断是否进行了修改操作
        bool IsModify = false;
        // 用于判断病名名称重复
        bool IsRepeat = false;
        // 创建对 PassValuesHandler 方法的引用的类
        public delegate void PassValuesHandler(object sender, PassValuesEventArgs e);
        // 声明事件
        public event PassValuesHandler PassValuesEvent;
        // 创建事件数据类
        public class PassValuesEventArgs : EventArgs
        {
            private string _name;
            private string _number;
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public string Number
            {
                get { return _number; }
                set { _number = value; }
            }

            public PassValuesEventArgs(string name, string number)
            {
                this.Name = name;
                this.Number = number;
            }
        }

        /// <summary>
        /// 功能：创建病名信息类
        /// 说明：1.DiseaseNumber --> 病名编号 Xi --> 系 DiseaseName --> 病名名称
        /// </summary>
        public class DiseaseInfo : INotifyPropertyChanged
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

            private string _DiseaseNumber; // 病名编号
            private string _Xi;            // 系
            private string _DiseaseName;   // 病名类型
            private string _Bingmjb;       // 病名级别
            private string _Neisbmlx;      // 内伤病名类型

            public string DiseaseNumber
            {
                get { return _DiseaseNumber; }
                set { _DiseaseNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("DiseaseNumber")); }
            }

            public string Xi
            {
                get { return _Xi; }
                set { _Xi = value; OnPropertyChanged(new PropertyChangedEventArgs("Xi")); }
            }

            public string DiseaseName
            {
                get { return _DiseaseName; }
                set { _DiseaseName = value; OnPropertyChanged(new PropertyChangedEventArgs("DiseaseName")); }
            }

            public string Bingmjb
            {
                get { return _Bingmjb; }
                set { _Bingmjb = value; OnPropertyChanged(new PropertyChangedEventArgs("Bingmjb")); }
            }

            public string Neisbmlx
            {
                get { return _Neisbmlx; }
                set { _Neisbmlx = value; OnPropertyChanged(new PropertyChangedEventArgs("Neisbmlx")); }
            }

            public DiseaseInfo(string diseasenumber, string bingmjb, string neisbmlx, string xi, string diseasename)
            {
                _DiseaseNumber = diseasenumber;
                _Bingmjb = bingmjb;
                _Neisbmlx = neisbmlx;
                _Xi = xi;
                _DiseaseName = diseasename;
            }
        }

        public DiseaseInfoAdmin()
        {
            InitializeComponent();

            // 病名类型默认选中第一项【内伤】
            search_bmlx.SelectedIndex = 1;
            // 病名级别默认选中第一项【全部】
            search_bmjb.SelectedIndex = 0;
            // 内伤病名类型默认选中第一项【全部】
            search_nsbmlx.SelectedIndex = 0;
            // 指定 listview 数据源
            search_lv.ItemsSource = listDisease;
            // 初始化控件的可编辑性
            input_bmbh.IsReadOnly = true;
            input_save.IsEnabled = false;
            input_cancel.IsEnabled = false;

            // 信息检索、录入中系下拉框初始化
            string sql = String.Format("select xmc from t_info_x");
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            //// 信息检索系指定数据源
            //search_xi.ItemsSource = ds.Tables[0].DefaultView;
            //search_xi.DisplayMemberPath = "xmc";
            //search_xi.SelectedValuePath = "xbh";
            //search_xi.SelectedIndex = 0;
            // 信息录入系指定数据源 
            input_xi.ItemsSource = ds.Tables[0].DefaultView;
            input_xi.DisplayMemberPath = "xmc";
            input_xi.SelectedValuePath = "xbh";
            // 系
            search_xi.Items.Clear();
            search_xi.Items.Add("全部");
            sql = String.Format("select xmc from t_info_x");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                search_xi.Items.Add(dr["xmc"].ToString());
            }
            dr.Close();
            conn.Close();
            search_xi.SelectedIndex = 0;

        }

        /// <summary>
        /// 功能：防止输入病名重复
        /// </summary>
        public void Is_Repeat()
        {
            string disease_name = input_bmmc.Text.Trim();
            string sql = String.Format("select count(*) from t_info_bm where bmmc = '{0}' and bmbh != '{1}' ", disease_name, Disease_Edit.DiseaseNumber);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count > 0)
            {
                MessageBox.Show("病名名称不能重复！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                IsRepeat = true;
            }
            else
                IsRepeat = false;
            conn.Close();
        }

        /// <summary>
        /// 功能：病名信息检索中的查询功能
        /// </summary>
        private void search_Click(object sender, RoutedEventArgs e)
        {
            listDisease.Clear(); // 先清空集合
            string sql = "";
            string sql_count = "";
            if (search_bmlx.Text.Trim() == "外感")
            {
                if (search_bmjb.SelectedIndex == 0)
                {
                    sql = String.Format("select * from t_info_bm where bmlx = '{0}' and bmmc like '%{1}%'", 0, search_bmmc.Text.Trim());
                    sql_count = String.Format("select count(*) from t_info_bm where bmlx = '{0}' and bmmc like '%{1}%' ", 0, search_bmmc.Text.Trim());
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        string bmjb = "";
                        switch(dr["bmjb"].ToString())
                        {
                            case "1":
                                bmjb = "危";
                                break;
                            case "2":
                                bmjb = "急";
                                break;
                            case "3":
                                bmjb = "重";
                                break;
                            case "4":
                                bmjb = "轻";
                                break;
                        }
                        listDisease.Add(new DiseaseInfo(dr["bmbh"].ToString(), bmjb, "", "", dr["bmmc"].ToString()));
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
                else 
                {
                    sql = String.Format("select * from t_info_bm where bmlx = '{0}' and bmjb = '{1}' and bmmc like '%{2}%'", 0, (search_bmjb.SelectedIndex ).ToString(), search_bmmc.Text.Trim());
                    sql_count = String.Format("select count(*) from t_info_bm where bmlx = '{0}' and bmjb = '{1}' and bmmc like '%{2}%' ", 0, (search_bmjb.SelectedIndex ).ToString(), search_bmmc.Text.Trim());
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        string bmjb = "";
                        switch (dr["bmjb"].ToString())
                        {
                            case "1":
                                bmjb = "危";
                                break;
                            case "2":
                                bmjb = "急";
                                break;
                            case "3":
                                bmjb = "重";
                                break;
                            case "4":
                                bmjb = "轻";
                                break;
                        }
                        listDisease.Add(new DiseaseInfo(dr["bmbh"].ToString(), bmjb, "", "", dr["bmmc"].ToString()));
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
            if (search_bmlx.Text.Trim() == "内伤")
            {
                // 病名级别、内伤病名类型、系都为空
                if (search_bmjb.SelectedIndex == 0 && search_nsbmlx.SelectedIndex == 0 && search_xi.SelectedIndex == 0)
                {
                    sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%'"
                                           , search_bmmc.Text.Trim());
                    sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%'"
                                        , search_bmmc.Text.Trim());
                }
                // 病名级别不为空，内伤病名类型、系都为空
                else if (search_bmjb.SelectedIndex != 0 && search_nsbmlx.SelectedIndex == 0 && search_xi.SelectedIndex == 0)
                {
                    sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and bmjb = '{1}'"
                       , search_bmmc.Text.Trim(), (search_bmjb.SelectedIndex).ToString());
                    sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and bmjb = '{1}'"
                                        , search_bmmc.Text.Trim(), (search_bmjb.SelectedIndex).ToString());
                }
                // 内伤病名类型不为空，病名级别、系都为空
                else if (search_bmjb.SelectedIndex == 0 && search_nsbmlx.SelectedIndex != 0 && search_xi.SelectedIndex == 0)
                {
                    sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}'"
                       , search_bmmc.Text.Trim(), (search_nsbmlx.SelectedIndex - 1).ToString());
                    sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}'"
                                        , search_bmmc.Text.Trim(), (search_nsbmlx.SelectedIndex - 1).ToString());
                }
                // 系不为空，病名级别、内伤病名类型都为空
                else if (search_bmjb.SelectedIndex == 0 && search_nsbmlx.SelectedIndex == 0 && search_xi.SelectedIndex != 0)
                {
                    sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and xmc = '{1}'"
                       , search_bmmc.Text.Trim(), search_xi.Text);
                    sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and xmc = '{1}'"
                                        , search_bmmc.Text.Trim(), search_xi.Text);
                }
                // 病名级别、内伤病名类型不为空，系为空
                else if (search_bmjb.SelectedIndex != 0 && search_nsbmlx.SelectedIndex != 0 && search_xi.SelectedIndex == 0)
                {
                    sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and bmjb = '{1}' and nsbmlx = '{2}'"
                       , search_bmmc.Text.Trim(), (search_bmjb.SelectedIndex).ToString(), (search_nsbmlx.SelectedIndex - 1).ToString());
                    sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and bmjb = '{1}' and nsbmlx = '{2}'"
                                        , search_bmmc.Text.Trim(), (search_bmjb.SelectedIndex).ToString(), (search_nsbmlx.SelectedIndex - 1).ToString());
                }
                // 病名级别、系不为空，内伤病名类型为空
                else if (search_bmjb.SelectedIndex != 0 && search_nsbmlx.SelectedIndex == 0 && search_xi.SelectedIndex != 0)
                {
                    sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and bmjb = '{1}' and xmc = '{2}'"
                       , search_bmmc.Text.Trim(), (search_bmjb.SelectedIndex ).ToString(), search_xi.Text);
                    sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and bmjb = '{1}' and xmc = '{2}'"
                                        , search_bmmc.Text.Trim(), (search_bmjb.SelectedIndex ).ToString(), search_xi.Text);
                }
                // 内伤病名类型、系不为空，病名级别为空
                else if (search_bmjb.SelectedIndex == 0 && search_nsbmlx.SelectedIndex != 0 && search_xi.SelectedIndex != 0)
                {
                    sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}' and xmc = '{2}'"
                       , search_bmmc.Text.Trim(), (search_nsbmlx.SelectedIndex - 1).ToString(), search_xi.Text);
                    sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}' and xmc = '{2}'"
                                        , search_bmmc.Text.Trim(), (search_nsbmlx.SelectedIndex - 1).ToString(), search_xi.Text);
                }
                else
                {
                    sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and bmjb = '{1}' and nsbmlx = '{2}' and xmc = '{3}'"
                           , search_bmmc.Text.Trim(), (search_bmjb.SelectedIndex).ToString(), (search_nsbmlx.SelectedIndex - 1).ToString(), search_xi.Text);
                            sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and bmjb = '{1}' and nsbmlx = '{2}' and xmc = '{3}'"
                                                , search_bmmc.Text.Trim(), (search_bmjb.SelectedIndex).ToString(), (search_nsbmlx.SelectedIndex - 1).ToString(), search_xi.Text);
                }
                //// 病名级别、内伤病名类型、系都不为空
                //else if (search_bmjb.SelectedIndex != 0 && search_nsbmlx.SelectedIndex != 0 && search_xi.SelectedIndex != 0)
                //{
                //    if (search_bmjb.Text.Trim() == "全部" && search_nsbmlx.Text.Trim() == "全部" && search_xi.Text.Trim() == "全部")
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%'"
                //       , search_bmmc.Text.Trim());
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%'"
                //                            , search_bmmc.Text.Trim());
                //    }
                //    else 
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and bmjb = '{1}' and nsbmlx = '{2}' and xmc = '{3}'"
                //       , search_bmmc.Text.Trim(), (search_bmjb.SelectedIndex + 1).ToString(), (search_nsbmlx.SelectedIndex).ToString(), search_xi.Text);
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and bmjb = '{1}' and nsbmlx = '{2}' and xmc = '{3}'"
                //                            , search_bmmc.Text.Trim(), (search_bmjb.SelectedIndex + 1).ToString(), (search_nsbmlx.SelectedIndex).ToString(), search_xi.Text);
                //    }
                   
                //}
                //// 内伤病名类型为空时
                //if (search_nsbmlx.Text.Trim() == "")
                //{
                //    if (search_xi.Text.Trim() == "")
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%'"
                //                               , search_bmmc.Text.Trim());
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%'"
                //                            , search_bmmc.Text.Trim());
                //    }
                //    else
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and xmc = '{1}'and bmjb = '{2}'"
                //                           , search_bmmc.Text.Trim(), search_xi.Text.Trim(), (search_bmjb.SelectedIndex + 1).ToString());
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and xmc = '{1}'and bmjb = '{2}'"
                //                        , search_bmmc.Text.Trim(), search_xi.Text.Trim(), (search_bmjb.SelectedIndex + 1).ToString());
                //    }
                //}
                //// 内伤病名类型为"甲类病名"时
                //if(search_nsbmlx.Text.Trim() == "甲类病名")
                //{
                //    if (search_xi.Text.Trim() == "")
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}'"
                //                          , search_bmmc.Text.Trim(), 0);
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}'"
                //                        , search_bmmc.Text.Trim(), 0);
                //    }
                //    else
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}' and xmc = '{2}'"
                //                           , search_bmmc.Text.Trim(), 0, search_xi.Text.Trim());
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}' and xmc = '{2}'"
                //                        , search_bmmc.Text.Trim(), 0, search_xi.Text.Trim());
                //    }
                //}
                //// 内伤病名类型为"乙1类病名"时
                //if(search_nsbmlx.Text.Trim() == "乙1类病名")
                //{
                //    if (search_xi.Text.Trim() == "")
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}'"
                //                           , search_bmmc.Text.Trim(), 1);
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}'"
                //                        , search_bmmc.Text.Trim(), 1);
                //    }
                //    else
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}' and xmc = '{2}'"
                //                           , search_bmmc.Text.Trim(), 1, search_xi.Text.Trim());
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}' and xmc = '{2}'"
                //                        , search_bmmc.Text.Trim(), 1, search_xi.Text.Trim());
                //    }
                //}
                //// 内伤病名类型为"乙2类病名"时
                //if (search_nsbmlx.Text.Trim() == "乙1类病名")
                //{
                //    if (search_xi.Text.Trim() == "")
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}'"
                //                           , search_bmmc.Text.Trim(), 1);
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}'"
                //                        , search_bmmc.Text.Trim(), 1);
                //    }
                //    else
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}' and xmc = '{2}'"
                //                           , search_bmmc.Text.Trim(), 1, search_xi.Text.Trim());
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}' and xmc = '{2}'"
                //                        , search_bmmc.Text.Trim(), 1, search_xi.Text.Trim());
                //    }
                //}
                //// 内伤病名类型为"乙3类病名"时
                //if (search_nsbmlx.Text.Trim() == "乙1类病名")
                //{
                //    if (search_xi.Text.Trim() == "")
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}'"
                //                           , search_bmmc.Text.Trim(), 1);
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}'"
                //                        , search_bmmc.Text.Trim(), 1);
                //    }
                //    else
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}' and xmc = '{2}'"
                //                           , search_bmmc.Text.Trim(), 1, search_xi.Text.Trim());
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}' and xmc = '{2}'"
                //                        , search_bmmc.Text.Trim(), 1, search_xi.Text.Trim());
                //    }
                //}
                //// 内伤病名类型为"乙4类病名"时
                //if (search_nsbmlx.Text.Trim() == "乙1类病名")
                //{
                //    if (search_xi.Text.Trim() == "")
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}'"
                //                           , search_bmmc.Text.Trim(), 1);
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}'"
                //                        , search_bmmc.Text.Trim(), 1);
                //    }
                //    else
                //    {
                //        sql = String.Format("select * from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}' and xmc = '{2}'"
                //                           , search_bmmc.Text.Trim(), 1, search_xi.Text.Trim());
                //        sql_count = String.Format("select count(*) from t_info_bm inner join t_info_x on t_info_x.xbh = t_info_bm.xbh where bmmc like '%{0}%' and nsbmlx = '{1}' and xmc = '{2}'"
                //                        , search_bmmc.Text.Trim(), 1, search_xi.Text.Trim());
                //    }
                //}
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    string bmjb = "";
                    string nsbmlx = "";
                    switch (dr["bmjb"].ToString())
                    {
                        case "1":
                            bmjb = "危";
                            break;
                        case "2":
                            bmjb = "急";
                            break;
                        case "3":
                            bmjb = "重";
                            break;
                        case "4":
                            bmjb = "轻";
                            break;
                    }
                    switch (dr["nsbmlx"].ToString())
                    {
                        case "0":
                            nsbmlx = "甲类";
                            break;
                        case "1":
                            nsbmlx = "乙类";
                            break;
                    }
                    listDisease.Add(new DiseaseInfo(dr["bmbh"].ToString(), bmjb, nsbmlx, dr["xmc"].ToString(), dr["bmmc"].ToString()));
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
        /// 功能：病名信息检索中【选定】操作
        /// </summary>
        private void select_search_Click(object sender, RoutedEventArgs e)
        {
            DiseaseInfo diseaseinfo = search_lv.SelectedItem as DiseaseInfo;
            if (diseaseinfo != null && diseaseinfo is DiseaseInfo)
            {
                PassValuesEventArgs args = new PassValuesEventArgs(diseaseinfo.DiseaseName.ToString(),diseaseinfo.DiseaseNumber.ToString());
                // 要判断 PassValuesEvent 是否为空，即判断该窗口是否被调用
                if (PassValuesEvent != null) 
                {
                    PassValuesEvent(this, args);               
                }
            }
            this.Close();
        }

        /// <summary>
        /// 功能：病名信息检索中删除操作
        /// </summary> 
        private void delete_search_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("执行删除操作将对数据库造成很大影响，强烈建议不要执行此操作，您确认要删除该病名吗？", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (MessageBox.Show("再次警告：您确认要删除该病名吗？", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    DiseaseInfo diseaseinfo = search_lv.SelectedItem as DiseaseInfo;
                    if (diseaseinfo != null && diseaseinfo is DiseaseInfo)
                    {
                        listDisease.Remove(diseaseinfo);
                    }
                    try
                    {
                        string sql = String.Format("delete from t_info_bm where bmbh = '{0}'", diseaseinfo.DiseaseNumber);
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

        /// <summary>
        /// 功能：病名信息检索中【修改】操作
        /// 说明：【修改】功能必须是对具体项有效，也就是必须是有编号的项
        /// </summary>
        private void modify_search_Click(object sender, RoutedEventArgs e)
        {           
            if (!IsAdd)
            {
                if (input_bmbh.Text.Trim() == "")
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
                    Disease_Edit.DiseaseNumber = input_bmbh.Text;
                }            
            }
        }

        /// <summary>
        /// 功能：病名信息检索中【返回】操作
        /// </summary>
        private void back_search_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：病名类型选项变化时触发事件
        /// </summary>
        private void bmlx_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 首先清空 listview
            listDisease.Clear();
            if (search_bmlx.SelectedIndex == 0)
            {
                search_nsbmlx.IsEnabled = false;
                search_xi.IsEnabled = false;
            }
            else
            {
                search_nsbmlx.IsEnabled = true;
                search_xi.IsEnabled = true;
            }
        }

        /// <summary>
        /// 功能：选择某项时病名信息录入界面显示相应的信息
        /// </summary>
        private void search_lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DiseaseInfo diseaseinfo = search_lv.SelectedItem as DiseaseInfo;
            if (diseaseinfo != null && diseaseinfo is DiseaseInfo)
            {
                string xi_number = "";
                string nsbmlx_number = "";
                string bmjb_number = "";
                string sql = String.Format("select * from t_info_bm where bmbh = '{0}'", diseaseinfo.DiseaseNumber);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["bmlx"].ToString() == "0")
                        input_bmlx.Text = "外感病名";
                    else
                        input_bmlx.Text = "内伤病名";
                    input_bmbh.Text = dr["bmbh"].ToString();
                    input_bmmc.Text = dr["bmmc"].ToString();
                    xi_number = dr["xbh"].ToString();
                    nsbmlx_number = dr["nsbmlx"].ToString();
                    bmjb_number = dr["bmjb"].ToString();
                    input_bz.Text = dr["bz"].ToString();
                }
                dr.Close();
                conn.Close();
                // 通过系编号获取系名称
                sql = String.Format("select xmc from t_info_x where xbh = '{0}'", xi_number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    input_xi.Text = dr[0].ToString();
                }
                dr.Close();
                conn.Close();
                // 通过编号获取内伤病名类型
                if (nsbmlx_number == "0")
                    input_nsbmlx.Text = "甲类病名";
                if (nsbmlx_number == "1")
                    input_nsbmlx.Text = "乙类病名";
                // 通过编号获取病名级别
                if (bmjb_number == "1")
                    input_bmjb.Text = "危";
                if (bmjb_number == "2")
                    input_bmjb.Text = "急";
                if (bmjb_number == "3")
                    input_bmjb.Text = "重";
                if (bmjb_number == "4")
                    input_bmjb.Text = "轻";
            }
        }


        /////////////////////////////////// 病名信息检索 //////////////////////////////////////////
        ////////////////////////////////////// 分割线 /////////////////////////////////////////////
        /////////////////////////////////// 病名信息录入 //////////////////////////////////////////


        /// <summary>
        /// 功能：返回主菜单
        /// </summary>
        private void input_back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：病名信息录入【增加】按钮
        /// 说明：1.在进行修改操作时，点击【增加】按钮是无效的
        ///       2.点击【增加】按钮时，应先将所有编辑框清空
        /// </summary>
        private void input_add_Click(object sender, RoutedEventArgs e)
        {
            if(!IsModify)
            {
                IsAdd = true;
                // 【增加】操作中，【保存】和【取消】应设置为可用
                input_save.IsEnabled = true;
                input_cancel.IsEnabled = true;
                //  清空编辑框
                //input_bmlx.Text = "";
                input_bmbh.Text = "";
                input_bmmc.Text = "";
                input_bmjb.Text = "";
                input_xi.Text = "";
                input_nsbmlx.Text = "";
                input_bz.Text = "";

                if (input_bmlx.Text.Trim() == "")
                {
                    MessageBox.Show("请选择病名类型！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (input_bmlx.Text.Trim() == "外感病名")
                    {
                        string sql = String.Format("select max(bmbh) from t_info_bm where bmlx = 0");
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        SqlDataReader dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            Disease_Edit = new DiseaseInfo(String.Format("{0:000000}", Convert.ToInt64(dr[0]) + 1), "1", "甲","", "");
                            listDisease.Add(Disease_Edit);
                            input_bmbh.Text = Disease_Edit.DiseaseNumber;
                        }
                        dr.Close();
                        conn.Close();
                    }
                    if(input_bmlx.Text.Trim() == "内伤病名")
                    {
                        string sql = String.Format("select max(bmbh) from t_info_bm where bmlx = 1");
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        SqlDataReader dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            Disease_Edit = new DiseaseInfo(String.Format("{0:000000}", Convert.ToInt64(dr[0]) + 1), "1", "甲", "", "");
                            listDisease.Add(Disease_Edit);
                            input_bmbh.Text = Disease_Edit.DiseaseNumber;
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
            // 检测病名名称是否输入重复
            Is_Repeat();
            // 【增加】功能下【保存】功能
            if(IsAdd == true && IsRepeat == false)
            {
                if (input_bmlx.Text.Trim() == "")
                {
                    MessageBox.Show("请选择病名类型！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (input_bmlx.Text.Trim() == "外感病名")
                    {
                        if (input_bmmc.Text.Trim() == "")
                        {
                            MessageBox.Show("病名名称不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if (input_bmjb.Text.Trim() == "")
                            {
                                MessageBox.Show("病名级别不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                try
                                {
                                    string sql = String.Format("INSERT INTO t_info_bm ( bmbh, bmmc, bmlx, bz, bmjb) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')"
                                                , Disease_Edit.DiseaseNumber, input_bmmc.Text, 0, input_bz.Text, (input_bmjb.SelectedIndex + 1).ToString());
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
                    if (input_bmlx.Text.Trim() == "内伤病名")
                    {
                        if (input_bmmc.Text.Trim() == "")
                        {
                            MessageBox.Show("病名名称不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if(input_xi.Text.Trim() == "")
                            {
                                MessageBox.Show("系不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                if (input_nsbmlx.Text.Trim() == "")
                                {
                                    MessageBox.Show("内伤病名类型不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    if (input_bmjb.Text == "")
                                    {
                                        MessageBox.Show("病名级别不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            // 获取系编号
                                            string xi_number = "";
                                            string sql = String.Format("select xbh from t_info_x where xmc = '{0}'", input_xi.Text);
                                            conn.Open();
                                            SqlCommand comm = new SqlCommand(sql, conn);
                                            SqlDataReader dr = comm.ExecuteReader();
                                            while (dr.Read())
                                            {
                                                xi_number = dr[0].ToString();
                                            }
                                            dr.Close();
                                            conn.Close();

                                            //// 获取内伤病名类型编号0:1
                                            //string nsbmlx_number = "";
                                            //if (input_nsbmlx.Text == "甲类病名")
                                            //    nsbmlx_number = "0";
                                            //if (input_nsbmlx.Text == "乙类病名")
                                            //    nsbmlx_number = "1";

                                            // 写入数据库
                                            sql = String.Format("INSERT INTO t_info_bm ( bmbh, bmmc, bmlx, xbh, nsbmlx, bz, bmjb) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}','{5}','{6}')"
                                                                        , Disease_Edit.DiseaseNumber, input_bmmc.Text, 1, xi_number, input_nsbmlx.SelectedIndex.ToString()
                                                                        , input_bz.Text, (input_bmjb.SelectedIndex + 1).ToString());
                                            conn.Open();
                                            comm = new SqlCommand(sql, conn);
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
                }
                
            }
            // 【修改】功能下【保存】功能
            if (IsModify == true && IsRepeat == false)
            {
                if (input_bmlx.Text.Trim() == "外感病名")
                {
                    if (input_bmmc.Text.Trim() == "")
                    {
                        MessageBox.Show("病名名称不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        if (input_bmjb.Text.Trim() == "")
                        {
                            MessageBox.Show("病名级别不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            try
                            {
                                string sql = String.Format("update t_info_bm set bmmc = '{0}', bmlx = '{1}', bz = '{2}', bmjb = '{3}' where  bmbh = '{4}'"
                                                            , input_bmmc.Text, 0, input_bz.Text, (input_bmjb.SelectedIndex + 1).ToString(), Disease_Edit.DiseaseNumber);
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
                if (input_bmlx.Text.Trim() == "内伤病名")
                {
                    if (input_bmmc.Text.Trim() == "")
                    {
                        MessageBox.Show("病名名称不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        if (input_xi.Text.Trim() == "")
                        {
                            MessageBox.Show("系不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if (input_nsbmlx.Text.Trim() == "")
                            {
                                MessageBox.Show("内伤病名类型不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                if (input_bmjb.Text == "")
                                {
                                    MessageBox.Show("病名级别不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    try
                                    {
                                        // 获取系编号
                                        string xi_number = "";
                                        string sql = String.Format("select xbh from t_info_x where xmc = '{0}'", input_xi.Text);
                                        conn.Open();
                                        SqlCommand comm = new SqlCommand(sql, conn);
                                        SqlDataReader dr = comm.ExecuteReader();
                                        while (dr.Read())
                                        {
                                            xi_number = dr[0].ToString();
                                        }
                                        dr.Close();
                                        conn.Close();

                                        //// 获取内伤病名类型编号0:1
                                        //string nsbmlx_number = "";
                                        //if (input_nsbmlx.Text == "甲类病名")
                                        //    nsbmlx_number = "0";
                                        //if (input_nsbmlx.Text == "乙类病名")
                                        //    nsbmlx_number = "1";

                                        // 写入数据库
                                        sql = String.Format("update t_info_bm set bmmc = '{0}', bmlx = '{1}', xbh = '{2}', nsbmlx = '{3}', bz = '{4}', bmjb = '{5}' where bmbh = '{6}'"
                                                                    , input_bmmc.Text, 1, xi_number, input_nsbmlx.SelectedIndex.ToString(), input_bz.Text, (input_bmjb.SelectedIndex + 1).ToString() ,Disease_Edit.DiseaseNumber);
                                        conn.Open();
                                        comm = new SqlCommand(sql, conn);
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
            }
        }

        /// <summary>
        /// 功能：病名信息录入中病名类型选择变化时触发事件
        /// 说明：1.当病名类型为“外感病名”时，系和内伤病名类型编辑框应设置为只读（病名编号始终为只读）
        ///       2.当病名类型为“内伤病名”时，系和内伤病名类型编辑框应设置为可编辑
        /// </summary>
        private void input_bmlx_DropDownClosed(object sender, EventArgs e)
        {
            if (input_bmlx.Text.Trim() == "外感病名")
            {
                input_xi.IsEnabled = false;
                input_nsbmlx.IsEnabled = false;
            }
            if (input_bmlx.Text.Trim() == "内伤病名")
            {
                input_xi.IsEnabled = true;
                input_nsbmlx.IsEnabled = true;
            }
        }

        /// <summary>
        /// 功能：病名信息录入中【修改】按钮功能
        /// 说明：【修改】功能必须是对具体项有效，也就是必须是有编号的项
        /// </summary>
        private void input_modify_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdd)
            {
                if (input_bmbh.Text.Trim() == "")
                {
                    MessageBox.Show("请选择具体项进行修改！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    IsModify = true;
                    // 【修改】操作中，【保存】和【取消】应设置为可用
                    input_save.IsEnabled = true;
                    input_cancel.IsEnabled = true;
                    Disease_Edit.DiseaseNumber = input_bmbh.Text;
                }               
            }
        }

        /// <summary>
        /// 功能：病名信息录入中【取消】按钮功能
        /// </summary>
        private void input_cancel_Click(object sender, RoutedEventArgs e)
        {
            // 【增加】功能下【取消】
            if(IsAdd)
            {
                IsAdd = false;
                input_save.IsEnabled = false;
                input_cancel.IsEnabled = false;
                input_bmbh.Text = "";
                input_bmmc.Text = "";
                input_xi.Text = "";
                input_nsbmlx.Text = "";
                input_bz.Text = "";
            }
            // 【修改】功能下【取消】
            if (IsModify)
            {
                IsModify = false;
                input_save.IsEnabled = false;
                input_cancel.IsEnabled = false;
                // 恢复修改前的数据
                DiseaseInfo diseaseinfo = search_lv.SelectedItem as DiseaseInfo;
                if (diseaseinfo != null && diseaseinfo is DiseaseInfo)
                {
                    string xi_number = "";
                    string nsbmlx_number = "";
                    string sql = String.Format("select * from t_info_bm where bmbh = '{0}'", diseaseinfo.DiseaseNumber);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr["bmlx"].ToString() == "0")
                            input_bmlx.Text = "外感病名";
                        else
                            input_bmlx.Text = "内伤病名";
                        input_bmbh.Text = dr["bmbh"].ToString();
                        input_bmmc.Text = dr["bmmc"].ToString();
                        xi_number = dr["xbh"].ToString();
                        nsbmlx_number = dr["nsbmlx"].ToString();
                        input_bz.Text = dr["bz"].ToString();
                    }
                    dr.Close();
                    conn.Close();
                    // 通过系编号获取系名称
                    sql = String.Format("select xmc from t_info_x where xbh = '{0}'", xi_number);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        input_xi.Text = dr[0].ToString();
                    }
                    dr.Close();
                    conn.Close();
                    // 通过编号获取内伤病名类型
                    if (nsbmlx_number == "0")
                        input_nsbmlx.Text = "甲类病名";
                    if (nsbmlx_number == "1")
                        input_nsbmlx.Text = "乙类病名";
                }
            }
        }

        /// <summary>
        /// 功能：实现 combobox 下拉绑定数据库并显示
        /// </summary>
        private void search_xi_DropDownOpened(object sender, EventArgs e)
        {

        }
        private void StuSort(ListView lv, string sortBy, ListSortDirection direction)
        {

            ICollectionView dataView = CollectionViewSource.GetDefaultView(lv.ItemsSource);//获取数据源视图
            dataView.SortDescriptions.Clear();//清空默认排序描述
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);//加入新的排序描述
            dataView.Refresh();//刷新视图
        }
        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader gch = e.OriginalSource as GridViewColumnHeader;
            StuSort(this.search_lv, "Bingmjb", ListSortDirection.Ascending);
           
        }

                  
    }
}
