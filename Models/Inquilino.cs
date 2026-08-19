using System;
using System.Collections.Generic;

namespace Inmobiliaria_BarrosoEsteban.Models;

public partial class Inquilino
{
    public int Idinquilino { get; set; }

    public string Dni { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Email { get; set; }
}
