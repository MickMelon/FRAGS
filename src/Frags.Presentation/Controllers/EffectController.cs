using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Common.Exceptions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Progression;
using Frags.Core.Effects;
using Frags.Presentation.Results;
using Frags.Presentation.ViewModels;
using Frags.Core.Statistics;
using Frags.Core.Campaigns;
using Frags.Core.Common.Extensions;

namespace Frags.Presentation.Controllers
{
    public class EffectController
    {
        /// <summary>
        /// Used to interact with the character database.
        /// </summary>
        private readonly ICharacterProvider _charProvider;

        /// <summary>
        /// Used to interact with the Effect database.
        /// </summary>
        private readonly IEffectProvider _effectProvider;

        private readonly IStatisticProvider _statProvider;

        /// <summary>
        /// Used to determine the limit of effects for one user.
        /// </summary>
        private readonly GeneralOptions _options;

        private readonly ICampaignProvider _campProvider;

        public EffectController(ICharacterProvider charProvider, 
            IEffectProvider effectProvider, 
            IStatisticProvider statProvider,
            GeneralOptions options,
            ICampaignProvider campProvider)
        {
            _charProvider = charProvider;
            _effectProvider = effectProvider;
            _statProvider = statProvider;
            _options = options;
            _campProvider = campProvider;
        }

        /// <summary>
        /// Creates a new Effect in the database associated with a campaign.
        /// </summary>
        /// <param name="callerId">The user identifier of the owner of the new effect.</param>
        /// <param name="effectName">The name for the new effect.</param>
        /// <param name="channelId">The channel the command was executed in to find the campaign.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> CreateCampaignEffectAsync(ulong callerId, string effectName, ulong channelId) =>
            await CreateEffectAsync(callerId, effectName, channelId, useCampaigns: true);

        public async Task<IResult> CreateEffectAsync(ulong callerId, string effectName) =>
            await CreateEffectAsync(callerId, effectName, 0, useCampaigns: false);

        private async Task<IResult> CreateEffectAsync(ulong callerId, string effectName, ulong channelId, bool useCampaigns)
        {
            Campaign campaign = null;

            if (useCampaigns)
            {
                campaign = await _campProvider.GetCampaignAsync(channelId);
                if (campaign == null) return CampaignResult.NotFoundByChannel();
                if (!await _campProvider.HasPermissionAsync(campaign, callerId)) return CampaignResult.AccessDenied();
            }
            
            if (await _effectProvider.GetEffectAsync(effectName, campaign) != null)
                return EffectResult.NameAlreadyExists();

            if ((await _effectProvider.GetOwnedEffectsAsync(callerId)).Count() >= _options.EffectsLimit)
                return EffectResult.TooManyEffects();

            var result = await _effectProvider.CreateEffectAsync(callerId, effectName, campaign);

            if (result == null) return EffectResult.EffectCreationFailed();
            return EffectResult.EffectCreatedSuccessfully();
        }

        /// <summary>
        /// Returns a result containing a string of effects created by the given user.
        /// </summary>
        /// <param name="id">The user identifier to show effects created by.</param>
        /// <returns>A GenericResult with a Message property containing the user's created effects.</returns>
        public async Task<IResult> ListCreatedEffectsAsync(ulong callerId)
        {
            var effects = await _effectProvider.GetOwnedEffectsAsync(callerId);
            return GenericResult.Generic(string.Join("\n", effects.OrderBy(x => x.Id).Select(x => x.Name)));
        }

        /// <summary>
        /// Sets the statistic effects of the specified effect that is associated with a campaign.
        /// </summary>
        /// <param name="callerId">The user identifier of the moderator or owner of the campaign.</param>
        /// <param name="effectName">The name of the effect to set the value to.</param>
        /// <param name="statName">The name of the statistic to associate the value with.</param>
        /// <param name="value">The value to add (or subtract) to the statistic.</param>
        /// <param name="channelId">The channel the command was executed in to find the campaign.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> SetCampaignStatisticEffectAsync(ulong callerId, string effectName, string statName, int value, ulong channelId) =>
            await SetStatisticEffectAsync(callerId, effectName, statName, value, channelId, useCampaigns: true);

        /// <summary>
        /// Sets the statistic effects of the specified effect that is associated with a campaign.
        /// </summary>
        /// <param name="callerId">The user identifier of the owner of the effect.</param>
        /// <param name="effectName">The name of the effect to set the value to.</param>
        /// <param name="statName">The name of the statistic to associate the value with.</param>
        /// <param name="value">The value to add (or subtract) to the statistic.</param>
        /// <param name="channelId">The channel the command was executed in to find the campaign.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> SetStatisticEffectAsync(ulong callerId, string effectName, string statName, int value) =>
            await SetStatisticEffectAsync(callerId, effectName, statName, value, 0, useCampaigns: false);

        private async Task<IResult> SetStatisticEffectAsync(ulong callerId, string effectName, string statName, int value, ulong channelId, bool useCampaigns)
        {
            Campaign campaign = null;

            if (useCampaigns)
            {
                campaign = await _campProvider.GetCampaignAsync(channelId);
                if (campaign == null) return CampaignResult.NotFoundByChannel();
                if (!await _campProvider.HasPermissionAsync(campaign, callerId)) return CampaignResult.AccessDenied();
            }

            var effect = await _effectProvider.GetEffectAsync(effectName, campaign);
            if (effect == null) return EffectResult.EffectNotFound();

            if (!useCampaigns && effect.Owner.UserIdentifier != callerId) return EffectResult.AccessDenied();

            var stat = await _statProvider.GetStatisticAsync(statName, campaign);
            if (stat == null) return StatisticResult.StatisticNotFound();

            if (effect.Statistics.ContainsKey(stat))
            {
                effect.Statistics[stat].Value = value;
            }
            else
            {
                effect.Statistics.Add(stat, new StatisticValue(value));
            }

            await _effectProvider.UpdateEffectAsync(effect);
            return EffectResult.EffectUpdatedSucessfully();
        }

        /// <summary>
        /// Renames an already existing Effect.
        /// </summary>
        /// <param name="callerId">The user identifier of the owner of the effect.</param>
        /// <param name="effectName">The name of the Effect to rename.</param>
        /// <param name="newName">The new name of the Effect.</param>
        /// <returns>A result detailing if the operation was successful or why it failed.</returns>
        /// <remarks>This method will also clear its aliases.</remarks>
        public async Task<IResult> RenameEffectAsync(ulong callerId, string effectName, string newName) =>
            await RenameEffectAsync(callerId, effectName, newName, 0, useCampaigns: false);

        /// <summary>
        /// Renames an already existing Effect associated with a campaign.
        /// </summary>
        /// <param name="callerId">The user identifier of the moderator or owner of the campaign.</param>
        /// <param name="effectName">The name of the Effect to rename.</param>
        /// <param name="newName">The new name of the Effect.</param>
        /// <param name="channelId">The channel the command was executed in to find the campaign.</param>
        /// <returns>A result detailing if the operation was successful or why it failed.</returns>
        /// <remarks>This method will also clear its aliases.</remarks>
        public async Task<IResult> RenameCampaignEffectAsync(ulong callerId, string effectName, string newName, ulong channelId) =>
            await RenameEffectAsync(callerId, effectName, newName, channelId, useCampaigns: true);

        private async Task<IResult> RenameEffectAsync(ulong callerId, string effectName, string newName, ulong channelId, bool useCampaigns)
        {
            Campaign campaign = null;

            if (useCampaigns)
            {
                campaign = await _campProvider.GetCampaignAsync(channelId);
                if (campaign == null) return CampaignResult.NotFoundByChannel();
                if (!await _campProvider.HasPermissionAsync(campaign, callerId)) return CampaignResult.AccessDenied();
            }

            var effect = await _effectProvider.GetEffectAsync(effectName, campaign);
            if (effect == null) return EffectResult.EffectNotFound();

            if (!useCampaigns && effect.Owner.UserIdentifier != callerId) return EffectResult.AccessDenied();

            if (await _effectProvider.GetEffectAsync(newName, campaign) != null)
                return EffectResult.NameAlreadyExists();

            effect.Name = newName;
            await _effectProvider.UpdateEffectAsync(effect);

            return EffectResult.EffectUpdatedSucessfully();
        }

        /// <summary>
        /// Sets an effect's description.
        /// </summary>
        /// <param name="callerId">The user identifier of the owner of the effect.</param>
        /// <param name="effectName">The name of the effect to set the description to.</param>
        /// <param name="desc">The new description of the effect.</param>
        /// <param name="channelId">The channel the command was executed in to find the campaign.</param>
        /// <returns>A result detailing if the operation was successful or why it failed.</returns>
        public async Task<IResult> SetEffectDescriptionAsync(ulong callerId, string effectName, string desc) =>
            await SetEffectDescriptionAsync(callerId, effectName, desc, 0, useCampaigns: false);

        /// <summary>
        /// Sets an effect's description that is associated with a campaign.
        /// </summary>
        /// <param name="callerId">The user identifier of the moderator or owner of the campaign.</param>
        /// <param name="effectName">The name of the effect to set the description to.</param>
        /// <param name="desc">The new description of the effect.</param>
        /// <param name="channelId">The channel the command was executed in to find the campaign.</param>
        /// <returns>A result detailing if the operation was successful or why it failed.</returns>
        public async Task<IResult> SetCampaignEffectDescriptionAsync(ulong callerId, string effectName, string desc, ulong channelId) =>
            await SetEffectDescriptionAsync(callerId, effectName, desc, channelId, useCampaigns: true);

        private async Task<IResult> SetEffectDescriptionAsync(ulong callerId, string effectName, string desc, ulong channelId, bool useCampaigns)
        {
            Campaign campaign = null;

            if (useCampaigns)
            {
                campaign = await _campProvider.GetCampaignAsync(channelId);
                if (campaign == null) return CampaignResult.NotFoundByChannel();
                if (!await _campProvider.HasPermissionAsync(campaign, callerId)) return CampaignResult.AccessDenied();
            }

            var effect = await _effectProvider.GetEffectAsync(effectName, campaign);
            if (effect == null) return EffectResult.EffectNotFound();

            if (!useCampaigns && effect.Owner.UserIdentifier != callerId) return EffectResult.AccessDenied();

            effect.Description = desc;
            await _effectProvider.UpdateEffectAsync(effect);

            return EffectResult.EffectUpdatedSucessfully();
        }

        /// <summary>
        /// Deletes a Effect from the database.
        /// </summary>
        /// <param name="callerId">The user identifier of the owner of the effect.</param>
        /// <param name="statName">The name of the effect to delete.</param>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> DeleteEffectAsync(ulong callerId, string statName) =>
            await DeleteEffectAsync(callerId, statName, 0, useCampaigns: false);

        /// <summary>
        /// Deletes a Effect from the database that is associated with a campaign.
        /// </summary>
        /// <param name="callerId">The user identifier of the moderator or owner of the campaign.</param>
        /// <param name="statName">The name of the effect to delete.</param>
        /// <param name="channelId">The channel the command was executed in to find the campaign.</param>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> DeleteCampaignEffectAsync(ulong callerId, string statName, ulong channelId) =>
            await DeleteEffectAsync(callerId, statName, channelId, useCampaigns: true);

        private async Task<IResult> DeleteEffectAsync(ulong callerId, string statName, ulong channelId, bool useCampaigns)
        {
            Campaign campaign = null;

            if (useCampaigns)
            {
                campaign = await _campProvider.GetCampaignAsync(channelId);
                if (campaign == null) return CampaignResult.NotFoundByChannel();
                if (!await _campProvider.HasPermissionAsync(campaign, callerId)) return CampaignResult.AccessDenied();
            }

            var effect = await _effectProvider.GetEffectAsync(statName, campaign);
            if (effect == null) return EffectResult.EffectNotFound();

            if (!useCampaigns && effect.Owner.UserIdentifier != callerId) return EffectResult.AccessDenied();

            await _effectProvider.DeleteEffectAsync(effect);
            return EffectResult.EffectDeletedSuccessfully();
        }

        /// <summary>
        /// Gets the caller's active character and returns its active effects.
        /// </summary> 
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <returns>A new EffectResult object.</returns>
        public async Task<IResult> ShowCharacterEffectsAsync(ulong callerId)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            if (character.Effects == null || character.Effects.Count <= 0)
                return EffectResult.CharacterHasNoEffectsApplied();

            return EffectResult.ShowCharacterEffects(character);
        }

        /// <summary>
        /// Used to add an effect assoicated with a campaign to a character.
        /// </summary>
        /// <param name="callerId">The user identifier of the caller.</param>
        /// <param name="effectName">The name of the effect to add to the character.</param>
        /// <param name="channelId">The optional identifier for the channel the command was executed in used to find the campaign.</param>
        public async Task<IResult> AddEffectAsync(ulong callerId, string effectName, ulong? channelId = null)
        {
            // Potentially we can find three different effects with the same name
            // 1. From the "default" pool of Effects which have a null Campaign
            // 2. From the current channel's associated campaign, if applicable
            // 3. From the character's campaign, if applicable

            // As it currently stands, this method will give priority as follows:
            // #2 then #1
            // ignore #3

            // TODO: write a unit test to guarantee this ^^^

            Campaign campaign;

            if (channelId.HasValue)
                campaign = await _campProvider.GetCampaignAsync(channelId.Value);
            else 
                campaign = null;

            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            // #2
            Effect effect = await _effectProvider.GetEffectAsync(effectName, campaign);

            bool failedToFindFromCampaign = false;
            if (effect == null)
            {
                // #1
                failedToFindFromCampaign = true;
                effect = await _effectProvider.GetEffectAsync(effectName, null);
                if (effect == null) return EffectResult.EffectNotFound();
            }

            if (character.Effects == null)
                character.Effects = new List<Effect>();

            if (character.Effects.Count(x => x.Equals(effect)) > 0)
                return EffectResult.EffectAlreadyAdded();

            character.Effects.Add(effect);
            await _charProvider.UpdateCharacterAsync(character);

            // Task failed successfully : ^)
            if (failedToFindFromCampaign) return EffectResult.EffectAddedFromDefaultPool();

            return EffectResult.EffectAdded();
        }

        /// <summary>
        /// Used to remove an effect from a character.
        /// </summary>
        /// <param name="callerId">The user identifier of the caller.</param>
        /// <param name="effectName">The name of the effect to remove from the character.</param>
        public async Task<IResult> RemoveEffectAsync(ulong callerId, string effectName)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            var match = character.Effects.FirstOrDefault(x => x.Name.EqualsIgnoreCase(effectName));

            if (match == null)
                return EffectResult.EffectNotFound();

            character.Effects.Remove(match);
            await _charProvider.UpdateCharacterAsync(character);

            return EffectResult.EffectRemoved();
        }
    }
}