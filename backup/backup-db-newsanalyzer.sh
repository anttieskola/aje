#!/bin/bash
pg_dump -U antti -F t newsanalyzer | bzip2 > db_newsanalyzer.tar.bz2

