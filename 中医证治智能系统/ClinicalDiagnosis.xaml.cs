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
    /// Interaction logic for ClinicalDiagnosis.xaml
    /// </summary>
    public partial class ClinicalDiagnosis : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        bool isadd = false;
        bool isnowzz = false;
        bool isfirstzz = false;
        bool iszstime = false;
        bool iszs = false;
        string ID = "";
        string temp = "";
        int wg_ff = 0;
        int count = 0;
        int fhbj = 0;
        int count_zm=0;
        int count_bm = 0;
        string bh = "";
        string xlbh = "";
        string subid = "";
        
        ObservableCollection<WesternMedicineZMInfo> listSymptom1 = new ObservableCollection<WesternMedicineZMInfo>();
        ObservableCollection<WesternMedicineInfo> listSymptom = new ObservableCollection<WesternMedicineInfo>();
        public ClinicalDiagnosis()
        {
            
            InitializeComponent();
            lv1.ItemsSource = listSymptom1;
            lv.ItemsSource = listSymptom;
        }
        public class WesternMedicineZMInfo : INotifyPropertyChanged
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

            private string _XYZMBH;

            public string XYZMBH
            {
                get { return _XYZMBH; }
                set { _XYZMBH = value; OnPropertyChanged(new PropertyChangedEventArgs("XYZMBH")); }
            }

            public WesternMedicineZMInfo(string XYZMBH)
            {
                _XYZMBH = XYZMBH;
            }
        }
        public class WesternMedicineInfo : INotifyPropertyChanged
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

            private string _WesternMedicineType;
            private string _WesternMedicineName;

            public string WesternMedicineType
            {
                get { return _WesternMedicineType; }
                set { _WesternMedicineType = value; OnPropertyChanged(new PropertyChangedEventArgs("WesternMedicineType")); }
            }

            public string WesternMedicineName
            {
                get { return _WesternMedicineName; }
                set { _WesternMedicineName = value; OnPropertyChanged(new PropertyChangedEventArgs("WesternMedicineName")); }
            }

            public WesternMedicineInfo(string WesternMedicinetype, string WesternMedicinename)
            {
                _WesternMedicineType = WesternMedicinetype;
                _WesternMedicineName = WesternMedicinename;
            }
        }

        private void zzsr_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (zzsr.Text != "")
            {
                listSymptom1.Clear();
                string sql = String.Format("select * from t_info_zxmx where zxmc like '%{0}%' order by zxmc",zzsr.Text.Trim());
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();            
                while (dr.Read())
                {

                    listSymptom1.Add(new WesternMedicineZMInfo(dr["zxmc"].ToString()));
                }
                dr.Close();
                conn.Close();
                }
            
        }

        private void newDiagnosis_Click(object sender, RoutedEventArgs e)
        {
            if(isadd==false)
            {
                isadd = true;
                string sql = String.Format("Insert into t_bl (bllx,jsxx,jzsj,ysbh,zt) values ('4','', GetDate(),'00000028','0')");
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                int count = comm.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("插入成功");                
                }
                conn.Close();
                sql = String.Format("select max(id) from t_bl where ysbh='00000028'");
                conn.Open();
                comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    ID = (Convert.ToInt64(dr[0])).ToString();
                }
                dr.Close();
                conn.Close();
                //sql = String.Format("update t_bl set blbh='{0}',jsxx='{1}',zt='0' where id='{0}'",ID,brbh.Text.ToString());//需预先输入病人信息
                //conn.Open();
                //comm = new SqlCommand(sql, conn);
                //count = comm.ExecuteNonQuery();
                //if (count > 0)
                //{
                //    MessageBox.Show("更新成功");
                //}
                //dr.Close();
                //conn.Close();

            }
            
        }

        private void Diagnosis_Click(object sender, RoutedEventArgs e)//需将添加的信息写入t_bl_mx表
        {
            for(int i=0;i<listSymptom.Count;i++)
            {
                WesternMedicineInfo item = listSymptom.ElementAt(i);
                if(item.WesternMedicineType=="现症象名称")
                {
                    isnowzz = true;
                }
                if(item.WesternMedicineType=="主诉时间名称")
                {
                    iszstime = true;
                }
                if (item.WesternMedicineType == "主诉名称")
                {
                    iszs = true;
                }
            }
            
            if(brbh.Text=="")
            {
                MessageBox.Show("请选择病人！");
            }
            else if(isadd==false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！");
            }
            else if(iszs==false)
            {
                MessageBox.Show("请输入病人主诉信息！");
            }
            else if(iszstime==false)
            {
                MessageBox.Show("请输入病人主诉时间信息！");
            }
            else if(isnowzz==false)
            {
                MessageBox.Show("请输入病人现症状信息！");
            }
            else
            {//诊断推理过程
                isadd = false;
                isnowzz = false;
                isfirstzz = false;
                iszstime = false;
                iszs = false;
                listSymptom1.Clear();
                zzsr.Text = "";
                listSymptom.Clear();
                brbh.Text = "";
                xm.Text = "";
                string sql = String.Format("exec p_zd_judgewg @id ='{0}' ",ID);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();            
                while (dr.Read())
                {

                    MessageBox.Show("调用判断外感/内伤存储过程p_zd_judgewg");
                }
                dr.Close();
                conn.Close();
                sql = String.Format("exec p_zd_jbbj @id = '{0}' ", ID);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {

                    MessageBox.Show("调用基本病机推理存储过程p_zd_jbbj");
                }
                dr.Close();
                conn.Close();
                sql = String.Format("select xxbh from t_bl_mx where id ='{0}'and xxdllx = '1'and xxxllx = '0'", ID); 
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {

                    temp=dr[0].ToString();
                }
                dr.Close();
                conn.Close();
                if (temp == "0")//病名类型为外感，则根据成立的方法调用外感病名推理
                {
                    sql = String.Format("select xxbh from t_bl_mx where id = '{0}' and xxdllx = '1' and xxxllx ='b' order by xxbh desc", ID);       //外感推理方法数
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
                            sql = String.Format("exec p_wg_bmtl @id = '{0}', @wg_ff = 1", ID);
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
                            sql = String.Format("exec p_wg_bmtl @id = '{0}', @wg_ff = 2", ID);
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
                            sql = String.Format("exec p_wg_bmtl @id = '{0}', @wg_ff = 3", ID);
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
                            sql = String.Format("exec p_wg_bmtl @id = '{0}', @wg_ff = 4", ID);
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
                sql = String.Format("exec p_wgbm_bs @id = '{0}'",ID);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {

                    MessageBox.Show("病史推理外感病名");
                }
                dr.Close();
                conn.Close();
                sql = String.Format("select xxbh from t_bl_mx where id = '{0}' and xxdllx = '1' and xxxllx ='0'", ID);        //再次获取病名类型
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
                if(temp=="0")
                {
                    //调用外感推理子程序wg_tl
                    if (wg_tl())//外感推理有结果时，进外感后续处理
                    {
                        //判断是否有外感证名推出
                        sql = String.Format("select count(*) from t_bl_mx a, t_info_jbzm b where a.id = '{0}'and a.xxdllx = '1' and a.xxxllx = '8' and a.xxbh = b.jbzmbh and b.jbzmlx = '0'", ID);  //外感基本证名
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
                        sql = String.Format("select count(*) from t_bl_mx a where a.id = '{0}'and a.xxdllx = '1' and a.xxxllx = '7'", ID);      //基本证名编号
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
                                sql = String.Format("exec p_fhbj_jstl @id = '{0}'", ID);
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
                                sql = String.Format("delete from t_ls_jbzm where id = '{0}'", ID);
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
                                sql = String.Format("exec p_wg_zm_tl @id = '{0}'", ID);
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
                                sql = String.Format("select count(*) from t_bl_mx a, t_info_jbzm b where a.id = '{0}'and a.xxdllx = '1' and a.xxxllx = '8' and a.xxbh = b.jbzmbh and b.jbzmlx = '0'", ID);  //外感基本证名
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
                                    sql = String.Format("exec p_wg_zm_jstl @id = '{0}'", ID);  //外感基本证名
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
                                    sql = String.Format("update t_bl set zt = ''8'' where id = '{0}'", ID);
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
                            else//如果有复合病机成立，则调外感基本证名近似推理
                            {
                                sql = String.Format("exec p_wg_zm_jstl @id = '{0}'", ID);
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
                //将服法添加到病历明细表
                sql = String.Format("insert t_bl_mx (id,xxdllx,xxxllx,xxbh) select	id = '{0}',xxdllx = '2',xxxllx = '5', xxbh = a.ff from t_info_jbcfxx a, t_bl_mx b where b.id = '{0}'and b.xxdllx = '2' and b.xxxllx = '4' and a.jbcfbh = b.xxbh",ID );
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
                sql = String.Format("update t_bl set bllx='0' where id='{0}'", ID);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count3 = comm.ExecuteNonQuery();
                if (count3 > 0)
                {
                    MessageBox.Show("更新成功");
                }
                dr.Close();
                conn.Close();
                display_lczd result = new display_lczd(ID);
                result.Show();
            }
        }
        public void wg_hstl()//外感后续推理过程
        {
            //调用证名合并推理子过程
            string sql = String.Format("exec p_zmhb @id = '{0}'", ID);
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
            sql = String.Format("exec p_wg_bm_zm_tl @id = '{0}'", ID);
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
            sql = String.Format("exec p_zd_cftl @id = '{0}'", ID);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("处方推理子过程");
            }
            dr.Close();
            conn.Close();

        }
        public bool wg_tl()//外感推理子过程wg_tl
        {
            //调用系推理存储过程（根据症象推理）p_x_zx
            string sql = String.Format("exec p_x_zx @id = '{0}'", ID); 
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
            sql = String.Format("exec p_zd_fhbj @id = '{0}'", ID); 
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
            sql = String.Format("exec p_wg_zm_tl @id = '{0}'", ID);
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
            sql = String.Format("select count(*) from t_bl_mx where id = '{0}' and xxdllx = '1' and xxxllx = '4'", ID);    
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                count_bm=Convert.ToInt16(dr[0].ToString()) ;
            }
            dr.Close();
            conn.Close();
            sql = String.Format("select count(*) from t_bl_mx a, t_info_jbzm b where a.id = '{0} 'and a.xxdllx = '1' and a.xxxllx = '8' and a.xxbh = b.jbzmbh and b.jbzmlx ='0'", ID);  //外感基本证名
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                count_zm=Convert.ToInt16(dr[0].ToString()) ;
            }
            dr.Close();
            conn.Close();
            if (count_bm <= 0 && count_zm <= 0)
                return false;
            else            
                return true;
        }
        public void program_ns() //内伤推理总程序，包含ns_tl和ns_hstl的处理
        {
            if (ns_tl())//内伤推理有结果时，进内伤后续处理
            {
                ns_hstl();
            }
            else//内伤证名没有结果时，进内伤近似处理
            {
                //判断是否有证名推出
                string sql = String.Format("select count(*) from t_bl_mx a, t_info_jbzm b where a.id = '{0}'and a.xxdllx = '1' and a.xxxllx = '8'and a.xxbh = b.jbzmbh and b.jbzmlx = '1'", ID);  //内伤基本证名
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    count = Convert.ToInt16(dr[0].ToString()) ;
                }
                dr.Close();
                conn.Close();
                sql = String.Format("select count(*) from t_bl_mx a where a.id ='{0}' and a.xxdllx = '1' and a.xxxllx = '7'",ID);           //复合病机编号
                conn.Open();
                comm = new SqlCommand(sql, conn);
                dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    fhbj = Convert.ToInt16(dr[0].ToString());
                }
                dr.Close();
                conn.Close();
                if(count==0)//如果没有证名成立，则近似
                {
                    if(fhbj==0)//如果没有复合病机成立，则先近似推理复合病机
                    {
                        sql = String.Format("exec p_fhbj_jstl @id = '{0}'", ID);           ////复合病机近似处理
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            MessageBox.Show("复合病机近似处理");
                        }
                        dr.Close();
                        conn.Close();
                        sql = String.Format("delete from t_ls_jbzm where id = '{0}'", ID);           //清除基本证名临时表
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        int count4 = comm.ExecuteNonQuery();
                        if (count4 > 0)
                        {
                            MessageBox.Show("清除基本证名临时表");
                        }
                        dr.Close();
                        conn.Close();
                        sql = String.Format("exec p_ns_zm_tl @id =  '{0}'", ID);           //重新调用基本证名推理
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            MessageBox.Show("重新调用基本证名推理");
                        }
                        dr.Close();
                        conn.Close();
                        sql = String.Format("exec p_ns_zm_tl @id =  '{0}'", ID);           //重新调用基本证名推理
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            MessageBox.Show("重新调用基本证名推理");
                        }
                        dr.Close();
                        conn.Close();
                        sql = String.Format("select count(*) from t_bl_mx a, t_info_jbzm b where a.id = '{0}' and a.xxdllx = '1' and a.xxxllx = '8' and a.xxbh = b.jbzmbh and b.jbzmlx = '1'",ID);  //内伤基本证名//再次判断是否有证名推出
                        conn.Open();
                        comm = new SqlCommand(sql, conn);
                        dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            count = Convert.ToInt16(dr[0].ToString()) ;
                        }
                        dr.Close();
                        conn.Close();
                        if(count==0)//如果ns基本证名还不成立，则调ns基本证名近似推理
                        {
                            sql = String.Format("exec p_ns_zm_jstl @id =  '{0}'", ID);
                            conn.Open();
                            comm = new SqlCommand(sql, conn);
                            dr = comm.ExecuteReader();
                            while (dr.Read())
                            {
                                MessageBox.Show("基本证名近似推理");
                            }
                            dr.Close();
                            conn.Close();
                            sql = String.Format("update t_bl set zt = '8' where id = '{0}'", ID);
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
                        sql = String.Format("exec p_ns_zm_jstl @id =  '{0}'", ID);
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
        public bool ns_tl ()//内伤推理子过程，如果有结果推出，返回true，否则返回false
        {//调用内伤系推理存储过程（根据主诉推理）
            String sql = String.Format("exec p_x_zs @id =  '{0}'", ID);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("主诉推理");
            }
            dr.Close();
            conn.Close();
            sql = String.Format("exec p_x_zx @id =  '{0}'", ID);//调用内伤系推理存储过程（根据症象推理）
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
            sql = String.Format("exec p_ns_jlbm @id =  '{0}'", ID);//甲类病名推理
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("甲类病名推理");
            }
            dr.Close();
            conn.Close();
            sql = String.Format("exec p_ns_ylbm @id =  '{0}'", ID);//乙类病名推理
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("乙类病名推理");
            }
            dr.Close();
            conn.Close();
            sql = String.Format("exec p_zd_fhbj @id =  '{0}'", ID);//调用复合病机推理存储过程
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("调用复合病机推理存储过程");
            }
            dr.Close();
            conn.Close();
            sql = String.Format("exec p_ns_zm_tl @id =  '{0}'", ID);//调用内伤证名推理存储过程
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("调用内伤证名推理存储过程");
            }
            dr.Close();
            conn.Close();
            sql = String.Format("exec p_ns_zm_tl @id =  '{0}'", ID);//调用内伤证名推理存储过程
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
            sql = String.Format("select count(*) from t_bl_mx a, t_info_jbzm b where a.id = '{0}'  and a.xxdllx = '1' and a.xxxllx = '8' and a.xxbh = b.jbzmbh  and b.jbzmlx = '1'", ID);  //内伤基本证名
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
        public void ns_hstl()//内伤后续推理
        {
            //调用证名合并推理子过程
            String sql = String.Format("exec p_zmhb @id =  '{0}'", ID);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("证名合并推理");
            }
            dr.Close();
            conn.Close();
            sql = String.Format("exec p_ns_bm_zm_tl @id =  '{0}'", ID);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("内伤病名来选择基本证名");
            }
            dr.Close();
            conn.Close();
            //调用根据已推出内伤病名来选择基本证名的子过程
            sql = String.Format("exec pp @id =  '{0}'", ID);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("内伤病名来选择基本证名");
            }
            dr.Close();
            conn.Close();
            //调用处方推理子过程
            sql = String.Format("exec p_zd_cftl @id =  '{0}'", ID);
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                MessageBox.Show("处方推理");
            }
            dr.Close();
            conn.Close();
        }
        private void clear_Click(object sender, RoutedEventArgs e)
        {
            listSymptom1.Clear();
            zzsr.Text = "";
            listSymptom.Clear();
            isadd = false;
            isnowzz = false;
            isfirstzz = false;
            iszstime = false;
            iszs = false;
            brbh.Text = "";
            xm.Text = "";
            ID = "0";
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            if (isadd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！");
            }
            else 
            {
                PatientinfoSearch patient = new PatientinfoSearch();
                patient.Show();
                patient.PassValuesEvent += new PatientinfoSearch.PassValuesHandler(ReceiveValues1);
            }
            
        }
        /// <summary>
        /// 功能：实现病名名称的读取和显示
        /// </summary>
        private void ReceiveValues1(object sender, PatientinfoSearch.PassValuesEventArgs e)
        {
                xm.Text = e.Name;
                brbh.Text = e.Number;
                string sql = String.Format("update t_bl set blbh='{0}', jsxx='{1}',zt='0' where id='{0}'", ID, brbh.Text.ToString());//需预先输入病人信息
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                int  count = comm.ExecuteNonQuery();
                if (count > 0)
                {
                    MessageBox.Show("更新成功");
                }
                conn.Close();
        }

        private void nowzz_Click(object sender, RoutedEventArgs e)
        {
            string bh="";
            if (isadd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！");
            }
            else if(lv1.SelectedIndex==-1)
            {
                MessageBox.Show("没有选中任何信息，请先在症状列表选择框中选中您需要的症状！");
                zzsr.Text = "";
                listSymptom1.Clear();
            }
            else
            {
                //判断是否重复输入
                WesternMedicineZMInfo Sel_Info = new WesternMedicineZMInfo("");
                Sel_Info = lv1.SelectedItem as WesternMedicineZMInfo;
                string sql = String.Format("select count(*) from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '8' and xxbh = '{1}'", ID, Sel_Info.XYZMBH);
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
                    zzsr.Text = "";
                    listSymptom.Add(new WesternMedicineInfo("现症象名称", Sel_Info.XYZMBH));
                    listSymptom1.Clear();
                    sql = String.Format("select zxbh from t_info_zxmx where zxmc ='{0}'", Sel_Info.XYZMBH);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {

                        bh = dr[0].ToString();
                    }
                    dr.Close();
                    conn.Close();
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', '2', '{0}')", bh,ID);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count1 = comm.ExecuteNonQuery();
                    if (count1 > 0)
                    {

                        MessageBox.Show("插入症象编号成功");
                    }
                    dr.Close();
                    conn.Close();
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', '8','{0}')", Sel_Info.XYZMBH,ID);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count1 = comm.ExecuteNonQuery();
                    if (count1 > 0)
                    {

                        MessageBox.Show("插入症象名称成功");
                    }
                    dr.Close();
                    conn.Close();
                }
                                                                    
            }

        }

        private void firstzz_Click(object sender, RoutedEventArgs e)
        {
            int count=0;
            string bh = "";
            if (isadd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！");
            }
            else if (lv1.SelectedIndex == -1)
            {
                MessageBox.Show("没有选中任何信息，请先在症状列表选择框中选中您需要的症状！");
                zzsr.Text = "";
                listSymptom1.Clear();
            }
            else
            {
                //判断是否重复输入
                WesternMedicineZMInfo Sel_Info = new WesternMedicineZMInfo("");
                Sel_Info = lv1.SelectedItem as WesternMedicineZMInfo;
                string sql = String.Format("select count(*) from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '7' and xxbh = '{1}'", ID, Sel_Info.XYZMBH);
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
                    zzsr.Text = "";
                    
                    listSymptom.Add(new WesternMedicineInfo("初起症象名称", Sel_Info.XYZMBH));
                    listSymptom1.Clear();
                    sql = String.Format("select zxbh from t_info_zxmx where zxmc ='{0}'", Sel_Info.XYZMBH);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        bh = dr[0].ToString();
                    }
                    dr.Close();
                    conn.Close();
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', '1', '{0}')", bh,ID);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count1 = comm.ExecuteNonQuery();
                    if (count1 > 0)
                    {

                        MessageBox.Show("插入初起症象名称编号成功");
                    }
                    dr.Close();
                    conn.Close();
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', '7','{0}')", Sel_Info.XYZMBH,ID);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count1 = comm.ExecuteNonQuery();
                    if (count1 > 0)
                    {

                        MessageBox.Show("插入初起症象名称成功");
                    }
                    dr.Close();
                    conn.Close();
                }                                
            }
        }

        private void zstime_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            string bh = "";
            if (isadd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！");
            }
            else if (lv1.SelectedIndex == -1)
            {
                MessageBox.Show("没有选中任何信息，请先在症状列表选择框中选中您需要的症状！");
                zzsr.Text = "";
                listSymptom1.Clear();
            }
            else
            {
                //判断是否重复输入
                WesternMedicineZMInfo Sel_Info = new WesternMedicineZMInfo("");
                Sel_Info = lv1.SelectedItem as WesternMedicineZMInfo;
                string sql = String.Format("select count(*) from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '9' and xxbh = '{1}'", ID, Sel_Info.XYZMBH);
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
                    zzsr.Text = "";
                    
                    listSymptom.Add(new WesternMedicineInfo("主诉时间名称", Sel_Info.XYZMBH));
                    listSymptom1.Clear();
                    sql = String.Format("select zxbh from t_info_zxmx where zxmc ='{0}'", Sel_Info.XYZMBH);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        bh = dr[0].ToString();
                    }
                    dr.Close();
                    conn.Close();
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', '3', '{0}')", bh,ID);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count1 = comm.ExecuteNonQuery();
                    if (count1 > 0)
                    {

                        MessageBox.Show("插入主诉时间名称编号成功");
                    }
                    dr.Close();
                    conn.Close();
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', '9','{0}')", Sel_Info.XYZMBH,ID);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count1 = comm.ExecuteNonQuery();
                    if (count1 > 0)
                    {

                        MessageBox.Show("插入主诉时间成功");
                    }
                    dr.Close();
                    conn.Close();
                }
            }
        }

        private void zs_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            string bh = "";
            if (isadd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！");
            }
            else if (lv1.SelectedIndex == -1)
            {
                MessageBox.Show("没有选中任何信息，请先在症状列表选择框中选中您需要的症状！");
                zzsr.Text = "";
                listSymptom1.Clear();
            }
            else
            {
                WesternMedicineZMInfo Sel_Info = new WesternMedicineZMInfo("");
                Sel_Info = lv1.SelectedItem as WesternMedicineZMInfo;
                string sql = String.Format("select count(*) from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '0' and xxbh = '{1}'", ID, Sel_Info.XYZMBH);
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

                    zzsr.Text = "";

                    listSymptom.Add(new WesternMedicineInfo("主诉名称", Sel_Info.XYZMBH));
                    listSymptom1.Clear();
                    sql = String.Format("select zxbh from t_info_zxmx where zxmc ='{0}'", Sel_Info.XYZMBH);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        bh = dr[0].ToString();
                    }
                    dr.Close();
                    conn.Close();
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', '0', '{0}')", bh,ID);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    int count1 = comm.ExecuteNonQuery();
                    if (count1 > 0)
                    {

                        MessageBox.Show("插入主诉名称编号成功");
                    }
                    dr.Close();
                    conn.Close();
                    sql = String.Format("delete from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '2' and xxbh = '{1}'", ID, bh);       //主诉编号
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count1 = comm.ExecuteNonQuery();
                    if (count1 > 0)
                    {

                        MessageBox.Show("删除同名症象编号");
                    }
                    dr.Close();
                    conn.Close();
                    sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', '6','{0}')", Sel_Info.XYZMBH,ID);
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count1 = comm.ExecuteNonQuery();
                    if (count1 > 0)
                    {

                        MessageBox.Show("插入主诉名称成功");
                    }
                    dr.Close();
                    conn.Close();
                    sql = String.Format("delete from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '8' and xxbh = '{1}'", ID, Sel_Info.XYZMBH);       //主诉编号
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    count1 = comm.ExecuteNonQuery();
                    if (count1 > 0)
                    {

                        MessageBox.Show("删除同名症象名称");
                    }
                    dr.Close();
                    conn.Close();
                }
            }
            
        }

        private void delete_Click(object sender, RoutedEventArgs e)//待完善与测试
        {
            if (isadd == false)
            {
                MessageBox.Show("表未处于打开状态，请先新建诊断！");
            }
            else if (lv.SelectedIndex == -1)
            {
                MessageBox.Show("没有选中任何信息，请先在信息列表选择框中选中您需要删除的项！");
            }
            else
            {
                WesternMedicineInfo Sel_Info = new WesternMedicineInfo("","");
                Sel_Info = lv.SelectedItem as WesternMedicineInfo;
                if (Sel_Info.WesternMedicineType == "西医病名名称")
                    xlbh = "b";
                if (Sel_Info.WesternMedicineType == "既往史名称")
                    xlbh = "a";
                if (Sel_Info.WesternMedicineType == "主诉名称")
                    xlbh = "6";
                if (Sel_Info.WesternMedicineType == "主诉时间名称")
                    xlbh = "9";
                if (Sel_Info.WesternMedicineType == "初起症象名称")
                    xlbh = "7";
                if (Sel_Info.WesternMedicineType == "现症象名称")
                    xlbh = "8";
                string sql = String.Format("select subid from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '{1}' and xxbh = '{2}'", ID, xlbh, Sel_Info.WesternMedicineName);
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    subid = dr["subid"].ToString();
                    
                }
                dr.Close();
                conn.Close();
                if(Sel_Info.WesternMedicineType.Trim()=="西医病名名称")
                {
                    sql = String.Format("select xybmbh from t_info_xybm where xybmmc ='{0}'", Sel_Info.WesternMedicineName);
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
                    sql = String.Format("select zxbh from t_info_zxmx where zxmc ='{0}'", Sel_Info.WesternMedicineName);
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
                sql = String.Format("delete from t_bl_mx where subid ='{0}'",subid);
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
                if(xlbh == "6") 
                xlbh = "0"; //主诉
                else if (xlbh == "7")
                xlbh = "1";
                else if (xlbh == "8") 
                xlbh = "2";
                else if (xlbh == "9")
                xlbh = "3";
                else if (xlbh== "a")
                xlbh = "4";
                else if (xlbh== "b")  
                xlbh = "5";
                sql = String.Format("delete from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '{1}' and xxbh = '{2}'", ID, xlbh, bh);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count1 = comm.ExecuteNonQuery();
                if (count1 > 0)
                {
                    MessageBox.Show("删除编号成功");
                }
                dr.Close();
                conn.Close();
                listSymptom.RemoveAt(lv.SelectedIndex);
            }
        }

        private void selectbs_Click(object sender, RoutedEventArgs e)
        {
            if (isadd == false)
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

        private void selxybm_Click(object sender, RoutedEventArgs e)
        {
            if (isadd == false)
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
        /// 功能：实现病名名称的读取和显示
        /// </summary>
        private void ReceiveValues(object sender, XYBMsel.PassValuesEventArgs e)
        {
            string sql = String.Format("select count(*) from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '5' and xxbh = '{1}'", ID, e.Number);
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
                lv.ItemsSource = listSymptom;
                listSymptom.Add(new WesternMedicineInfo("西医病名名称", e.Name));
                sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', '5', '{0}')", e.Number,ID);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count1 = comm.ExecuteNonQuery();
                if (count1 > 0)
                {

                    MessageBox.Show("插入西医病名编号成功");
                }
                dr.Close();
                conn.Close();
                sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', 'b','{0}')", e.Name,ID);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count1 = comm.ExecuteNonQuery();
                if (count1 > 0)
                {

                    MessageBox.Show("插入西医病名成功");
                }
                dr.Close();
                conn.Close();
            }
        }
        /// <summary>
        /// 功能：实现病名名称的读取和显示
        /// </summary>
        private void ReceiveValues1(object sender, selectbs.PassValuesEventArgs e)
        {
            string sql = String.Format("select count(*) from t_bl_mx where id = '{0}' and xxdllx = '0' and xxxllx = '4' and xxbh = '{1}'", ID, e.Number);
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
                lv.ItemsSource = listSymptom;
                listSymptom.Add(new WesternMedicineInfo("既往史名称", e.Name));
                sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', '4', '{0}')", e.Number,ID);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                int count1 = comm.ExecuteNonQuery();
                if (count1 > 0)
                {

                    MessageBox.Show("插入病史编号成功");
                }
                dr.Close();
                conn.Close();
                sql = String.Format("Insert into t_bl_mx (id,xxdllx,xxxllx,xxbh) values ('{1}','0', 'a','{0}')", e.Name,ID);
                conn.Open();
                comm = new SqlCommand(sql, conn);
                count1 = comm.ExecuteNonQuery();
                if (count1 > 0)
                {

                    MessageBox.Show("插入病史成功");
                }
                dr.Close();
                conn.Close();
            }
        }
        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
