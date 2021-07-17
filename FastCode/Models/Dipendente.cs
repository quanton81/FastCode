using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FastCode.Models
{
    public class Dipendente
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Matricola { get; set; }
        [ForeignKey("Dipendente")]
        public int? Manager { get; set; }
    }
}
