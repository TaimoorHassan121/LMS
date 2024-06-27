using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace LMSProject.Services.Interface
{
    public interface IAuthInterface
    {

        Task<bool> CreateRole(string role);
    }
}
