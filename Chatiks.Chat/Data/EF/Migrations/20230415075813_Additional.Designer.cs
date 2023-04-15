﻿// <auto-generated />
using System;
using Chatiks.Chat.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Chatiks.Chat.Data.EF.Migrations
{
    [DbContext(typeof(ChatContext))]
    [Migration("20230415075813_Additional")]
    partial class Additional
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-preview.1.23111.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Chatiks.Chat.Domain.ChatBase", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp");

                    b.HasKey("Id");

                    b.ToTable("ChatBase", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Chatiks.Chat.Domain.ChatMessage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<long>("ChatUserId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("EditTime")
                        .HasColumnType("timestamp");

                    b.Property<long?>("RepliedMessageId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("SendTime")
                        .HasColumnType("timestamp");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("ChatUserId");

                    b.HasIndex("RepliedMessageId");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("Chatiks.Chat.Domain.ChatMessageImageLink", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("ChatMessageId")
                        .HasColumnType("bigint");

                    b.Property<long>("ExternalImageId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ChatMessageId");

                    b.ToTable("ImageLinks");
                });

            modelBuilder.Entity("Chatiks.Chat.Domain.ChatUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<long>("ExternalUserId")
                        .HasColumnType("bigint");

                    b.Property<long?>("InviterId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsChatCreator")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("InviterId");

                    b.ToTable("ChatUsers");
                });

            modelBuilder.Entity("Chatiks.Chat.Domain.PrivateChat", b =>
                {
                    b.HasBaseType("Chatiks.Chat.Domain.ChatBase");

                    b.ToTable("PrivateChat", (string)null);
                });

            modelBuilder.Entity("Chatiks.Chat.Domain.PublicChat", b =>
                {
                    b.HasBaseType("Chatiks.Chat.Domain.ChatBase");

                    b.ToTable("PublicChat", (string)null);
                });

            modelBuilder.Entity("Chatiks.Chat.Domain.ChatMessage", b =>
                {
                    b.HasOne("Chatiks.Chat.Domain.ChatBase", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chatiks.Chat.Domain.ChatUser", "ChatUser")
                        .WithMany("Messages")
                        .HasForeignKey("ChatUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chatiks.Chat.Domain.ChatMessage", "RepliedMessage")
                        .WithMany("Replies")
                        .HasForeignKey("RepliedMessageId");

                    b.OwnsOne("Chatiks.Chat.Domain.ValueObjects.MessageText", "Text", b1 =>
                        {
                            b1.Property<long>("ChatMessageId")
                                .HasColumnType("bigint");

                            b1.Property<string>("Value")
                                .HasColumnType("text")
                                .HasColumnName("Text");

                            b1.HasKey("ChatMessageId");

                            b1.ToTable("ChatMessages");

                            b1.WithOwner()
                                .HasForeignKey("ChatMessageId");
                        });

                    b.Navigation("Chat");

                    b.Navigation("ChatUser");

                    b.Navigation("RepliedMessage");

                    b.Navigation("Text");
                });

            modelBuilder.Entity("Chatiks.Chat.Domain.ChatMessageImageLink", b =>
                {
                    b.HasOne("Chatiks.Chat.Domain.ChatMessage", "ChatMessage")
                        .WithMany("MessageImageLinks")
                        .HasForeignKey("ChatMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChatMessage");
                });

            modelBuilder.Entity("Chatiks.Chat.Domain.ChatUser", b =>
                {
                    b.HasOne("Chatiks.Chat.Domain.ChatBase", "Chat")
                        .WithMany("ChatUsers")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chatiks.Chat.Domain.ChatUser", "Inviter")
                        .WithMany("InvitedUsers")
                        .HasForeignKey("InviterId");

                    b.Navigation("Chat");

                    b.Navigation("Inviter");
                });

            modelBuilder.Entity("Chatiks.Chat.Domain.PrivateChat", b =>
                {
                    b.HasOne("Chatiks.Chat.Domain.ChatBase", null)
                        .WithOne()
                        .HasForeignKey("Chatiks.Chat.Domain.PrivateChat", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Chatiks.Chat.Domain.PublicChat", b =>
                {
                    b.HasOne("Chatiks.Chat.Domain.ChatBase", null)
                        .WithOne()
                        .HasForeignKey("Chatiks.Chat.Domain.PublicChat", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Chatiks.Chat.Domain.ValueObjects.ChatName", "ChatName", b1 =>
                        {
                            b1.Property<long>("PublicChatId")
                                .HasColumnType("bigint");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("ChatName");

                            b1.HasKey("PublicChatId");

                            b1.ToTable("PublicChat");

                            b1.WithOwner()
                                .HasForeignKey("PublicChatId");
                        });

                    b.Navigation("ChatName")
                        .IsRequired();
                });

            modelBuilder.Entity("Chatiks.Chat.Domain.ChatBase", b =>
                {
                    b.Navigation("ChatUsers");

                    b.Navigation("Messages");
                });

            modelBuilder.Entity("Chatiks.Chat.Domain.ChatMessage", b =>
                {
                    b.Navigation("MessageImageLinks");

                    b.Navigation("Replies");
                });

            modelBuilder.Entity("Chatiks.Chat.Domain.ChatUser", b =>
                {
                    b.Navigation("InvitedUsers");

                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
