select sp."Id"
from sentimentpolarities sp
group by sp."Id"
HAVING count(*) > 1;
