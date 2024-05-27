using CrudApplication.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CrudApplication.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(user => user.Guid);
            builder.Property(user => user.Login);
            builder.Property(user => user.Password);
            builder.Property(user => user.Name);
            builder.Property(user => user.Gender);
            builder.Property(user => user.Birthday);
            builder.Property(user => user.Admin);
            builder.Property(user => user.CreatedOn);
            builder.Property(user => user.CreatedBy);
            builder.Property(user => user.ModifiedOn);
            builder.Property(user => user.ModifiedBy);
            builder.Property(user => user.RevokedOn);
            builder.Property(user => user.RevokedBy);
        }
    }
}