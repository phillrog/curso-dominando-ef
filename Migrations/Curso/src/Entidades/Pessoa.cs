using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Meu.NameSpace
{
    public partial class Pessoa
    {
        [Key]
        public int Id { get; set; }
        public int Nome { get; set; }
    }
}
