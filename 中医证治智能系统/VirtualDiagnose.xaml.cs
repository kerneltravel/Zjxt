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
    /// Interaction logic for VirtualDiagnose.xaml
    /// </summary>
    public partial class VirtualDiagnose : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        string oldnumber = "";
        string number = "";
        string number1 = "";
        string title = "";
        string zxnumber;
        string bh = "";
        string xlbh = "";
        string subid = "";
        // 用于判断是否进行了 Add 操作
        bool IsAdd = false;
        bool iscopy = false;
        bool isbltq = false;
        bool isnowzz = false;
        bool isfirstzz = false;
        bool iszstime = false;
        bool iszs = false;
        string temp = "";
        int wg_ff = 0;
        int count = 0;
        int fhbj = 0;
        int count_zm = 0;
        int count_bm = 0;
        int count_hfyw = 0;
        int flagpp = 0; //标记是否有病名推出
        // 全局变量，用于用户添加、修改功能存储信息
        ZdInfo Zd_Edit = new ZdInfo("", "");
        ZzInfo Zz_Edit = new ZzInfo("");
        // 创建集合实例
        ObservableCollection<ZdInfo> listZd = new ObservableCollection<ZdInfo>();
        ObservableCollection<ZzInfo> listZz = new ObservableCollection<ZzInfo>();
        
        string selectIndex = "0";

        public VirtualDiagnose()
        {
            InitializeComponent();
            lb.ItemsSource = listZz;
            lv.ItemsSource = listZd;
        }

        public class ZdInfo : INotifyPropertyChanged
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
            private string _xxLx;
            private string _xxName;

            public string XxLx
            {
                get { return _xxLx; }
                set { _xxLx = value; OnPropertyChanged(new PropertyChangedEventArgs("XxLx")); }
            }

            public string XxName
            {
                get { return _xxName; }
                set { _xxName = value; OnPropertyChanged(new PropertyChangedEventArgs("XxName")); }
            }
            public ZdInfo(string xxlx, string xxname)
            {
                _xxLx = xxlx;
                _xxName = xxname;
            }
        }

        public class ZzInfo : INotifyPropertyChanged
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
            private string _zzname;
            public string ZzName
            {
                get { return _zzname; }
                set { _zzname = value; OnPropertyChanged(new PropertyChangedEventArgs("ZzName")); }
            }

            public ZzInfo(string zzname)
            {
                _zzname = zzname;
            }
        }

        /// <summary>
        /// 功能：退出
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 功能：新建诊断
        /// </summary>
        private void Button_Click_new(object sender, RoutedEventArgs e)
        {
            if (IsAdd == false)
            {
                IsAdd = true;

                // 查找出最大id号，并将其加1赋值给number
                string sql = String.Format("select max(id) from t_bl where ysbh='00000028'");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    number = (Convert.ToInt64(dr[0]) + 1).ToString();
                }
                dr.Close();
                conn.Close();

                // 插入新的病例
                sql = String.Format("Insert into t_bl (blbh,bllx,jsxx,jzsj,ysbh,zt) values ('{0}','3','', GetDate(),'00000028','0')", number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count = comm.ExecuteNonQuery();
                //if (count > 0)
                //{
                //    MessageBox.Show("插入成功");
                //}
                conn.Close();

                // 新建诊断后，病例提取及新建拷贝诊断将不可用
                bltq.IsEnabled = false;
                copyzd.IsEnabled = false;
            }
        }

        /// <summary>
        /// 功能：清除
        /// </summary>
        private void Button_Click_clear(object sender, RoutedEventArgs e)
        {
            listZz.Clear();
            listZd.Clear();
            zz.Text = "";
            bljs.Text = "";
        }

        /// <summary>
        /// 功能：添加现症象
        /// 说明：xxdllx = 0 xxxllx = 2 xxbh --> 现症象编号
        ///       xxdllx = 0 xxxllx = 8 xxbh --> 现症象名称
        /// </summary>
        private void Button_Click_xzz(object sender, RoutedEventArgs e)
        {
            if (IsAdd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (lb.SelectedItem == null)
            {
                MessageBox.Show("没有选中任何信息，请先在症状列表选择框中选择您需要的症状！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ZzInfo Sel_Info = new ZzInfo("");
                Sel_Info = lb.SelectedItem as ZzInfo;

                // 先判断现症状是否重复添加
                string sql = String.Format("select count(*) from t_bl_mx where id = '{0}'and xxdllx = '0' and xxxllx = '8'and xxbh ='{1}' ", number, Sel_Info.ZzName);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    count = Convert.ToInt16(dr[0].ToString());
                }
                dr.Close();
                conn.Close();
                if (count > 0)
                {
                    MessageBox.Show("已添加！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // 显示现症象名称
                    listZd.Add(new ZdInfo("现症象名称", Sel_Info.ZzName));
                    
                    // 根据现症象名称查找现症象编号
                    sql = String.Format("select zxbh from t_info_zxmx where zxmc ='{0}'", Sel_Info.ZzName);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        zxnumber = dr[0].ToString();
                    }
                    dr.Close();
                    conn.Close();

                    // 添加现症象编号
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh)VALUES ('{0}', '{1}', '{2}', '{3}')", number, '0', '2', zxnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count1 = comm.ExecuteNonQuery();
                    //if (count1 > 0)
                    //{
                    //    MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    conn.Close();

                    // 添加现症象名称
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh)VALUES ('{0}', '{1}', '{2}', '{3}')", number, '0', '8', Sel_Info.ZzName);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count2 = comm.ExecuteNonQuery();
                    //if (count2 > 0)
                    //{
                    //    MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    conn.Close();
                }
            }
            listZz.Clear();
            zz.Text = ""; 
        }

        /// <summary>
        /// 功能：添加初期症象
        /// 说明：xxdllx = 0 xxxllx = 1 xxbh --> 初期症象编号
        ///       xxdllx = 0 xxxllx = 7 xxbh --> 初期症象名称
        /// </summary>
        private void Button_Click_cqzz(object sender, RoutedEventArgs e)
        {
            if (IsAdd == false) 
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (lb.SelectedItem == null)
            {
                MessageBox.Show("没有选中任何信息，请先在症状列表选择框中选择您需要的症状！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ZzInfo Sel_Info = new ZzInfo("");
                Sel_Info = lb.SelectedItem as ZzInfo;

                // 先判断初期症象名称是否重复添加
                string sql = String.Format("select count(*) from t_bl_mx where id = '{0}'and xxdllx = '0' and xxxllx = '7'and xxbh ='{1}' ", number, Sel_Info.ZzName);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                int count = (int)comm.ExecuteScalar();
                conn.Close();
                if (count > 0)
                {
                    MessageBox.Show("已添加！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // 显示初期症象名称
                    listZd.Add(new ZdInfo("初期症象名称", Sel_Info.ZzName));

                    // 根据初期症象名称查找初期症象编号
                    sql = String.Format("select zxbh from t_info_zxmx where zxmc ='{0}'", Sel_Info.ZzName);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        zxnumber = dr[0].ToString();
                    }
                    dr.Close();
                    conn.Close();

                    // 添加初期症象编号
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh)VALUES ('{0}', '{1}', '{2}', '{3}')", number, '0', '1', zxnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count1 = comm.ExecuteNonQuery();
                    //if (count1 > 0)
                    //{
                    //    MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    conn.Close();

                    // 添加初期症象名称
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh)VALUES ('{0}', '{1}', '{2}', '{3}')", number, '0', '7', Sel_Info.ZzName);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count2 = comm.ExecuteNonQuery();
                    //if (count2 > 0)
                    //{
                    //    MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    conn.Close();
                }
            }
            zz.Text = "";
            listZz.Clear();
        }

        /// <summary>
        /// 功能：添加主诉时间
        /// 说明：xxdllx = 0 xxxllx = 3 xxbh --> 主诉时间编号
        ///       xxdllx = 0 xxxllx = 9 xxbh --> 主诉时间名称
        /// </summary>
        private void Button_Click_zssj(object sender, RoutedEventArgs e)
        {
            if (IsAdd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (lb.SelectedItem == null)
            {
                MessageBox.Show("没有选中任何信息，请先在症状列表选择框中选择您需要的症状！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ZzInfo Sel_Info = new ZzInfo("");
                Sel_Info = lb.SelectedItem as ZzInfo;

                // 先判断主诉时间名称是否重复添加
                string sql = String.Format("select count(*) from t_bl_mx where id = '{0}'and xxdllx = '0' and xxxllx = '9'and xxbh ='{1}' ", number, Sel_Info.ZzName);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                int count = (int)comm.ExecuteScalar();
                conn.Close();
                if (count > 0)
                {
                    MessageBox.Show("已添加！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // 显示主诉时间名称
                    listZd.Add(new ZdInfo("主诉时间名称", Sel_Info.ZzName));

                    // 根据主诉时间名称查找主诉时间编号
                    sql = String.Format("select zxbh from t_info_zxmx where zxmc ='{0}'", Sel_Info.ZzName);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        zxnumber = dr[0].ToString();
                    }
                    dr.Close();
                    conn.Close();

                    // 添加主诉时间编号
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh)VALUES ('{0}', '{1}', '{2}', '{3}')", number, '0', '3', zxnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count1 = comm.ExecuteNonQuery();
                    //if (count1 > 0)
                    //{
                    //    MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    conn.Close();

                    // 添加主诉时间名称
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh)VALUES ('{0}', '{1}', '{2}', '{3}')", number, '0', '9', Sel_Info.ZzName);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count2 = comm.ExecuteNonQuery();
                    //if (count2 > 0)
                    //{
                    //    MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    conn.Close();
                }
            }
            zz.Text = "";
            listZz.Clear();
        }

        /// <summary>
        /// 功能：添加主诉
        /// 说明：xxdllx = 0 xxxllx = 0 xxbh --> 主诉编号
        ///       xxdllx = 0 xxxllx = 6 xxbh --> 主诉名称
        /// </summary>
        private void Button_Click_zs(object sender, RoutedEventArgs e)
        {
            if (IsAdd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (lb.SelectedItem == null)
            {
                MessageBox.Show("没有选中任何信息，请先在症状列表选择框中选择您需要的症状！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ZzInfo Sel_Info = new ZzInfo("");
                Sel_Info = lb.SelectedItem as ZzInfo;
                // 先检查该主诉名称有没有重复添加，若没有，则进行下一步
                string sql = String.Format("select count(*) from t_bl_mx where id = '{0}'and xxdllx = '0' and xxxllx = '6' and xxbh ='{1}' ", number, Sel_Info.ZzName);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                int count = (int)comm.ExecuteScalar();
                conn.Close();
                if (count == 0)
                {
                    // 将主诉名称显示至列表
                    listZd.Add(new ZdInfo("主诉名称", Sel_Info.ZzName));

                    // 根据主诉名称查找主诉编号
                    sql = String.Format("select zxbh from t_info_zxmx where zxmc ='{0}'", Sel_Info.ZzName);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        zxnumber = dr[0].ToString();
                    }
                    dr.Close();
                    conn.Close();

                    // 添加主诉编号
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh)VALUES ('{0}', '{1}', '{2}', '{3}')", number, '0', '0', zxnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count1 = comm.ExecuteNonQuery();
                    //if (count1 > 0)
                    //{
                    //    MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    conn.Close();

                    // 根据主诉编号删除所对应的现症象编号（两者不能同时存在同一编号，主诉编号为主）
                    sql = String.Format("delete from t_bl_mx  where id = '{0}'and xxdllx = '0' and xxxllx = '2'and xxbh ='{1}'", number, zxnumber);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count2 = comm.ExecuteNonQuery();
                    //if (count2 > 0)
                    //{
                    //    MessageBox.Show("成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    conn.Close();

                    // 添加主诉名称
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh)VALUES ('{0}', '{1}', '{2}', '{3}')", number, '0', '6', Sel_Info.ZzName);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count3 = comm.ExecuteNonQuery();
                    //if (count3 > 0)
                    //{
                    //    MessageBox.Show("成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    conn.Close();

                    // 根据主诉名称删除所对应的现症象名称（两者不能同时存在同一名称，主诉名称为主）
                    sql = String.Format("delete from t_bl_mx  where id = '{0}'and xxdllx = '0' and xxxllx = '8'and xxbh ='{1}'", number, Sel_Info.ZzName);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count4 = comm.ExecuteNonQuery();
                    //if (count4 > 0)
                    //{
                    //    MessageBox.Show("成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    conn.Close();
                }
            }
            zz.Text = "";
            listZz.Clear();
        }

        /// <summary>
        /// 功能：病例提取
        /// </summary>
        private void Button_Click_bltq(object sender, RoutedEventArgs e)
        {
            listZd.Clear();
            IsAdd = true;
            iscopy = false;
            zd.IsEnabled = false;
            ClassiccaseSearch classicalcase = new ClassiccaseSearch();
            classicalcase.Show();
            classicalcase.PassValuesEvent += new ClassiccaseSearch.PassValuesHandler(ReceiveValues);
        }

        /// <summary>
        /// 功能：显示病例信息
        /// 说明：e.Name --> 病例名称
        ///       e.Number --> 病例编号
        /// </summary>
        private void ReceiveValues(object sender, ClassiccaseSearch.PassValuesEventArgs e)
        {
            bljs.Text = e.Name; 
            title = e.Name;     
            number = e.Number;

            // 病例提取有效
            if (e.Name != "")
                isbltq = true;

            // 显示现症象名称 
            string sql = String.Format("select xxbh from t_bl_mx where id = '{0}'and xxdllx = '0' and xxxllx = '8'", number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listZd.Add(new ZdInfo("现症象名称", dr[0].ToString()));
            }
            dr.Close();
            conn.Close();

            // 初期症象名称
            sql = String.Format("select xxbh from t_bl_mx where id = '{0}'and xxdllx = '0' and xxxllx = '7'", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listZd.Add(new ZdInfo("初期症象名称", dr[0].ToString()));
            }
            dr.Close();
            conn.Close();

            // 主诉时间名称
            sql = String.Format("select xxbh from t_bl_mx where id = '{0}'and xxdllx = '0' and xxxllx = '9'", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listZd.Add(new ZdInfo("主诉时间名称", dr[0].ToString()));
            }
            dr.Close();
            conn.Close();

            // 主诉名称
            sql = String.Format("select xxbh from t_bl_mx where id = '{0}'and xxdllx = '0' and xxxllx = '6'", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listZd.Add(new ZdInfo("主诉名称", dr[0].ToString()));
            }
            dr.Close();
            conn.Close();

            // 既往史名称
            sql = String.Format("select xxbh from t_bl_mx where id = '{0}'and xxdllx = '0' and xxxllx = 'a'", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listZd.Add(new ZdInfo("既往史名称", dr[0].ToString()));
            }
            dr.Close();
            conn.Close();

            // 西医病名名称
            sql = String.Format("select xxbh from t_bl_mx where id = '{0}'and xxdllx = '0' and xxxllx = 'b'", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listZd.Add(new ZdInfo("西医病名名称", dr[0].ToString()));
            }
            dr.Close();
            conn.Close();
        }

        private void zz_TextChanged(object sender, TextChangedEventArgs e)
        {    
            if (zz.Text != "")
            {
                listZz.Clear();
                string sql = String.Format("select * from t_info_zxmx where zxmc like '%{0}%' order by zxmc", zz.Text.Trim());
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {

                    listZz.Add(new ZzInfo(dr["zxmc"].ToString()));
                }
                dr.Close();
                conn.Close();
            }
        }

        /// <summary>
        /// 功能：选择病史
        /// </summary>
        private void Button_Click_selectbs(object sender, RoutedEventArgs e)
        {
            if (IsAdd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！");
            }
            else
            {
                selectbs diseaseinfoadmin = new selectbs();
                diseaseinfoadmin.PassValuesEvent += new selectbs.PassValuesHandler(ReceiveValues1);
                diseaseinfoadmin.Show();
            }
        }

        /// <summary>
        /// 功能：添加既往史
        /// </summary>
        private void ReceiveValues1(object sender, selectbs.PassValuesEventArgs e)
        {
            if (IsAdd == true)
            {
                // 先查重
                string sql = String.Format("select count(*) from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '4' and xxbh = '{1}'", number, e.Number);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    count = Convert.ToInt16(dr[0].ToString());
                }
                dr.Close();
                conn.Close();
                if (count > 0)
                {
                    MessageBox.Show("已添加");
                }
                else
                {
                    lv.ItemsSource = listZd;
                    // 显示既往病史名称
                    listZd.Add(new ZdInfo("既往病史名称", e.Name));
                    // 添加既往病史编号
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{0}','0', '4', '{1}')", number, e.Number);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count1 = comm.ExecuteNonQuery();
                    //if (count1 > 0)
                    //{
                    //    MessageBox.Show("插入病史编号成功");
                    //}
                    dr.Close();
                    conn.Close();
                    // 添加既往病史名称
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{0}','0', 'a','{1}')", number, e.Name);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count1 = comm.ExecuteNonQuery();
                    //if (count1 > 0)
                    //{
                    //    MessageBox.Show("插入病史成功");
                    //}
                    dr.Close();
                    conn.Close();
                }
            }
        }
        
        /// <summary>
        /// 功能：添加西医病名
        /// </summary>
        private void Button_Click_xybm(object sender, RoutedEventArgs e)
        {
            if (IsAdd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！");
            }
            else
            {
                XYBMsel diseaseinfoadmin = new XYBMsel();
                diseaseinfoadmin.PassValuesEvent += new XYBMsel.PassValuesHandler(ReceiveValues);
                diseaseinfoadmin.Show();
            }
        }

        /// <summary>
        /// 功能：实现西医病名名称的读取和显示
        /// </summary>
        private void ReceiveValues(object sender, XYBMsel.PassValuesEventArgs e)
        {
            if (IsAdd == true)
            {
                listZd.Add(new ZdInfo("西医病名名称", e.Name));
            }
            else
            {
                // 检查是否重复添加
                string sql = String.Format("select count(*) from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '5' and xxbh = '{1}'", number, e.Number);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    count = Convert.ToInt16(dr[0].ToString());
                }
                dr.Close();
                conn.Close();
                if (count > 0)
                {
                    MessageBox.Show("已添加");
                }
                else
                {
                    lv.ItemsSource = listZd;
                    listZd.Add(new ZdInfo("西医病名名称", e.Name));

                    // 添加西医病名编号
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', '5', '{0}')", e.Number, number);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count1 = comm.ExecuteNonQuery();
                    //if (count1 > 0)
                    //{
                    //    MessageBox.Show("插入西医病名编号成功");
                    //}
                    dr.Close();
                    conn.Close();

                    // 添加西医病名名称
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', 'b','{0}')", e.Name, number);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count1 = comm.ExecuteNonQuery();
                    //if (count1 > 0)
                    //{
                    //    MessageBox.Show("插入西医病名成功");
                    //}
                    dr.Close();
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 功能：诊断
        /// </summary>
        private void Button_Click_zd(object sender, RoutedEventArgs e)
        {
            // 在直接病例提取并诊断的情况下需要利用以下四步将原有诊断结果清空并保留原有诊断输入
            if (isbltq && !iscopy) 
            {
                /// 1.清空病例明细临时表
                String sql = String.Format("delete from	temp_bl_mx");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                int count = comm.ExecuteNonQuery();
                conn.Close();

                /// 2.拷贝原有诊断输入
                sql = String.Format("insert	into temp_bl_mx(id,xxdllx,xxxllx,xxbh) (select id=a.id, xxdllx=a.xxdllx, xxxllx=a.xxxllx, xxbh=a.xxbh from t_bl_mx a where a.id ='{0}' and a.xxdllx = '0')", number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                conn.Close();

                /// 3.清空原有诊断输入及结果
                sql = String.Format("delete from t_bl_mx where id ='{0}'", number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                conn.Close();

                /// 4.拷贝原有诊断输入
                sql = String.Format("insert	into t_bl_mx(id,xxdllx,xxxllx,xxbh) (select id=a.id, xxdllx=a.xxdllx, xxxllx=a.xxxllx, xxbh=a.xxbh from temp_bl_mx a where 	a.id ='{0}')", number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                conn.Close();

                isbltq = false;
            }           

            /// 诊断前必须要输入以下四个信息：
            /// 1.主诉名称 2.主诉时间名称 3.现症象名称
            /// 4.病例检索信息
            for (int i = 0; i < listZd.Count; i++)
            {
                ZdInfo item = listZd.ElementAt(i);
                if (item.XxLx == "现症象名称")
                {
                    isnowzz = true;
                }
                if (item.XxLx == "主诉时间名称")
                {
                    iszstime = true;
                }
                if (item.XxLx == "主诉名称")
                {
                    iszs = true;
                }
            }

            if (bljs.Text == "")
            {
                MessageBox.Show("请输入病历检索信息！");
            }
            else if (IsAdd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！");
            }
            else if (iszs == false)
            {
                MessageBox.Show("请输入病人主诉信息！");
            }
            else if (iszstime == false)
            {
                MessageBox.Show("请输入病人主诉时间信息！");
            }
            else if (isnowzz == false)
            {
                MessageBox.Show("请输入病人现症状信息！");
            }
            else
            {
                // 诊断推理过程
                IsAdd = false;
                isnowzz = false;
                isfirstzz = false;
                iszstime = false;
                iszs = false;

                // 更新检索信息和当前时间
                String sql = String.Format("update t_bl set jsxx ='{0}', jzsj = '{1}' where id='{2}'", bljs.Text, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), number);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                //if (count > 0)
                //{
                //    MessageBox.Show("修改成功！", "操作", MessageBoxButton.OK, MessageBoxImage.Information);
                //}
                conn.Close();
                listZz.Clear();
                zz.Text = "";
                listZd.Clear();
                bljs.Text = "";
                sql = String.Format("exec p_zd_judgewg @id ='{0}' ", number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    MessageBox.Show("调用判断外感/内伤存储过程p_zd_judgewg");
                }
                dr.Close();
                conn.Close();
                sql = String.Format("exec p_zd_jbbj @id = '{0}' ", number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    MessageBox.Show("调用基本病机推理存储过程p_zd_jbbj");
                }
                dr.Close();
                conn.Close();
                sql = String.Format("select xxbh from t_bl_mx where id ='{0}'and xxdllx = '1'and xxxllx = '0'", number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    temp = dr[0].ToString();
                }
                dr.Close();
                conn.Close();
                if (temp == "0")//病名类型为外感，则根据成立的方法调用外感病名推理
                {
                    sql = String.Format("select xxbh from t_bl_mx where id = '{0}' and xxdllx = '1' and xxxllx ='b' order by xxbh desc", number);       //外感推理方法数
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        temp = dr[0].ToString();
                    }
                    dr.Close();
                    conn.Close();
                    wg_ff = Convert.ToInt16(temp);
                    switch (wg_ff)
                    {
                        case 1:
                        case 2:
                            sql = String.Format("exec p_wg_bmtl @id = '{0}', @wg_ff = 1", number);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                MessageBox.Show("调用外感病名推理存储过程p_wg_bmtl");
                            }
                            dr.Close();
                            conn.Close();
                            break;
                        case 3:
                            sql = String.Format("exec p_wg_bmtl @id = '{0}', @wg_ff = 2", number);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                MessageBox.Show("调用外感病名推理存储过程p_wg_bmtl");
                            }
                            dr.Close();
                            conn.Close();
                            break;
                        case 4:
                            sql = String.Format("exec p_wg_bmtl @id = '{0}', @wg_ff = 3", number);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                MessageBox.Show("调用外感病名推理存储过程p_wg_bmtl");
                            }
                            dr.Close();
                            conn.Close();
                            break;
                        case 5:
                            sql = String.Format("exec p_wg_bmtl @id = '{0}', @wg_ff = 4", number);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                MessageBox.Show("调用外感病名推理存储过程p_wg_bmtl");
                            }
                            dr.Close();
                            conn.Close();
                            break;
                        default:
                            break;
                    }
                }
                //如果存在病史，调用病史推理外感病名存储过程p_wgbm_bs
                sql = String.Format("exec p_wgbm_bs @id = '{0}'", number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    MessageBox.Show("病史推理外感病名");
                }
                dr.Close();
                conn.Close();
                sql = String.Format("select xxbh from t_bl_mx where id = '{0}' and xxdllx = '1' and xxxllx ='0'", number);        //再次获取病名类型
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    temp = dr[0].ToString();
                }
                dr.Close();
                conn.Close();
                //病名类型为外感
                if (temp == "0")
                {
                    //调用外感推理子程序wg_tl
                    if (wg_tl())//外感推理有结果时，进外感后续处理
                    {
                        //判断是否有外感证名推出
                        sql = String.Format("select count(*) from t_bl_mx a, t_info_jbzm b where a.id = '{0}'and a.xxdllx = '1' and a.xxxllx = '8' and a.xxbh = b.jbzmbh and b.jbzmlx = '0'", number);  //外感基本证名
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            count = Convert.ToInt16(dr[0].ToString());
                        }
                        dr.Close();
                        conn.Close();
                        //判断是否有复合病机推出
                        sql = String.Format("select count(*) from t_bl_mx a where a.id = '{0}'and a.xxdllx = '1' and a.xxxllx = '7'", number);      //基本证名编号
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            wg_ff = Convert.ToInt16(dr[0].ToString());
                        }
                        dr.Close();
                        conn.Close();
                        if (count == 0)
                        {
                            //如果没有外感证名成立，则近似
                            if (wg_ff == 0)//如果没有复合病机成立，则先近似推理复合病机
                            {
                                //复合病机近似处理
                                sql = String.Format("exec p_fhbj_jstl @id = '{0}'", number);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                dr = comm.ExecuteReader();
                                while (dr.Read())
                                {
                                    MessageBox.Show("复合病机近似处理");
                                }
                                dr.Close();
                                conn.Close();
                                //清除基本证名临时表
                                sql = String.Format("delete from t_ls_jbzm where id = '{0}'", number);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                dr = comm.ExecuteReader();
                                while (dr.Read())
                                {
                                    MessageBox.Show("清除基本证名临时表");
                                }
                                dr.Close();
                                conn.Close();
                                //重新调用外感基本证名推理
                                sql = String.Format("exec p_wg_zm_tl @id = '{0}'", number);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                dr = comm.ExecuteReader();
                                while (dr.Read())
                                {
                                    MessageBox.Show("重新调用外感基本证名推理");
                                }
                                dr.Close();
                                conn.Close();
                                //再次判断是否有外感证名推出
                                sql = String.Format("select count(*) from t_bl_mx a, t_info_jbzm b where a.id = '{0}'and a.xxdllx = '1' and a.xxxllx = '8' and a.xxbh = b.jbzmbh and b.jbzmlx = '0'", number);  //外感基本证名
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                dr = comm.ExecuteReader();
                                while (dr.Read())
                                {
                                    count = Convert.ToInt16(dr[0].ToString());
                                }
                                dr.Close();
                                conn.Close();
                                if (count == 0)//如果外感基本证名还不成立，则调外感基本证名近似推理
                                {
                                    sql = String.Format("exec p_wg_zm_jstl @id = '{0}'", number);  //外感基本证名
                                    conn.Open();
                                    comm = new SqlCommand(sql, conn);
                                    dr = comm.ExecuteReader();
                                    while (dr.Read())
                                    {
                                        MessageBox.Show("外感基本证名近似推理");
                                    }
                                    dr.Close();
                                    conn.Close();
                                    //更新近似度为8
                                    sql = String.Format("update t_bl set zt = ''8'' where id = '{0}'", number);
                                    conn.Open();
                                    comm = new SqlCommand(sql, conn);
                                    int count1 = comm.ExecuteNonQuery();
                                    if (count1 > 0)
                                    {
                                        //MessageBox.Show("更新近似度为8");
                                    }
                                    dr.Close();
                                    conn.Close();
                                }
                            }
                            else//如果有复合病机成立，则调外感基本证名近似推理
                            {
                                sql = String.Format("exec p_wg_zm_jstl @id = '{0}'", number);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                dr = comm.ExecuteReader();
                                while (dr.Read())
                                {
                                    MessageBox.Show("外感基本证名近似推理");
                                }
                                dr.Close();
                                conn.Close();
                            }
                        }
                        wg_hstl();//外感后续推理过程
                    }
                    //----------------------------外感推理全部结束--------------------------
                    else//外感推理没有结果时，进内伤推理，进了内伤后就不出内伤了
                        program_ns();
                }
                else if (temp == "1")//病名类型为内伤，调用内伤推理子程序
                {
                    program_ns();
                }
                else
                    MessageBox.Show("病名类型推理失败，请检查！");
                ////将服法添加到病历明细表
                //sql = String.Format("insert t_bl_mx (id,xxdllx,xxxllx,xxbh) select	id = '{0}',xxdllx = '2',xxxllx = '5', xxbh = a.ff from t_info_jbcfxx a, t_bl_mx b where b.id = '{0}'and b.xxdllx = '2' and b.xxxllx = '4' and a.jbcfbh = b.xxbh", number);
                //conn.Open();
                //comm = new SqlCommand(sql, conn);
                //int count2 = comm.ExecuteNonQuery();
                //if (count2 > 0)
                //{

                //    MessageBox.Show("诊断完毕");
                //}
                //dr.Close();
                //conn.Close();

                ////显示结果
                ////更新病历类型
                ////sql = String.Format("update t_bl set bllx='0' where id='{0}'", number);
                ////conn.Open();
                ////comm = new SqlCommand(sql, conn);
                ////int count3 = comm.ExecuteNonQuery();
                ////if (count3 > 0)
                ////{
                ////    MessageBox.Show("更新成功");
                ////}
                ////dr.Close();
                ////conn.Close();
                //display_bl result = new display_bl(number, oldnumber);
                //result.Show();           
            }
        }

        /// <summary>
        /// 功能：外感后续推理过程
        /// </summary>
        public void wg_hstl()
        {
            //调用证名合并推理子过程
            string sql = String.Format("exec p_zmhb @id = '{0}'", number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("证名合并推理子过程");
            }
            dr.Close();
            conn.Close();
            //调用根据已推出外感病名来选择基本证名的子过程
            sql = String.Format("exec p_wg_bm_zm_tl @id = '{0}'", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("外感病名来选择基本证名");
            }
            dr.Close();
            conn.Close();
            //调用处方推理子过程
            sql = String.Format("exec p_zd_cftl @id = '{0}'", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("处方推理子过程");
            }
            dr.Close();
            conn.Close();
            //互反药物选择
            sql = String.Format("select count(*) from temp_fywcl");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                count_hfyw = Convert.ToInt16(dr[0].ToString());
            }
            dr.Close();
            conn.Close();
            if (count_hfyw > 0)
            {
                // 反药物处理方式选择
                display_fywclxz displayFywclxz = new display_fywclxz();
                displayFywclxz.PassValuesEvent += new display_fywclxz.PassValuesHandler(ReceiveValues);
                displayFywclxz.Show();
            }
            else
            {
                // MessageBox.Show("不存在互反药物！");
                //将服法添加到病历明细表
                sql = String.Format("insert t_bl_mx (id,xxdllx,xxxllx,xxbh) select	id = '{0}',xxdllx = '2',xxxllx = '5', xxbh = a.ff from t_info_jbcfxx a, t_bl_mx b where b.id = '{0}'and b.xxdllx = '2' and b.xxxllx = '4' and a.jbcfbh = b.xxbh", number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count2 = comm.ExecuteNonQuery();
                if (count2 > 0)
                {
                    MessageBox.Show("诊断完毕");
                }
                dr.Close();
                conn.Close();

                //显示结果
                //更新病历类型
                //sql = String.Format("update t_bl set bllx='0' where id='{0}'", number);
                //conn.Open();
                //comm = new SqlCommand(sql, conn);
                //int count3 = comm.ExecuteNonQuery();
                //if (count3 > 0)
                //{
                //    MessageBox.Show("更新成功");
                //}
                //dr.Close();
                //conn.Close();
                display_bl result = new display_bl(number, oldnumber);
                result.Show();
            }
        }

        /// <summary>
        /// 功能：接收传递的选择项
        /// "0":未选择 "1":人工选择  "2":系统自动处理  "3":全部保留
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveValues(object sender, display_fywclxz.PassValuesEventArgs e)
        {
            selectIndex = e.Number;
            String sql = "";
            SqlCommand comm;
            if (e.Number == "1") // 人工处理
            {
                display_fywcl result = new display_fywcl(number);                        
            }
            if (e.Number == "2") // 系统自动处理
            { 
                sql = String.Format("delete	t_zd_ywzc from t_zd_ywzc a, temp_fywcl b where a.id = '{0}' and a.ywbh = b.hfywbh", number);                
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count = comm.ExecuteNonQuery();
                conn.Close();
            }
            if (e.Number == "3") // 全部保留
            { 
                // 不做任何处理
            }
            //将服法添加到病历明细表
            sql = String.Format("insert t_bl_mx (id,xxdllx,xxxllx,xxbh) select	id = '{0}',xxdllx = '2',xxxllx = '5', xxbh = a.ff from t_info_jbcfxx a, t_bl_mx b where b.id = '{0}'and b.xxdllx = '2' and b.xxxllx = '4' and a.jbcfbh = b.xxbh", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            int count2 = comm.ExecuteNonQuery();
            if (count2 > 0)
            {
                MessageBox.Show("诊断完毕");
            }
            conn.Close();

            //显示结果
            //更新病历类型
            //sql = String.Format("update t_bl set bllx='0' where id='{0}'", number);
            //conn.Open();
            //comm = new SqlCommand(sql, conn);
            //int count3 = comm.ExecuteNonQuery();
            //if (count3 > 0)
            //{
            //    MessageBox.Show("更新成功");
            //}
            //dr.Close();
            //conn.Close();
            display_bl result1 = new display_bl(number, oldnumber);
            result1.Show();
        }

        /// <summary>
        /// 功能：外感推理子过程wg_tl
        /// </summary>
        public bool wg_tl()
        {
            //调用系推理存储过程（根据症象推理）p_x_zx
            string sql = String.Format("exec p_x_zx @id = '{0}'", number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("根据症象推理");
            }
            dr.Close();
            conn.Close();
            //调用复合病机推理存储过程p_zd_fhbj
            sql = String.Format("exec p_zd_fhbj @id = '{0}'", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("复合病机推理存储过程p_zd_fhbj");
            }
            dr.Close();
            conn.Close();
            //调用外感基本证名推理存储过程
            sql = String.Format("exec p_wg_zm_tl @id = '{0}'", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("外感基本证名推理存储过程");
            }
            dr.Close();
            conn.Close();
            //判断外感病名、外感证名是否推出，推出则返回true，否则返回false
            sql = String.Format("select count(*) from t_bl_mx where id = '{0}' and xxdllx = '1' and xxxllx = '4'", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                count_bm = Convert.ToInt16(dr[0].ToString());
            }
            dr.Close();
            conn.Close();
            sql = String.Format("select count(*) from t_bl_mx a, t_info_jbzm b where a.id = '{0} 'and a.xxdllx = '1' and a.xxxllx = '8' and a.xxbh = b.jbzmbh and b.jbzmlx ='0'", number);  //外感基本证名
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                count_zm = Convert.ToInt16(dr[0].ToString());
            }
            dr.Close();
            conn.Close();
            if (count_bm <= 0 && count_zm <= 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 功能：内伤推理总程序，包含ns_tl和ns_hstl的处理
        /// </summary>
        public void program_ns() 
        {
            if (ns_tl())//内伤推理有结果时，进内伤后续处理
            {
                ns_hstl();
            }
            else//内伤证名没有结果时，进内伤近似处理
            {
                //判断是否有证名推出
                string sql = String.Format("select count(*) from t_bl_mx a, t_info_jbzm b where a.id = '{0}'and a.xxdllx = '1' and a.xxxllx = '8'and a.xxbh = b.jbzmbh and b.jbzmlx = '1'", number);  //内伤基本证名
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    count = Convert.ToInt16(dr[0].ToString());
                }
                dr.Close();
                conn.Close();
                sql = String.Format("select count(*) from t_bl_mx a where a.id ='{0}' and a.xxdllx = '1' and a.xxxllx = '7'", number);           //复合病机编号
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    fhbj = Convert.ToInt16(dr[0].ToString());
                }
                dr.Close();
                conn.Close();
                if (count == 0)//如果没有证名成立，则近似
                {
                    if (fhbj == 0)//如果没有复合病机成立，则先近似推理复合病机
                    {
                        sql = String.Format("exec p_fhbj_jstl @id = '{0}'", number);           ////复合病机近似处理
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            MessageBox.Show("复合病机近似处理");
                        }
                        dr.Close();
                        conn.Close();
                        sql = String.Format("delete from t_ls_jbzm where id = '{0}'", number);           //清除基本证名临时表
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        int count4 = comm.ExecuteNonQuery();
                        if (count4 > 0)
                        {
                            MessageBox.Show("清除基本证名临时表");
                        }
                        dr.Close();
                        conn.Close();
                        sql = String.Format("exec p_ns_zm_tl @id =  '{0}'", number);           //重新调用基本证名推理
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            MessageBox.Show("重新调用基本证名推理");
                        }
                        dr.Close();
                        conn.Close();
                        sql = String.Format("exec p_ns_zm_tl @id =  '{0}'", number);           //重新调用基本证名推理
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            MessageBox.Show("重新调用基本证名推理");
                        }
                        dr.Close();
                        conn.Close();
                        sql = String.Format("select count(*) from t_bl_mx a, t_info_jbzm b where a.id = '{0}' and a.xxdllx = '1' and a.xxxllx = '8' and a.xxbh = b.jbzmbh and b.jbzmlx = '1'", number);  //内伤基本证名//再次判断是否有证名推出
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            count = Convert.ToInt16(dr[0].ToString());
                        }
                        dr.Close();
                        conn.Close();
                        if (count == 0)//如果ns基本证名还不成立，则调ns基本证名近似推理
                        {
                            sql = String.Format("exec p_ns_zm_jstl @id =  '{0}'", number);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                MessageBox.Show("基本证名近似推理");
                            }
                            dr.Close();
                            conn.Close();
                            sql = String.Format("update t_bl set zt = '8' where id = '{0}'", number);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            int count1 = comm.ExecuteNonQuery();
                            if (count1 > 0)
                            {
                                MessageBox.Show("更新近似度为8");
                            }
                            dr.Close();
                            conn.Close();
                        }
                    }
                    else        //如果有复合病机成立，则调ns基本证名近似推理
                    {
                        sql = String.Format("exec p_ns_zm_jstl @id =  '{0}'", number);
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            MessageBox.Show("基本证名近似推理");
                        }
                        dr.Close();
                        conn.Close();
                    }
                }
                //调用内伤后续处理过程
                ns_hstl();
            }
        }

        /// <summary>
        /// 功能：内伤推理子过程，如果有结果推出，返回true，否则返回false
        /// </summary>
        public bool ns_tl()
        {//调用内伤系推理存储过程（根据主诉推理）
            String sql = String.Format("exec p_x_zs @id =  '{0}'", number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("主诉推理");
            }
            dr.Close();
            conn.Close();
            sql = String.Format("exec p_x_zx @id =  '{0}'", number);//调用内伤系推理存储过程（根据症象推理）
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("症象推理");
            }
            dr.Close();
            conn.Close();
            //调用内伤病名推理子存储过程
            sql = String.Format("exec p_ns_jlbm @id =  '{0}'", number);//甲类病名推理
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("甲类病名推理");
            }
            dr.Close();
            conn.Close();
            sql = String.Format("exec p_ns_ylbm @id =  '{0}'", number);//乙类病名推理
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("乙类病名推理");
            }
            dr.Close();
            conn.Close();
            sql = String.Format("exec p_zd_fhbj @id =  '{0}'", number);//调用复合病机推理存储过程
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("调用复合病机推理存储过程");
            }
            dr.Close();
            conn.Close();
            sql = String.Format("exec p_ns_zm_tl @id =  '{0}'", number);//调用内伤证名推理存储过程
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("调用内伤证名推理存储过程");
            }
            dr.Close();
            conn.Close();
            sql = String.Format("exec p_ns_zm_tl @id =  '{0}'", number);//调用内伤证名推理存储过程
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("调用内伤证名推理存储过程");
            }
            dr.Close();
            conn.Close();
            //判断内伤病名、内伤证名是否推出，推出则返回true，否则返回false
            sql = String.Format("select count(*) from t_bl_mx a, t_info_jbzm b where a.id = '{0}'  and a.xxdllx = '1' and a.xxxllx = '8' and a.xxbh = b.jbzmbh  and b.jbzmlx = '1'", number);  //内伤基本证名
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                count_zm = Convert.ToInt16(dr[0].ToString());
            }
            dr.Close();
            conn.Close();
            if (count_zm > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 功能：内伤后续推理
        /// </summary>
        public void ns_hstl()
        {
            //调用证名合并推理子过程
            String sql = String.Format("exec p_zmhb @id =  '{0}'", number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("证名合并推理");
            }
            dr.Close();
            conn.Close();
            //调用根据已推出内伤病名来选择基本证名的子过程
            sql = String.Format("exec p_ns_bm_zm_tl @id =  '{0}'", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("内伤病名来选择基本证名");
            }
            dr.Close();
            conn.Close();
            sql = String.Format("exec pp @id =  '{0}'", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("内伤病名来选择基本证名");
            }
            dr.Close();
            conn.Close();
            /******************************************/
            sql = String.Format("select xxbh from t_bl_mx where id ='{0}' and xxdllx = '1' and xxxllx in ('5','6')", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                flagpp = 1;
            }
            dr.Close();
            conn.Close();
            if (flagpp == 1)
            {
                sql = String.Format("exec pp @id =  '{0}'", number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    MessageBox.Show("内伤病名来选择基本证名");
                }
                dr.Close();
                conn.Close();
            }
            /******************************************/
            //调用处方推理子过程
            sql = String.Format("exec p_zd_cftl @id =  '{0}'", number);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("处方推理");
            }
            dr.Close();
            conn.Close();
            //互反药物选择
            sql = String.Format("select count(*) from temp_fywcl");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                count_hfyw = Convert.ToInt16(dr[0].ToString());
            }
            dr.Close();
            conn.Close();
            if (count_hfyw > 0)
            {
                // 反药物处理方式选择
                display_fywclxz displayFywclxz = new display_fywclxz();               
                displayFywclxz.PassValuesEvent += new display_fywclxz.PassValuesHandler(ReceiveValues);
                displayFywclxz.Show();
            }
            else
            {
                // MessageBox.Show("不存在互反药物！");
                //将服法添加到病历明细表
                sql = String.Format("insert t_bl_mx (id,xxdllx,xxxllx,xxbh) select	id = '{0}',xxdllx = '2',xxxllx = '5', xxbh = a.ff from t_info_jbcfxx a, t_bl_mx b where b.id = '{0}'and b.xxdllx = '2' and b.xxxllx = '4' and a.jbcfbh = b.xxbh", number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count2 = comm.ExecuteNonQuery();
                //if (count2 > 0)
                //{

                //    MessageBox.Show("诊断完毕");
                //}
                dr.Close();
                conn.Close();

                //显示结果
                //更新病历类型
                //sql = String.Format("update t_bl set bllx='0' where id='{0}'", number);
                //conn.Open();
                //comm = new SqlCommand(sql, conn);
                //int count3 = comm.ExecuteNonQuery();
                //if (count3 > 0)
                //{
                //    MessageBox.Show("更新成功");
                //}
                //dr.Close();
                //conn.Close();
                display_bl result = new display_bl(number, oldnumber);
                result.Show();
            }
        }

        /// <summary>
        /// 功能：新建拷贝诊断
        /// </summary>
        private void Button_Click_copyzd(object sender, RoutedEventArgs e)
        {
            if (bljs.Text == "")
            {
                MessageBox.Show("请先提取病历！");
            }
            else
            {
                iscopy = true;
                oldnumber = number;
                bljs.Text = "西医诊断—" + bljs.Text;

                // 插入新病例
                string sql = String.Format("Insert into t_bl (bllx,jsxx,jzsj,ysbh,zt) values ('3','{0}', GetDate(),'00000028','0')", bljs.Text);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                int count = comm.ExecuteNonQuery();
                //if (count > 0)
                //{
                //    MessageBox.Show("插入成功");
                //}
                conn.Close();

                // 查找当前最大id号并赋值给新的病例编号
                sql = String.Format("select max(id) from t_bl where ysbh='00000028'");
                conn.Open();
                comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    number = (Convert.ToInt64(dr[0])).ToString();
                }
                dr.Close();
                conn.Close();

                // 更新病例表中的病例编号
                sql = String.Format("update t_bl set blbh ='{0}' where id ='{1}'", number, number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                conn.Close();

                // 将旧的病历表中的输入信息复制给新的病例表（只复制诊断输入不复制诊断结果）
                sql = String.Format("exec p_copybl @source_id ='{0}',@object_id ='{1}'", oldnumber, number);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// 功能：删除
        /// </summary>
        private void Button_Click_delete(object sender, RoutedEventArgs e)
        {
            if (IsAdd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！");
            }
            else if (lv.SelectedIndex == -1)
            {
                MessageBox.Show("没有选中任何信息，请先在信息列表选择框中选中您需要删除的项！");
            }
            else
            {
                ZdInfo Sel_Info = new ZdInfo("", "");
                Sel_Info = lv.SelectedItem as ZdInfo;
                if (Sel_Info.XxLx == "西医病名名称")
                    xlbh = "b";
                if (Sel_Info.XxLx == "既往史名称")
                    xlbh = "a";
                if (Sel_Info.XxLx == "主诉名称")
                    xlbh = "6";
                if (Sel_Info.XxLx == "主诉时间名称")
                    xlbh = "9";
                if (Sel_Info.XxLx == "初起症象名称")
                    xlbh = "7";
                if (Sel_Info.XxLx == "现症象名称")
                    xlbh = "8";
                string sql = String.Format("select subid from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '{1}' and xxbh = '{2}'", number, xlbh, Sel_Info.XxName);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    subid = dr["subid"].ToString();
                }
                dr.Close();
                conn.Close();
                if (Sel_Info.XxLx.Trim() == "西医病名名称")
                {
                    sql = String.Format("select xybmbh from t_info_xybm where xybmmc ='{0}'", Sel_Info.XxName);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        bh = dr["xybmbh"].ToString();
                    }
                    dr.Close();
                    conn.Close();
                }
                else
                {
                    sql = String.Format("select zxbh from t_info_zxmx where zxmc ='{0}'", Sel_Info.XxName);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        bh = dr["zxbh"].ToString();
                    }
                    dr.Close();
                    conn.Close();
                }
                //删除当前指定记录(名称)
                sql = String.Format("delete from t_bl_mx where subid ='{0}'", subid);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count1 = comm.ExecuteNonQuery();
                if (count1 > 0)
                {
                    MessageBox.Show("删除名称成功");
                }
                dr.Close();
                conn.Close();
                //再删除编号
                if (xlbh == "6")
                    xlbh = "0"; //主诉
                else if (xlbh == "7")
                    xlbh = "1";
                else if (xlbh == "8")
                    xlbh = "2";
                else if (xlbh == "9")
                    xlbh = "3";
                else if (xlbh == "a")
                    xlbh = "4";
                else if (xlbh == "b")
                    xlbh = "5";
                sql = String.Format("delete from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '{1}' and xxbh = '{2}'", number, xlbh, bh);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count1 = comm.ExecuteNonQuery();
                if (count1 > 0)
                {
                    MessageBox.Show("删除编号成功");
                }
                dr.Close();
                conn.Close();
                listZd.RemoveAt(lv.SelectedIndex);
            }
        }
    }
}

