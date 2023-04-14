using Chatiks.Chat.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Chat.Data.EF;

public class ChatContext : DbContext
{
    public DbSet<ChatBase> Chats { get; set; }
    public DbSet<ChatUser> ChatUsers { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatMessageImageLink> ImageLinks { get; set; }
    
    public ChatContext(DbContextOptions<ChatContext> options) : base(options)
    {

    }
    
    public ChatContext()
    {

    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseNpgsql("");
    //     
    //     base.OnConfiguring(optionsBuilder);
    // }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ChatBase>(cb =>
        {
            cb.HasKey(x => x.Id);

            cb.HasMany(x => x.ChatUsers)
                .WithOne(x => x.Chat)
                .HasForeignKey(x => x.ChatId);
            
            cb.HasMany(x => x.Messages)
                .WithOne(x => x.Chat)
                .HasForeignKey(x => x.ChatId);

            cb.Property(x => x.CreationTime)
                .HasColumnType("timestamp");

            cb.ToTable("ChatBase");
        });

        builder.Entity<PrivateChat>(pc =>
        {
            pc.ToTable("PrivateChat");
        });
        
        builder.Entity<PublicChat>(pc =>
        {
            pc.OwnsOne(x => x.ChatName, cn =>
            {
                cn.Property(x => x.Value)
                    .HasColumnName("ChatName")
                    .IsRequired();
            });
            
            pc.ToTable("PublicChat");
        });

        builder.Entity<ChatMessage>(cm =>
        {
            cm.HasKey(x => x.Id);
            
            cm.HasOne(x => x.ChatUser)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ChatUserId);

            cm.HasOne(x => x.Chat)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ChatId);
            
            cm.Property(x => x.SendTime)
                .HasColumnType("timestamp")
                .IsRequired();
            
            cm.Property(x => x.EditTime)
                .HasColumnType("timestamp")
                .IsRequired(false);
            
            cm.OwnsOne(x => x.Text, text =>
            {
                text.Property(x => x.Value)
                    .HasColumnName("Text")
                    .IsRequired();
            });
        });
        
        builder.Entity<ChatMessageImageLink>(cmil =>
        {
            cmil.HasKey(x => x.Id);
            
            cmil.HasOne(x => x.ChatMessage)
                .WithMany(x => x.MessageImageLinks)
                .HasForeignKey(x => x.ChatMessageId);
        });

        builder.Entity<ChatUser>(cu =>
        {
            cu.HasKey(x => x.Id);
            
            cu.HasOne(x => x.Inviter)
                .WithMany(x => x.InvitedUsers)
                .HasForeignKey(x => x.InviterId);
        });
    }


}