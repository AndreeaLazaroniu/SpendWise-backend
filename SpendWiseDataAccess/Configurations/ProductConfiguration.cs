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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products").HasKey(p => p.Id);

            builder.Property(p => p.Name).HasMaxLength(255);

            builder.HasMany(p => p.Categories).WithMany(c => c.Products)
                .UsingEntity<Dictionary<String, object>>("ProductCategory", 
                o => o.HasOne<Category>().WithMany().HasForeignKey("CategoryId").HasConstraintName("FK_ProductCategory_Category"),
                o => o.HasOne<Product>().WithMany().HasForeignKey("ProductId").HasConstraintName("FK_ProductCategory_Product"));
        }

    }
}
