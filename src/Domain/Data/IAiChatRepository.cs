namespace AJE.Domain.Data;

public interface IAiChatRepository
{
    Task AddAsync(AiChat aiChat);
}
