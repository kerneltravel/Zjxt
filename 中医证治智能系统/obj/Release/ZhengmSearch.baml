   M S B A M L   `   `   `  ����   中医证治智能系统+    %中医证治智能系统.ZhengmSearch   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$G  A中医证治智能系统V2.0-【病名检索证明信息管理】��$	��650��$	��900�� � WindowStartupLocation$ CenterScreen=�.��� #��� �� � RowDefinitions �� $��87 ��� .+��� $��查询条件��   TabItemHotBackground#���   $	��Top=��� $��病名名称：��$
��Left=�$��	24,43,0,0q�$	��Top=��� -   $
��bmmc��$
��Left=�$��24��$��	99,45,0,0q� �� TextWrapping$
 Wrap=�$�� ��$	��Top=�$	��300���� -   $��btn_bmmc��$��. . .��$��24��$
��Left=�$��
420,45,0,0q�$	��Top=�$��40���� -   $
��back��$
��Left=�$��66��$��
529,43,0,0q�$	��Top=�.��� $��24��$��59��.+��� $
��Left=�$��3,4,0,3q�$��18��$��Resources/home.ico��$��Center=�$��17���� $��返回��$��0,0,4,0q�$��Right=�$��Center=�$��34��$��24��� $
��Left=�$��1��$	��892��#��� ��.+��� -   $��lv��$
��Left=�$��
30,46,0,10q�$	��823�� �� View �� .P��� $��病人编号�� �� Width$	 150�� �� DisplayMemberBinding ��  �� Path$ BmNumber��� $��病人名称��$	 250�� �� $ BmName��� $��基本证名编号��$	 150�� �� $ BasiczmNumber��� $��基本证明名称��$	 250�� �� $ BasiczmName��� -   $��display��$��
749,10,0,0q�$
��Left=�$	��104��$��31��$	��Top=�.��� $��26��$��94��.+��� $
��Left=�$��6,2,0,0q�$��28��$��Resources/search.ico��$��Center=�$��24���� $��显示全部��$��0,0,4,2q�$��Right=�$��Center=�$��64��$��24��