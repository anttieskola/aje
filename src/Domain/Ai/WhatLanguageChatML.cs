namespace AJE.Domain.Ai;

/// <summary>
/// LLM's just can't do this in reliable way
/// Better to create service using lingua-rs and use that
/// </summary>
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
