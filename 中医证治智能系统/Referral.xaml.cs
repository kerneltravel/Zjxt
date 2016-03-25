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
    /// Interaction logic for Referral.xaml
    /// </summary>
    public partial class Referral : Window
    {
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 用于判断是否进行了“新建诊断”操作
        bool IsAdd = false;
        // 此次就诊id
        string mid = "";
        // 上次就诊id
        string lid = "";
        // 病历编号
        string id = "-1";
        // 新增症象数 
        int xzzxs = 0;
        // 愈合症象数
        int yhzxs = 0;
        // 能否删除
        bool nengshan1;
        // 能否删除
        bool nengshan2;
        int ccount = 0;
        string subid = "";
        string xlbh = "";
        string mc = "";
        string bh = "";
        // 全局变量，用于用户添加、修改功能存储信息
        PatientDisease Patient_Edit = new PatientDisease("", "");
        // 创建集合实例
        ObservableCollection<PatientDisease> listPatient1 = new ObservableCollection<PatientDisease>();
        ObservableCollection<PatientDisease> listPatient2 = new ObservableCollection<PatientDisease>();

        /// <summary>
        /// 功能：定义病人症象信息类
        /// </summary>
        public class PatientDisease : INotifyPropertyChanged
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
            private string _xxlx;
            private string _xxmc;

            public string Xxlx
            {
                get { return _xxlx; }
                set { _xxlx = value; OnPropertyChanged(new PropertyChangedEventArgs("Xxlx")); }
            }

            public string Xxmc
            {
                get { return _xxmc; }
                set { _xxmc = value; OnPropertyChanged(new PropertyChangedEventArgs("Xxmc")); }
            }

            public PatientDisease(string xxlx, string xxmc)
            {
                _xxlx = xxlx;
                _xxmc = xxmc;
            }
        }

        /// <summary>
        /// 功能：初始化
        /// </summary>
        public Referral()
        {
            InitializeComponent();
            // 文本框初始化
            brbh.IsEnabled = false;
            name.IsEnabled = false;
            comb_lx.SelectedIndex = 0;
            // 指定数据源
            lv_1.ItemsSource = listPatient1;
            lv_2.ItemsSource = listPatient2;
        }



        /// <summary>
        /// 功能：返回
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //string sql = String.Format("select * from t_br_info where brid= '{0}'", e.brNumber);
            //conn.Open();
            //SqlCommand comm = new SqlCommand(sql, conn);
            //SqlDataReader dr = comm.ExecuteReader();
            //while (dr.Read())
            //{
            //    name.Text = dr["xm"].ToString();
            //}
            //dr.Close();
            //conn.Close();

            this.Close();
        }

        /// <summary>
        /// 功能：【新建诊断】
        /// </summary>
        private void btn_xjzd_Click(object sender, RoutedEventArgs e)
        {
            IsAdd = true;
            // 获取当前时间
            string time = DateTime.Now.Date.ToString().Substring(0, 10) + " " + DateTime.Now.TimeOfDay.ToString().Substring(0, 12);
            // 删除临时案例
            string sql = String.Format("delete from t_bl where bllx = '4' and ysbh = '00000028'");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = comm.ExecuteNonQuery();
            conn.Close();
            // 主表添加一条记录，打开病历明细表，并且返回主表id号
            sql = String.Format("Insert into t_bl (bllx,jsxx,jzsj,ysbh,zt) values ('4', '', getdate(), '00000028' ,'0')");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            count = comm.ExecuteNonQuery();
            conn.Close();

            sql = String.Format("select max(id) from t_bl where ysbh = '00000028'");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                id = dr[0].ToString();
            }
            dr.Close();
            conn.Close();

            sql = String.Format("update t_bl set blbh = id where id = id");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            count = comm.ExecuteNonQuery();
            conn.Close();

            mid = id;
            xzzxs = 0;
        }

        /// <summary>
        /// 功能：【病例提取】
        /// </summary>
        private void btn_bltq_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdd)
            {
                ClinicalcaseSearch clinicalcasesearch = new ClinicalcaseSearch();
                clinicalcasesearch.Show();
                clinicalcasesearch.PassValuesEvent += new ClinicalcaseSearch.PassValuesHandler(ReceiveValues);

                // 清空临时表
                string sql = String.Format("delete t_ls_fz where id = '{0}'", mid);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                int count = comm.ExecuteNonQuery();
                conn.Close();
                sql = String.Format("delete t_ls_qy where id = '{0}'", mid);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count = comm.ExecuteNonQuery();
                conn.Close();

            }
            else
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 功能：【选定】
        /// </summary>
        private void ReceiveValues(object sender, ClinicalcaseSearch.PassValuesEventArgs e)
        {
            // 显示病人编号
            brbh.Text = e.brNumber.ToString();

            id = e.blNumber.ToString();
            lid = id;
            // 显示姓名
            string sql = String.Format("select * from t_br_info where brid= '{0}'", e.brNumber);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                name.Text = dr["xm"].ToString();
            }
            dr.Close();
            conn.Close();
            
            

            
            // 将病历中的症象列入临时表
            sql = String.Format("insert into t_ls_fz (id,xxdllx,xxxllx,xxbh) select id = '{0}' , xxdllx, xxxllx, xxbh from t_bl_mx a where a.id = '{1}'", mid, lid);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            int count = comm.ExecuteNonQuery();
            conn.Close();
            // 将药物添加到药物组成表
            lid = id;
            sql = String.Format("insert into t_zd_ywzc (id,ywbh,ywjl,dw) select id = '{0}' , ywbh,ywjl,dw from t_zd_ywzc a where a.id = '{1}'", mid, lid);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            count = comm.ExecuteNonQuery();
            conn.Close();
            id = mid;
            yhzxs = 0;
            //---------------病历结果显示---------------
            sql = String.Format("select t2.bz from t_br_info as t1 inner join t_bl as t2 on t2.jsxx = t1.brid where t1.brid= '{0}' and t2.blbh = '{1}'"
                , e.brNumber, e.blNumber);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                blxs.Text = dr[0].ToString();
            }
            dr.Close();
            conn.Close();
            // 病人的症象显示
            // 说明：1.主诉名称 --> xxdllx = '0' and xxxllx = '6'
            //       2.初起症象 --> xxdllx = '0' and xxxllx = '7'
            //       3.现症象名 --> xxdllx = '0' and xxxllx = '8'
            //       4.主诉时间 --> xxdllx = '0' and xxxllx = '9'
            //       5.既往史名 --> xxdllx = '0' and xxxllx = 'a'
            //       6.西医病名 --> xxdllx = '0' and xxxllx = 'b'
            // 清空
            listPatient1.Clear();
            // 主诉名称
            sql = String.Format("select xxbh from t_bl_mx where id='{0}' and xxdllx = '0' and xxxllx = '6'", e.blNumber);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listPatient1.Add(new PatientDisease("上次就诊时的主诉名称", dr[0].ToString()));
            }
            dr.Close();
            conn.Close();
            // 初起症象
            sql = String.Format("select xxbh from t_bl_mx where id='{0}' and xxdllx = '0' and xxxllx = '7'", e.blNumber);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listPatient1.Add(new PatientDisease("上次就诊时的初起症象", dr[0].ToString()));
            }
            dr.Close();
            conn.Close();
            // 现症象名
            sql = String.Format("select xxbh from t_bl_mx where id='{0}' and xxdllx = '0' and xxxllx = '8'", e.blNumber);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listPatient1.Add(new PatientDisease("上次就诊时的现症象名", dr[0].ToString()));
            }
            dr.Close();
            conn.Close();
            // 主诉时间
            sql = String.Format("select xxbh from t_bl_mx where id='{0}' and xxdllx = '0' and xxxllx = '9'", e.blNumber);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listPatient1.Add(new PatientDisease("上次就诊时的主诉时间", dr[0].ToString()));
            }
            dr.Close();
            conn.Close();
            // 既往史名
            sql = String.Format("select xxbh from t_bl_mx where id='{0}' and xxdllx = '0' and xxxllx = 'a'", e.blNumber);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listPatient1.Add(new PatientDisease("上次就诊时的既往史名", dr[0].ToString()));
            }
            dr.Close();
            conn.Close();
            // 西医病名
            sql = String.Format("select xxbh from t_bl_mx where id='{0}' and xxdllx = '0' and xxxllx = 'b'", e.blNumber);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listPatient1.Add(new PatientDisease("上次就诊时的西医病名", dr[0].ToString()));
            }
            dr.Close();
            conn.Close();
            ////---------------病历结果显示---------------
            //blxs.Text = "";
            //blxs.Text = "病历编号：";
            //blxs.Text += e.blNumber.ToString() + "\n";
            //blxs.Text += "-------------------------------------------------------------------------\n";
            //// 显示姓名
            //sql = String.Format("select * from t_br_info as t1 inner join t_bl as t2 on t2.jsxx = t1.brid where brid= '{0}'", e.brNumber);
            //conn.Open();
            //comm = new SqlCommand(sql, conn);
            //dr = comm.ExecuteReader();
            //while (dr.Read())
            //{
            //    blxs.Text += string.Format("姓名：{0}    性别：{1}    年龄：{2}    就诊时间：{3}"
            //        , dr["xm"].ToString(), dr["xb"].ToString(), dr["nl"].ToString(), dr["jzsj"].ToString());
            //}
            //dr.Close();
            //conn.Close();

        }

        /// <summary>
        /// 功能：文本变化时触发事件
        /// </summary>
        private void NewDisease_TextChanged(object sender, TextChangedEventArgs e)
        {
            listbox.Items.Clear();
            string sql = String.Format("select * from t_info_zxmx where zxmc like '%{0}%'", NewDisease.Text);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                listbox.Items.Add(dr["zxmc"].ToString());
            }
            dr.Close();
            conn.Close();
            // 若新症象输入为空，则清空列表
            if (NewDisease.Text == "")
                listbox.Items.Clear();
        }

        /// <summary>
        /// 功能：【添加到病人的症象】
        /// </summary>
        private void Disease_Add_Click(object sender, RoutedEventArgs e)
        {
            string bh = "";
            string mc = listbox.SelectedItem.ToString();
            if (IsAdd) // 判断病历明细表是否打开
            {
                if (id == "-1") // 判断主表是否处于新增状态
                {
                    MessageBox.Show("未处于新增诊断状态，请先新建诊断！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (listbox.SelectedIndex == -1)
                    {
                        MessageBox.Show("没有选中任何消息，请先在症状列表选择框中选中您需要的症状！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        //判断是否重复输入
                        string sql = String.Format("select count(*) from t_bl_mx where id = '{0}' and xxbh = '{1}'", lid, mc);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        SqlDataReader dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            ccount = Convert.ToInt16(dr[0].ToString());
                        }
                        dr.Close();
                        conn.Close();
                        if (ccount > 0)
                        {
                            MessageBox.Show("这是上次就诊时的症象！");
                        }
                        else
                        {

                            sql = String.Format("select count(*) from t_bl_mx where id = '{0}' and xxbh = '{1}'", mid, mc);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                ccount = Convert.ToInt16(dr[0].ToString());
                            }
                            dr.Close();
                            conn.Close();
                            if (ccount > 0)
                            {
                                MessageBox.Show("已添加");
                            }
                            else
                            {
                                listPatient2.Add(new PatientDisease("现症象名称", listbox.SelectedItem.ToString()));
                                NewDisease.Text = "";
                                // 添加录入信息
                                sql = String.Format("select zxbh from t_info_zxmx where zxmc = '{0}'", mc);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                dr = comm.ExecuteReader();
                                while (dr.Read())
                                {
                                    bh = dr[0].ToString();
                                }
                                dr.Close();
                                conn.Close();

                                sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ( '{0}', '0', '2', '{1}')", Convert.ToInt32(mid), Convert.ToInt32(bh));
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                int count = comm.ExecuteNonQuery();
                                conn.Close();

                                sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ( '{0}', '0', '8', '{1}')", mid.ToString(), mc.ToString());
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                count = comm.ExecuteNonQuery();
                                conn.Close();

                                xzzxs = xzzxs + 1;
                            }

                        }

                    }
                }
            }
            else
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                NewDisease.Text = "";
            }
        }

        /// <summary>
        /// 功能：【删除错误添加的新症象】
        /// </summary>
        private void Disease_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！");
            }
            else
            {
                PatientDisease patientinfo = lv_2.SelectedItem as PatientDisease;
                if (patientinfo != null && patientinfo is PatientDisease)
                {
                    if (MessageBox.Show(string.Format("您确定症象：" + "\"" + "{0}" + "\"" + "是错误添加的吗？", patientinfo.Xxmc.ToString()), "确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        listPatient2.Remove(patientinfo);
                        xlbh = "8";
                        string sql = String.Format("select subid from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '{1}' and xxbh = '{2}'", mid, xlbh, patientinfo.Xxmc);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        SqlDataReader dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            subid = dr["subid"].ToString();

                        }
                        dr.Close();
                        conn.Close();
                        sql = String.Format("select zxbh from t_info_zxmx where zxmc ='{0}'", patientinfo.Xxmc);
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            bh = dr["zxbh"].ToString();
                        }
                        dr.Close();
                        conn.Close();
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
                        xlbh = "2";
                        sql = String.Format("delete from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '{1}' and xxbh = '{2}'", mid, xlbh, bh);
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        count1 = comm.ExecuteNonQuery();
                        if (count1 > 0)
                        {
                            MessageBox.Show("删除编号成功");
                        }
                        dr.Close();
                        conn.Close();
                        xzzxs--;
                    }
                    nengshan2 = false;
                }
                else
                {
                    MessageBox.Show("请用鼠标点选添加错误的症象！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }

        /// <summary>
        /// 功能：诊断
        /// </summary>
        private void btn_zd_Click(object sender, RoutedEventArgs e)
        {
            if (brbh.Text == "")
            {
                MessageBox.Show("请选择病人！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (!IsAdd)
                {
                    MessageBox.Show("表未处于打开状态，请先新建诊断！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (id == "-1")
                    {
                        MessageBox.Show("未处于新增诊断状态，请先新建诊断！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        if (comb_lx.SelectedIndex == 0)
                        {
                            MessageBox.Show("没有选择疗效，请先选择服药后的疗效！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            string m = "0";
                            string sql = String.Format("select count(*) from t_ls_fz where xxdllx = '0' and xxxllx = '6' and id = '{0}'", mid);
                            conn.Open();
                            SqlCommand comm = new SqlCommand(sql, conn);
                            SqlDataReader dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                m = dr[0].ToString();
                            }
                            dr.Close();
                            conn.Close();
                            if (m == "0")
                            {
                                MessageBox.Show("主诉已经改变，建议您重新诊断！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                // 更新状态标志
                                sql = String.Format("update t_bl set zt = '0' where id = '{0}'", mid);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                int count = comm.ExecuteNonQuery();
                                conn.Close();

                                // 更新检索标题信息
                                sql = String.Format("update t_bl set jsxx = '{0}' where id = '{1}'", brbh.Text.ToString(), mid);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                count = comm.ExecuteNonQuery();
                                conn.Close();

                                //----------------------------------------开始复诊 ---------------------------------------
                                //comb_lx.ItemIndex --> 疗效      yhzxs --> 愈合症象数     xzzxs --> 新增症象数
                                if (yhzxs > 0)
                                {
                                    // 先进行对证药物删除
                                    sql = String.Format("exec p_fz_dzywsc @id = '{0}'", mid);
                                    conn.Open();
                                    comm = new SqlCommand(sql, conn);
                                    count = comm.ExecuteNonQuery();
                                    conn.Close();
                                    // 西医药物删除 + 普通药物删除
                                    sql = String.Format("exec p_fz_ywsc @id = '{0}'", mid);
                                    conn.Open();
                                    comm = new SqlCommand(sql, conn);
                                    count = comm.ExecuteNonQuery();
                                    conn.Close();
                                }
                                if (xzzxs > 0)
                                {
                                    // 对证药物添加
                                    sql = String.Format("exec p_fz_dzywtj @id = '{0}'", mid);
                                    conn.Open();
                                    comm = new SqlCommand(sql, conn);
                                    count = comm.ExecuteNonQuery();
                                    conn.Close();
                                    // 西医药物添加 + 普通药物添加
                                    sql = String.Format("exec p_fz_ywtj @id = '{0}'", mid);
                                    conn.Open();
                                    comm = new SqlCommand(sql, conn);
                                    count = comm.ExecuteNonQuery();
                                    conn.Close();
                                }
                                // 药物处理
                                sql = String.Format("exec p_fz_ywcl @id = '{0}'", mid);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                count = comm.ExecuteNonQuery();
                                conn.Close();
                                // 将 t_ls_fz 表中信息加入 t_bl_mx
                                sql = String.Format("insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) select id = '{0}', xxdllx, xxxllx, xxbh from t_ls_fz a where a.id = '{1}'", mid, mid);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                count = comm.ExecuteNonQuery();
                                conn.Close();
                                //删除表 t_ls_fz 中信息
                                sql = String.Format("delete t_ls_fz where id = '{0}'", mid);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                count = comm.ExecuteNonQuery();
                                conn.Close();
                                //删除表 t_ls_qy 中信息
                                sql = String.Format("delete t_ls_qy where id = '{0}'", mid);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                count = comm.ExecuteNonQuery();
                                conn.Close();
                                //有一定疗效   改服法
                                if (comb_lx.SelectedIndex == 1)
                                {
                                    sql = String.Format("update t_bl_mx set xxbh = '一日二次，每剂服1.5天。' where xxdllx = '2' and xxxllx = '5' and id = '{0}'", mid);
                                    conn.Open();
                                    comm = new SqlCommand(sql, conn);
                                    count = comm.ExecuteNonQuery();
                                    if (count > 0)
                                        MessageBox.Show("更新剂量");
                                    conn.Close();
                                }
                                //有显著疗效    改服法
                                if (comb_lx.SelectedIndex == 2)
                                {
                                    sql = String.Format("update t_bl_mx set xxbh = '一日二次，每剂服4次（二天）。' where xxdllx = '2' and xxxllx = '5' and id = '{0}'", mid);
                                    conn.Open();
                                    comm = new SqlCommand(sql, conn);
                                    count = comm.ExecuteNonQuery();
                                    if (count > 0)
                                        MessageBox.Show("更新剂量");
                                    conn.Close();
                                }
                                //疗效不明显
                                if (comb_lx.SelectedIndex == 3)
                                {

                                }
                                //更新病历类型
                                sql = String.Format("update t_bl set bllx='0' where id='{0}'", id);
                                conn.Open();
                                comm = new SqlCommand(sql, conn);
                                int count3 = comm.ExecuteNonQuery();
                                if (count3 > 0)
                                {
                                    MessageBox.Show("更新成功");
                                }
                                dr.Close();
                                conn.Close();
                                display_lczd result = new display_lczd(mid);
                                result.Show();
                            }
                        }
                    }
                }
            }
        }

        private void btn_qc_Click(object sender, RoutedEventArgs e)
        {
            brbh.Text = "";
            name.Text = "";
            lid = "";
            mid = "";
            comb_lx.Text = "--请选择服药后的疗效！--";
            NewDisease.Text = "";
            blxs.Text = "";
            listPatient1.Clear();
            listPatient2.Clear();
            IsAdd = false;
            id = "";
        }

        private void btn_cxzd_Click(object sender, RoutedEventArgs e)
        {


        }

        private void Button_Click_yqy(object sender, RoutedEventArgs e)
        {
            if (!IsAdd)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (nengshan1 == false)
                {
                    MessageBox.Show("请用鼠标点选一个已经痊愈的症象！");
                }
                else
                {
                    //删除当前指定记录(名称)
                    PatientDisease Sel_Info = new PatientDisease("", "");
                    Sel_Info = lv_1.SelectedItem as PatientDisease;
                    if (Sel_Info.Xxlx == "上次就诊时的西医病名")
                        xlbh = "b";
                    if (Sel_Info.Xxlx == "上次就诊时的既往史名")
                        xlbh = "a";
                    if (Sel_Info.Xxlx == "上次就诊时的主诉名称")
                        xlbh = "6";
                    if (Sel_Info.Xxlx == "上次就诊时的主诉时间")
                        xlbh = "9";
                    if (Sel_Info.Xxlx == "上次就诊时的初起症象")
                        xlbh = "7";
                    if (Sel_Info.Xxlx == "上次就诊时的现症象名")
                        xlbh = "8";
                    string sql = String.Format("select subid,xxbh from t_ls_fz where id = '{0}' and xxdllx = '0' and xxxllx = '{1}' and xxbh = '{2}'", lid, xlbh, Sel_Info.Xxmc);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        subid = dr["subid"].ToString();
                        mc = dr["xxbh"].ToString();
                    }
                    dr.Close();
                    conn.Close();
                    if (xlbh == "9")
                    {
                        MessageBox.Show("这是上次就诊时的主诉时间,请不要删除！");
                    }
                    else if (xlbh == "a")
                    {
                        MessageBox.Show("这是病人的既往史,请不要删除！");
                    }
                    else if (xlbh == "b")
                    {
                        MessageBox.Show("这是病人的西医病名,请不要删除！");
                    }
                    else if (mc == "发于春季" || mc == "发于夏季" || mc == "发于秋季" || mc == "发于冬季")
                    {
                        MessageBox.Show("这是上次就诊时的发病季节,请不要删除！");
                    }
                    else
                    {
                        sql = String.Format("select zxbh from t_info_zxmx where zxmc ='{0}'", Sel_Info.Xxmc);
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            bh = dr["zxbh"].ToString();
                        }
                        dr.Close();
                        conn.Close();
                        if (MessageBox.Show("你确定症象：“" + mc + "”已经痊愈了吗?", "确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            sql = String.Format("delete from t_ls_fz where subid = '{0}'", subid);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            int count = comm.ExecuteNonQuery();
                            if (count > 0)
                            {
                                MessageBox.Show("删除成功");
                            }
                            dr.Close();
                            conn.Close();
                            if (xlbh == "6")
                                xlbh = "0"; //主诉
                            else if (xlbh == "7")
                                xlbh = "1";
                            else if (xlbh == "8")
                                xlbh = "2";
                            sql = String.Format("delete from t_ls_fz where id = '{0}' and xxdllx = '0' and xxxllx = '{1}' and xxbh = '{2}'", mid, xlbh, bh);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            count = comm.ExecuteNonQuery();
                            if (count > 0)
                            {
                                MessageBox.Show("删除编号成功");
                            }
                            dr.Close();
                            conn.Close();
                            listPatient1.RemoveAt(lv_1.SelectedIndex);
                            //将痊愈的症象编号加入临时表
                            sql = String.Format("insert into t_ls_qy (id,xxbh) values('{0}', '{1}' )", mid, bh);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            count = comm.ExecuteNonQuery();
                            if (count > 0)
                            {
                                MessageBox.Show("插入编号成功");
                            }
                            dr.Close();
                            conn.Close();
                            yhzxs++;
                        }
                        nengshan1 = false;
                    }

                }

            }
        }

        private void lv_1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            nengshan1 = true;
            nengshan2 = false;
        }

        private void lv_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            nengshan2 = true;
            nengshan1 = false;
        }


    }
}
