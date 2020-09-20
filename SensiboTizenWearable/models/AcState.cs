using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensiboTizenWearable.models
{
    [Newtonsoft.Json.JsonObject("result[0].acState")]
    public class AcState
    {
        public bool on { get; set; }
        public string mode { get; set; }
        public string fanLevel { get; set; }

        public int targetTemperature {get;set;}

        public string swing { get; set; }

    }
}
