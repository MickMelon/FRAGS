using System.Collections.Generic;
using System.Text.Json;
using Frags.Core.Campaigns;
using Frags.Core.Statistics;
using Xunit;
using Xunit.Abstractions;

namespace Frags.Test.Core
{
    public class ConfigTests
    {
        private readonly ITestOutputHelper _output;

        public ConfigTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void JsonConfig_ExpEnabledChannels_EntityMatchesInput()
        {
            string json = "{ ";
            //json += "\"" + nameof(StatisticOptions) + "\": { ";
            json += "\"" + nameof(StatisticOptions.ExpEnabledChannels) + "\": ";
            ulong channel1 = 123, channel2 = 456;
            json += "[ " + "{ \"Id\": " + channel1 + ", \"IsExperienceEnabled\": true }, " + "{ \"Id\": " + channel2 + ", \"IsExperienceEnabled\": true } " + " ] ";
            //json += "} }";
            json += "}";

            StatisticOptions statOpts = JsonSerializer.Deserialize<StatisticOptions>(json);

            _output.WriteLine(json);

            foreach (var channel in statOpts.ExpEnabledChannels) 
                _output.WriteLine(channel.Id + " " + channel.IsExperienceEnabled);

            // StatisticOptions statOpts = new StatisticOptions
            // {
            //     ExpEnabledChannels = new List<Channel>{ new Channel(123), new Channel(456) }
            // };

            // JsonSerializerOptions jsonOpts = new JsonSerializerOptions
            // {
            //     WriteIndented = true
            // };

            //_output.WriteLine(statOpts.ExpEnabledChannels.ToString());
        }
    }
}