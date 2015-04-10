using SignalR.Hubs;
using System.Collections.Concurrent;
using System.Linq;
using LiveSupport.Data;
using System.Collections.Generic;
using System;

namespace LiveSupport
{

    public class LiveSupport : Hub, IDisconnect
    {
        // Mapping between client and operator
        static List<LoggedClient> LoggedClients = new List<LoggedClient>();

        // List of all users crawling the sites - potential clients
        static List<AvailableClient> AvailableClients = new List<AvailableClient>();

        // Mapping between operator and Company
        static List<LoggedOperator> LoggedOperators = new List<LoggedOperator>();
        static MopligEntities context = new MopligEntities();

        enum Message {LogIn, GiveUp, Online, Update}


        // Client who asks for support
        public void Init(string aSupportCompanyID, Browser aBrowser, UserInfo aUserInfo)
        {
            LoggedClient client = new LoggedClient(this.Context.ConnectionId, aSupportCompanyID, aBrowser, aUserInfo);
            Browser browser = aBrowser;
            browser.LogOnTime = DateTime.Now;
            try
            {
                string[] ip = browser.IP.Split('.');
                long clientIPAddress = Convert.ToInt64(ip[0]) * 256 * 256 * 256 +
                    Convert.ToInt64(ip[1]) * 256 * 256 +
                    Convert.ToInt64(ip[2]) * 256 +
                    Convert.ToInt64(ip[3]);

                var country = (from i in context.IPtoCountries
                              where  clientIPAddress >= i.StartAddress && clientIPAddress <=i.EndAddress
                              select i.Country).FirstOrDefault();
                browser.Country = country;
                browser.LogOnTime = DateTime.Now;
            }
            catch
            {
            }

            var c = (from i in LoggedClients
                         where i.Company == aSupportCompanyID
                         && (i.ClientID == client.ClientID)
                         select i).FirstOrDefault();
            // This is indeed new client
            if (c == null)
            {
                lock (LoggedClients)
                {
                    LoggedClients.Add(client);
                }

                // Inform all supporters that a new client asks for support
                SendToAllSupporters(aSupportCompanyID, client.ClientID, Message.LogIn, aUserInfo, aBrowser);
                SendSystemMessage(client, "Waiting for an operator to contact you...");
            }
        }

        // Client who has come online
        public void Available(string aSupportCompanyID, Browser aBrowser)
        {
            AvailableClient client = new AvailableClient(this.Context.ConnectionId, aSupportCompanyID, aBrowser);
            Browser browser = aBrowser;
            try
            {
                string[] ip = browser.IP.Split('.');
                long clientIPAddress = Convert.ToInt64(ip[0]) * 256 * 256 * 256 +
                    Convert.ToInt64(ip[1]) * 256 * 256 +
                    Convert.ToInt64(ip[2]) * 256 +
                    Convert.ToInt64(ip[3]);

                var country = (from i in context.IPtoCountries
                               where clientIPAddress >= i.StartAddress && clientIPAddress <= i.EndAddress
                               select i.Country).FirstOrDefault();
                browser.Country = country;
                browser.LogOnTime = DateTime.Now;
            }
            catch
            {
                // Cannot convert ip address - most probably the address is IPv6
            }

            var c = (from i in AvailableClients
                     where i.Company == aSupportCompanyID
                     && (i.ClientID == client.ClientID)
                     select i).FirstOrDefault();
            // This is indeed new client
            if (c == null)
            {
                lock (AvailableClients)
                {
                    AvailableClients.Add(client);
                }

                // Inform all supporters that a new client is avaiable
                SendToAllSupporters(aSupportCompanyID, client.ClientID, Message.Online, null, aBrowser);
            }
        }

        // Client asks to close his session
        public void Close()
        {
            LoggedClient client = (from i in LoggedClients
                                  where i.ClientID == this.Context.ConnectionId
                                  select i).FirstOrDefault();
                if (client != null)
                {
                    if (client.AssignedOperator != null)
                    {
                        SendMessageTo(client, client.AssignedOperator, "Client left");
                        var session = (from i in context.Sessions
                                      where i.ID == client.SessionID
                                      select i).FirstOrDefault();
                        session.EndDate = DateTime.Now;
                        session.Country = client.Browser.Country;
                        session.CurrentURL = client.Browser.CurrentPage;
                        session.Question = client.Info.Question;
                        session.UserAgent = client.Browser.UserAgent;
                        session.Email = client.Info.eMail;

                        // The client has closed a working session 
                        foreach (string message in client.Messages)
                        {
                            Data.Message new_message = new Data.Message();
                            new_message.SessionID = client.SessionID;
                            new_message.Text = message;
                            context.AddToMessages(new_message);
                        }

                        context.SaveChanges();
                    }
                    else
                    {
                        // Send information to all other supporters that the client has given up
                        SendToAllSupporters(client.Company, client.ClientID, Message.GiveUp);
                    }
                }
                LoggedClients.Remove(client);

        }

        private void SendToAllSupporters(string aCompany, string aClientID, Message aMessage, UserInfo aUserInfo = null, Browser aBrowser = null)
        {
            var query = from i in LoggedOperators
                        where i.Company == aCompany
                        select i;

            foreach (LoggedOperator oper in query)
            {
                switch (aMessage){
                    case Message.LogIn: Clients[oper.ClientID].clientAsksForSupport(aClientID, aUserInfo, aBrowser);
                        break;
                    case Message.GiveUp: Clients[oper.ClientID].clientHasGivenUp(aClientID);
                        break;
                    case Message.Online: Clients[oper.ClientID].clientHasComeOnline(aClientID, aBrowser);
                        break;
                    case Message.Update: Clients[oper.ClientID].clientHasChangedDetails(aClientID, aUserInfo.InvitedToChat);
                        break;
                }
            }
        }

        private void SendToSupporter(string aOperatorID, string aClientID, Message aMessage, Browser aBrowser = null)
        {
            var oper = (from i in LoggedOperators
                        where i.ClientID == aOperatorID
                        select i).FirstOrDefault();

                switch (aMessage)
                {
                    case Message.LogIn: Clients[oper.ClientID].clientAsksForSupport(aClientID, aBrowser);
                        break;
                    case Message.GiveUp: Clients[oper.ClientID].clientHasGivenUp(aClientID);
                        break;
                    case Message.Online: Clients[oper.ClientID].clientHasComeOnline(aClientID, aBrowser);
                        break;
                }
        }

        public static bool ISAvailableOperator(string aCompany)
        {
            var query = from i in LoggedOperators
                        where i.Company == aCompany
                        select i;

            return (query.Count() > 0);        
        }

        public static int CountAvailableOperators(string aCompany)
        {
            var query = from i in LoggedOperators
                        where i.Company == aCompany
                        select i;

            return query.Count();        
        }

        public static int CountOnlineClients(string aCompany)
        {
            var query = from i in AvailableClients
                        where i.Company == aCompany
                        select i;

            return query.Count();
        }

        public void LogInAsOperator(string aSupportCompanyID)
        {
            string username = System.Web.Security.Membership.GetUser().UserName;
            LoggedOperator new_operator = new LoggedOperator(this.Context.ConnectionId, aSupportCompanyID, UserProfile.GetUserProfile(username).FullName, true);

            lock (LoggedOperators)
            {
                LoggedOperators.Add(new_operator);
            }

            foreach (LoggedClient client in LoggedClients.Where(a=>a.Company==aSupportCompanyID))
            {
                if (client.AssignedOperator == null)
                {
                    SendToSupporter(new_operator.ClientID, client.ClientID, Message.LogIn, client.Browser);
                }
            }

            foreach (AvailableClient client in AvailableClients.Where(a => a.Company == aSupportCompanyID))
            {
                    SendToSupporter(new_operator.ClientID, client.ClientID, Message.Online, client.Browser);
            }
        }

        public void OperatorLogOff()
        {
            string client_identifier = this.Context.ConnectionId;
            OperatorLogOff(client_identifier);
        }

        public void OperatorLogOff(string ClientID)
        {
            string client_identifier = this.Context.ConnectionId;
            if (ClientID != null)
            {
                client_identifier = ClientID;
            }
            var op = (from i in LoggedOperators
                          where i.ClientID == client_identifier
                          select i).FirstOrDefault();

            foreach (LoggedClient client in op.AssignedClients)
            {
                SendSystemMessage(client, "Operator left");
            }

            LoggedOperators.Remove(op);
        }

        public void SendMessage(string aMessage)
        {
            var client = (from i in LoggedClients
                         where i.ClientID == this.Context.ConnectionId
                         select i).FirstOrDefault();

            if (client != null)
            {
                SendMessageTo(client, client, aMessage);
                SendMessageTo(client, client.AssignedOperator, aMessage);
                client.Messages.Add(String.Format("{0} {1}: {2}", DateTime.Now.ToShortTimeString(), client.Info.Name, aMessage));
            }
        }

        public void SendOperatorMessage(string aMessage, string aClientID)
        {
            LoggedOperator oper = (from i in LoggedOperators
                                    where i.ClientID == this.Context.ConnectionId
                                    select i).FirstOrDefault();
            if (oper != null)
            {
                LoggedClient client = (from i in oper.AssignedClients
                                       where i.ClientID == aClientID
                                       select i).FirstOrDefault();
                SendSelfMessage(oper, client, Helper.Linkify(aMessage));
                SendMessageTo(oper, client, aMessage);
                client.Messages.Add(String.Format("{0} {1}: {2}", DateTime.Now.ToShortTimeString(), oper.Name, aMessage));
            }
            
        }

        public void Typing()
        {
            var client = (from i in LoggedClients
                         where i.ClientID == this.Context.ConnectionId
                         select i).FirstOrDefault();

            if (client != null)
            {
                PartyIsTyping(client, client.AssignedOperator);
            }
        }

        private void PartyIsTyping(ILoggedUser fromUser, ILoggedUser toUser)
        {
            Clients[toUser.ClientID].isTyping(fromUser.Name, fromUser.ClientID);        
        }

        private void SendMessageTo(ILoggedUser fromUser, ILoggedUser toUser, string aMessage)
        {
            Clients[toUser.ClientID].receive(fromUser.Name, Helper.Linkify(aMessage), fromUser.ClientID);        
        }

        private void SendSelfMessage(ILoggedUser fromUser, ILoggedUser toUser, string aMessage)
        {
            Clients[fromUser.ClientID].receive(fromUser.Name, aMessage, toUser.ClientID);
        }


        private void SendSystemMessage(ILoggedUser toUser, string aMessage)
        {
            Clients[toUser.ClientID].receive("System", aMessage, "System");        
        }

        public void InviteToChat(string ClientID)
        {
            AvailableClient client = (from i in AvailableClients
                                   where i.ClientID == ClientID
                                   select i).FirstOrDefault();
            client.InvitedToChat = true;
            Clients[client.ClientID].invite();

            // That's the operator
            LoggedOperator oper = (from i in LoggedOperators
                                   where i.ClientID == this.Context.ConnectionId
                                   select i).FirstOrDefault();
            SendToAllSupporters(oper.Company, ClientID, Message.Update, new UserInfo() { InvitedToChat = true }, null);
        }
        
        public void AcceptSupportCall(string ClientID)
        {
            // Who am I?
            LoggedOperator oper = (from i in LoggedOperators
                                  where i.ClientID == this.Context.ConnectionId
                                  select i).FirstOrDefault();

            LoggedClient client = (from i in LoggedClients
                                   where i.ClientID == ClientID
                                   select i).FirstOrDefault();

            oper.AssignedClients.Add(client);
            client.AssignedOperator = oper;
            SendSystemMessage(client, "Operator connected");
            Clients[client.ClientID].operatorConnected(oper.Name);

            // New session was established
            System.Guid currentUserID = (System.Guid)System.Web.Security.Membership.GetUser().ProviderUserKey;
            Session session = new Session();
            session.StartDate = DateTime.Now;
            var current_user = (from i in context.Users
                               where i.UserId == currentUserID
                               select i).FirstOrDefault();
            session.User = current_user;
            session.Client = client.Info.Name;
            context.Sessions.AddObject(session);
            context.SaveChanges();
            client.SessionID = session.ID;

        }

        public System.Threading.Tasks.Task Disconnect() 
        {
            var client = from i in LoggedClients
                         where i.ClientID == Context.ConnectionId
                         select i;

            var op = (from i in LoggedOperators
                         where i.ClientID == Context.ConnectionId
                         select i).FirstOrDefault();

            var onlineClient = (from i in AvailableClients
                         where i.ClientID == Context.ConnectionId
                         select i).FirstOrDefault();
            
            if (client != null)
            {
                Close();
            }

            if (op != null)
            {
                OperatorLogOff(op.ClientID);
            }

            if (onlineClient != null)
            {
                SendToAllSupporters(onlineClient.Company, onlineClient.ClientID, Message.GiveUp);
                lock (AvailableClients)
                {
                    AvailableClients.Remove(onlineClient);
                }
            }

            return Clients.disconnected("");
        }

    }
}