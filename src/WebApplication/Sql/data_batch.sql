--BatchJobDet
--BatchTrigger
--BatchTriggerParam
--BatchJobParam


set identity_insert BatchJobDet on;
INSERT INTO "BatchJobDet" (Id,Name,Desc1,ServiceName) VALUES (1,'LeanEngineJob','Job of Automatic Generate Orders','LeanEngineJob')
INSERT INTO "BatchJobDet" (Id,Name,Desc1,ServiceName) VALUES (2,'OrderCloseJob','Job of Automatic Close Orders','OrderCloseJob')
INSERT INTO "BatchJobDet" (Id,Name,Desc1,ServiceName) VALUES (3,'WOBackflushJob','Job of Automatic Backflush Orders','WOBackflushJob')
INSERT INTO "BatchJobDet" (Id,Name,Desc1,ServiceName) VALUES (5,'MRPJob','Job of MRP','MRPJob')
set identity_insert BatchJobDet off;

set identity_insert BatchTrigger on;
INSERT INTO "BatchTrigger" (Id,Name,Desc1,JobId,NextFireTime,PrevFireTime,RepeatCount,Interval,IntervalType,TimesTriggered,Status) VALUES (1,'LeanEngineTrigger','Trigger of Automatic Generate Orders',1,'2010-08-12 13:09:42','2010-07-23 13:40:00',0,10,'Minutes',2944,'Pause')
INSERT INTO "BatchTrigger" (Id,Name,Desc1,JobId,NextFireTime,PrevFireTime,RepeatCount,Interval,IntervalType,TimesTriggered,Status) VALUES (2,'OrderCloseTrigger','Trigger of Automatic Close Orders',2,'2010-08-12 13:09:42','2010-07-06 00:00:00',0,1,'Days',2,'Pause')
INSERT INTO "BatchTrigger" (Id,Name,Desc1,JobId,NextFireTime,PrevFireTime,RepeatCount,Interval,IntervalType,TimesTriggered,Status) VALUES (3,'WOBackflushTrigger','Trigger of WO Backflush',3,'2011-05-13 05:30:00.000','2011-05-12 05:30:00.000',0,1,'Days',88,'Pause')
INSERT INTO "BatchTrigger" (Id,Name,Desc1,JobId,NextFireTime,PrevFireTime,RepeatCount,Interval,IntervalType,TimesTriggered,Status) VALUES (5,'MRPTrigger','Trigger of MRP',5,'2011-05-13 05:30:00.000','2011-05-12 05:30:00.000',0,1,'Days',88,'Pause')
set identity_insert BatchTrigger off;


set identity_insert BatchTriggerParam on;
set identity_insert BatchTriggerParam off;