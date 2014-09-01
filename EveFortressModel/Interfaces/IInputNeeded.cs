using System.Threading.Tasks;

namespace EveFortressModel
{
    public interface IInputNeeded
    {
        Task<bool> ManageInput();
    }
}