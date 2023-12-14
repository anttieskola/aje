-- select sp.source
-- from sentimentpolarities sp
-- group by sp.source
-- HAVING count(*) > 1;

--select * from sentimentpolarities order by serial desc;

select * from analyses LIMIT 1000;
--select * from analyses where id = '10000000-efa7-4200-6442-700000000000';

-- SELECT column_name, data_type, character_maximum_length
-- FROM information_schema.columns
-- WHERE table_name = 'sentimentpolarities';

-- set serial to max value (after restore/copy of data)
-- SELECT setval('sentimentpolarities_serial_seq', (SELECT MAX(serial) FROM sentimentpolarities));

