using GestionPersonnel.Models.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Models.TypeDePaiment
{
    public class TypeDePaiement
    {
        private List<TypeDePaiement> typeDePaiements = new List<TypeDePaiement>();
        public int TypePaiementID { get; set; }
        public string NomTypePaiement { get; set; }
    }
}
