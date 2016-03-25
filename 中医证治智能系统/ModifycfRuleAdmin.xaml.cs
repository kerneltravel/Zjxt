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
    /// Interaction logic for ModifycfRuleAdmin.xaml
    /// </summary>
    public partial class ModifycfRuleAdmin : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 全局变量，基本证名编号
        public string m_jbzmbh = "";
        // 创建集合实例
        ObservableCollection<Rule> listRule = new ObservableCollection<Rule>();
        ObservableCollection<Drug> listDrug = new ObservableCollection<Drug>();
        // 全局变量，用于规则类信息
        Rule Rule_Edit = new Rule("", "", "", "", "");
        // 全局变量，用于药物类信息
        Drug Drug_Edit = new Drug("", "", "", "");
        // 全局变量，用于暂时存储【修改处方规则】中的 id
        public string m_id = "";
        // 全局变量，用于暂时存储【修改处方规则】中的 tjbh
        public string m_tjbh = "";
        // 全局变量，用于暂时存储【修改处方规则】中的 xgcfbh
        public string m_xgcfbh = "";
        // 用于判断在【药物】中是否进行了添加操作
        bool IsAdd = false;
        // 用于判断在【药物】中是否进行了修改操作
        bool IsModify = false;
        // 用于判断【规则】中是否重复添加
        bool IsRepeat_Rule = false;
        // 用于判断【药物】中是否重复添加
        bool IsRepeat_Drug = false;

        public ModifycfRuleAdmin()
        {
            InitializeComponent();
            // 初始化默认选择项
            comb_ffs.SelectedIndex = 0;
            comb_tjlx.SelectedIndex = 0;
            save_yw.IsEnabled = false;
            // 指定 listview 数据源
            lv_rule.ItemsSource = listRule;
            lv_yw.ItemsSource = listDrug;
        }

        /// <summary>
        /// 功能：创建规则信息类
        /// 说明：1.Ff --> 方法 Tjbh --> 条件编号 Tjmc --> 条件名称
        ///        Tjqz --> 条件权值 Gzyz --> 规则阀值
        /// </summary>
        public class Rule : INotifyPropertyChanged
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

            private string _Ff;
            private string _Tjbh;
            private string _Tjmc;
            private string _Tjqz;
            private string _Gzyz;

            public string Ff
            {
                get { return _Ff; }
                set { _Ff = value; OnPropertyChanged(new PropertyChangedEventArgs("Ff")); }
            }

            public string Tjbh
            {
                get { return _Tjbh; }
                set { _Tjbh = value; OnPropertyChanged(new PropertyChangedEventArgs("Tjbh")); }
            }

            public string Tjmc
            {
                get { return _Tjmc; }
                set { _Tjmc = value; OnPropertyChanged(new PropertyChangedEventArgs("Tjmc")); }
            }

            public string Tjqz
            {
                get { return _Tjqz; }
                set { _Tjqz = value; OnPropertyChanged(new PropertyChangedEventArgs("Tjqz")); }
            }

            public string Gzyz
            {
                get { return _Gzyz; }
                set { _Gzyz = value; OnPropertyChanged(new PropertyChangedEventArgs("Gzyz")); }
            }

            public Rule(string ff, string tjbh, string tjmc, string tjqz, string gzyz)
            {
                _Ff = ff;
                _Tjbh = tjbh;
                _Tjmc = tjmc;
                _Tjqz = tjqz;
                _Gzyz = gzyz;
            }
        }

        /// <summary>
        /// 功能：创建药物信息类
        /// 说明：1.Ff --> 方法 Xgfs --> 修改方式 Xgcfbh --> 修改处方编号 Xgcfmc --> 修改处方名称
        /// </summary>
        public class Drug : INotifyPropertyChanged
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

            private string _Ff;
            private string _Xgfs;
            private string _Xgcfbh;
            private string _Xgcfmc;

            public string Ff
            {
                get { return _Ff; }
                set { _Ff = value; OnPropertyChanged(new PropertyChangedEventArgs("Ff")); }
            }

            public string Xgfs
            {
                get { return _Xgfs; }
                set { _Xgfs = value; OnPropertyChanged(new PropertyChangedEventArgs("Xgfs")); }
            }

            public string Xgcfbh
            {
                get { return _Xgcfbh; }
                set { _Xgcfbh = value; OnPropertyChanged(new PropertyChangedEventArgs("Xgcfbh")); }
            }

            public string Xgcfmc
            {
                get { return _Xgcfmc; }
                set { _Xgcfmc = value; OnPropertyChanged(new PropertyChangedEventArgs("Xgcfmc")); }
            }

            public Drug(string ff, string xgfs, string xgcfbh, string xgcfmc)
            {
                _Ff = ff;
                _Xgfs = xgfs;
                _Xgcfbh = xgcfbh;
                _Xgcfmc = xgcfmc;
            }
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
        /// 功能：调用【基本证名信息管理】窗口,【选定】基本证名
        /// </summary>
        private void btn_jbzmmc_Click(object sender, RoutedEventArgs e)
        {
            BasicZhengm basiczhengm = new BasicZhengm();
            basiczhengm.PassValuesEvent += new BasicZhengm.PassValuesHandler(ReceiveValues);
            basiczhengm.Show();
        }

        /// <summary>
        /// 功能：实现基本证名名称和基本处方名称的读取和显示
        /// </summary>
        private void ReceiveValues(object sender, BasicZhengm.PassValuesEventArgs e)
        {
            m_jbzmbh = e.Number;           
            string sql = String.Format("select t2.id, t3.jbzmmc ,t1.jbcfmc  from (t_info_jbcfxx as t1 inner join t_rule_dzjbcf as t2 on t2.cfbh = t1.jbcfbh) inner join t_info_jbzm as t3 on t3.jbzmbh = t2.zmbh where t3.jbzmbh = '{0}'", e.Number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                m_id = dr["id"].ToString();
                this.jbzmmc.Text = dr["jbzmmc"].ToString();
                this.jbcfmc.Text = dr["jbcfmc"].ToString();
            }
            dr.Close();
            conn.Close();
            // 方法数清空
            comb_ffs.Items.Clear();
            comb_ffs.Items.Add("--请选择方法数列表--");
            comb_ffs.SelectedIndex = 0;
        }

        /// <summary>
        /// 功能：实现方法数的显示和选择
        /// 说明：1.方法数假定最多不超过8，否则需更改程序，在 switch 中添加 case.
        /// </summary>
        private void comb_ffs_DropDownOpened(object sender, EventArgs e)
        {
            if (comb_ffs.Items.Count == 1)
            {
                string sql = String.Format("select distinct t1.ff from t_rule_xgcfgz as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id where t2.zmbh = '{0}' order by t1.ff", m_jbzmbh);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    comb_ffs.Items.Add("方法" + numbertochinese(dr["ff"].ToString()));
                }
                dr.Close();
                conn.Close();  
            }
        }

        /// <summary>
        /// 功能：实现【规则】ListView 的显示
        /// 说明：1.采用数组的形式将条件编号和条件名称暂时保存，缺陷：当添加的条目多余数组定义的个数时需扩大数组维数
        /// </summary>
        private void comb_ffs_DropDownClosed(object sender, EventArgs e)
        {
            if (comb_ffs.SelectedIndex > 0)
            {
                /***********************************************/
                /*********** 显示【规则】中的 Listview **********/
                /***********************************************/
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
                int i = 0;
                int j = 0;
                listRule.Clear();
                // 读取条件编号
                string sql = String.Format("select tjbh = (case when zxbh is not null then zxbh when jbbjbh is not null then jbbjbh when fhbjbh is not null then fhbjbh when bmbh is not null then bmbh when jbzmbh is not null then jbzmbh end),"
                + "zxbh, jbbjbh, fhbjbh, bmbh, jbzmbh from t_rule_xgcfgz as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id where t2.zmbh = '{0}' and t1.ff = '{1}'", m_jbzmbh, comb_ffs.SelectedIndex);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    tjbh[i][0] = dr["tjbh"].ToString();
                    tjbh[i][1] = dr["zxbh"].ToString();
                    tjbh[i][2] = dr["jbbjbh"].ToString();
                    tjbh[i][3] = dr["fhbjbh"].ToString();
                    tjbh[i][4] = dr["bmbh"].ToString();
                    tjbh[i][5] = dr["jbzmbh"].ToString();
                    i++;
                }
                dr.Close();
                conn.Close();
                i = 0;
                // 根据条件编号读取条件名称
                while (tjbh[i][0] != "")
                {
                    if (tjbh[i][1] != "")
                    {
                        sql = String.Format("select zxmc from t_info_zxmx where zxbh = '{0}'", tjbh[i][1]);
                    }
                    if (tjbh[i][2] != "")
                    {
                        sql = String.Format("select jbbjmc from t_info_jbbj where jbbjbh = '{0}'", tjbh[i][2]);
                    }
                    if (tjbh[i][3] != "")
                    {
                        sql = String.Format("select fhbjmc from t_info_fhbj where fhbjbh = '{0}'", tjbh[i][3]);
                    }
                    if (tjbh[i][4] != "")
                    {
                        sql = String.Format("select bmmc from t_info_bm where bmbh = '{0}'", tjbh[i][4]);
                    }
                    if (tjbh[i][5] != "")
                    {
                        sql = String.Format("select jbzmmc from t_info_jbzm where jbzmbh = '{0}'", tjbh[i][5]);
                    }
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        tjmc[j] = dr[0].ToString();
                        //j++;
                    }
                    dr.Close();
                    conn.Close();
                    i++;
                    j++; // 没有写在while里，防止同一编号的不同名称重复写入
                }
                // 添加到 ListView 
                sql = String.Format("select tjbh = (case when zxbh is not null then zxbh when jbbjbh is not null then jbbjbh when fhbjbh is not null then fhbjbh when bmbh is not null then bmbh when jbzmbh is not null then jbzmbh end),"
                        + "znfz, gzfz from t_rule_xgcfgz as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id where t2.zmbh = '{0}' and t1.ff = '{1}'", m_jbzmbh, comb_ffs.SelectedIndex);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                j = 0;
                while (dr.Read())
                {
                    listRule.Add(new Rule(comb_ffs.SelectedIndex.ToString(), dr["tjbh"].ToString(), tjmc[j], dr["znfz"].ToString(), dr["gzfz"].ToString()));
                    j++;
                }
                dr.Close();
                conn.Close();
                lv_rule.SelectedIndex = 0;

                /***********************************************/
                /*********** 显示【药物】中的 Listview **********/
                /***********************************************/
                listDrug.Clear();
                sql = String.Format("select t1.ff, xgfs, jbcfbh, jbcfmc from (t_rule_xgcf as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id)"
                                    + "inner join t_info_jbcfxx as t3 on t3.jbcfbh = t1.xgcfbh where t2.zmbh = '{0}' and t1.ff = '{1}'", m_jbzmbh, comb_ffs.SelectedIndex);
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
                    listDrug.Add(new Drug(dr["ff"].ToString(), xgfs, dr["jbcfbh"].ToString(), dr["jbcfmc"].ToString()));
                }
                dr.Close();
                conn.Close();
                lv_yw.SelectedIndex = 0;
            }
            else
            {
                listRule.Clear();
                listDrug.Clear();
            }
        }


        /// <summary>
        /// 功能：增加方法数
        /// 说明：1.前提是基本证名不能为空
        ///       2.不能重复增加（根据数据库最大方法数与 items 总数目的关系）
        /// </summary>
        private void add_ff_Click(object sender, RoutedEventArgs e)
        {
            if (jbzmmc.Text == "")
            {
                MessageBox.Show("请先选择基本证名！");
            }
            else
            {
                string sql = String.Format("select max(t1.ff) from t_rule_xgcfgz as t1 inner join t_rule_dzjbcf as t2 on t1.id = t2.id where t2.zmbh = '{0}' ", m_jbzmbh);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    if (dr[0].ToString() == "" && comb_ffs.Items.Count == 1)
                    {
                        comb_ffs.Items.Add("方法" + numbertochinese(comb_ffs.Items.Count.ToString()));
                        comb_ffs.SelectedIndex = comb_ffs.Items.Count - 1;
                    }
                    else
                    {
                        if (dr[0].ToString() != "")
                        {
                            if (Convert.ToInt32(dr[0].ToString()) >= comb_ffs.Items.Count - 1)
                            {
                                comb_ffs.Items.Add("方法" + numbertochinese(comb_ffs.Items.Count.ToString()));
                                comb_ffs.SelectedIndex = comb_ffs.Items.Count - 1;
                            }
                        }                        
                    }
                }
                dr.Close();
                conn.Close();                 
            }
        }

        /// <summary>
        /// 功能：条件名称选择
        /// </summary>
        private void btn_tjmc_Click(object sender, RoutedEventArgs e)
        {
            switch(comb_tjlx.SelectedIndex)
            {
                case 0:
                    MessageBox.Show("请选择条件类型！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 1:
                    {
                        Info_Symptom symptom = new Info_Symptom();
                        symptom.PassValuesEvent += new Info_Symptom.PassValuesHandler(Receive_tjmc);
                        symptom.Show();
                    }
                    break;
                case 2:
                    {
                        BasicBingji basicbingji = new BasicBingji();
                        basicbingji.PassValuesEvent += new BasicBingji.PassValuesHandler(Receive_tjmc);
                        basicbingji.Show();
                    }
                    break;
                case 3:
                    {
                        FuheBingji fuhebingji = new FuheBingji();
                        fuhebingji.PassValuesEvent += new FuheBingji.PassValuesHandler(Receive_tjmc);
                        fuhebingji.Show();
                    }
                    break;
                case 4:
                    {
                        DiseaseInfoAdmin diseaseinfoadmin = new DiseaseInfoAdmin();
                        diseaseinfoadmin.PassValuesEvent += new DiseaseInfoAdmin.PassValuesHandler(Receive_tjmc);
                        diseaseinfoadmin.Show();
                    }
                    break;
                case 5:
                    {
                        BasicZhengm symptom = new BasicZhengm();
                        symptom.PassValuesEvent += new BasicZhengm.PassValuesHandler(Receive_tjmc);
                        symptom.Show();
                    }
                    break;
            }
        }

        /// <summary>
        /// 功能：调用【基本证名信息管理】实现条件名称的读取
        /// </summary>
        private void Receive_tjmc(object sender, BasicZhengm.PassValuesEventArgs e)
        {
            string sql = String.Format("select * from t_info_jbzm where jbzmbh = '{0}'", e.Number);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                tjmc.Text = dr["jbzmmc"].ToString();
            }
            dr.Close();
            conn.Close();  
        }

        /// <summary>
        /// 功能：调用【病名信息管理】实现条件名称的读取
        /// </summary>
        private void Receive_tjmc(object sender, DiseaseInfoAdmin.PassValuesEventArgs e)
        {
            tjmc.Text = e.Name;
        }

        /// <summary>
        /// 功能：调用【复合病机信息管理】实现条件名称的读取
        /// </summary>
        private void Receive_tjmc(object sender, FuheBingji.PassValuesEventArgs e)
        {
            tjmc.Text = e.Name;
        }

        /// <summary>
        /// 功能：调用【基本病机信息管理】实现条件名称的读取
        /// </summary>
        private void Receive_tjmc(object sender, BasicBingji.PassValuesEventArgs e)
        {
            tjmc.Text = e.Name;
        }

        /// <summary>
        /// 功能：调用【症象信息管理】实现条件名称的读取
        /// </summary>
        private void Receive_tjmc(object sender, Info_Symptom.PassValuesEventArgs e)
        {
            tjmc.Text = e.Name;
        }

        /// <summary>
        /// 功能：【规则】中的添加
        /// </summary>
        private void add_rule_Click(object sender, RoutedEventArgs e)
        {
            if (comb_ffs.SelectedIndex == 0)
            {
                MessageBox.Show("请先选择方法数！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (comb_tjlx.SelectedIndex == 0)
                {
                    MessageBox.Show("请选择条件类型！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (tjmc.Text == "")
                    {
                        MessageBox.Show("请输入条件！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        if (comb_tjqz.SelectedIndex == -1)
                        {
                            MessageBox.Show("请先录入该条件下的条件权值！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if (comb_gzfz.SelectedIndex == -1)
                            {
                                MessageBox.Show("请先录入该条件下的规则阀值！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                IsRepeatRule();
                                if (!IsRepeat_Rule)
                                {
                                    string sql = "";
                                    switch (comb_tjlx.SelectedIndex)
                                    {
                                        case 1: // 症象
                                            sql = String.Format("select zxbh from t_info_zxmx where zxmc = '{0}'", tjmc.Text);
                                            break;
                                        case 2: // 基本病机
                                            sql = String.Format("select jbbjbh from t_info_jbbj where jbbjmc = '{0}'", tjmc.Text);
                                            break;
                                        case 3: // 复合病机
                                            sql = String.Format("select fhbjbh from t_info_fhbj where fhbjmc = '{0}'", tjmc.Text);
                                            break;
                                        case 4: // 病名
                                            sql = String.Format("select bmbh from t_info_bm where bmmc = '{0}'", tjmc.Text);
                                            break;
                                        case 5: // 基本证名
                                            sql = String.Format("select jbzmbh from t_info_jbzm where jbzmmc = '{0}'", tjmc.Text);
                                            break;
                                    }
                                    conn.Open();
                                    SqlCommand comm = new SqlCommand(sql, conn);
                                    SqlDataReader dr = comm.ExecuteReader();
                                    while (dr.Read())
                                    {
                                        m_tjbh = dr[0].ToString();
                                        listRule.Add(new Rule(comb_ffs.SelectedIndex.ToString(), dr[0].ToString(), tjmc.Text, comb_tjqz.Text, comb_gzfz.Text));
                                        lv_rule.SelectedIndex = lv_rule.Items.Count - 1;
                                    }
                                    dr.Close();
                                    conn.Close();
                                    // 写入数据库
                                    try
                                    {
                                        switch (comb_tjlx.SelectedIndex)
                                        {
                                            case 1: // 症象
                                                sql = String.Format("insert into t_rule_xgcfgz (id, zxbh, ff, znfz, gzfz) values('{0}', '{1}', '{2}', '{3}', '{4}') "
                                                    , m_id, m_tjbh, comb_ffs.SelectedIndex, comb_tjqz.Text, comb_gzfz.Text);
                                                break;
                                            case 2: // 基本病机
                                                sql = String.Format("insert into t_rule_xgcfgz (id, jbbjbh, ff, znfz, gzfz) values('{0}', '{1}', '{2}', '{3}', '{4}') "
                                                    , m_id, m_tjbh, comb_ffs.SelectedIndex, comb_tjqz.Text, comb_gzfz.Text);
                                                break;
                                            case 3: // 复合病机
                                                sql = String.Format("insert into t_rule_xgcfgz (id, fhbjbh, ff, znfz, gzfz) values('{0}', '{1}', '{2}', '{3}', '{4}') "
                                                    , m_id, m_tjbh, comb_ffs.SelectedIndex, comb_tjqz.Text, comb_gzfz.Text);
                                                break;
                                            case 4: // 病名
                                                sql = String.Format("insert into t_rule_xgcfgz (id, bmbh, ff, znfz, gzfz) values('{0}', '{1}', '{2}', '{3}', '{4}') "
                                                    , m_id, m_tjbh, comb_ffs.SelectedIndex, comb_tjqz.Text, comb_gzfz.Text);
                                                break;
                                            case 5: // 基本证名
                                                sql = String.Format("insert into t_rule_xgcfgz (id, jbzmbh, ff, znfz, gzfz) values('{0}', '{1}', '{2}', '{3}', '{4}') "
                                                    , m_id, m_tjbh, comb_ffs.SelectedIndex, comb_tjqz.Text, comb_gzfz.Text);
                                                break;
                                        }
                                        conn.Open();
                                        comm = new SqlCommand(sql, conn);
                                        int count = comm.ExecuteNonQuery();
                                        //if (count > 0)
                                        //{
                                        //    MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                                        //}
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("添加失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                    finally
                                    {
                                        conn.Close();
                                    }
                                }
                            }
                        }
                    }
                }
            }            
        }

        /// <summary>
        /// 功能：【规则】中的删除
        /// </summary>
        private void delete_rule_Click(object sender, RoutedEventArgs e)
        {
            Rule rule = lv_rule.SelectedItem as Rule;
            if (rule != null && rule is Rule)
            {
                if (MessageBox.Show("您确认要删除吗？", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    rule = lv_rule.SelectedItem as Rule;
                    if (rule != null && rule is Rule)
                    {
                        listRule.Remove(rule);
                    }
                    try
                    {
                        string sql = String.Format("delete from t_rule_xgcfgz where zxbh = '{0}' or jbbjbh = '{0}' or fhbjbh = '{0}' or bmbh = '{0}' or jbzmbh = '{0}'", rule.Tjbh);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                        //if (count > 0)
                        //{
                        //    MessageBox.Show("删除成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        //}
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
        /// 功能：【药物】中的添加功能
        /// </summary>
        private void add_yw_Click(object sender, RoutedEventArgs e)
        {
            if (!IsModify)
            {
                if (comb_ffs.SelectedIndex == 0)
                {
                    MessageBox.Show("请先选择方法数！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    listDrug.Add(new Drug(comb_ffs.SelectedIndex.ToString(), "", "", ""));
                    lv_yw.SelectedIndex = lv_yw.Items.Count - 1;
                    IsAdd = true;
                    save_yw.IsEnabled = true;
                }
            }           
        }

        /// <summary>
        /// 功能：【药物】中的修改功能
        /// </summary>
        private void modify_yw_Click(object sender, RoutedEventArgs e)
        {
            Drug drug = lv_yw.SelectedItem as Drug;
            if (drug != null && drug is Drug)
            {
                if (!IsAdd)
                {
                    IsModify = true;
                    save_yw.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// 功能：选择处方名称
        /// </summary>
        private void btn_cfmc_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdd)
            {
                JibenCF jibencf = new JibenCF();
                jibencf.PassValuesEvent += new JibenCF.PassValuesHandler(Receive_cfmc);
                jibencf.Show();
            }
            else
            {
                MessageBox.Show("表未处于添加状态！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 功能：调用【基本处方信息管理】实现处方名称的读取
        /// </summary>
        private void Receive_cfmc(object sender, JibenCF.PassValuesEventArgs e)
        {
            cfmc.Text = e.Name;
            Drug drug = lv_yw.SelectedItem as Drug;
            if (drug != null && drug is Drug)
            {
                string sql = String.Format("select jbcfbh from t_info_jbcfxx where jbcfmc = '{0}'", cfmc.Text);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    m_xgcfbh = dr[0].ToString();
                }
                dr.Close();
                conn.Close();
                drug.Xgcfbh = m_xgcfbh;
                drug.Xgcfmc = cfmc.Text;
            }
        }

        /// <summary>
        /// 功能：【药物】中的保存功能
        /// </summary>
        private void save_yw_Click(object sender, RoutedEventArgs e)
        {
            // 添加后保存
            IsRepeatDrug();
            if (!IsRepeat_Drug)
            {
                if (IsAdd)
                {
                    if (cfmc.Text == "")
                    {
                        MessageBox.Show("修改处方不能为空！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        if (comb_xgfs.SelectedIndex == -1)
                        {
                            MessageBox.Show("请选择修改方式！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            try
                            {
                                string xgfs = "";
                                if (comb_xgfs.SelectedIndex == 0)  // 修改方式为“添加”
                                    xgfs = "+";
                                if (comb_xgfs.SelectedIndex == 1)  // 修改方式为“替换”
                                    xgfs = "=";
                                string sql = String.Format("insert into t_rule_xgcf (id, ff, xgfs, xgcfbh) values('{0}', '{1}', '{2}', '{3}') "
                                                    , m_id, comb_ffs.SelectedIndex, xgfs, m_xgcfbh);
                                conn.Open();
                                SqlCommand comm = new SqlCommand(sql, conn);
                                int count = comm.ExecuteNonQuery();
                                if (count > 0)
                                {
                                    IsAdd = false;
                                    save_yw.IsEnabled = false;
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("添加失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            finally
                            {
                                conn.Close();
                            }
                        }
                    }
                }
            }           
            // 修改后保存
            if (IsModify)
            {
                Drug drug = lv_yw.SelectedItem as Drug;
                if (drug != null && drug is Drug)
                {
                    if (comb_xgfs.Text == "")
                    {
                        MessageBox.Show("请选择修改方式！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        drug.Xgfs = comb_xgfs.Text;
                        try
                        {
                            string xgfs = "";
                            if (comb_xgfs.SelectedIndex == 0)  // 修改方式为“添加”
                                xgfs = "+";
                            if (comb_xgfs.SelectedIndex == 1)  // 修改方式为“替换”
                                xgfs = "=";
                            string sql = String.Format("update t_rule_xgcf set xgfs = '{0}' where id = '{1}' and xgcfbh = '{2}'", xgfs, m_id, drug.Xgcfbh);
                            conn.Open();
                            SqlCommand comm = new SqlCommand(sql, conn);
                            int count = comm.ExecuteNonQuery();
                            //if (count > 0)
                            //{
                            //    MessageBox.Show("保存成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            //}
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("保存失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        finally
                        {
                            IsModify = false;
                            save_yw.IsEnabled = false;
                            conn.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 功能：显示修改方式
        /// </summary>
        private void comb_xgfs_DropDownClosed(object sender, EventArgs e)
        {
            if (IsAdd || IsModify)
            {
                Drug drug = lv_yw.SelectedItem as Drug;
                if (drug != null && drug is Drug)
                {
                    drug.Xgfs = comb_xgfs.Text;
                }
            }
        }

        /// <summary>
        /// 功能：【药物】中的删除功能
        /// </summary>
        private void delete_yw_Click(object sender, RoutedEventArgs e)
        {
            Drug drug = lv_yw.SelectedItem as Drug;
            if (drug != null && drug is Drug)
            {
                if (!IsAdd && !IsModify)
                {
                    try
                    {
                        drug = lv_yw.SelectedItem as Drug;
                        if (drug != null && drug is Drug)
                        {
                            listDrug.Remove(drug);
                            cfmc.Text = "";
                            comb_xgfs.Text = "";
                        }
                        string sql = String.Format("delete from t_rule_xgcf where id = '{0}' and xgcfbh = '{1}'", m_id, drug.Xgcfbh);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
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
        /// 功能：【药物】中的取消功能
        /// </summary>
        private void cancel_yw_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdd)
            {
                Drug drug = lv_yw.SelectedItem as Drug;
                if (drug != null && drug is Drug)
                {
                    listDrug.Remove(drug);
                }
                cfmc.Text = "";
                comb_xgfs.Text = "";
                IsAdd = false;
                save_yw.IsEnabled = false;
            }
            if (IsModify)
            {
                cfmc.Text = "";
                comb_xgfs.Text = "";
                IsModify = false;
                save_yw.IsEnabled = false;
            }
        }

        /// <summary>
        /// 功能：防止【规则】重复添加
        /// </summary>
        public void IsRepeatRule()
        {
            string sql = "";
            switch (comb_tjlx.SelectedIndex)
            {
                case 1: // 症象
                    sql = String.Format("select zxbh from t_info_zxmx where zxmc = '{0}'", tjmc.Text);
                    break;
                case 2: // 基本病机
                    sql = String.Format("select jbbjbh from t_info_jbbj where jbbjmc = '{0}'", tjmc.Text);
                    break;
                case 3: // 复合病机
                    sql = String.Format("select fhbjbh from t_info_fhbj where fhbjmc = '{0}'", tjmc.Text);
                    break;
                case 4: // 病名
                    sql = String.Format("select bmbh from t_info_bm where bmmc = '{0}'", tjmc.Text);
                    break;
                case 5: // 基本证名
                    sql = String.Format("select jbzmbh from t_info_jbzm where jbzmmc = '{0}'", tjmc.Text);
                    break;
            }
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                m_tjbh = dr[0].ToString();
            }
            dr.Close();
            conn.Close();

            switch (comb_tjlx.SelectedIndex)
            {
                case 1: // 症象
                    sql = String.Format("select count(*) from t_rule_xgcfgz where id = '{0}' and ff = '{1}' and zxbh = '{2}' ", m_id, comb_ffs.SelectedIndex, m_tjbh);
                    break;
                case 2: // 基本病机
                    sql = String.Format("select count(*) from t_rule_xgcfgz where id = '{0}' and ff = '{1}' and jbbjbh = '{2}' ", m_id, comb_ffs.SelectedIndex, m_tjbh);
                    break;
                case 3: // 复合病机
                    sql = String.Format("select count(*) from t_rule_xgcfgz where id = '{0}' and ff = '{1}' and fhbjbh = '{2}' ", m_id, comb_ffs.SelectedIndex, m_tjbh);
                    break;
                case 4: // 病名
                    sql = String.Format("select count(*) from t_rule_xgcfgz where id = '{0}' and ff = '{1}' and bmbh = '{2}' ", m_id, comb_ffs.SelectedIndex, m_tjbh);
                    break;
                case 5: // 基本证名
                    sql = String.Format("select count(*) from t_rule_xgcfgz where id = '{0}' and ff = '{1}' and jbzmbh = '{2}' ", m_id, comb_ffs.SelectedIndex, m_tjbh);
                    break;
            }         
            conn.Open();
            comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count > 0)
            {
                MessageBox.Show("该条件已添加！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                IsRepeat_Rule = true;
            }
            else
                IsRepeat_Rule = false;
            conn.Close();
        }

        /// <summary>
        /// 功能：防止【药物】重复添加
        /// </summary>
        public void IsRepeatDrug()
        {
            string sql = String.Format("select count(*) from t_rule_xgcf where id = '{0}' and ff = '{1}' and xgcfbh = '{2}' ", m_id, comb_ffs.SelectedIndex, m_xgcfbh);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count > 0)
            {
                MessageBox.Show("该处方已存在！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                IsRepeat_Drug = true;
            }
            else
                IsRepeat_Drug = false;
            conn.Close();
        }
    }
}
