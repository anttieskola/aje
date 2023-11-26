select sp.source
from sentimentpolarities sp
group by sp.source
HAVING count(*) > 1;
