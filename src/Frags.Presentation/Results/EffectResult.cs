using System;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Effects;
using Frags.Presentation.ViewModels.Effects;

namespace Frags.Presentation.Results
{
    public class EffectResult : BaseResult
    {
        public EffectResult(string message, bool success = true, object viewModel = null) : base(message, success, viewModel)
        {
        }

        public static EffectResult EffectCreatedSuccessfully() =>
            new EffectResult(Messages.EFFECT_CREATE_SUCCESS, true);

        public static EffectResult EffectCreationFailed()  =>
            new EffectResult(Messages.EFFECT_CREATE_FAILURE, false);

        public static EffectResult EffectUpdatedSucessfully() =>
            new EffectResult(Messages.EFFECT_UPDATE_SUCCESS, true);

        public static EffectResult EffectDeletedSuccessfully() =>
            new EffectResult(Messages.EFFECT_DELETE_SUCCESS, true);

        public static EffectResult EffectAdded() =>
            new EffectResult(Messages.EFFECT_ADDED, true);

        public static EffectResult EffectRemoved() =>
            new EffectResult(Messages.EFFECT_REMOVED, true);

        public static EffectResult EffectAlreadyAdded() =>
            new EffectResult(Messages.EFFECT_ALREADY_ADDED, false);

        public static EffectResult NameAlreadyExists() =>
            new EffectResult(Messages.EFFECT_NAME_EXISTS, false);

        public static EffectResult EffectNotFound() =>
            new EffectResult(Messages.EFFECT_NOT_FOUND, false);

        public static EffectResult TooManyEffects() =>
            new EffectResult(Messages.EFFECT_TOO_MANY, false);

        public static EffectResult Show(Effect effect)
        {
            var viewModel = new ShowEffectViewModel(effect.Name, effect.Description, effect.Statistics);
            return new EffectResult(viewModel.Name, true, viewModel);
        }

        public static EffectResult ShowCharacterEffects(Character character)
        {
            var viewModel = new ShowCharacterEffectsViewModel();
            foreach (var effect in character.Effects)
            {
                // Take the ViewModel from Show() and put them in a list
                viewModel.Effects.Add((ShowEffectViewModel)Show(effect).ViewModel);
            }

            return new EffectResult(character.Name + "'s Effects", true, viewModel);
        }
    }
}