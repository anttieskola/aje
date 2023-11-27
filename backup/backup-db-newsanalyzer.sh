#!/bin/bash
rm ~/aje-backup/db_newsanalyzer.tar.bz2
pg_dump -U antti -F t newsanalyzer | bzip2 > ~/aje-backup/db_newsanalyzer.tar.bz2

