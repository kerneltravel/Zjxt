   M S B A M L   `   `   `  ����   中医证治智能系统'    !中医证治智能系统.RukeRule   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$7  1中医证治智能系统-【入科规则管理】��$	��700��$	��900�� � WindowStartupLocation$ CenterScreen=�.���  � ColumnDefinitions ��  � RowDefinitions �� $��70 ��� $	��90* ��� $
��107* ��� $
��403* �.+��� $��70��$	��Top=� �� Orientation$ 
Horizontal=�   ButtonNormalBackground#���   .+��� -   $	��add��$��55�� J� RenderTransformOrigin$ 0.505,0.355&�$��60��.��� $��55��$��49��.+��� $
��Left=�$��31��$��9,2,0,0q�$	��Top=�$��30��$��Resources/plus.ico���� $��增加��$��0,30,0,0q�$��25��$��49��$��Center=�$��Center=�$��Center=�$��Center=��� -   $��delete��$��55��$ 0.505,0.355&�$��60��.��� $��55��$��49��.+��� $
��Left=�$��5��$��	10,16,0,0q�$	��Top=�$��29��$��Resources/minus.ico���� $��删除��$��0,30,0,0q�$��25��$��49��$��Center=�$��Center=�$��Center=�$��Center=��� -   $
��save��$��55��$ 0.505,0.355&�$��60��.��� $��55��$��49��.+��� $
��Left=�$��30��$��10,0,0,0q�$	��Top=�$��30��$��Resources/save.ico��$ 	0.567,0.5&��� $��保存��$��0,30,0,0q�$��25��$��49��$��Center=�$��Center=�$��Center=�$��Center=��� -   $��55��$ 0.505,0.355&�$��60��.��� $��55��$��49��.+��� $
��Left=�$��26��$��11,4,0,0q�$	��Top=�$��28��$��Resources/error.ico���� $��取消��$��0,30,0,0q�$��25��$��49��$��Center=�$��Center=�$��Center=�$��Center=��� -   $��55��$ 0.505,0.355&�$��60��.��� $��55��$��49��.+��� $
��Left=�$��34��$��8,0,0,0q�$	��Top=�$��33��$��Resources/home.ico���� $��返回��$��0,30,0,0q�$��25��$��49��$��Center=�$��Center=�$��Center=�$��Center=��� $��1��$��Center=�$��Center=�$��90��$	��892��.+��� $��入 科 规 则 录 入��$
��Left=�$��0,0,0,0q�$��1��$	��Top=�$	��892�����	#FF1414FD$��36(�$��Center=�$��Center=�$��90���� $��1��$��Center=�$��Center=�$	��892��$	��109��$��0,88,0,0q�$��2��.+��� $��录入框：��$
��Left=�$��90,0,0,0q�$	��Top=�$	��100��$	��720��$��14(�.��� .+��� $��入科名称：��$
��Left=�$��	38,19,0,0q�$	��Top=�$��31��$��79��$��Center=��� -   $
��zxmc��$
��Left=�$��24��$��
410,27,0,0q� �� TextWrapping$
 Wrap=�$�� ��$	��Top=�$	��180���� -   $��combobox_rk��$
��Left=�$��
122,26,0,0q�$	��Top=�$	��180��$��24�� �� 
IsEditable . �� $��症象编号：��$
��Left=�$��
326,20,0,0q�$	��Top=�$��31��$��79��$��Center=��� -   $��. . .��$
��Left=�$��
604,27,0,0q�$	��Top=�$��40��$��24���� $��3��$��Center=�$��Center=�$	��892��$	��383��$
��0,10q�.+��� -	   $��lv��$
��Left=�$	��338��$��	90,29,0,0q�$	��Top=�$	��719�� �� View �� .P��� $��入科编号��	 �� Width$	 60��
 �� DisplayMemberBinding
 ��  �� Path$ RkNumber��� $��入科名称��$		 150��
 �� $ RkName��� $��症象编号��$		 150��
 �� $ ZxNumber��� $��症象名称��$		 300��
 �� $ ZxName�