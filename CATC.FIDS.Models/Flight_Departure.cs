//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CATC.FIDS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Flight_Departure
    {
        public int departureFlightId { get; set; }
        public System.DateTime scheduledTime { get; set; }
        public System.DateTime mostConfidentTime { get; set; }
        public int airlineID { get; set; }
        public int routeID { get; set; }
    }
}
