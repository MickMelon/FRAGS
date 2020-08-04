using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Statistics;
using Frags.Presentation.ViewModels.Campaigns;
using Frags.Presentation.ViewModels.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frags.Presentation.Results
{
    /// <summary>
    /// Represents a result type for rolls.
    /// </summary>
    public class CampaignResult : BaseResult
    {
        /// <summary>
        /// Initializes a new <see cref="CampaignResult" /> class.
        /// </summary>
        public CampaignResult(string message, bool success = true, object viewModel = null) 
            : base(message, success, viewModel)
        {
        }

        /// <summary>
        /// Used after adding a Channel to a Campaign successfully.
        /// </summary>
        public static CampaignResult ChannelAdded() =>
            new CampaignResult(Messages.CAMP_CHANNEL_ADDED);

        public static CampaignResult CampaignCreated() =>
            new CampaignResult(Messages.CAMP_CREATED);

        /// <summary>
        /// Used after attempting to add a Channel but it was already there.
        /// </summary>
        public static CampaignResult ChannelAlreadyPresent() =>
            new CampaignResult(Messages.CAMP_CHANNEL_ALREADY_ADDED, success: false);

        public static  CampaignResult NameChanged() =>
            new CampaignResult(Messages.CAMP_NAME_CHANGED);

        public static CampaignResult NameAlreadyExists() =>
            new CampaignResult(Messages.CAMP_EXISTING_NAME, success: false);

        public static CampaignResult NotFoundByName() =>
            new CampaignResult(Messages.CAMP_NOT_FOUND_NAME, success: false);

        public static CampaignResult NotFoundByChannel() =>
            new CampaignResult(Messages.CAMP_NOT_FOUND_CHANNEL, success: false);

        public static CampaignResult AccessDenied() =>
            new CampaignResult(Messages.CAMP_ACCESS_DENIED, success: false);

        public static CampaignResult StatisticOptionsNotFound() =>
            new CampaignResult(Messages.CAMP_STATOPTIONS_NOT_FOUND, success: false);

        public static CampaignResult InvalidProgressionStrategy() =>
            new CampaignResult(Messages.CAMP_PROGSTRATEGY_INVALID, success: false);

        public static CampaignResult CharacterConverted() =>
            new CampaignResult(Messages.CAMP_CHARACTER_CONVERTED);

        public static CampaignResult CharacterAlreadyPresent() =>
            new CampaignResult(Messages.CAMP_CHARACTER_ALREADY_ADDED, success: false);

        public static CampaignResult PropertyChanged() =>
            new CampaignResult(Messages.CAMP_PROPERTY_VALUECHANGED);

        public static CampaignResult InvalidProperty() =>
            new CampaignResult(Messages.CAMP_PROPERTY_INVALID, success: false);

        public static CampaignResult InvalidPropertyValue(string exceptionMessage) =>
            new CampaignResult(string.Format(Messages.CAMP_PROPERTY_INVALID_VALUE, exceptionMessage), success: false);

        public static IResult Show(Campaign campaign, List<Channel> channels, List<Character> characters, StatisticOptions statOptions)
        {
            ShowCampaignViewModel vm = new ShowCampaignViewModel
            {
                Name = campaign.Name,
                Owner = campaign.Owner,
                Channels = channels,
                StatisticOptions = statOptions
            };

            if (characters != null) 
                vm.CharacterNames = characters.Select(x => x.Name);

            return new CampaignResult(campaign.Name, true, vm);
        }
    }
}