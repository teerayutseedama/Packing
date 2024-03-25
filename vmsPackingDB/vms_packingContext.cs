using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Packing.vmsPackingDB;

public partial class vms_packingContext : DbContext
{
    public vms_packingContext(DbContextOptions<vms_packingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<tbm_materail_type> tbm_materail_type { get; set; }

    public virtual DbSet<tbm_material> tbm_material { get; set; }

    public virtual DbSet<tbm_material_bak> tbm_material_bak { get; set; }

    public virtual DbSet<tbm_pk_batch_slip> tbm_pk_batch_slip { get; set; }

    public virtual DbSet<tbm_pk_batch_status> tbm_pk_batch_status { get; set; }

    public virtual DbSet<tbm_pk_production_line> tbm_pk_production_line { get; set; }

    public virtual DbSet<tbm_pk_sloc> tbm_pk_sloc { get; set; }

    public virtual DbSet<tbm_pk_work_shift> tbm_pk_work_shift { get; set; }

    public virtual DbSet<tbm_plant_move2stored> tbm_plant_move2stored { get; set; }

    public virtual DbSet<tbm_product_group_move2stored> tbm_product_group_move2stored { get; set; }

    public virtual DbSet<tbm_product_package_move2stored> tbm_product_package_move2stored { get; set; }

    public virtual DbSet<tbm_product_type_move2stored> tbm_product_type_move2stored { get; set; }

    public virtual DbSet<tbm_sap_sloc> tbm_sap_sloc { get; set; }

    public virtual DbSet<tbm_unit_move2stored> tbm_unit_move2stored { get; set; }

    public virtual DbSet<tbt_pk_batch_no_detail> tbt_pk_batch_no_detail { get; set; }

    public virtual DbSet<tbt_pk_batch_no_header> tbt_pk_batch_no_header { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Thai_CI_AS");

        modelBuilder.Entity<tbm_materail_type>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__tbm_pk_s__3214EC27C94DE153_copy2");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.MATERIAL_TYPE)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tbm_material>(entity =>
        {
            entity.HasKey(e=>e.id);

            entity.Property(e => e.BUN).HasMaxLength(10);
            entity.Property(e => e.DEFAULT_UOM).HasMaxLength(3);
            entity.Property(e => e.FORM_NO).HasMaxLength(30);
            entity.Property(e => e.MATERIAL_CODE).HasMaxLength(50);
            entity.Property(e => e.MATERIAL_GROUP).HasMaxLength(100);
            entity.Property(e => e.MATERIAL_NAME).HasMaxLength(100);
            entity.Property(e => e.MATERIAL_TYPE_ID)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("join to tbm_materail_type");
            entity.Property(e => e.PKG_SIZE_KG).HasMaxLength(10);
            entity.Property(e => e.REV).HasMaxLength(5);
            entity.Property(e => e.SHELF_LIFT_MONTH).HasMaxLength(10);
            entity.Property(e => e.SLOC_ID)
                .HasMaxLength(10)
                .HasComment("join to tbm_pk_sloc");
        });

        modelBuilder.Entity<tbm_material_bak>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.BUN).HasMaxLength(10);
            entity.Property(e => e.MATERIAL_CODE).HasMaxLength(50);
            entity.Property(e => e.MATERIAL_GROUP).HasMaxLength(100);
            entity.Property(e => e.MATERIAL_INFO).HasMaxLength(100);
            entity.Property(e => e.PKG_SIZE_KG).HasMaxLength(10);
            entity.Property(e => e.SAP_SLOC).HasMaxLength(10);
            entity.Property(e => e.SHELF_LIFT_MONTH).HasMaxLength(10);
            entity.Property(e => e.SLOC).HasMaxLength(10);
        });

        modelBuilder.Entity<tbm_pk_batch_slip>(entity =>
        {
            entity.HasKey(e=>e.id);
            entity.Property(e => e.FONT_SIZE).HasMaxLength(10);
            entity.Property(e => e.FORM_NO_SIZE).HasMaxLength(10);
            entity.Property(e => e.QR_CODE_HEIGHT).HasMaxLength(10);
            entity.Property(e => e.QR_CODE_SIZE_UNIT).HasMaxLength(10);
            entity.Property(e => e.QR_CODE_WIDTH).HasMaxLength(10);
            entity.Property(e => e.RUNNING_FONT_SIZE).HasMaxLength(10);
            entity.Property(e => e.STICKER_HEIGH).HasMaxLength(10);
            entity.Property(e => e.STICKER_WIDTH).HasMaxLength(10);
        });

        modelBuilder.Entity<tbm_pk_batch_status>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__tbm_pk_b__3214EC2743E1BEC7");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.BATCH_STATUS)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tbm_pk_production_line>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.PACKING_LINE_ID).ValueGeneratedOnAdd();
            entity.Property(e => e.PK_LINE_NAME).HasMaxLength(10);
            entity.Property(e => e.PLANT_ID).HasComment("join to tbm_plant");
            entity.Property(e => e.SLOC_ID).HasComment("join to tbm_pk_sloc");
        });

        modelBuilder.Entity<tbm_pk_sloc>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__tbm_pk_s__3214EC274E4E0337");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.SLOC)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tbm_pk_work_shift>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__tbm_pk_w__3214EC27429121C0");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.TIME_END).HasPrecision(0);
            entity.Property(e => e.TIME_START).HasPrecision(0);
            entity.Property(e => e.WORK_SHIFT).HasMaxLength(20);
        });

        modelBuilder.Entity<tbm_plant_move2stored>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CREATE_BY).HasMaxLength(10);
            entity.Property(e => e.CREATE_DATE).HasColumnType("datetime");
            entity.Property(e => e.PLANT).HasMaxLength(1);
            entity.Property(e => e.PLANT_NAME).HasMaxLength(35);
            entity.Property(e => e.UPDATE_BY).HasMaxLength(10);
            entity.Property(e => e.UPDATE_DATE).HasColumnType("datetime");
        });

        modelBuilder.Entity<tbm_product_group_move2stored>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CREATE_BY).HasMaxLength(10);
            entity.Property(e => e.CREATE_DATE).HasColumnType("datetime");
            entity.Property(e => e.GROUP_NAME).HasMaxLength(35);
            entity.Property(e => e.UPDATE_BY).HasMaxLength(10);
            entity.Property(e => e.UPDATE_DATE).HasColumnType("datetime");
            entity.Property(e => e.ZGROUP).HasMaxLength(3);
        });

        modelBuilder.Entity<tbm_product_package_move2stored>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CREATE_BY).HasMaxLength(10);
            entity.Property(e => e.CREATE_DATE).HasColumnType("datetime");
            entity.Property(e => e.UPDATE_BY).HasMaxLength(10);
            entity.Property(e => e.UPDATE_DATE).HasColumnType("datetime");
            entity.Property(e => e.ZSIZE).HasMaxLength(3);
            entity.Property(e => e.ZSIZE_NAME).HasMaxLength(35);
        });

        modelBuilder.Entity<tbm_product_type_move2stored>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CREATE_BY).HasMaxLength(10);
            entity.Property(e => e.CREATE_DATE).HasColumnType("datetime");
            entity.Property(e => e.PRODUCT_TYPE_ID).ValueGeneratedOnAdd();
            entity.Property(e => e.PRODUCT_TYPE_NAME).HasMaxLength(50);
            entity.Property(e => e.UPDATE_BY).HasMaxLength(10);
            entity.Property(e => e.UPDATE_DATE).HasColumnType("datetime");
        });

        modelBuilder.Entity<tbm_sap_sloc>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__tbm_pk_s__3214EC27C94DE153_copy1");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.SLOC)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tbm_unit_move2stored>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CREATE_BY).HasMaxLength(10);
            entity.Property(e => e.CREATE_DATE).HasColumnType("datetime");
            entity.Property(e => e.UOM).HasMaxLength(3);
            entity.Property(e => e.UOM_NAME).HasMaxLength(35);
            entity.Property(e => e.UPDATE_BY).HasMaxLength(10);
            entity.Property(e => e.UPDATE_DATE).HasColumnType("datetime");
        });

        modelBuilder.Entity<tbt_pk_batch_no_detail>(entity =>
        {
            entity.HasKey(e=>e.id);

            entity.Property(e => e.BATCH_NO).HasMaxLength(50);
            entity.Property(e => e.APPROVE_BY).HasMaxLength(50);
            entity.Property(e => e.APPROVE_DATE).HasColumnType("datetime");
            entity.Property(e => e.BATCH_STATUS).HasComment("Pass/Hold/Reject -> tbm_pk_batch_status");
            entity.Property(e => e.CREATE_BY).HasMaxLength(50);
            entity.Property(e => e.CREATE_DATE).HasColumnType("datetime");
            entity.Property(e => e.REMARK_HOLD).HasMaxLength(100);
            entity.Property(e => e.REMARK_HOLD_TO_PASS).HasMaxLength(100);
            entity.Property(e => e.REMARK_REJECT).HasMaxLength(100);
            entity.Property(e => e.UPDATE_BY).HasMaxLength(50);
            entity.Property(e => e.UPDATE_DATE).HasColumnType("datetime");
            entity.Property(e => e.WORK_SHIFT_ID).HasComment("กะการทำงาน -> tbm_pk_work_shift");
        });

        modelBuilder.Entity<tbt_pk_batch_no_header>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.Property(e => e.BATCH_NO).HasMaxLength(50);
            entity.Property(e => e.APPROVE_BY).HasMaxLength(50);
            entity.Property(e => e.APPROVE_DATE).HasColumnType("datetime");
            entity.Property(e => e.CREATE_BY).HasMaxLength(50);
            entity.Property(e => e.CREATE_DATE).HasColumnType("datetime");
            entity.Property(e => e.EXPIRE_DATE)
                .HasComment("เอามาจาก SHELF_LIFT_MONTH")
                .HasColumnType("datetime");
            entity.Property(e => e.MATERIAL_CODE).HasMaxLength(50);
            entity.Property(e => e.MFG_DATE).HasColumnType("datetime");
            entity.Property(e => e.PACKAGE)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SLOC).HasMaxLength(10);
            entity.Property(e => e.UOM)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UPDATE_BY).HasMaxLength(50);
            entity.Property(e => e.UPDATE_DATE).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
