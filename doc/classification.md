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

