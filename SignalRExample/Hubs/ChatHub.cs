using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using SignalRExample.Models;

namespace SignalRExample.Hubs
{

    public class ChatHub : Hub
    {

        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();

        public void SendMessage(string name, string message)
        {
            Clients.Others.GetMessageOther(name, message);

            Clients.Caller.GetMessageCaller(message);
        }

        public void Connect(string userName)
        {
            var id = Context.ConnectionId;


            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = userName });

                // send to caller
                Clients.Caller.onConnected(id, userName, ConnectedUsers, CurrentMessage);
                // send to all except caller client
                Clients.AllExcept(id).onNewUserConnected(id, userName);

            }

        }

       

        public override Task OnConnected()
        {
            //    Clients.Others.GetMessageOther(Context.User.Identity.Name, "Kullanıcı Katıldı");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.UserName);

            }
            return base.OnDisconnected(stopCalled);
        }
    }
}