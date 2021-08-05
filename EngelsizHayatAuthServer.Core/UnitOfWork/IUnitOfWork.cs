using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task CommitAsync();

        void Commit();
    }
}