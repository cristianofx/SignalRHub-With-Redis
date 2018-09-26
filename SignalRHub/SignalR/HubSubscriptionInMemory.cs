using Newtonsoft.Json;
using SignalRHub.Models.Config;
using SignalRHub.Models.Models;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SignalRHub.SignalR
{
    public class HubSubscriptionInMemory
    {
        private AppSettings _appSettings;
        ConnectionMultiplexer redisDb;
        IDatabase subscriptionDb;

        public HubSubscriptionInMemory(AppSettings appSettings)
        {
            _appSettings = appSettings;
            var options = ConfigurationOptions.Parse($"{_appSettings.Redis.Host}:{_appSettings.Redis.Port},password={_appSettings.Redis.Password},defaultDatabase={Constants.RedisSignalRSubscriptionDatabase}");
            options.AllowAdmin = true;
            redisDb = ConnectionMultiplexer.Connect(options);
            subscriptionDb = redisDb.GetDatabase();
            FlushConnections();
        }

        public void Subscribe(string process, string userId, string connectionId)
        {
            var userInfo = new HubSubscriptionInfo
            {
                UserId = userId,
                Process = process,
                ConnectionId = connectionId
            };
            subscriptionDb.SetAdd(process, JsonConvert.SerializeObject(userInfo));
        }

        public void RemoveAllSubscriptionFromClient(string connectionId)
        {
            List<HubSubscriptionInfo> subscriptionMembers = new List<HubSubscriptionInfo>();
            RedisKey[] keys = GetAllKeys();
            foreach (var key in keys)
            {
                var keyString = key.ToString();
                var setscan = subscriptionDb.SetScan(key);
                foreach (var value in setscan)
                {
                    var sub = JsonConvert.DeserializeObject<HubSubscriptionInfo>(value.ToString());
                    if (sub.ConnectionId == connectionId)
                        subscriptionDb.SetRemove(key, value);
                }
            }
        }

        public void RemoveSubscription(string process, string connectionId)
        {
            List<HubSubscriptionInfo> subscriptionMembers = new List<HubSubscriptionInfo>();
            RedisKey[] keys = GetAllKeys();
            foreach (var key in keys)
            {
                var keyString = key.ToString();
                if (keyString == process)
                {
                    var setscan = subscriptionDb.SetScan(key);
                    foreach (var value in setscan)
                    {
                        var sub = JsonConvert.DeserializeObject<HubSubscriptionInfo>(value.ToString());
                        if (sub.ConnectionId == connectionId)
                            subscriptionDb.SetRemove(key, value);
                    }
                }
            }
        }

        public IEnumerable<HubSubscriptionInfo> GetAllUsersByProcess(string process)
        {
            return GetAllConnections().Where(item => item.Process == process);
        }

        public IEnumerable<HubSubscriptionInfo> GetAllConnections()
        {
            List<HubSubscriptionInfo> subscriptionMembers = new List<HubSubscriptionInfo>();
            RedisKey[] keys = GetAllKeys();
            foreach (var key in keys)
            {
                var keyString = key.ToString();
                var setscan = subscriptionDb.SetScan(key);
                foreach (var value in setscan)
                {
                    subscriptionMembers.Add(JsonConvert.DeserializeObject<HubSubscriptionInfo>(value.ToString()));
                }
            }
            return subscriptionMembers;
        }

        private RedisKey[] GetAllKeys()
        {
            EndPoint endPoint = redisDb.GetEndPoints().First();
            var endpoints = redisDb.GetEndPoints();
            RedisKey[] keys = redisDb.GetServer(endPoint).Keys(Constants.RedisSignalRSubscriptionDatabase).ToArray();
            return keys;
        }

        public HubSubscriptionInfo GetUserInfo(string connectionId)
        {
            return GetAllConnections().Where(item => item.ConnectionId == connectionId).FirstOrDefault();
        }

        private void FlushConnections()
        {
            EndPoint endPoint = redisDb.GetEndPoints().First();
            var server = redisDb.GetServer(endPoint);
            server.FlushDatabase(Constants.RedisSignalRSubscriptionDatabase);
        }
    }
}
