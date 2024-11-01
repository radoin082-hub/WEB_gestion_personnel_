using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Models.Salaires
{
    public class SalaireDetail
    {
        public string NomEmploye { get; set; }
        public string PrenomEmploye { get; set; }
        public string NomFonction { get; set; }
        public decimal Salaire { get; set; }
        public decimal Primes { get; set; }
        public decimal Avances { get; set; }
        public decimal Dettes { get; set; }
        public decimal SalaireNet { get; set; }
        public string TypePaiement { get; set; }
    }
}
