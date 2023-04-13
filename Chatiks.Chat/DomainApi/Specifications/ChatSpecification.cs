using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chatiks.Chat.Domain;
using LinqKit;
using LinqSpecs;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Chat.DomainApi.Specifications;

// TODO Refactor
public class ChatSpecification : Specification<ChatBase>
{
    public long[] Ids { get; private set; }
    public bool? IsPrivate { get; private set; }
    public string? NameFilter { get; private set; }
    public long? WithExternalUserId { get; private set; }
    public IReadOnlyCollection<long> RequiredIncludedIds { get; private set; }


    protected ChatSpecification(string nameFilter, long? withExternalUserId, long[] requiredIncludedIds)
    {
        NameFilter = nameFilter;
        WithExternalUserId = withExternalUserId;
        RequiredIncludedIds = requiredIncludedIds;
    }

    public ChatSpecification WithNameFilter(string nameFilter)
    {
        if (string.IsNullOrEmpty(nameFilter))
        {
            return this;
        }

        if (IsPrivate == true)
        {
            throw new Exception("Cannot filter private chats by name");
        }
        
        var newSpecification = new ChatSpecification(nameFilter, WithExternalUserId, RequiredIncludedIds?.ToArray());
        
        newSpecification.NameFilter = nameFilter;

        return newSpecification;
    }

    public ChatSpecification HavingUser(long externalUserId)
    {
        var newSpecification = new ChatSpecification(NameFilter, WithExternalUserId, RequiredIncludedIds?.ToArray());
        
        newSpecification.WithExternalUserId = externalUserId;
        
        return newSpecification;
    }

    public ChatSpecification WithRequiredChatIds(long[] requiredIncludedIds)
    {
        var newSpecification = new ChatSpecification(NameFilter, WithExternalUserId, RequiredIncludedIds?.ToArray());
        
        newSpecification.RequiredIncludedIds = requiredIncludedIds;
        
        return newSpecification;
    }
    
    public ChatSpecification OnlyPrivate()
    {
        if (NameFilter != null)
        {
            throw new Exception("Cannot filter private chats by name");
        }
        
        var newSpecification = new ChatSpecification(NameFilter, WithExternalUserId, RequiredIncludedIds?.ToArray());
        
        newSpecification.IsPrivate = true;
        
        return newSpecification;
    }

    public override Expression<Func<ChatBase, bool>> ToExpression()
    {
        var trueExpression = PredicateBuilder.New<ChatBase>(true);
        
        if (!string.IsNullOrEmpty(NameFilter))
        {
            trueExpression = trueExpression.And(x => x is PublicChat && EF.Functions.Like((x as PublicChat).ChatName.Value, $"%{NameFilter}%"));
        }
        
        if (WithExternalUserId.HasValue)
        {
            trueExpression = trueExpression.And(x => x.ChatUsers.Any(u => u.ExternalUserId == WithExternalUserId.Value));
        }
        
        if (RequiredIncludedIds != null && RequiredIncludedIds.Any())
        {
            trueExpression = trueExpression.And(x => RequiredIncludedIds.Contains(x.Id));
        }
        
        if (IsPrivate == true)
        {
            trueExpression = trueExpression.And(x => x is PrivateChat);
        }
        
        if (Ids != null && Ids.Any())
        {
            trueExpression = trueExpression.And(x => Ids.Contains(x.Id));
        }
        
        return trueExpression;
    }
    
    public static ChatSpecification Empty()
    {
        return new ChatSpecification(null, null, null);
    }
}