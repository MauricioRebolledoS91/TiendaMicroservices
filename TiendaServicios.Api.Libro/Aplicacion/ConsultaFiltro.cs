﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;

namespace TiendaServicios.Api.Libro.Aplicacion
{
    public class ConsultaFiltro
    {
        public class LibroUnico: IRequest<LibroMaterialDto>
        {
            public Guid? LibroId { get; set; }


        }

        public class Manejador : IRequestHandler<LibroUnico, LibroMaterialDto>
        {
            private readonly ContextoLibreria _contextoLibreria;
            private readonly IMapper _mapper;
            
            public Manejador(ContextoLibreria contextoLibreria, IMapper mapper)
            {
                _contextoLibreria = contextoLibreria;
                _mapper = mapper;
            }

            public async Task<LibroMaterialDto> Handle(LibroUnico request, CancellationToken cancellationToken)
            {
                var libro = await _contextoLibreria.LibreriaMaterial.Where(
                    x => x.LibreriaMaterialId == request.LibroId)
                    .FirstOrDefaultAsync();

                if(libro == null)
                {
                    throw new Exception("No se encontró el libro");
                }

                var libroDto = _mapper.Map<LibreriaMaterial, LibroMaterialDto>(libro);

                return libroDto;
            }
        }
    }
}