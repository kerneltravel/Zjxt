   M S B A M L   `   `   `  ����   中医证治智能系统'    !中医证治智能系统.rukeInfo   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$;  5中医证治智能系统V2.0-【入科信息管理】��$	��700��$	��900�� � WindowStartupLocation$ CenterScreen=�.���  � ColumnDefinitions ��  � RowDefinitions �� $��70 ��� $	��45* ��� $
��122* ��� $
��133* �.+��� $��70��$	��Top=� �� Orientation$ 
Horizontal=�   ButtonNormalBackground#���   .+��� -   $��
button_add��$��55�� J� RenderTransformOrigin$ 0.505,0.355&�$��60��.��� $��55��$��49��.+��� $
��Left=�$��31��$��9,2,0,0q�$	��Top=�$��30��$��Resources/plus.ico���� $��增加��$��0,30,0,0q�$��25��$��49��$��Center=�$��Center=�$��Center=�$��Center=��� -   $��button_delete��$��55��$ 0.505,0.355&�$��60��.��� $��55��$��49��.+��� $
��Left=�$��5��$��	10,16,0,0q�$	��Top=�$��29��$��Resources/minus.ico���� $��删除��$��0,30,0,0q�$��25��$��49��$��Center=�$��Center=�$��Center=�$��Center=��� -   $��button_modify��$��55��$ 0.505,0.355&�$��60��.��� $��55��$��49��.+��� $
��Left=�$��35��$��6,6,0,0q�$	��Top=�$��37��$��Resources/arrow_up.ico���� $��修改��$��0,30,0,0q�$��25��$��49��$��Center=�$��Center=�$��Center=�$��Center=��� -   $��button_save��$��55��$ 0.505,0.355&�$��60��.��� $��55��$��49��.+��� $
��Left=�$��30��$��10,0,0,0q�$	��Top=�$��30��$��Resources/save.ico��$ 	0.567,0.5&��� $��保存��$��0,30,0,0q�$��25��$��49��$��Center=�$��Center=�$��Center=�$��Center=��� -   $��button_cancel��$��55��$ 0.505,0.355&�$��60��.��� $��55��$��49��.+��� $
��Left=�$��26��$��11,4,0,0q�$	��Top=�$��28��$��Resources/error.ico���� $��取消��$��0,30,0,0q�$��25��$��49��$��Center=�$��Center=�$��Center=�$��Center=��� -   $��button_back��$��55��$ 0.505,0.355&�$��60��.��� $��55��$��49��.+��� $
��Left=�$��34��$��8,0,0,0q�$	��Top=�$��33��$��Resources/home.ico���� $��返回��$��0,30,0,0q�$��25��$��49��$��Center=�$��Center=�$��Center=�$��Center=��� $��1��$��Center=�$��Center=�.+��� $��入 科 信 息 录 入��$
��Left=�$��0,0,0,0q�$��1��$	��Top=�$	��892�����	#FF1414FD$��36(�$��Center=�$��Center=�$��90���� $��2��$��Center=�$��Center=�$	��892��$	��244��.+��� $��录入框：��$
��Left=�$��90,0,0,0q�$	��Top=�$	��223��$	��720��$��14(�.��� .+��� $��入科名称：��$
��Left=�$��	38,10,0,0q�$	��Top=�$��31��$��79��$��Center=��� $��	备注：��$��38,65,591,101q�$��Center=�$ 	0.54,0.96&��� $��入科编号：��$��318,10,311,156q��� -   $��text_box_rkmc��$
��Left=�$��25��$��
122,10,0,0q� �� TextWrapping$
 Wrap=�$�� ��$	��Top=�$	��177���� -   $��text_block_rkbh��$
��Left=�#���   $��25��$��
415,10,0,0q�$
��Wrap=�$�� ��$	��Top=�$	��177���� -	   $��text_box_bz��$��122,71,116,10q��� $��3��$��Center=�$��Center=�$	��892��$	��266��.+��� -
    ��Name$	 lv1��$
��Left=�$	��200��$��	90,29,0,0q�$	��Top=�$	��719��$ �� IsSynchronizedWithCurrentItem$ FalseY�	 �� View	 �� 
 �� AllowsColumnReorder
 . .P��� $��入科编号�� �� Width$ 60�� �� DisplayMemberBinding ��  �� Path$ XiNumber��� $��入科名称��$	 150�� �� $ XiName��� $��备注��$	 482�� �� $ BeiZu�