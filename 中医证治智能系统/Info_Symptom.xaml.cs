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
    /// Interaction logic for Info_Symptom.xaml
    /// </summary>
    public partial class Info_Symptom : Window
    {
        // 定义连接字符串
        static public string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
        // 创建 Connection 对象
        static public SqlConnection conn = new SqlConnection(connString);
        // 创建集合实例
        ObservableCollection<SymptomInfo> listSymptom = new ObservableCollection<SymptomInfo>();
        ObservableCollection<SymptomInfo> listSymptom2 = new ObservableCollection<SymptomInfo>();
        // 全局变量，用于存储症象信息
        SymptomInfo Symptom_Display = new SymptomInfo("", "", "");

        //Node node = new Node();
        // 创建 List 集合实例
        List<Node> nodes = new List<Node>();

        Hashtable httree = new Hashtable(9000);

        bool isadd = false;
        bool IsRepeat = false;
        bool is_zxmc_add = false;
        bool is_zxmc_edit = false;
        int nochange_item=0;
        string nochange_name = "";
        string zxlxforsave = "";
        string zxbhforsave = "";
        string zxmcforsave = "";

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
        /// 功能：类的构造函数，用于初始化
        /// 说明：1.通过三次数据库操作将数据从库中写入集合
        ///       2.
        /// </summary>
        public Info_Symptom()
        {
            InitializeComponent();
            // 指定 listview 数据源
            lv.ItemsSource = listSymptom;
            savebutton.IsEnabled = false;     
            // 将数据库数据写入 List 集合
            // 一级树写入           
            string sql = String.Format("select * from t_info_zxlx");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node { ID = Convert.ToInt32(dr["zxlxbh"]), Name = dr["zxlxmc"].ToString().Trim() });
            }
            dr.Close();
            conn.Close();

            // 二级树写入
            sql = String.Format("select t1.zxbh ,t2.zxlxbh, min(t3.zxmc) from (t_info_zxxx as t1 inner join t_info_zxlx as t2 on t2.zxlxbh = t1.zxlxbh) inner join  t_info_zxmx as t3 on t1.zxbh = t3.zxbh group by t1.zxbh, t2.zxlxbh");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                //？？？ 有问题，数据库中的 zxlxbh = 30 对应的 zxbh 为空会出现问题，赋值为0问题解决
                nodes.Add(new Node { ID = Convert.ToInt32(dr["zxbh"]), Name = dr["zxbh"].ToString() + "  " + dr[2].ToString(), ParentID = Convert.ToInt32(dr["zxlxbh"])});
                //nodes.Add(new Node { ID = Convert.ToInt32(dr["zxbh"]), Name = dr["zxbh"].ToString() + "  " + "默认名称", ParentID = Convert.ToInt32(dr["zxlxbh"]) });
            }
            dr.Close();
            conn.Close();

            // 三级树写入
            sql = String.Format("select t_info_zxmx.id,t_info_zxmx.zxmc,t_info_zxmx.zxbh from (t_info_zxxx inner join t_info_zxlx on t_info_zxlx.zxlxbh = t_info_zxxx.zxlxbh) inner join  t_info_zxmx on t_info_zxxx.zxbh = t_info_zxmx.zxbh order by t_info_zxmx.zxmc");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node { ID = Convert.ToInt32(dr["id"]), Name = dr["zxmc"].ToString().Trim(), ParentID = Convert.ToInt32(dr["zxbh"]) });
            }
            dr.Close();
            conn.Close();
            BuildENTree();
            Text_Readonly();
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            string ZXBH = "0101001";
            ZxBh.Text = "0101001";
            zxlxmc.Text = "寒";
            int nodenum = -1;
            Node ZXMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == Convert.ToInt64(ZXBH))
                {
                    listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                    nodenum++;
                    if (nodenum == 0)
                        ZXMC = newnode;
                }

            }
            ZxMc.Text = ZXMC.Name;
            lv2.SelectedIndex = 0;
        }

        /// <summary>
        /// 功能：创建症象信息类
        /// 说明：1.SymptomTypes-->症象类型名称 SymptomNumber-->症象编号 SymptomName-->症象名称
        ///       2.
        /// </summary>
        public class SymptomInfo : INotifyPropertyChanged
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

            private string _SymptomTypes;
            private string _SymptomNumber;
            private string _SymptomName;

            public string SymptomTypes
            {
                get { return _SymptomTypes; }
                set { _SymptomTypes = value; OnPropertyChanged(new PropertyChangedEventArgs("SymptomTypes")); }
            }

            public string SymptomNumber
            {
                get { return _SymptomNumber; }
                set { _SymptomNumber = value; OnPropertyChanged(new PropertyChangedEventArgs("SymptomNumber")); }
            }

            public string SymptomName
            {
                get { return _SymptomName; }
                set { _SymptomName = value; OnPropertyChanged(new PropertyChangedEventArgs("SymptomName")); }
            }

            public SymptomInfo(string symptomtypes, string symptomnumber, string symptomname)
            {
                _SymptomTypes = symptomtypes;
                _SymptomNumber = symptomnumber;
                _SymptomName = symptomname;
            }
            public SymptomInfo(string symptomnumber, string symptomname)
            {
                _SymptomNumber = symptomnumber;
                _SymptomName = symptomname;
            }
        }
        /// <summary>
        /// 功能：设置文本只读
        /// </summary>
        public void Text_Readonly()
        {
            zxlxmc.IsReadOnly = true;
            ZxBh.IsReadOnly = true;
            ZxMc.IsReadOnly = true;
        }

        public void Text_Editable()
        {
            zxlxmc.IsReadOnly = false;
            ZxBh.IsReadOnly = false;
            ZxMc.IsReadOnly = false;
        }

        /// <summary>
        /// 功能：实现 combobox 下拉绑定数据库并显示
        /// </summary>
        private void combobox_symptom_types_search_DropDownOpened(object sender, EventArgs e)
        {
            //string sql = String.Format("select zxlxbh, zxlxmc from t_info_zxlx");
            //SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            //DataSet ds = new DataSet();
            //ds.Clear();
            //da.Fill(ds);
            //combobox_symptom_types_search.ItemsSource = ds.Tables[0].DefaultView;
            //combobox_symptom_types_search.DisplayMemberPath = "zxlxmc";
            //combobox_symptom_types_search.SelectedValuePath = "zxlxbh";
            combobox_symptom_types_search.Items.Clear();
            combobox_symptom_types_search.Items.Add("全部");
            string sql = String.Format("select zxlxbh, zxlxmc from t_info_zxlx");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                combobox_symptom_types_search.Items.Add(dr["zxlxbh"].ToString() + " " + dr["zxlxmc"].ToString());
            }
            dr.Close();
            conn.Close();
        }

        /// <summary>
        /// 功能：搜索匹配项
        /// 说明：1.搜索前集合清空
        ///       2.
        /// </summary>
        private void search_Click(object sender, RoutedEventArgs e)
        {
            listSymptom.Clear();
            string sql;
            if (combobox_symptom_types_search.Text != "全部" && combobox_symptom_types_search.Text != "")
            {
                //MessageBox.Show(combobox_symptom_types_search.Text.Trim().Substring(2));
                if (combobox_symptom_types_search.Text.Substring(2).Trim() == "")
                {
                    sql = String.Format("select * from (t_info_zxxx inner join t_info_zxlx on t_info_zxlx.zxlxbh = t_info_zxxx.zxlxbh) inner join  t_info_zxmx on t_info_zxxx.zxbh = t_info_zxmx.zxbh where zxmc like '%{0}%'",
                    text_symptom.Text.Trim());
                }
                else
                {
                    sql = String.Format("select * from (t_info_zxxx inner join t_info_zxlx on t_info_zxlx.zxlxbh = t_info_zxxx.zxlxbh) inner join  t_info_zxmx on t_info_zxxx.zxbh = t_info_zxmx.zxbh where zxlxmc = '{0}' and zxmc like '%{1}%'",
                    combobox_symptom_types_search.Text.Substring(2).Trim(), text_symptom.Text.Trim());
                }
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    Symptom_Display = new SymptomInfo(dr["zxlxmc"].ToString(), dr["zxbh"].ToString(), dr["zxmc"].ToString());
                    listSymptom.Add(Symptom_Display);
                }

                dr.Close();
                conn.Close();
            }
            else
            {
                sql = String.Format("select * from (t_info_zxxx inner join t_info_zxlx on t_info_zxlx.zxlxbh = t_info_zxxx.zxlxbh) inner join  t_info_zxmx on t_info_zxxx.zxbh = t_info_zxmx.zxbh where zxmc like '%{0}%'",
                    text_symptom.Text.Trim());
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    Symptom_Display = new SymptomInfo(dr["zxlxmc"].ToString(), dr["zxbh"].ToString(), dr["zxmc"].ToString());
                    listSymptom.Add(Symptom_Display);
                }

                dr.Close();
                conn.Close();
            }

        }

        /// <summary>
        /// 功能：返回
        /// </summary>
        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /*****************************************症象信息检索**************************************************/
        /********************************************分割线*****************************************************/
        /*****************************************症象信息录入**************************************************/


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

        ///// <summary>
        ///// 功能：递归向下查找
        ///// </summary>
        //Node FindDownward(List<Node> nodes, int id)
        //{
        //    if (nodes == null)
        //        return null;
        //    for (int i = 0; i < nodes.Count; i++)
        //    {
        //        if (nodes[i].ID == id)
        //        {
        //            return nodes[i];
        //        }
        //        Node node = FindDownward(nodes[i].Nodes, id);
        //        if (node != null)
        //        {
        //            return node;
        //        }
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// 功能：绑定树
        ///// </summary>        
        //List<Node> Bind(List<Node> nodes)
        //{
        //    List<Node> outputList = new List<Node>();
        //    for (int i = 0; i < nodes.Count; i++)
        //    {
        //        if (nodes[i].ParentID == -1)
        //        {
        //            outputList.Add(nodes[i]);
        //        }
        //        else
        //        {
        //            FindDownward(nodes, nodes[i].ParentID).Nodes.Add(nodes[i]);
        //        }
        //    }
        //    return outputList;
        //}

        /// <summary>
        /// 功能：实现 combobox 下拉绑定数据库并显示
        /// </summary>
        private void combobox_symptom_types_input_DropDownOpened(object sender, EventArgs e)
        {
            string sql = String.Format("select zxlxmc from t_info_zxlx");
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            combobox_symptom_types_input.ItemsSource = ds.Tables[0].DefaultView;
            combobox_symptom_types_input.DisplayMemberPath = "zxlxmc";
            combobox_symptom_types_input.SelectedValuePath = "zxlxbh";
            

        }


        private void treeview1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            Node item = (Node)treeview1.SelectedItem;
            string ZXBH="";
            int x1=0,x2=0;
            int nodenum=-1;
            int maxid = 0;
            Node ZXMC=item;//保存症象名称的testbox
            //显示症象类型
            Node ZxLx = item;
            while(ZxLx.ParentID!=-1&&ZxLx!=null)
            {
                ZxLx = (Node)httree[ZxLx.ParentID];
            }
            zxlxmc.Text = ZxLx.Name;

            //根据选择的树节点决定listview的内容
            if(item.ParentID==-1)
            {
                if(item.ID <10)
                ZXBH="010"+item.ID.ToString()+"001";
                else 
                ZXBH="01"+item.ID.ToString()+"001";
                //Node newnode = (Node)httree[Convert.ToInt64(ZXBH)];
                for (int i = 0; i < nodes.Count; i++)
                {
                    Node newnode = (Node)httree[nodes[i].ID];
                    if(newnode.ParentID==Convert.ToInt64(ZXBH))
                    { 
                        listSymptom2.Add(new SymptomInfo("0"+newnode.ParentID.ToString(),newnode.Name));
                        nodenum++;
                        if(nodenum==0)
                            ZXMC = newnode;
                    }
                    
                }
                
                ZxMc.Text = ZXMC.Name;
                lv2.SelectedIndex = 0;
                ZxBh.Text =ZXBH;
            }
            else if(item.ID<=0100000)
            {
                ZXBH = item.ParentID.ToString();
                for (int i = 0; i < nodes.Count; i++)
                {
                    Node newnode = (Node)httree[nodes[i].ID];
                    if (newnode.ParentID == Convert.ToInt64(ZXBH))
                    {
                        listSymptom2.Add(new SymptomInfo("0"+newnode.ParentID.ToString(), newnode.Name)); 
                        if(item.ID!=newnode.ID&&x2==0)
                             x1++;
                        if(item.ID==newnode.ID)
                            x2=1;
                    }

                }
                lv2.SelectedIndex=x1;
                ZxMc.Text = item.Name;
                ZxBh.Text = "0" + ZXBH;
            }
            else if(item.ID>0100000)
            {
                ZXBH = item.ID.ToString();
                for (int i = 0; i < nodes.Count; i++)
                {
                    Node newnode = (Node)httree[nodes[i].ID];
                    if (newnode.ParentID == Convert.ToInt64(ZXBH))
                    {
                        listSymptom2.Add(new SymptomInfo("0"+newnode.ParentID.ToString(), newnode.Name));
                        nodenum++;
                        if (nodenum == 0)
                            ZXMC = newnode;
                    }
                    if(newnode.ID==Convert.ToInt64(ZXBH))
                    {
                        if ((ZXBH == "1" + newnode.ParentID.ToString() + "001") || (ZXBH == "10" + newnode.ParentID.ToString() + "001"))
                        {
                            zxbhback.IsEnabled = false;
                            zxbhsy.IsEnabled = false;
                            zxbhwy.IsEnabled = true;
                            zxbhfront.IsEnabled = true;
                            
                        }
                        else
                        {
                            zxbhback.IsEnabled = true;
                            zxbhsy.IsEnabled = true;
                        }
                        for (int j = 0; j < nodes.Count; j++)
                        {
                            Node newnode1 = (Node)httree[nodes[j].ID];
                            if (newnode1.ParentID == newnode.ParentID)
                            {
                                
                                if (newnode1.ID > maxid)
                                    maxid = newnode1.ID;

                            }
                        }
                        if(maxid.ToString()==ZXBH)
                        {
                            zxbhback.IsEnabled = true;
                            zxbhsy.IsEnabled = true;
                            zxbhwy.IsEnabled = false;
                            zxbhfront.IsEnabled = false;
                        }
                        else
                        {
                            zxbhwy.IsEnabled = true;
                            zxbhfront.IsEnabled = true;
                        }
                    }

                }
                lv2.SelectedIndex = 0;
                ZxMc.Text=ZXMC.Name;
                ZxBh.Text ="0"+ ZXBH;
            }
            if (zxlxmc.Text.Trim() == "其它")
            {
                zxlxwy.IsEnabled = false;
                zxlxfront.IsEnabled = false;
            }
            else
            {
                zxlxwy.IsEnabled = true;
                zxlxfront.IsEnabled = true;
            }
            if (zxlxmc.Text.Trim() == "寒")
            {
                zxlxsy.IsEnabled = false;
                zxlxback.IsEnabled = false;
            }
            else
            {
                zxlxsy.IsEnabled = true;
                zxlxback.IsEnabled = true;
            }
        }

        private void fresh_Click(object sender, RoutedEventArgs e)
        {
            httree.Clear();
            nodes.Clear();
            // 一级树写入           
            string sql = String.Format("select * from t_info_zxlx");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node { ID = Convert.ToInt32(dr["zxlxbh"]), Name = dr["zxlxmc"].ToString().Trim() });
            }
            dr.Close();
            conn.Close();

            // 二级树写入
            sql = String.Format("select t1.zxbh ,t2.zxlxbh, min(t3.zxmc) from (t_info_zxxx as t1 inner join t_info_zxlx as t2 on t2.zxlxbh = t1.zxlxbh) inner join  t_info_zxmx as t3 on t1.zxbh = t3.zxbh group by t1.zxbh, t2.zxlxbh");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                //？？？ 有问题，数据库中的 zxlxbh = 30 对应的 zxbh 为空会出现问题，赋值为0问题解决
                nodes.Add(new Node { ID = Convert.ToInt32(dr["zxbh"]), Name = dr["zxbh"].ToString() + "  " + dr[2].ToString(), ParentID = Convert.ToInt32(dr["zxlxbh"]) });
                //nodes.Add(new Node { ID = Convert.ToInt32(dr["zxbh"]), Name = dr["zxbh"].ToString() + "  " + "默认名称", ParentID = Convert.ToInt32(dr["zxlxbh"]) });
            }
            dr.Close();
            conn.Close();

            // 三级树写入
            sql = String.Format("select t_info_zxmx.id,t_info_zxmx.zxmc,t_info_zxmx.zxbh from (t_info_zxxx inner join t_info_zxlx on t_info_zxlx.zxlxbh = t_info_zxxx.zxlxbh) inner join  t_info_zxmx on t_info_zxxx.zxbh = t_info_zxmx.zxbh order by t_info_zxmx.zxmc");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node { ID = Convert.ToInt32(dr["id"]), Name = dr["zxmc"].ToString(), ParentID = Convert.ToInt32(dr["zxbh"]) });
            }
            dr.Close();
            conn.Close();
            BuildENTree();
            Text_Readonly();
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            string ZXBH = ZxBh.Text;
            int nodenum = -1;
            Node ZXMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == Convert.ToInt64(ZXBH))
                {
                    listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                    nodenum++;
                    if (nodenum == 0)
                        ZXMC = newnode;
                }

            }
            ZxMc.Text = ZXMC.Name;
            lv2.SelectedIndex = 0;
            //for (int i = 0; i < nodes.Count; i++)
            //{
            //    System.Windows.Controls.TreeViewItem treeItem = treeview1.SelectedItem as System.Windows.Controls.TreeViewItem;
            //    treeItem.IsExpanded = true;
            //}
        }


        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// 功能：【症象信息管理】中的【选定】按钮功能
        /// </summary>
        private void select_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            SymptomInfo symptominfo = lv.SelectedItem as SymptomInfo;
            if (symptominfo != null && symptominfo is SymptomInfo)
            {
                PassValuesEventArgs args = new PassValuesEventArgs(symptominfo.SymptomName.ToString(),symptominfo.SymptomNumber.ToString());
                // 要判断 PassValuesEvent 是否为空，即判断该窗口是否被调用
                if (PassValuesEvent != null)
                { 
                    PassValuesEvent(this, args);
                }     
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
                    if ((parentnode1.ID != parentnode.ID) && parentnode.ParentID == -1)
                    {
                        outputList.Add(parentnode);
                        parentnode1.ID = parentnode.ID;
                    }
                }
            }
            this.treeview1.ItemsSource = outputList;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((is_zxmc_add||is_zxmc_edit)&&lv2.SelectedIndex != nochange_item)
            {
                lv2.SelectedIndex = nochange_item;
                MessageBox.Show("请先保存当前操作！");
            }
            else if(listSymptom2.Count!=0)
            {
                SymptomInfo Sel_Info = lv2.SelectedItem as SymptomInfo;
                ZxMc.Text = Sel_Info.SymptomName;
            }
            if(lv2.SelectedIndex==0)
            {
                zxmcsy.IsEnabled = false;
                zxmcback.IsEnabled = false;
                zxmcfront.IsEnabled = true;
                zxmcwy.IsEnabled = true;
            }
            if(lv2.SelectedIndex==lv2.Items.Count - 1)
            {
                zxmcsy.IsEnabled = true;
                zxmcback.IsEnabled = true;
                zxmcfront.IsEnabled = false;
                zxmcwy.IsEnabled = false;
            }
            if(lv2.SelectedIndex!=0&&lv2.SelectedIndex!=lv2.Items.Count - 1)
            {
                zxmcsy.IsEnabled = true;
                zxmcback.IsEnabled = true;
                zxmcfront.IsEnabled = true;
                zxmcwy.IsEnabled = true;
            }
        }

        private void zxlxsy_Click(object sender, RoutedEventArgs e)
        {
            
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            int nodenum = -1;
            Node ZXMC=new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == 101001)
                {
                    listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                    nodenum++;
                    if (nodenum == 0)
                        ZXMC = newnode;
                }

            }
            ZxBh.Text = "0101001";
            zxlxmc.Text = "寒";
            ZxMc.Text = ZXMC.Name;
            lv2.SelectedIndex = 0;
            if (zxlxmc.Text.Trim() == "寒")
            {
                zxlxsy.IsEnabled = false;
                zxlxback.IsEnabled = false;
                zxlxwy.IsEnabled = true;
                zxlxfront.IsEnabled = true;
            }
            else
            {
                zxlxsy.IsEnabled = true;
                zxlxback.IsEnabled = true;
            }
        }

        private void zxlxwy_Click(object sender, RoutedEventArgs e)
        {
            
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            int nodenum = -1;
            Node ZXMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == 130001)
                {
                    listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                    nodenum++;
                    if (nodenum == 0)
                        ZXMC = newnode;
                }

            }
            ZxBh.Text = "0130001";
            zxlxmc.Text = "其它";
            ZxMc.Text = ZXMC.Name;
            lv2.SelectedIndex = 0;
            if (zxlxmc.Text.Trim() == "其它")
            {
                zxlxwy.IsEnabled = false;
                zxlxfront.IsEnabled = false;
                zxlxsy.IsEnabled = true;
                zxlxback.IsEnabled = true;
                zxbhwy.IsEnabled = true;
                zxbhfront.IsEnabled = true;
            }
        }

        private void zxbhsy_Click(object sender, RoutedEventArgs e)
        {
            string ZXBH = "";
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            int nodenum = -1;
            Node ZXMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.Name == zxlxmc.Text)
                { 
                    if (newnode.ID>=10)
                        ZXBH = "01"+newnode.ID.ToString()+"001";
                    else
                        ZXBH = "010" + newnode.ID.ToString() + "001";
                }
                    
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == Convert.ToInt64(ZXBH))
                {
                    listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                    nodenum++;
                    if (nodenum == 0)
                        ZXMC = newnode;
                }

            }
            ZxBh.Text = ZXBH;
            //zxlxmc.Text = "其它";
            ZxMc.Text = ZXMC.Name;
            lv2.SelectedIndex = 0;
            zxbhsy.IsEnabled = false;
            zxbhback.IsEnabled = false;
            zxbhwy.IsEnabled = true;
            zxbhfront.IsEnabled = true;
        }

        private void zxbhwy_Click(object sender, RoutedEventArgs e)
        {
            int ZXBH =0;
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            int nodenum = -1;
            Node ZXMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.Name.Trim() == zxlxmc.Text.Trim())
                {
                   ZXBH= newnode.ID;
                }
                    
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == ZXBH)
                {
                    //listSymptom2.Clear();
                    //listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                    ZXMC=newnode;
                }
                
            }
            ZxBh.Text ="0"+ ZXMC.ID.ToString();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID ==Convert.ToInt64 (ZxBh.Text))
                {
                    listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                    nodenum++;
                    if (nodenum == 0)
                        ZXMC = newnode;
                }
            }
            
            //zxlxmc.Text = "其它";
            ZxMc.Text = ZXMC.Name;
            lv2.SelectedIndex = 0;
            zxbhwy.IsEnabled = false;
            zxbhfront.IsEnabled = false;
            zxbhsy.IsEnabled = true;
            zxbhback.IsEnabled = true;
        }

        private void zxlxback_Click(object sender, RoutedEventArgs e)
        {
            
                
            int ZXLXBH = 0;
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            int nodenum = -1;
            Node ZXMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.Name.Trim() == zxlxmc.Text.Trim())
                {
                    ZXLXBH = newnode.ID;
                    break;
                }

            }
            ZXLXBH--;
            string ZXBH = "";
            if(ZXLXBH <10)
                ZXBH="010"+ZXLXBH+"001";
            else if (ZXLXBH == 29)
                ZXBH = "01" + ZXLXBH + "002";
            else
                ZXBH = "01" + ZXLXBH + "001";
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ID == Convert.ToInt64(ZXLXBH))
                {
                    zxlxmc.Text = newnode.Name; 
                }

            }
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if(newnode.ParentID==Convert.ToInt64(ZXBH))
                { 
                    listSymptom2.Add(new SymptomInfo("0"+newnode.ParentID.ToString(),newnode.Name));
                    nodenum++;
                    if(nodenum==0)
                        ZXMC = newnode;
                }
                    
            }
            ZxBh.Text = ZXBH;
            ZxMc.Text = ZXMC.Name;
            lv2.SelectedIndex = 0;
            if (zxlxmc.Text.Trim() == "寒")
            {
                zxlxsy.IsEnabled = false;
                zxlxback.IsEnabled = false;
                zxlxfront.IsEnabled = true;
                zxlxwy.IsEnabled = true;
            }
            else
            {
                zxlxback.IsEnabled = true;
                zxlxsy.IsEnabled = true;
                zxlxfront.IsEnabled = true;
                zxlxwy.IsEnabled = true;
            }
        }

        private void zxlxfront_Click(object sender, RoutedEventArgs e)
        {
            
            int ZXLXBH = 0;
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            int nodenum = -1;
            Node ZXMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.Name.Trim() == zxlxmc.Text.Trim())
                {
                    ZXLXBH = newnode.ID;
                    break;
                }
            }
            ZXLXBH++;
            string ZXBH = "";
            if (ZXLXBH < 10)
                ZXBH = "010" + ZXLXBH + "001";
            else if(ZXLXBH==29)
                ZXBH = "01" + ZXLXBH + "002";
            else
                ZXBH = "01" + ZXLXBH + "001";
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ID == Convert.ToInt64(ZXLXBH))
                {
                    zxlxmc.Text = newnode.Name;
                }

            }
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == Convert.ToInt64(ZXBH))
                {
                    listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                    nodenum++;
                    if (nodenum == 0)
                        ZXMC = newnode;
                }

            }
            ZxBh.Text = ZXBH;
            ZxMc.Text = ZXMC.Name;
            lv2.SelectedIndex = 0;
            if (zxlxmc.Text.Trim() == "其它")
            {
                zxlxfront.IsEnabled = false;
                zxlxwy.IsEnabled = false;
                zxlxback.IsEnabled = true;
                zxlxsy.IsEnabled = true;
            }
            else
            {
                zxlxfront.IsEnabled = true;
                zxlxwy.IsEnabled = true;
                zxlxback.IsEnabled = true;
                zxlxsy.IsEnabled = true;
            }
        }

        private void zxbhback_Click(object sender, RoutedEventArgs e)
        {
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            int nodenum = -1;
            int maxid = 0;
            Node ZXMC = new Node();
            bool IsEmpty=true;
            string ZXBH = "";
            ZXBH = ZxBh.Text;
            
            while(IsEmpty){
                    IsEmpty = true;
                    ZXBH=(Convert.ToInt64(ZXBH) - 1).ToString();
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        Node newnode = (Node)httree[nodes[i].ID];
                        if (newnode.ParentID == Convert.ToInt64(ZXBH))
                        {
                            IsEmpty = false;
                            listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                            nodenum++;
                            if (nodenum == 0)
                                ZXMC = newnode;
                        }
                        if (newnode.ID == Convert.ToInt64(ZXBH))
                        {
                            if ((ZXBH == "1" + newnode.ParentID.ToString() + "001") || (ZXBH == "10" + newnode.ParentID.ToString() + "001"))
                            {
                                zxbhback.IsEnabled = false;
                                zxbhsy.IsEnabled = false;
                                zxbhwy.IsEnabled = true;
                                zxbhfront.IsEnabled = true;

                            }
                            else
                            {
                                zxbhback.IsEnabled = true;
                                zxbhsy.IsEnabled = true;
                            }
                            for (int j = 0; j < nodes.Count; j++)
                            {
                                Node newnode1 = (Node)httree[nodes[j].ID];
                                if (newnode1.ParentID == newnode.ParentID)
                                {

                                    if (newnode1.ID > maxid)
                                        maxid = newnode1.ID;

                                }
                            }
                            if (maxid.ToString() == ZXBH)
                            {
                                zxbhback.IsEnabled = true;
                                zxbhsy.IsEnabled = true;
                                zxbhwy.IsEnabled = false;
                                zxbhfront.IsEnabled = false;
                            }
                            else
                            {
                                zxbhwy.IsEnabled = true;
                                zxbhfront.IsEnabled = true;
                            }
                        }

                    }
            }
            
            ZxBh.Text = "0"+ZXBH;
            ZxMc.Text = ZXMC.Name;
            lv2.SelectedIndex = 0; 
        }

        private void zxbhfront_Click(object sender, RoutedEventArgs e)
        {
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            int nodenum = -1;
            int maxid = 0;
            Node ZXMC = new Node();
            bool IsEmpty=true;
            string ZXBH = "";
            ZXBH = ZxBh.Text;
            
            while(IsEmpty){
                 IsEmpty = true;
                 ZXBH = (Convert.ToInt64(ZXBH) + 1).ToString();
                 for (int i = 0; i < nodes.Count; i++)
                 {
                    Node newnode = (Node)httree[nodes[i].ID];
                    if (newnode.ParentID == Convert.ToInt64(ZXBH))
                    {
                        IsEmpty=false;
                        listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                        nodenum++;
                        if (nodenum == 0)
                            ZXMC = newnode;
                    }
                    if (newnode.ID == Convert.ToInt64(ZXBH))
                    {
                        if ((ZXBH == "1" + newnode.ParentID.ToString() + "001") || (ZXBH == "10" + newnode.ParentID.ToString() + "001"))
                        {
                            zxbhback.IsEnabled = false;
                            zxbhsy.IsEnabled = false;
                            zxbhwy.IsEnabled = true;
                            zxbhfront.IsEnabled = true;

                        }
                        else
                        {
                            zxbhback.IsEnabled = true;
                            zxbhsy.IsEnabled = true;
                        }
                        for (int j = 0; j < nodes.Count; j++)
                        {
                            Node newnode1 = (Node)httree[nodes[j].ID];
                            if (newnode1.ParentID == newnode.ParentID)
                            {

                                if (newnode1.ID > maxid)
                                    maxid = newnode1.ID;

                            }
                        }
                        if (maxid.ToString() == ZXBH)
                        {
                            zxbhback.IsEnabled = true;
                            zxbhsy.IsEnabled = true;
                            zxbhwy.IsEnabled = false;
                            zxbhfront.IsEnabled = false;
                        }
                        else
                        {
                            zxbhwy.IsEnabled = true;
                            zxbhfront.IsEnabled = true;
                        }
                    }
                 }
            }
            
            ZxBh.Text = "0"+ZXBH;
            ZxMc.Text = ZXMC.Name;
            lv2.SelectedIndex = 0; 
        }

        private void combobox_symptom_types_input_DropDownClosed(object sender, EventArgs e)
        {
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            string zxlxname = combobox_symptom_types_input.Text.Trim();
            int ZXLXBH = 0, nodenum = -1;
            Node ZXMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.Name.Trim() == zxlxname.Trim())
                {
                    if(newnode.ParentID==-1)
                    {
                        ZXLXBH = newnode.ID;
                    }
                    
                }
            }
            string ZXBH = "";
            if (ZXLXBH < 10)
                ZXBH = "10" + ZXLXBH + "001";
            else
                ZXBH = "1" + ZXLXBH + "001";
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ID == Convert.ToInt64(ZXLXBH))
                {
                    zxlxmc.Text = newnode.Name;
                }

            }
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == Convert.ToInt64(ZXBH))
                {
                    listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                    nodenum++;
                    if (nodenum == 0)
                        ZXMC = newnode;
                }

            }
            ZxBh.Text ="0"+ ZXBH;
            ZxMc.Text = ZXMC.Name;
            lv2.SelectedIndex = 0;  
        }


        private void add_Click(object sender, RoutedEventArgs e)
        {
            zxlxsy.IsEnabled = false;
            zxlxback.IsEnabled = false;
            zxlxfront.IsEnabled = false;
            zxlxwy.IsEnabled = false;
            zxbhsy.IsEnabled = false;
            zxbhback.IsEnabled = false;
            zxbhfront.IsEnabled = false;
            zxbhwy.IsEnabled = false;
            zxmcsy.IsEnabled = false;
            zxmcback.IsEnabled = false;
            zxmcfront.IsEnabled = false;
            zxmcwy.IsEnabled = false;
            backzxmc.IsEnabled = false;
            savezxmc.IsEnabled = false;
            if (zxlxmc.Text == "")
            {
                MessageBox.Show("症象类型名称不能为空！");
            }
            else if (isadd)
                MessageBox.Show("请先保存！");
            else
            {
            isadd = true;
            string zxname = zxlxmc.Text;
            int ZXLXBH = 0,ZXBH=0;
            Keyboard.Focus(ZxMc);
            ZxMc.Text="";
            ZxMc.IsReadOnly = false;
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.Name == zxname)
                {
                    ZXLXBH = newnode.ID;
                }
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == Convert.ToInt64(ZXLXBH))
                {
                    if (newnode.ID > ZXBH)
                        ZXBH = newnode.ID;
                }

            }
            ZXBH++;
            ZxBh.Text ="0"+ ZXBH.ToString();
            listSymptom2.Clear();
            savebutton.IsEnabled = true;
            del_zxbh.IsEnabled = false;
            zxlxforsave = ZXLXBH.ToString();
            zxbhforsave = ZxBh.Text;
            
            }
            
        }

        private void ZxMc_LostFocus(object sender, RoutedEventArgs e)
        {
            if (isadd)
            {
                lv2.ItemsSource = listSymptom2;
                listSymptom2.Clear();
                listSymptom2.Add(new SymptomInfo(ZxBh.Text, ZxMc.Text));
            }
            else if (is_zxmc_add || is_zxmc_edit)
            {
                SymptomInfo Sel_Info = lv2.SelectedItem as SymptomInfo;
                Sel_Info.SymptomName = ZxMc.Text;
            }
            
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (ZxMc.Text == "")
            {
                MessageBox.Show("请输入症象名称！");
                Keyboard.Focus(ZxMc);
            }
                
            else
            {
                isadd = false;
                zxmcforsave = ZxMc.Text;
                //外键约束，比如B表存在一个字段b，有外键约束，引用于A表的主键a，那么在向B表插入数据时，字段b必须为A表中a已经存在的值，如过向b中存放一个a中没有的值，则会报违反外键约束。
                try      
                {
                    string sql = String.Format("INSERT INTO t_info_zxxx (zxbh,zxlxbh) VALUES ('{0}', '{1}')", zxbhforsave, "0" + zxlxforsave);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    if (count > 0)
                    {
                        MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("添加失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                finally
                {
                    conn.Close();
                    Text_Readonly();
                }
                try
                {
                    string sql = String.Format("INSERT INTO t_info_zxmx (zxbh,zxmc) VALUES ('{0}', '{1}')", zxbhforsave, zxmcforsave);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    if (count > 0)
                    {
                        MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                        savebutton.IsEnabled = false;
                        del_zxbh.IsEnabled = true;
                        zxlxsy.IsEnabled = true;
                        zxlxback.IsEnabled = true;
                        zxlxfront.IsEnabled = true;
                        zxlxwy.IsEnabled = true;
                        zxbhsy.IsEnabled = true;
                        zxbhback.IsEnabled = true;
                        zxbhfront.IsEnabled = true;
                        zxbhwy.IsEnabled = true;
                        zxmcsy.IsEnabled = true;
                        zxmcback.IsEnabled = true;
                        zxmcfront.IsEnabled = true;
                        zxmcwy.IsEnabled = true;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("添加失败！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                finally
                {
                    conn.Close();
                    Text_Readonly();
                }
                reftree();
            }
        }


        private void delete_Click(object sender, RoutedEventArgs e)
        {
            int Isdel = 0, nodenum=-1;
            bool IsEmpty=true;
            Node ZXMC=new Node();
            lv2.ItemsSource = listSymptom2;
            if (MessageBox.Show("确定要删除该项吗？", "确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                listSymptom2.Clear();
                try
                {
                    string sql = String.Format("delete from t_info_zxmx where zxbh = '{0}'", ZxBh.Text);
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
                    string sql = String.Format("delete from t_info_zxxx where zxbh = '{0}'", ZxBh.Text);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    int count = comm.ExecuteNonQuery();
                    if (count > 0&&Isdel==1)
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
                if(Isdel==2)
                {
                    while(IsEmpty){
                            IsEmpty = true;
                            ZxBh.Text = (Convert.ToInt64(ZxBh.Text) - 1).ToString();
                            for (int i = 0; i < nodes.Count; i++)
                            {
                                
                                Node newnode = (Node)httree[nodes[i].ID];
                                if (newnode.ParentID == Convert.ToInt64(ZxBh.Text))
                                {
                                    IsEmpty = false;
                                    listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                                    nodenum++;
                                    if (nodenum == 0)
                                        ZXMC = newnode;
                                }

                            }
                    }
                    ZxMc.Text = ZXMC.Name;
                    ZxBh.Text ="0"+ ZXMC.ParentID.ToString();
                    lv2.SelectedIndex = 0;

                }
                reftree();
            }                        
        }

        private void zxmcsy_Click(object sender, RoutedEventArgs e)
        {
            SymptomInfo Sel_Info=new SymptomInfo("","");
            lv2.SelectedIndex = 0;
            Sel_Info = lv2.SelectedItem as SymptomInfo;
            ZxMc.Text = Sel_Info.SymptomName;
            zxmcsy.IsEnabled = false;
            zxmcback.IsEnabled = false;
            zxmcfront.IsEnabled = true;
            zxmcwy.IsEnabled = true;
        }

        private void zxmcwy_Click(object sender, RoutedEventArgs e)
        {
            lv2.SelectedIndex = lv2.Items.Count - 1;
            SymptomInfo Sel_Info = new SymptomInfo("", "");
            Sel_Info = lv2.SelectedItem as SymptomInfo;
            ZxMc.Text = Sel_Info.SymptomName;
            zxmcsy.IsEnabled = true;
            zxmcback.IsEnabled = true;
            zxmcfront.IsEnabled = false;
            zxmcwy.IsEnabled = false;
        }

        private void zxmcback_Click(object sender, RoutedEventArgs e)
        {
            if (lv2.SelectedIndex == -1 || lv2.SelectedIndex==0)
            {
                zxmcsy.IsEnabled = false;
                zxmcback.IsEnabled = false;
                zxmcfront.IsEnabled = true;
                zxmcwy.IsEnabled = true;
            }
            else
            {
                lv2.SelectedIndex = lv2.SelectedIndex - 1;
                SymptomInfo Sel_Info = new SymptomInfo("", "");
                Sel_Info = lv2.SelectedItem as SymptomInfo;
                ZxMc.Text = Sel_Info.SymptomName;
            }
            
        }

        private void zxmcfront_Click(object sender, RoutedEventArgs e)
        {
            if (lv2.SelectedIndex == -1 || lv2.SelectedIndex ==lv2.Items.Count - 1)
            {
                zxmcsy.IsEnabled = true;
                zxmcback.IsEnabled = true;
                zxmcfront.IsEnabled = false;
                zxmcwy.IsEnabled = false;
            }
            else
            {
                lv2.SelectedIndex = lv2.SelectedIndex + 1;
                SymptomInfo Sel_Info = new SymptomInfo("", "");
                Sel_Info = lv2.SelectedItem as SymptomInfo;
                ZxMc.Text = Sel_Info.SymptomName;
            }
            
        }

        private void addzxmc_Click(object sender, RoutedEventArgs e)
        {
            if(is_zxmc_add)
            {
                MessageBox.Show("请先保存！");
            }
            else
            {
                
                ZxMc.Text = "";
                ZxMc.IsReadOnly = false;
                listSymptom2.Insert(lv2.SelectedIndex,new SymptomInfo(ZxBh.Text,""));
                lv2.SelectedIndex--;
                nochange_item=lv2.SelectedIndex;
                is_zxmc_add = true;
                zxlxsy.IsEnabled = false;
                zxlxback.IsEnabled = false;
                zxlxfront.IsEnabled = false;
                zxlxwy.IsEnabled = false;
                zxbhsy.IsEnabled = false;
                zxbhback.IsEnabled = false;
                zxbhfront.IsEnabled = false;
                zxbhwy.IsEnabled = false;
                del_zxbh.IsEnabled = false;
                addzxbh.IsEnabled = false;
                zxmcsy.IsEnabled = false;
                zxmcback.IsEnabled = false;
                zxmcfront.IsEnabled = false;
                zxmcwy.IsEnabled = false;
                editzxmc.IsEnabled = false;
                backzxmc.IsEnabled = true;
                savezxmc.IsEnabled = true;
                del_zxmc.IsEnabled = false;
            }
            
        }

        private void savezxmc_Click(object sender, RoutedEventArgs e)
        {
            Is_Repeat();
            if (ZxMc.Text == "")
                MessageBox.Show("症象名称不能为空！");
            if (IsRepeat== false&&ZxMc.Text!="")
            {
                if(is_zxmc_edit==true)
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
                        string sql = String.Format("UPDATE t_info_zxmx SET zxbh='{0}',zxmc='{1}' WHERE id='{2}'", ZxBh.Text, ZxMc.Text,newnode.ID.ToString());
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                        if (count > 0)
                        {
                            MessageBox.Show("修改成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            is_zxmc_edit = false;
                            backzxmc.IsEnabled = false;
                            savezxmc.IsEnabled = false;
                            addzxmc.IsEnabled = true;
                            editzxmc.IsEnabled = true;
                            zxmcsy.IsEnabled = true;
                            zxmcback.IsEnabled = true;
                            zxmcfront.IsEnabled = true;
                            zxmcwy.IsEnabled = true;
                            del_zxmc.IsEnabled = true;
                            zxlxsy.IsEnabled = true;
                            zxlxback.IsEnabled = true;
                            zxlxfront.IsEnabled = true;
                            zxlxwy.IsEnabled = true;
                            zxbhsy.IsEnabled = true;
                            zxbhback.IsEnabled = true;
                            zxbhfront.IsEnabled = true;
                            zxbhwy.IsEnabled = true;
                            del_zxbh.IsEnabled = true;
                            addzxbh.IsEnabled = true;
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
                if(is_zxmc_add==true)
                {
                    try
                    {
                        string sql = String.Format("INSERT INTO t_info_zxmx (zxbh,zxmc) VALUES ('{0}', '{1}')", ZxBh.Text, ZxMc.Text);
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        int count = comm.ExecuteNonQuery();
                        if (count > 0)
                        {
                            MessageBox.Show("添加成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                            is_zxmc_add = false;
                            backzxmc.IsEnabled = false;
                            savezxmc.IsEnabled = false;
                            addzxmc.IsEnabled = true;
                            editzxmc.IsEnabled = true;
                            zxmcsy.IsEnabled = true;
                            zxmcback.IsEnabled = true;
                            zxmcfront.IsEnabled = true;
                            zxmcwy.IsEnabled = true;
                            del_zxmc.IsEnabled = true;
                            zxlxsy.IsEnabled = true;
                            zxlxback.IsEnabled = true;
                            zxlxfront.IsEnabled = true;
                            zxlxwy.IsEnabled = true;
                            zxbhsy.IsEnabled = true;
                            zxbhback.IsEnabled = true;
                            zxbhfront.IsEnabled = true;
                            zxbhwy.IsEnabled = true;
                            del_zxbh.IsEnabled = true;
                            addzxbh.IsEnabled = true;
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
                ZxMc.IsReadOnly = true;
                reftree();
            }
            
     
        }
        /// <summary>
        /// 功能：防止姓名重复
        /// </summary>
        public void Is_Repeat()
        {
            string zxname = ZxMc.Text.Trim();
            string sql = String.Format("select count(*) from t_info_zxmx where zxbh = '{0}' and zxmc='{1}' ",ZxBh.Text, ZxMc.Text);
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            int count = (int)comm.ExecuteScalar();
            if (count == 1)
            {
                MessageBox.Show("该症象名称已存在！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                IsRepeat = true;
            }
            else
                IsRepeat = false;
            conn.Close();
        }

        private void del_zxmc_Click(object sender, RoutedEventArgs e)
        {
            bool IsEmpty = true;
            Node ZXMC = new Node();
            int nodenum = -1;
            if (MessageBox.Show("确定要删除该项吗？", "确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    string sql = String.Format("delete from t_info_zxmx where zxmc = '{0}'", ZxMc.Text);
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
                if (lv2.SelectedIndex == lv2.Items.Count - 1)
                {
                    lv2.SelectedIndex--;
                    listSymptom2.RemoveAt(lv2.SelectedIndex + 1);
                }
                else if (lv2.Items.Count==1)
                {
                    while (IsEmpty)
                    {
                        IsEmpty = true;
                        ZxBh.Text = (Convert.ToInt64(ZxBh.Text) - 1).ToString();
                            for (int i = 0; i < nodes.Count; i++)
                            {
                                
                                Node newnode = (Node)httree[nodes[i].ID];
                                if (newnode.ParentID == Convert.ToInt64(ZxBh.Text))
                                {
                                    IsEmpty = false;
                                    listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                                    nodenum++;
                                    if (nodenum == 0)
                                        ZXMC = newnode;
                                }

                            }
                    }
                    ZxMc.Text = ZXMC.Name;
                    ZxBh.Text ="0"+ ZXMC.ParentID.ToString();
                    lv2.SelectedIndex = 0;
                }
                else
                {
                    lv2.SelectedIndex++;
                    listSymptom2.RemoveAt(lv2.SelectedIndex-1);   
                }
                             
                SymptomInfo Sel_Info = lv2.SelectedItem as SymptomInfo;
                ZxMc.Text = Sel_Info.SymptomName;
                reftree();
            }
        }

        private void backzxmc_Click(object sender, RoutedEventArgs e)
        {
            if(is_zxmc_add||is_zxmc_edit)
            {
                is_zxmc_edit = false;
                is_zxmc_add = false;
                lv2.ItemsSource = listSymptom2;
                listSymptom2.Clear();
                string ZXBH = ZxBh.Text;
                int nodenum = -1;
                Node ZXMC = new Node();
                for (int i = 0; i < nodes.Count; i++)
                {
                    Node newnode = (Node)httree[nodes[i].ID];
                    if (newnode.ParentID == Convert.ToInt64(ZXBH))
                    {
                        listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                        nodenum++;
                        if (nodenum == 0)
                            ZXMC = newnode;
                    }

                }
                ZxMc.Text = ZXMC.Name;
                lv2.SelectedIndex = nochange_item;
                backzxmc.IsEnabled = false;
                savezxmc.IsEnabled = false;
                editzxmc.IsEnabled = true;
                addzxmc.IsEnabled = true;
                zxmcsy.IsEnabled = true;
                zxmcback.IsEnabled = true;
                zxmcfront.IsEnabled = true;
                zxmcwy.IsEnabled = true;
                del_zxmc.IsEnabled = true;
                zxlxsy.IsEnabled = true;
                zxlxback.IsEnabled = true;
                zxlxfront.IsEnabled = true;
                zxlxwy.IsEnabled = true;
                zxbhsy.IsEnabled = true;
                zxbhback.IsEnabled = true;
                zxbhfront.IsEnabled = true;
                zxbhwy.IsEnabled = true;
                del_zxbh.IsEnabled = true;
                addzxbh.IsEnabled = true;
            }
        }

        private void shuaxin_Click(object sender, RoutedEventArgs e)
        {
            httree.Clear();
            nodes.Clear();
            // 一级树写入           
            string sql = String.Format("select * from t_info_zxlx");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node { ID = Convert.ToInt32(dr["zxlxbh"]), Name = dr["zxlxmc"].ToString().Trim() });
            }
            dr.Close();
            conn.Close();

            // 二级树写入
            sql = String.Format("select t1.zxbh ,t2.zxlxbh, min(t3.zxmc) from (t_info_zxxx as t1 inner join t_info_zxlx as t2 on t2.zxlxbh = t1.zxlxbh) inner join  t_info_zxmx as t3 on t1.zxbh = t3.zxbh group by t1.zxbh, t2.zxlxbh");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                //？？？ 有问题，数据库中的 zxlxbh = 30 对应的 zxbh 为空会出现问题，赋值为0问题解决
                nodes.Add(new Node { ID = Convert.ToInt32(dr["zxbh"]), Name = dr["zxbh"].ToString() + "  " + dr[2].ToString(), ParentID = Convert.ToInt32(dr["zxlxbh"]) });
                //nodes.Add(new Node { ID = Convert.ToInt32(dr["zxbh"]), Name = dr["zxbh"].ToString() + "  " + "默认名称", ParentID = Convert.ToInt32(dr["zxlxbh"]) });
            }
            dr.Close();
            conn.Close();

            // 三级树写入
            sql = String.Format("select t_info_zxmx.id,t_info_zxmx.zxmc,t_info_zxmx.zxbh from (t_info_zxxx inner join t_info_zxlx on t_info_zxlx.zxlxbh = t_info_zxxx.zxlxbh) inner join  t_info_zxmx on t_info_zxxx.zxbh = t_info_zxmx.zxbh order by t_info_zxmx.zxmc");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node { ID = Convert.ToInt32(dr["id"]), Name = dr["zxmc"].ToString(), ParentID = Convert.ToInt32(dr["zxbh"]) });
            }
            dr.Close();
            conn.Close();
            BuildENTree();
            Text_Readonly();
            lv2.ItemsSource = listSymptom2;
            listSymptom2.Clear();
            string ZXBH = ZxBh.Text;
            int nodenum = -1;
            Node ZXMC = new Node();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node newnode = (Node)httree[nodes[i].ID];
                if (newnode.ParentID == Convert.ToInt64(ZXBH))
                {
                    listSymptom2.Add(new SymptomInfo("0" + newnode.ParentID.ToString(), newnode.Name));
                    nodenum++;
                    if (nodenum == 0)
                        ZXMC = newnode;
                }

            }
            ZxMc.Text = ZXMC.Name;
            lv2.SelectedIndex = 0;
        }

        private void editzxmc_Click(object sender, RoutedEventArgs e)
        {
            if (is_zxmc_edit)
            {
                MessageBox.Show("请先保存！");
            }
            else
            {

                ZxMc.Text = "";
                ZxMc.IsReadOnly = false;
                SymptomInfo Sel_Info = lv2.SelectedItem as SymptomInfo;
                nochange_name=Sel_Info.SymptomName;
                Sel_Info.SymptomName= ZxMc.Text;
                nochange_item = lv2.SelectedIndex;
                is_zxmc_edit = true;
                zxlxsy.IsEnabled = false;
                zxlxback.IsEnabled = false;
                zxlxfront.IsEnabled = false;
                zxlxwy.IsEnabled = false;
                zxbhsy.IsEnabled = false;
                zxbhback.IsEnabled = false;
                zxbhfront.IsEnabled = false;
                zxbhwy.IsEnabled = false;
                del_zxbh.IsEnabled = false;
                addzxbh.IsEnabled = false;
                zxmcsy.IsEnabled = false;
                zxmcback.IsEnabled = false;
                zxmcfront.IsEnabled = false;
                zxmcwy.IsEnabled = false;
                addzxmc.IsEnabled = false;
                backzxmc.IsEnabled = true;
                savezxmc.IsEnabled = true;
                del_zxmc.IsEnabled = false;
            }
        }
        public void reftree()
        {
            httree.Clear();
            nodes.Clear();
            // 一级树写入           
            string sql = String.Format("select * from t_info_zxlx");
            conn.Open();
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node { ID = Convert.ToInt32(dr["zxlxbh"]), Name = dr["zxlxmc"].ToString().Trim() });
            }
            dr.Close();
            conn.Close();

            // 二级树写入
            sql = String.Format("select t1.zxbh ,t2.zxlxbh, min(t3.zxmc) from (t_info_zxxx as t1 inner join t_info_zxlx as t2 on t2.zxlxbh = t1.zxlxbh) inner join  t_info_zxmx as t3 on t1.zxbh = t3.zxbh group by t1.zxbh, t2.zxlxbh");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                //？？？ 有问题，数据库中的 zxlxbh = 30 对应的 zxbh 为空会出现问题，赋值为0问题解决
                nodes.Add(new Node { ID = Convert.ToInt32(dr["zxbh"]), Name = dr["zxbh"].ToString() + "  " + dr[2].ToString(), ParentID = Convert.ToInt32(dr["zxlxbh"]) });
                //nodes.Add(new Node { ID = Convert.ToInt32(dr["zxbh"]), Name = dr["zxbh"].ToString() + "  " + "默认名称", ParentID = Convert.ToInt32(dr["zxlxbh"]) });
            }
            dr.Close();
            conn.Close();

            // 三级树写入
            sql = String.Format("select t_info_zxmx.id,t_info_zxmx.zxmc,t_info_zxmx.zxbh from (t_info_zxxx inner join t_info_zxlx on t_info_zxlx.zxlxbh = t_info_zxxx.zxlxbh) inner join  t_info_zxmx on t_info_zxxx.zxbh = t_info_zxmx.zxbh order by t_info_zxmx.zxmc");
            conn.Open();
            comm = new SqlCommand(sql, conn);
            dr = comm.ExecuteReader();
            while (dr.Read())
            {
                nodes.Add(new Node { ID = Convert.ToInt32(dr["id"]), Name = dr["zxmc"].ToString(), ParentID = Convert.ToInt32(dr["zxbh"]) });
            }
            dr.Close();
            conn.Close();
            BuildENTree();
            Text_Readonly();
        }
    }
}
