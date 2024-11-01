using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Models.Salaires
{
    public class Salaire
    {
        public int SalaireID { get; set; }
        public int EmployeID { get; set; }
        public DateTime Mois { get; set; }
        public decimal Salairee { get; set; }
        public decimal Primes { get; set; }
        public decimal Avances { get; set; }
        public decimal Dettes { get; set; }
        public decimal SalaireNet { get; set; }
        public int TypePaiementID { get; set; }
    }
}
