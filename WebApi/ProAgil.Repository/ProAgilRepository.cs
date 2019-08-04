using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;
using ProAgil.Repository.Model;

namespace ProAgil.Repository
{
    public class ProAgilRepository : IProAgilRepository
    {
        private readonly ProAgilContext context;

        public ProAgilRepository(ProAgilContext context)
        {
            this.context = context;
            this.context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public void Add<T>(T entity) where T : class
        {
            this.context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            this.context.Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            this.context.Update(entity);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await context.SaveChangesAsync()) > 0;
        }

        public async Task<Evento[]> GetAllEventosAsync(bool includePalestrantes = false)
        {
            IQueryable<Evento> query =  QueryBasicaEventos(includePalestrantes);

            return await query.ToArrayAsync();

        }

        private IQueryable<Evento> QueryBasicaEventos(bool includePalestrantes)
        {
            IQueryable<Evento> query = this.context.Eventos
                        .Include(c => c.Lotes)
                        .Include(c => c.RedesSociais);

            if (includePalestrantes)
            {
                query = query.Include(pe => pe.PalestranteEventos)
                    .ThenInclude(p => p.Palestrante);
            }

            query = query.OrderByDescending(c => c.DataEvento);
            return query;
        }

        public async Task<Evento[]> GetAllEventosAsyncByTema(string tema, bool includePalestrantes)
        {
            IQueryable<Evento> query =  QueryBasicaEventos(includePalestrantes);
            if (!string.IsNullOrWhiteSpace(tema)){
                query = query.Where(e => e.Tema.Contains(tema));
            }
            return await query.ToArrayAsync();
        }
        public async Task<Evento> GetEventoAsyncById(int eventoId, bool includePalestrantes)
        {
            IQueryable<Evento> query =  QueryBasicaEventos(includePalestrantes);
            
            query = query.Where(e => e.Id == eventoId);
            
            return await query.FirstOrDefaultAsync();
        }

        private IQueryable<Palestrante> QueryBasicaPalestrantes(bool includeEvento)
        {
            IQueryable<Palestrante> query = this.context.Palestrantes
                        .Include(c => c.RedesSociais);

            if (includeEvento)
            {
                query = query.Include(pe => pe.PalestranteEventos)
                    .ThenInclude(e => e.Evento);
            }

            query = query.OrderBy(c => c.Nome);
            return query;
        }
        public async Task<Palestrante> GetAllPalestranteAsync(int PalestranteId, bool includePalestrantes = false)
        {
            IQueryable<Palestrante> query =  QueryBasicaPalestrantes(includePalestrantes);
            
            query = query.Where(p => p.Id == PalestranteId);
            
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Palestrante[]> GetAllPalestrantesByName(bool includePalestrantes, string name)
        {
            IQueryable<Palestrante> query =  QueryBasicaPalestrantes(includePalestrantes);
            
            query = query.Where(p => p.Nome.ToLower().Contains(name.ToLower()));
            
            return await query.ToArrayAsync();
        }

       
    }
}