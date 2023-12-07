-- select * from pg_tables;
select id, reasoning from articles
where "isValid" = false;