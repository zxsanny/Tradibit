﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tradibit.DataAccess;

#nullable disable

namespace Tradibit.Api.Migrations
{
    [DbContext(typeof(TradibitDb))]
    [Migration("20230126070646_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Tradibit.Shared.Entities.BaseOperation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("OperationType")
                        .HasColumnType("integer");

                    b.Property<int>("OrderNo")
                        .HasColumnType("integer");

                    b.Property<Guid>("TransitionId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TransitionId");

                    b.ToTable("BaseOperation");

                    b.HasDiscriminator<int>("OperationType");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.Condition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Operation")
                        .HasColumnType("integer");

                    b.Property<Guid>("TransitionId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TransitionId");

                    b.ToTable("Condition");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.Scenario", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CurrentStepId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("StrategyId")
                        .HasColumnType("uuid");

                    b.Property<string>("UserVars")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("StrategyId");

                    b.ToTable("Scenarios");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.Step", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid>("StrategyId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("StrategyId");

                    b.ToTable("Steps");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.Strategy", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<Guid>("InitialStepId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Strategies");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.StrategyUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("StrategyId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("StrategyId");

                    b.HasIndex("UserId");

                    b.ToTable("StrategyUsers");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.Transition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("DestinationStepId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid>("StepId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("StepId");

                    b.ToTable("Transition");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("BinanceKeyHash")
                        .HasColumnType("text");

                    b.Property<string>("BinanceSecretHash")
                        .HasColumnType("text");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int[]>("Permissions")
                        .HasColumnType("integer[]");

                    b.Property<Guid?>("UserStateId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("UserStateId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.UserFund", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Value")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("UserFunds");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.UserSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MaxActiveTradings")
                        .HasColumnType("integer");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.UserState", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ActivePairs")
                        .HasColumnType("text");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("CurrentDeposit")
                        .HasColumnType("numeric");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserState");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.OrderBaseOperation", b =>
                {
                    b.HasBaseType("Tradibit.Shared.Entities.BaseOperation");

                    b.Property<int>("OrderSide")
                        .HasColumnType("integer");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.SetOperandBaseOperation", b =>
                {
                    b.HasBaseType("Tradibit.Shared.Entities.BaseOperation");

                    b.HasDiscriminator().HasValue(2);
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.BaseOperation", b =>
                {
                    b.HasOne("Tradibit.Shared.Entities.Transition", "Transition")
                        .WithMany("SuccessOperations")
                        .HasForeignKey("TransitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Transition");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.Condition", b =>
                {
                    b.HasOne("Tradibit.Shared.Entities.Transition", "Transition")
                        .WithMany("Conditions")
                        .HasForeignKey("TransitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Tradibit.Shared.Entities.Operand", "Operand1", b1 =>
                        {
                            b1.Property<Guid>("ConditionId")
                                .HasColumnType("uuid");

                            b1.Property<int?>("Indicator")
                                .HasColumnType("integer");

                            b1.Property<decimal?>("NumValue")
                                .HasColumnType("numeric");

                            b1.Property<int?>("Quote")
                                .HasColumnType("integer");

                            b1.Property<string>("UserVarName")
                                .HasColumnType("text");

                            b1.HasKey("ConditionId");

                            b1.ToTable("Condition");

                            b1.WithOwner()
                                .HasForeignKey("ConditionId");
                        });

                    b.OwnsOne("Tradibit.Shared.Entities.Operand", "Operand2", b1 =>
                        {
                            b1.Property<Guid>("ConditionId")
                                .HasColumnType("uuid");

                            b1.Property<int?>("Indicator")
                                .HasColumnType("integer");

                            b1.Property<decimal?>("NumValue")
                                .HasColumnType("numeric");

                            b1.Property<int?>("Quote")
                                .HasColumnType("integer");

                            b1.Property<string>("UserVarName")
                                .HasColumnType("text");

                            b1.HasKey("ConditionId");

                            b1.ToTable("Condition");

                            b1.WithOwner()
                                .HasForeignKey("ConditionId");
                        });

                    b.Navigation("Operand1")
                        .IsRequired();

                    b.Navigation("Operand2")
                        .IsRequired();

                    b.Navigation("Transition");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.Scenario", b =>
                {
                    b.HasOne("Tradibit.Shared.Entities.Strategy", "Strategy")
                        .WithMany("Scenarios")
                        .HasForeignKey("StrategyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Tradibit.SharedUI.DTO.Primitives.Pair", "Pair", b1 =>
                        {
                            b1.Property<Guid>("ScenarioId")
                                .HasColumnType("uuid");

                            b1.HasKey("ScenarioId");

                            b1.ToTable("Scenarios");

                            b1.WithOwner()
                                .HasForeignKey("ScenarioId");

                            b1.OwnsOne("Tradibit.SharedUI.DTO.Primitives.Currency", "BaseCurrency", b2 =>
                                {
                                    b2.Property<Guid>("PairScenarioId")
                                        .HasColumnType("uuid");

                                    b2.HasKey("PairScenarioId");

                                    b2.ToTable("Scenarios");

                                    b2.WithOwner()
                                        .HasForeignKey("PairScenarioId");
                                });

                            b1.OwnsOne("Tradibit.SharedUI.DTO.Primitives.Currency", "QuoteCurrency", b2 =>
                                {
                                    b2.Property<Guid>("PairScenarioId")
                                        .HasColumnType("uuid");

                                    b2.HasKey("PairScenarioId");

                                    b2.ToTable("Scenarios");

                                    b2.WithOwner()
                                        .HasForeignKey("PairScenarioId");
                                });

                            b1.Navigation("BaseCurrency")
                                .IsRequired();

                            b1.Navigation("QuoteCurrency")
                                .IsRequired();
                        });

                    b.Navigation("Pair")
                        .IsRequired();

                    b.Navigation("Strategy");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.Step", b =>
                {
                    b.HasOne("Tradibit.Shared.Entities.Strategy", "Strategy")
                        .WithMany("Steps")
                        .HasForeignKey("StrategyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Strategy");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.StrategyUser", b =>
                {
                    b.HasOne("Tradibit.Shared.Entities.Strategy", "Strategy")
                        .WithMany("Users")
                        .HasForeignKey("StrategyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tradibit.Shared.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Strategy");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.Transition", b =>
                {
                    b.HasOne("Tradibit.Shared.Entities.Step", "Step")
                        .WithMany("Transitions")
                        .HasForeignKey("StepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Step");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.User", b =>
                {
                    b.HasOne("Tradibit.Shared.Entities.UserState", "UserState")
                        .WithMany()
                        .HasForeignKey("UserStateId");

                    b.Navigation("UserState");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.UserSettings", b =>
                {
                    b.HasOne("Tradibit.Shared.Entities.User", null)
                        .WithOne("UserSettings")
                        .HasForeignKey("Tradibit.Shared.Entities.UserSettings", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.UserState", b =>
                {
                    b.HasOne("Tradibit.Shared.Entities.User", null)
                        .WithMany("HistoryUserState")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.SetOperandBaseOperation", b =>
                {
                    b.OwnsOne("Tradibit.Shared.Entities.Operand", "OperandSource", b1 =>
                        {
                            b1.Property<Guid>("SetOperandBaseOperationId")
                                .HasColumnType("uuid");

                            b1.Property<int?>("Indicator")
                                .HasColumnType("integer");

                            b1.Property<decimal?>("NumValue")
                                .HasColumnType("numeric");

                            b1.Property<int?>("Quote")
                                .HasColumnType("integer");

                            b1.Property<string>("UserVarName")
                                .HasColumnType("text");

                            b1.HasKey("SetOperandBaseOperationId");

                            b1.ToTable("BaseOperation");

                            b1.WithOwner()
                                .HasForeignKey("SetOperandBaseOperationId");
                        });

                    b.OwnsOne("Tradibit.Shared.Entities.Operand", "OperandTo", b1 =>
                        {
                            b1.Property<Guid>("SetOperandBaseOperationId")
                                .HasColumnType("uuid");

                            b1.Property<int?>("Indicator")
                                .HasColumnType("integer");

                            b1.Property<decimal?>("NumValue")
                                .HasColumnType("numeric");

                            b1.Property<int?>("Quote")
                                .HasColumnType("integer");

                            b1.Property<string>("UserVarName")
                                .HasColumnType("text");

                            b1.HasKey("SetOperandBaseOperationId");

                            b1.ToTable("BaseOperation");

                            b1.WithOwner()
                                .HasForeignKey("SetOperandBaseOperationId");
                        });

                    b.Navigation("OperandSource")
                        .IsRequired();

                    b.Navigation("OperandTo")
                        .IsRequired();
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.Step", b =>
                {
                    b.Navigation("Transitions");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.Strategy", b =>
                {
                    b.Navigation("Scenarios");

                    b.Navigation("Steps");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.Transition", b =>
                {
                    b.Navigation("Conditions");

                    b.Navigation("SuccessOperations");
                });

            modelBuilder.Entity("Tradibit.Shared.Entities.User", b =>
                {
                    b.Navigation("HistoryUserState");

                    b.Navigation("UserSettings");
                });
#pragma warning restore 612, 618
        }
    }
}
