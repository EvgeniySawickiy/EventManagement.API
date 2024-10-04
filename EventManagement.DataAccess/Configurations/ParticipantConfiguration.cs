using EventManagement.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagement.DataAccess.Configurations
{
    public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.BirthDate)
                .IsRequired();

            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            builder.HasOne(p => p.User)
               .WithMany(u => u.Participants) 
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
