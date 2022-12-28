using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ProjectBoots2.Models.structures;

namespace ProjectBoots2.Models.contexts
{
    public partial class dbBootsContext : DbContext
    {
        public dbBootsContext()
        {
        }

        public dbBootsContext(DbContextOptions<dbBootsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<Variation> Variations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=madchipmunk03.cz;user=sssvt;password=123456;database=dbBoots;port=3306", ServerVersion.Parse("10.5.15-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.ToTable("tbAdministrators");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("tbCategories");

                entity.HasIndex(e => e.ParentId, "parentId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(255)
                    .HasColumnName("categoryName");

                entity.Property(e => e.ParentId)
                    .HasColumnType("int(11)")
                    .HasColumnName("parentId");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("tbCategories_ibfk_1");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("tbOrders");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.City)
                    .HasMaxLength(255)
                    .HasColumnName("city");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .HasColumnName("firstName");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .HasColumnName("lastName");

                entity.Property(e => e.OrderedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("orderedTime");

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(255)
                    .HasColumnName("postalCode");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("tbOrderItems");

                entity.HasIndex(e => e.OrderId, "orderId");

                entity.HasIndex(e => e.VariationId, "variationId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.OrderId)
                    .HasColumnType("int(11)")
                    .HasColumnName("orderId");

                entity.Property(e => e.Price)
                    .HasColumnType("int(11)")
                    .HasColumnName("price");

                entity.Property(e => e.Quantity)
                    .HasColumnType("int(11)")
                    .HasColumnName("quantity");

                entity.Property(e => e.VariationId)
                    .HasColumnType("int(11)")
                    .HasColumnName("variationId");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("tbOrderItems_ibfk_1");

                entity.HasOne(d => d.Variation)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.VariationId)
                    .HasConstraintName("tbOrderItems_ibfk_2");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("tbProducts");

                entity.HasIndex(e => e.CategoryId, "categoryId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(11)")
                    .HasColumnName("categoryId");

                entity.Property(e => e.Description)
                    .HasMaxLength(4096)
                    .HasColumnName("description");

                entity.Property(e => e.ParamCode)
                    .HasMaxLength(255)
                    .HasColumnName("paramCode");

                entity.Property(e => e.ParamMaterial)
                    .HasMaxLength(255)
                    .HasColumnName("paramMaterial");

                entity.Property(e => e.ParamPurpose)
                    .HasMaxLength(255)
                    .HasColumnName("paramPurpose");

                entity.Property(e => e.ParamType)
                    .HasMaxLength(255)
                    .HasColumnName("paramType");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("tbProducts_ibfk_1");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("tbProductImages");

                entity.HasIndex(e => e.ProductId, "productId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Priority)
                    .HasColumnType("int(11)")
                    .HasColumnName("priority");

                entity.Property(e => e.ProductId)
                    .HasColumnType("int(11)")
                    .HasColumnName("productId");

                entity.Property(e => e.Url)
                    .HasMaxLength(255)
                    .HasColumnName("url");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("tbProductImages_ibfk_1");
            });

            modelBuilder.Entity<Variation>(entity =>
            {
                entity.ToTable("tbVariations");

                entity.HasIndex(e => e.ProductId, "productId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Color)
                    .HasMaxLength(255)
                    .HasColumnName("color");

                entity.Property(e => e.InStock)
                    .HasColumnType("int(11)")
                    .HasColumnName("inStock");

                entity.Property(e => e.Price)
                    .HasColumnType("int(11)")
                    .HasColumnName("price");

                entity.Property(e => e.ProductId)
                    .HasColumnType("int(11)")
                    .HasColumnName("productId");

                entity.Property(e => e.SalePrice)
                    .HasColumnType("int(11)")
                    .HasColumnName("salePrice");

                entity.Property(e => e.Size)
                    .HasColumnType("int(11)")
                    .HasColumnName("size");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Variations)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("tbVariations_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
