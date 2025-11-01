// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using pishrooAsp.Models;
using pishrooAsp.Models.Newses;
using pishrooAsp.Models.ProductRequest;
using pishrooAsp.Models.Products;
using pishrooAsp.Models.Slider;
using pishrooAsp.Models.WhyUss;

namespace pishrooAsp.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<News> News { get; set; }
		public DbSet<NewsImage> NewsImages { get; set; }
		public DbSet<NewsAttachment> NewsAttachments { get; set; }
		public DbSet<NewsTranslation> NewsTranslations { get; set; }
		public DbSet<Lang> Langs { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductTranslation> ProductTranslations { get; set; }
		public DbSet<WhyUsItem> WhyUs { get; set; }
		public DbSet<WhyUsTranslation> WhyUsTranslations { get; set; }
		public DbSet<AboutUsItem> AboutUs { get; set; }
		public DbSet<AboutUsTranslation> AboutUsTranslations { get; set; }
		public DbSet<Slider> Sliders { get; set; }
		public DbSet<SliderTranslation> SliderTranslations { get; set; }
		public DbSet<ProductRequest> ProductRequests { get; set; }
		public DbSet<GalleryImage> GalleryImages { get; set; }
		public DbSet<Catalog> Catalogs { get; set; } // تغییر به صورت جمع
		public DbSet<ProductRequestFile> ProductRequestFiles { get; set; } // تغییر به صورت جمع
		public DbSet<Background> Backgrounds { get; set; } // تغییر به صورت جمع
		public DbSet<ProductRequestMessage> ProductRequestMessages { get; set; } // تغییر به صورت جمع

		public DbSet<message> Message { get; set; }
		public DbSet<VisitLog> VisitLogs { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configure relationships
			modelBuilder.Entity<News>()
				.HasMany(n => n.Translations)
				.WithOne(t => t.News)
				.HasForeignKey(t => t.NewsId);

			modelBuilder.Entity<News>()
				.HasMany(n => n.Images)
				.WithOne(i => i.News)
				.HasForeignKey(i => i.NewsId);

			modelBuilder.Entity<News>()
				.HasMany(n => n.Attachments)
				.WithOne(a => a.News)
				.HasForeignKey(a => a.NewsId);

			modelBuilder.Entity<SliderTranslation>()
				.HasOne(st => st.Slider)
				.WithMany(s => s.Translations)
				.HasForeignKey(st => st.SliderId);

			// رابطه با جدول زبان
			modelBuilder.Entity<SliderTranslation>()
				.HasOne(st => st.Lang)
				.WithMany()
				.HasForeignKey(st => st.LangId);

			// تنظیمات برای Catalogs
			modelBuilder.Entity<Catalog>(entity =>
			{
				entity.ToTable("Catalogs");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Title).HasMaxLength(100);
				entity.Property(e => e.Description).HasMaxLength(500);
				entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
				entity.Property(e => e.OriginalFileName).HasMaxLength(255);
				entity.Property(e => e.FilePath).HasMaxLength(500);
				entity.Property(e => e.UploadDate).HasDefaultValueSql("GETDATE()");
				entity.Property(e => e.IsActive).HasDefaultValue(true);
			});

			// تنظیمات برای GalleryImages
			modelBuilder.Entity<GalleryImage>(entity =>
			{
				entity.ToTable("GalleryImages");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Title).HasMaxLength(100);
				entity.Property(e => e.Description).HasMaxLength(500);
				entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
				entity.Property(e => e.OriginalFileName).HasMaxLength(255);
				entity.Property(e => e.FilePath).HasMaxLength(500);
				entity.Property(e => e.UploadDate).HasDefaultValueSql("GETDATE()");
			});
		}
	}
}