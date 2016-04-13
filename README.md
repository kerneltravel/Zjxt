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
- 按照【基本病机信息管理】和【基本病机规则管理】布局来更新【系信息管理】和【系规则管理】，同时新建表x_t_info_x和表x_t_rule_x
- 修复部分搜索重复显示问题

### 2016.04.11
- 更新有关症象信息的数据库表`t_info_zxlx`、`t_info_zxmx`、`t_info_zxxx`。注意：更新完表之后，凡涉及到症象信息树显示，都需要微调整，即不能将`t_info_zxmx`中的`id`单独作为树的`ID`，而应该把`id`和`zxbh`放在一起作为树的`ID`，避免与一级树ID重合。