using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hamsterland.MyAnimeList.Models
{
    public class MalAccount
    {
        public int Id { get; set; }
        
        public ulong UserId { get; set; }
        
        public int? MalId { get; set; }
        
        [NotMapped]
        public bool IsVerified => MalId != 0;

        public MalAccountDto ToMalAccountDto()
        {
            return new(UserId, MalId);
        }
    }
    
    public class MalAccountConfiguration : IEntityTypeConfiguration<MalAccount>
    {
        public void Configure(EntityTypeBuilder<MalAccount> builder)
        {
            builder
                .Property(x => x.UserId)
                .HasConversion<long>();

            builder
                .HasIndex(x => x.UserId)
                .IsUnique();
            
            builder
                .HasIndex(x => x.MalId)
                .IsUnique();
        }
    }
}