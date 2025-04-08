using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrupoXpert.Blazor.Models
{
    public class Servicio
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }
        public string ImagenUrl { get; set; }
        public string UrlDestino { get; set; }
        public string ColorFondo { get; set; }
        public bool Destacado { get; set; }
        public string Categoria { get; set; }
    }
}
