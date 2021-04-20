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

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int messageId)
        {
            return await context.Messages
            .Include(u => u.Sender)
            .Include(u => u.Recipient)
            .SingleOrDefaultAsync(message => message.Id == messageId);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = context.Messages
                .OrderByDescending(message => message.MessageSent)
                .AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(message => message.Recipient.UserName == messageParams.Username &&
                message.RecipientDeleted == false),  //mesages i received
                "Outbox" => query.Where(message => message.Sender.UserName == messageParams.Username &&
                message.SenderDeleted == false),     //messages i sent
                _ => query.Where(message => message.Recipient.UserName == messageParams.Username
                 && message.RecipientDeleted == false && message.DateRead == null)
            };
            var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string reciepentUsername)
        {
            var messages = await context.Messages                   
                .Include(u => u.Sender).ThenInclude(photo => photo.Photos)
                .Include(u => u.Recipient).ThenInclude(photo => photo.Photos)
                .Where(m => m.Recipient.UserName == currentUsername && m.RecipientDeleted == false     //get conversations of the users
                && m.Sender.UserName == reciepentUsername           
                || m.Recipient.UserName == reciepentUsername           
                && m.Sender.UserName == currentUsername && m.SenderDeleted == false             
                )
                .OrderBy(m => m.MessageSent)
                .ToListAsync();

                var unreadMessages = messages.Where(
                messages => messages.DateRead == null && //is there any unread messages for the current user that he received
                messages.Recipient.UserName == currentUsername).ToList();

                if(unreadMessages.Any())
                {
                    foreach (var message in unreadMessages)
                    {
                        message.DateRead = DateTime.Now;
                    }

                    await context.SaveChangesAsync();
                }
                return mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}