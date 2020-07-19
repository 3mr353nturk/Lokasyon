using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless_Google_Geocoding_Api.Domain.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LocationDBContext context;
        public UnitOfWork(LocationDBContext context)
        {
            this.context = context;
        }
        public void Complete()
        {
            this.context.SaveChanges();
        }

        public async Task CompleteAsync()
        {
            await this.context.SaveChangesAsync();
        }
    }
}
