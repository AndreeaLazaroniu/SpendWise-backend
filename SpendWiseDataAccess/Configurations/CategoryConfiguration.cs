using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.DataAccess.Entities;

namespace SpendWise.DataAccess.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories").HasKey(c => c.Id);

            builder.Property(c => c.Name).HasMaxLength(255);

            builder.HasData(new Category { Id = 1, Name = "Altele" });

        }
    }   
}
