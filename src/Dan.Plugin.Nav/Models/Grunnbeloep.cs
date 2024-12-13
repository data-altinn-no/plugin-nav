using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dan.Plugin.Nav.Models
{

    public class Grunnbeloep
    {
        public string dato { get; set; }
        public int grunnbeløp { get; set; }
        public int grunnbeløpPerMåned { get; set; }
        public int gjennomsnittPerÅr { get; set; }
        public float omregningsfaktor { get; set; }
        public string virkningstidspunktForMinsteinntekt { get; set; }
    }
}
