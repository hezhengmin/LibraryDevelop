using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Zheng.Infra.Data.Models.Configurations;

namespace Zheng.Infra.Data.Models;

public partial class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    {
    }

    // Custom Identity tables
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<RolePermission> RolePermissions { get; set; }
    public virtual DbSet<UserExternalLogin> UserExternalLogins { get; set; }
    public virtual DbSet<UserToken> UserTokens { get; set; }

    // Business tables
    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookPhoto> BookPhotos { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<SysMenu> SysMenus { get; set; }

    public virtual DbSet<UploadFile> UploadFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("系統編號");
            entity.Property(e => e.Attachment).HasComment("附件");
            entity.Property(e => e.Authority).HasComment("授權資訊");
            entity.Property(e => e.Binding).HasComment("裝訂");
            entity.Property(e => e.Catalog).HasComment("目次");
            entity.Property(e => e.CeasedDate).HasComment("停刊註記");
            entity.Property(e => e.Classify).HasComment("書籍分類");
            entity.Property(e => e.Cover).HasComment("書封連結");
            entity.Property(e => e.CreatedAt).HasComment("新增時間");
            entity.Property(e => e.CreatedBy).HasComment("新增者");
            entity.Property(e => e.Creator).HasComment("作者資訊");
            entity.Property(e => e.Duration).HasComment("播放時間長度");
            entity.Property(e => e.Edition).HasComment("版次");
            entity.Property(e => e.GPN).HasComment("GPN(政府出版品統一編號)");
            entity.Property(e => e.GPNType).HasComment("出版品分類");
            entity.Property(e => e.Governance).HasComment("施政分類");
            entity.Property(e => e.Grade).HasComment("級別");
            entity.Property(e => e.Introduction).HasComment("abstract(書籍介紹)");
            entity.Property(e => e.Language).HasComment("語言");
            entity.Property(e => e.NumberOfCopies).HasComment("庫存數量");
            entity.Property(e => e.Numbers).HasComment("字號");
            entity.Property(e => e.Pages).HasComment("頁數");
            entity.Property(e => e.Price).HasComment("價格");
            entity.Property(e => e.PublishDate).HasComment("出版日期");
            entity.Property(e => e.Publisher).HasComment("出版單位");
            entity.Property(e => e.Restriction).HasComment("權利範圍");
            entity.Property(e => e.RightCondition).HasComment("出版情況");
            entity.Property(e => e.Size).HasComment("開數");
            entity.Property(e => e.Status).HasComment("狀態(0有庫存1無庫存)");
            entity.Property(e => e.Subject).HasComment("主題分類");
            entity.Property(e => e.TargetPeople).HasComment("適用對象");
            entity.Property(e => e.Title).HasComment("書名標題");
            entity.Property(e => e.Types).HasComment("資料類型");
            entity.Property(e => e.UpdatedAt).HasComment("修改時間");
            entity.Property(e => e.UpdatedBy).HasComment("修改者");
            entity.Property(e => e.Url).HasComment("出版品網址-線上版或試閱版");
        });

        modelBuilder.Entity<BookPhoto>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("系統編號");
            entity.Property(e => e.BookId).HasComment("書籍Id");
            entity.Property(e => e.SystemDate).HasComment("系統日期");
            entity.Property(e => e.UploadFileId).HasComment("檔案Id");

            entity.HasOne(d => d.Book).WithMany(p => p.BookPhotos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookPhoto_Book");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("系統編號");
            entity.Property(e => e.CreatedAt).HasComment("新增時間");
            entity.Property(e => e.CreatedBy).HasComment("新增者");
            entity.Property(e => e.Title).HasComment("名稱");
            entity.Property(e => e.UpdatedAt).HasComment("修改時間");
            entity.Property(e => e.UpdatedBy).HasComment("修改者");
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("系統編號");
            entity.Property(e => e.CreatedAt).HasComment("新增時間");
            entity.Property(e => e.CreatedBy).HasComment("新增者");
            entity.Property(e => e.DueDate).HasComment("借出結束日期");
            entity.Property(e => e.IssueDate).HasComment("借出開始日期");
            entity.Property(e => e.ReturnDate).HasComment("書籍歸還日期");
            entity.Property(e => e.UpdatedAt).HasComment("修改時間");
            entity.Property(e => e.UpdatedBy).HasComment("修改者");

            entity.HasOne(d => d.User).WithMany(p => p.Loans)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loan_User");

            entity.HasOne(d => d.Book).WithMany(p => p.Loans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loan_Book");
        });

        modelBuilder.Entity<SysMenu>(entity =>
        {
            entity.Property(e => e.Component).HasComment("元件設定");
            entity.Property(e => e.CreatedAt).HasComment("新增時間");
            entity.Property(e => e.CreatedBy).HasComment("新增者");
            entity.Property(e => e.Hidden).HasComment("是否隱藏（1：是，0：否）");
            entity.Property(e => e.Icon).HasComment("圖示");
            entity.Property(e => e.IsMenu).HasComment("是否為選單（1：選單，0：按鈕）");
            entity.Property(e => e.IsOpen).HasComment("是否預設展開（1：是，0：否）");
            entity.Property(e => e.Levels).HasComment("層級");
            entity.Property(e => e.ModuleNo).HasComment("選單編號");
            entity.Property(e => e.Name).HasComment("名稱");
            entity.Property(e => e.ParentModuleId).HasComment("父選單編號");
            entity.Property(e => e.SortOrder).HasComment("排序順序");
            entity.Property(e => e.Status).HasComment("狀態（1：啟用，0：停用）");
            entity.Property(e => e.Tips).HasComment("滑鼠懸停提示文字");
            entity.Property(e => e.UpdatedAt).HasComment("修改時間");
            entity.Property(e => e.UpdatedBy).HasComment("修改者");
            entity.Property(e => e.Url).HasComment("連結網址");
        });

        modelBuilder.Entity<UploadFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_File");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("系統編號");
            entity.Property(e => e.ContentType).HasComment("檔案類型");
            entity.Property(e => e.CreatedAt).HasComment("新增時間");
            entity.Property(e => e.CreatedBy).HasComment("新增者");
            entity.Property(e => e.Extension).HasComment("副檔名");
            entity.Property(e => e.Length).HasComment("檔案大小");
            entity.Property(e => e.Name).HasComment("名稱");
            entity.Property(e => e.UpdatedAt).HasComment("修改時間");
            entity.Property(e => e.UpdatedBy).HasComment("修改者");
        });

        // Apply custom identity table configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
        modelBuilder.ApplyConfiguration(new UserExternalLoginConfiguration());
        modelBuilder.ApplyConfiguration(new UserTokenConfiguration());

        // Seed data
        SeedData.Apply(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
