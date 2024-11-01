using GestionPersonnel.Models.TypeDePaiment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Models.SalairesBase
{
    public class SalairesBase
    {
       
            public int IdSalaireBase { get; set; }
            public decimal SalaireBase { get; set; }
            public int TypePaiementID { get; set; }
           public int EmplyeId { get; set; }
        public TypeDePaiement TypePaiement { get; set; }
        


    }
}
