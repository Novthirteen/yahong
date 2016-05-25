--20130812 设施管理脚本
insert into ACC_MenuCommon  (Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
values ('Menu.Facility','Menu.Home',1,200,1,getdate(),'su',getdate(),'su')

insert into acc_menu values ('Menu.Facility',1,'设施管理','',1,'~/Images/Nav/Application.png',getdate(),'su',getdate(),'su',null)

insert into acc_permissioncategory values('Facility','设施管理','Menu')


insert into ACC_MenuCommon (Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
values ('Menu.Facility.Trans','Menu.Facility',2,10,1,getdate(),'su',getdate(),'su')

insert into ACC_MenuCommon (Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
values ('Menu.Facility.Info','Menu.Facility',2,20,1,getdate(),'su',getdate(),'su')

insert into ACC_MenuCommon (Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
values ('Menu.Facility.Setup','Menu.Facility',2,30,1,getdate(),'su',getdate(),'su')

insert into acc_menu values ('Menu.Facility.Trans',1,'事务','',1,'~/Images/Nav/Transaction.png',getdate(),'su',getdate(),'su',null)
insert into acc_menu values ('Menu.Facility.Info',1,'信息','',1,'~/Images/Nav/Information.png',getdate(),'su',getdate(),'su',null)
insert into acc_menu values ('Menu.Facility.Setup',1,'设置','',1,'~/Images/Nav/Setup.png',getdate(),'su',getdate(),'su',null)

insert into acc_menu values ('Menu.Facility.FacilityMaster',1,'设施','~/Main.aspx?mid=Facility.FacilityMaster',1,'~/Images/Nav/Item.png',getdate(),'su',getdate(),'su',null)
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityMaster','设施','Facility')

insert into ACC_MenuCommon (Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
values ('Menu.Facility.FacilityMaster','Menu.Facility.Trans',3,10,1,getdate(),'su',getdate(),'su')

insert into acc_permissioncategory values('FacilityOperation','设施管理操作','Page')
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('CreateFacility','设施新建','FacilityOperation')

insert into acc_menu values ('Menu.Facility.FacilityCategory',1,'设施类型','~/Main.aspx?mid=Facility.FacilityCategory',1,'~/Images/Nav/Item.png',getdate(),'su',getdate(),'su',null)
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityCategory','设施类型','Facility')

insert into ACC_MenuCommon (Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
values ('Menu.Facility.FacilityCategory','Menu.Facility.Setup',3,10,1,getdate(),'su',getdate(),'su')

insert into codemstr values('FacilityOwner','Own',10,1,'自有')
insert into codemstr values('FacilityOwner','Customer',20,0,'客户')
insert into codemstr values('FacilityOwner','Supplier',30,0,'供应商')



----新增表

/****** Object:  Table [dbo].[Fac_Facility]    Script Date: 09/06/2013 10:04:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Fac_Facility](
	[FCID] [varchar](100) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Specification] [varchar](100) NULL,
	[Capacity] [varchar](100) NULL,
	[ManufactureDate] [datetime] NULL,
	[Manufacturer] [varchar](100) NULL,
	[SerialNo] [varchar](100) NULL,
	[AssetNo] [varchar](100) NULL,
	[WarrantyInfo] [varchar](100) NULL,
	[TechInfo] [varchar](100) NULL,
	[Supplier] [varchar](100) NULL,
	[SupplierInfo] [varchar](100) NULL,
	[PONo] [varchar](100) NULL,
	[EffDate] [varchar](100) NULL,
	[Price] [varchar](100) NULL,
	[Owner] [varchar](100) NULL,
	[OwnerDesc] [varchar](100) NULL,
	[Remark] [varchar](100) NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
	[Status] [varchar](100) NOT NULL,
	[OldChargePerson] [varchar](100) NULL,
	[CurrChargePerson] [varchar](100) NULL,
	[ChargeSite] [varchar](100) NULL,
	[ChargeOrg] [varchar](100) NULL,
	[ChargeDate] [datetime] NULL,
	[Category] [varchar](100) NOT NULL,
	[MaintainType] [varchar](100) NULL,
	[MaintainRule] [varchar](100) NULL,
 CONSTRAINT [PK_Fac_Facility] PRIMARY KEY CLUSTERED 
(
	[FCID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



/****** Object:  Table [dbo].[Fac_FacilityCategory]    Script Date: 09/06/2013 10:04:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Fac_FacilityCategory](
	[Code] [varchar](50) NOT NULL,
	[ChargePerson] [varchar](50) NULL,
	[ParentCategory] [varchar](50) NULL,
	[Description] [varchar](50) NULL,
 CONSTRAINT [PK_Fac_FacilityCategory] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [dbo].[Fac_FacilityTrans]    Script Date: 09/06/2013 10:05:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Fac_FacilityTrans](
	[FCID] [varchar](50) NOT NULL,
	[TransType] [varchar](50) NOT NULL,
	[FromChargePerson] [varchar](50) NULL,
	[ToChargePerson] [varchar](50) NULL,
	[FromParty] [varchar](50) NULL,
	[ToParty] [varchar](50) NULL,
	[Remark] [varchar](200) NULL,
	[EffDate] [datetime] NULL,
	[Attachment] [varchar](200) NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [varchar](50) NULL,
 CONSTRAINT [PK_Fac_FacilityTrans] PRIMARY KEY CLUSTERED 
(
	[FCID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO
insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityApply',1,'领用','~/Main.aspx?mid=Facility.FacilityApply',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityApply','Menu.Facility.Trans',3,20,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityApply','领用','Facility')

insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityLend',1,'租赁','~/Main.aspx?mid=Facility.FacilityLend',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityLend','Menu.Facility.Trans',3,40,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityLend','租赁','Facility')

insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilitySell',1,'转让','~/Main.aspx?mid=Facility.FacilitySell',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilitySell','Menu.Facility.Trans',3,40,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilitySell','转让','Facility')

insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityTransfer',1,'移交','~/Main.aspx?mid=Facility.FacilityTransfer',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityTransfer','Menu.Facility.Trans',3,50,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityTransfer','移交','Facility')



insert into codemstr values('FacilityStatus','Test',10,1,'调试')
insert into codemstr values('FacilityStatus','Available',20,0,'可用')
insert into codemstr values('FacilityStatus','Inspect',30,0,'检测')
insert into codemstr values('FacilityStatus','Maintain',40,0,'保养')
insert into codemstr values('FacilityStatus','Fixing',50,0,'维修')
insert into codemstr values('FacilityStatus','Lend',60,0,'租赁')
insert into codemstr values('FacilityStatus','Envelop',70,0,'封存')
insert into codemstr values('FacilityStatus','Scrap',80,0,'报废')
insert into codemstr values('FacilityStatus','Sell',90,0,'转让')
insert into codemstr values('FacilityStatus','Lose',100,0,'盘亏')

alter table fac_facility add MaintainStartDate datetime null
alter table fac_facility add MaintainPeriod int null
alter table fac_facility add MaintainLeadTime int null
alter table fac_facility drop column MaintainRule


insert into codemstr values('FacilityMaintainType','Once',10,1,'一次性')
insert into codemstr values('FacilityMaintainType','Day',20,0,'天')
insert into codemstr values('FacilityMaintainType','Month',30,0,'月')
insert into codemstr values('FacilityMaintainType','Year',40,0,'年')



alter table Fac_Facility add IsInStore bit
go

drop table dbo.Fac_Facilitytrans
go
/****** Object:  Table [dbo].[Fac_FacilityTrans]    Script Date: 09/17/2013 11:10:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Fac_FacilityTrans](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FCID] [varchar](50) NOT NULL,
	[TransType] [varchar](50) NOT NULL,
	[FromChargePerson] [varchar](50) NULL,
	[ToChargePerson] [varchar](50) NULL,
	[FromParty] [varchar](50) NULL,
	[ToParty] [varchar](50) NULL,
	[Remark] [varchar](200) NULL,
	[EffDate] [datetime] NULL,
	[Attachment] [varchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [varchar](50) NULL,
 CONSTRAINT [PK_Fac_FacilityTrans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


alter table fac_facility add NextMaintainTime datetime null;
alter table fac_facilitytrans add FromChargeSite varchar(50)
alter table fac_facilitytrans add ToChargeSite varchar(50)
alter table fac_facilitycategory add ChargeSite varchar(50)
alter table fac_facilitycategory add ChargeOrg varchar(50)
alter table fac_facilitytrans drop column Fromparty
alter table fac_facilitytrans drop column Toparty
alter table fac_facilitytrans add FromOrg varchar(50)
alter table fac_facilitytrans add ToOrg varchar(50)


alter table fac_facility add MaintainTypePeriod int null


insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityTrans',1,'设施事务','~/Main.aspx?mid=Facility.FacilityTrans',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityTrans','Menu.Facility.Info',3,20,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityTrans','设施事务','Facility')
alter table fac_facilitytrans add FacilityCategory varchar(50)


insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityMaintain',1,'保养','~/Main.aspx?mid=Facility.FacilityMaintain',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityMaintain','Menu.Facility.Trans',3,40,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityMaintain','保养','Facility')

insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityFix',1,'维修','~/Main.aspx?mid=Facility.FacilityFix',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityFix','Menu.Facility.Trans',3,50,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityFix','维修','Facility')



insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityInspect',1,'检测','~/Main.aspx?mid=Facility.FacilityInspect',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityInspect','Menu.Facility.Trans',3,20,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityInspect','检测','Facility')


insert into codemstr values('FacilityTransType','MaintainStart',10,1,'开始保养')
insert into codemstr values('FacilityTransType','MaintainFinish',20,0,'结束保养')
insert into codemstr values('FacilityTransType','Apply',30,0,'领用')
insert into codemstr values('FacilityTransType','Return',40,0,'归还')
insert into codemstr values('FacilityTransType','Transfer',50,0,'移交')
insert into codemstr values('FacilityTransType','Envelop',60,0,'封存')
insert into codemstr values('FacilityTransType','FixStart',70,0,'开始维修')
insert into codemstr values('FacilityTransType','FixFinish',80,0,'结束维修')
insert into codemstr values('FacilityTransType','InspectStart',90,0,'开始检测')
insert into codemstr values('FacilityTransType','InspectFinish',100,0,'结束检测')
insert into codemstr values('FacilityTransType','Lend',110,0,'租赁')
insert into codemstr values('FacilityTransType','Sell',120,0,'转让')
insert into codemstr values('FacilityTransType','Lose',130,0,'盘亏')
insert into codemstr values('FacilityTransType','Scrap',140,0,'报废')




insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityEnvelop',1,'封存','~/Main.aspx?mid=Facility.FacilityEnvelop',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityEnvelop','Menu.Facility.Trans',3,80,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityEnvelop','封存','Facility')


insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.MaintainPlan',1,'预防计划','~/Main.aspx?mid=Facility.MaintainPlan',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.MaintainPlan','Menu.Facility.Setup',3,20,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.MaintainPlan','预防计划','Facility')

/****** Object:  Table [dbo].[Fac_MaintainPlan]    Script Date: 10/21/2013 07:59:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Fac_MaintainPlan](
	[Code] [varchar](50) NOT NULL,
	[Description] [varchar](100) NULL,
	[Type] [varchar](50) NULL,
	[Period] [int] NULL,
	[LeadTime] [int] NULL,
	[TypePeriod] [int] NULL,
 CONSTRAINT [PK_Fac_MaintainPlan] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[Fac_FacilityMaintainPlan]    Script Date: 10/21/2013 07:59:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Fac_FacilityMaintainPlan](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MPCode] [varchar](50) NOT NULL,
	[FCID] [varchar](50) NOT NULL,
	[StartDate] [datetime] NULL,
	[NextMaintainDate] [datetime] NULL,
	[NextWarnDate] [datetime] NULL,
 CONSTRAINT [PK_Fac_FacilityMaintainPlan] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



alter table fac_facility add OldChargePersonNm varchar(100) null
alter table fac_facility add CurrChargePersonNm varchar(100) null
alter table Fac_FacilityCategory add ChargePersonNm varchar(100) null


insert into codemstr values('FacilityOwner','ELSE',40,0,'其它')


insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityScrap',1,'报废','~/Main.aspx?mid=Facility.FacilityScrap',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityScrap','Menu.Facility.Trans',3,100,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityScrap','报废','Facility')

update ACC_MenuCommon set seq=30 where menu='Menu.Facility.FacilityTransfer'
update ACC_MenuCommon set seq=60 where menu='Menu.Facility.FacilityInspect'
update ACC_MenuCommon set seq=70 where menu='Menu.Facility.FacilityLend'
update ACC_MenuCommon set seq=100 where menu='Menu.Facility.FacilityScrap'
update ACC_MenuCommon set seq=110 where menu='Menu.Facility.FacilitySell'

update  ACC_Menu set desc_='变更'  where code='Menu.Facility.FacilityTransfer'
update  ACC_Menu set desc_='台账'  where code='Menu.Facility.FacilityMaster'

update ACC_permission set pm_desc='变更'  where pm_code='Menu.Facility.FacilityTransfer'
update ACC_permission set pm_desc='台账'  where pm_code='Menu.Facility.FacilityMaster'
alter table fac_facilitytrans add FromChargePersonNm varchar(100) null
alter table fac_facilitytrans add ToChargePersonNm varchar(100) null
alter table fac_facilitytrans add StartDate DateTime null
alter table fac_facilitytrans add EndDate DateTime null

insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityLost',1,'盘亏','~/Main.aspx?mid=Facility.FacilityLost',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityLost','Menu.Facility.Trans',3,90,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityLost','盘亏','Facility')

alter table fac_facility add IsOffBalance bit default 0

insert into codemstr values('FacilityMaintainType','Minute',50,0,'分钟')
insert into codemstr values('FacilityMaintainType','Hour',60,0,'小时')

alter table Fac_MaintainPlan add FacilityCategory varchar(50)
alter table Fac_MaintainPlan add StartUpUser varchar(max)

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('UpdateFacilityTrans','设施事务修改','FacilityOperation')


alter table dbo.Fac_FacilityTrans add  AssetNo varchar(100)
alter table dbo.Fac_FacilityTrans add  FacilityName varchar(100)

--设施管理任务
insert into BatchJobDet values('FacilityMaintainJob','设施预防提醒','FacilityMaintainJob')
insert into BatchTrigger values('FacilityMaintainJob','设施预防提醒',903,'2013-11-21 06:30:00.000',null,0,7,'Days',0,'Pause')

insert into codemstr values('FacilityTransType','Reopen',65,0,'解封')
insert into codemstr values('FacilityTransType','Create',1,0,'创建')
insert into codemstr values('FacilityTransType','Enable',5,0,'启用')
--add by zs for chargesite master data
--insert into acc_menu values ('Menu.Facility.ChargeSite',1,'存放地点','~/Main.aspx?mid=Facility.ChargeSite',1,'~/Images/Nav/Item.png',getdate(),'su',getdate(),'su',null)
--insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
--select 'Menu.Facility.ChargeSite','Menu.Facility.Setup',3,300,1,GETDATE(),'su',GETDATE(),'su'

--insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.ChargeSite','存放地点','Facility')

--select * from ACC_MenuCommon

--CREATE TABLE Fac_ChargeSite
--(
--	Code varchar(50) primary key,
--	Desc1 varchar(255)
--)

ALTER TABLE Fac_Facility ADD IsAsset bit
ALTER TABLE Fac_Facility ADD RefCode varchar(50)



/****** Object:  Table [dbo].[Fac_FacilityItem]    Script Date: 01/14/2014 13:38:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Fac_FacilityItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FCID] [varchar](50) NOT NULL,
	[ItemCode] [varchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsAllocate] [bit] NOT NULL,
	[Qty] [decimal](18, 8) NOT NULL,
	[Amount] [decimal](18, 8) NOT NULL,
	[AllocatedQty] [decimal](18, 8) NOT NULL,
	[AllocatedAmount] [decimal](18, 8) NOT NULL,
	[AllocateType] [varchar](50) NOT NULL,
	[WarnRate] [decimal](18, 8) NOT NULL,
	[PassRate] [decimal](18, 8) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Fac_FacilityItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


insert into acc_menu values ('Menu.Facility.FacilityItem',1,'设施零件对照','~/Main.aspx?mid=Facility.Facilityitem',1,'~/Images/Nav/Item.png',getdate(),'su',getdate(),'su',null)
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityItem','设施零件对照','Facility')

insert into ACC_MenuCommon (Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
values ('Menu.Facility.FacilityItem','Menu.Facility.Setup',3,30,1,getdate(),'su',getdate(),'su')

insert into codemstr values('FacilityAllocateType','Procurement',10,1,'采购')
insert into codemstr values('FacilityAllocateType','Distribution',20,0,'销售')
insert into codemstr values('FacilityAllocateType','Production',30,0,'生产')


--设施盘点
/****** Object:  Table [dbo].[Fac_StockTakeMstr]    Script Date: 01/26/2014 13:49:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Fac_StockTakeMstr](
	[StNo] [varchar](50) NOT NULL,
	[EffDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[ChargeOrg] [varchar](max) NULL,
	[ChargePerson] [varchar](max) NULL,
	[ChargeSite] [varchar](max) NULL,
	[FacilityCategory] [varchar](max) NULL,
	[ChargePersonNm] [varchar](max) NULL,
	[Status] [varchar](50) NULL,
	[ReleaseUser] [varchar](50) NULL,
	[ReleaseDate] [datetime] NULL,
	[CancelUser] [varchar](50) NULL,
	[CancelDate] [datetime] NULL,
	[CloseUser] [varchar](50) NULL,
	[CloseDate] [datetime] NULL,
	[StartUser] [varchar](50) NULL,
	[StartDate] [datetime] NULL,
	[CompleteUser] [varchar](50) NULL,
	[CompleteDate] [datetime] NULL,
 CONSTRAINT [PK_FAC_StockTakeMstr] PRIMARY KEY CLUSTERED 
(
	[StNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [dbo].[Fac_StockTakeDet]    Script Date: 01/26/2014 13:49:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Fac_StockTakeDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StNo] [varchar](50) NOT NULL,
	[FCID] [varchar](50) NOT NULL,
	[Qty] [decimal](18, 8) NOT NULL,
	[InvQty] [decimal](18, 8) NOT NULL,
	[DiffQty] [decimal](18, 8) NOT NULL,
	[DiffReason] [varchar](50) NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
 CONSTRAINT [PK_FAC_StockTakeDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityStock',1,'盘点','~/Main.aspx?mid=Facility.FacilityStock',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityStock','Menu.Facility.Trans',3,95,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityStock','盘点','Facility')
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('EditFacilityStock','设施盘点单操作','FacilityOperation')


INSERT INTO [dbo].[ISI_TaskSubType] ([Code],[Desc_],[Parent],[Type],[IsActive],[Seq],[AssignUser],[IsAutoAssign],[StartUser],[IsAssignUp],[AssignUpTime],[AssignUpUser],[IsStartUp],[StartUpTime],[StartUpUser],[IsCloseUp],[CloseUpTime],[CloseUpUser],[CreateDate],[CreateUser],
		   [LastModifyDate],[LastModifyUser],[IsPublic],[IsCompleteUp],[CompleteUpTime],[IsStart],[StartPercent],[ProjectType],[IsQuote],[IsInitiation],[IsReport],[ViewUser],[IsOpen],[OpenTime],[Org],[Version],[ECType],[ECUser],[IsEC],[IsAutoStart],
		   [IsAutoComplete],[IsAutoClose],[IsAutoStatus],[RegisterNo],[ExtNo],[Amount],[IsWF],[IsAmount],[IsApply],[ProcessNo],[Desc2],[IsAssignUser],[IsTrace],[IsCostCenter],[IsPrint],[Color],[IsAttachment],[Template],[IsRemoveForm],[IsCtrl],
		   [IsRemind],[IsCost],[IsAmountDetail],[IsBudget],[FormType],[CostCenter],[Account1],[Account2])
 values('SSGL',	'设施管理',	NULL,	'Plan',	1,		40,	null,0,	null,	0,	4320,	null,	0,	4320,	null,	0,	4320,	null,	getdate(),	'su',	
getdate(),	'su',	0,	1,	0,	1,	0.7,	NULL,	1,	1,	0,null,0,null,'亚虹',1,null,null,0,1,1,1,1,1,	NULL,NULL,0,0,0,null,null,
0,0,0,0,null,0,null,0,0,0,0,0,0,null,null,null,null)

--20140312修改
update acc_menu set desc_='分摊模' where code='Menu.Facility.FacilityItem'
delete from codemstr where code='FacilityAllocateType'

insert into codemstr values('FacilityAllocateType','PO',10,1,'采购')
insert into codemstr values('FacilityAllocateType','SO',20,1,'销售')


insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityDistribution',1,'经销模','~/Main.aspx?mid=Facility.FacilityDistribution',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityDistribution','Menu.Facility.Setup',3,40,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityDistribution','经销模','Facility')


insert into codemstr values ('FacilityMaintainType','Week',70,0,'周')


insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityBatchMaintain',1,'批量保养','~/Main.aspx?mid=Facility.FacilityBatchMaintain',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityBatchMaintain','Menu.Facility.Trans',3,45,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityBatchMaintain','批量保养','Facility')

alter table fac_facility add MaintainGroup varchar(100)
alter table fac_facilitytrans add BatchNo varchar(50)



/****** Object:  Table [dbo].[Fac_FacilityDistribution]    Script Date: 04/22/2014 10:57:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Fac_FacilityDistribution](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FCID] [varchar](50) NOT NULL,
	[SupplierName] [varchar](50) NULL,
	[CustomerName] [varchar](50) NULL,
	[PurchaseContractCode] [varchar](50) NULL,
	[PurchaseContractAmount] [decimal](18, 8) NOT NULL,
	[PurchaseBilledAmount] [decimal](18, 8) NOT NULL,
	[PurchasePayAmount] [decimal](18, 8) NOT NULL,
	[DistributionContractCode] [varchar](50) NULL,
	[DistributionContractAmount] [decimal](18, 8) NOT NULL,
	[DistributionBilledAmount] [decimal](18, 8) NOT NULL,
	[DistributionPayAmount] [decimal](18, 8) NOT NULL,
	[Status] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Fac_FacilityDistribution] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [dbo].[Fac_FacilityDistributionDet]    Script Date: 04/22/2014 10:58:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Fac_FacilityDistributionDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FacilityDistributionId] [int] NOT NULL,
	[Type] [varchar](50) NOT NULL,
	[PayDate] [datetime] NULL,
	[PayAmount] [decimal](18, 8) NULL,
	[BillDate] [datetime] NULL,
	[BillAmount] [decimal](18, 8) NULL,
	[Contact] [varchar](255) NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Fac_FacilityDistributionDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


alter table fac_facility add PrintTemplate varchar(50)

alter table fac_facilitydistribution add Remark varchar(255)
alter table fac_facilitydistributionDet add Remark varchar(255),Invoice varchar(100)


delete from codemstr where code='FacilityDistributionStatus'
insert into CodeMstr values ('FacilityDistributionStatus','Create',10,1,'创建')
insert into CodeMstr values ('FacilityDistributionStatus','In-Process',20,0,'执行中')
insert into CodeMstr values ('FacilityDistributionStatus','DistributionComplete',30,0,'销售完成采购未完成')
insert into CodeMstr values ('FacilityDistributionStatus','PurchaseComplete',40,0,'销售未完成采购完成')
insert into CodeMstr values ('FacilityDistributionStatus','Close',50,0,'关闭')


insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('SaveFacilityDistribution','修改经销模','FacilityOperation')
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('DeleteFacilityDistritbution','删除经销模','FacilityOperation')
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('PurchaseComplete','经销模采购完成','FacilityOperation')
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('DistributionComplete','经销模销售完成','FacilityOperation')
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('DistributionClose','关闭经销模','FacilityOperation')
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('CreateFacilityDistributionDetail','创建经销模明细','FacilityOperation')


alter table Fac_FacilityDistributionDet add  BatchNo varchar(255)

alter table Fac_FacilityDistribution add  PurchaseContact varchar(255)
alter table Fac_FacilityDistribution add  DistributionContact varchar(255)


insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityDistributionDetail',1,'经销模明细','~/Main.aspx?mid=Facility.FacilityDistributionDetail',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityDistributionDetail','Menu.Facility.Info',3,20,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityDistributionDetail','经销模明细','Facility')


insert into ACC_Menu(Code, Version, Desc_, PageUrl, IsActive, ImageUrl, CreateDate, CreateUser, LastModifyDate, LastModifyUser, Remark)
select 'Menu.Facility.FacilityMaintainPlanDetail',1,'预防策略明细','~/Main.aspx?mid=Facility.FacilityMaintainPlanDetail',1,'~/Images/Nav/Item.png',GETDATE(),'su',GETDATE(),'su',''

insert into ACC_MenuCommon(Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
select 'Menu.Facility.FacilityMaintainPlanDetail','Menu.Facility.Info',3,30,1,GETDATE(),'su',GETDATE(),'su'

insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityMaintainPlanDetail','预防策略明细','Facility')

alter table fac_stocktakemstr add AssetNo varchar(50)


--模具分摊
/****** Object:  Table [dbo].[Fac_FacilityItem]    Script Date: 05/29/2014 09:01:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Fac_FacilityAllocate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FCID] [varchar](50) NOT NULL,
	[ItemCode] [varchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[AllocatedQty] [decimal](18, 8) NOT NULL,
	[AllocateType] [varchar](50) NOT NULL,
	[WarnQty] [decimal](18, 8) NOT NULL,
	[NextWarnQty] [decimal](18, 8) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [varchar](50) NOT NULL,
	[LastModifyDate] [datetime] NOT NULL,
	[LastModifyUser] [varchar](50) NOT NULL,
	[StartUpUser] [varchar](max) NULL,
 CONSTRAINT [PK_Fac_FacilityAllocate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


insert into acc_menu values ('Menu.Facility.FacilityAllocate',1,'模具使用统计','~/Main.aspx?mid=Facility.FacilityAllocate',1,'~/Images/Nav/Item.png',getdate(),'su',getdate(),'su',null)
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.FacilityAllocate','模具使用统计','Facility')

insert into ACC_MenuCommon (Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
values ('Menu.Facility.FacilityAllocate','Menu.Facility.Setup',3,30,1,getdate(),'su',getdate(),'su')

insert into codemstr values('MouldAllocateType','Procurement',10,1,'采购')
insert into codemstr values('MouldAllocateType','Distribution',20,0,'销售')
insert into codemstr values('MouldAllocateType','Production',30,0,'生产')

insert into codemstr values('FacilityMaintainType','Frequency',80,0,'次数')

alter table fac_facilitymaintainplan add StartQty decimal(18,8),NextMaintainQty decimal(18,8),NextWarnQty decimal(18,8)


insert into acc_menu values ('Menu.Facility.Reports.FacilityTransReport',1,'设施台账变更统计报表','~/Main.aspx?mid=Facility.Reports.FacilityTransReport',1,'~/Images/Nav/Item.png',getdate(),'su',getdate(),'su',null)
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.Reports.FacilityTransReport','设施台账变更统计报表','Facility')

insert into ACC_MenuCommon (Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
values ('Menu.Facility.Reports.FacilityTransReport','Menu.Facility.Info',3,40,1,getdate(),'su',getdate(),'su')


insert into acc_menu values ('Menu.Facility.Reports.FacilityMaintainReport',1,'设施运维报表','~/Main.aspx?mid=Facility.Reports.FacilityMaintainReport',1,'~/Images/Nav/Item.png',getdate(),'su',getdate(),'su',null)
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.Reports.FacilityMaintainReport','设施运维报表','Facility')

insert into ACC_MenuCommon (Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
values ('Menu.Facility.Reports.FacilityMaintainReport','Menu.Facility.Info',3,40,1,getdate(),'su',getdate(),'su')




/****** Object:  StoredProcedure [dbo].[USP_Rep_FacilityTrans]    Script Date: 06/25/2014 08:32:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[USP_Rep_FacilityTrans]
(

	@StartDate datetime,
	@EndDate datetime,
	@ChargeOrg varchar(50),
	@CategoryCode varchar(50)
)
AS
BEGIN

	create table #tempCategoryTrans
		(
			CTRowId int Identity(1, 1),
			CategoryCode varchar(50),
			ChargeOrg varchar(50),
			TransType varchar(50),
			TransCount int
		)

    DECLARE @SqlStr varchar(8000);

    --按分类汇总好
	insert into #tempCategoryTrans(CategoryCode,ChargeOrg ,TransType,TransCount)
	select isnull(p.code,g.code) as CategoryCode,f.ChargeOrg as ChargeOrg,t.TransType,count(1) as TransCount
	from fac_facilitytrans t
	inner join fac_facility f on t.fcid = f.fcid
	inner join fac_facilitycategory g on f.category = g.code
	left join fac_facilitycategory p on g.parentcategory = p.code
	where (isnull(g.parentcategory,'')='' or isnull(p.parentcategory,'')='')
	and t.EffDate >= @StartDate and t.EffDate <= @EndDate
	group by isnull(p.code,g.code),t.transtype,f.ChargeOrg
	
	
	--盘点的也加进去
	insert into #tempCategoryTrans(CategoryCode, ChargeOrg,TransType,TransCount)
	select isnull(p.code,g.code) as CategoryCode,f.ChargeOrg as ChargeOrg,'StockTake',count(1) as TransCount
	from fac_Stocktakedet d
	inner join fac_Stocktakemstr m on d.StNo = m.StNo
	inner join fac_facility f on d.fcid = f.fcid
	inner join fac_facilitycategory g on f.category = g.code
	left join fac_facilitycategory p on g.parentcategory = p.code
	where (isnull(g.parentcategory,'')='' or isnull(p.parentcategory,'')='')
	and m.EffDate >= @StartDate and m.EffDate <= @EndDate
	group by isnull(p.code,g.code),f.ChargeOrg
	


    --拼sql
    set @SqlStr = '';

    set @SqlStr = @SqlStr+ ' select fg.ChargeOrg as ChargeOrg,g.Code as CategoryCode,g.Description as CategoryDescription,t1.TransCount as CreateCount,t2.TransCount as EnableCount, '
    set @SqlStr = @SqlStr+ ' t3.TransCount as ApplyCount,t4.TransCount as ReturnCount,t5.TransCount as TransferCount,t6.TransCount as MaintainStartCount, '
    set @SqlStr = @SqlStr+ ' t7.TransCount as MaintainFinishCount,t8.TransCount as FixStartCount,t9.TransCount as FixFinishCount,t10.TransCount as InspectStartCount, '
    set @SqlStr = @SqlStr+ ' t11.TransCount as InspectFinishCount,t12.TransCount as LendCount,t13.TransCount as SellCount,t14.TransCount as EnvelopCount, '
    set @SqlStr = @SqlStr+ ' t15.TransCount as ReOpenCount,t16.TransCount as LoseCount,t17.TransCount as ScrapCount,t18.TransCount as StockTakeCount '
    set @SqlStr = @SqlStr+ ' from fac_facilitycategory g '
    set @SqlStr = @SqlStr+ ' inner join ( select isnull(p.code,g.code) as CategoryCode,f.ChargeOrg as ChargeOrg from  fac_facility f inner join fac_facilitycategory g on f.category = g.code left join fac_facilitycategory p on g.parentcategory = p.code '
	set @SqlStr = @SqlStr+ ' where (isnull(g.parentcategory,'''')='''' or isnull(p.parentcategory,'''')='''') group by isnull(p.code,g.code),f.ChargeOrg)fg on g.Code = fg.CategoryCode '
    set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t1 on g.Code = t1.CategoryCode and fg.ChargeOrg = t1.ChargeOrg and t1.TransType = ''Create'' '
    set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t2 on g.Code = t2.CategoryCode and fg.ChargeOrg = t2.ChargeOrg and t2.TransType = ''Enable'' '
    set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t3 on g.Code = t3.CategoryCode and fg.ChargeOrg = t3.ChargeOrg and t3.TransType = ''Apply'' '
    set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t4 on g.Code = t4.CategoryCode and fg.ChargeOrg = t4.ChargeOrg and t4.TransType = ''Return'' '
    set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t5 on g.Code = t5.CategoryCode and fg.ChargeOrg = t5.ChargeOrg and t5.TransType = ''Transfer'' '
    set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t6 on g.Code = t6.CategoryCode and fg.ChargeOrg = t6.ChargeOrg and t6.TransType = ''MaintainStart'' ' 
    set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t7 on g.Code = t7.CategoryCode and fg.ChargeOrg = t7.ChargeOrg and t7.TransType = ''MaintainFinish'' '
	set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t8 on g.Code = t8.CategoryCode and fg.ChargeOrg = t8.ChargeOrg and t8.TransType = ''FixStart'' '
	set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t9 on g.Code = t9.CategoryCode and fg.ChargeOrg = t9.ChargeOrg and t9.TransType = ''FixFinish'' '
	set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t10 on g.Code = t10.CategoryCode and fg.ChargeOrg = t10.ChargeOrg and t10.TransType = ''InspectStart'' '
	set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t11 on g.Code = t11.CategoryCode and fg.ChargeOrg = t11.ChargeOrg and t11.TransType = ''InspectFinish'' '
	set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t12 on g.Code = t12.CategoryCode and fg.ChargeOrg = t12.ChargeOrg and t12.TransType = ''Lend'' '
	set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t13 on g.Code = t13.CategoryCode and fg.ChargeOrg = t13.ChargeOrg and t13.TransType = ''Sell'' '
	set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t14 on g.Code = t14.CategoryCode and fg.ChargeOrg = t14.ChargeOrg and t14.TransType = ''Envelop'' '
	set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t15 on g.Code = t15.CategoryCode and fg.ChargeOrg = t15.ChargeOrg and t15.TransType = ''Reopen'' '
	set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t16 on g.Code = t16.CategoryCode and fg.ChargeOrg = t16.ChargeOrg and t16.TransType = ''Lose'' '
	set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t17 on g.Code = t17.CategoryCode and fg.ChargeOrg = t17.ChargeOrg and t17.TransType = ''Scrap'' '
	set @SqlStr = @SqlStr+ ' left join #tempCategoryTrans t18 on g.Code = t18.CategoryCode and fg.ChargeOrg = t18.ChargeOrg and t18.TransType = ''StockTake'' '
    set @SqlStr = @SqlStr+ ' where isnull(g.parentcategory,'''')='''' '
    
 	IF(ISNULL(@ChargeOrg,'') <> '')
	BEGIN
		set @SqlStr = @SqlStr+ ' and  fg.ChargeOrg = '''+@ChargeOrg+'''';
	END
	
	IF(ISNULL(@CategoryCode,'') <> '')
	BEGIN
		set @SqlStr = @SqlStr+ ' and  fg.CategoryCode = '''+@CategoryCode+'''';
	END
    exec (@SqlStr)
		
END




/****** Object:  StoredProcedure [dbo].[USP_Rep_FacilityMaintain]    Script Date: 06/25/2014 08:33:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[USP_Rep_FacilityMaintain]
(
		@StartDate datetime,
	    @EndDate datetime,
	    @ChargeOrg varchar(50),
	    @ChargeSite varchar(50),
	    @MaintainType varchar(50)
)
AS
BEGIN

	create table #tempFacilityMaintain
		(
			CTRowId int Identity(1, 1),
			ChargeOrg varchar(100) ,
			ChargeSite varchar(100) ,
			TransType varchar(50) ,
			TransCount decimal(18,8) 
		) 
	
	
    DECLARE @SqlStr varchar(8000);
	
    --- 停机时间
	insert into #tempFacilityMaintain(ChargeOrg,ChargeSite, TransType,TransCount)
	select f.ChargeOrg,f.ChargeSite,'DownTime',sum(DATEDIFF(second,t.StartDate,t.EndDate)) as TransCount
	from fac_facility f
	inner join fac_facilitytrans t on f.fcid = t.fcid
	where t.transtype = 'FixFinish' 
	and t.EffDate >= @StartDate and t.EffDate <= @EndDate
	group by f.ChargeOrg,f.ChargeSite
	
	
	----Fix次数
	insert into  #tempFacilityMaintain(ChargeOrg,ChargeSite, TransType,TransCount)
	select f.ChargeOrg,f.ChargeSite,'Fix',count(1) as TransCount
	from fac_facility f
	inner join fac_facilitytrans t on f.fcid = t.fcid
	where  t.transtype = 'FixFinish' 
	and t.EffDate >= @StartDate and t.EffDate <= @EndDate
	group by f.ChargeOrg,f.ChargeSite
	
	 --- 保养时间
	insert into #tempFacilityMaintain(ChargeOrg,ChargeSite,TransType,TransCount)
	select f.ChargeOrg,f.ChargeSite,'MaintainTime',sum(DATEDIFF(second,t.StartDate,t.EndDate)) as TransCount
	from fac_facility f
	inner join fac_facilitytrans t on f.fcid = t.fcid
	where t.transtype = 'MaintainFinish' 
	and exists (select 1 from fac_facilitymaintainplan p where p.fcid = f.fcid)
	and t.EffDate >= @StartDate and t.EffDate <= @EndDate
	group by f.ChargeOrg,f.ChargeSite
	
    --PlanMaintain(计划保养)
    insert into  #tempFacilityMaintain(ChargeOrg,ChargeSite,TransType,TransCount)
	select f.ChargeOrg,f.ChargeSite,'PlanMaintain',count(1) as TransCount
	from fac_facility f
	inner join isi_taskmstr t on f.fcid =  t.desc2
	where isnull(f.MaintainType,'') != ''
	and t.type = 'Plan' and t.tasksubtype = 'SSGL'
	and exists (select 1 from fac_facilitymaintainplan p where p.fcid = f.fcid)
	and t.PlanStartDate >= @StartDate and dateadd(day,-1,t.PlanStartDate) <= @EndDate
	and t.PlanCompleteDate >= @StartDate and dateadd(day,-1,t.PlanCompleteDate) <= @EndDate
	group by f.ChargeOrg,f.ChargeSite

	--ActualMaintain(实际保养)
    insert into  #tempFacilityMaintain(ChargeOrg,ChargeSite, TransType,TransCount)
	select f.ChargeOrg,f.ChargeSite,'ActualMaintain',count(1) as TransCount
	from fac_facility f
	inner join fac_facilitytrans t on f.fcid = t.fcid
    and exists (select 1 from fac_facilitymaintainplan p where p.fcid = f.fcid)
	where  t.transtype in ('MaintainFinish' ,'InspectFinish')
	and t.EffDate >= @StartDate and t.EffDate <= @EndDate
	group by f.ChargeOrg,f.ChargeSite


    --拼最后的SQL    
    set @SqlStr = '';

    set @SqlStr = @SqlStr+ ' select f.ChargeOrg as ChargeOrg,f.ChargeSite as ChargeSite, f.MaintainType as MaintainType, f.FacilityCount as FacilityCount, '
    set @SqlStr = @SqlStr+ ' case when isnull(Cast(t1.TransCount as varchar),'''') = '''' then 0 else t1.TransCount/3600 end as DownTime, '
    set @SqlStr = @SqlStr+ ' case when isnull(Cast(t2.TransCount as varchar),'''') = '''' then 0 else t2.TransCount end as FixCount, '
    set @SqlStr = @SqlStr+ ' case when isnull(Cast(t5.TransCount as varchar),'''') = '''' then 0 else t5.TransCount end as MaintainTime, '
    set @SqlStr = @SqlStr+ ' case when isnull(Cast(t3.TransCount as varchar),'''') = '''' then 0 else t3.TransCount end as PlanMaintainCount, '
    set @SqlStr = @SqlStr+ ' case when isnull(Cast(t4.TransCount as varchar),'''') = '''' then 0 else t4.TransCount end as ActualMaintainCount, '
    set @SqlStr = @SqlStr+ ' case when isnull(Cast(t4.TransCount as varchar),'''') = '''' then 0 else 100 * (case when isnull(Cast(t4.TransCount as varchar),'''') = '''' then 0 else t4.TransCount end) / t3.TransCount  end as MaintainRate '
    set @SqlStr = @SqlStr+ ' from (select f.ChargeOrg as ChargeOrg,f.ChargeSite as ChargeSite,f.MaintainType as MaintainType,count(1) as FacilityCount from fac_facility f where f.Status not in (''Scrap'',''Sell'',''Lose'') group by f.ChargeOrg,f.ChargeSite,f.MaintainType) f '
    set @SqlStr = @SqlStr+ ' left join #tempFacilityMaintain t1 on f.ChargeOrg = t1.ChargeOrg and f.ChargeSite = t1.ChargeSite and t1.TransType = ''DownTime'' '
    set @SqlStr = @SqlStr+ ' left join #tempFacilityMaintain t2 on f.ChargeOrg = t2.ChargeOrg and f.ChargeSite = t2.ChargeSite and t2.TransType = ''Fix'' '
    set @SqlStr = @SqlStr+ ' left join #tempFacilityMaintain t3 on f.ChargeOrg = t3.ChargeOrg and f.ChargeSite = t3.ChargeSite and t3.TransType = ''PlanMaintain'' '
    set @SqlStr = @SqlStr+ ' left join #tempFacilityMaintain t4 on f.ChargeOrg = t4.ChargeOrg and f.ChargeSite = t4.ChargeSite and t4.TransType = ''ActualMaintain'' '
    set @SqlStr = @SqlStr+ ' left join #tempFacilityMaintain t5 on f.ChargeOrg = t5.ChargeOrg and f.ChargeSite = t5.ChargeSite and t5.TransType = ''MaintainTme'' '
    set @SqlStr = @SqlStr+ ' where 1 = 1 '
   
   	IF(ISNULL(@ChargeOrg,'') <> '')
	BEGIN
		set @SqlStr = @SqlStr+ ' and  f.ChargeOrg = '''+@ChargeOrg+'''';
	END
	
	IF(ISNULL(@ChargeSite,'') <> '')
	BEGIN
		set @SqlStr = @SqlStr+ ' and  f.ChargeSite = '''+@ChargeSite+'''';
	END
	
	IF(ISNULL(@MaintainType,'') <> '')
	BEGIN
		set @SqlStr = @SqlStr+ ' and  f.MaintainType = '''+@MaintainType+'''';
	END
	
    exec (@SqlStr)
END



GO

--增加日志备注字段长度
alter table fac_facilitytrans alter column remark varchar(max)



insert into acc_menu values ('Menu.Facility.MouldMaster',1,'模具','~/Main.aspx?mid=Facility.MouldMaster',1,'~/Images/Nav/Item.png',getdate(),'su',getdate(),'su',null)
insert into acc_permission (pm_code,pm_desc,pm_catecode) values ('Menu.Facility.MouldMaster','模具','Facility')

insert into ACC_MenuCommon (Menu, ParentMenu, Level_, Seq, IsActive, CreateDate, CreateUser, LastModifyDate, LastModifyUser)
values ('Menu.Facility.MouldMaster','Menu.Facility.Trans',3,11,1,getdate(),'su',getdate(),'su')

insert into CodeMstr values('ISICheckupProjectType','Cadre',10,0,'干部')
insert into CodeMstr values('ISICheckupProjectType','Employee',10,0,'员工')
insert into CodeMstr values('ISICheckupProjectType','General',10,0,'通用')
insert into CodeMstr values('ISICheckupStatus','Approval',10,0,'批准')
insert into CodeMstr values('ISICheckupStatus','Cancel',10,0,'取消')
insert into CodeMstr values('ISICheckupStatus','Close',10,0,'关闭')
insert into CodeMstr values('ISICheckupStatus','Create',10,0,'创建')
insert into CodeMstr values('ISICheckupStatus','Submit',10,0,'提交')
insert into CodeMstr values('ISIColor','green',10,0,'绿')
insert into CodeMstr values('ISIColor','red',10,0,'红')
insert into CodeMstr values('ISIColor','yellow',10,0,'黄')
insert into CodeMstr values('ISIDepartment','本部',10,0,'本部')
insert into CodeMstr values('ISIDepartment','承运商',10,0,'承运商')
insert into CodeMstr values('ISIDepartment','春申',10,0,'春申')
insert into CodeMstr values('ISIDepartment','供应商',10,0,'供应商')
insert into CodeMstr values('ISIDepartment','广州延鑫',10,0,'广州延鑫')
insert into CodeMstr values('ISIDepartment','极纺',10,0,'极纺')
insert into CodeMstr values('ISIDepartment','杰康',10,0,'杰康')
insert into CodeMstr values('ISIDepartment','恺杰',10,0,'恺杰')
insert into CodeMstr values('ISIDepartment','客户',10,0,'客户')
insert into CodeMstr values('ISIDepartment','如皋延康',10,0,'如皋延康')
insert into CodeMstr values('ISIDepartment','上海延康',10,0,'上海延康')
insert into CodeMstr values('ISIDepartment','上海延鑫',10,0,'上海延鑫')
insert into CodeMstr values('ISIDepartment','芜湖江森',10,0,'芜湖江森')
insert into CodeMstr values('ISIDepartment','延华',10,0,'延华')
insert into CodeMstr values('ISIDept2','财务',10,0,'财务')
insert into CodeMstr values('ISIDept2','采购',10,0,'采购')
insert into CodeMstr values('ISIDept2','产品',10,0,'产品')
insert into CodeMstr values('ISIDept2','党工办',10,0,'党工办')
insert into CodeMstr values('ISIDept2','服务',10,0,'服务')
insert into CodeMstr values('ISIDept2','工厂工程',10,0,'工厂工程')
insert into CodeMstr values('ISIDept2','经理室',10,0,'经理室')
insert into CodeMstr values('ISIDept2','客户部',10,0,'客户部')
insert into CodeMstr values('ISIDept2','人事行政',10,0,'人事行政')
insert into CodeMstr values('ISIDept2','商贸',10,0,'商贸')
insert into CodeMstr values('ISIDept2','市场',10,0,'市场')
insert into CodeMstr values('ISIDept2','物控部',10,0,'物控部')
insert into CodeMstr values('ISIDept2','物流',10,0,'物流')
insert into CodeMstr values('ISIDept2','项目',10,0,'项目')
insert into CodeMstr values('ISIDept2','信息',10,0,'信息')
insert into CodeMstr values('ISIDept2','执委会',10,0,'执委会')
insert into CodeMstr values('ISIDept2','制造',10,0,'制造')
insert into CodeMstr values('ISIDept2','质量',10,0,'质量')
insert into CodeMstr values('ISIECType','EC',10,0,'常规')
insert into CodeMstr values('ISIFlag','DI1',10,0,'DI1')
insert into CodeMstr values('ISIFlag','DI2',10,0,'DI2')
insert into CodeMstr values('ISIFlag','DI3',10,0,'DI3')
insert into CodeMstr values('ISIFlag','DI4',10,0,'DI4')
insert into CodeMstr values('ISIFlag','DI5',10,0,'DI5')
insert into CodeMstr values('ISIFlag','Plan',10,0,'计划')
insert into CodeMstr values('ISIPhase','PS00',10,0,'阶段0')
insert into CodeMstr values('ISIPhase','PS01',10,0,'阶段1')
insert into CodeMstr values('ISIPhase','PS02',10,0,'阶段2')
insert into CodeMstr values('ISIPhase','PS03',10,0,'阶段3')
insert into CodeMstr values('ISIPhase','PS04',10,0,'阶段4')
insert into CodeMstr values('ISIPhase','PS05',10,0,'阶段5')
insert into CodeMstr values('ISIPosition','班组长',10,0,'班组长')
insert into CodeMstr values('ISIPosition','办公室小时工',10,0,'办公室小时工')
insert into CodeMstr values('ISIPosition','承运商',10,0,'承运商')
insert into CodeMstr values('ISIPosition','二线小时工',10,0,'二线小时工')
insert into CodeMstr values('ISIPosition','高层',10,0,'高层')
insert into CodeMstr values('ISIPosition','供应商',10,0,'供应商')
insert into CodeMstr values('ISIPosition','客户',10,0,'客户')
insert into CodeMstr values('ISIPosition','劳务工',10,0,'劳务工')
insert into CodeMstr values('ISIPosition','平台经理',10,0,'平台经理')
insert into CodeMstr values('ISIPosition','企业负责人',10,0,'企业负责人')
insert into CodeMstr values('ISIPosition','企业书记',10,0,'企业书记')
insert into CodeMstr values('ISIPosition','条线经理',10,0,'条线经理')
insert into CodeMstr values('ISIPosition','薪水工',10,0,'薪水工')
insert into CodeMstr values('ISIPosition','一线小时工',10,0,'一线小时工')
insert into CodeMstr values('ISIPosition','主管',10,0,'主管')
insert into CodeMstr values('ISIPriority','High',10,0,'高')
insert into CodeMstr values('ISIPriority','Low',10,0,'低')
insert into CodeMstr values('ISIPriority','Normal',10,0,'正常')
insert into CodeMstr values('ISIPriority','Urgent',10,0,'紧急')
insert into CodeMstr values('ISIProjectSubType','Initiation',10,0,'开发')
insert into CodeMstr values('ISIProjectSubType','Quote',10,0,'报价')
insert into CodeMstr values('ISIProjectType','BD',10,0,'基建项目')
insert into CodeMstr values('ISIProjectType','IT',10,0,'信息技术项目')
insert into CodeMstr values('ISIProjectType','PE',10,0,'产品项目')
insert into CodeMstr values('ISIResRoleType','Direct',10,0,'一线')
insert into CodeMstr values('ISIResRoleType','Indirect',10,0,'二线')
insert into CodeMstr values('ISIResRoleType','Management',10,0,'管理')
insert into CodeMstr values('ISIResRoleType','Office',10,0,'白领')
insert into CodeMstr values('ISISendStatus','Fail',10,0,'失败')
insert into CodeMstr values('ISISendStatus','NotSend',10,0,'未发送')
insert into CodeMstr values('ISISendStatus','Success',10,0,'成功')
insert into CodeMstr values('ISISkillLevel','A',10,0,'精通')
insert into CodeMstr values('ISISkillLevel','B',10,0,'熟练')
insert into CodeMstr values('ISISkillLevel','C',10,0,'一般')
insert into CodeMstr values('ISISkillLevel','D',10,0,'实习')
insert into CodeMstr values('ISISkillLevel','E',10,0,'不会')
insert into CodeMstr values('ISISkillLevel','O',10,0,'')
insert into CodeMstr values('ISIStatus','Approve',10,0,'批准')
insert into CodeMstr values('ISIStatus','Assign',10,0,'分派')
insert into CodeMstr values('ISIStatus','Cancel',10,0,'取消')
insert into CodeMstr values('ISIStatus','Close',10,0,'关闭')
insert into CodeMstr values('ISIStatus','Complete',10,0,'完成')
insert into CodeMstr values('ISIStatus','Create',10,0,'创建')
insert into CodeMstr values('ISIStatus','In-Approve',10,0,'审批中')
insert into CodeMstr values('ISIStatus','In-Dispute',10,0,'审批中(异议)')
insert into CodeMstr values('ISIStatus','In-Process',10,0,'执行中')
insert into CodeMstr values('ISIStatus','Refuse',10,0,'不批准')
insert into CodeMstr values('ISIStatus','Return',10,0,'退回')
insert into CodeMstr values('ISIStatus','Submit',10,0,'提交')
insert into CodeMstr values('ISISummaryStatus','Approval',10,0,'批准')
insert into CodeMstr values('ISISummaryStatus','Cancel',10,0,'取消')
insert into CodeMstr values('ISISummaryStatus','Close',10,0,'关闭')
insert into CodeMstr values('ISISummaryStatus','Create',10,0,'创建')
insert into CodeMstr values('ISISummaryStatus','In-Approve',10,0,'审批中')
insert into CodeMstr values('ISISummaryStatus','Submit',10,0,'提交')
insert into CodeMstr values('ISISummaryType','Excellent',10,0,'优')
insert into CodeMstr values('ISISummaryType','Moderate',10,0,'良')
insert into CodeMstr values('ISISummaryType','Poor',10,0,'需改进')
insert into CodeMstr values('ISITemplate','WFS.xls',10,0,'审批单')
insert into CodeMstr values('ISIType','Audit',10,0,'审核')
insert into CodeMstr values('ISIType','Change',10,0,'变化')
insert into CodeMstr values('ISIType','Enc',10,0,'工程更改')
insert into CodeMstr values('ISIType','General',10,0,'通用')
insert into CodeMstr values('ISIType','Improve',10,0,'改进')
insert into CodeMstr values('ISIType','Issue',10,0,'问题')
insert into CodeMstr values('ISIType','Plan',10,0,'计划')
insert into CodeMstr values('ISIType','Privacy',10,0,'密报')
insert into CodeMstr values('ISIType','PrjIss',10,0,'项目问题')
insert into CodeMstr values('ISIType','Project',10,0,'项目')
insert into CodeMstr values('ISIType','ResMatrix',10,0,'责任方阵')
insert into CodeMstr values('ISIType','Response',10,0,'响应')
insert into CodeMstr values('ISIType','WFS',10,0,'流程')


update ISI_TaskSubType set IsAutoAssign=1,IsReport=1,IsOpen=1,IsEC=1,IsAutoStart=0,IsAutoComplete=0,IsAutoClose=0,IsAutoStatus=0,IsAssignUser=1,IsTrace=1;
insert into CodeMstr values('FacilityStatus','InUse',25,0,'使用中');