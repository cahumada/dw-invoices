using iaas.app.dw.invoices.Infrastructure.Support;

namespace iaas.app.visualtime.process.Infrastructure.Repositories
{
    public class BaseRepository
    {
        protected readonly DapperContext _context;

        public BaseRepository(DapperContext context)
        {
            _context = context;
        }
    }
}
