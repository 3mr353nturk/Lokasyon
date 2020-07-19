using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless_Google_Geocoding_Api.Domain.Repositories
{
    public class BaseRepository
    {
        protected readonly LocationDBContext context;
        public BaseRepository(LocationDBContext context)
        {
            this.context = context;
        }
    }
}
