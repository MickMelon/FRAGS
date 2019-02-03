using System.Threading.Tasks;
using Frags.Presentation.Results;

namespace Frags.Presentation.Controllers.Interfaces
{
    public interface ICharacterController
    {
        Task<IResult> ShowCharacterAsync(ulong callerId);
    }
}