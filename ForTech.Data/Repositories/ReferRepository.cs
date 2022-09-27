using ForTech.Core;
using ForTech.Data.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.Data.Repositories
{
    public class ReferRepository : IReferRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public ReferRepository(ApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<Refer> AddRefer(Refer Refer)
        {
            var refer = await _dbContext.Refers.AddAsync(Refer);
            await _dbContext.SaveChangesAsync();
            return refer.Entity;
        }

        public async Task<List<Refer>> GetReceivedRefers(string ReceiverId)
        {
            var refers = await _dbContext.Refers.Where(x => x.ReceiverUserId == ReceiverId).ToListAsync();
            return refers;
        }

        public async Task<List<Refer>> GetSentRefers(string SentId)
        {
            var refers = await _dbContext.Refers.Where(x => x.SenderUserId == SentId).ToListAsync();
            return refers;
        }

        public async Task<bool> UpdateIsOpenedRefer(Guid ReferId)
        {
            var refer = await _dbContext.Refers.FirstOrDefaultAsync(x => x.Id == ReferId);
            if (refer == null)
            {
                return false;
            }
            var updatedRefer = new Refer()
            {
                Id = ReferId,
                SenderUserId = refer.SenderUserId,
                ReceiverUserId = refer.ReceiverUserId,
                ForumId = refer.ForumId,
                DateCreated = refer.DateCreated,
                IsReferOpened = true,
            };
            _dbContext.Refers.Update(updatedRefer);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
