namespace AJE.Domain.Ai;

public class WhatLanguageChatML : ChatMLCreator
{
    public const string EntityName = "assistant";

    public static readonly string[] SystemInstructions = [
            "You are an assistant who is tasked is to examine given context and report back in which language it is written"
        ];

    public WhatLanguageChatML() :
        base(EntityName, SystemInstructions)
    {
    }
}
