using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Models.CoefficientTravail
{
    public class CoefficientTravail
    {
        public int CoefficientID { get; set; }
        public int EmployeID { get; set; }
        public DateTime Date { get; set; }
        public decimal JourneeCoefficient { get; set; } //
        public decimal HeuresSupplementairesCoefficient { get; set; }

    }
}
