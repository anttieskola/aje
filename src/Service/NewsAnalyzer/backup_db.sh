#!/bin/bash
pg_dump -U antti -F t newsanalyzer | gzip > backup_newsanalyzer.tar.gz

