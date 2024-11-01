using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Models.Equipe
{
    public class EquipesInfos
    {
        public int EquipeID { get; set; }
        public string NomEquipe { get; set; }
        public string ChefEquipeNom { get; set; }
        public int NombreTotalDesPostes { get; set; }
    }
}
