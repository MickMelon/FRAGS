using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Frags.Database.Characters;
using Frags.Database.Effects;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfCharacterProvider : ICharacterProvider
    {
        private readonly RpgContext _context;

        private readonly IMapper _mapper;

        public EfCharacterProvider(RpgContext context)
        {
            _context = context;
            
            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<Character, CharacterDto>();
                cfg.CreateMap<CharacterDto, Character>();
                cfg.CreateMap<Effect, EffectDto>();
                cfg.CreateMap<EffectDto, Effect>();
            });

            _mapper = new Mapper(mapperConfig);
        }

        private async Task<Character> CreateCharacterAsync(Character character)
        {
            var charDto = _mapper.Map<CharacterDto>(character);

            // Check the database for a character with the same ID as the new one
            // If one exists, don't add it
            if (await _context.Characters.CountAsync(x => x.Id.Equals(charDto.Id)) > 0)
                return null;

            await _context.AddAsync(charDto);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == character.UserIdentifier);

            if (user == null)
            {
                await _context.AddAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = charDto });
            }
            else if (character.Active)
            {
                user.ActiveCharacter = charDto;
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<Character>(charDto);
        }

        /// <inheritdoc/>
        public async Task<Character> CreateCharacterAsync(ulong userIdentifier, string name) =>
            await CreateCharacterAsync(new Character(userIdentifier, name));

        /// <inheritdoc/>
        public async Task<Character> CreateCharacterAsync(string id, ulong userIdentifier, bool active, string name,
            string description = "", string story = "") =>
            await CreateCharacterAsync(new Character(id, userIdentifier, active, name, description, story));

        /// <inheritdoc/>
        public async Task<Character> GetActiveCharacterAsync(ulong userIdentifier)
        {
            // hope you packed your holy water
            var character = await _context.Users.Where(c => c.UserIdentifier == userIdentifier)
                .Select(usr => usr.ActiveCharacter)
                .Include(_context.GetIncludePaths(typeof(CharacterDto)))
                .FirstOrDefaultAsync();
            
            if (character == null) return null;
            character.Active = true;

            var mapped = _mapper.Map<Character>(character);
            mapped.Effects = _mapper.Map<List<Effect>>(character.EffectMappings.Select(x => x.Effect).ToList());
            return mapped;
        }

        /// <inheritdoc/>
        public async Task<List<Character>> GetAllCharactersAsync(ulong userIdentifier)
        {
            var charDtos = await _context.Characters.Where(c => c.UserIdentifier == userIdentifier)
                .Include(_context.GetIncludePaths(typeof(CharacterDto)))
                .ToListAsync();

            if (charDtos == null) return null;

            charDtos.ForEach(x => x.Active = false);
            var activeChar = (await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == userIdentifier))?.ActiveCharacter;
            if (activeChar != null)
            {
                var match = charDtos.FirstOrDefault(x => x.Id.Equals(activeChar.Id));
                if (match != null)
                    match.Active = true;
            }
            // TODO: populate effects
            var mappedList = _mapper.Map<List<Character>>(charDtos);
            return mappedList;
        }

        /// <inheritdoc/>
        public async Task UpdateCharacterAsync(Character character)
        {
            // If the character does not exist in the database, abort
            if (await _context.Characters.CountAsync(c => c.Id.Equals(character.Id)) <= 0)
                return;
            
            var dbChar = await _context.Characters.Where(x => x.Id.Equals(character.Id))
                .Include(_context.GetIncludePaths(typeof(CharacterDto)))
                .FirstOrDefaultAsync();

            _mapper.Map<Character, CharacterDto>(character, dbChar);
            foreach (var effect in character.Effects)
            {
                if (dbChar.EffectMappings == null) 
                    dbChar.EffectMappings = new List<EffectMapping>();

                dbChar.EffectMappings.Clear();
                var effectDto = _mapper.Map<EffectDto>(effect);
                dbChar.EffectMappings.Add(new EffectMapping { Effect = effectDto, Character = dbChar });
            }

            _context.Update(dbChar);

            if (character.Active)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == character.UserIdentifier);
                
                if (user != null)
                {
                    user.ActiveCharacter = dbChar;
                    _context.Update(user);
                }
                else
                {
                    await _context.AddAsync(new User { UserIdentifier = character.UserIdentifier, ActiveCharacter = dbChar });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}