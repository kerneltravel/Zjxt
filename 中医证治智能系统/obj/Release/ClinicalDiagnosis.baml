   M S B A M L   `   `   `  ����   中医证治智能系统0    *中医证治智能系统.ClinicalDiagnosis   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$4  .中医证治智能系统V2.0【临床诊断】��$	��650��$	��860�� � WindowStartupLocation$ CenterScreen=�.����    ButtonNormalBackground#���   .+��  � RowDefinitions �� $��60 ��� $��80 ��� $	��450 ��� $��40 �.+��� -   $��newDiagnosis��$��新建诊断��$��16(�$��0��$��
10,10,0,10q�$
��Left=�$��74���� -    ��Name$ 	Diagnosis��$��诊断��$��16(�$��0��$��106,10,0,10q�$
��Left=�$��54���� -   $ clear��$��清除��$��16(�$��0��$��184,10,0,10q�$
��Left=�$��54���� -   $
 back��$��退出��$��16(�$��0��$��255,10,0,10q�$
��Left=�$��51���� $��病 人 信 息 录 入��$��18(�$��1��$��Center=�$	��852��$��0,0,0,45q��� $��病人编号：��$��20(�$��1��$��Center=�$
��Left=�$��	10,29,0,0q��� $��	姓名：��$��20(�$
��Left=�$��
359,37,0,0q�$��1��$	��Top=�$��40��$��Center=� J� RenderTransformOrigin$ 0.971,1.242&��� -   $��xm��{�.  $
��Left=�$��28��$��
434,43,0,0q�$��1�� �� TextWrapping$
 Wrap=�$�� ��$	��Top=�$	��172���� -   $��search��$��病人信息提取��$��18(�$��1��$��
657,37,0,8q�$
��Left=�$	��130���� -    ��Name$
 brbh��{�.  $
��Left=�$��28��$��
125,43,0,0q�$��1��$
 Wrap=�$�� ��$	��Top=�$	��181���� $��症状输入框��$��20(�$��1��$��Center=�$
��Left=�$��10,69,0,375q�$��2���� $��症状选择框��$��20(�$��2��$��Center=�$
��Left=�$��10,91,0,295q��� -   $
 zzsr��$
��Left=�$��21��$��	10,70,0,0q�$��2��$
 Wrap=�$�� ��$	��Top=�$	��181���� -	   $ nowzz��$��>>现症状��$��18(�$��2��$��222,17,0,402q�$
��Left=�$	��113���� -
   $ firstzz��$��>>初期症状��$��18(�$��2��$��222,62,0,359q�$
��Left=�$	��113���� -   $ zstime��$��>>主诉时间��$��18(�$��2��$��222,108,0,312q�$
��Left=�$	��113���� -   $ zs��$��>>主诉��$��18(�$��2��$��222,155,0,263q�$
��Left=�$	��113���� -   $ delete��$��删除��$��18(�$��2��$��222,202,0,214q�$
��Left=�$	��113���� -   $ selectbs��$��选择病史��$��18(�$��2��$��222,252,0,165q�$
��Left=�$	��113���� -   $ selxybm��$��选择西医病名��$��18(�$��2��$��222,351,0,64q�$
��Left=�$	��113���� -   $��lv��$��
340,17,5,0q�$��2�� J� 	AllowDrop . $	��Top=�$	��463��$��2�� �� View �� .P��� $��信息类型��	 �� Width$		 150��
 �� DisplayMemberBinding
 ��  �� Path$ WesternMedicineType��� $��信息名称��$		 330��
 �� $ WesternMedicineName��� -   $	��lv1��$��10,155,661,54q�$��2�� �� .P��� $		 160��
 �� $ XYZMBH�	� $��自动生成季节��$��16(�$
��Left=�$	��Top=�$��
22,412,0,0q�$��2��$��26��$	��118���� -   $
 jjxs��#���   $
��Left=�$��30��$��10,0,0,0q�$��3��$
 Wrap=�$�� ��$	��Top=�$	��330�� �� 
IsReadOnly . 