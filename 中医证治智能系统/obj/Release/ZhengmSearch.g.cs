﻿#pragma checksum "..\..\ZhengmSearch.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F96535C1EF61AC1E0637771DF0EA8662"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace 中医证治智能系统 {
    
    
    /// <summary>
    /// ZhengmSearch
    /// </summary>
    public partial class ZhengmSearch : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\ZhengmSearch.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox bmmc;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\ZhengmSearch.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_bmmc;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\ZhengmSearch.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button back;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\ZhengmSearch.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lv;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\ZhengmSearch.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button display;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/中医证治智能系统;component/zhengmsearch.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ZhengmSearch.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.bmmc = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.btn_bmmc = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\ZhengmSearch.xaml"
            this.btn_bmmc.Click += new System.Windows.RoutedEventHandler(this.btn_bmmc_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.back = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\ZhengmSearch.xaml"
            this.back.Click += new System.Windows.RoutedEventHandler(this.back_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.lv = ((System.Windows.Controls.ListView)(target));
            return;
            case 5:
            this.display = ((System.Windows.Controls.Button)(target));
            
            #line 31 "..\..\ZhengmSearch.xaml"
            this.display.Click += new System.Windows.RoutedEventHandler(this.display_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

