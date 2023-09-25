namespace AJE.BackBone.Services
{
    public class ArticleService : Article.ArticleBase
    {
        private readonly ILogger<ArticleService> _logger;
        public ArticleService(ILogger<ArticleService> logger)
        {
            _logger = logger;
        }

        public override Task<CreateReply> Create(CreateRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CreateReply
            {
                Message = "Hello " + request.Title
            });
        }
    }
}