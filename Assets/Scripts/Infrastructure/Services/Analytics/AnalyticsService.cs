using System.Collections.Generic;
using AppsFlyerSDK;

namespace Infrastructure.Services.Analytics
{
    public class AnalyticsService : IAnalyticsService
    {
        public void SendEvent(string eventName) => 
            AppsFlyer.sendEvent(eventName, new Dictionary<string, string>()
            {
                {string.Empty, string.Empty}
            });
        
        public void SendEvent(string eventName, Dictionary<string,string> parameters) =>
            AppsFlyer.sendEvent(eventName, parameters);
    }
}