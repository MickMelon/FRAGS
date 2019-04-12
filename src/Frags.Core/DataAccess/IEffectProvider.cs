using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Effects;

namespace Frags.Core.DataAccess
{
    public interface IEffectProvider
    {
        /// <summary>
        /// Adds a new effect to the database.
        /// Effect names should be unique.
        /// </summary>
        /// <param name="name">Effect's name.</param>
        /// <returns>The added effect if successful, null if not.</returns>
        Task<Effect> CreateEffectAsync(string name);

        /// <summary>
        /// Deletes an effect from the database. 
        /// This method should also remove any and all EffectMapping's that reference it.
        /// </summary>
        /// <param name="effect">The Effect to delete.</param>
        Task DeleteEffectAsync(Effect effect);

        /// <summary>
        /// Gets the Effect with the matching name.
        /// </summary>
        /// <param name="name">Effects's name.</param>
        /// <returns>The matching effect or null if none.</returns>
        Task<Effect> GetEffectAsync(string name);

        /// <summary>
        /// Gets every Effect currently in use.
        /// </summary>
        /// <returns>An Enumerable of Effects currently in use.</returns>
        Task<IEnumerable<Effect>> GetAllEffectsAsync();

        /// <summary>
        /// Updates a Effect in the database.
        /// </summary>
        /// <param name="Effect">The Effect to be saved.</param>
        Task UpdateEffectAsync(Effect Effect);
    }
}