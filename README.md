# 中医证治智能系统
## 更新日志
### 2016.03.31
- 问题1：在【证用病机规则管理】中证用病机规则录入成功添加多级复合病机后，刷新出现闪退.  
解决方案：由于在刷新中，多级复合病机字段错写，导致数据库读出出错，更改过来即可
- 问题2：TreeView没有清空导致上次查询记录依旧存在（问题存在于【病名规则管理】、【基本病机规则管理】、【复合病机规则管理】、【多级病机 管理】、【基本证名规则管理】以及【证用病机管理】）  
解决方案：先清空结点集，再调用创建树函数生成树，即可清空（需要在选定和刷新处更改）
- 问题3：在【基本处方信息管理】中的信息检索显示栏加入“备注”一列  
解决方案：ListView直接绑定备注信息即可
- 问题4：【基本证名信息管理】中信息检索证名分类选择与检索结果显示不符  
解决方案：由于增加了“全部”这个选项，导致检索结果错位，只需将检索值向后移一位即可

### 2016.04.04
- 更新【规则库管理】中，选择条件阈值和组内阈值修改时自动保存功能
- 更新【信息库管理】中，修改后自动更新目录，而不需要再点击搜索

### 2016.04.06
- 按照【基本病机信息管理】和【基本病机规则管理】布局来更新【系信息管理】和【系规则管理】，同时新建表`x_t_info_x`和表`x_t_rule_x`
- 修复部分搜索重复显示问题

### 2016.04.11
- 更新有关症象信息的数据库表`t_info_zxlx`、`t_info_zxmx`、`t_info_zxxx`。注意：更新完表之后，凡涉及到症象信息树显示，都需要微调整，即不能将`t_info_zxmx`中的`id`单独作为树的`ID`，而应该把`id`和`zxbh`放在一起作为树的`ID`，避免与一级树ID重合。

### 2016.04.15
- 解决【病名信息管理】中添加和删除不规范问题
- 解决规则库管理中个别出现添加闪退、阈值选择闪退问题以及删除问题
- 解决【经典案例检索】中保存修改结果即时显示问题

### 2016.04.16
- 优化模拟诊断（提示框显示、主诉名称不显示、西医病名显示错位、病史显示错位、服法显示重复等问题）
- “反药物规则”名称修改为“药物取舍规则”

### 2016.04.22
- 解决规则库管理中，当调用没有录入规则的病名时，方法数不清空问题

### 2016.05.04
- 更新规则管理数据库，删除重复行
- 更新规则管理库，解决树中偶尔出现的病名重复显示问题

### 2016.05.13
- 更新系信息管理和入科信息管理

### 2016.05.16
- 解决信息管理库中进行删除操作后selectedindex未重新设置问题
- 更新基本证名规则管理，条件类型添加“多级复合病机”和“证用病机”

### 2016.05.29
- 更新病名信息录入(将系更改为科别)
- 更新复合病机规则管理(系添加不进去)
- 更新多级复合病机规则管理(系添加不进去)
- 解决系规则管理信息录入闪退问题

### 2016.05.30
- 更新数据库系推理过程(p_x_zs和p_x_zx)

### 2016.05.31
- 内伤/外感病名推理过程添加病名级别推理优先级(危、急、重、轻)
- 更新数据库内伤/外感病名推理过程(p_ns_jlbm、p_ns_ylbm和p_wg_bmtl)

### 2016.06.02
- 添加多级复合病机和证用病机推理子过程

### 2016.06.06
- 更新症象信息录入

## 附加说明
### 1.病例明细表`t_bl_mx`数据结构说明
`xxdllx=0`为病人输入信息   
`xxdllx=1`为诊断中间结果信息   
`xxdllx=2`为诊断输出信息  
`xxdllx=3`为诊断中间近似结果信息  

| xxdllx | xxxllx | xxbh |
| :--------: | :--------:| :-- |
| 0| 0| 主诉编号|
| 0| 1| 初期症象编号|
| 0| 2| 现症象编号|
| 0| 3| 主诉时间编号|
| 0| 4| 既往史编号|
| 0| 5| 西医病名编号|
| 0| 6| 主诉名称|
| 0| 7| 初期症象名称|
| 0| 8| 现症象名称|
| 0| 9| 主诉时间名称|
| 0| a| 既往史名称|
| 0| b| 西医诊断|
| 1| 0| 病名类型:0-外感 1-内伤|
| 1| 1| 内伤系编号(根据主诉推理得出(p_x_zs))|
| 1| 2| 内伤系编号(根据症象(主诉和现症象)推理得出(p_x_zx))|
| 1| 3| 基本病机编号|
| 1| 4| 外感病名编号|
| 1| 5| 甲类病名编号|
| 1| 6| 乙类病名编号|
| 1| 7| 复合病机编号|
| 1| 8| 基本证名编号|
| 1| b| 外感规则所成立的方法(针对外感类型)|
| 1| c| 多级复合病机编号|
| 1| d| 证用病机编号|
| 2| 0| 病名编号|
| 2| 2| 基本证名编号|
| 2| 4| 基本处方编号|
| 2| 5| 服法|
| 2| 6| 服药后的疗效|
| 3| 1| 基本病机编号|
| 3| 2| 外感病名编号|
| 3| 3| 甲类病名编号|
| 3| 4| 乙类病名编号|
| 3| 5| 复合病机编号|
| 3| 6| 基本证名编号|
| 3| 7| 多级复合病机编号|
| 3| 8| 证用病机编号|

### 2.编号规则
| 信息名称 | 起始编号 | 终止编号 | 编号格式 | 说明 |
| :-------- | :--------| :-- | :-- | :-- |
| 症象信息 | 01+01+001 | 01+60+999 | 01+##+### |症象编号，第三、四位组成的数字代表症象类型编号|
| 系信息 | 031 | 099 | 0## |系编号|
| 入科信息 | 031 | 099 | 0## |入科编号|
| 基本病机信息 | 04+0001 | 04+9999 | 04+#### |基本病机编号|
| 复合病机信息 | 05+0001 | 05+9999 | 05+#### |复合病机编号|
| 外感病名信息 | 06+0001 | 06+9999 | 06+#### |病名编号(外感)|
| 内伤病名信息 | 07+0001 | 07+9999 | 07+#### |病名编号(内伤)|
| 基本证名信息(外感) | 08+000001 | 08+999999 | 08+###### |基本证名编号(外感)|
| 基本证名信息(内伤) | 09+000001 | 09+999999 | 09+###### |基本证名编号(内伤)|
| 处方信息 | 12000001 | 12999999 | 12+###### |处方编号|
| 西医病名信息 | 14+01+001 | 14+19+999| 14+##+### |西医病名编号，第三、四位组成的数字代表西医病名类型编号|
| 药物信息 | 14000001 | 14999999 | 14+###### |药物编号|
| 证用病机信息  | 150001 | 159999 | 15+#### |证用病机编号(新增加)|
| 多级复合病机信息 | 160001 | 169999 | 16+#### |多级复合病机编号(新增加)|
### 3.诊断近似度
| 情形 | 诊断近似度 |
| :-------- | :--------|
| 基本病机层近似 | 9 |
| 复合病机层近似 | 6 |
| 基本证名层近似 | 7 |
| 进了外感，但没有病名或证名成立 | 5 |
| 病名层近似 | 4 |
| 病名不成立+基本证名近似 | 8(程序中更改) |
| 准确推理，且病名、证名对应有结果 | 0 |
| 准确推理，病名、证名不对应 | 2(程序中更改) |
| 准确推理，病名为甲类时，无对应证，但有病名、证名成立 | 2 |
| 内伤推理中，系的推理直接从症象中获取，且结果成立 | 1(保留) |
### 4.外感规则表`t_rule_wg`数据结构
| 字段 | 名称 | 类型 | 宽度 | 说明 |
| :-------- | :--------| :-- | :-- | :-- |
| id | 流水号 | numeric | 18,0 | 表内排序用的流水号，不允许空 |
| zxbh | 症象编号 | char | 7 | 症象的编号 |
| ff | 方法 | int | 4 | 判断外感成立的方法数 |
| blgz | 并列规则 | int | 4 | 每个方法下必须成立的几个规则 |
| tjzb | 条件组别 | int | 4 | 症象所属的组别，隶属于规则 |
| gzfz | 规则阀值 | int | 4 | 规则成立所需要成立的组别数 |
| znfz | 组内阀值 | int | 4 | 组成立所需要存在的症象数 |
| bz | 备注 | text | 16 | |
