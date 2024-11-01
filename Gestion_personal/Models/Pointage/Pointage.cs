using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Models.Pointage
{

    public class Pointage
    {
        public int PointageID { get; set; }
        public int EmployeID { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan HeureEntree { get; set; }
        public TimeSpan HeureSortie { get; set; }
        public decimal HeuresTravaillees { get; set; }
        public string Remarque { get; set; }
        public string Stat => HeuresTravaillees > 1 ? "Présent" : "Absent";
        public int persontage => (int)Math.Floor((HeuresTravaillees / 8m) * 100);


    }
}
