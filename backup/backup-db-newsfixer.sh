#!/bin/bash
rm ~/aje-backup/db_newsfixer.tar.bz2
pg_dump -U antti -F t newsfixer | bzip2 > ~/aje-backup/db_newsfixer.tar.bz2

