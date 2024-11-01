using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Models.RapportPointage
{

    public class RapportPointage
    {
        public int RapportID { get; set; }
        public int EmployeID { get; set; }
        public int EquipeID { get; set; }
        public DateTime Mois { get; set; }
        public decimal HeuresTotales { get; set; }
        public decimal CofficientsTotales { get; set; }

    }
}
