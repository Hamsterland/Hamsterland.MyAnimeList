namespace Hamsterland.MyAnimeList.Models
{
    public record MalAccountDto(ulong UserId, int? MalId)
    {
        public bool IsVerified => MalId != 0;
    }
}