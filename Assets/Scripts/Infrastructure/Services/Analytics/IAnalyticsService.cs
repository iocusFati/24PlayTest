using System.Collections.Generic;

namespace Infrastructure.Services.Analytics
{
    public interface IAnalyticsService
    {
        void SendEvent(string eventName);
        void SendEvent(string eventName, Dictionary<string,string> parameters);
    }
}