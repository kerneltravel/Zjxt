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
    /// Interaction logic for WesternMedicine_manage.xaml
    /// </summary>
    public partial class WesternMedicine_manage : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 创建集合实例
        ObservableCollection<WesternMedicineInfo> listSymptom = new ObservableCollection<WesternMedicineInfo>();
        ObservableCollection<WesternMedicineZMInfo> listSymptom1 = new ObservableCollection<WesternMedicineZMInfo>();
        ObservableCollection<BasicZhengmbmInfo> listSymptom2 = new ObservableCollection<BasicZhengmbmInfo>();
        // 全局变量，用于存储症象信息
        WesternMedicineInfo Symptom_Display = new WesternMedicineInfo("", "", "");

        //Node node = new Node();
        // 创建 List 集合实例
        List<Node> nodes = new List<Node>();

        Hashtable httree = new Hashtable(9000);
        bool IsEmpty = true;
        bool isadd = false;
        bool isedit = false;
        bool IsRepeat = false;
        bool is_xylx_add = false;
        bool is_xylx_edit = false;
        int nochange_item = 0;
        string nochange_name = "";
        string zxlxforsave = "";
        string zxbhforsave = "";
        string zxmcforsave = "";
        // 全局变量，用于暂时存储辅证编号
        public string m_bmbh = "";
        bool ischange = true;
        bool IsAdd = false;
        bool IsModify = false;
        bool isdel = false;
        int tisel = 0;
        public WesternMedicine_manage()
        {
            InitializeComponent();
            //savebutton.IsEnabled = false;
            // 将数据库数据写入 List 集合
            // 一级树写入           
            reftree();
            lv.ItemsSource = listSymptom;
            lv1.ItemsSource = listSymptom1;
            lv2.ItemsSource = listSymptom2;
            listSymptom1.Clear();
            listSymptom.Clear();
            string XYBMLXBH = "01";
            XyBmBh.Text = "1401001";
            XyBmLx.Text = "病毒性传染病";
            int nodenum = -1;
            Node ZXMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == Convert.ToInt64(XYBMLXBH))
                {
                    listSymptom.Add(new WesternMedicineInfo("病毒性传染病", newnode.ID.ToString(), newnode.Name.Substring(newnode.Name.IndexOf(' ') + 2, newnode.Name.Length - 2 - newnode.Name.IndexOf(' '))));
                    nodenum++;
                    if (nodenum == 0)
                        ZXMC = newnode;
                }

            }
            XyBmMc.Text =ZXMC.Name.Substring(ZXMC.Name.IndexOf(' ') + 2, ZXMC.Name.Length - 2 - ZXMC.Name.IndexOf(' '));
            lv.SelectedIndex = 0;
            xybmlxsy.IsEnabled = false;
            xybmlxwy.IsEnabled = true;
            xybmlxfront.IsEnabled = true;
            xybmlxback.IsEnabled = false;
            addxybmlx.IsEnabled = true;
            editxybmlx.IsEnabled = true;
            savexybmlx.IsEnabled = false;
            backxybmlx.IsEnabled = false;
            shuaxinxybmlx.IsEnabled = true;
            editxybmbh.IsEnabled = true;
            savexybmbh.IsEnabled = false;
            backxybmbh.IsEnabled = false;
            refxybmbh.IsEnabled = true;


        }

        /// <summary>
        /// 功能：创建症象信息类
        /// 说明：1.SymptomTypes-->症象类型名称 SymptomNumber-->症象编号 SymptomName-->症象名称
        ///       2.
        /// </summary>
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
            private string _WesternMedicineNumber;
            private string _WesternMedicineName;

            public string WesternMedicineType
            {
                get { return _WesternMedicineType; }
                set { _WesternMedicineType = value; OnPropertyChanged(new PropertyChangedEventArgs("WesternMedicineType")); }
            }

            public string WesternMedicineNumber
            {
                get { return _WesternMedicineNumber; }
                set { _WesternMedicineNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("WesternMedicineNumber")); }
            }

            public string WesternMedicineName
            {
                get { return _WesternMedicineName; }
                set { _WesternMedicineName = value; OnPropertyChanged(new PropertyChangedEventArgs("WesternMedicineName")); }
            }

            public WesternMedicineInfo(string WesternMedicinetype, string WesternMedicinenumber, string WesternMedicinename)
            {
                _WesternMedicineType = WesternMedicinetype;
                _WesternMedicineNumber = WesternMedicinenumber;
                _WesternMedicineName = WesternMedicinename;
            }
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
            private string _XYZMMC;
            private string _XYZF;
            private string _XYCFMC;
            private string _FF;

            public string XYZMBH
            {
                get { return _XYZMBH; }
                set { _XYZMBH = value; OnPropertyChanged(new PropertyChangedEventArgs("XYZMBH")); }
            }

            public string XYZMMC
            {
                get { return _XYZMMC; }
                set { _XYZMMC = value; OnPropertyChanged(new PropertyChangedEventArgs("XYZMMC")); }
            }

            public string XYZF
            {
                get { return _XYZF; }
                set { _XYZF = value; OnPropertyChanged(new PropertyChangedEventArgs("XYZF")); }
            }

            public string XYCFMC
            {
                get { return _XYCFMC; }
                set { _XYCFMC = value; OnPropertyChanged(new PropertyChangedEventArgs("XYCFMC")); }
            }

            public string FF
            {
                get { return _FF; }
                set { _FF = value; OnPropertyChanged(new PropertyChangedEventArgs("FF")); }
            }

            public WesternMedicineZMInfo(string XYZMBH, string XYZMMC, string XYZF, string XYCFMC, string FF)
            {
                _XYZMBH = XYZMBH;
                _XYZMMC = XYZMMC;
                _XYZF = XYZF;
                _XYCFMC = XYCFMC;
                _FF = FF;
            }
        }

        /// <summary>
        /// 功能：创建基本证名信息类
        /// 说明：1.JibzmNumber --> 基本证名编号  JibzmName --> 基本证名名称  BingmNumber --> 病名编号
        ///         BingmName --> 病名名称 
        /// </summary>
        public class BasicZhengmbmInfo : INotifyPropertyChanged
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

            private string _JibzmNumber;
            private string _JibzmName;
            private string _BingmNumber;
            private string _BingmName;

            public string JibzmNumber
            {
                get { return _JibzmNumber; }
                set { _JibzmNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("JibzmNumber")); }
            }

            public string JibzmName
            {
                get { return _JibzmName; }
                set { _JibzmName = value; OnPropertyChanged(new PropertyChangedEventArgs("JibzmName")); }
            }

            public string BingmNumber
            {
                get { return _BingmNumber; }
                set { _BingmNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("BingmNumber")); }
            }

            public string BingmName
            {
                get { return _BingmName; }
                set { _BingmName = value; OnPropertyChanged(new PropertyChangedEventArgs("BingmName")); }
            }


            public BasicZhengmbmInfo(string jibzmnumber, string jibzmname, string bingmnumber, string bingmname)
            {
                _JibzmNumber = jibzmnumber;
                _JibzmName = jibzmname;
                _BingmNumber = bingmnumber;
                _BingmName = bingmname;
            }
        }
        /// <summary>
        /// 功能：设置文本只读
        /// </summary>
        public void Text_Readonly()
        {
            XyBmLx.IsReadOnly = true;
            XyBmBh.IsReadOnly = true;
            XyBmMc.IsReadOnly = true;
            text_box_ff.IsReadOnly = true;
            text_box_xycfmc.IsReadOnly = true;
            text_box_xyzf.IsReadOnly = true;
            text_box_xyzmbh.IsReadOnly = true;
            text_box_xyzmmc.IsReadOnly = true;
        }
        public void reftree()
        {
            httree.Clear();
            nodes.Clear();
            string sql = String.Format("select * from t_info_xybmlx");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node { ID = Convert.ToInt32(dr["xybmlxbh"]), Name = dr["xybmlxmc"].ToString().Trim() });
            }
            dr.Close();
            conn.Close();

            // 二级树写入
            sql = String.Format("select t1.xybmlxbh ,t2.xybmbh,t2.xybmmc from (t_info_xybmlx as t1 inner join t_info_xybm as t2 on t2.xybmlxbh = t1.xybmlxbh)");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                //？？？ 有问题，数据库中的 zxlxbh = 30 对应的 zxbh 为空会出现问题，赋值为0问题解决
                nodes.Add(new Node { ID = Convert.ToInt32(dr["xybmbh"]), Name = dr["xybmbh"].ToString() + "  " + dr[2].ToString(), ParentID = Convert.ToInt32(dr["xybmlxbh"]) });

            }
            dr.Close();
            conn.Close();

            BuildENTree();
            Text_Readonly();
        }
        /// <summary>
        /// 功能：创建节点类
        /// </summary>
        public class Node
        {
            public Node()
            {
                this.Nodes = new List<Node>();
                this.ParentID = -1;
            }
            public int ID { get; set; }
            public string Name { get; set; }
            public int ParentID { get; set; }
            public List<Node> Nodes { get; set; }
        }
        public void BuildENTree()
        {
            Node parentnode1 = new Node();
            List<Node> outputList = new List<Node>();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = new Node();
                newnode.ID = nodes[i].ID;
                newnode.Name = nodes[i].Name;
                newnode.ParentID = nodes[i].ParentID;
                httree.Add(newnode.ID, newnode);
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                Node parentnode = (Node)httree[newnode.ParentID];
                if (parentnode != null)
                {
                    parentnode.Nodes.Add(newnode);
                    //if ((parentnode1.ID != parentnode.ID) && parentnode.ParentID == -1)
                    //{
                    //    outputList.Add(parentnode);
                    //    parentnode1.ID = parentnode.ID;
                    //}
                }
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                Node parentnode = (Node)httree[newnode.ParentID];
                if(parentnode == null)
                    outputList.Add(newnode);
            }
            this.treeview1.ItemsSource = outputList;
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            if(isadd)
            {
                MessageBox.Show("请先保存当前操作！");
            }
            else
            {
                isadd = true;
                int XYBMLXBH=0;
                int maxid=0;
                Node ZXMC = new Node();
                for (int i = 0; i < nodes.Count; i++)
                {
                    Node newnode = (Node)httree[nodes[i].ID];
                    if (newnode.Name == XyBmLx.Text.Trim())
                    {
                        XYBMLXBH = newnode.ID;
                        break;
                    }
                }
                for (int i = 0; i < nodes.Count; i++)
                {
                    Node newnode = (Node)httree[nodes[i].ID];
                    if (newnode.ParentID == XYBMLXBH)
                    {
                        if (maxid < newnode.ID)
                            maxid = newnode.ID;
                    }
                }
                listSymptom.Add(new WesternMedicineInfo(XyBmLx.Text,(maxid+1).ToString(), ""));
                lv.ItemsSource = listSymptom;
                lv.SelectedIndex = lv.Items.Count - 1;
                XyBmBh.Text = (maxid + 1).ToString();
                XyBmMc.IsReadOnly = false;
                xybmlxsy.IsEnabled = false;
                xybmlxback.IsEnabled = false;
                xybmlxfront.IsEnabled = false;
                xybmlxwy.IsEnabled = false;
                editxybmbh.IsEnabled = false;
                addxybmbh.IsEnabled = false;
                del_xybmbh.IsEnabled = false;
                addxybmlx.IsEnabled = false;
                editxybmlx.IsEnabled = false;
                backxybmbh.IsEnabled = true;
                savexybmbh.IsEnabled = true;
                del_xybmlx.IsEnabled = false;
            }
            
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            int Isdel = 0, nodenum = -1;
            bool IsEmpty = true;
            Node ZXMC = new Node();
            int XYLXBH = 0;
            string xylxbh = "";
            lv.ItemsSource = listSymptom;
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.Name.Trim() == XyBmLx.Text.Trim())
                {
                    XYLXBH = newnode.ID;
                    break;
                }
            }
            if (XYLXBH < 10)
                xylxbh = "0" + XYLXBH.ToString();
            else
                xylxbh = XYLXBH.ToString();
            if (MessageBox.Show("确定要删除该项吗？", "确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    string sql = String.Format("delete from t_info_xybm where xybmbh = '{0}'", XyBmBh.Text);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    if (count > 0)
                    {
                        MessageBox.Show("删除成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        Isdel++;
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
                if(Isdel>=1)
                {
                    if (lv.SelectedIndex == lv.Items.Count - 1)
                    {
                        lv.SelectedIndex--;
                        listSymptom.RemoveAt(lv.SelectedIndex + 1);
                    }
                    else if (lv.Items.Count == 1)
                    {
                        while (IsEmpty)
                        {
                            IsEmpty = true;
                            XYLXBH = Convert.ToInt16(xylxbh) - 1;
                            for (int i = 0; i < nodes.Count; i++)
                            {
                                Node newnode = (Node)httree[nodes[i].ID];
                                if (newnode.ID == XYLXBH)
                                {
                                    XyBmLx.Text = newnode.Name;
                                    break;
                                }
                            }
                            for (int i = 0; i < nodes.Count; i++)
                            {

                                Node newnode = (Node)httree[nodes[i].ID];
                                if (newnode.ParentID == XYLXBH)
                                {
                                    IsEmpty = false;
                                    listSymptom.Add(new WesternMedicineInfo(XyBmLx.Text, newnode.ID.ToString(), newnode.Name.Substring(newnode.Name.IndexOf(' ') + 2, newnode.Name.Length - 2 - newnode.Name.IndexOf(' '))));
                                    nodenum++;
                                    if (nodenum == 0)
                                        ZXMC = newnode;
                                }

                            }
                        }
                        XyBmMc.Text = ZXMC.Name.Substring(ZXMC.Name.IndexOf(' ') + 2, ZXMC.Name.Length - 2 - ZXMC.Name.IndexOf(' '));
                        XyBmBh.Text = ZXMC.ID.ToString();
                        lv.SelectedIndex = 0;
                    }
                    else
                    {
                        lv.SelectedIndex++;
                        listSymptom.RemoveAt(lv.SelectedIndex - 1);
                    }
                    
                   
                    }
                    

                
                reftree();
            }                        
        }

        private void XyBmMc_LostFocus(object sender, RoutedEventArgs e)
        {

            if (isadd || isedit)
            {
                WesternMedicineInfo Sel_Info = new WesternMedicineInfo("", "","");
                Sel_Info = lv.SelectedItem as WesternMedicineInfo;
                Sel_Info.WesternMedicineName = XyBmMc.Text;
            }

        }

        private void editxybmbh_Click(object sender, RoutedEventArgs e)
        {

            if(isedit)
            {
                MessageBox.Show("请先保存当前操作！");
            }
            else
            {
                nochange_item = lv.SelectedIndex;
                isedit = true;
                XyBmMc.IsReadOnly = false;
                xybmlxsy.IsEnabled = false;
                xybmlxback.IsEnabled = false;
                xybmlxfront.IsEnabled = false;
                xybmlxwy.IsEnabled = false;
                editxybmbh.IsEnabled = false;
                addxybmbh.IsEnabled = false;
                del_xybmbh.IsEnabled = false;
                addxybmlx.IsEnabled = false;
                editxybmlx.IsEnabled = false;
                backxybmbh.IsEnabled = true;
                savexybmbh.IsEnabled = true;
                del_xybmlx.IsEnabled = false;
            }
        }

        private void savexybmbh_Click(object sender, RoutedEventArgs e)
        {
            int XYBMLXBH = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.Name == XyBmLx.Text.Trim())
                {
                    XYBMLXBH = newnode.ID;
                    break;
                }
            }
            string xylxbh = "";
            if (XYBMLXBH < 10)
                xylxbh = "0" + XYBMLXBH.ToString();
            else
                xylxbh = XYBMLXBH.ToString();
            Is_Repeat1();
            Text_Readonly();
            if (XyBmMc.Text == "")
                MessageBox.Show("西医病名名称不能为空！");
            if (IsRepeat == false && XyBmMc.Text != "")
            {
                if (isedit == true)
                {
                    try
                    {
                        Node newnode = new Node();
                        //for (int i = 0; i < nodes.Count; i++)
                        //{
                        //    newnode = (Node)httree[nodes[i].ID];
                        //    if (newnode.Name.Trim() == nochange_name.Trim())
                        //    {
                        //        break;
                        //    }

                        //}
                        string sql = String.Format("UPDATE t_info_xybm SET xybmlxbh='{0}',xybmmc='{1}' WHERE xybmbh='{2}'", xylxbh, XyBmMc.Text, XyBmBh.Text);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                        if (count > 0)
                        {
                            MessageBox.Show("修改成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            isedit = false;
                            backxybmbh.IsEnabled = false;
                            savexybmbh.IsEnabled = false;
                            addxybmlx.IsEnabled = true;
                            editxybmlx.IsEnabled = true;
                            xybmlxsy.IsEnabled = true;
                            xybmlxback.IsEnabled = true;
                            xybmlxfront.IsEnabled = true;
                            xybmlxwy.IsEnabled = true;
                            del_xybmlx.IsEnabled = true;
                            del_xybmbh.IsEnabled = true;
                            addxybmbh.IsEnabled = true;
                            editxybmbh.IsEnabled = true;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("修改失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        conn.Close();
                    }
                    
                }
                    if (isadd == true)
                    {
                        
                        try
                        {
                            string sql = String.Format("INSERT INTO t_info_xybm (xybmlxbh,xybmbh,xybmmc) VALUES ('{0}', '{1}','{2}')", xylxbh, XyBmBh.Text, XyBmMc.Text);
                            conn.Open();
                            SqlCommand comm = new SqlCommand(sql, conn);
                            int count = comm.ExecuteNonQuery();
                            if (count > 0)
                            {
                                MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                                isadd = false;
                                backxybmbh.IsEnabled = false;
                                savexybmbh.IsEnabled = false;
                                addxybmlx.IsEnabled = true;
                                editxybmlx.IsEnabled = true;
                                xybmlxsy.IsEnabled = true;
                                xybmlxback.IsEnabled = true;
                                xybmlxfront.IsEnabled = true;
                                xybmlxwy.IsEnabled = true;
                                del_xybmlx.IsEnabled = true;
                                del_xybmbh.IsEnabled = true;
                                addxybmbh.IsEnabled = true;
                                editxybmbh.IsEnabled = true;
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
                    XyBmMc.IsReadOnly = true;
                    reftree();
                }
            
        }

        private void backxybmbh_Click(object sender, RoutedEventArgs e)
        {
            if (isadd || isedit)
            {
                isadd = false;
                isedit = false;
                reftree();
                backxybmbh.IsEnabled = false;
                savexybmbh.IsEnabled = false;
                addxybmlx.IsEnabled = true;
                editxybmlx.IsEnabled = true;
                xybmlxsy.IsEnabled = true;
                xybmlxback.IsEnabled = true;
                xybmlxfront.IsEnabled = true;
                xybmlxwy.IsEnabled = true;
                del_xybmlx.IsEnabled = true;
                del_xybmbh.IsEnabled = true;
                addxybmbh.IsEnabled = true;
                editxybmbh.IsEnabled = true;
            }
        }

        private void refxybmbh_Click(object sender, RoutedEventArgs e)
        {
            reftree();
        }

        private void xybmlxsy_Click(object sender, RoutedEventArgs e)
        {
            listSymptom.Clear();
            string XYBMLXBH = "01";
            XyBmBh.Text = "1401001";
            XyBmLx.Text = "病毒性传染病";
            int nodenum = -1;
            Node ZXMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == Convert.ToInt64(XYBMLXBH))
                {
                    listSymptom.Add(new WesternMedicineInfo("病毒性传染病", newnode.ID.ToString(), newnode.Name.Substring(newnode.Name.IndexOf(' ') + 2, newnode.Name.Length - 2 - newnode.Name.IndexOf(' '))));
                    nodenum++;
                    if (nodenum == 0)
                        ZXMC = newnode;
                }
            }
            if (nodenum == -1)
            {
                XyBmMc.Text = "";
                XyBmBh.Text = "";
            }
            else
            {
                XyBmMc.Text = ZXMC.Name.Substring(ZXMC.Name.IndexOf(' ') + 2, ZXMC.Name.Length - 2 - ZXMC.Name.IndexOf(' '));
                lv.SelectedIndex = 0;
            }
            xybmlxsy.IsEnabled = false;
            xybmlxwy.IsEnabled = true;
            xybmlxfront.IsEnabled = true;
            xybmlxback.IsEnabled = false;
        }

        private void xybmlxback_Click(object sender, RoutedEventArgs e)
        {
            IsEmpty = true;
            int XYLXBH = 0;
            lv.ItemsSource = listSymptom;
            listSymptom.Clear();
            int nodenum = -1;
            Node XYBMMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.Name.Trim() == XyBmLx.Text.Trim())
                {
                    XYLXBH = newnode.ID;
                    break;
                }

            }
            while(IsEmpty)
            {
                XYLXBH--;
                string XYBH = "";
                if (XYLXBH < 10)
                    XYBH = "140" + XYLXBH + "001";
                //else if (ZXLXBH == 29)
                //    ZXBH = "01" + XYLXBH + "002";
                else
                    XYBH = "14" + XYLXBH + "001";
                for (int i = 0; i < nodes.Count; i++)
                {
                    Node newnode = (Node)httree[nodes[i].ID];
                    if (newnode.ID == Convert.ToInt64(XYLXBH))
                    {
                        XyBmLx.Text = newnode.Name.Trim();
                        IsEmpty = false;
                    }

                }
            }
            
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == Convert.ToInt64(XYLXBH))
                {
                    listSymptom.Add(new WesternMedicineInfo(XyBmLx.Text, newnode.ID.ToString(), newnode.Name.Substring(newnode.Name.IndexOf(' ') + 2, newnode.Name.Length - 2 - newnode.Name.IndexOf(' '))));
                    nodenum++;
                    if (nodenum == 0)
                        XYBMMC = newnode;
                }

            }
            if (nodenum == -1)
            {
                XyBmMc.Text = "";
                XyBmBh.Text = "";
            }
            else
            {
                XyBmBh.Text = XYLXBH.ToString();
                XyBmMc.Text = XYBMMC.Name.Substring(XYBMMC.Name.IndexOf(' ') + 2, XYBMMC.Name.Length - 2 - XYBMMC.Name.IndexOf(' '));
                lv.SelectedIndex = 0;
            }
            if (XyBmLx.Text.Trim() == "病毒性传染病")
            {
                xybmlxsy.IsEnabled = false;
                xybmlxback.IsEnabled = false;
                xybmlxfront.IsEnabled = true;
                xybmlxwy.IsEnabled = true;
            }
            else
            {
                xybmlxback.IsEnabled = true;
                xybmlxsy.IsEnabled = true;
                xybmlxfront.IsEnabled = true;
                xybmlxwy.IsEnabled = true;
            }
        }

        private void xybmlxfront_Click(object sender, RoutedEventArgs e)
        {
            IsEmpty = true;
            int XYLXBH = 0;
            int maxid = 0;
            lv.ItemsSource = listSymptom;
            listSymptom.Clear();
            int nodenum = -1;
            Node XYBMMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.Name.Trim() == XyBmLx.Text.Trim())
                {
                    XYLXBH = newnode.ID;
                    break;
                }

            }
            while (IsEmpty)
            {
                XYLXBH++;
                string XYBH = "";
                if (XYLXBH < 10)
                    XYBH = "140" + XYLXBH + "001";
                //else if (ZXLXBH == 29)
                //    ZXBH = "01" + XYLXBH + "002";
                else
                    XYBH = "14" + XYLXBH + "001";
                for (int i = 0; i < nodes.Count; i++)
                {
                    Node newnode = (Node)httree[nodes[i].ID];
                    if (newnode.ID == Convert.ToInt64(XYLXBH))
                    {
                        XyBmLx.Text = newnode.Name.Trim();
                        IsEmpty = false;
                    }

                }
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == Convert.ToInt64(XYLXBH))
                {
                    listSymptom.Add(new WesternMedicineInfo(XyBmLx.Text, newnode.ID.ToString(), newnode.Name.Substring(newnode.Name.IndexOf(' ') + 2, newnode.Name.Length - 2 - newnode.Name.IndexOf(' '))));
                    nodenum++;
                    if (nodenum == 0)
                        XYBMMC = newnode;
                }
                if (newnode.ID > maxid && newnode.ParentID == -1)
                {
                    maxid = newnode.ID;
                }

            }
            if (nodenum == -1)
            {
                XyBmMc.Text = "";
                XyBmBh.Text = "";
            }
            else
            {
                XyBmBh.Text = XYLXBH.ToString();
                XyBmMc.Text = XYBMMC.Name.Substring(XYBMMC.Name.IndexOf(' ') + 2, XYBMMC.Name.Length - 2 - XYBMMC.Name.IndexOf(' '));
                lv.SelectedIndex = 0;
            }
            if (XYLXBH==maxid)
            {
                xybmlxsy.IsEnabled = true;
                xybmlxback.IsEnabled = true;
                xybmlxfront.IsEnabled = false;
                xybmlxwy.IsEnabled = false;
            }
            else
            {
                xybmlxback.IsEnabled = true;
                xybmlxsy.IsEnabled = true;
                xybmlxfront.IsEnabled = true;
                xybmlxwy.IsEnabled = true;
            }
        }

        private void xybmlxwy_Click(object sender, RoutedEventArgs e)
        {
            lv.ItemsSource = listSymptom;
            listSymptom.Clear();
            int nodenum = -1;
            int maxid = 0;
            Node XYBMMC = new Node();
            Node XYBMLX = new Node();
            //显示症象类型
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ID > maxid && newnode.ParentID == -1)
                {
                    XYBMLX = newnode;
                    maxid = newnode.ID;
                }

            }
            XyBmLx.Text = XYBMLX.Name;
            //根据选择的树节点决定listview的内容
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ID > maxid && newnode.ParentID == -1)
                    maxid = newnode.ID;
                if (newnode.ParentID == XYBMLX.ID)
                {
                    listSymptom.Add(new WesternMedicineInfo(XyBmLx.Text, newnode.ID.ToString(), newnode.Name.Substring(newnode.Name.IndexOf(' ') + 2, newnode.Name.Length - 2 - newnode.Name.IndexOf(' '))));
                    nodenum++;
                    if (nodenum == 0)
                        XYBMMC = newnode;
                }

            }
            if (nodenum == -1)
            {
                XyBmMc.Text = "";
                XyBmBh.Text = "";
            }
            else
            {
                XyBmMc.Text = XYBMMC.Name.Substring(XYBMMC.Name.IndexOf(' ') + 2, XYBMMC.Name.Length - 2 - XYBMMC.Name.IndexOf(' '));
                lv.SelectedIndex = 0;
                XyBmBh.Text = XYBMMC.ID.ToString();
            }
            xybmlxsy.IsEnabled = true;
            xybmlxwy.IsEnabled = false;
            xybmlxfront.IsEnabled = false;
            xybmlxback.IsEnabled = true;
        }

        private void addxybmlx_Click(object sender, RoutedEventArgs e)
        {
            int maxid = 0;
            if (is_xylx_add)
            {
                MessageBox.Show("请先保存！");
            }
            else
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    Node newnode = (Node)httree[nodes[i].ID];
                    if (newnode.ID > maxid && newnode.ParentID == -1)
                        maxid = newnode.ID;
                }
                XyBmLx.Text = "";
                XyBmBh.Text = "";
                XyBmMc.Text = "";
                XyBmLx.IsReadOnly = false;
                XyBmBh.IsReadOnly = true;
                XyBmMc.IsReadOnly = true;
                listSymptom.Clear();
                is_xylx_add = true;
                xybmlxsy.IsEnabled = false;
                xybmlxback.IsEnabled = false;
                xybmlxfront.IsEnabled = false;
                xybmlxwy.IsEnabled = false;
                editxybmbh.IsEnabled = false;
                addxybmbh.IsEnabled = false;
                del_xybmbh.IsEnabled = false;
                addxybmlx.IsEnabled = false;
                editxybmlx.IsEnabled = false;
                backxybmlx.IsEnabled = true;
                savexybmlx.IsEnabled = true;
                del_xybmlx.IsEnabled = false;
            }
        }

        private void editxybmlx_Click(object sender, RoutedEventArgs e)
        {
            if (is_xylx_edit)
            {
                MessageBox.Show("请先保存！");
            }
            else
            {

                XyBmLx.IsReadOnly = false;
                WesternMedicineInfo Sel_Info = lv.SelectedItem as WesternMedicineInfo;
                if (Sel_Info!=null)
                {
                    nochange_name = Sel_Info.WesternMedicineType;
                    Sel_Info.WesternMedicineType = XyBmLx.Text;
                }
                else
                {
                    Node item = (Node)treeview1.SelectedItem;
                    Node ZxLx = item;
                    while (ZxLx.ParentID != -1 && ZxLx != null)
                    {
                        ZxLx = (Node)httree[ZxLx.ParentID];
                        
                    }
                    nochange_name = ZxLx.Name;
                }
                
                nochange_item = lv.SelectedIndex;
                is_xylx_edit = true;
                xybmlxsy.IsEnabled = false;
                xybmlxback.IsEnabled = false;
                xybmlxfront.IsEnabled = false;
                xybmlxwy.IsEnabled = false;
                editxybmbh.IsEnabled = false;
                addxybmbh.IsEnabled = false;
                del_xybmbh.IsEnabled = false;
                addxybmlx.IsEnabled = false;
                editxybmlx.IsEnabled = false;
                backxybmlx.IsEnabled = true;
                savexybmlx.IsEnabled = true;
                del_xybmlx.IsEnabled = false;
            }
        }

        private void savexybmlx_Click(object sender, RoutedEventArgs e)
        {
            int maxid = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ID > maxid && newnode.ParentID == -1)
                    maxid = newnode.ID;
            }
            Is_Repeat();
            if (XyBmLx.Text == "")
                MessageBox.Show("类型名称不能为空！");
            if (IsRepeat== false&&XyBmLx.Text!="")
            {
                if(is_xylx_edit==true)
                {
                    try
                    {
                        Node newnode = new Node();
                        for (int i = 0; i < nodes.Count; i++)
                        {
                            newnode = (Node)httree[nodes[i].ID];
                            if (newnode.Name.Trim() == nochange_name.Trim())
                            {
                                break;
                            }

                        }
                        string sql = String.Format("UPDATE t_info_xybmlx SET xybmlxmc='{0}' WHERE xybmlxbh='{1}'", XyBmLx.Text ,newnode.ID.ToString());
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                        if (count > 0)
                        {
                            MessageBox.Show("修改成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            is_xylx_edit = false;
                            backxybmlx.IsEnabled = false;
                            savexybmlx.IsEnabled = false;
                            addxybmlx.IsEnabled = true;
                            editxybmlx.IsEnabled = true;
                            xybmlxsy.IsEnabled = true;
                            xybmlxback.IsEnabled = true;
                            xybmlxfront.IsEnabled = true;
                            xybmlxwy.IsEnabled = true;
                            del_xybmlx.IsEnabled = true;
                            del_xybmbh.IsEnabled = true;
                            addxybmbh.IsEnabled = true;
                            editxybmbh.IsEnabled = true;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("修改失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                if (is_xylx_add == true)
                {
                    try
                    {
                        string sql = String.Format("INSERT INTO t_info_xybmlx (xybmlxbh,xybmlxmc) VALUES ('{0}', '{1}')", (maxid+1).ToString(), XyBmLx.Text);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                        if (count > 0)
                        {
                            MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            is_xylx_add = false;
                            backxybmlx.IsEnabled = false;
                            savexybmlx.IsEnabled = false;
                            addxybmlx.IsEnabled = true;
                            editxybmlx.IsEnabled = true;
                            xybmlxsy.IsEnabled = true;
                            xybmlxback.IsEnabled = true;
                            xybmlxfront.IsEnabled = true;
                            xybmlxwy.IsEnabled = true;
                            del_xybmlx.IsEnabled = true;
                            del_xybmbh.IsEnabled = true;
                            addxybmbh.IsEnabled = true;
                            editxybmbh.IsEnabled = true;
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
                XyBmLx.IsReadOnly = true;
                reftree();
            }
        }
        /// <summary>
        /// 功能：防止姓名重复
        /// </summary>
        public void Is_Repeat()
        {
            string xyname = XyBmLx.Text.Trim();
            string sql = String.Format("select count(*) from t_info_xybmlx where xybmlxmc = '{0}'", xyname);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count == 1)
            {
                MessageBox.Show("该类型名称已存在！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                IsRepeat = true;
            }
            else
                IsRepeat = false;
            conn.Close();
        }
        public void Is_Repeat1()
        {
            string xyname = XyBmMc.Text.Trim();
            string sql = String.Format("select count(*) from t_info_xybm where xybmmc = '{0}'", xyname);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count == 1)
            {
                MessageBox.Show("该类型名称已存在！请重新输入！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                XyBmMc.Text = "";
                IsRepeat = true;
            }
            else
                IsRepeat = false;
            conn.Close();
        }
        public void Is_Repeat2()
        {
            string xyname = text_box_xyzmmc.Text.Trim();
            string sql = String.Format("select count(*) from t_rule_xycf where xyzmmc = '{0}'", xyname);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count == 1)
            {
                MessageBox.Show("该西医证名名称已存在！请重新输入！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                IsRepeat = true;
            }
            else
                IsRepeat = false;
            conn.Close();
        }
        public void Is_Repeat3()
        {
            string xyname = combobox_xyzmmc_input.Text.Trim();
            string bmname = bmmc.Text.Trim();
            string sql = String.Format("select count(*) from t_rule_xyzz where xyzmmc = '{0}' and xybmmc='{1}'", xyname, bmname);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count == 1)
            {
                MessageBox.Show("该西医病名已存在！请重新输入！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                IsRepeat = true;
            }
            else
                IsRepeat = false;
            conn.Close();
        }
        private void backxybmlx_Click(object sender, RoutedEventArgs e)
        {

            if(is_xylx_add||is_xylx_edit)
            {
                is_xylx_add = false;
                is_xylx_edit = false;
                reftree();
                backxybmlx.IsEnabled = false;
                savexybmlx.IsEnabled = false;
                addxybmlx.IsEnabled = true;
                editxybmlx.IsEnabled = true;
                xybmlxsy.IsEnabled = true;
                xybmlxback.IsEnabled = true;
                xybmlxfront.IsEnabled = true;
                xybmlxwy.IsEnabled = true;
                del_xybmlx.IsEnabled = true;
                del_xybmbh.IsEnabled = true;
                addxybmbh.IsEnabled = true;
                editxybmbh.IsEnabled = true;
            }
        }

        private void shuaxinxybmlx_Click(object sender, RoutedEventArgs e)
        {
            reftree();
        }

        private void del_xybmlx_Click(object sender, RoutedEventArgs e)
        {
            int Isdel = 0, nodenum=-1;
            bool IsEmpty=true;
            Node ZXMC=new Node();
            int XYLXBH = 0;
            string xylxbh = "";
            lv.ItemsSource = listSymptom;
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.Name.Trim() == XyBmLx.Text.Trim())
                {
                    XYLXBH = newnode.ID;
                    break;
                }
            }
            if (XYLXBH < 10)
                xylxbh = "0" + XYLXBH.ToString();
            else
                xylxbh =XYLXBH.ToString();
            if (MessageBox.Show("确定要删除该项吗？", "确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                listSymptom.Clear();
                try
                {
                    string sql = String.Format("delete from t_info_xybm where xybmlxbh = '{0}'", xylxbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    if (count > 0)
                    {
                        Isdel++;
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
                try
                {
                    string sql = String.Format("delete from t_info_xybmlx where xybmlxbh = '{0}'", xylxbh);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    if (count > 0)
                    {
                        Isdel++;
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
                if(Isdel>=1)
                {
                    while(IsEmpty){
                            IsEmpty = true;
                            XYLXBH = Convert.ToInt16(xylxbh) - 1;
                            for (int i = 0; i < nodes.Count; i++)
                            {
                                Node newnode = (Node)httree[nodes[i].ID];
                                if (newnode.ID == XYLXBH)
                                {
                                    XyBmLx.Text = newnode.Name;
                                    break;
                                }
                            }
                            for (int i = 0; i < nodes.Count; i++)
                            {
                                
                                Node newnode = (Node)httree[nodes[i].ID];
                                if (newnode.ParentID == XYLXBH)
                                {
                                    IsEmpty = false;
                                    listSymptom.Add(new WesternMedicineInfo(XyBmLx.Text, newnode.ID.ToString(), newnode.Name.Substring(newnode.Name.IndexOf(' ') + 2, newnode.Name.Length - 2 - newnode.Name.IndexOf(' '))));
                                    nodenum++;
                                    if (nodenum == 0)
                                        ZXMC = newnode;
                                }

                            }
                    }
                    XyBmMc.Text = ZXMC.Name.Substring(ZXMC.Name.IndexOf(' ') + 2, ZXMC.Name.Length - 2 - ZXMC.Name.IndexOf(' '));
                    XyBmBh.Text =ZXMC.ID.ToString();
                    lv.SelectedIndex = 0;

                }
                reftree();
            }                        
        
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            if(ti.SelectedIndex==1)
            {
                tisel = 1;
                if (IsAdd == false && IsModify == false)
                {
                    text_box_ff.Text = "";
                    text_box_xycfmc.Text = "";
                    text_box_xyzf.Text = "";
                    text_box_xyzmmc.Text = "";
                    text_box_ff.IsReadOnly = false;
                    text_box_xycfmc.IsReadOnly = false;
                    text_box_xyzf.IsReadOnly = false;
                    text_box_xyzmmc.IsReadOnly = false;
                    //Text_Editable();
                   
                    string sql = String.Format("select max(xyzmbh) from t_rule_xycf");
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        WesternMedicineZMInfo User_Edit = new WesternMedicineZMInfo((Convert.ToInt64(dr[0]) + 1).ToString(), "", "","","");
                        listSymptom1.Add(User_Edit);
                        text_box_xyzmbh.Text = (Convert.ToInt64(dr[0]) + 1).ToString();
                    }
                    dr.Close();
                    conn.Close();
                    Keyboard.Focus(text_box_xyzmmc); // 设置焦点
                    lv1.SelectedIndex = lv1.Items.Count - 1; // 设置增加项被选中
                    nochange_item = lv1.SelectedIndex;
                    IsAdd = true;
                    button_save.IsEnabled = true;
                    button_modify.IsEnabled = false;
                    button_delete.IsEnabled = false;
                }              

            }
            if (ti.SelectedIndex == 2)
            {
                tisel = 2;
                if (combobox_xyzmmc_input.Text == "")
                {
                    MessageBox.Show("请选择西医证名!");                
                }
                
                else if(IsAdd == false && IsModify == false)
                {
                    string sql = String.Format("select top 1 xyzmbh from t_rule_xycf where xyzmmc='{0}'", combobox_xyzmmc_input.Text);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        BasicZhengmbmInfo User_Edit = new BasicZhengmbmInfo(dr[0].ToString(), combobox_xyzmmc_input.Text, "", "");
                        listSymptom2.Add(User_Edit);
                        lv2.SelectedIndex=lv2.Items.Count-1;
                    }
                    dr.Close();
                    conn.Close();
                    IsAdd = true;
                    button_save.IsEnabled = true;
                    button_modify.IsEnabled = false;
                    button_delete.IsEnabled = false;                
                }
            }

        }

        private void button_modify_Click(object sender, RoutedEventArgs e)
        {
            if (ti.SelectedIndex == 1)
            {
                tisel = 1;
                if (IsAdd == false && IsModify == false)
                {
                    text_box_ff.IsReadOnly = false;
                    text_box_xycfmc.IsReadOnly = false;
                    text_box_xyzf.IsReadOnly = false;
                    text_box_xyzmmc.IsReadOnly = false;
                    //Text_Editable();

                    
                    Keyboard.Focus(text_box_xyzmmc); // 设置焦点
                    //lv1.SelectedIndex = lv1.Items.Count - 1; // 设置增加项被选中
                    nochange_item = lv1.SelectedIndex;
                    IsModify = true;
                    button_save.IsEnabled = true;
                    button_add.IsEnabled = false;
                    button_delete.IsEnabled = false;
                }

            }
            if (ti.SelectedIndex == 2)
            {
                tisel = 2;
            }
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            isdel = true;
            if (MessageBox.Show("确定要删除该项吗？", "确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (ti.SelectedIndex == 2)
                {
                    tisel = 2;
                    try
                    {
                        BasicZhengmbmInfo Sel_Info = lv2.SelectedItem as BasicZhengmbmInfo;
                        string sql = String.Format("delete from t_rule_xyzz where xybmbh = '{0}' and xyzmbh='{1}'", Sel_Info.BingmNumber,Sel_Info.JibzmNumber);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                        if (count > 0)
                        {
                            MessageBox.Show("删除成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            listSymptom2.RemoveAt(lv2.SelectedIndex);
                            isdel = false;
                            if (lv2.SelectedIndex > 0)
                                lv2.SelectedIndex--;
                            else
                                lv2.SelectedIndex++;
                            bmmc.Text = "";
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
                else if (ti.SelectedIndex == 1)
                {
                    tisel = 1;
                    try
                    {
                        string sql = String.Format("delete from t_rule_xycf where xyzmbh = '{0}'", text_box_xyzmbh.Text);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                        if (count > 0)
                        {
                            MessageBox.Show("删除成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            listSymptom1.RemoveAt(lv1.SelectedIndex);
                            isdel = false;
                            if (lv1.SelectedIndex > 0)
                                lv1.SelectedIndex--;
                            else
                                lv1.SelectedIndex++;
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

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            if (ti.SelectedIndex == 2)
            {
                tisel = 2;
                Is_Repeat3();
                if(bmmc.Text == "")
                {
                    MessageBox.Show("西医病名名称不能为空！");
                }
                else if (IsRepeat == false && (bmmc.Text != ""))
                {
                    if(IsAdd)
                    {
                        try
                        {
                            BasicZhengmbmInfo Sel_Info = lv2.SelectedItem as BasicZhengmbmInfo;
                            string sql = String.Format("INSERT INTO t_rule_xyzz (xyzmbh,xyzmmc,xybmbh,xybmmc) VALUES ('{0}', '{1}','{2}','{3}')", Sel_Info.JibzmNumber, Sel_Info.JibzmName, Sel_Info.BingmNumber, Sel_Info.BingmName);
                            conn.Open();
                            SqlCommand comm = new SqlCommand(sql, conn);
                            int count = comm.ExecuteNonQuery();
                            if (count > 0)
                            {
                                MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                                IsAdd = false;
                                button_save.IsEnabled = false;
                                button_modify.IsEnabled = true;
                                button_delete.IsEnabled = true;
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
            else if (ti.SelectedIndex == 1)
            {
                tisel = 1;
                Is_Repeat2();
            
                if (text_box_xyzmmc.Text == "")
                    MessageBox.Show("西医证名名称不能为空！");
                if (text_box_xyzf.Text == "")
                    MessageBox.Show("西医治法不能为空！");
                if (text_box_xycfmc.Text == "")
                    MessageBox.Show("西医处方名称不能为空！");
                if (text_box_ff.Text == "")
                    MessageBox.Show("服法不能为空！");
                if (IsRepeat == false && (text_box_xyzmmc.Text != "")&&(text_box_xyzf.Text != "")&&(text_box_xycfmc.Text != "")&&(text_box_ff.Text != ""))
                {
                    Text_Readonly();
                    if (IsModify == true)
                    {
                        try
                        {
                        
                            string sql = String.Format("UPDATE t_rule_xycf SET xyzmmc='{0}',xyzf='{1}',xycfmc='{2}',xyff='{3}' WHERE xyzmbh='{4}'", text_box_xyzmmc.Text, text_box_xyzf.Text, text_box_xycfmc.Text, text_box_ff.Text, text_box_xyzmbh.Text);
                            conn.Open();
                            SqlCommand comm = new SqlCommand(sql, conn);
                            int count = comm.ExecuteNonQuery();
                            if (count > 0)
                            {
                                MessageBox.Show("修改成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                                IsModify = false;
                                button_save.IsEnabled = false;
                                button_add.IsEnabled = true;
                                button_delete.IsEnabled = true;
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("修改失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        finally
                        {
                            conn.Close();
                        }

                    }
                    if (IsAdd == true)
                    {

                        try
                        {
                            string sql = String.Format("INSERT INTO t_rule_xycf (xyzmbh,xyzmmc,xyzf,xycfmc,xyff) VALUES ('{0}', '{1}','{2}','{3}','{4}')", text_box_xyzmbh.Text, text_box_xyzmmc.Text, text_box_xyzf.Text,text_box_xycfmc.Text,text_box_ff.Text);
                            conn.Open();
                            SqlCommand comm = new SqlCommand(sql, conn);
                            int count = comm.ExecuteNonQuery();
                            if (count > 0)
                            {
                                MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                                IsAdd = false;
                                button_save.IsEnabled = false;
                                button_modify.IsEnabled = true;
                                button_delete.IsEnabled = true;
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
                    text_box_ff.IsReadOnly = true;
                    text_box_xycfmc.IsReadOnly = true;
                    text_box_xyzf.IsReadOnly = true;
                    text_box_xyzmmc.IsReadOnly = true;
                    //reftree();
                }
            }
            
        }

        private void Button_Click_back(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void combobox_xybmlx_types_input_DropDownOpened(object sender, EventArgs e)
        {
            string sql = String.Format("select xybmlxmc from t_info_xybmlx");
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            combobox_xybmlx_types_input.ItemsSource = ds.Tables[0].DefaultView;
            combobox_xybmlx_types_input.DisplayMemberPath = "xybmlxmc";
            combobox_xybmlx_types_input.SelectedValuePath = "xybmlxbh";
        }

        private void combobox_xybmlx_types_input_DropDownClosed(object sender, EventArgs e)
        {
            lv.ItemsSource = listSymptom;
            listSymptom.Clear();
            string xylxname = combobox_xybmlx_types_input.Text.Trim();
            int XYLXBH = 0, nodenum = -1;
            Node XYBMMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.Name.Trim() == xylxname.Trim())
                {
                    if (newnode.ParentID == -1)
                    {
                        XYLXBH = newnode.ID;
                    }

                }
            }
            string XYBH = "";
            if (XYLXBH < 10)
                XYBH = "140" + XYLXBH + "001";
            else
                XYBH = "14" + XYLXBH + "001";
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ID == Convert.ToInt64(XYLXBH))
                {
                    XyBmLx.Text = newnode.Name;
                }

            }
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == Convert.ToInt64(XYLXBH))
                {
                    listSymptom.Add(new WesternMedicineInfo(XyBmLx.Text, newnode.ID.ToString(), newnode.Name.Substring(newnode.Name.IndexOf(' ') + 2, newnode.Name.Length - 2 - newnode.Name.IndexOf(' '))));
                    nodenum++;
                    if (nodenum == 0)
                        XYBMMC = newnode;
                }

            }
            if (nodenum == -1)
            {
                XyBmBh.Text = "";
                XyBmMc.Text = "";
            }
            else
            {
                XyBmBh.Text = XYBH;
                XyBmMc.Text = XYBMMC.Name.Substring(XYBMMC.Name.IndexOf(' ') + 2, XYBMMC.Name.Length - 2 - XYBMMC.Name.IndexOf(' '));
                lv.SelectedIndex = 0;  
            }
            
        }

        private void fresh_Click(object sender, RoutedEventArgs e)
        {
            reftree();
        }

        private void treeview1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(is_xylx_add||is_xylx_edit)
            {
                MessageBox.Show("请先保存当前操作！");
            }
            else
            {
                lv.ItemsSource = listSymptom;
                listSymptom.Clear();
                Node item = (Node)treeview1.SelectedItem;
                string XYBMLXBH = "";
                int x1 = 0, x2 = 0;
                int nodenum = -1;
                int maxid = 0;
                Node XYBMMC = item;//保存症象名称的textbox
                //显示症象类型
                Node ZxLx = item;
                while (ZxLx.ParentID != -1 && ZxLx != null)
                {
                    ZxLx = (Node)httree[ZxLx.ParentID];
                }
                XyBmLx.Text = ZxLx.Name;
                //根据选择的树节点决定listview的内容
                if (item.ParentID == -1)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        Node newnode = (Node)httree[nodes[i].ID];
                        if (newnode.ID > maxid&&newnode.ParentID==-1)
                            maxid = newnode.ID;
                        if (newnode.ParentID == item.ID)
                        {
                            listSymptom.Add(new WesternMedicineInfo(XyBmLx.Text, newnode.ID.ToString(), newnode.Name.Substring(newnode.Name.IndexOf(' ') + 2, newnode.Name.Length - 2 - newnode.Name.IndexOf(' '))));
                            nodenum++;
                            if (nodenum == 0)
                                XYBMMC = newnode;
                        }

                    }
                    if (nodenum == -1)
                    {
                        XyBmMc.Text ="";
                        XyBmBh.Text = "";
                    }
                    else
                    {
                        XyBmMc.Text = XYBMMC.Name.Substring(XYBMMC.Name.IndexOf(' ')+2, XYBMMC.Name.Length - 2 - XYBMMC.Name.IndexOf(' '));
                        lv.SelectedIndex = 0;
                        XyBmBh.Text = XYBMMC.ID.ToString();
                    }
                
                }
                else
                {
                    XYBMLXBH = item.ParentID.ToString();
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        Node newnode = (Node)httree[nodes[i].ID];
                        if (newnode.ParentID == Convert.ToInt64(XYBMLXBH))
                        {
                            listSymptom.Add(new WesternMedicineInfo(XyBmLx.Text, newnode.ID.ToString(), newnode.Name.Substring(newnode.Name.IndexOf(' ') + 2, newnode.Name.Length - 2 - newnode.Name.IndexOf(' '))));
                            if (item.ID != newnode.ID && x2 == 0)
                                x1++;
                            if (item.ID == newnode.ID)
                                x2 = 1;
                        }

                    }
                    lv.SelectedIndex = x1;
                    XyBmMc.Text = XYBMMC.Name.Substring(XYBMMC.Name.IndexOf(' ') + 2, XYBMMC.Name.Length - 2 - XYBMMC.Name.IndexOf(' '));
                    XyBmBh.Text = item.ID.ToString();
                }
                if (ZxLx.ID == maxid)
                {
                    xybmlxwy.IsEnabled = false;
                    xybmlxfront.IsEnabled = false;
                    xybmlxsy.IsEnabled = true;
                    xybmlxback.IsEnabled = true;
                
                }
                else
                {
                    xybmlxwy.IsEnabled = true;
                    xybmlxfront.IsEnabled = true;
                }
                if (ZxLx.ID ==1)
                {
                    xybmlxsy.IsEnabled = false;
                    xybmlxback.IsEnabled = false;
                    xybmlxwy.IsEnabled = true;
                    xybmlxfront.IsEnabled = true;
                }
                else
                {
                    xybmlxsy.IsEnabled = true;
                    xybmlxback.IsEnabled = true;
                }
            }
            
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((is_xylx_add || is_xylx_edit) && lv.SelectedIndex != nochange_item)
            {
                lv.SelectedIndex = nochange_item;
                MessageBox.Show("请先保存当前操作！");
            }

            else if (listSymptom.Count != 0)
            {
                WesternMedicineInfo Sel_Info = lv.SelectedItem as WesternMedicineInfo;
                XyBmMc.Text = Sel_Info.WesternMedicineName;
                XyBmBh.Text = Sel_Info.WesternMedicineNumber;
            }
            if(isadd&&lv.SelectedIndex != lv.Items.Count - 1)
            {
                MessageBox.Show("请先保存当前操作！");
                lv.SelectedIndex = lv.Items.Count - 1;
            }
            if (isedit && nochange_item != lv.SelectedIndex)
            {
                MessageBox.Show("请先保存当前操作！");
                lv.SelectedIndex = nochange_item;
            }
        }

        private void XyBmLx_LostFocus(object sender, RoutedEventArgs e)
        {

            //if (is_xylx_edit)
            //{
            //    WesternMedicineInfo Sel_Info = lv.SelectedItem as WesternMedicineInfo;
            //    Sel_Info.WesternMedicineType = XyBmLx.Text;
            //}
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ti.SelectedIndex==0)
            {
                button_add.IsEnabled = false;
                button_delete.IsEnabled = false;
                button_modify.IsEnabled = false;
                button_save.IsEnabled = false;
                ischange = true;
            }
            else if(isadd||isedit||is_xylx_add||is_xylx_edit)
            {
                MessageBox.Show("西医病名表处于编辑状态！请保存或取消！");
                ti.SelectedIndex = 0;
            }
            else if((IsAdd||IsModify)&&tisel==1&&ti.SelectedIndex!=1)
            {
                MessageBox.Show("西医证名表处于编辑状态！请保存或取消！");
                ti.SelectedIndex = 1;
            }
            else if ((IsAdd || IsModify) && tisel == 2 && ti.SelectedIndex != 2)
            {
                MessageBox.Show("西医证名-病名表处于编辑状态！请保存或取消！");
                ti.SelectedIndex = 2;
            }

            else
            {
                button_add.IsEnabled = true;
                button_delete.IsEnabled = true;
                button_modify.IsEnabled = true;
                
                if(ti.SelectedIndex==1&&ischange==true)
                {
                    listSymptom1.Clear();
                    Text_Readonly();
                    button_save.IsEnabled = false;
                    string sql = String.Format("select * from t_rule_xycf");
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader dr = comm.ExecuteReader();
                    listSymptom1.Clear();
                    while (dr.Read())
                    {

                        listSymptom1.Add(new WesternMedicineZMInfo(dr["xyzmbh"].ToString(), dr["xyzmmc"].ToString(),dr["xyzf"].ToString(), dr["xycfmc"].ToString(),dr["xyff"].ToString()));
                    }
                    dr.Close();
                    conn.Close();
                    sql = String.Format("select top 1 * from t_rule_xycf");
                    conn.Open();
                    comm = new SqlCommand(sql, conn);
                    dr = comm.ExecuteReader();
                    while (dr.Read())
                    {
                        text_box_xyzmbh.Text=dr["xyzmbh"].ToString();
                        text_box_xyzmmc.Text=dr["xyzmmc"].ToString();
                        text_box_xycfmc.Text=dr["xycfmc"].ToString();
                        text_box_xyzf.Text=dr["xyzf"].ToString();
                        text_box_ff.Text=dr["xyff"].ToString();
                    }
                    dr.Close();
                    conn.Close();
                    lv1.SelectedIndex = 0;
                }
                if(ti.SelectedIndex==2)
                {
                    ischange = true;
                }
            }
        }

        private void lv1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((IsAdd || IsModify )&& lv1.SelectedIndex != nochange_item)
            {
                MessageBox.Show("请先保存当前操作！");
                lv1.SelectedIndex = nochange_item;
            }
            else
            if(lv1.Items.Count!=0&&isdel != true)
            {
                ischange = false;
                WesternMedicineZMInfo User_Edit = lv1.SelectedItem as WesternMedicineZMInfo;
                text_box_xyzmbh.Text = User_Edit.XYZMBH;
                text_box_xyzmmc.Text = User_Edit.XYZMMC;
                text_box_xycfmc.Text = User_Edit.XYCFMC;
                text_box_xyzf.Text = User_Edit.XYZF;
                text_box_ff.Text = User_Edit.FF;
            }
            
        }

        private void text_box_ff_LostFocus(object sender, RoutedEventArgs e)
        {
            WesternMedicineZMInfo User_Edit = lv1.SelectedItem as WesternMedicineZMInfo;
             User_Edit.FF= text_box_ff.Text;
        }

        private void text_box_xycfmc_LostFocus(object sender, RoutedEventArgs e)
        {
            WesternMedicineZMInfo User_Edit = lv1.SelectedItem as WesternMedicineZMInfo;
             User_Edit.XYCFMC= text_box_xycfmc.Text;
        }

        private void text_box_xyzf_LostFocus(object sender, RoutedEventArgs e)
        {
            WesternMedicineZMInfo User_Edit = lv1.SelectedItem as WesternMedicineZMInfo;
            User_Edit.XYZF= text_box_xyzf.Text ;
        }

        private void text_box_xyzmmc_LostFocus(object sender, RoutedEventArgs e)
        {
            WesternMedicineZMInfo User_Edit = lv1.SelectedItem as WesternMedicineZMInfo;
            User_Edit.XYZMMC = text_box_xyzmmc.Text;
        }

        private void btn_bmmc_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdd)
            {
                MessageBox.Show("表未处于添加状态！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                XYBMsel diseaseinfoadmin = new XYBMsel();
                diseaseinfoadmin.PassValuesEvent += new XYBMsel.PassValuesHandler(ReceiveValues1);
                diseaseinfoadmin.Show();
            }
        }
        /// <summary>
        /// 功能：实现病名名称的读取和显示
        /// </summary>
        private void ReceiveValues1(object sender, XYBMsel.PassValuesEventArgs e)
        {
            if (lv2.SelectedIndex == lv2.Items.Count - 1)
            {
                bmmc.Text = e.Name;
                m_bmbh = e.Number;
                BasicZhengmbmInfo basiczhengmbminfo = lv2.SelectedItem as BasicZhengmbmInfo;
                if (basiczhengmbminfo != null && basiczhengmbminfo is BasicZhengmbmInfo)
                {
                    basiczhengmbminfo.BingmNumber = m_bmbh;
                    basiczhengmbminfo.BingmName = bmmc.Text;
                }
            }
        }

        private void combobox_xyzmmc_input_DropDownOpened(object sender, EventArgs e)
        {
            string sql = String.Format("select xyzmmc from t_rule_xycf");
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            combobox_xyzmmc_input.ItemsSource = ds.Tables[0].DefaultView;
            combobox_xyzmmc_input.DisplayMemberPath = "xyzmmc";
            combobox_xyzmmc_input.SelectedValuePath = "xyzmbh";
        }

        private void combobox_xyzmmc_input_DropDownClosed(object sender, EventArgs e)
        {
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            string xylxname = combobox_xyzmmc_input.Text.Trim();
            string sql = String.Format("select * from t_rule_xyzz where xyzmmc='{0}'",xylxname);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                BasicZhengmbmInfo User_Edit = new BasicZhengmbmInfo(dr["xyzmbh"].ToString(),dr["xyzmmc"].ToString(),dr["xybmbh"].ToString(),dr["xybmmc"].ToString());
                listSymptom2.Add(User_Edit);
            }
            dr.Close();
            conn.Close();
        }

        private void lv2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((IsAdd || IsModify) && lv2.SelectedIndex != nochange_item)
            {
                MessageBox.Show("请先保存当前操作！");
                lv2.SelectedIndex = nochange_item;
            }
        }
    }
}
