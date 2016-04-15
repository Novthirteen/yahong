
INSERT ACC_Permission VALUES ('ForceRelease','强制释放','OrderOperation')
insert into entityopt values('OverOrderRate','0.2','允许超需求下订单的比例,如:0.2','601');

alter table Item add Category1 varchar(50) null;
alter table Item add Category2 varchar(50) null;

/****** Object:  Table [dbo].[ItemType]    Script Date: 06/26/2013 18:44:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ItemType](
	[Code] [varchar](50) NOT NULL,
	[Name] [varchar](50) NULL,
	[ShortName] [varchar](50) NULL,
	[Level] [int] NOT NULL,
 CONSTRAINT [PK_ItemType] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


INSERT ACC_Permission VALUES ('Menu.MasterData.ItemType','物料类型','MasterData')
INSERT acc_menu VALUES ('Menu.MasterData.ItemType',1,'物料类型','~/Main.aspx?mid=MasterData.ItemType',1,'~/Images/Nav/ItemType.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.MasterData.ItemType','Menu.MasterData',2,434,1,getdate(),null,getdate(),null)


--begion tiansu 20130621 异常报表
insert into codemstr(code,codevalue,seq,isdefault,desc1) values('PermissionCategoryType','Setup',200,0,'配置');
GO
INSERT INTO ACC_PermissionCategory values('Report','报表','Setup');
GO
insert into ACC_Permission(PM_Code,PM_Desc,PM_CateCode) values('PercentPassRep','合格率报表','Report');
go

set IDENTITY_INSERT BatchJobDet on;
insert into BatchJobDet(Id,Name,Desc1,ServiceName) values(560,'PercentPassJob','Job of Percent Pass','PercentPassJob');
set IDENTITY_INSERT BatchJobDet off;
go

set IDENTITY_INSERT BatchTrigger on;
insert into [BatchTrigger](Id,Name,Desc1,JobId,NextFireTime,PrevFireTime,RepeatCount,Interval,IntervalType,TimesTriggered,Status) values(560,'PercentPassTrigger','Trigger of Percent Pass',560,GETDATE(),null,0,1,'Days',0,'Pause');
set IDENTITY_INSERT BatchTrigger off;
go

insert into entityopt values('PercentPassStartTime','08:00;08:00','合格率报表起始时间','600');
insert into entityopt values('PercentPassFlowCode','P-ASS1,P-ASS12,P-ASS2,P-ASS3,P-ASS4,P-ASS5,P-INJ1,P-INJ2,P-INJ3,P-INJ4,P-INJ5,P-PNT,P-SPT','合格率路线(半角逗号分隔)','610');

insert into entityopt values('WebAddress','sconit.yahong-mould.com','网址','620');

--insert into entityopt values('DebugMode','FALSE','调试模式','190');
--insert into entityopt values('DebugModeEmail','tiansu@yfgm.com.cn','调试模式Email账户','195');

alter table InspectResult add Disposition varchar(50) null;
go

CREATE TABLE [InspectComfirmResult](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InspComfirmResultNo] [varchar](50) NOT NULL,
	[InspResultId] [int] NULL,
	[InspDetId] [int] NOT NULL,
	[Disposition] [varchar](50) NULL,
	[QualifyQty] [numeric](18, 8) NOT NULL,
	[RejectQty] [numeric](18, 8) NOT NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [varchar](50) NULL,
	[CreateUserNm] [varchar](50) NULL,
 CONSTRAINT [PK_InspectComfirmResult] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
--end tiansu 20130621





with tr as
(
select d.Item as Rm, d.Uom as RmUom, ISNULL(d.OrderQty,0) as RmQty
from OrderMstr  m
left join OrderDet d on m.OrderNo = d.OrderNo
where m.Flow ='RM1-WIPINJ1' and m.Status in('submit','in-process')
and m.StartTime >'2012-5-1' and m.SubType ='Nml' 
union
select d.Item as Rm, d.Uom as RmUom, ISNULL(d.RecQty,0) as RmQty
from OrderMstr  m
left join OrderDet d on m.OrderNo = d.OrderNo
where m.Flow ='RM1-WIPINJ1' and m.Status in('complete','close')
and m.StartTime >'2012-5-1' and m.SubType ='Nml' 
)
select tr.Rm as 原材料,Item.Desc1 as 描述1,Item.Desc2 as 描述2,tr.RmUom as 单位,
SUM(RMQty) as 数量 into #temptr
from tr left join Item on Item.Code = tr.Rm
group by tr.Rm,Item.Desc1,Item.Desc2,tr.RmUom;

with wo as
(
select t.Item as Rm,t.Uom as RmUom,
ISNULL(d.OrderQty,0)*t.UnitQty as RMQty, 0 as RmScrapQty
from OrderMstr  m
left join OrderDet d on m.OrderNo = d.OrderNo
left join OrderLocTrans t on t.OrderDetId = d.Id
where m.Flow ='P-INJ1' and m.Status in('submit','in-process')
and m.StartTime >'2012-5-1' and m.SubType ='Nml' 
and t.IOType = 'Out'
union
select t.Item as Rm,t.Uom as RmUom,
ISNULL(d.RecQty,0)*t.UnitQty as RMQty,ISNULL(d.ScrapQty,0)*t.UnitQty as RmScrapQty
 from OrderMstr  m
left join OrderDet d on m.OrderNo = d.OrderNo
left join OrderLocTrans t on t.OrderDetId = d.Id
where m.Flow ='P-INJ1' and m.Status in('complete','close')
and m.StartTime >'2012-5-1' and m.SubType ='Nml' 
and t.IOType = 'Out'
)
select  wo.Rm as 原材料,Item.Desc1 as 描述1,Item.Desc2 as 描述2,wo.RmUom as 单位,
SUM(RMQty)+ SUM(RmScrapQty) as 数量 into #tempwo
from wo left join Item on Item.Code = wo.Rm
group by wo.Rm,Item.Desc1,Item.Desc2,wo.RmUom;

select ISNULL(a.原材料,b.原材料) as 原材料,ISNULL(a.描述1,b.描述1) as 描述1,
ISNULL(a.描述2,b.描述2) as 描述2,ISNULL(a.单位,b.单位) as 单位,ISNULL(a.数量,0) as 理论耗用,
ISNULL(b.数量,0) as 实际领用
from #temptr a
full join #tempwo b on a.原材料 = b.原材料



INSERT ACC_Permission VALUES ('Menu.Production.RMConsume','材料消耗','Production')
INSERT acc_menu VALUES ('Menu.Production.RMConsume',1,'材料消耗','~/Main.aspx?mid=Reports.RMConsume',1,'~/Images/Nav/RMConsume.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Production.RMConsume','Menu.Production.Info',3,245,1,getdate(),null,getdate(),null)




alter table location add IsMrp bit;
update location set IsMrp = 1 ;


-----------------------20120503-==============================
alter table ActBill add InvIOTime datetime;
alter table ActBill add RecTime datetime;

alter table PlanBill add InvIOTime datetime;
alter table PlanBill add RecTime datetime;

alter table billdet add InvIOTime datetime;
alter table billdet add RecTime datetime;
alter table billdet drop column TextField1;
alter table billdet drop column TextField2;
alter table billdet drop column TextField3;
alter table billdet drop column TextField4;
alter table billdet drop column NumField1;
alter table billdet drop column NumField2;
alter table billdet drop column NumField3;
alter table billdet drop column NumField4;
alter table billdet drop column DateField1;
alter table billdet drop column DateField2;

update a set a.RecTime = b.CreateDate,a.InvIOTime = b.CreateDate
from PlanBill a, ReceiptMstr b where a.RecNo = b.RecNo
and a.TransType ='PO' and a.RecTime is null

update a set a.RecTime = b.CreateDate
from PlanBill a, ReceiptMstr b where a.RecNo = b.RecNo
and a.TransType ='SO' and a.RecTime is null

update a set a.InvIOTime = b.CreateDate
from PlanBill a, IpMstr b where a.IpNo = b.IpNo
and a.TransType ='SO' and a.InvIOTime is null

---------------

update a set a.RecTime = b.CreateDate,a.InvIOTime = b.CreateDate
from ActBill a, ReceiptMstr b where a.RecNo = b.RecNo
and a.TransType ='PO' and a.RecTime is null 

update a set a.RecTime = b.CreateDate
from ActBill a, ReceiptMstr b where a.RecNo = b.RecNo
and a.TransType ='SO' and a.RecTime is null

update a set a.InvIOTime = b.CreateDate
from ActBill a, IpMstr b where a.IpNo = b.IpNo
and a.TransType ='SO' and a.InvIOTime is null

---------------------

update a set a.InvIOTime = b.InvIOTime,a.RecTime = b.RecTime
from BillDet a, ActBill b where a.TransId = b.Id
and a.InvIOTime is null

-------------------------------

alter table ActBill alter column InvIOTime datetime not null;
alter table ActBill alter column RecTime datetime not null;

alter table PlanBill alter column InvIOTime datetime not null;
alter table PlanBill alter column RecTime datetime not null;

alter table billdet alter column InvIOTime datetime not null;
alter table billdet alter column RecTime datetime not null;
-------------==========================================

--增加金额/单价的精度
--orderdet
alter table orderdet alter column UnitPrice decimal(18,12)
alter table orderdet alter column UnitPriceAfterDiscount decimal(18,12)
--PriceListDet
alter table PriceListDet alter column UnitPrice decimal(18,12)
--PlanBill
alter table PlanBill alter column UnitPrice decimal(18,12)
alter table PlanBill alter column ListPrice decimal(18,12)
alter table PlanBill alter column PlanAmount decimal(18,10)
alter table PlanBill alter column ActAmount decimal(18,10)
--ActBill
alter table ActBill alter column UnitPrice decimal(18,12)
alter table ActBill alter column ListPrice decimal(18,12)
alter table ActBill alter column BillAmount decimal(18,10)
alter table ActBill alter column BilledAmount decimal(18,10)
--BillDet
alter table BillDet alter column UnitPrice decimal(18,12)
alter table BillDet alter column ListPrice decimal(18,12)
alter table BillDet alter column Discount decimal(18,12)
alter table BillDet alter column Amount decimal(18,10)



INSERT ACC_Permission VALUES ('Menu.Production.WorkHours','工时报表','Production')
INSERT acc_menu VALUES ('Menu.Production.WorkHours',1,'工时报表','~/Main.aspx?mid=Reports.WorkHours',1,'~/Images/Nav/WorkHours.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Production.WorkHours','Menu.Production.Info',3,245,1,getdate(),null,getdate(),null)

update rm set rm.DateField1 = om.SettleTime from 
ReceiptMstr rm,ReceiptDet rd,OrderLocTrans lld,OrderDet od,OrderMstr om
where rd.RecNo = rm.RecNo and lld.Id = rd.OrderLocTransId and od.Id =lld.OrderDetId
and om.OrderNo = od.OrderNo


update rm set rm.DateField1 = om.SettleTime from 
IpMstr rm,IpDet rd,OrderLocTrans lld,OrderDet od,OrderMstr om
where rd.IpNo = rm.IpNo and lld.Id = rd.OrderLocTransId and od.Id =lld.OrderDetId
and om.OrderNo = od.OrderNo

insert codemstr values('Status','Confirm',75,0,'已复核')


INSERT ACC_Permission VALUES ('ConfirmBill','复核账单','BillOperation')
--------------------------

--供应商退货
select OrderDet.OrderNo as 订单号,OrderMstr.Flow as 路线,
OrderDet.Item as 物料号,OrderDet.RefItemCode as 参考物料号,Item.Desc1 as 描述1,Item.Desc2 as 描述2,
OrderDet.Uom as 单位,OrderDet.RecQty as 数量 ,OrderMstr.CreateDate as 时间
from OrderDet
left join OrderMstr on OrderMstr.OrderNo = OrderDet.OrderNo
left join Item on Item.Code = OrderDet.Item
where OrderMstr.SubType in('Adj','Rtn') and OrderMstr.Type='Procurement' and OrderDet.RecQty<0 
and OrderMstr.CreateDate>='2011-7-1' and OrderMstr.CreateDate<='2011-7-30'
order by OrderMstr.Flow,OrderDet.Item
---------------------------

INSERT ACC_Permission VALUES ('Menu.Inventory.Container','周转箱报表','Inventory')
INSERT acc_menu VALUES ('Menu.Inventory.Container',1,'周转箱报表','~/Main.aspx?mid=Reports.Container',1,'~/Images/Nav/Container.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Inventory.Container','Menu.Inventory.Info',3,330,1,getdate(),null,getdate(),null)

alter   table   dbo.BillTrans   alter   column   ExtRecNo   varchar(255)
alter   table  dbo.PlanBill   alter   column   ExtRecNo   varchar(255)
alter   table   dbo.ActBill  alter   column   ExtRecNo   varchar(255)

declare @dt datetime
set @dt=getdate()
select c.Party as '客户',item as '物料',currency as '货币',
a.uom as '单位',isprovest as '暂估',unitprice as '单价',startdate as '开始时间', enddate as '结束时间'
from (
select rowid=row_number() over(partition by Item order by datediff(day,(case when StartDate is null then '1800-06-01 00:00:00' else StartDate end),@dt)),
datedif=datediff(day,(case when StartDate is null then '1800-06-01 00:00:00' else StartDate end),@dt),* from [PriceListDet]
where @dt between (case when StartDate is null then '1800-06-01 00:00:00' else StartDate end) and (case when EndDate is null then '9999-01-01 00:00:00' else EndDate end )) a
left join pricelistmstr c on a.PriceList = c.Code
left join item b on a.item = b.code
where rowid=1 and c.type='sales'
order by c.Party


insert into sqlreport values('
declare @dt datetime
set @dt=getdate()
select c.Party as 客户,item as 物料,currency as 货币,
a.uom as 单位,isprovest as 暂估,unitprice as 单价,startdate as 开始时间, enddate as 结束时间
from (
select rowid=row_number() over(partition by Item order by datediff(day,(case when StartDate is null then ''1800-06-01 00:00:00'' else StartDate end),@dt)),
datedif=datediff(day,(case when StartDate is null then ''1800-06-01 00:00:00'' else StartDate end),@dt),* from [PriceListDet]
where @dt between (case when StartDate is null then ''1800-06-01 00:00:00'' else StartDate end) and (case when EndDate is null then ''9999-01-01 00:00:00'' else EndDate end )) a
left join pricelistmstr c on a.PriceList = c.Code
left join item b on a.item = b.code
where rowid=1 and c.type=''sales''
order by c.Party
',
'查询销售价格单,无参数',70)


INSERT ACC_Permission VALUES ('Page_ExportAsn','导出ASN','OrderOperation')

select loctrans.item as 物料,item.desc1 as 描述1,item.desc2 as 描述2,
loctrans.uom as 单位,loctrans.loc as 库位,routingdet.runtime as 单位耗时分钟,sum(qty) as 总入库数,
routingdet.runtime * sum(qty)/60 as 总耗时小时 from loctrans
left join routingdet on routingdet.routing= loctrans.item
left join item on item.code = loctrans.item
left join location on location.code = loctrans.loc
where transtype ='RCT-WO' and location.type='NML'
and loctrans.CreateDate >'2011-08-01' and loctrans.CreateDate <'2011-11-30'
group by loctrans.item,item.desc1,item.desc2,loctrans.uom,loctrans.loc,routingdet.runtime
order by loctrans.loc


select orderdet.item as 物料代码,item.desc1 as 描述1,item.desc2 as 描述2,orderdet.Uom as 单位,
cast(sum(scrapqty) as numeric(12,2)) as 数量 from orderdet
left join ordermstr on orderdet.orderno = ordermstr.orderno
left join item on item.code = orderdet.item
where ordermstr.subtype='NML' and scrapqty>0 and ordermstr.type='production'
and ordermstr.createdate>='2011-10-9' and ordermstr.createdate<'2011-10-11'
group by orderdet.item,item.desc1,item.desc2,orderdet.Uom


INSERT ACC_Permission VALUES ('Menu.Production.ReuseScrap','原材料回用报废报表','Production')
INSERT acc_menu VALUES ('Menu.Production.ReuseScrap',1,'原材料回用报废报表','~/Main.aspx?mid=Reports.ReuseScrap',1,'~/Images/Nav/ReuseScrap.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Production.ReuseScrap','Menu.Production.Info',3,243,1,getdate(),null,getdate(),null)


INSERT ACC_Permission VALUES ('Menu.Production.Reuse','原材料回用报表','Production')
INSERT acc_menu VALUES ('Menu.Production.Reuse',1,'原材料回用报表','~/Main.aspx?mid=Reports.Reuse',1,'~/Images/Nav/Reuse.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Production.Reuse','Menu.Production.Info',3,240,1,getdate(),null,getdate(),null)


update acc_menucommon set level_='3' where  menu='Menu.Production.Scrap'
update acc_permission set pm_desc='原材料报废报表' where pm_code='Menu.Production.Scrap'
update acc_menu set desc_='原材料报废报表' where code='Menu.Production.Scrap'

alter table CustScheduleMstr add FinanceCalendar varchar(50);


INSERT ACC_Permission VALUES ('Menu.SupplierMenu.SupplierSchedule1','供应商月度计划','SupplierMenu')
INSERT acc_menu VALUES ('Menu.SupplierMenu.SupplierSchedule1',1,'供应商月度计划','~/Main.aspx?mid=MRP.Schedule.SupplierSchedule',1,'~/Images/Nav/SupplierSchedule.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.SupplierMenu.SupplierSchedule1','Menu.SupplierMenu',2,239,1,getdate(),null,getdate(),null)

alter table CostDet add TotalAmount decimal(18, 8);
alter table CostDet add TotalQty decimal(18, 8);

insert into sqlreport values('
select bomlevel-1 as 层级,  bomtree.bom as 父半成品代码,bomtree.BomDesc as 父半成品描述,FGUom as 父半成品单位,bomtree.item as 子半成品,
bomtree.itemdesc as 子半成品描述,cast( rateqty as numeric(12,2)) as 用量,bomtree.uom as 单位
from bomtree
left join item i1 on i1.code = bomtree.bom
where i1.category like ''2%'' and bomtree.itemcategorycode like ''2%''  
',
'查询Bom用量,半成品展开到半成品,无参数',70)

insert into sqlreport values('
select bomlevel as 层级, fg as 成品代码,fgdesc as 成品描述,FGUom as 成品单位,item as 原材料,itemdesc as 原材料描述,
cast( accumqty as numeric(12,2)) as 用量,bomtree.uom as 单位,itemcategorydesc as 产品类
 from bomtree where itemcategorycode like ''1%'' 
',
'查询Bom用量,展开到最底层,无参数',50)


insert into sqlreport values('
select bomlevel as 层级 ,fg as 成品代码,fgdesc as 成品描述,FGUom as 成品单位,item as 半成品,itemdesc as 半成品描述,
cast( accumqty as numeric(12,2)) as 用量,bomtree.uom as 单位,itemcategorydesc as 产品类
 from bomtree where itemcategorycode like ''2%'' 
',
'查询Bom用量,展开到半成品,无参数',60)



GO
/****** 对象:  Table [dbo].[BomTree]    脚本日期: 08/24/2011 14:32:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BomTree](
	[Bom] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[BomDesc] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Item] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[ItemDesc] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[RateQty] [decimal](18, 8) NULL,
	[UomCode] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[BomLevel] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF

alter table costtrans add AdjType varchar(50);

insert into sqlreport values('
--Bom
select bom, item, RateQty,uom into #temp_det 
from bomdet
where (enddate is null or enddate > getdate()) and startdate < getdate() 
select distinct code into #temp_bom from bommstr where isactive = 1
------------------------------------------------
--select b.code from #temp_det as d inner join #temp_bom as b on d.item = b.code
select * into #temp_semibom from #temp_det as a where a.bom in 
(select b.code from #temp_det as d inner join #temp_bom as b on d.item = b.code)
select d.bom, s.item, d.RateQty * s.RateQty as RateQty,s.UOM as uom into #temp_semidet1 
from #temp_det as d inner join #temp_semibom as s
on d.item = s.bom
------------------------------------------------
--select b.code from  #temp_semidet1 as d inner join #temp_bom as b on d.item = b.code
select * into #temp_semibom2 from #temp_det as a where a.bom in 
(select b.code from #temp_semidet1 as d inner join #temp_bom as b on d.item = b.code)
select d.bom, s.item, d.RateQty * s.RateQty as RateQty,s.UOM as uom into #temp_semidet2 
from #temp_det as d inner join #temp_semibom2 as s
on d.item = s.bom
------------------------------------------------
delete from #temp_det where #temp_det.item in (select b.code from #temp_det as d inner join #temp_bom as b on d.item = b.code)
delete from #temp_semidet1 where #temp_semidet1.item in (select b.code from #temp_semidet1 as d inner join #temp_bom as b on d.item = b.code)
delete from #temp_semidet2 where #temp_semidet2.item in (select b.code from #temp_semidet2 as d inner join #temp_bom as b on d.item = b.code)
insert into  #temp_det select * from #temp_semidet1
insert into  #temp_det select * from #temp_semidet2
select t.bom as BOM,i1.desc1 as 描述1,i1.desc2 as 描述2,t.item as 物料,i2.desc1 as 物料描述1,i2.desc2 as 物料描述2
,t.rateqty as 用量,t.uom as 单位
from #temp_det t left join item i1 on i1.code = t.bom
left join item i2 on i2.code = t.item
order by t.bom 
',
'查询Bom用量,展开到最底层,无参数',40)



--OrderDetView
SELECT     MAX(dbo.OrderDet.Id) AS Id, dbo.OrderMstr.Flow, dbo.FlowMstr.Desc1, dbo.OrderMstr.Type, dbo.OrderMstr.PartyFrom, dbo.OrderMstr.PartyTo, 
                      CONVERT(datetime, CONVERT(varchar(8), dbo.OrderMstr.StartTime, 112)) AS EffDate, dbo.OrderMstr.Shift, dbo.OrderDet.Item, dbo.OrderDet.Uom, 
                      SUM(dbo.OrderDet.ReqQty) AS ReqQty, SUM(dbo.OrderDet.OrderQty) AS OrderQty, ISNULL(SUM(dbo.OrderDet.ShipQty), 0) AS ShipQty, 
                      ISNULL(SUM(dbo.OrderDet.RecQty), 0) AS RecQty, ISNULL(SUM(dbo.OrderDet.RejQty), 0) AS RejQty, ISNULL(SUM(dbo.OrderDet.ScrapQty), 0) 
                      AS ScrapQty, dbo.OrderMstr.Status, dbo.OrderDet.NumField1, dbo.OrderMstr.SubType
FROM         dbo.OrderDet INNER JOIN
                      dbo.OrderMstr ON dbo.OrderDet.OrderNo = dbo.OrderMstr.OrderNo INNER JOIN
                      dbo.FlowMstr ON dbo.OrderMstr.Flow = dbo.FlowMstr.Code
GROUP BY dbo.OrderMstr.Flow, dbo.FlowMstr.Desc1, dbo.OrderMstr.Type, dbo.OrderMstr.PartyFrom, dbo.OrderMstr.PartyTo, CONVERT(varchar(8), 
                      dbo.OrderMstr.StartTime, 112), dbo.OrderMstr.Shift, dbo.OrderDet.Item, dbo.OrderDet.Uom, dbo.OrderMstr.Status, dbo.OrderDet.NumField1, 
                      dbo.OrderMstr.SubType

--begin tiansu 20110818 状态中文化

update CodeMstr set desc1='取消'   where Code='Status' and  codevalue='Cancel';
update CodeMstr set desc1='关闭'   where Code='Status' and  codevalue='Close';
update CodeMstr set desc1='完成'   where Code='Status' and  codevalue='Complete';
update CodeMstr set desc1='创建'   where Code='Status' and  codevalue='Create';
update CodeMstr set desc1='执行中' where Code='Status' and  codevalue='In-Process';
update CodeMstr set desc1='暂停'   where Code='Status' and  codevalue='Pause';
update CodeMstr set desc1='提交'   where Code='Status' and  codevalue='Submit';
update CodeMstr set desc1='作废'   where Code='Status' and  codevalue='Void';

--end tiansu 20110818




INSERT ACC_Permission VALUES ('Menu.Inventory.InvKanBan','库存看板','Inventory')
INSERT acc_menu VALUES ('Menu.Inventory.InvKanBan',1,'库存看板','~/Main.aspx?mid=Reports.InvKanBan',1,'~/Images/Nav/InvKanBan.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Inventory.InvKanBan','Menu.Inventory.Info',3,325,1,getdate(),null,getdate(),null)


update entityopt set prevalue ='-1',codedesc='允许过量发货和收货,-1不做控制,正数控制容差范围' where precode = 'AllowExceedGiGR'
delete from codemstr where code like '%gi%'


alter table FlowBinding add InTrans bit;
alter table OrderBinding add InTrans bit;

insert codemstr values('FlowStrategy','WO',70,0,'WO')

insert into sqlreport values('
with odlts as
(
select  olt.Item as item, olt.Uom as uom,
sum(od.OrderQty - (case when od.RecQty is null then 0 else od.RecQty end)) as recqty
from OrderLocTrans olt left join orderdet od on olt.orderdetid = od.id
left join ordermstr oh on oh.orderno = od.orderno
where oh.Status in ('submit', 'In-Process') and oh.SubType = 'Nml' and not oh.Type = 'Distribution'
 and olt.IOType = 'In' and olt.Loc = @p0
group by olt.Item,olt.Uom
),
stock as (
select flowdet.item,isnull( flowdet.locto, flowmstr.locto) as location,safestock,maxstock from flowdet 
left join flowmstr on flowdet.flow = flowmstr.code
where flowdet.locto is not null or flowmstr.locto is not null
)
select stock.location as 库位,LocationDet.item as 物料,item.desc1 as 描述1,item.desc2 as 描述2, Item.Uom as 单位,
cast(LocationDet.qty as numeric(12,2)) as 当前库存,
cast(case when stock.safestock is null then 0 else stock.safestock end as numeric(12,2)) as 安全库存,
cast(case when stock.maxstock  is null then 0 else stock.maxstock end as numeric(12,2)) as 最大库存,
cast(case when odlts.recqty is null then 0 else odlts.recqty end as numeric(12,2)) as 待收,
cast(LocationDet.qty-stock.safestock + 
case when odlts.recqty is null then 0 else odlts.recqty end as numeric(12,2)) as 安全差额,
cast(LocationDet.qty-stock.maxstock + 
case when odlts.recqty is null then 0 else odlts.recqty end as numeric(12,2)) as 最大差额,
case when odlts.uom is null then Item.Uom else odlts.uom end as 订单单位,
case when LocationDet.qty-stock.safestock+
case when odlts.recqty is null then 0 else odlts.recqty end>0 then 0 else LocationDet.qty-stock.safestock+
case when odlts.recqty is null then 0 else odlts.recqty end end as rankid
from LocationDet left join stock
on LocationDet.location =stock.location
left join item on item.code = LocationDet.item
left join odlts on odlts.item = LocationDet.item
where locationdet.item = stock.item and
stock.safestock is not null and locationdet.location = @p0
and (LocationDet.qty>0 or case when odlts.recqty is null then 0 else odlts.recqty end>0)
order by rankid,最大差额
',
'查询3PL当前库存,安全库存,最大库存,差额.参数1:库位',30)


INSERT ACC_Permission VALUES ('Menu.Cost.BillPO','采购成本报表','Cost')
INSERT acc_menu VALUES ('Menu.Cost.BillPO',1,'采购成本报表','~/Main.aspx?mid=Cost.Report.Bill__mp--ModuleType-PO',1,'~/Images/Nav/BillPO.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Cost.BillPO','Menu.Cost.Info',3,25,1,getdate(),null,getdate(),null)

INSERT ACC_Permission VALUES ('Menu.Cost.BillSO','销售成本报表','Cost')
INSERT acc_menu VALUES ('Menu.Cost.BillSO',1,'销售成本报表','~/Main.aspx?mid=Cost.Report.Bill__mp--ModuleType-SO',1,'~/Images/Nav/BillSO.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Cost.BillSO','Menu.Cost.Info',3,26,1,getdate(),null,getdate(),null)

INSERT ACC_Permission VALUES ('Menu.Cost.Profit','销售毛利报表','Cost')
INSERT acc_menu VALUES ('Menu.Cost.Profit',1,'销售毛利报表','~/Main.aspx?mid=Cost.Report.Profit',1,'~/Images/Nav/Profit.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Cost.Profit','Menu.Cost.Info',3,27,1,getdate(),null,getdate(),null)

update acc_menu set pageurl='~/Main.aspx?mid=MRP.Schedule.MPS' where code ='Menu.MRP.MPS'
INSERT ACC_MenuCommon VALUES ('Menu.MRP.MPS','Menu.MRP.Info',3,540,1,getdate(),null,getdate(),null)

INSERT ACC_Permission VALUES ('Menu.Distribution.PrintASN','送货单打印','Distribution')
INSERT acc_menu VALUES ('Menu.Distribution.PrintASN',1,'送货单打印','~/Main.aspx?mid=Warehouse.PrintASN',1,'~/Images/Nav/PrintASN.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Distribution.PrintASN','Menu.Distribution.Trans',3,540,1,getdate(),null,getdate(),null)

update acc_permission set pm_code='Menu.MasterData.ItemCategory',pm_desc='产品类', pm_catecode='MasterData' where pm_code='Menu.Application.Check'


INSERT ACC_Permission VALUES ('Menu.Cost.InvIOB','进销存报表','Cost')
INSERT acc_menu VALUES ('Menu.Cost.InvIOB',1,'进销存报表','~/Main.aspx?mid=Cost.Report.InvIOB',1,'~/Images/Nav/InvIOB.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Cost.InvIOB','Menu.Cost.Info',3,25,1,getdate(),null,getdate(),null)


alter table FlowMstr add IsMRP bit;

--没有价格单的物流明细
insert into sqlreport values('
select  flowdet.flow as 路线,flowmstr.desc1 as 路线描述, 
case when flowmstr.type=''Procurement'' then ''采购''  when flowmstr.type=''Distribution'' then ''销售''  when flowmstr.type=''Subconctracting'' then ''委外'' end  as 类型,
item.code as 物料代码, item.desc1 as 描述1,item.desc2 as 描述2,flowdet.Uom as 单位,cast(flowdet.uc as numeric(12,2)) as 单包装 from flowdet 
left join flowmstr on flowdet.flow = flowmstr.code
left join pricelistdet on flowmstr.pricelist = pricelistdet.pricelist and flowdet.item = pricelistdet.item 
left join item on item.code = flowdet.item
where (flowmstr.type =''Procurement''or flowmstr.type=''Distribution''or flowmstr.type=''Subconctracting'') 
and  pricelistdet.id is null order by flowmstr.type',
'查询所有没有价格单的销售,采购,委外路线.无参数',10)

---没有工艺流程的生产路线,委外生产线除外.无参数
insert into sqlreport values('
select  flowdet.flow as 路线,flowmstr.desc1 as 路线描述, 
item.code as 物料代码, item.desc1 as 描述1,item.desc2 as 描述2,flowdet.Uom as 单位,cast(flowdet.uc as numeric(12,2)) as 单包装 from flowdet 
left join flowmstr on flowdet.flow = flowmstr.code
left join RoutingDet on flowdet.Routing = RoutingDet.Routing
left join item on item.code = flowdet.item
where flowmstr.type =''Production''and  RoutingDet.id is null 
and flowmstr.code not in (select refFlow.refflow from flowmstr refFlow where refFlow.type=''Subconctracting'')
order by flowmstr.Code',
'没有工艺流程的生产路线,委外生产线除外.无参数',20)

update acc_permission set pm_desc='成本分摊事务' where pm_code='Menu.Cost.CostAllocateTrans'
INSERT ACC_Permission VALUES ('Menu.Cost.CostAllocateMethod','分摊方法','Cost')
--INSERT ACC_Permission VALUES ('Menu.Cost.CostAllocateTrans','成本分摊事务','Cost')
INSERT ACC_Permission VALUES ('Menu.Cost.CostElement','成本要素','Cost')
INSERT ACC_Permission VALUES ('Menu.Cost.CostGroup','成本单元','Cost')
INSERT ACC_Permission VALUES ('Menu.Cost.ExpenseElement','费用要素','Cost')
INSERT ACC_Permission VALUES ('Menu.Cost.FinanceCalendar','会计期间','Cost')
--INSERT ACC_Permission VALUES ('Menu.Cost.FinanceClose','月结','Cost')
INSERT ACC_Permission VALUES ('Menu.Cost.GrossProfitReport','毛利报表','Cost')
INSERT ACC_Permission VALUES ('Menu.Cost.StandardCost','标准成本','Cost')


INSERT ACC_Permission VALUES ('Page_CustomReport','自定义报表','MasterDataOperation')



INSERT ACC_Permission VALUES ('Menu.Application.Console','报表查询','Application')
INSERT acc_menu VALUES ('Menu.Application.Console',1,'报表查询','~/Main.aspx?mid=ManageSconit.Console',1,'~/Images/Nav/Console.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Application.Console','Menu.Application',2,240,1,getdate(),null,getdate(),null)

INSERT ACC_Permissioncategory VALUES ('Cost','成本管理','Menu')

INSERT ACC_Permission VALUES ('Menu.Cost.CostAllocateTrans','费用','Cost')
INSERT acc_menu VALUES ('Menu.Cost.CostAllocateTrans',1,'费用','~/Main.aspx?mid=Cost.CostAllocateTrans',1,'~/Images/Nav/CostAllocateTrans.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Cost.CostAllocateTrans','Menu.Cost.Trans',3,23,1,getdate(),null,getdate(),null)


INSERT ACC_Permission VALUES ('Menu.Cost.FinanceClose','月结','Cost')
INSERT acc_menu VALUES ('Menu.Cost.FinanceClose',1,'月结','~/Main.aspx?mid=Cost.FinaceClose',1,'~/Images/Nav/CostAllocateTrans.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Cost.FinanceClose','Menu.Cost.Trans',3,22,1,getdate(),null,getdate(),null)


GO

SELECT     dbo.OrderLocTrans.Id, dbo.OrderMstr.OrderNo, dbo.OrderMstr.Type, dbo.OrderMstr.Flow, dbo.OrderMstr.Status, dbo.OrderMstr.StartTime, 
                      dbo.OrderMstr.WindowTime, dbo.OrderMstr.PartyFrom, dbo.OrderMstr.PartyTo, dbo.OrderLocTrans.Loc, dbo.OrderDet.Item AS ItemCode, 
                      dbo.Item.Desc1 + dbo.Item.Desc2 AS ItemDesc, dbo.OrderDet.Uom, dbo.OrderDet.ReqQty, dbo.OrderDet.OrderQty, ISNULL(dbo.OrderDet.ShipQty, 0) 
                      AS ShipQty, ISNULL(dbo.OrderDet.RecQty, 0) AS RecQty, dbo.OrderLocTrans.Item, dbo.OrderLocTrans.IOType, dbo.OrderLocTrans.UnitQty, 
                      dbo.OrderLocTrans.OrderQty AS PlanQty, ISNULL(dbo.OrderLocTrans.AccumQty, 0) AS AccumQty, ISNULL(dbo.OrderDet.RejQty, 0) AS RejQty, 
                      ISNULL(dbo.OrderDet.ScrapQty, 0) AS ScrapQty
FROM         dbo.OrderDet INNER JOIN
                      dbo.OrderMstr ON dbo.OrderDet.OrderNo = dbo.OrderMstr.OrderNo INNER JOIN
                      dbo.OrderLocTrans ON dbo.OrderDet.Id = dbo.OrderLocTrans.OrderDetId INNER JOIN
                      dbo.Item ON dbo.OrderDet.Item = dbo.Item.Code


/****** 对象:  Table [dbo].[SqlReport]    脚本日期: 07/22/2011 23:09:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SqlReport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Sql] [varchar](max) NOT NULL,
	[Desc1] [varchar](255) NULL,
	[Seq] [int] NOT NULL,
 CONSTRAINT [PK_SqlReport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF

update acc_menu set pageurl='~/Main.aspx?mid=Inventory.MiscOrder__mp--ModuleType-Gi'
where  pageurl='~/Main.aspx?mid=Inventory.MiscOrder.__mp--ModuleType-Gi'

update acc_menu set pageurl='~/Main.aspx?mid=Inventory.MiscOrder__mp--ModuleType-Gr'
where  pageurl='~/Main.aspx?mid=Inventory.MiscOrder.__mp--ModuleType-Gr'

INSERT ACC_Permission VALUES ('CancelMiscOrder','取消报废单','OrderOperation')
INSERT ACC_Permission VALUES ('ConfirmMiscOrder','确认报废单','OrderOperation')

alter table MiscOrderMstr add CostGroup varchar(50);
alter table MiscOrderMstr add CostElement varchar(50);

INSERT ACC_Permissioncategory VALUES ('Cost','成本管理','Menu')

INSERT ACC_Permission VALUES ('Menu.Cost.AjustValue','库存价值调整','Cost')
INSERT acc_menu VALUES ('Menu.Cost.AjustValue',1,'库存价值调整','~/Main.aspx?mid=Cost.MiscOrder',1,'~/Images/Nav/MiscOrder.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Cost.AjustValue','Menu.Cost.Trans',3,24,1,getdate(),null,getdate(),null)

update acc_menu set pageUrl ='~/Main.aspx?mid=Cost.InvAdjust' where pageurl='~/Main.aspx?mid=Cost.MiscOrder'


alter table MiscOrderMstr add Status varchar(50);
alter table MiscOrderDet add Cost decimal;

insert  codemstr values('MrpOpt','PlanMinusOrder','50','0','计划减订单')
insert  codemstr values('MrpOpt','PlanAddOrder','60','0','计划加订单')
insert  codemstr values('MrpOpt','SafeStockOnly','70','0','只看安全库存')



INSERT ACC_Permission VALUES ('Menu.Production.Scrap','原材料报废报表','Production')
INSERT acc_menu VALUES ('Menu.Production.Scrap',1,'原材料报废报表','~/Main.aspx?mid=Reports.Scrap',1,'~/Images/Nav/Scrap.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Production.Scrap','Menu.Production.Info',3,239,1,getdate(),null,getdate(),null)




update codemstr set desc1='返工(回用)' where desc1='返工'

update codemstr set desc1='调整(报废)' where desc1='调整'

USE [LPP_Yahong]
GO
/****** 对象:  Table [dbo].[MRP_PlanTrans]    脚本日期: 06/30/2011 21:41:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MRP_PlanTrans](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Flow] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[FlowType] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Item] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[Qty] [decimal](18, 8) NOT NULL,
	[Uom] [varchar](5) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[UC] [decimal](18, 8) NOT NULL,
	[StrartTime] [datetime] NOT NULL,
	[WindowTime] [datetime] NOT NULL,
	[Location] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[SourceType] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[PeriodType] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[EffDate] [datetime] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[Ref] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [PK_MRP_PlanTrans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF

alter table orderdet add Routing varchar(50);
alter table orderdet add ReturnRouting varchar(50);

insert into entityopt values('NoPriceListCreate', 'False', '没有价格单进行订单创建', 501);
insert into entityopt values('NoRoutingCreate', 'False', '没有工艺流程进行生产单收货', 502);
insert into entityopt values('EnableMailToSupplier', 'False', '是否发送供应商邮件通知', 1);


alter table Item add IsFreeze bit;
alter table Location add IsFreeze bit;
update item set IsFreeze = 0
update Location set IsFreeze = 0

alter table ipmstr add Flow varchar(50);
alter table receiptmstr add Flow varchar(50);
alter table ipmstr add ExtOrderNo varchar(255);

--ALTER TABLE Item ALTER COLUMN IsFreeze ADD bit NOT NULL;
--ALTER TABLE Item ALTER COLUMN IsFreeze SET NOT NULL;


update acc_menu set pageurl=
'~/Main.aspx?mid=Order.ReceiptNotes__mp--ModuleType-Procurement_IsSupplier-true'
 where pageurl=
'~/Main.aspx?mid=Order.ReceiptNotes__mp--ModuleType-Distribution_IsSupplier-true'

USE [LPP_Yahong]
GO
/****** 对象:  Table [dbo].[MRP_ScheduleHead]    脚本日期: 06/25/2011 18:39:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MRP_ScheduleHead](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleNo] [varchar](50) NOT NULL,
	[DateFrom] [datetime] NOT NULL,
	[DateTo] [datetime] NOT NULL,
	[PeriodType] [varchar](50) NOT NULL,
 CONSTRAINT [PK_MRP_ScheduleHead] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF


USE [LPP_Yahong]
GO
/****** 对象:  Table [dbo].[MRP_ScheduleDet]    脚本日期: 06/25/2011 18:38:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MRP_ScheduleDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleBodyId] [int] NOT NULL,
	[SourceType] [varchar](50) NOT NULL,
	[PeriodType] [varchar](50) NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[WinTime] [datetime] NOT NULL,
	[Qty] [decimal](18, 8) NOT NULL,
	[OrderNo] [varchar](50) NOT NULL,
	[IdRef] [int] NOT NULL,
 CONSTRAINT [PK_MRP_ScheduleDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF


GO
/****** 对象:  Table [dbo].[MRP_ScheduleBody]    脚本日期: 06/25/2011 18:38:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MRP_ScheduleBody](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleHeadId] [int] NOT NULL,
	[Item] [varchar](50) NOT NULL,
	[ItemDesc] [varchar](255) NULL,
	[ItemRef] [varchar](255) NULL,
	[Uom] [varchar](50) NOT NULL,
	[UC] [decimal](18, 8) NOT NULL,
	[Qty] [decimal](18, 8) NOT NULL,
	[ActQty] [decimal](18, 8) NOT NULL,
	[DisconActQty0] [decimal](18, 8) NOT NULL,
 CONSTRAINT [PK_MRP_ScheduleBody] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF

GO
/****** 对象:  Table [dbo].[MRP_Schedule]    脚本日期: 06/25/2011 18:38:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MRP_Schedule](
	[ScheduleNo] [varchar](50) NOT NULL,
	[FlowCode] [varchar](50) NULL,
	[FlowDesc] [varchar](255) NULL,
	[Location] [varchar](50) NULL,
	[FlowType] [varchar](50) NULL,
	[PartyCode] [varchar](50) NULL,
	[PartyDesc] [varchar](50) NULL,
 CONSTRAINT [PK_MRP_Schedule] PRIMARY KEY CLUSTERED 
(
	[ScheduleNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF

GO
/****** 对象:  Table [dbo].[MRP_LocLotDet]    脚本日期: 06/25/2011 18:37:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MRP_LocLotDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Location] [varchar](50) NOT NULL,
	[Item] [varchar](50) NOT NULL,
	[SafeQty] [decimal](18, 8) NOT NULL,
	[Qty] [decimal](18, 8) NOT NULL,
	[EffDate] [datetime] NOT NULL,
 CONSTRAINT [PK_MRP_LocationLotDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF

INSERT into acc_permission values('Menu.MRP.DmdScheduleRouting','需求日程视图','MRP')
INSERT acc_menu VALUES ('Menu.MRP.DmdScheduleRouting',1,'需求日程视图','~/Main.aspx?mid=MRP.Schedule.DmdScheduleRouting',1,'~/Images/Nav/DmdScheduleRouting.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.MRP.DmdScheduleRouting','Menu.MRP.Info',3,241,1,getdate(),null,getdate(),null)

-----------------------------------------------

update acc_menu set pageurl='~/Main.aspx?mid=Order.OrderHead.Procurement__mp--ModuleType-Procurement_ModuleSubType-Nml_StatusGroupId-6_IsSupplier-true'
where code='Menu.SupplierMenu.ViewProcurementOrder'

--liqiuyun 2011-5-18
INSERT ACC_Permission VALUES ('Menu.SupplierMenu.SupplierSchedule','供应商日程','SupplierMenu')
INSERT acc_menu VALUES ('Menu.SupplierMenu.SupplierSchedule',1,'供应商日程','~/Main.aspx?mid=MRP.Schedule.DmdSchedule__mp--IsSupplier-true',1,'~/Images/Nav/SupplierSchedule.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.SupplierMenu.SupplierSchedule','Menu.SupplierMenu',2,239,1,getdate(),null,getdate(),null)

INSERT ACC_Permission VALUES ('Menu.SupplierMenu.ActingBill','采购未开票','SupplierMenu')
INSERT acc_menu VALUES ('Menu.SupplierMenu.ActingBill',1,'采购未开票','~/Main.aspx?mid=Finance.ActingBill',1,'~/Images/Nav/SupplierSchedule.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.SupplierMenu.ActingBill','Menu.SupplierMenu',2,239,1,getdate(),null,getdate(),null)

INSERT ACC_Permission VALUES ('Menu.Application.ShowLog','查看日志','Application')
INSERT acc_menu VALUES ('Menu.Application.ShowLog',1,'查看日志','~/Main.aspx?mid=ManageSconit.ShowLog',1,'~/Images/Nav/ShowLog.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Application.ShowLog','Menu.Application',2,239,1,getdate(),null,getdate(),null)

INSERT ACC_Permission VALUES ('Menu.Application.Check','基础数据检查','Application')
INSERT acc_menu VALUES ('Menu.Application.Check',1,'基础数据检查','~/Main.aspx?mid=ManageSconit.Check',1,'~/Images/Nav/Check.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Application.Check','Menu.Application',2,240,1,getdate(),null,getdate(),null)



insert into acc_permission values('Menu.MRP.DmdSchedule','需求日程','MRP')
--acc_menu todo
--ACC_MenuCommon todo
insert into acc_permission values('Menu.MRP.CustomerScheduleReport','客户日程报表','MRP')
--acc_menu todo
--ACC_MenuCommon todo
insert into acc_permission values('Menu.MRP.CustomerSchedule','客户日程','MRP')
--acc_menu todo
--ACC_MenuCommon todo

insert  codemstr values('MrpOpt','OrderBeforePlan','10','1','定单优先')
insert  codemstr values('MrpOpt','OrderOnly','30','0','只看定单')
insert  codemstr values('MrpOpt','PlanOnly','20','0','只看计划')
insert  codemstr values('PhyCntRule','Amount','20','1','数量')
insert  codemstr values('PhyCntRule','Concentration','10','0','容积率(浓度)')
insert  codemstr values('PhyCntRule','Hu','30','0','条码')

--begin jienyuan 20110427
insert entityopt values('IsAllowCreateMultiFG','False','单张工单是否包含多个成品',0)
--end jienyuan 20110427

--begin jienyuan 20110425
insert into codemstr values('PhyCntRule','Concentration',10,1,'容积率(浓度)')
insert into codemstr values('PhyCntRule','Amount',20,0,'数量')
insert into codemstr values('PhyCntRule','Hu',30,0,'条码')
--end jienyuan 20110425

--begin jienyuan 20110418
insert entityopt values('IsIncludeRejectAndScrap','False','生产单完成原则',0)
--end jienyuan 20110418


--begin wangxiang 20110418 增加检验结果
GO
/****** 对象:  Table [dbo].[InspectResult]    脚本日期: 04/18/2011 10:17:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InspectResult](
	[QualifyQty] [numeric](18, 8) NOT NULL,
	[RejectQty] [numeric](18, 8) NOT NULL,
	[InspResultNo] [varchar](50) NOT NULL,
	[InspectDetId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Printno] [varchar](50) NULL,
	[printcount] [int] NULL,
	[isprinted] [bit] NULL,
	[lastmodifydate] [datetime] NULL,
	[lastmodifyuser] [varchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [varchar](50) NULL,
 CONSTRAINT [PK_InspectResult_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF


alter table inspectdet add PendingQualifyQty decimal(18, 8);
alter table inspectdet add PendingRejectQty decimal(18, 8);

INSERT ACC_Permission VALUES ('Menu.Quality.UnqualifiedGoods','不合格品打印','Production')
INSERT acc_menu VALUES ('Menu.Quality.UnqualifiedGoods',1,'不合格品打印','~/Main.aspx?mid=Inventory.UnqualifiedGoods',1,'',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Quality.UnqualifiedGoods','Menu.Quality',2,148,1,getdate(),null,getdate(),null)

INSERT ACC_Permission VALUES ('Menu.Quality.PendingInspectOrder','未确认报验单','Production')
INSERT acc_menu VALUES ('Menu.Quality.PendingInspectOrder',1,'未确认报验单','~/Main.aspx?mid=Inventory.InspectOrder.PendingInspectOrder',1,'',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Quality.PendingInspectOrder','Menu.Quality',2,150,1,getdate(),null,getdate(),null)


INSERT INTO "EntityOpt" (PreCode,PreValue,CodeDesc,Seq) VALUES ('NeedInspectConfirm','True','需要检验确认',100)
--end wangxiang

--begin jienyuan 20110411
alter table CycleCountMstr add IsPlotRatio Bit;
alter table StorageBin add Volume decimal(18,8) Null;
--end jienyuan 20110411

--begin tiansu 20110407 生产单排序按钮
INSERT ACC_Permission VALUES ('SeqWO','生产单排序','OrderOperation')
--end tiansu 20110407


--begin tiansu 20110406 增加 生产单排序 菜单
INSERT ACC_Permission VALUES ('Menu.Production.SeqWO','生产单排序','Production')
INSERT acc_menu VALUES ('Menu.Production.SeqWO',1,'生产单排序','~/Main.aspx?mid=Production.SeqWO__mp--ModuleType-Production_ModuleSubType-Nml',1,'~/Images/Nav/SeqWO.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Production.SeqWO','Menu.Production.Trans',3,148,1,getdate(),null,getdate(),null)
GO
insert language values('Menu.Production.SeqWO','生产单排序','Seq WO');
GO
--end tiansu 20110406



--begin dingxin 20110303

update acc_menu set code ='Menu.Planning.SupplierSchedule',
pageUrl ='~/Main.aspx?mid=Planning.SupplierSchedule' where code='Menu.Planning.PS'

insert acc_menucommon values('Menu.Planning.SupplierSchedule', 'Menu.Planning', 2, 76, 1,'2010-07-15 10:20:15.000'
,NULL,'2010-07-15 10:20:15.000',NULL);

--begin dingxin 20110303

insert codemstr values('BackFlushMethod', 'BatchFeedGR', 30, 0, '投料收货回冲');
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemDisCon](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Item] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[DisconItem] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[UnitQty] [decimal](18, 8) NOT NULL,
	[Priority] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[LastModifyUser] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
 CONSTRAINT [PK_ItemDisCon] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[ItemDisCon]  WITH CHECK ADD  CONSTRAINT [FK_ItemDisCon_Create_User] FOREIGN KEY([CreateUser])
REFERENCES [dbo].[ACC_User] ([USR_Code])
GO
ALTER TABLE [dbo].[ItemDisCon] CHECK CONSTRAINT [FK_ItemDisCon_Create_User]
GO
ALTER TABLE [dbo].[ItemDisCon]  WITH CHECK ADD  CONSTRAINT [FK_ItemDisCon_Item] FOREIGN KEY([Item])
REFERENCES [dbo].[Item] ([Code])
GO
ALTER TABLE [dbo].[ItemDisCon] CHECK CONSTRAINT [FK_ItemDisCon_Item]
GO
ALTER TABLE [dbo].[ItemDisCon]  WITH CHECK ADD  CONSTRAINT [FK_ItemDisCon_Item1] FOREIGN KEY([DisconItem])
REFERENCES [dbo].[Item] ([Code])
GO
ALTER TABLE [dbo].[ItemDisCon] CHECK CONSTRAINT [FK_ItemDisCon_Item1]
GO
ALTER TABLE [dbo].[ItemDisCon]  WITH CHECK ADD  CONSTRAINT [FK_ItemDisCon_LastModify_User] FOREIGN KEY([LastModifyUser])
REFERENCES [dbo].[ACC_User] ([USR_Code])
GO
ALTER TABLE [dbo].[ItemDisCon] CHECK CONSTRAINT [FK_ItemDisCon_LastModify_User]
--end dingxin 20110303

--begin dingxin 20110302
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StandardCost_CostCenter]') AND parent_object_id = OBJECT_ID(N'[dbo].[StandardCost]'))
ALTER TABLE [dbo].[StandardCost] DROP CONSTRAINT [FK_StandardCost_CostCenter]

alter table standardcost drop column costcenter;

alter table standardcost add CostGroup varchar(50) not null;

ALTER TABLE [dbo].[StandardCost]  WITH CHECK ADD  CONSTRAINT [FK_StandardCost_CostGroup] FOREIGN KEY([CostGroup])
REFERENCES [dbo].[CostGroup] ([Code])
GO
ALTER TABLE [dbo].[StandardCost] CHECK CONSTRAINT [FK_StandardCost_CostGroup]
GO
--end dingxin 20110302

--begin liqiuyun 20110301
update codemstr set codevalue='Daily' where code ='TimePeriodType' and codevalue='Day'
update codemstr set codevalue='Hourly' where code ='TimePeriodType' and codevalue='Hour'
update codemstr set codevalue='Monthly' where code ='TimePeriodType' and codevalue='Month'
update codemstr set codevalue='Quarterly' where code ='TimePeriodType' and codevalue='Quarter'
update codemstr set codevalue='Weekly' where code ='TimePeriodType' and codevalue='Week'
update codemstr set codevalue='Yearly' where code ='TimePeriodType' and codevalue='Year'
--end liqiuyun 20110301

--begin dingxin 20110228
alter table inventoryBalance add ItemCategory varchar(50);
GO
ALTER TABLE [dbo].[InventoryBalance]  WITH CHECK ADD  CONSTRAINT [FK_InventoryBalance_ItemCategory] FOREIGN KEY([ItemCategory])
REFERENCES [dbo].[ItemCategory] ([Code])
GO
ALTER TABLE [dbo].[InventoryBalance] CHECK CONSTRAINT [FK_InventoryBalance_ItemCategory]
GO

alter table item add ScrapPct decimal(18,8);
alter table item add TextField5 varchar(255);
alter table item add TextField6 varchar(255);
alter table item add TextField7 varchar(255);
alter table item add TextField8 varchar(255);

alter table BomDet add TextField1 varchar(255);
alter table BomDet add TextField2 varchar(255);
alter table BomDet add TextField3 varchar(255);
alter table BomDet add TextField4 varchar(255);
alter table BomDet add TextField5 varchar(255);
alter table BomDet add TextField6 varchar(255);
alter table BomDet add TextField7 varchar(255);
alter table BomDet add TextField8 varchar(255);
--end dingxin 20110228

--begin dingxin 20110221
alter table InventoryBalance add location varchar(50) not null;

ALTER TABLE [dbo].[InventoryBalance]  WITH CHECK ADD  CONSTRAINT [FK_InventoryBalance_Location] FOREIGN KEY([Location])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[InventoryBalance] CHECK CONSTRAINT [FK_InventoryBalance_Location]
GO

alter table costtrans add CostAllocateTrans int;

alter table CostBalance drop column lastmodifydate;
alter table CostBalance drop column lastmodifyuser;
alter table CostBalance add FinanceYear int not null;
alter table CostBalance add FinanceMonth int not null;
alter table CostBalance add ItemCategory varchar(50) not null;
ALTER TABLE [dbo].[CostBalance]  WITH CHECK ADD  CONSTRAINT [FK_CostBalance_ItemCategory] FOREIGN KEY([ItemCategory])
REFERENCES [dbo].[ItemCategory] ([Code]);
GO
ALTER TABLE [dbo].[CostBalance] CHECK CONSTRAINT [FK_CostBalance_ItemCategory];
GO

alter table RoutingDet add TactTime numeric(18, 8);
alter table OrderOp drop column Activity;
alter table IpMstr drop column CurrAct;
alter table IpMstr drop column CurrOp;
--end dingxin 20110221 


update acc_menu set pageUrl ='~/Main.aspx?mid=Planning.MPS' where code ='Menu.Planning.MPS'
update acc_menucommon set isactive =1 where id =23

--begin tiansu 20110127 
ALTER  table   dbo.ItemCategory
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL,
	[BitField1] [bit] NULL,--用于慕盛LED类别
	[BitField2] [bit] NULL;
GO
update ItemCategory set [BitField1]=0;
GO
--end tiansu 20110127






--begin dx 20110126 质量管理
alter table RoutingDet drop column Activity;
alter table Location alter column Type varchar(50) not null;

delete from codemstr where code = 'LocationType';

insert into entityopt values('DefaultInspectLocation', '', '默认检验库位', 1);
insert into entityopt values('DefaultRejectLocation', '', '默认不合格品库位', 1);

insert into codemstr values('LocationType', 'Nml', 1, 1, '普通库位');
insert into codemstr values('LocationType', 'INP', 2, 0, '待验库位');
insert into codemstr values('LocationType', 'REJ', 3, 0, '不合格品库位');

alter table InspectMstr add InspLoc varchar(50);
alter table InspectMstr add RejLoc varchar(50);

ALTER TABLE [dbo].[InspectMstr]  WITH CHECK ADD  CONSTRAINT [FK_InspectMstr_InspectLocation] FOREIGN KEY([InspLoc])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[InspectMstr]  WITH CHECK ADD  CONSTRAINT [FK_InspectMstr_RejectLocation] FOREIGN KEY([RejLoc])
REFERENCES [dbo].[Location] ([Code])
GO

ALTER TABLE [dbo].[InspectMstr] CHECK CONSTRAINT [FK_InspectMstr_InspectLocation]
GO
ALTER TABLE [dbo].[InspectMstr] CHECK CONSTRAINT [FK_InspectMstr_RejectLocation]
GO
--end dx 20110126

--begin wx 20110125
alter table orderdet add UnitPriceFrom decimal(18,8);
alter table orderdet add UnitPriceTo decimal(18,8);
alter table orderdet add IsProvEst bit;
alter table orderdet add TaxCode varchar(50);
alter table orderdet add IsIncludeTax bit;
--end wx 20110125

--begin dx 20110125 质量管理
alter table flowmstr add InspLocFrom varchar(50);
alter table flowmstr add InspLocTo varchar(50);
alter table flowmstr add RejLocFrom varchar(50);
alter table flowmstr add RejLocTo varchar(50);
alter table flowmstr add NeedRejInspect bit;
alter table flowdet add InspLocFrom varchar(50);
alter table flowdet add InspLocTo varchar(50);
alter table flowdet add RejLocFrom varchar(50);
alter table flowdet add RejLocTo varchar(50);
alter table flowdet add NeedRejInspect bit;
alter table OrderMstr add InspLocFrom varchar(50);
alter table OrderMstr add InspLocTo varchar(50);
alter table OrderMstr add RejLocFrom varchar(50);
alter table OrderMstr add RejLocTo varchar(50);
alter table OrderMstr add NeedRejInspect bit;
alter table OrderDet add InspLocFrom varchar(50);
alter table OrderDet add InspLocTo varchar(50);
alter table OrderDet add RejLocFrom varchar(50);
alter table OrderDet add RejLocTo varchar(50);
alter table OrderDet add NeedRejInspect bit;
alter table OrderLocTrans add InspLoc varchar(50);
alter table OrderOp add Loc varchar(50);
GO

ALTER TABLE [dbo].[FlowMstr]  WITH CHECK ADD  CONSTRAINT [FK_FlowMstr_InspectLocationFrom] FOREIGN KEY([InspLocFrom])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[FlowMstr]  WITH CHECK ADD  CONSTRAINT [FK_FlowMstr_InspectLocationTo] FOREIGN KEY([InspLocTo])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[FlowMstr]  WITH CHECK ADD  CONSTRAINT [FK_FlowMstr_RejectLocationFrom] FOREIGN KEY([RejLocFrom])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[FlowMstr]  WITH CHECK ADD  CONSTRAINT [FK_FlowMstr_RejectLocationTo] FOREIGN KEY([RejLocTo])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[FlowDet]  WITH CHECK ADD  CONSTRAINT [FK_FlowDet_InspectLocationFrom] FOREIGN KEY([InspLocFrom])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[FlowDet]  WITH CHECK ADD  CONSTRAINT [FK_FlowDet_InspectLocationTo] FOREIGN KEY([InspLocTo])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[FlowDet]  WITH CHECK ADD  CONSTRAINT [FK_FlowDet_RejectLocationFrom] FOREIGN KEY([RejLocFrom])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[FlowDet]  WITH CHECK ADD  CONSTRAINT [FK_FlowDet_RejectLocationTo] FOREIGN KEY([RejLocTo])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[OrderMstr]  WITH CHECK ADD  CONSTRAINT [FK_OrderMstr_InspectLocationFrom] FOREIGN KEY([InspLocFrom])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[OrderMstr]  WITH CHECK ADD  CONSTRAINT [FK_OrderMstr_InspectLocationTo] FOREIGN KEY([InspLocTo])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[OrderMstr]  WITH CHECK ADD  CONSTRAINT [FK_OrderMstr_RejectLocationFrom] FOREIGN KEY([RejLocFrom])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[OrderMstr]  WITH CHECK ADD  CONSTRAINT [FK_OrderMstr_RejectLocationTo] FOREIGN KEY([RejLocTo])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[OrderDet]  WITH CHECK ADD  CONSTRAINT [FK_OrderDet_InspectLocationFrom] FOREIGN KEY([InspLocFrom])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[OrderDet]  WITH CHECK ADD  CONSTRAINT [FK_OrderDet_InspectLocationTo] FOREIGN KEY([InspLocTo])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[OrderDet]  WITH CHECK ADD  CONSTRAINT [FK_OrderDet_RejectLocationFrom] FOREIGN KEY([RejLocFrom])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[OrderDet]  WITH CHECK ADD  CONSTRAINT [FK_OrderDet_RejectLocationTo] FOREIGN KEY([RejLocTo])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[OrderLocTrans]  WITH CHECK ADD  CONSTRAINT [FK_OrderLocTrans_InspectLocation] FOREIGN KEY([InspLoc])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[OrderOp]  WITH CHECK ADD  CONSTRAINT [FK_OrderOp_Location] FOREIGN KEY([Loc])
REFERENCES [dbo].[Location] ([Code])
GO

ALTER TABLE [dbo].[FlowMstr] CHECK CONSTRAINT [FK_FlowMstr_InspectLocationFrom]
GO
ALTER TABLE [dbo].[FlowMstr] CHECK CONSTRAINT [FK_FlowMstr_InspectLocationTo]
GO
ALTER TABLE [dbo].[FlowMstr] CHECK CONSTRAINT [FK_FlowMstr_RejectLocationFrom]
GO
ALTER TABLE [dbo].[FlowMstr] CHECK CONSTRAINT [FK_FlowMstr_RejectLocationTo]
GO
ALTER TABLE [dbo].[FlowDet] CHECK CONSTRAINT [FK_FlowDet_InspectLocationFrom]
GO
ALTER TABLE [dbo].[FlowDet] CHECK CONSTRAINT [FK_FlowDet_InspectLocationTo]
GO
ALTER TABLE [dbo].[FlowDet] CHECK CONSTRAINT [FK_FlowDet_RejectLocationFrom]
GO
ALTER TABLE [dbo].[FlowDet] CHECK CONSTRAINT [FK_FlowDet_RejectLocationTo]
GO
ALTER TABLE [dbo].[OrderMstr] CHECK CONSTRAINT [FK_OrderMstr_InspectLocationFrom]
GO
ALTER TABLE [dbo].[OrderMstr] CHECK CONSTRAINT [FK_OrderMstr_InspectLocationTo]
GO
ALTER TABLE [dbo].[OrderMstr] CHECK CONSTRAINT [FK_OrderMstr_RejectLocationFrom]
GO
ALTER TABLE [dbo].[OrderMstr] CHECK CONSTRAINT [FK_OrderMstr_RejectLocationTo]
GO
ALTER TABLE [dbo].[OrderDet] CHECK CONSTRAINT [FK_OrderDet_InspectLocationFrom]
GO
ALTER TABLE [dbo].[OrderDet] CHECK CONSTRAINT [FK_OrderDet_InspectLocationTo]
GO
ALTER TABLE [dbo].[OrderDet] CHECK CONSTRAINT [FK_OrderDet_RejectLocationFrom]
GO
ALTER TABLE [dbo].[OrderDet] CHECK CONSTRAINT [FK_OrderDet_RejectLocationTo]
GO
ALTER TABLE [dbo].[OrderLocTrans] CHECK CONSTRAINT [FK_OrderLocTrans_InspectLocation]
GO
ALTER TABLE [dbo].[OrderOp] CHECK CONSTRAINT [FK_OrderOp_Location]
GO

insert into codemstr values('OrderSubType', 'Rej', 5, 0, '不合格品退货');
insert into codemstr values('OrderSubType', 'Rus', 6, 0, '让不使用');
--end dx 20110125


--begin tiansu 20110117 CS 库存管理->生成条码 按钮权限
insert into ACC_Permission(PM_Code,PM_Desc,PM_CateCode) values('Module_GenerateHu','生成条码','Terminal');
GO
insert into ACC_Permission(PM_Code,PM_Desc,PM_CateCode) values('Module_LoadMaterial','上料','Terminal');
GO
insert into ACC_Permission(PM_Code,PM_Desc,PM_CateCode) values('Module_ReloadMaterial','换料','Terminal');
GO

CREATE TABLE [dbo].[PartyGrade](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartyCode] [varchar](50) NULL,
	[Type] [varchar](50) NULL,
	[Value] [varchar](50) NULL,
	[Seq] [int] NULL
) ON [PRIMARY]

GO

Alter table Item Add ManufactureParty varchar(50) null;
GO
Alter table HuDet Add CATPartyGrade int null;
GO
Alter table HuDet Add HUEPartyGrade int null;
GO
--end tiansu 20110117



--20110105 新增成本表
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CostGroup](
	[Code] [varchar](50) NOT NULL,
	[Desc1] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Cost_Group] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FinanceCalendar](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FinYear] [int] NOT NULL,
	[FinMonth] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[IsClose] [bit] NOT NULL,
 CONSTRAINT [PK_FinanceCalendar] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

Drop table CurrencyExchange
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CurrencyExchange](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BaseCurrency] [varchar](50) NOT NULL,
	[ExchangeCurrency] [varchar](50) NOT NULL,
	[BaseQty] [decimal](18, 8) NOT NULL,
	[ExchangeQty] [decimal](18, 8) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_ExchangeRate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[CurrencyExchange]  WITH CHECK ADD  CONSTRAINT [FK_CurrencyExchange_BaseCurrency] FOREIGN KEY([BaseCurrency])
REFERENCES [dbo].[Currency] ([Code])
GO
ALTER TABLE [dbo].[CurrencyExchange] CHECK CONSTRAINT [FK_CurrencyExchange_BaseCurrency]
GO
ALTER TABLE [dbo].[CurrencyExchange]  WITH CHECK ADD  CONSTRAINT [FK_CurrencyExchange_ExchangeCurrency] FOREIGN KEY([ExchangeCurrency])
REFERENCES [dbo].[Currency] ([Code])
GO
ALTER TABLE [dbo].[CurrencyExchange] CHECK CONSTRAINT [FK_CurrencyExchange_ExchangeCurrency]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemCategory](
	[Code] [varchar](50) NOT NULL,
	[Desc1] [varchar](255) NOT NULL,
	[Parent] [varchar](50) NULL,
 CONSTRAINT [PK_ItemCategory] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[ItemCategory]  WITH CHECK ADD  CONSTRAINT [FK_ItemCategory_ItemCategory] FOREIGN KEY([Parent])
REFERENCES [dbo].[ItemCategory] ([Code])
GO
ALTER TABLE [dbo].[ItemCategory] CHECK CONSTRAINT [FK_ItemCategory_ItemCategory]
GO

GO
SET ANSI_PADDING OFF

Alter table Item Add Category varchar(50) null;
GO
ALTER TABLE [dbo].[Item]  WITH CHECK ADD  CONSTRAINT [FK_Item_ItemCategory] FOREIGN KEY([Category])
REFERENCES [dbo].[ItemCategory] ([Code])
GO
ALTER TABLE [dbo].[Item] CHECK CONSTRAINT [FK_Item_ItemCategory]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CostCenter](
	[Code] [varchar](50) NOT NULL,
	[Desc1] [varchar](255) NOT NULL,
	[CostGroup] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CostCenter] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[CostCenter]  WITH CHECK ADD  CONSTRAINT [FK_CostCenter_CostGroup] FOREIGN KEY([CostGroup])
REFERENCES [dbo].[CostGroup] ([Code])
GO
ALTER TABLE [dbo].[CostCenter] CHECK CONSTRAINT [FK_CostCenter_CostGroup]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CostElement](
	[Code] [varchar](50) NOT NULL,
	[Desc1] [varchar](255) NOT NULL,
	[Category] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CostElement] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

alter table workcenter add CostCenter varchar(50);
GO
ALTER TABLE [dbo].[WorkCenter]  WITH CHECK ADD  CONSTRAINT [FK_WorkCenter_CostCenter] FOREIGN KEY([CostCenter])
REFERENCES [dbo].[CostCenter] ([Code])
GO
ALTER TABLE [dbo].[WorkCenter] CHECK CONSTRAINT [FK_WorkCenter_CostCenter]
GO

insert into codemstr values('CostCategory', 'Material', 10, false, '材料');
insert into codemstr values('CostCategory', 'Labor', 20, false, '人工');
insert into codemstr values('CostCategory', 'Expense', 30, false, '费用');

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StandardCost](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Item] [varchar](50) NOT NULL,
	[CostElement] [varchar](50) NOT NULL,
	[CostCenter] [varchar](50) NOT NULL,
	[Cost] [numeric](18, 8) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_StandardCost] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[StandardCost]  WITH CHECK ADD  CONSTRAINT [FK_StandardCost_CostCenter] FOREIGN KEY([CostCenter])
REFERENCES [dbo].[CostCenter] ([Code])
GO
ALTER TABLE [dbo].[StandardCost] CHECK CONSTRAINT [FK_StandardCost_CostCenter]
GO
ALTER TABLE [dbo].[StandardCost]  WITH CHECK ADD  CONSTRAINT [FK_StandardCost_CostElement] FOREIGN KEY([CostElement])
REFERENCES [dbo].[CostElement] ([Code])
GO
ALTER TABLE [dbo].[StandardCost] CHECK CONSTRAINT [FK_StandardCost_CostElement]
GO
ALTER TABLE [dbo].[StandardCost]  WITH CHECK ADD  CONSTRAINT [FK_StandardCost_Create_User] FOREIGN KEY([CreateUser])
REFERENCES [dbo].[ACC_User] ([USR_Code])
GO
ALTER TABLE [dbo].[StandardCost] CHECK CONSTRAINT [FK_StandardCost_Create_User]
GO
ALTER TABLE [dbo].[StandardCost]  WITH CHECK ADD  CONSTRAINT [FK_StandardCost_Item] FOREIGN KEY([Item])
REFERENCES [dbo].[Item] ([Code])
GO
ALTER TABLE [dbo].[StandardCost] CHECK CONSTRAINT [FK_StandardCost_Item]
GO
ALTER TABLE [dbo].[StandardCost]  WITH CHECK ADD  CONSTRAINT [FK_StandardCost_LastModify_User] FOREIGN KEY([LastModifyUser])
REFERENCES [dbo].[ACC_User] ([USR_Code])
GO
ALTER TABLE [dbo].[StandardCost] CHECK CONSTRAINT [FK_StandardCost_LastModify_User]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExpenseElement](
	[Code] [varchar](50) NOT NULL,
	[Desc1] [varchar](255) NULL,
 CONSTRAINT [PK_ExpenseElement] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CostTrans](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Item] [varchar](50) NOT NULL,
	[ItemCategory] [varchar](50) NOT NULL,
	[OrderNo] [varchar](50) NULL,
	[RecNo] [varchar](50) NULL,
	[CostGroup] [varchar](50) NOT NULL,
	[CostCenter] [varchar](50) NOT NULL,
	[CostElement] [varchar](50) NOT NULL,
	[Currency] [varchar](50) NOT NULL,
	[BaseCurrency] [varchar](50) NOT NULL,
	[ExchangeRate] [numeric](18, 8) NOT NULL,
	[Qty] [numeric](18, 8) NOT NULL,
	[Amount] [numeric](18, 8) NOT NULL,
	[DiffAmount] [numeric](18, 8) NOT NULL,
	[RefItem] [varchar](50) NULL,
	[RefQty] [numeric](18, 8) NULL,
	[EffDate] [datetime] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CostTrans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[CostTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostTrans_ACC_User] FOREIGN KEY([CreateUser])
REFERENCES [dbo].[ACC_User] ([USR_Code])
GO
ALTER TABLE [dbo].[CostTrans] CHECK CONSTRAINT [FK_CostTrans_ACC_User]
GO
ALTER TABLE [dbo].[CostTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostTrans_BaseCurrency] FOREIGN KEY([BaseCurrency])
REFERENCES [dbo].[Currency] ([Code])
GO
ALTER TABLE [dbo].[CostTrans] CHECK CONSTRAINT [FK_CostTrans_BaseCurrency]
GO
ALTER TABLE [dbo].[CostTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostTrans_CostCenter] FOREIGN KEY([CostCenter])
REFERENCES [dbo].[CostCenter] ([Code])
GO
ALTER TABLE [dbo].[CostTrans] CHECK CONSTRAINT [FK_CostTrans_CostCenter]
GO
ALTER TABLE [dbo].[CostTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostTrans_CostElement] FOREIGN KEY([CostElement])
REFERENCES [dbo].[CostElement] ([Code])
GO
ALTER TABLE [dbo].[CostTrans] CHECK CONSTRAINT [FK_CostTrans_CostElement]
GO
ALTER TABLE [dbo].[CostTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostTrans_CostGroup] FOREIGN KEY([CostGroup])
REFERENCES [dbo].[CostGroup] ([Code])
GO
ALTER TABLE [dbo].[CostTrans] CHECK CONSTRAINT [FK_CostTrans_CostGroup]
GO
ALTER TABLE [dbo].[CostTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostTrans_Currency] FOREIGN KEY([Currency])
REFERENCES [dbo].[Currency] ([Code])
GO
ALTER TABLE [dbo].[CostTrans] CHECK CONSTRAINT [FK_CostTrans_Currency]
GO
ALTER TABLE [dbo].[CostTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostTrans_Item] FOREIGN KEY([Item])
REFERENCES [dbo].[Item] ([Code])
GO
ALTER TABLE [dbo].[CostTrans] CHECK CONSTRAINT [FK_CostTrans_Item]
GO
ALTER TABLE [dbo].[CostTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostTrans_ItemCategory] FOREIGN KEY([ItemCategory])
REFERENCES [dbo].[ItemCategory] ([Code])
GO
ALTER TABLE [dbo].[CostTrans] CHECK CONSTRAINT [FK_CostTrans_ItemCategory]
GO
ALTER TABLE [dbo].[CostTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostTrans_OrderMstr] FOREIGN KEY([OrderNo])
REFERENCES [dbo].[OrderMstr] ([OrderNo])
GO
ALTER TABLE [dbo].[CostTrans] CHECK CONSTRAINT [FK_CostTrans_OrderMstr]
GO
ALTER TABLE [dbo].[CostTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostTrans_ReceiptMstr] FOREIGN KEY([RecNo])
REFERENCES [dbo].[ReceiptMstr] ([RecNo])
GO
ALTER TABLE [dbo].[CostTrans] CHECK CONSTRAINT [FK_CostTrans_ReceiptMstr]
GO
ALTER TABLE [dbo].[CostTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostTrans_RefItem] FOREIGN KEY([RefItem])
REFERENCES [dbo].[Item] ([Code])
GO
ALTER TABLE [dbo].[CostTrans] CHECK CONSTRAINT [FK_CostTrans_RefItem]
GO

Alter table Region add CostCenter varchar(50);
Alter table Region add InspectLoc varchar(50);
Alter table Region add RejectLoc varchar(50);
GO
ALTER TABLE [dbo].[Region]  WITH CHECK ADD  CONSTRAINT [FK_Region_CostCenter] FOREIGN KEY([CostCenter])
REFERENCES [dbo].[CostCenter] ([Code])
GO
ALTER TABLE [dbo].[Region] CHECK CONSTRAINT [FK_Region_CostCenter]
GO
ALTER TABLE [dbo].[Region]  WITH CHECK ADD  CONSTRAINT [FK_Region_InspectLocation] FOREIGN KEY([InspectLoc])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[Region] CHECK CONSTRAINT [FK_Region_InspectLocation]
GO
ALTER TABLE [dbo].[Region]  WITH CHECK ADD  CONSTRAINT [FK_Region_RejectLocation] FOREIGN KEY([RejectLoc])
REFERENCES [dbo].[Location] ([Code])
GO
ALTER TABLE [dbo].[Region] CHECK CONSTRAINT [FK_Region_RejectLocation]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CostDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Item] [varchar](50) NOT NULL,
	[ItemCategory] [varchar](50) NOT NULL,
	[CostGroup] [varchar](50) NOT NULL,
	[CostElement] [varchar](50) NOT NULL,
	[Cost] [numeric](18, 8) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[FinanceYear] [int] NOT NULL,
	[FinanceMonth] [int] NOT NULL,
 CONSTRAINT [PK_CostDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[CostDet]  WITH CHECK ADD  CONSTRAINT [FK_CostDet_CostElement] FOREIGN KEY([CostElement])
REFERENCES [dbo].[CostElement] ([Code])
GO
ALTER TABLE [dbo].[CostDet] CHECK CONSTRAINT [FK_CostDet_CostElement]
GO
ALTER TABLE [dbo].[CostDet]  WITH CHECK ADD  CONSTRAINT [FK_CostDet_CostGroup] FOREIGN KEY([CostGroup])
REFERENCES [dbo].[CostGroup] ([Code])
GO
ALTER TABLE [dbo].[CostDet] CHECK CONSTRAINT [FK_CostDet_CostGroup]
GO
ALTER TABLE [dbo].[CostDet]  WITH CHECK ADD  CONSTRAINT [FK_CostDet_Create_User] FOREIGN KEY([CreateUser])
REFERENCES [dbo].[ACC_User] ([USR_Code])
GO
ALTER TABLE [dbo].[CostDet] CHECK CONSTRAINT [FK_CostDet_Create_User]
GO
ALTER TABLE [dbo].[CostDet]  WITH CHECK ADD  CONSTRAINT [FK_CostDet_Item] FOREIGN KEY([Item])
REFERENCES [dbo].[Item] ([Code])
GO
ALTER TABLE [dbo].[CostDet] CHECK CONSTRAINT [FK_CostDet_Item]
GO
ALTER TABLE [dbo].[CostDet]  WITH CHECK ADD  CONSTRAINT [FK_CostDet_ItemCategory] FOREIGN KEY([ItemCategory])
REFERENCES [dbo].[ItemCategory] ([Code])
GO
ALTER TABLE [dbo].[CostDet] CHECK CONSTRAINT [FK_CostDet_ItemCategory]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CostBalance](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Item] [varchar](50) NOT NULL,
	[CostGroup] [varchar](50) NOT NULL,
	[CostElement] [varchar](50) NOT NULL,
	[Balance] [numeric](18, 8) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CostBalance_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[CostBalance]  WITH CHECK ADD  CONSTRAINT [FK_CostBalance_CostElement] FOREIGN KEY([CostElement])
REFERENCES [dbo].[CostElement] ([Code])
GO
ALTER TABLE [dbo].[CostBalance] CHECK CONSTRAINT [FK_CostBalance_CostElement]
GO
ALTER TABLE [dbo].[CostBalance]  WITH CHECK ADD  CONSTRAINT [FK_CostBalance_CostGroup] FOREIGN KEY([CostGroup])
REFERENCES [dbo].[CostGroup] ([Code])
GO
ALTER TABLE [dbo].[CostBalance] CHECK CONSTRAINT [FK_CostBalance_CostGroup]
GO
ALTER TABLE [dbo].[CostBalance]  WITH CHECK ADD  CONSTRAINT [FK_CostBalance_Create_User] FOREIGN KEY([CreateUser])
REFERENCES [dbo].[ACC_User] ([USR_Code])
GO
ALTER TABLE [dbo].[CostBalance] CHECK CONSTRAINT [FK_CostBalance_Create_User]
GO
ALTER TABLE [dbo].[CostBalance]  WITH CHECK ADD  CONSTRAINT [FK_CostBalance_Item] FOREIGN KEY([Item])
REFERENCES [dbo].[Item] ([Code])
GO
ALTER TABLE [dbo].[CostBalance] CHECK CONSTRAINT [FK_CostBalance_Item]
GO
ALTER TABLE [dbo].[CostBalance]  WITH CHECK ADD  CONSTRAINT [FK_CostBalance_LastModify_User] FOREIGN KEY([LastModifyUser])
REFERENCES [dbo].[ACC_User] ([USR_Code])
GO
ALTER TABLE [dbo].[CostBalance] CHECK CONSTRAINT [FK_CostBalance_LastModify_User]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InventoryBalance](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Item] [varchar](50) NOT NULL,
	[CostGroup] [varchar](50) NOT NULL,
	[Qty] [decimal](18, 8) NOT NULL,
	[FinanceYear] [int] NOT NULL,
	[FinanceMonth] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_InventoryBalance] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[InventoryBalance]  WITH CHECK ADD  CONSTRAINT [FK_InventoryBalance_ACC_User] FOREIGN KEY([CreateUser])
REFERENCES [dbo].[ACC_User] ([USR_Code])
GO
ALTER TABLE [dbo].[InventoryBalance] CHECK CONSTRAINT [FK_InventoryBalance_ACC_User]
GO
ALTER TABLE [dbo].[InventoryBalance]  WITH CHECK ADD  CONSTRAINT [FK_InventoryBalance_CostGroup] FOREIGN KEY([CostGroup])
REFERENCES [dbo].[CostGroup] ([Code])
GO
ALTER TABLE [dbo].[InventoryBalance] CHECK CONSTRAINT [FK_InventoryBalance_CostGroup]
GO
ALTER TABLE [dbo].[InventoryBalance]  WITH CHECK ADD  CONSTRAINT [FK_InventoryBalance_Item] FOREIGN KEY([Item])
REFERENCES [dbo].[Item] ([Code])
GO
ALTER TABLE [dbo].[InventoryBalance] CHECK CONSTRAINT [FK_InventoryBalance_Item]
GO
insert into EntityOpt values ('CostElementMaterial', '1000', '成本要素直接材料代码', 0);
insert into EntityOpt values ('CostElementLabor', '2000', '成本要素直接人工代码', 0);

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TaxRate](
	[Code] [varchar](50) NOT NULL,
	[Desc1] [varchar](50) NOT NULL,
	[TaxRate] [decimal](18, 8) NOT NULL,
 CONSTRAINT [PK_Tax] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

alter table RoutingDet drop column TactTime;
alter table RoutingDet add MachQty numeric(18, 8);
alter table RoutingDet add Yield numeric(18, 8);
alter table OrderOp drop column UnitTime;
alter table OrderOp drop column WorkTime;
alter table OrderOp add SetupTime numeric(18, 8);
alter table OrderOp add RunTime numeric(18, 8);
alter table OrderOp add MoveTime numeric(18, 8);
alter table OrderOp add MachQty numeric(18, 8);
alter table OrderOp add Yield numeric(18, 8);
GO

ALTER TABLE [dbo].[WorkCenter] DROP CONSTRAINT [FK_WorkCenter_Party];
alter table WorkCenter drop column Party;
alter table WorkCenter add column Region varchar(50);
GO
ALTER TABLE [dbo].[WorkCenter]  WITH CHECK ADD  CONSTRAINT [FK_WorkCenter_Region] FOREIGN KEY([Region])
REFERENCES [dbo].[Region] ([Code])
GO
ALTER TABLE [dbo].[WorkCenter] CHECK CONSTRAINT [FK_WorkCenter_Region]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CostAllocateMethod](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExpenseElement] [varchar](50) NOT NULL,
	[CostCenter] [varchar](50) NOT NULL,
	[CostElement] [varchar](50) NOT NULL,
	[DependCostElement] [varchar](50) NOT NULL,
	[AllocateBy] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CostAllocateMethod] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[CostAllocateMethod]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocateMethod_CostCenter] FOREIGN KEY([CostCenter])
REFERENCES [dbo].[CostCenter] ([Code])
GO
ALTER TABLE [dbo].[CostAllocateMethod] CHECK CONSTRAINT [FK_CostAllocateMethod_CostCenter]
GO
ALTER TABLE [dbo].[CostAllocateMethod]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocateMethod_CostElement] FOREIGN KEY([CostElement])
REFERENCES [dbo].[CostElement] ([Code])
GO
ALTER TABLE [dbo].[CostAllocateMethod] CHECK CONSTRAINT [FK_CostAllocateMethod_CostElement]
GO
ALTER TABLE [dbo].[CostAllocateMethod]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocateMethod_CostElement1] FOREIGN KEY([DependCostElement])
REFERENCES [dbo].[CostElement] ([Code])
GO
ALTER TABLE [dbo].[CostAllocateMethod] CHECK CONSTRAINT [FK_CostAllocateMethod_CostElement1]
GO
ALTER TABLE [dbo].[CostAllocateMethod]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocateMethod_ExpenseElement] FOREIGN KEY([ExpenseElement])
REFERENCES [dbo].[ExpenseElement] ([Code])
GO
ALTER TABLE [dbo].[CostAllocateMethod] CHECK CONSTRAINT [FK_CostAllocateMethod_ExpenseElement]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CostAllocateTrans](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExpenseElement] [varchar](50) NULL,
	[CostCenter] [varchar](50) NOT NULL,
	[CostElement] [varchar](50) NOT NULL,
	[DependCostElement] [varchar](50) NOT NULL,
	[AllocateBy] [varchar](50) NOT NULL,
	[Amount] [decimal](18, 8) NOT NULL,
	[EffDate] [datetime] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CostAllocateTrans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[CostAllocateTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocateTrans_ACC_User] FOREIGN KEY([CreateUser])
REFERENCES [dbo].[ACC_User] ([USR_Code])
GO
ALTER TABLE [dbo].[CostAllocateTrans] CHECK CONSTRAINT [FK_CostAllocateTrans_ACC_User]
GO
ALTER TABLE [dbo].[CostAllocateTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocateTrans_CostCenter] FOREIGN KEY([CostCenter])
REFERENCES [dbo].[CostCenter] ([Code])
GO
ALTER TABLE [dbo].[CostAllocateTrans] CHECK CONSTRAINT [FK_CostAllocateTrans_CostCenter]
GO
ALTER TABLE [dbo].[CostAllocateTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocateTrans_CostElement] FOREIGN KEY([CostElement])
REFERENCES [dbo].[CostElement] ([Code])
GO
ALTER TABLE [dbo].[CostAllocateTrans] CHECK CONSTRAINT [FK_CostAllocateTrans_CostElement]
GO
ALTER TABLE [dbo].[CostAllocateTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocateTrans_CostElement1] FOREIGN KEY([DependCostElement])
REFERENCES [dbo].[CostElement] ([Code])
GO
ALTER TABLE [dbo].[CostAllocateTrans] CHECK CONSTRAINT [FK_CostAllocateTrans_CostElement1]
GO
ALTER TABLE [dbo].[CostAllocateTrans]  WITH CHECK ADD  CONSTRAINT [FK_CostAllocateTrans_ExpenseElement] FOREIGN KEY([ExpenseElement])
REFERENCES [dbo].[ExpenseElement] ([Code])
GO
ALTER TABLE [dbo].[CostAllocateTrans] CHECK CONSTRAINT [FK_CostAllocateTrans_ExpenseElement]
GO

insert into codemstr values('AllocateBy', 'Amount', 10, 1, '按金额');
insert into codemstr values('AllocateBy', 'Qty', 20, 1, '按数量');
GO

Alter table PlanBill add CostCenter varchar(50);
Alter table PlanBill add CostGroup varchar(50);

Alter table ActBill add CostCenter varchar(50);
Alter table ActBill add CostGroup varchar(50);

Alter table BillTrans add CostCenter varchar(50);
Alter table BillTrans add CostGroup varchar(50);

Alter table LocTrans add CostCenterFrom varchar(50);
Alter table LocTrans add CostGroupFrom varchar(50);
Alter table LocTrans add CostCenterTo varchar(50);
Alter table LocTrans add CostGroupTo varchar(50);
--20110105

--20101207 盘点并上架企业选项
Insert into entityopt values('PutWhenCycleCount',  'True', '盘点并上架', 100);
--20101207

--dingxin 盘点 2010-11-29
alter table cyclecountmstr add IsDynamic bit;
update cyclecountmstr set IsDynamic = 0;
alter table cyclecountmstr alter column IsDynamic bit not null;
alter table cyclecountdet add StartTime datetime;
alter table cyclecountdet add EndTime datetime;
alter table cyclecountdet add Memo varchar(255);
alter table cyclecountresult add Memo varchar(255);


/****** 对象:  View [dbo].[MenuView]    脚本日期: 11/24/2010 15:21:07 ******/
SELECT     Menu1.Code, Menu1.Version, Menu1.Desc_, Menu1.PageUrl, Menu1.IsActive, Menu1.ImageUrl, Menu1.Remark, 
                      dbo.ACC_MenuCommon.Id AS MenuRelationId, 'ACC_MenuCommon' AS Type, '' AS IndustryOrCompanyCode, ParentMenu1.Code AS ParentCode, 
                      ParentMenu1.Version AS ParenVersion, dbo.ACC_MenuCommon.Level_, dbo.ACC_MenuCommon.Seq, 
                      dbo.ACC_MenuCommon.IsActive AS MenuRelationIsActive, dbo.ACC_MenuCommon.CreateDate, dbo.ACC_MenuCommon.CreateUser, 
                      dbo.ACC_MenuCommon.LastModifyDate, dbo.ACC_MenuCommon.LastModifyUser
FROM         dbo.ACC_MenuCommon INNER JOIN
                      dbo.ACC_Menu AS Menu1 ON Menu1.Code = dbo.ACC_MenuCommon.Menu LEFT OUTER JOIN
                      dbo.ACC_Menu AS ParentMenu1 ON dbo.ACC_MenuCommon.ParentMenu = ParentMenu1.Code
WHERE     (NOT EXISTS
                          (SELECT     1 AS Expr1
                            FROM          dbo.ACC_MenuIndustry INNER JOIN
                                                   dbo.ACC_Industry ON dbo.ACC_MenuIndustry.Industry = dbo.ACC_Industry.Code INNER JOIN
                                                   dbo.ACC_Company ON dbo.ACC_Industry.Code = dbo.ACC_Company.Industry INNER JOIN
                                                   dbo.EntityOpt ON dbo.ACC_Company.Code = dbo.EntityOpt.PreValue INNER JOIN
                                                   dbo.ACC_Menu ON dbo.ACC_MenuIndustry.Menu = dbo.ACC_Menu.Code
                            WHERE      (dbo.EntityOpt.PreCode = 'CompanyCode') AND (dbo.ACC_Menu.Code = Menu1.Code))) AND (NOT EXISTS
                          (SELECT     1 AS Expr1
                            FROM          dbo.ACC_MenuCompany INNER JOIN
                                                   dbo.ACC_Company AS ACC_Company_3 ON dbo.ACC_MenuCompany.Company = ACC_Company_3.Code INNER JOIN
                                                   dbo.EntityOpt AS EntityOpt_3 ON ACC_Company_3.Code = EntityOpt_3.PreValue INNER JOIN
                                                   dbo.ACC_Menu AS ACC_Menu_2 ON dbo.ACC_MenuCompany.Menu = ACC_Menu_2.Code
                            WHERE      (EntityOpt_3.PreCode = 'CompanyCode') AND (Menu1.Code = ACC_Menu_2.Code)))
UNION
SELECT     Menu2.Code, Menu2.Version, Menu2.Desc_, Menu2.PageUrl, Menu2.IsActive, Menu2.ImageUrl, Menu2.Remark, 
                      ACC_MenuIndustry_1.Id AS MenuRelationId, 'ACC_MenuIndustry' AS Type, ACC_MenuIndustry_1.Industry AS IndustryOrCompanyCode, 
                      ParentMenu2.Code AS ParentCode, ParentMenu2.Version AS ParentVersion, ACC_MenuIndustry_1.Level_, ACC_MenuIndustry_1.Seq, 
                      ACC_MenuIndustry_1.IsActive AS MenuRelationIsActive, ACC_MenuIndustry_1.CreateDate, ACC_MenuIndustry_1.CreateUser, 
                      ACC_MenuIndustry_1.LastModifyDate, ACC_MenuIndustry_1.LastModifyUser
FROM         dbo.ACC_MenuIndustry AS ACC_MenuIndustry_1 INNER JOIN
                      dbo.ACC_Industry AS ACC_Industry_1 ON ACC_MenuIndustry_1.Industry = ACC_Industry_1.Code INNER JOIN
                      dbo.ACC_Company AS ACC_Company_4 ON ACC_Company_4.Industry = ACC_Industry_1.Code INNER JOIN
                      dbo.EntityOpt AS EntityOpt_4 ON ACC_Company_4.Code = EntityOpt_4.PreValue INNER JOIN
                      dbo.ACC_Menu AS Menu2 ON Menu2.Code = ACC_MenuIndustry_1.Menu LEFT OUTER JOIN
                      dbo.ACC_Menu AS ParentMenu2 ON ACC_MenuIndustry_1.ParentMenu = ParentMenu2.Code
WHERE     (EntityOpt_4.PreCode = 'CompanyCode') AND (NOT EXISTS
                          (SELECT     1 AS Expr1
                            FROM          dbo.ACC_MenuCompany AS ACC_MenuCompany_2 INNER JOIN
                                                   dbo.ACC_Company AS ACC_Company_2 ON ACC_MenuCompany_2.Company = ACC_Company_2.Code INNER JOIN
                                                   dbo.EntityOpt AS EntityOpt_2 ON ACC_Company_2.Code = EntityOpt_2.PreValue INNER JOIN
                                                   dbo.ACC_Menu AS ACC_Menu_1 ON ACC_MenuCompany_2.Menu = ACC_Menu_1.Code
                            WHERE      (EntityOpt_2.PreCode = 'CompanyCode') AND (ACC_Menu_1.Code = Menu2.Code)))
UNION
SELECT     Menu3.Code, Menu3.Version, Menu3.Desc_, Menu3.PageUrl, Menu3.IsActive, Menu3.ImageUrl, Menu3.Remark, 
                      ACC_MenuCompany_1.Id AS MenuRelationId, 'ACC_MenuCompany' AS Type, ACC_MenuCompany_1.Company AS IndustryOrCompanyCode, 
                      ParentMenu3.Code AS ParentCode, ParentMenu3.Version AS ParentVersion, ACC_MenuCompany_1.Level_, ACC_MenuCompany_1.Seq, 
                      ACC_MenuCompany_1.IsActive AS MenuRelationIsActive, ACC_MenuCompany_1.CreateDate, ACC_MenuCompany_1.CreateUser, 
                      ACC_MenuCompany_1.LastModifyDate, ACC_MenuCompany_1.LastModifyUser
FROM         dbo.ACC_MenuCompany AS ACC_MenuCompany_1 INNER JOIN
                      dbo.ACC_Menu AS Menu3 ON ACC_MenuCompany_1.Menu = Menu3.Code INNER JOIN
                      dbo.ACC_Company AS ACC_Company_1 ON ACC_MenuCompany_1.Company = ACC_Company_1.Code INNER JOIN
                      dbo.EntityOpt AS EntityOpt_1 ON ACC_Company_1.Code = EntityOpt_1.PreValue LEFT OUTER JOIN
                      dbo.ACC_Menu AS ParentMenu3 ON ACC_MenuCompany_1.ParentMenu = ParentMenu3.Code
WHERE     (EntityOpt_1.PreCode = 'CompanyCode')



--begin liqiuyun 20101123 增加 产品类 菜单
INSERT ACC_Permission VALUES ('Menu.ItemCategory','产品类','MasterData')
INSERT acc_menu VALUES ('ItemCategory.166','Menu.ItemCategory.166',1,'Menu.ItemCategory','Menu.ItemCategory.Description','产品类','~/MasterData/ItemCategory/Default.aspx',1,'~/Images/Nav/ItemCategory.png','2010-07-15 10:15:12',null,'2010-07-15 10:15:12',null,null)
INSERT ACC_MenuCommon VALUES ('ItemCategory.166',11,2,27,1,'2010-09-03 00:00:00',null,'2010-09-03 00:00:00',null)

--end liqiuyun 20101123

alter table OrderDet add Remark varchar(50);


--修复供应商菜单bug
update acc_permission set pm_code='~/Main.aspx?mid=Warehouse.InProcessLocation__mp--ModuleType-Procurement_Action-View_IsSupplier-true'
where pm_code='~/Main.aspx?mid=Warehouse.InProcessLocation__mp--ModuleType-Distribution_Action-View_IsSupplier-true'

update acc_menu set pageurl='~/Main.aspx?mid=Warehouse.InProcessLocation__mp--ModuleType-Procurement_Action-View_IsSupplier-true'
where pageurl='~/Main.aspx?mid=Warehouse.InProcessLocation__mp--ModuleType-Distribution_Action-View_IsSupplier-true'
--


----增加发货单模板
INSERT INTO [Sconit_Xng_Test].[dbo].[CodeMstr]([Code],[CodeValue],[Seq],[IsDefault],[Desc1])
     VALUES('OrderTemplate','DeliveryOrder.xls',40,0,'发货单模板')

--beging 盘点
alter table CycleCountMstr add Bins varchar(max);
alter table CycleCountMstr add Items varchar(max);
alter table CycleCountMstr add IsScanHu bit;
alter table CycleCountMstr add StartUser varchar(50);
alter table CycleCountMstr add StartDate datetime;
alter table CycleCountMstr add CompleteUser varchar(50);
alter table CycleCountMstr add CompleteDate datetime;
alter table CycleCountResult add IsProcess bit;

ALTER TABLE [dbo].[CycleCountMstr]  WITH CHECK ADD  CONSTRAINT [FK_CycleCountMstr_ACC_User5] FOREIGN KEY([StartUser])
REFERENCES [dbo].[ACC_User] ([USR_Code]);

ALTER TABLE [dbo].[CycleCountMstr]  WITH CHECK ADD  CONSTRAINT [FK_CycleCountMstr_ACC_User6] FOREIGN KEY([CompleteUser])
REFERENCES [dbo].[ACC_User] ([USR_Code]);

delete from codemstr where code = 'PhysicalCountType' and codevalue = 'SpotCheck';
GO
update ACC_Menu set PageUrl='~/Main.aspx?mid=Inventory.Stocktaking' where Id='94';
update dbo.ACC_Permission set pm_code='~/Main.aspx?mid=Inventory.Stocktaking' where pm_id=541;
GO
--end 




--增加默认班次
INSERT INTO "EntityOpt" (PreCode,PreValue,CodeDesc,Seq) VALUES ('DefaultShift','A','默认班次',0)

--nng增加工单自动完工程序
INSERT INTO [dbo].[BatchJobDet]([Name],[Desc1],[ServiceName]) VALUES ('WOCompleteJob','Job of Automatic Complete WorkOrders','OrderCompleteJob');
INSERT INTO [dbo].[BatchTrigger]([Name],[Desc1],[JobId],[NextFireTime],[PrevFireTime],[RepeatCount],[Interval],[IntervalType],[TimesTriggered],[Status]) VALUES ('WOCompleteTrigger','Trigger of Wo Complete',5,'2010-11-1',null,0,1,'Days',0,'Pause')


--begin tiansu 20101104  销售单折扣精度bug

alter table orderdet alter column DiscountTo decimal(18, 8);
alter table ordermstr alter column DiscountTo decimal(18, 8);
GO

--end tiansu 


--begin wangxiang 10/27/2010 修改订单明细视图，显示米数
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[OrderDetView]
AS
SELECT     MAX(dbo.OrderDet.Id) AS Id, dbo.OrderMstr.Flow, dbo.FlowMstr.Desc1, dbo.OrderMstr.Type, dbo.OrderMstr.PartyFrom, dbo.OrderMstr.PartyTo, 
                      CONVERT(datetime, CONVERT(varchar(8), dbo.OrderMstr.StartTime, 112)) AS EffDate, dbo.OrderMstr.Shift, dbo.OrderDet.Item, dbo.OrderDet.Uom, 
                      SUM(dbo.OrderDet.ReqQty) AS ReqQty, SUM(dbo.OrderDet.OrderQty) AS OrderQty, ISNULL(SUM(dbo.OrderDet.ShipQty), 0) AS ShipQty, 
                      ISNULL(SUM(dbo.OrderDet.RecQty), 0) AS RecQty, ISNULL(SUM(dbo.OrderDet.RejQty), 0) AS RejQty, ISNULL(SUM(dbo.OrderDet.ScrapQty), 0) 
                      AS ScrapQty, dbo.OrderMstr.Status, ISNULL(SUM(dbo.OrderDet.NumField1), 0) AS NumField1
FROM         dbo.OrderDet INNER JOIN
                      dbo.OrderMstr ON dbo.OrderDet.OrderNo = dbo.OrderMstr.OrderNo INNER JOIN
                      dbo.FlowMstr ON dbo.OrderMstr.Flow = dbo.FlowMstr.Code
GROUP BY dbo.OrderMstr.Flow, dbo.FlowMstr.Desc1, dbo.OrderMstr.Type, dbo.OrderMstr.PartyFrom, dbo.OrderMstr.PartyTo, CONVERT(varchar(8), 
                      dbo.OrderMstr.StartTime, 112), dbo.OrderMstr.Shift, dbo.OrderDet.Item, dbo.OrderDet.Uom, dbo.OrderMstr.Status, dbo.OrderDet.NumField1
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
--end wangxiang 10/27/2010

--begin liqiuyun  10/25/2010  修改账单
alter table Actbill alter column LocFrom varchar(50)
alter table Actbill alter column IpNo  varchar(50)
alter table Actbill add Flow varchar(50);

alter table BillDet alter column LocFrom varchar(50)
alter table BillDet alter column IpNo  varchar(50)
alter table BillDet alter column RefItemCode  varchar(50)
alter table BillDet add Flow varchar(50);

alter table PlanBill alter column LocFrom varchar(50)
alter table PlanBill alter column IpNo  varchar(50)
alter table PlanBill add Flow varchar(50);
--end liqiuyun  10/25/2010  修改账单

--begin liqiuyun  10/19/2010  修改外部订单号
alter table Ordermstr alter column extOrderNo varchar(255)
alter table Receiptmstr alter column extrecno  varchar(255)
--end liqiuyun

/****** 对象:  View [dbo].[LocBinItemDet]    脚本日期: 10/19/2010 08:04:20  liqiuyun******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER VIEW [dbo].[LocBinItemDet]
AS
SELECT     MAX(dbo.LocationLotDet.Id) AS Id, dbo.LocationLotDet.Location, dbo.LocationLotDet.Bin, dbo.LocationLotDet.Item, 
                      dbo.Item.Desc1 + ISNULL(' [' + dbo.Item.Desc2 + ']', '') AS ItemDesc, dbo.Item.UC, dbo.Item.Uom, SUM(dbo.LocationLotDet.Qty) AS Qty
FROM         dbo.LocationLotDet INNER JOIN
                      dbo.Item ON dbo.LocationLotDet.Item = dbo.Item.Code
WHERE     (dbo.LocationLotDet.Qty <> 0)
GROUP BY dbo.LocationLotDet.Location, dbo.LocationLotDet.Bin, dbo.LocationLotDet.Item, dbo.Item.Desc1 + ISNULL(' [' + dbo.Item.Desc2 + ']', ''), dbo.Item.UC, 
                      dbo.Item.Uom

--begin wangxiang 20100925 修改菜单
INSERT ACC_Permission VALUES ('~/Main.aspx?mid=Visualization.ProductLineInprocessLocation','生产投料明细','Visualization');
INSERT acc_menu VALUES ('165','Menu.ProductLineInprocessLocation.165',1,'Menu.ProductLineInprocessLocation','Menu.ProductLineInprocessLocation.Description','生产投料明细','~/Main.aspx?mid=Visualization.ProductLineInprocessLocation',1,'~/Images/Nav/ViewWOIP.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('165','134',3,150,1,getdate(),null,getdate(),null)
--end wangxiang

delete from codemstr where code='FlowType' and codevalue='Inspection'
delete from codemstr where code='OrderType' and codevalue='Inspection'

alter table location add IsSetCS bit;
update location set IsSetCS = 0;

--begin tiansu 20100913 修改菜单
update acc_menu set id='Menu.POPlanBill.165',Code='Menu.POPlanBill.165' where id='164';
GO
update ACC_MenuCommon set MenuId='Menu.POPlanBill.165'  where MenuId='164';
GO
--end tiansu 20100913


--begin liqiuyun 20100913 增加供应商-供应商寄售 菜单
INSERT ACC_Permission VALUES ('~/Main.aspx?mid=Finance.PlanBill.PO__mp--ModuleType-PO_IsSupplier-true','供应商寄售','SupplierMenu')
INSERT acc_menu VALUES ('164','Menu.POPlanBill.164',1,'Menu.POPlanBill','Menu.POPlanBill.Description','供应商寄售','~/Main.aspx?mid=Finance.PlanBill.PO__mp--ModuleType-PO_IsSupplier-true',1,'~/Images/Nav/POPlanBill.png','2010-07-15 10:15:12',null,'2010-07-15 10:15:12',null,null)
INSERT ACC_MenuCommon VALUES ('164','142',2,289,1,'2010-09-03 00:00:00',null,'2010-09-03 00:00:00',null)
--end liqiuyun 20100913




alter table HuDet add HuTemplate varchar(50);




--begin liqiuyun 20100903 权限修改
update acc_permissioncategory set pmc_desc='目视管理' where pmc_code='Visualization';
update ACC_PERMISSION set  pm_desc='委外路线' where  pm_desc='委外加工';
insert into acc_permissioncategory(PMC_Code,PMC_Desc,PMC_Type) values ('Quality','质量管理','Menu');
update acc_permission set pm_catecode='Quality' where pm_code ='~/Main.aspx?mid=Order.OrderHead.Procurement__mp--ModuleType-Procurement_ModuleSubType-Rtn_StatusGroupId-1_IsQuick-true__act--NewAction';
update acc_permission set pm_catecode='Quality' where pm_code ='~/Main.aspx?mid=Order.OrderHead.Procurement__mp--ModuleType-Procurement_ModuleSubType-Rtn_StatusGroupId-1_IsQuick-true_IsReject-true__act--NewAction';
update acc_permission set pm_catecode='Quality' where pm_code ='~/Main.aspx?mid=Order.OrderHead.Production__mp--ModuleType-Production_ModuleSubType-Rwo_StatusGroupId-1__act--NewAction';
update acc_permission set pm_catecode='Quality' where pm_code ='~/Main.aspx?mid=Production.WorkshopScrap';
update acc_permission set pm_catecode='Quality' where pm_code ='~/Main.aspx?mid=Order.OrderHead.Production__mp--ModuleType-Production_ModuleSubType-Adj_StatusGroupId-1_IsScrap-true__act--NewAction';
update acc_permission set pm_catecode='Quality' where pm_code ='~/Main.aspx?mid=Order.OrderHead.Production__mp--ModuleType-Production_ModuleSubType-Rwo_StatusGroupId-1_IsScrap-true_IsReuse-true__act--NewAction';
update acc_permission set pm_catecode='Quality' where pm_code ='~/Main.aspx?mid=Order.OrderHead.Distribution__mp--ModuleType-Distribution_ModuleSubType-Rtn_StatusGroupId-1_IsQuick-true__act--NewAction';
update acc_permission set pm_catecode='Quality' where pm_code ='~/Main.aspx?mid=Inventory.InspectOrder';
update acc_permission set pm_catecode='Quality' where pm_code ='~/Main.aspx?mid=Order.OrderHead.Transfer__mp--ModuleType-Transfer_ModuleSubType-Rtn_StatusGroupId-1_IsQuick-true_IsReject-true__act--NewAction';
update acc_permission set pm_desc='让步使用' where pm_code ='~/Main.aspx?mid=Order.OrderHead.Transfer__mp--ModuleType-Transfer_ModuleSubType-Rtn_StatusGroupId-1_IsQuick-true_IsReject-true__act--NewAction';
insert acc_permission values('~/Main.aspx?mid=Warehouse.InProcessLocation__mp--ModuleType-Distribution_Action-View_AsnType-Gap','收货差异处理','Quality');
GO
--end liqiuyun 20100903




--begin tiansu 20100903 添加面套模板
INSERT INTO "CodeMstr" (Code,CodeValue,Seq,IsDefault,Desc1) VALUES ('HuTemplate','BarCodeShellFabric.xls',30,0,'条码模板(面套)');
GO
update acc_menu set pageurl='' where id=32;
GO
update acc_menucommon set isactive=0 where menuid=32;
GO

INSERT INTO "dbo"."ACC_Menu" (Id,Code,Version,Title,Description,Desc_,PageUrl,IsActive,ImageUrl,CreateDate,CreateUser,LastModifyDate,LastModifyUser,Remark) VALUES ('163','Menu.GRAdjustment.163',1,'Menu.GRAdjustment','Menu.GRAdjustment.Description','收货调整','~/Main.aspx?mid=Order.ReceiptNotes__mp--ModuleType-Procurement_ModuleSubType-Adj',1,'~/Images/Nav/GRAdjustment.png','2010-09-03 00:00:00',null,'2010-09-03 00:00:00',null,null)
GO

set IDENTITY_INSERT ACC_MenuCommon on;
INSERT INTO "dbo"."ACC_MenuCommon" (Id,MenuId,ParentMenuId,Level_,Seq,IsActive,CreateDate,CreateUser,LastModifyDate,LastModifyUser) VALUES (160,'163','27',3,97,1,'2010-09-03 00:00:00',null,'2010-09-03 00:00:00',null)
set IDENTITY_INSERT ACC_MenuCommon off;
--end tiansu 20100903


--begin tiansu 20100902 就改菜单视图

/****** 对象:  View [dbo].[MenuView]    脚本日期: 09/02/2010 09:52:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER VIEW [dbo].[MenuView]
AS
SELECT     Menu1.Id, Menu1.Code, Menu1.Version, Menu1.Title, Menu1.Desc_, Menu1.Description, Menu1.PageUrl, Menu1.IsActive, Menu1.ImageUrl, 
                      Menu1.Remark, dbo.ACC_MenuCommon.Id AS MenuRelationId, 'ACC_MenuCommon' AS Type, '' AS IndustryOrCompanyCode, 
                      dbo.ACC_MenuCommon.ParentMenuId AS ParentId, ParentMenu1.Code AS ParentCode, ParentMenu1.Version AS ParenVersion, 
                      dbo.ACC_MenuCommon.Level_, dbo.ACC_MenuCommon.Seq, dbo.ACC_MenuCommon.IsActive AS MenuRelationIsActive, 
                      dbo.ACC_MenuCommon.CreateDate, dbo.ACC_MenuCommon.CreateUser, dbo.ACC_MenuCommon.LastModifyDate, 
                      dbo.ACC_MenuCommon.LastModifyUser
FROM         dbo.ACC_MenuCommon INNER JOIN
                      dbo.ACC_Menu AS Menu1 ON Menu1.Id = dbo.ACC_MenuCommon.MenuId LEFT OUTER JOIN
                      dbo.ACC_Menu AS ParentMenu1 ON dbo.ACC_MenuCommon.ParentMenuId = ParentMenu1.Id
WHERE     (NOT EXISTS
                          (SELECT     1 AS Expr1
                            FROM          dbo.ACC_MenuIndustry INNER JOIN
                                                   dbo.ACC_Industry ON dbo.ACC_MenuIndustry.IndustryCode = dbo.ACC_Industry.Code INNER JOIN
                                                   dbo.ACC_Company ON dbo.ACC_Industry.Code = dbo.ACC_Company.IndustryCode INNER JOIN
                                                   dbo.EntityOpt ON dbo.ACC_Company.Code = dbo.EntityOpt.PreValue INNER JOIN
                                                   dbo.ACC_Menu ON dbo.ACC_MenuIndustry.MenuId = dbo.ACC_Menu.Id
                            WHERE      (dbo.EntityOpt.PreCode = 'CompanyCode') AND (dbo.ACC_Menu.Code = Menu1.Code))) AND (NOT EXISTS
                          (SELECT     1 AS Expr1
                            FROM          dbo.ACC_MenuCompany INNER JOIN
                                                   dbo.ACC_Company AS ACC_Company_3 ON dbo.ACC_MenuCompany.CompanyCode = ACC_Company_3.Code INNER JOIN
                                                   dbo.EntityOpt AS EntityOpt_3 ON ACC_Company_3.Code = EntityOpt_3.PreValue INNER JOIN
                                                   dbo.ACC_Menu AS ACC_Menu_2 ON dbo.ACC_MenuCompany.MenuId = ACC_Menu_2.Id
                            WHERE      (EntityOpt_3.PreCode = 'CompanyCode') AND (Menu1.Code = ACC_Menu_2.Code)))
UNION
SELECT     Menu2.Id, Menu2.Code, Menu2.Version, Menu2.Title, Menu2.Desc_, Menu2.Description, Menu2.PageUrl, Menu2.IsActive, Menu2.ImageUrl, 
                      Menu2.Remark, ACC_MenuIndustry_1.Id AS MenuRelationId, 'ACC_MenuIndustry' AS Type, 
                      ACC_MenuIndustry_1.IndustryCode AS IndustryOrCompanyCode, ACC_MenuIndustry_1.ParentMenuId AS ParentId, ParentMenu2.Code AS ParentCode, 
                      ParentMenu2.Version AS ParentVersion, ACC_MenuIndustry_1.Level_, ACC_MenuIndustry_1.Seq, 
                      ACC_MenuIndustry_1.IsActive AS MenuRelationIsActive, ACC_MenuIndustry_1.CreateDate, ACC_MenuIndustry_1.CreateUser, 
                      ACC_MenuIndustry_1.LastModifyDate, ACC_MenuIndustry_1.LastModifyUser
FROM         dbo.ACC_MenuIndustry AS ACC_MenuIndustry_1 INNER JOIN
                      dbo.ACC_Industry AS ACC_Industry_1 ON ACC_MenuIndustry_1.IndustryCode = ACC_Industry_1.Code INNER JOIN
                      dbo.ACC_Company AS ACC_Company_4 ON ACC_Company_4.IndustryCode = ACC_Industry_1.Code INNER JOIN
                      dbo.EntityOpt AS EntityOpt_4 ON ACC_Company_4.Code = EntityOpt_4.PreValue INNER JOIN
                      dbo.ACC_Menu AS Menu2 ON Menu2.Id = ACC_MenuIndustry_1.MenuId LEFT OUTER JOIN
                      dbo.ACC_Menu AS ParentMenu2 ON ACC_MenuIndustry_1.ParentMenuId = ParentMenu2.Id
WHERE     (EntityOpt_4.PreCode = 'CompanyCode') AND (NOT EXISTS
                          (SELECT     1 AS Expr1
                            FROM          dbo.ACC_MenuCompany AS ACC_MenuCompany_2 INNER JOIN
                                                   dbo.ACC_Company AS ACC_Company_2 ON ACC_MenuCompany_2.CompanyCode = ACC_Company_2.Code INNER JOIN
                                                   dbo.EntityOpt AS EntityOpt_2 ON ACC_Company_2.Code = EntityOpt_2.PreValue INNER JOIN
                                                   dbo.ACC_Menu AS ACC_Menu_1 ON ACC_MenuCompany_2.MenuId = ACC_Menu_1.Id
                            WHERE      (EntityOpt_2.PreCode = 'CompanyCode') AND (ACC_Menu_1.Code = Menu2.Code)))
UNION
SELECT     Menu3.Id, Menu3.Code, Menu3.Version, Menu3.Title, Menu3.Desc_, Menu3.Description, Menu3.PageUrl, Menu3.IsActive, Menu3.ImageUrl, 
                      Menu3.Remark, ACC_MenuCompany_1.Id AS MenuRelationId, 'ACC_MenuCompany' AS Type, 
                      ACC_MenuCompany_1.CompanyCode AS IndustryOrCompanyCode, ACC_MenuCompany_1.ParentMenuId AS ParentId, 
                      ParentMenu3.Code AS ParentCode, ParentMenu3.Version AS ParentVersion, ACC_MenuCompany_1.Level_, ACC_MenuCompany_1.Seq, 
                      ACC_MenuCompany_1.IsActive AS MenuRelationIsActive, ACC_MenuCompany_1.CreateDate, ACC_MenuCompany_1.CreateUser, 
                      ACC_MenuCompany_1.LastModifyDate, ACC_MenuCompany_1.LastModifyUser
FROM         dbo.ACC_MenuCompany AS ACC_MenuCompany_1 INNER JOIN
                      dbo.ACC_Menu AS Menu3 ON ACC_MenuCompany_1.MenuId = Menu3.Id INNER JOIN
                      dbo.ACC_Company AS ACC_Company_1 ON ACC_MenuCompany_1.CompanyCode = ACC_Company_1.Code INNER JOIN
                      dbo.EntityOpt AS EntityOpt_1 ON ACC_Company_1.Code = EntityOpt_1.PreValue LEFT OUTER JOIN
                      dbo.ACC_Menu AS ParentMenu3 ON ACC_MenuCompany_1.ParentMenuId = ParentMenu3.Id
WHERE     (EntityOpt_1.PreCode = 'CompanyCode')

--end tiansu 20100902




ALTER  table   dbo.HuDet
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetNextSequence]
	@CodePrefix varchar(50),
	@NextSequence int OUTPUT
AS
Begin Tran
	Declare @invValue int;
	select  @invValue = IntValue FROM NumCtrl WITH (UPDLOCK, ROWLOCK) where Code = @CodePrefix;
	if @invValue is null
	begin
		if @NextSequence is not null
		begin 
			insert into NumCtrl(Code, IntValue) values (@CodePrefix, @NextSequence + 1);
		end	
		else
		begin
			set @NextSequence = 1;
			insert into NumCtrl(Code, IntValue) values (@CodePrefix, 2);
		end
	end 
	else
	begin
		if @NextSequence is not null
		begin 
			if @invValue <= @NextSequence
			begin
				update NumCtrl set IntValue = @NextSequence + 1 where Code = @CodePrefix;
			end
		end
		else
		begin
			set @NextSequence = @invValue;
			update NumCtrl set IntValue = @NextSequence + 1 where Code = @CodePrefix;
		end
	end	
Commit tran

alter table FlowDet add ExtraDmdSource varchar(255);

alter table locationlotdet add RefLoc varchar(50);

ALTER table dbo.BillTrans
add	[BillDet] int NULL;

ALTER  table   dbo.OrderMstr
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[TextField5] [varchar](255) NULL,
	[TextField6] [varchar](255) NULL,
	[TextField7] [varchar](255) NULL,
	[TextField8] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[NumField5] [decimal](18, 8) NULL,
	[NumField6] [decimal](18, 8) NULL,
	[NumField7] [decimal](18, 8) NULL,
	[NumField8] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL,
	[DateField3] [datetime] NULL,
	[DateField4] [datetime] NULL;
	
	
ALTER  table   dbo.OrderDet
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[TextField5] [varchar](255) NULL,
	[TextField6] [varchar](255) NULL,
	[TextField7] [varchar](255) NULL,
	[TextField8] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[NumField5] [decimal](18, 8) NULL,
	[NumField6] [decimal](18, 8) NULL,
	[NumField7] [decimal](18, 8) NULL,
	[NumField8] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL,
	[DateField3] [datetime] NULL,
	[DateField4] [datetime] NULL;
	
	
ALTER  table   dbo.FlowMstr
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[TextField5] [varchar](255) NULL,
	[TextField6] [varchar](255) NULL,
	[TextField7] [varchar](255) NULL,
	[TextField8] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[NumField5] [decimal](18, 8) NULL,
	[NumField6] [decimal](18, 8) NULL,
	[NumField7] [decimal](18, 8) NULL,
	[NumField8] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL,
	[DateField3] [datetime] NULL,
	[DateField4] [datetime] NULL;
	
ALTER  table   dbo.FlowDet
add	[ExtraDmdSource] [varchar](255) Null,
    [TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[TextField5] [varchar](255) NULL,
	[TextField6] [varchar](255) NULL,
	[TextField7] [varchar](255) NULL,
	[TextField8] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[NumField5] [decimal](18, 8) NULL,
	[NumField6] [decimal](18, 8) NULL,
	[NumField7] [decimal](18, 8) NULL,
	[NumField8] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL,
	[DateField3] [datetime] NULL,
	[DateField4] [datetime] NULL;
	
	

ALTER  table   dbo.BillMstr
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
	
ALTER  table   dbo.BillDet
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
	
ALTER  table   dbo.InspectMstr
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
	
ALTER  table   dbo.InspectDet
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
	
ALTER  table   dbo.IpMstr
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
ALTER  table   dbo.IpDet
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
ALTER  table   dbo.PickListMstr
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
ALTER  table   dbo.PickListDet
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
ALTER  table   dbo.PriceListMstr
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
ALTER  table   dbo.PriceListDet
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
ALTER  table   dbo.ReceiptMstr
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
ALTER  table   dbo.ReceiptDet
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
	
ALTER  table   dbo.Item
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
	
ALTER  table   dbo.Location
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
ALTER  table   dbo.Party
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
ALTER  table   dbo.PartyAddr
add	[TextField1] [varchar](255) NULL,
	[TextField2] [varchar](255) NULL,
	[TextField3] [varchar](255) NULL,
	[TextField4] [varchar](255) NULL,
	[NumField1] [decimal](18, 8) NULL,
	[NumField2] [decimal](18, 8) NULL,
	[NumField3] [decimal](18, 8) NULL,
	[NumField4] [decimal](18, 8) NULL,
	[DateField1] [datetime] NULL,
	[DateField2] [datetime] NULL;
	
go


--
-- Script To Create dbo.OrderTracer Table
-- Generated 星期三, 八月 25, 2010, at 02:58 PM
--
-- Author: Deng Xuyao
--
BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Creating dbo.OrderTracer Table'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO

CREATE TABLE [dbo].[OrderTracer] (
   [Id] [int] IDENTITY (1, 1) NOT NULL,
   [OrderDetId] [int] NOT NULL,
   [TracerType] [varchar] (50) COLLATE Chinese_PRC_CI_AS NOT NULL,
   [Code] [varchar] (50) COLLATE Chinese_PRC_CI_AS NOT NULL,
   [Item] [varchar] (50) COLLATE Chinese_PRC_CI_AS NOT NULL,
   [ReqTime] [datetime] NOT NULL,
   [OrderQty] [decimal] (18, 8) NOT NULL CONSTRAINT [DF_OrderTracer_OrderQty] DEFAULT ((0)),
   [AccumQty] [decimal] (18, 8) NOT NULL CONSTRAINT [DF_OrderTracer_AccumQty] DEFAULT ((0)),
   [Qty] [decimal] (18, 8) NOT NULL CONSTRAINT [DF_OrderTracer_Qty] DEFAULT ((0)),
   [RefOrderLocTransId] [int] NOT NULL CONSTRAINT [DF_OrderTracer_RefOrderLocTransId] DEFAULT ((0)),
   [Memo] [varchar] (255) COLLATE Chinese_PRC_CI_AS NULL
)
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   ALTER TABLE [dbo].[OrderTracer] ADD CONSTRAINT [PK_OrderTracer] PRIMARY KEY CLUSTERED ([Id])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   CREATE INDEX [IX_OrderTracer] ON [dbo].[OrderTracer] ([OrderDetId])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.OrderTracer Table Added Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Add dbo.OrderTracer Table'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Creating dbo.OrderTracer Table'
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF NOT EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_OrderTracer_OrderDet')
      ALTER TABLE [dbo].[OrderTracer] ADD CONSTRAINT [FK_OrderTracer_OrderDet] FOREIGN KEY ([OrderDetId]) REFERENCES [dbo].[OrderDet] ([Id])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.OrderTracer Table Added Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Add dbo.OrderTracer Table'
END
GO


--
-- Script To Update dbo.LeanEngineView View 
-- Generated 星期四, 八月 26, 2010, at 08:40 AM
--
-- Author: Deng Xuyao
--
BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.LeanEngineView View'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   exec sp_dropextendedproperty N'MS_DiagramPane1', 'Schema', N'dbo', 'View', N'LeanEngineView'
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   exec sp_dropextendedproperty N'MS_DiagramPaneCount', 'Schema', N'dbo', 'View', N'LeanEngineView'
GO

exec('ALTER VIEW dbo.LeanEngineView
AS
SELECT     dbo.FlowView.FlowDetId, dbo.FlowView.Flow, dbo.FlowView.IsAutoCreate, dbo.FlowView.LocFrom, dbo.FlowView.LocTo, dbo.FlowDet.Item, 
                      dbo.FlowDet.Uom, dbo.FlowDet.UC, dbo.FlowDet.HuLotSize, dbo.FlowDet.Bom, dbo.FlowDet.SafeStock, dbo.FlowDet.MaxStock, 
                      dbo.FlowDet.MinLotSize, dbo.FlowDet.OrderLotSize, dbo.FlowDet.BatchSize, dbo.FlowDet.RoundUpOpt, dbo.FlowMstr.Type, dbo.FlowMstr.PartyFrom, 
                      dbo.FlowMstr.PartyTo, dbo.FlowMstr.FlowStrategy, dbo.FlowMstr.LeadTime, dbo.FlowMstr.EmTime, dbo.FlowMstr.MaxCirTime, 
                      dbo.FlowMstr.WinTime1, dbo.FlowMstr.WinTime2, dbo.FlowMstr.WinTime3, dbo.FlowMstr.WinTime4, dbo.FlowMstr.WinTime5, 
                      dbo.FlowMstr.WinTime6, dbo.FlowMstr.WinTime7, dbo.FlowMstr.NextOrderTime, dbo.FlowMstr.NextWinTime, dbo.FlowMstr.WeekInterval, 
                      dbo.FlowDet.ExtraDmdSource
FROM         dbo.FlowView INNER JOIN
                      dbo.FlowDet ON dbo.FlowView.FlowDetId = dbo.FlowDet.Id INNER JOIN
                      dbo.FlowMstr ON dbo.FlowDet.Flow = dbo.FlowMstr.Code')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   exec sp_addextendedproperty  N'MS_DiagramPane1', N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "FlowView"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 166
               Right = 184
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "FlowDet"
            Begin Extent = 
               Top = 6
               Left = 222
               Bottom = 191
               Right = 396
            End
            DisplayFlags = 280
            TopColumn = 36
         End
         Begin Table = "FlowMstr"
            Begin Extent = 
               Top = 6
               Left = 434
               Bottom = 211
               Right = 618
            End
            DisplayFlags = 280
            TopColumn = 69
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', 'Schema', N'dbo', 'View', N'LeanEngineView'
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   exec sp_addextendedproperty  N'MS_DiagramPaneCount', N'1', 'Schema', N'dbo', 'View', N'LeanEngineView'
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.LeanEngineView View Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.LeanEngineView View'
END
GO



INSERT ACC_Permission VALUES ('Menu.Production.ProdIONew','消耗差异','Production')
INSERT acc_menu VALUES ('Menu.Production.ProdIONew',1,'消耗差异','~/Main.aspx?mid=Reports.ProdIONew',1,'~/Images/Nav/ProdIONew.png',getdate(),null,getdate(),null,null)
INSERT ACC_MenuCommon VALUES ('Menu.Production.ProdIONew','Menu.Production.Info',3,245,1,getdate(),null,getdate(),null)