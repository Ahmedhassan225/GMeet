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

        public Task UserConnected(string username, string connectionId){
            
            //lock untill the what is inside of the {} do 
            lock(onLineUsers){
                if(onLineUsers.ContainsKey(username)){
                    onLineUsers[username].Add(connectionId);
                }
                else{
                    onLineUsers.Add(username, new List<string>{connectionId});
                }
            }
            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionId){
            lock(onLineUsers){
                if(!onLineUsers.ContainsKey(username)){
                    return Task.CompletedTask;
                }
                
                onLineUsers[username].Remove(connectionId);
                if(onLineUsers[username].Count == 0)
                    onLineUsers.Remove(username);

            }

            return Task.CompletedTask ;
        }

        public Task<string[]> getOnlineUsers(){
            string[] onlineUserslist ;
            lock(onLineUsers){
                onlineUserslist = onLineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUserslist);
        }
    }
}