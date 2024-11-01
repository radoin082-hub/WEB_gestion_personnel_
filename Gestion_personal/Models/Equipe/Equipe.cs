using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Models.Equipe
{
    public class Equipe
    {
        public int EquipeID { get; set; }
        public string NomEquipe { get; set; }
        public int ? ChefEquipeID { get; set; }
        public int Status { get; set; }
    }
}
