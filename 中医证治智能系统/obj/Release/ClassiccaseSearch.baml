   M S B A M L   `   `   `  ����   中医证治智能系统0    *中医证治智能系统.ClassiccaseSearch   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$;  5中医证治智能系统V2.0-【经典案例检索】��$	��650��$	��900�� � WindowStartupLocation$ CenterScreen=�.��� #��� �� � ColumnDefinitions �� $	��334 ���  � RowDefinitions �� $	��111 ��� $��45 ��� .+��� $��查询条件��   TabItemHotBackground#���   $��2��$	��Top=�	� -   $��
radio_info��$��检索信息��$
��Left=�$��	22,43,0,0q�$	��Top=�	� -   $��radio_id��$��病例编号��$
��Left=�$��	22,79,0,0q�$	��Top=��� -   $
��info��$
��Left=�$��24��$��
113,39,0,0q� �� TextWrapping$
 Wrap=�$�� ��$	��Top=�$	��200���� -   $��id��$
��Left=�$��24��$��
113,75,0,0q�$
 Wrap=�$�� ��$	��Top=�$	��200���� -    ��Name$ search��$��1��$
��Left=�$��64��$��	20,54,0,0q�$	��Top=�.��� $��24��$��54��.+��� $
��Left=�$��4,2,0,3q�$��16��$��Resources/search.ico��$��Center=�$��19���� $��检索��$��Right=�$��Center=�$��38��$��24���� -   $ select��$��1��$
��Left=�$��66��$��
120,54,0,0q�$	��Top=�.��� $��25��$��59��.+��� $
��Left=�$��3,3,0,4q�$��16��$��Resources/check.ico��$��Center=�$��18���� $��选定��$��0,0,1,1q�$��Right=�$��Bottom=�$��34��$��24���� -   $
 back��$��1��$
��Left=�$��66��$��
230,54,0,0q�$	��Top=�.��� $��24��$��59��.+��� $
��Left=�$��3,4,0,3q�$��18��$��Resources/home.ico��$��Center=�$��17���� $��返回��$��0,0,4,0q�$��Right=�$��Center=�$��34��$��24���� -   $��title��$
��Left=�$��24��$��	10,10,0,0q�$
 Wrap=�$�� ��$	��Top=�$	��165��$��1���� -	   $
��save��$��保存修改��$
��Left=�$��
193,10,0,0q�$��1��$	��Top=�$��75��$��24���� -
   $��delete��$��删除��$
��Left=�$��
281,10,0,0q�$��1��$	��Top=�$��50��$��24���� -   $��lv��$��2��$��0,0,0,10q� �� View �� .P��� $��检索标题�� �� Width$	 250�� �� DisplayMemberBinding �� 	 �� Path$	 Title��� $��	近似度��$ 50�� �� $	 Approximate��� -   $��bz��$��1��$��1��$
��Wrap=�$�� ��$��2��$��
5,10,10,10q�  ButtonNormalBackground
 J� OpacityMask#
 �  #��� ��