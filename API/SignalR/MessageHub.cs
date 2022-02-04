using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;
        private readonly IHubContext<PresenceHub> presenceHub;
        private readonly PresenceTracker tracker;

        public MessageHub(IMessageRepository messageRepository, IMapper mapper
            , IUserRepository userRepository, IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
        {
            this.messageRepository = messageRepository;
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.presenceHub = presenceHub;
            this.tracker = tracker;
        }

        public override async Task OnConnectedAsync(){

            var httpContext = Context.GetHttpContext();
            var currentUser = Context.User.GetUserName();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(currentUser, otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await messageRepository.GetMessageThread(currentUser, otherUser);

            // not optimal i think now we will send thread to both users every time.
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception){
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup",group);
            await base.OnDisconnectedAsync(exception);

        }

        public async Task SendMessage(CreateMessageDto createMessageDto){
            var userName = Context.User.GetUserName();

            if(userName.ToLower() == createMessageDto.RecipientUserName.ToLower()){
                throw new HubException("You Cannot send messages to yourself");
            }

            var sender = await userRepository.GetUserByUserNameAsync(userName);
            var recipient = await userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUserName);
            
            if(recipient == null) throw new HubException("Not Found User");

            var message = new Message{
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var group = await messageRepository.GetMessageGroup(groupName);

            if(group.Connections.Any(x => x.Username == recipient.UserName)){
                message.DateRead = DateTime.UtcNow;
            }
            else{
                var connections = await tracker.GetConnectionsForUser(recipient.UserName);
                if(connections != null){
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageRecieved"
                        , new {userName = sender.UserName, knownAss = sender.KnownAs});
                }
            }

            messageRepository.AddMessage(message);

            if(await messageRepository.SaveAllAsync()) 
            {
                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
            }
        }

        private string GetGroupName(string caller, string other){
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<Group> AddToGroup(string groupName){
            var group = await messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

            if(group == null){
                group = new Group(groupName);
                messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if(await messageRepository.SaveAllAsync())
                return group;

            throw new HubException("Failed to Join Group");
        }

        private async Task<Group> RemoveFromMessageGroup(){
            var group = await messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x =>x.ConnectionId == Context.ConnectionId);
            messageRepository.RemoveConnection(connection);

            if(await messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to Remove From Group");
        }
    }
}