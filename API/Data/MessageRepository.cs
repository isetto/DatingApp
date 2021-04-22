using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public void AddGroup(Group group)
        {
            context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await context.Groups
            .Include(c => c.Connections)
            .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int messageId)
        {
            return await context.Messages
            .Include(u => u.Sender)
            .Include(u => u.Recipient)
            .SingleOrDefaultAsync(message => message.Id == messageId);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await context.Groups
            .Include(group => group.Connections)
            .FirstOrDefaultAsync(group => group.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = context.Messages
                .OrderByDescending(message => message.MessageSent)
                .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
                .AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(message => message.RecipientUsername == messageParams.Username &&
                message.RecipientDeleted == false),  //mesages i received
                "Outbox" => query.Where(message => message.SenderUsername == messageParams.Username &&
                message.SenderDeleted == false),     //messages i sent
                _ => query.Where(message => message.RecipientUsername == messageParams.Username
                 && message.RecipientDeleted == false && message.DateRead == null)
            };
            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string reciepentUsername)
        {
            var messages = await context.Messages                   
                .Where(m => m.Recipient.UserName == currentUsername && m.RecipientDeleted == false     //get conversations of the users
                && m.Sender.UserName == reciepentUsername           
                || m.Recipient.UserName == reciepentUsername           
                && m.Sender.UserName == currentUsername && m.SenderDeleted == false             
                )
                .OrderBy(m => m.MessageSent)
                .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
                .ToListAsync();

                var unreadMessages = messages.Where(
                messages => messages.DateRead == null && //is there any unread messages for the current user that he received
                messages.RecipientUsername == currentUsername).ToList();

                if(unreadMessages.Any())
                {
                    foreach (var message in unreadMessages)
                    {
                        message.DateRead = DateTime.UtcNow;
                    }

                }
                return messages;
        }

        public void RemoveConnection(Connection connection)
        {
            context.Connections.Remove(connection);
        }
    }
}