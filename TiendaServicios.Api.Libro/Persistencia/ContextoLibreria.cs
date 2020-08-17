using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaServicios.Api.Libro.Modelo;

namespace TiendaServicios.Api.Libro.Persistencia
{
    public class ContextoLibreria: DbContext
    {
        //para que la sobre escritura de la propiedad virtual no nos genere error
        //también debemos crear un constructor por defecto
        public ContextoLibreria()
        {

        }
        public ContextoLibreria(DbContextOptions<ContextoLibreria> options): base(options)
        {

        }
        //el virtual permite que esta propiedad se pueda sobre escribir.
        //De esta manera, podemos sobre escribirla en los tests unitarios
        public virtual DbSet<LibreriaMaterial> LibreriaMaterial { get; set; }
    }
}
