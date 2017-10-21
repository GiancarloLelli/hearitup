using System.Threading.Tasks;

namespace HIU.Core.Contract
{
    public interface INavigable
    {
        Task OnNavigateTo(object parameter);
        Task OnNavigateFrom(object parameter);
    }
}
