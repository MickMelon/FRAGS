using System;
using Frags.Core.Characters;
using Frags.Core.Common;

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

        public static EffectResult EffectAlreadyAdded() =>
            new EffectResult(Messages.EFFECT_ALREADY_ADDED, false);

        public static EffectResult NameAlreadyExists() =>
            new EffectResult(Messages.EFFECT_NAME_EXISTS, false);

        public static EffectResult EffectNotFound() =>
            new EffectResult(Messages.EFFECT_NOT_FOUND, false);

        public static EffectResult ShowCharacterEffects(Character character)
        {
            throw new NotImplementedException();
        }
    }
}