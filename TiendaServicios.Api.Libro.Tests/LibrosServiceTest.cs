using AutoMapper;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using GenFu;//para generar data de prueba
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiendaServicios.Api.Libro.Aplicacion;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;
using Xunit;

namespace TiendaServicios.Api.Libro.Tests
{
    public class LibrosServiceTest
    {
        //este método es para llenar con data de gemfu
        private IEnumerable<LibreriaMaterial> ObtenerDataPrueba()
        {
            A.Configure<LibreriaMaterial>()
                .Fill(x => x.Titulo).AsArticleTitle()
                .Fill(x => x.LibreriaMaterialId, () => { return Guid.NewGuid(); });

            var lista = A.ListOf<LibreriaMaterial>(30);
            lista[0].LibreriaMaterialId = Guid.Empty;

            return lista;
        }

        //este método es para crear el contexto de base de datos para los test
        private Mock<ContextoLibreria> CrearContexto()
        {
            var dataPrueba = ObtenerDataPrueba().AsQueryable();

            var dbSet = new Mock<DbSet<LibreriaMaterial>>();

            //al no tener una instancia real de sql server, debemos crear las propiedades de la instancia
            //manualmente(en pocas palabras, en las proòximas líneas, se convierte la clase LibreriaMaterial
            // a una entidad de entity framework).
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(X => X.Provider).Returns(dataPrueba.Provider);
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(X => X.Expression).Returns(dataPrueba.Expression);
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(X => X.ElementType).Returns(dataPrueba.ElementType);
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(X => X.GetEnumerator()).Returns(dataPrueba.GetEnumerator());

            //Aquí estamos realizando la configuración para mi entity framework instance local, que estoy
            //haciendo dentro del LibrosServiceTest
            dbSet.As<IAsyncEnumerable<LibreriaMaterial>>()
                .Setup(x => x.GetAsyncEnumerator(
                    new System.Threading.CancellationToken()))
                .Returns(new AsyncEnumerator<LibreriaMaterial>(dataPrueba.GetEnumerator()));

            //aquì estoy agregando el provider , para que sea posible
            //hacer los filtros hacia la entidad LibreriaMaterial. (Ej: hacer filtros por id)
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.Provider).
                Returns(new AsyncQueryProvider<LibreriaMaterial>(dataPrueba.Provider));

            var contexto = new Mock<ContextoLibreria>();
            contexto.Setup(x => x.LibreriaMaterial).Returns(dbSet.Object);
            return contexto;
        }

        //[Fact]//con este decorador, decimos que es un método de tipo testing
        //public async void GetLibroPorId()
        //{
        //    var mockContexto = CrearContexto();

        //    var mapConfig = new MapperConfiguration(cfg =>
        //    {
        //        cfg.AddProfile(new MappingTest());
        //    });

        //    var mapper = mapConfig.CreateMapper();

        //    var request = new ConsultaFiltro.LibroUnico();

        //    request.LibroId = Guid.Empty;

        //    var manejador = new ConsultaFiltro.Manejador(mockContexto.Object, mapper);

        //    var libro = await manejador.Handle(request, new System.Threading.CancellationToken());

        //    //esto significa que si al hacer la transacción
        //    //y nos devuelva un objeto instanciado correctamente
        //    //esto va a dar success o true y va a pasar el test.
        //    //pero si devuelve un nulo, no habrá pasado el test
        //    Assert.NotNull(libro);

        //    Assert.True(libro.LibreriaMaterialId == Guid.Empty);
        //}

        [Fact]//con este decorador, decimos que es un método de tipo testing
        public async void GetLibros()
        {
            //System.Diagnostics.Debugger.Launch();//para poder debuguear el test
            //revisamos qué método dentro de mi microservice libro se está encargando
            //de realizar la consulta de libros en la base de datos???
            //1. Emular a la instancia de entity framework core / ContextoLibreria
            //para emular las acciones y eventos de un objeto en un ambiente de unit test
            //utilizamos objetos de tipo Mock(un objeto que puede representar cualquier elemento de nuestro codigo).

            var mockContexto = CrearContexto();

            //2 emular el mapping IMapper
            var mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingTest());
            });

            var mapper = mapConfig.CreateMapper();

            //3. Instanciar a la clase Manejador y pasarle como parámetros los mocks que he creado
            Consulta.Manejador manejador = 
                new Consulta.Manejador(mockContexto.Object, mapper);

            Consulta.Ejecuta request = new Consulta.Ejecuta();

            var lista = await manejador.Handle(request, new System.Threading.CancellationToken());

            //Any me va a devolver true o false. Si hay algún valor en la lista , devuelve true y quiere decir
            //que el test pasò.
            //sino devuelve false y querrá decir que este test falló porque hay algùn error en mi lògica yd ebo revisar
            Assert.True(lista.Any());

        }

        [Fact]//con este decorador, decimos que es un método de tipo testing
        public async void GuardarLibro()
        {
            
            //este objeto me representa la configuración que va a tener
            //la base de datos en memoria
            var options = new DbContextOptionsBuilder<ContextoLibreria>()
                .UseInMemoryDatabase(databaseName: "BaseDatosLibro")
                .Options;

            var contexto = new ContextoLibreria(options);

            var request = new Nuevo.Ejecuta();
            request.Titulo = "Libro de Microservice";
            request.AutorLibro = Guid.Empty;
            request.FechaPublicacion = DateTime.Now;

            var manejador = new Nuevo.Manejador(contexto);

            var libro = await manejador.Handle(request, new System.Threading.CancellationToken());

            //si liobro es diferente de null, pasará el test con true
            Assert.True(libro != null);
                
        }
    }
}
