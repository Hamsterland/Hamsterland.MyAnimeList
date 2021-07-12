#nullable enable
namespace Hamsterland.MyAnimeList.Services.Verification
{
    public class VerifierResult
    {
        public bool IsSuccess { get; init; }
        public VerifierResultType? Type { get; init; }
        public string? Message { get; init; }

        public string? ImageUrl { get; init; }
        
        public static VerifierResult FromSuccess()
        {
            return new() { IsSuccess = true };
        }
        
        public static VerifierResult FromSuccess(VerifierResultType resultType, string message)
        {
            return new()
            {
                Type = resultType,
                IsSuccess = true,
                Message = message
            };
        }

        public static VerifierResult FromError(VerifierResultType resultType, string message)
        {
            return new()
            {
                IsSuccess = false,
                Type = resultType,
                Message = message,
            };
        }
        
        public static VerifierResult FromError(VerifierResultType resultType, string message, string imageUrl)
        {
            return new()
            {
                IsSuccess = false,
                Type = resultType,
                Message = message,
                ImageUrl = imageUrl
            };
        }
    }
}