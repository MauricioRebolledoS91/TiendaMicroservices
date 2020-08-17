using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaServicios.Api.Autor.Modelo;

namespace TiendaServicios.Api.Autor.Persistencia
{
    public class ContextoAutor: DbContext
    {
        //el constructor se define de esta forma
        //para que la conexiòn y el inicio de sesiòn hacia postgress sql, arranque en el momento
        //en que se ejecute la clase startup
        public ContextoAutor(DbContextOptions<ContextoAutor> options): base(options) { }

        public DbSet<AutorLibro> AutorLibro { get; set; }

        public DbSet<GradoAcademico> GradoAcademico { get; set; }
    }
}
