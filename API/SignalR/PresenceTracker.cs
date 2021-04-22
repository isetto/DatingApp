using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = 
        new Dictionary<string, List<string>>(); //dictionary will be shared to everyone that joins app which is not thread safe resource
        //if users want to update dictionary in the same time then there will be a problem 
        public Task UserConnected(string username, string connectionId)
        {
            lock(OnlineUsers)   //to solve above problem we need to lock a dictionary. We lock dictionary until we finish what we were doing
            {
                if(OnlineUsers.ContainsKey(username))   //if we have user then we add connectionId to the list
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string>{connectionId});
                }
            }
            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionId)
        {
            lock(OnlineUsers)
            {
                   if(!OnlineUsers.ContainsKey(username)) return Task.CompletedTask;
                   OnlineUsers[username].Remove(connectionId);
                    if (OnlineUsers[username].Count == 0)
                    {
                        OnlineUsers.Remove(username);
                    }
            }
            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock(OnlineUsers){
                onlineUsers = OnlineUsers.OrderBy(key => key.Key).Select(key => key.Key).ToArray(); //it happens in memory of backend
            }
            return Task.FromResult(onlineUsers);
        }

    }
}