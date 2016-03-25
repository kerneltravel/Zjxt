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
    /// Interaction logic for MainInterface.xaml
    /// </summary>
    public partial class MainInterface : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);

        public MainInterface()
        {
            InitializeComponent();
        }

        private bool Access_Permission(string function) 
        {
            // 获取权限
            string sql = String.Format("SELECT count(*) FROM ([zjxtserver].[dbo].[t_xtgl_czry] t1 inner join [zjxtserver].[dbo].[t_info_qxz] t2 on t1.qx = t2.qxzmc) inner join  [zjxtserver].[dbo].[t_info_qxzmx] t3 on t3.id = t2.subid where t1.name = '{0}' and t3.qxck = '{1}'"
                , Application.Current.Properties["user_name"].ToString(), function);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            conn.Close();
            if (count == 0)
            {
                MessageBox.Show("您没有操作权限！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            else
            {
                return true;
            }                                           
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void maxButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void mniButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void tabSteps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 功能：【用户管理】
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("用户管理"))
            {
                UserManagement usermanagement = new UserManagement();
                usermanagement.Show();
                this.Visibility = 0;
            }
        }

        private void tabback_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabback.SelectedIndex == 6) 
            {
                // 关闭当前所有窗口
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// 功能：【个人信息管理】
        /// </summary>
        private void Button_Click_PersonInfo(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("个人信息管理"))
            {
                PersonalInfoAdmin personalinfoadmin = new PersonalInfoAdmin();
                personalinfoadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【用户权限组管理】
        /// </summary>
        private void qxzgl_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("用户权限组管理"))
            {
                QxAdmin qxadmin = new QxAdmin();
                qxadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【症象信息管理】
        /// </summary>
        private void Button_Click_Symptom(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("症象信息管理"))
            {
                Info_Symptom info_symptom = new Info_Symptom();
                info_symptom.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【系信息管理】
        /// </summary>
        private void Button_Click_Xi(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("系信息管理"))
            {
                XiInfoAdmin xiinfoadmin = new XiInfoAdmin();
                xiinfoadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【病名信息管理】
        /// </summary>
        private void Disease_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("病名信息管理"))
            {
                DiseaseInfoAdmin diseaseinfoadmin = new DiseaseInfoAdmin();
                diseaseinfoadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【基本病机信息管理】
        /// </summary>
        private void BasicBingji_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("基本病机信息管理"))
            {
                BasicBingji basicbingji = new BasicBingji();
                basicbingji.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【复合病机信息管理】
        /// </summary>
        private void FuheBingji_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("复合病机信息管理"))
            {
                FuheBingji fuhebingji = new FuheBingji();
                fuhebingji.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【基本证名信息管理】
        /// </summary>
        private void BasicZhengm_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("基本证名信息管理"))
            {
                BasicZhengm basiczhengm = new BasicZhengm();
                basiczhengm.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【西医证治管理】
        /// </summary>
        private void Button_Click_west(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("西医证治管理"))
            {
                WesternMedicine_manage west = new WesternMedicine_manage();
                west.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【外感规则管理】
        /// </summary>
        private void WaigRule_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("外感规则管理"))
            {
                WaigRuleAdmin waigruleadmin = new WaigRuleAdmin();
                waigruleadmin.Show();
                this.Visibility = 0;
            }
        }
         
        /// <summary>
        /// 功能：【系规则管理】
        /// </summary>
        private void XiRule_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("系规则管理"))
            {
                XiRuleAdmin xiruleadmin = new XiRuleAdmin();
                xiruleadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【病名规则管理】
        /// </summary>
        private void BingmRule_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("病名规则管理"))
            {
                BingmRuleAdmin bingmruleadmin = new BingmRuleAdmin();
                bingmruleadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【基本病机规则管理】
        /// </summary>
        private void BasicBjRule_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("基本病机规则管理"))
            {
                BasicBjRuleAdmin basicbjruleadmin = new BasicBjRuleAdmin();
                basicbjruleadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【复合病机规则管理】
        /// </summary>
        private void FuheBjRule_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("复合病机规则管理"))
            {
                FuheBjRuleAdmin fuhebjruleadmin = new FuheBjRuleAdmin();
                fuhebjruleadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【基本证名规则管理】
        /// </summary>
        private void BasicZmRule_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("基本证名规则管理"))
            {
                BasicZmRuleAdmin basiczmruleadmin = new BasicZmRuleAdmin();
                basiczmruleadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【证名合并规则管理】
        /// </summary>
        private void ZhengmhbRule_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("证名合并规则管理"))
            {
                ZhengmhbRuleAdmin zhengmhbruleadmin = new ZhengmhbRuleAdmin();
                zhengmhbruleadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【基本证名－病名管理】
        /// </summary>
        private void BasicZhengmbmRule_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("基本证名－病名管理"))
            {
                BasicZhengmbmRuleAdmin basiczhengmbmruleadmin = new BasicZhengmbmRuleAdmin();
                basiczhengmbmruleadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【基本处方信息管理】
        /// </summary>
        private void JibenCF_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("基本处方信息管理"))
            {
                JibenCF jibencf = new JibenCF();
                jibencf.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【药物信息管理】
        /// </summary>
        private void MedicineInfoAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("药物信息管理"))
            {
                MedicineInfoAdmin medicineinfoadmin = new MedicineInfoAdmin();
                medicineinfoadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【处方组成信息管理】
        /// </summary>
        private void ChufzcInfoAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("处方组成信息管理"))
            {
                ChufzcInfoAdmin chufzcinfoadmin = new ChufzcInfoAdmin();
                chufzcinfoadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【对证基本处方规则管理】
        /// </summary>
        private void DuizBasiccfRuleAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("对证基本处方规则管理"))
            {
                DuizBasiccfRuleAdmin duizbasiccfruleadmin = new DuizBasiccfRuleAdmin();
                duizbasiccfruleadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【反药物规则管理】
        /// </summary>
        private void FanywRuleAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("反药物规则管理"))
            {
                FanywRuleAdmin fanywruleadmin = new FanywRuleAdmin();
                fanywruleadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【修改处方规则管理】
        /// </summary>
        private void ModifycfRuleAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("修改处方规则管理"))
            {
                ModifycfRuleAdmin modifycfruleadmin = new ModifycfRuleAdmin();
                modifycfruleadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【修改药物规则管理】
        /// </summary>
        private void ModifyywRuleAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("修改药物规则管理"))
            {
                ModifyywRuleAdmin modifyywruleadmin = new ModifyywRuleAdmin();
                modifyywruleadmin.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【临床诊断】
        /// </summary>
        private void Button_Click_clinicaldiagnosis(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("临床诊断"))
            {
                ClinicalDiagnosis clinical = new ClinicalDiagnosis();
                clinical.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【复诊】
        /// </summary>
        private void Referral_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("复诊"))
            {
                Referral referral = new Referral();
                referral.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【模拟诊断】
        /// </summary>
        private void Virtual_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("模拟诊断"))
            {
                VirtualDiagnose virtualdiagnose = new VirtualDiagnose();
                virtualdiagnose.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【经典案例检索】
        /// </summary>
        private void ClassiccaseSearch_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("经典案例检索"))
            {
                ClassiccaseSearch classiccasesearch = new ClassiccaseSearch();
                classiccasesearch.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【病人信息查询】
        /// </summary>
        private void PatientinfoSearch_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("病人信息查询"))
            {
                PatientinfoSearch patientinfosearch = new PatientinfoSearch();
                patientinfosearch.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【临床病历检索】
        /// </summary>
        private void ClinicalcaseSearch_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("临床病历检索"))
            {
                ClinicalcaseSearch clinicalcasesearch = new ClinicalcaseSearch();
                clinicalcasesearch.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【病名下证名查询】
        /// </summary>
        private void ZhengmSearch_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("病名下证名查询"))
            {
                ZhengmSearch zhengmsearch = new ZhengmSearch();
                zhengmsearch.Show();
                this.Visibility = 0;
            }
        }

        /// <summary>
        /// 功能：【基本证名联动查询】
        /// </summary>
        private void ZhengmldSearch_Click(object sender, RoutedEventArgs e)
        {
            if (Access_Permission("基本证名联动查询"))
            {
                ZhengmldSearch zhengmldsearch = new ZhengmldSearch();
                zhengmldsearch.Show();
                this.Visibility = 0;
            }
        }

        private void Button_Click_season(object sender, RoutedEventArgs e)
        {
            Season se = new Season();
            se.Show();
            this.Visibility = 0;

        }

        private void Button_Click_ruke(object sender, RoutedEventArgs e)
        {
            rukeInfo ruke = new rukeInfo();
            ruke.Show();
            this.Visibility = 0;

        }

        /// <summary>
        /// 功能：【入科规则管理】
        /// </summary>
        private void RukeRule_Click(object sender, RoutedEventArgs e)
        {
            RukeRule rukerule = new RukeRule();
            rukerule.Show();
            this.Visibility = 0;
        }

        /// <summary>
        /// 功能：【多级复合病机规则管理】
        /// </summary>
        private void DuojifuheBjRule_Click(object sender, RoutedEventArgs e)
        {
            DuojifuheBjRule duojifuhebjrule = new DuojifuheBjRule();
            duojifuhebjrule.Show();
            this.Visibility = 0;
        }

        /// <summary>
        /// 功能：【证用病机规则管理】
        /// </summary>
        private void ZhengybjRule_Click(object sender, RoutedEventArgs e)
        {
            ZhengybjRule zhengybjrule = new ZhengybjRule();
            zhengybjrule.Show();
            this.Visibility = 0;
        }

        private void Button_Click_dj(object sender, RoutedEventArgs e)
        {
            djfhbjInfo dj = new djfhbjInfo();
            dj.Show();
            this.Visibility = 0;

        }

        private void Button_Click_zy(object sender, RoutedEventArgs e)
        {
            zybjInfo zy = new zybjInfo();
            zy.Show();
            this.Visibility = 0;
        }
    }
}
