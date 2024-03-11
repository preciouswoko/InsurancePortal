using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace InsuranceInfrastructure.Services
{
    public class UserSessionManager
    {
        private static readonly ConcurrentDictionary<string, string> UserSessions = new ConcurrentDictionary<string, string>();

        public static bool IsUserAlreadyLoggedIn(string userId, string sessionId)
        {
            return UserSessions.TryGetValue(userId, out var existingSessionId) && existingSessionId != sessionId;
        }

        public static void AddUserSession(string userId, string sessionId)
        {
            UserSessions.AddOrUpdate(userId, _ => sessionId, (_, existingSessionId) => sessionId);
        }


        public static void RemoveUserSession(string userId)
        {
            UserSessions.TryRemove(userId, out _);
        }
    }

}
