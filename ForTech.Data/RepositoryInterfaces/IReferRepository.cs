using ForTech.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.Data.RepositoryInterfaces
{
    public interface IReferRepository
    {
        Task<Refer> AddRefer(Refer Refer);
        Task<List<Refer>> GetReceivedRefers(string ReceiverId);
        Task<List<Refer>> GetSentRefers(string SentId);
        Task<bool> UpdateIsOpenedRefer(Guid ReferId);
        Task<bool> RemoveRefer(Guid id);
    }
}
