-- select * from pg_tables;
-- delete from articles;
-- select count(*) from articles;
-- select * from articles limit 10000;
--select * from articles where tokencount < 1000 order by tokencount;

--live feed articles
-- select *
-- from articles where id IN (
--     '10000000-efa7-4200-6420-000000000000',
--     '10000000-efa7-4200-6376-200000000000',
--     '10000000-efa7-4200-6374-500000000000',
--     '10000000-efa7-4200-6373-200000000000',
--     '10000000-efa7-4200-6323-200000000000',
--     '10000000-efa7-4200-6300-100000000000',
--     '10000000-efa7-4200-6301-100000000000',
--     '10000000-efa7-4200-6203-800000000000',
--     '10000000-efa7-4200-6184-100000000000',
--     '10000000-efa7-4200-6168-500000000000',
--     '10000000-efa7-4200-6140-800000000000',
--     '10000000-efa7-4200-6124-100000000000',
--     '10000000-efa7-4200-6094-900000000000',
--     '10000000-efa7-4200-6090-800000000000',
--     '10000000-efa7-4200-6076-300000000000',
--     '10000000-efa7-4200-6050-400000000000',
--     '10000000-efa7-4200-5993-100000000000',
--     '10000000-efa7-4200-5962-400000000000',
--     '10000000-efa7-4200-5956-500000000000',
--     '10000000-efa7-4200-5917-200000000000'
-- )

-- "id","tokencount","wordcount","isvalid"
-- "10000000-efa7-4200-5917-200000000000",973,-1,""
-- "10000000-efa7-4200-5956-500000000000",733,-1,""
-- "10000000-efa7-4200-5962-400000000000",285,-1,""
-- "10000000-efa7-4200-5993-100000000000",131,-1,""
-- "10000000-efa7-4200-6050-400000000000",1060,-1,""
-- "10000000-efa7-4200-6076-300000000000",888,-1,""
-- "10000000-efa7-4200-6090-800000000000",583,-1,""
-- "10000000-efa7-4200-6094-900000000000",806,-1,""
-- "10000000-efa7-4200-6124-100000000000",2674,-1,""
-- "10000000-efa7-4200-6140-800000000000",564,-1,""
-- "10000000-efa7-4200-6168-500000000000",344,-1,""
-- "10000000-efa7-4200-6184-100000000000",798,-1,""
-- "10000000-efa7-4200-6203-800000000000",717,-1,""
-- "10000000-efa7-4200-6300-100000000000",1277,-1,""
-- "10000000-efa7-4200-6301-100000000000",1512,-1,""
-- "10000000-efa7-4200-6323-200000000000",98,-1,""
-- "10000000-efa7-4200-6373-200000000000",1053,-1,""
-- "10000000-efa7-4200-6374-500000000000",2081,-1,""
-- "10000000-efa7-4200-6376-200000000000",452,-1,""
-- "10000000-efa7-4200-6420-000000000000",289,-1,""
