# Notes on sentiment analysis
Currently just wanting polarity
- Positive / Neutral / Negative

# Learning to classify content
Atleats for starters using models that work with [ChatML syntax](https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/chatgpt?pivots=programming-language-chat-ml#working-with-chat-markup-language-chatml)
or similar that only uses just different tags/elements for the same thing


## Basic layout
```chatml
<|im_start|>system
{instructions how we want the model to respond to the classification}
<|im_end|>
<|im_start|>system
Context: {selecting content from article here}
<|im_end|>
<|im_start|>assistant classify provided context
```

Mistral Instruct difference here is just that tags are
- `<|im_start|>` -> `[INST]`
- `<|im_end|>` -> `[/INST]`

## Models
- Mistral
    - [Instruct version](https://huggingface.co/mistralai/Mistral-7B-Instruct-v0.1)
    - [OpenOrca version](https://huggingface.co/Open-Orca/Mistral-7B-OpenOrca)

## Negative++

### ChatML
```
<|im_start|>system \
Clear Context \
<|im_end|> \
<|im_start|>system \
You are an assistant that responds using sentences \
<|im_end|> \
<|im_start|>system \
Context \
The court has ordered that the identities of the victims be kept secret, due to the sensitive issues related to the cases. \
The prosecutor's office has filed charges in the case against Aleksanteri Kivimäki, who is suspected of hacking into a mental health firm's database and then extorting his victims. \
Due to the massive scale of the case, prosecutors held a press conference on Wednesday to announce the charges that were being filed against the suspect. Wednesday was the deadline for charges to be filed. \
Kivimäki faces charges of aggravated blackmail and its attempt, aggravated data trespass, as well as aggravated dissemination of information violating personal privacy. \
The suspect, who has previously used the name Julius Kivimäki, has denied all the allegations. Kivimäki is suspected of hacking a patient record database belonging to the psychotherapy centre Vastaamo, aiming to blackmail tens of thousands of victims. \
He is suspected of stealing the sensitive personal data of more than 33,000 of the therapy centre's clients and then posting them on the dark web. Around two-thirds of the victims filed criminal reports with authorities. \
Around 21,000 of the clients filed reports of attempted aggravated extortion, while around 9,600 filed reports of aggravated dissemination of information violating personal privacy. \
Meanwhile 20 of the affected clients claimed they were victims of aggravated extortion. \
The difference between extortion and aggravated extortion is determined by whether the victim paid the perpetrator or not. \
The court has ordered that the identities of the victims be kept secret, due to the sensitive issues related to the cases. \
However, the court did make the exception of allowing the announcement of charges at this stage of the legal proceedings. Usually criminal charges are not publicly announced until a case's preparatory session or when charges are read at the beginning of a criminal trial. \
The District Court of Western Uusimaa said the trial would begin on 13 November and continue until the end of February 2024. The court noted the case was exceptional in terms of its content and scale. \
Kivimäki has been in remand custody since February, after being extradited from France. \
Vastaamo first announced the breach in October 2020. The company's board fired its CEO Ville Tapio shortly after that, accusing him of keeping the breach a secret for a year and a half. \
Vastaamo's owners, which included Tapio and members of his family, sold their majority stake in the company to a private equity firm before news of the breach was announced. Tax data showed that the sell-off made millionaires of the family of owners. \
The firm struggled for several months with repercussions of the scandal, before filing for bankruptcy in February 2021. \
In April of this year, Tapio was handed a three-month suspended sentence for a data protection crime. Both the former CEO and the prosecutor appealed the sentence in May. \
<|im_end|> \
<|im_start|>user \
please classify the context \
<|im_end> \
<|im_start|>assistant
```

```
<|im_start|>system \
Clear Context \
<|im_end|> \
<|im_start|>system \
You are an assistant that responds using only one word \
<|im_end|> \
<|im_start|>system \
Context \
The court has ordered that the identities of the victims be kept secret, due to the sensitive issues related to the cases. \
The prosecutor's office has filed charges in the case against Aleksanteri Kivimäki, who is suspected of hacking into a mental health firm's database and then extorting his victims. \
Due to the massive scale of the case, prosecutors held a press conference on Wednesday to announce the charges that were being filed against the suspect. Wednesday was the deadline for charges to be filed. \
Kivimäki faces charges of aggravated blackmail and its attempt, aggravated data trespass, as well as aggravated dissemination of information violating personal privacy. \
The suspect, who has previously used the name Julius Kivimäki, has denied all the allegations. Kivimäki is suspected of hacking a patient record database belonging to the psychotherapy centre Vastaamo, aiming to blackmail tens of thousands of victims. \
He is suspected of stealing the sensitive personal data of more than 33,000 of the therapy centre's clients and then posting them on the dark web. Around two-thirds of the victims filed criminal reports with authorities. \
Around 21,000 of the clients filed reports of attempted aggravated extortion, while around 9,600 filed reports of aggravated dissemination of information violating personal privacy. \
Meanwhile 20 of the affected clients claimed they were victims of aggravated extortion. \
The difference between extortion and aggravated extortion is determined by whether the victim paid the perpetrator or not. \
The court has ordered that the identities of the victims be kept secret, due to the sensitive issues related to the cases. \
However, the court did make the exception of allowing the announcement of charges at this stage of the legal proceedings. Usually criminal charges are not publicly announced until a case's preparatory session or when charges are read at the beginning of a criminal trial. \
The District Court of Western Uusimaa said the trial would begin on 13 November and continue until the end of February 2024. The court noted the case was exceptional in terms of its content and scale. \
Kivimäki has been in remand custody since February, after being extradited from France. \
Vastaamo first announced the breach in October 2020. The company's board fired its CEO Ville Tapio shortly after that, accusing him of keeping the breach a secret for a year and a half. \
Vastaamo's owners, which included Tapio and members of his family, sold their majority stake in the company to a private equity firm before news of the breach was announced. Tax data showed that the sell-off made millionaires of the family of owners. \
The firm struggled for several months with repercussions of the scandal, before filing for bankruptcy in February 2021. \
In April of this year, Tapio was handed a three-month suspended sentence for a data protection crime. Both the former CEO and the prosecutor appealed the sentence in May. \
<|im_end|> \
<|im_start|>user \
please classify the context using single word that is either negative, neutral or positive \
<|im_end> \
<|im_start|>assistant
```

### Instruct
```
[INST]system \
Clear Context \
[/INST] \
[INST]system \
Please classify provided context to your best ability \
[/INST] \
[INST]system \
Context \
The court has ordered that the identities of the victims be kept secret, due to the sensitive issues related to the cases. \
The prosecutor's office has filed charges in the case against Aleksanteri Kivimäki, who is suspected of hacking into a mental health firm's database and then extorting his victims. \
Due to the massive scale of the case, prosecutors held a press conference on Wednesday to announce the charges that were being filed against the suspect. Wednesday was the deadline for charges to be filed. \
Kivimäki faces charges of aggravated blackmail and its attempt, aggravated data trespass, as well as aggravated dissemination of information violating personal privacy. \
The suspect, who has previously used the name Julius Kivimäki, has denied all the allegations. Kivimäki is suspected of hacking a patient record database belonging to the psychotherapy centre Vastaamo, aiming to blackmail tens of thousands of victims. \
He is suspected of stealing the sensitive personal data of more than 33,000 of the therapy centre's clients and then posting them on the dark web. Around two-thirds of the victims filed criminal reports with authorities. \
Around 21,000 of the clients filed reports of attempted aggravated extortion, while around 9,600 filed reports of aggravated dissemination of information violating personal privacy. \
Meanwhile 20 of the affected clients claimed they were victims of aggravated extortion. \
The difference between extortion and aggravated extortion is determined by whether the victim paid the perpetrator or not. \
The court has ordered that the identities of the victims be kept secret, due to the sensitive issues related to the cases. \
However, the court did make the exception of allowing the announcement of charges at this stage of the legal proceedings. Usually criminal charges are not publicly announced until a case's preparatory session or when charges are read at the beginning of a criminal trial. \
The District Court of Western Uusimaa said the trial would begin on 13 November and continue until the end of February 2024. The court noted the case was exceptional in terms of its content and scale. \
Kivimäki has been in remand custody since February, after being extradited from France. \
Vastaamo first announced the breach in October 2020. The company's board fired its CEO Ville Tapio shortly after that, accusing him of keeping the breach a secret for a year and a half. \
Vastaamo's owners, which included Tapio and members of his family, sold their majority stake in the company to a private equity firm before news of the breach was announced. Tax data showed that the sell-off made millionaires of the family of owners. \
The firm struggled for several months with repercussions of the scandal, before filing for bankruptcy in February 2021. \
In April of this year, Tapio was handed a three-month suspended sentence for a data protection crime. Both the former CEO and the prosecutor appealed the sentence in May. \
[/INST] \
[INST]user classify context
```

```
[INST]system \
Clear Context \
[/INST] \
[INST]system \
Please classify provided context with single word that is either negative, neutral or positive \
[/INST] \
[INST]system \
Context \
The court has ordered that the identities of the victims be kept secret, due to the sensitive issues related to the cases. \
The prosecutor's office has filed charges in the case against Aleksanteri Kivimäki, who is suspected of hacking into a mental health firm's database and then extorting his victims. \
Due to the massive scale of the case, prosecutors held a press conference on Wednesday to announce the charges that were being filed against the suspect. Wednesday was the deadline for charges to be filed. \
Kivimäki faces charges of aggravated blackmail and its attempt, aggravated data trespass, as well as aggravated dissemination of information violating personal privacy. \
The suspect, who has previously used the name Julius Kivimäki, has denied all the allegations. Kivimäki is suspected of hacking a patient record database belonging to the psychotherapy centre Vastaamo, aiming to blackmail tens of thousands of victims. \
He is suspected of stealing the sensitive personal data of more than 33,000 of the therapy centre's clients and then posting them on the dark web. Around two-thirds of the victims filed criminal reports with authorities. \
Around 21,000 of the clients filed reports of attempted aggravated extortion, while around 9,600 filed reports of aggravated dissemination of information violating personal privacy. \
Meanwhile 20 of the affected clients claimed they were victims of aggravated extortion. \
The difference between extortion and aggravated extortion is determined by whether the victim paid the perpetrator or not. \
The court has ordered that the identities of the victims be kept secret, due to the sensitive issues related to the cases. \
However, the court did make the exception of allowing the announcement of charges at this stage of the legal proceedings. Usually criminal charges are not publicly announced until a case's preparatory session or when charges are read at the beginning of a criminal trial. \
The District Court of Western Uusimaa said the trial would begin on 13 November and continue until the end of February 2024. The court noted the case was exceptional in terms of its content and scale. \
Kivimäki has been in remand custody since February, after being extradited from France. \
Vastaamo first announced the breach in October 2020. The company's board fired its CEO Ville Tapio shortly after that, accusing him of keeping the breach a secret for a year and a half. \
Vastaamo's owners, which included Tapio and members of his family, sold their majority stake in the company to a private equity firm before news of the breach was announced. Tax data showed that the sell-off made millionaires of the family of owners. \
The firm struggled for several months with repercussions of the scandal, before filing for bankruptcy in February 2021. \
In April of this year, Tapio was handed a three-month suspended sentence for a data protection crime. Both the former CEO and the prosecutor appealed the sentence in May. \
[/INST] \
[INST]user classify context
```

## Negative

# Odd model responses

## dramatic 74-20054239
The race to become Veikkausliiga champions will go right down to the wire following a dramatic day of twists and turns on Sunday.The destination of this years Veikkausliiga football league title will be decided on the final day of the season, following a dramatic afternoon of twists and turns on Sunday.Defending champions HJK looked to have secured their 33rd championship after fighting back from 2-0 to draw level against VPS in Vaasa.One point from the game would have been enough to clinch the title for HJK, but a goal by Nigerian striker Peter Godly Michael in the 94th minute gave VPS a 3-2 win and means this years race will go right down to the wire.Thats because HJKs nearest rivals KuPs won their penultimate game against FC Honka on a scoreline of 3-1, setting up a mouth-watering, title-deciding final game against HJK on 21 October.Sundays results mean HJK are on top of the standings with 53 points and a goal difference of 25. KuPs sit in second place on 50 points, with a goal difference of 20.If KuPs were to win the game 3-0, they would pip HJK to the championship in what would be one of the most dramatic ends to a Veikkausliiga season in Finnish football history.Although unlikely, such a result is not impossible, according to Yle Sports football pundit Antti Pohja.Fellow analyst Sebastian Sorsa agreed, noting that HJK will be feeling the pressure having failed to secure the point needed on Sunday.HJKs experienced players will need to show character and take control of the situation. Still, HJK will certainly be nervous, Sorsa said.Peter Godly Michaels winning goal against HJK  which he later described to Yle as the best moment of his career  also secured a third place finish for VPS, a feat all the more impressive considering the Vaasa-based team were bottom of the league after 12 games.The championship decider between HJK and KuPs will be broadcast on Yle TV2 and Yle Areena on Saturday 21 October. Kick-off is at 5pm.Would you like a roundup of the weeks top stories in your inbox every Thursday Then sign up to receive our weekly email.

## challenging 74-20053674
Parents can find it hard to get their paperwork in order when they move to Finland with Finnish children.Helsingin Sanomat has a story about a mother who has moved to Finland with her 12-year-old Finnish-German dual citizen son  but cannot get her residency rights approved by the immigration authorities.Isabel moved to Helsinki with her son Max. Shes separated from the boys Finnish father, who remained in Germany, and wanted to move to Helsinki to give the boy better exposure to his Finnish roots and the Finnish language.Isabel is in the film industry and currently between projects, after her previous job ended in August.Max started at the German school in Helsinki and is enjoying it, but his mother has been unable to claim residency rights for herself.The immigration authorities have told her that she would need to have lived with her son in a third EU country, so that he would have exercised his right to free movement under EU legislation, to have automatic rights to residency in Finland.Migri also told her she should not officially move to Helsinki but instead spend 90 days at a time in the city, spending a day in Tallinn every three months to avoid exceeding the maximum time allowed in the country.Her son, therefore, would officially live alone in Finland. Authorities have told her verbally that EU freedom of movement rights dont apply to her, they might reject her application, and after that she could apply via the process for non-EU citizens  and she would then be accepted.Russias assault on Ukraine forced a shakeup in many parts of Finnish society, not least ice hockey. The countrys biggest club, Jokerit, found their Russian-owned arena and place in Russias KHL untenable, and left that competition.They are now heading back to the Finnish league, via the second tier competition Mestis. Although their home arena is not usable due to its sanctioned oligarch owners, Iltalehti reports that they are set to break an attendance record on Friday when they play at Helsinki ice hall.On Wednesday the club announced they had sold some 7,300 tickets for the game against Kiekko-Vantaa.That beats the previous record of just under 7,000, which was set by Kiekko-Espoo when Jokerit played them earlier this season.Ilta-Sanomat says that the unseasonably warm conditions Finland has basked in at the end of September may be coming to a close.For the next couple of weeks there will be colder than normal weather in the north, while the south will have typical autumnal conditions  so cooler than the summery weather weve had in September.There could even be a touch of frost overnight leading to Friday morning, offering a hint of what Finnish winter has to offer over the coming months.

## preparedness 74-20052916
Various government agencies are testing their readiness in the face of a sudden wave of migrants.This week the Finnish Border Guard is leading a major joint preparedness exercise focused on a major migrant influx into Finland.The exercise, called Latu 23, tests how Finland would cope with a significant number of people arriving at its borders in a short period of time.The exercise involves the authorities trying to manage a situation where a large number of migrants arrive in a short period, said Jussi Napola of the Finnish Border Guard, who added that combating hybrid influence is another priority.While led by the Finnish Border Guard, the effort includes representatives from the Defense Forces, the Finnish Immigration Service, ministries and regional Centres for Economic Development, Transport, and the Environment ELY centers.All in all, some 1,000 people are participating in Latu 23, including international experts from the European Union Agency for Asylum, the European Border and Coast Guard Agency Frontex as well as the Estonian Police and Border Guard.The exercise, running from 2-6 October, is taking place in the Helsinki metropolitan area, the eastern part of the Gulf of Finland as well as in southeastern Finland.Last year, Finnish MPs approved changes to the countrys border laws to enable authorities to shut down borders or limit the number of border crossing points during exceptional circumstances.The law grew out of concern that a hybrid attack could potentially be aimed at Finlands eastern border with Russia. In 2021, Belarus attempted to push throusands of migrants into the EU.During Europes migration crisis in 2015, some 32,000 asylum seekers arrived in Finland.Would you like a roundup of the weeks top stories in your inbox every Thursday Then sign up to receive our weekly email.

## smugling 74-20052871
Yle journalists placed geolocation trackers on luxury cars near the Russian border, pinpointing the vehicles as they made a 5,000 km journey to a Siberian car dealership.Smugglers are still sending luxury cars to Russia across the Finnish border, using false documents to evade sanctions enforcement.Reporters from Yles Swedish language department used geolocation trackers to follow the route of the cars after they crossed the border into Russia.According to the tracker data, the vehicles ended up at a dealership in the Siberian town of Tomsk, which would be a clear violation of EU sanctions on Russia.A documentary about the investigative teams findings can be seen on the Finnish-language programme MOT and Spotlight, in Swedish.The journalists initially received a tip that there were Russian-bound cars headed to Finland via Germany.The investigative team suspected they would arrive at the port of Kotka before being illegally exported to Russia.In order to see where the new high-end cars were going, the journalists headed to the Vaalimaa border crossing in southeastern Finland, where they hid geolocation trackers on two cars that were being towed behind a box lorry.The vehicles in question were a dark blue BMW X3 and a white Lexus RX350. But the journalists suspect there may have also been a third car in the box lorry itself.The volume of goods crossing the Russian border has fallen dramatically since the introduction of EU sanctions, so Finnish Customs have had a good deal more time to carry out inspections. Even so, authorities are not able to examine every single lorry and container going by.We cant inspect every load, because traffic and trade would come to a standstill, said Sami Rakshit, head of Finnish Customs Enforcement Department.Officially, the cars Yle tracked may have been on their way to Central Asia. For example, it would not violate sanctions to transport cars through Russia to Kazakhstan, but it would be illegal to leave them in Russia.However, Finnish Customs has seen cases of lorry drivers having two different customs documents.We were shown a document saying the shipment was going to Kazakhstan, but there was also another document for Russian customs on the truck. That document showed the shipment was headed to Russia, Rakshit said.The cars crossed into Russia the day after the trackers were put on them in Vaalimaa.The vehicles first destination was to a St Petersburg-based forwarding company that provides clients with customs clearance help.A few days later, the cars were taken to an area in Moscow that appeared to be a new vehicle collection depot.Then, a few weeks later, the white Lexus was pinpointed at a car dealership in the city of Tomsk, Siberia, where it was being sold for around 100,000 euros.The tracker on the dark-blue BMW also showed that it was moving in the same area, suggesting that it had already found a new owner.The distance between the Finnish border and Tomsk is roughly 5,000 kilometres.The white Lexus that Svenska Yle followed has been for sale on a Russian car website since February this year. The car was brand new at the time, with only 27 km on the odometer. The photos in the ad were taken in Canada.A picture showing a licence plate on the rear of the car led to the identity of the first owner in Canada, a private individual whose name appears to be Russian.A few days after buying the car, he sold it to Deluxauto Incorporated, a Canadian export company.But the firm denied selling cars to Russia, while a Tomsk car dealer confirmed that the white Lexus came from Canada.Both Canada, the US and EU all have similar sanctions against Russia, making the export of new cars to Russia illegal.A long list of companies are often involved in sending goods across the Russian border, but transport firms play a pivotal role.There are several Russian-owned transport companies registered in Finland that deliver goods from Finnish ports to the Vaalimaa border crossing. A handful of these companies turnover and profits have increased sharply in the wake of Western sanctions against Russia. However, none of the firms wanted to be interviewed on the subject when reached for comment by Svenska Yle.Finlands Foreign Minister Elina Valtonen NCP said something needs to be done about skirting the sanctions.It should not be possible. We all have to take responsibility, step up our efforts and prevent sanctions from being circumvented, she said.Would you like a roundup of the weeks top stories in your inbox every Thursday Then sign up to receive our weekly email.
