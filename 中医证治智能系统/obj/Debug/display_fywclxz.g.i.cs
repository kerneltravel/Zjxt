﻿#pragma checksum "..\..\display_fywclxz.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "41568726B401C3DE66887AD792564C7F"
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
    /// display_fywclxz
    /// </summary>
    public partial class display_fywclxz : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\display_fywclxz.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox Manual;
        
        #line default
        #line hidden
        
        
        #line 7 "..\..\display_fywclxz.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox Auto;
        
        #line default
        #line hidden
        
        
        #line 8 "..\..\display_fywclxz.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox AllReserved;
        
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
            System.Uri resourceLocater = new System.Uri("/中医证治智能系统;component/display_fywclxz.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\display_fywclxz.xaml"
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
            this.Manual = ((System.Windows.Controls.CheckBox)(target));
            
            #line 6 "..\..\display_fywclxz.xaml"
            this.Manual.Click += new System.Windows.RoutedEventHandler(this.Manual_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Auto = ((System.Windows.Controls.CheckBox)(target));
            
            #line 7 "..\..\display_fywclxz.xaml"
            this.Auto.Click += new System.Windows.RoutedEventHandler(this.Auto_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.AllReserved = ((System.Windows.Controls.CheckBox)(target));
            
            #line 8 "..\..\display_fywclxz.xaml"
            this.AllReserved.Click += new System.Windows.RoutedEventHandler(this.AllReserved_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 9 "..\..\display_fywclxz.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

