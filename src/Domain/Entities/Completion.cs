namespace AJE.Domain.Entities;

/// <summary>
/// llama.cpp server completion request
/// </summary>
public class CompletionRequest
{
    /// <summary>
    /// Provide the prompt for this completion as a string or as an array of strings or numbers representing tokens.
    /// Internally, the prompt is compared to the previous completion and only the "unseen" suffix is evaluated.
    /// If the prompt is a string or an array with the first element given as a string, a bos token is inserted in the front like main does.
    /// </summary>
    [JsonPropertyName("prompt")]
    public required string Prompt { get; set; }

    /// <summary>
    /// Adjust the randomness of the generated text
    /// (default: 0.8)
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.8;

    /// <summary>
    /// Limit the next token selection to the K most probable tokens
    /// (default: 40)
    /// </summary>
    [JsonPropertyName("top_k")]
    public int TopK { get; set; } = 40;

    /// <summary>
    /// Limit the next token selection to a subset of tokens with a cumulative probability above a threshold P
    /// (default: 0.95)
    /// </summary>
    [JsonPropertyName("top_p")]
    public double TopP { get; set; } = 0.95;

    /// <summary>
    ///  Set the maximum number of tokens to predict when generating text.
    ///  Note: May exceed the set limit slightly if the last token is a partial multibyte character.
    ///  When 0, no tokens will be generated but the prompt is evaluated into the cache.
    ///  (default: -1, -1 = infinity)
    /// </summary>
    [JsonPropertyName("n_predict")]
    public int NumberOfTokensToPredict { get; set; } = -1;

    /// <summary>
    /// Specify the number of tokens from the prompt to retain when the context size is exceeded and tokens need to be discarded.
    /// By default, this value is set to 0 (meaning no tokens are kept). Use -1 to retain all tokens from the prompt.
    /// </summary>
    [JsonPropertyName("n_keep")]
    public int NumberOfTokensToKeep { get; set; } = 0;

    /// <summary>
    /// It allows receiving each predicted token in real-time instead of waiting for the completion to finish.
    /// To enable this, set to true.
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;

    /// <summary>
    /// Specify a JSON array of stopping strings. These words will not be included in the completion,
    /// so make sure to add them to the prompt for the next iteration (default: []).
    /// </summary>
    [JsonPropertyName("stop")]
    public IEnumerable<string> Stop { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Enable tail free sampling with parameter z
    /// (default: 1.0, 1.0 = disabled)
    /// </summary>
    [JsonPropertyName("tfs_z")]
    public double TailFreeSamplingZ { get; set; } = 1.0;

    /// <summary>
    /// Enable locally typical sampling with parameter p
    /// (default: 1.0, 1.0 = disabled)
    /// </summary>
    [JsonPropertyName("typical_p")]
    public double TypicalP { get; set; } = 1.0;

    /// <summary>
    /// Control the repetition of token sequences in the generated text
    /// (default: 1.1)
    /// </summary>
    [JsonPropertyName("repeat_penalty")]
    public double RepeatPenalty { get; set; } = 1.1;

    /// <summary>
    /// Last n tokens to consider for penalizing repetition
    /// (default: 64, 0 = disabled, -1 = ctx-size)
    /// </summary>
    [JsonPropertyName("repeat_last_n")]
    public int RepeatLastNumberOfTokens { get; set; } = 64;

    /// <summary>
    /// Penalize newline tokens when applying the repeat penalty
    /// (default: true)
    /// </summary>
    [JsonPropertyName("penalize_nl")]
    public bool PenalizeNewLine { get; set; } = true;

    /// <summary>
    /// Repeat alpha presence penalty
    /// (default: 0.0, 0.0 = disabled)
    /// </summary>
    [JsonPropertyName("presence_penalty")]
    public double PresencePenalty { get; set; } = 0.0;

    /// <summary>
    /// Repeat alpha frequency penalty
    /// (default: 0.0, 0.0 = disabled)
    /// </summary>
    [JsonPropertyName("frequency_penalty")]
    public double FrequencyPenalty { get; set; } = 0.0;

    /// <summary>
    /// Enable Mirostat sampling, controlling perplexity during text generation
    /// (default: 0, 0 = disabled, 1 = Mirostat, 2 = Mirostat 2.0)
    /// </summary>
    [JsonPropertyName("mirostat")]
    public int Mirostat { get; set; } = 0;

    /// <summary>
    /// Set the Mirostat target entropy, parameter tau
    /// (default: 5.0)
    /// </summary>
    [JsonPropertyName("mirostat_tau")]
    public double MirostatTargetEntropy { get; set; } = 5.0;

    /// <summary>
    /// Set the Mirostat learning rate, parameter eta
    /// (default: 0.1)
    /// </summary>
    [JsonPropertyName("mirostat_eta")]
    public double MirostatLearningRate { get; set; } = 0.1;

    /// <summary>
    /// Set grammar for grammar-based sampling
    /// (default: "")
    /// </summary>
    [JsonPropertyName("grammar")]
    public string Grammar { get; set; } = string.Empty;

    /// <summary>
    /// Set the random number generator (RNG) seed
    /// (default: -1, -1 = random seed)
    /// </summary>
    [JsonPropertyName("seed")]
    public long Seed { get; set; } = -1;

    /// <summary>
    /// Ignore end of stream token and continue generating
    /// (default: false)
    /// </summary>
    [JsonPropertyName("ignore_eos")]
    public bool IgnoreEndOfStreamToken { get; set; } = false;

    /// <summary>
    /// Modify the likelihood of a token appearing in the generated text completion.
    /// For example, use "logit_bias": [[15043,1.0]] to increase the likelihood of the token 'Hello',
    /// or "logit_bias": [[15043,-1.0]] to decrease its likelihood. Setting the value to false,
    /// "logit_bias": [[15043,false]] ensures that the token Hello is never produced (default: [])
    /// </summary>
    [JsonPropertyName("logit_bias")]
    public IEnumerable<Tuple<int, double>> LogitBias { get; set; } = Array.Empty<Tuple<int, double>>();

    /// <summary>
    /// If greater than 0, the response also contains the probabilities of top N tokensfor each generated token
    /// (default: 0)
    /// </summary>
    [JsonPropertyName("n_probs")]
    public int NumberOfTokenProbabilities { get; set; } = 0;
}

/// <summary>
/// llama.cpp server completion response
/// Note: When using streaming mode (stream) only content and stop will be returned until end of completion
/// </summary>
public class CompletionResponse
{
    /// <summary>
    /// Completion result as a string (excluding stopping_word if any). In case of streaming mode,
    /// will contain the next token as a string
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Boolean for use with stream to check whether the generation has stopped
    /// (Note: This is not related to stopping words array stop from input options)
    /// </summary>
    [JsonPropertyName("stop")]
    public bool Stop { get; set; }

    /// <summary>
    /// The provided options above excluding prompt but including n_ctx, model
    /// </summary>
    [JsonPropertyName("generation_settings")]
    public GenerationSettings GenerationSettings { get; set; } = default!;

    /// <summary>
    /// The path to the model loaded with -m
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// The provided prompt
    /// </summary>
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// Indicating whether the completion has stopped because it encountered the EOS token
    /// </summary>
    [JsonPropertyName("stopped_eos")]
    public bool StoppedBecauseEOS { get; set; }

    /// <summary>
    /// Indicating whether the completion stopped because n_predict tokens were generated
    /// before stop words or EOS was encountered
    /// </summary>
    [JsonPropertyName("stopped_limit")]
    public bool StoppedBecauseLimit { get; set; }

    /// <summary>
    /// Indicating whether the completion stopped due to encountering a stopping word from stop JSON array provided
    /// </summary>
    [JsonPropertyName("stopped_word")]
    public bool StoppedBecauseWord { get; set; }

    /// <summary>
    /// The stopping word encountered which stopped the generation (or "" if not stopped due to a stopping word)
    /// </summary>
    [JsonPropertyName("stopping_word")]
    public string StoppingWord { get; set; } = string.Empty;

    /// <summary>
    /// Hash of timing information about the completion such as the number of tokens predicted_per_second
    /// </summary>
    [JsonPropertyName("timings")]
    public TimingInformation Timings { get; set; } = default!;

    /// <summary>
    /// Number of tokens from the prompt which could be re-used from previous completion (n_past)
    /// </summary>
    [JsonPropertyName("tokens_cached")]
    public int TokensCached { get; set; }

    /// <summary>
    /// Number of tokens evaluated in total from the prompt
    /// </summary>
    [JsonPropertyName("tokens_evaluated")]
    public int TokensEvaluated { get; set; }

    /// <summary>
    /// Boolean indicating if the context size was exceeded during generation, i.e. the number of tokens provided
    /// in the prompt (tokens_evaluated) plus tokens generated (tokens predicted) exceeded the context size (n_ctx)
    /// </summary>
    [JsonPropertyName("truncated")]
    public bool Truncated { get; set; }

    /// <summary>
    /// Assign the completion task to an specific slot. If is -1 the task will be assigned to a Idle slot (default: -1)
    /// </summary>
    [JsonPropertyName("slotId")]
    public int SlotId { get; set; }

    /// <summary>
    /// Is model Multimodal
    /// </summary>
    [JsonPropertyName("multimodal")]
    public bool Multimodal { get; set; } = false;
}

/// <summary>
/// llama.cpp generation settings
/// </summary>
public class GenerationSettings
{
    /// <summary>
    /// Repeat alpha frequency penalty
    /// (default: 0.0, 0.0 = disabled)
    /// </summary>
    [JsonPropertyName("frequency_penalty")]
    public double FrequencyPenalty { get; set; } = 0.0;

    /// <summary>
    /// Set grammar for grammar-based sampling
    /// (default: "")
    /// </summary>
    [JsonPropertyName("grammar")]
    public string Grammar { get; set; } = string.Empty;

    /// <summary>
    /// Ignore end of stream token and continue generating
    /// (default: false)
    /// </summary>
    [JsonPropertyName("ignore_eos")]
    public bool IgnoreEndOfStreamToken { get; set; } = false;

    /// <summary>
    /// Modify the likelihood of a token appearing in the generated text completion.
    /// For example, use "logit_bias": [[15043,1.0]] to increase the likelihood of the token 'Hello',
    /// or "logit_bias": [[15043,-1.0]] to decrease its likelihood. Setting the value to false,
    /// "logit_bias": [[15043,false]] ensures that the token Hello is never produced (default: [])
    /// </summary>
    [JsonPropertyName("logit_bias")]
    public IEnumerable<Tuple<int, double>> LogitBias { get; set; } = Array.Empty<Tuple<int, double>>();

    /// <summary>
    /// Enable Mirostat sampling, controlling perplexity during text generation
    /// (default: 0, 0 = disabled, 1 = Mirostat, 2 = Mirostat 2.0)
    /// </summary>
    [JsonPropertyName("mirostat")]
    public int Mirostat { get; set; } = 0;

    /// <summary>
    /// Set the Mirostat learning rate, parameter eta
    /// (default: 0.1)
    /// </summary>
    [JsonPropertyName("mirostat_eta")]
    public double MirostatLearningRate { get; set; } = 0.1;

    /// <summary>
    /// Set the Mirostat target entropy, parameter tau
    /// (default: 5.0)
    /// </summary>
    [JsonPropertyName("mirostat_tau")]
    public double MirostatTargetEntropy { get; set; } = 5.0;

    /// <summary>
    /// The path to the model loaded with -m
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Context size represented as the number of tokens to keep in memory
    /// </summary>
    [JsonPropertyName("n_ctx")]
    public int NumberOfTokensContext { get; set; }

    /// <summary>
    ///  Set the maximum number of tokens to predict when generating text.
    ///  Note: May exceed the set limit slightly if the last token is a partial multibyte character.
    ///  When 0, no tokens will be generated but the prompt is evaluated into the cache.
    ///  (default: -1, -1 = infinity)
    /// </summary>
    [JsonPropertyName("n_predict")]
    public int NumberOfTokensToPredict { get; set; } = -1;

    /// <summary>
    /// If greater than 0, the response also contains the probabilities of top N tokensfor each generated token
    /// (default: 0)
    /// </summary>
    [JsonPropertyName("n_probs")]
    public int NumberOfTokenProbabilities { get; set; } = 0;

    /// <summary>
    /// Penalize newline tokens when applying the repeat penalty
    /// (default: true)
    /// </summary>
    [JsonPropertyName("penalize_nl")]
    public bool PenalizeNewLine { get; set; } = true;

    /// <summary>
    /// Repeat alpha presence penalty
    /// (default: 0.0, 0.0 = disabled)
    /// </summary>
    [JsonPropertyName("presence_penalty")]
    public double PresencePenalty { get; set; } = 0.0;

    /// <summary>
    /// Last n tokens to consider for penalizing repetition
    /// (default: 64, 0 = disabled, -1 = ctx-size)
    /// </summary>
    [JsonPropertyName("repeat_last_n")]
    public int RepeatLastNumberOfTokens { get; set; } = 64;

    /// <summary>
    /// Control the repetition of token sequences in the generated text
    /// (default: 1.1)
    /// </summary>
    [JsonPropertyName("repeat_penalty")]
    public double RepeatPenalty { get; set; } = 1.1;

    /// <summary>
    /// Set the random number generator (RNG) seed
    /// (default: -1, -1 = random seed)
    /// </summary>
    [JsonPropertyName("seed")]
    public long Seed { get; set; } = -1;

    /// <summary>
    /// Specify a JSON array of stopping strings. These words will not be included in the completion,
    /// so make sure to add them to the prompt for the next iteration (default: []).
    /// </summary>
    [JsonPropertyName("stop")]
    public IEnumerable<string> Stop { get; set; } = Array.Empty<string>();

    /// <summary>
    /// It allows receiving each predicted token in real-time instead of waiting for the completion to finish.
    /// To enable this, set to true.
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;

    /// <summary>
    /// Adjust the randomness of the generated text
    /// (default: 0.8)
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.8;

    /// <summary>
    /// Enable tail free sampling with parameter z
    /// (default: 1.0, 1.0 = disabled)
    /// </summary>
    [JsonPropertyName("tfs_z")]
    public double TailFreeSamplingZ { get; set; } = 1.0;

    /// <summary>
    /// Limit the next token selection to the K most probable tokens
    /// (default: 40)
    /// </summary>
    [JsonPropertyName("top_k")]
    public int TopK { get; set; } = 40;

    /// <summary>
    /// Limit the next token selection to a subset of tokens with a cumulative probability above a threshold P
    /// (default: 0.95)
    /// </summary>
    [JsonPropertyName("top_p")]
    public double TopP { get; set; } = 0.95;

    /// <summary>
    /// Enable locally typical sampling with parameter p
    /// (default: 1.0, 1.0 = disabled)
    /// </summary>
    [JsonPropertyName("typical_p")]
    public double TypicalP { get; set; } = 1.0;
}

/// <summary>
/// llama.cpp timing information
/// </summary>
public class TimingInformation
{
    [JsonPropertyName("predicted_ms")]
    public double PredictedMS { get; set; }

    [JsonPropertyName("predicted_n")]
    public int PredictedN { get; set; }

    [JsonPropertyName("predicted_per_second")]
    public double? PredictedPerSecond { get; set; }

    [JsonPropertyName("predicted_per_token_ms")]
    public double PredictedPerTokenMS { get; set; }

    [JsonPropertyName("prompt_ms")]
    public double PromptMS { get; set; }

    [JsonPropertyName("prompt_n")]
    public int PromptN { get; set; }

    [JsonPropertyName("prompt_per_second")]
    public double? PromptPerSecond { get; set; }

    [JsonPropertyName("prompt_per_token_ms")]
    public double PromptPerTokenMS { get; set; }
}
