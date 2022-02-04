using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string,  List<string>> onLineUsers = 
        new Dictionary<string, List<string>>();

        public Task<bool> UserConnected(string username, string connectionId){
            
            bool isOnline = false;
            lock(onLineUsers){
                if(onLineUsers.ContainsKey(username)){
                    onLineUsers[username].Add(connectionId);
                }
                else{
                    onLineUsers.Add(username, new List<string>{connectionId});
                    isOnline = true;
                }
            }
            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectionId){
            bool isOffline = false;
            lock(onLineUsers){
                if(!onLineUsers.ContainsKey(username)){
                    return Task.FromResult(isOffline);
                }
                
                onLineUsers[username].Remove(connectionId);
                if(onLineUsers[username].Count == 0)
                {
                    onLineUsers.Remove(username);
                    isOffline = true;
                }

            }

            return Task.FromResult(isOffline);
        }

        public Task<string[]> getOnlineUsers(){
            string[] onlineUserslist ;
            lock(onLineUsers){
                onlineUserslist = onLineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUserslist);
        }

        public Task<List<string>> GetConnectionsForUser(string username){
            List<string> connectionIds;
            lock(onLineUsers){
                connectionIds = onLineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionIds);
        }
    }
}