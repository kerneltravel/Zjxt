   M S B A M L   `   `   `  ����   中医证治智能系统&     中医证治智能系统.XYBMsel   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$;  5中医证治智能系统V2.0-【西医病名选择】��$	��650��$	��860�� � WindowStartupLocation$ CenterScreen=�.��� ���	#FFE5E5E5 � ColumnDefinitions �� $��0 ��� $	��842 ��� $��0* � � RowDefinitions �� $��25 ��� $��40 ��� $��25 ��� $��40 ��� .+��� $��查询条件��   TabItemHotBackground#���   $��2���� -   $��combobox_xybmlx_input��$��1��$
��Left=�$��
184,10,0,0q�$	��Top=�$	��180��$��24��$��1���� $��西医病名名称：��$
��Left=�$��	447,0,0,0q�$��1��$	��Top=�$��40��$��Center=�$��Right=�$��90��$��1���� $��西医病名类型：��$
��Left=�$��91,0,0,0q�$��1��$	��Top=�$��40��$��Center=�$��Right=�$��88��$��1���� -   $��text_box_bjmc��$
��Left=�$��24��$��
542,10,0,0q�$��1�� �� TextWrapping$
 Wrap=�$�� ��$	��Top=�$	��231��$��1���� -    ��Name$ search��$��3��$��1��$��213,5,560,5q�$��Center=�$��70��.��� $��23��$��50��.+��� $
��Left=�$��-6,5,0,0q�$��21��$��Resources/search.ico��$��Center=�$��18���� $��查找��$��
10,0,-1,-1q�$��Center=�$��24���� $��查询结果��#���   $��2��$��2���� -   $ select_search��$��3��$��1��$��388,5,385,5q�$��Center=�$��70��.��� $��24��$��56��.+��� $
��Left=�$��5,6,0,4q�$��17��$��Resources/check.ico��$��Center=� J� RenderTransformOrigin$ 0.651,1.015&�$��14���� $��选定��$��Right=�$��Center=�$��34��$��24���� -   $ back_search��$��3��$��1��$��563,5,210,5q�$��Center=�$��70��.��� $��24��$��54��.+��� $
��Left=�$��0,5,0,3q�$��17��$��Resources/home.ico��$��Center=�$��16���� $��返回��$��Right=�$��Center=�$��37��$��24���� -   $��lv��$��4��$��9,0,6,0q�$��1�� �� View �� .P��� $��西医病名类型�� �� Width$	 150��	 �� DisplayMemberBinding	 �� 
 �� Path$

 type��� $��西医病名编号��$	 300��	 �� $
 BingjiNumber��� $��西医病名名称��$	 400��	 �� $
 
BingjiName�